using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class FileAttachmentViewModel : BaseViewModel
    {
        #region commands

        public ICommand DownloadCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand DeleteFileCommand { get; set; }

        #endregion commands

        private ObservableCollection<FileAttachmentListModel> fileAttachmentList_;

        public ObservableCollection<FileAttachmentListModel> FileAttachmentList
        {
            get { return fileAttachmentList_; }
            set { fileAttachmentList_ = value; RaisePropertyChanged(() => FileAttachmentList); }
        }

        private double progressValue_;

        public double ProgressValue
        {
            get { return progressValue_; }
            set { progressValue_ = value; RaisePropertyChanged(() => ProgressValue); }
        }

        private bool isDownloading_;

        public bool IsDownloading
        {
            get { return isDownloading_; }
            set { isDownloading_ = value; RaisePropertyChanged(() => IsDownloading); }
        }

        private readonly ICommonDataService commonDataService_;
        private readonly IDownloadDataService downloadService_;

        public FileAttachmentViewModel(ICommonDataService commonDataService,
            IDownloadDataService downloadService)
        {
            commonDataService_ = commonDataService;
            downloadService_ = downloadService;
        }

        public void Init(INavigation navigation, FileAttachmentParams param)
        {
            NavigationBack = navigation;
            CloseCommand = new Command(async () => await NavigationService.PopModalAsync());
            DownloadCommand = new Command(DownloadFile);
            DeleteFileCommand = new Command<FileAttachmentListModel>(DeleteFile);
            RetrieveFileAttachments(param);
        }

        private async void RetrieveFileAttachments(FileAttachmentParams param)
        {
            try
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    await Task.Delay(500);
                    FileAttachmentList = await commonDataService_.RetrieveFileAttachments(param);
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void DownloadFile(object obj)
        {
            try
            {
                if (obj != null)
                {
                    /*var eventArgs = obj as Syncfusion.ListView.XForms.ItemTappedEventArgs;*/
                    /* var item = (obj as FileAttachmentListModel);*/

                    if (!(obj is FileAttachmentListModel item))
                        return;

                    if (item == null) return;

                    if (!string.IsNullOrWhiteSpace(item.Attachment) && item.Attachment.Contains("base64"))
                    {
                        await commonDataService_.PreviewFileBase64(item.Attachment, "", item.FileName);
                        return;
                    }

                    using (Dialogs.Loading("Downloading file.."))
                    {
                        await Task.Delay(500);

                        IsDownloading = true;

                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.FileAttachmentApi}/{item.FileAttachmentId}/download-file"
                        };

                        var progressIndicator = new Progress<double>(ReportProgress);
                        var cts = new CancellationTokenSource();

                        var response = await downloadService_.DownloadFileAsync(builder.ToString(), progressIndicator, cts.Token, item.FileName);
                        await commonDataService_.OpenFile(response);
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Error(false, ex.Message);
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
            finally
            {
                IsDownloading = false;
            }
        }

        private async void DeleteFile(FileAttachmentListModel item)
        {
            try
            {
                if (item != null)
                {
                    using (Dialogs.Loading("Deleting file.."))
                    {
                        await Task.Delay(500);
                        FileAttachmentList.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        internal void ReportProgress(double value)
        {
            ProgressValue = value;
        }
    }
}