using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using Plugin.FilePicker.Abstractions;
using Syncfusion.SfNumericTextBox.XForms;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace EatWork.Mobile.Models.FormHolder
{
    public class RequestHolder : ExtendedBindableObject
    {
        public RequestHolder()
        {
            FileData = new FileData();
            Success = false;
            ShowCancelButton = false;
            IsEnabled = false;
            FontSize = Device.GetNamedSize(NamedSize.Small, typeof(SfNumericTextBox));
            Proceed = false;
            FileUploadResponse = new FileUploadResponse();
            FileAttachments = new ObservableCollection<FileUploadResponse>();
            SelectedFile = new FileUploadResponse();
            IsEditable = true;
        }

        private FileData fileData_;

        public FileData FileData
        {
            get { return fileData_; }
            set { fileData_ = value; RaisePropertyChanged(() => FileData); }
        }

        private FileUploadResponse fileUploadResponse;

        public FileUploadResponse FileUploadResponse
        {
            get { return fileUploadResponse; }
            set { fileUploadResponse = value; RaisePropertyChanged(() => FileUploadResponse); }
        }

        private bool success_;

        public bool Success
        {
            get { return success_; }
            set { success_ = value; RaisePropertyChanged(() => Success); }
        }

        private bool showCancelButton_;

        public bool ShowCancelButton
        {
            get { return showCancelButton_; }
            set { showCancelButton_ = value; RaisePropertyChanged(() => ShowCancelButton); }
        }

        private bool isEnabled_;

        public bool IsEnabled
        {
            get { return isEnabled_; }
            set { isEnabled_ = value; RaisePropertyChanged(() => IsEnabled); }
        }

        private long actionTypeId_;

        public long ActionTypeId
        {
            get { return actionTypeId_; }
            set { actionTypeId_ = value; RaisePropertyChanged(() => ActionTypeId); }
        }

        private string msg_;

        public string Msg
        {
            get { return msg_; }
            set { msg_ = value; RaisePropertyChanged(() => Msg); }
        }

        private double fontSize_;

        public double FontSize
        {
            get { return fontSize_; }
            set { fontSize_ = value; RaisePropertyChanged(() => FontSize); }
        }

        private bool continue_;

        public bool IsContinue
        {
            get { return continue_; }
            set { continue_ = value; RaisePropertyChanged(() => IsContinue); }
        }

        private bool proceed_;

        public bool Proceed
        {
            get { return proceed_; }
            set { proceed_ = value; RaisePropertyChanged(() => Proceed); }
        }

        private ObservableCollection<string> msgs_;

        public ObservableCollection<string> Message
        {
            get { return msgs_; }
            set { RaisePropertyChanged(() => Proceed); }
        }

        private ObservableCollection<FileUploadResponse> attachments_;

        public ObservableCollection<FileUploadResponse> FileAttachments
        {
            get { return attachments_; }
            set { attachments_ = value; RaisePropertyChanged(() => FileAttachments); }
        }

        private FileUploadResponse selectedFile_;

        public FileUploadResponse SelectedFile
        {
            get { return selectedFile_; }
            set { selectedFile_ = value; RaisePropertyChanged(() => SelectedFile); }
        }

        private bool isEditable_;

        public bool IsEditable
        {
            get { return isEditable_; }
            set { isEditable_ = value; RaisePropertyChanged(() => IsEditable); }
        }
    }
}