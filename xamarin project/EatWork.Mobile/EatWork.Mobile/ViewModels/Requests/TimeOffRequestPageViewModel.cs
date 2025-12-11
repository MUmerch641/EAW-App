using EatWork.Mobile.Contracts;
using EatWork.Mobile.Views.Shared;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EatWork.Mobile.ViewModels
{
    public class TimeOffRequestPageViewModel : BaseViewModel
    {
        #region commands

        public ICommand SubmitCommand { get; set; }
        public ICommand CameraCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CloseCommand { get; set; }


        #endregion commands

        public INavigation Navigation { get; set; }

        private readonly ITimeOffRequestPageDataService timeOffRequestPageDataService_;
        private readonly IMyRequestCommonDataService myRequestCommonDataService_;

        public TimeOffRequestPageViewModel(ITimeOffRequestPageDataService timeOffRequestPageDataService, IMyRequestCommonDataService myRequestCommonDataService)
        {
            timeOffRequestPageDataService_ = timeOffRequestPageDataService;
            myRequestCommonDataService_ = myRequestCommonDataService;
        }

        public void Init(INavigation navigation)
        {
            Navigation = navigation;

            //==commands
            SubmitCommand = new Command(async () => await SubmitRequest());
            CameraCommand = new Command(async () => await TakePhoto());
            SelectFileCommand = new Command(async () => await SelectFile());
            CloseCommand = new Command(async () => await CloseCurrentPage());
        }

        private async Task SubmitRequest()
        {
            try
            {
                using (Dialogs.Loading())
                    await timeOffRequestPageDataService_.SubmitRequest();

                //==if success
                await Navigation.PushModalAsync(new SuccessPage());
                await Task.Delay(2000);
                await Navigation.PopModalAsync();
                await Navigation.PopToRootAsync(true);
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
            finally
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private async Task TakePhoto()
        {
            try
            {
                await myRequestCommonDataService_.TakePhoto();
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
        }

        private async Task SelectFile()
        {
            try
            {
                await myRequestCommonDataService_.FileUpload();
            }
            catch (Exception ex)
            {
                await Dialogs.AlertAsync(ex.Message, "", "Close");
            }
        }

        private async Task CloseCurrentPage()
        {
            await Navigation.PopToRootAsync(true);
            //await Navigation.PopAsync(true);
        }
    }
}
