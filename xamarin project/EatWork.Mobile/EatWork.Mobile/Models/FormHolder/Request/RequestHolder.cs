using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.ViewModels;
using Plugin.FilePicker.Abstractions;
using Syncfusion.SfNumericTextBox.XForms;
using Xamarin.Forms;

namespace EatWork.Mobile.Models.FormHolder.Request.OLD
{
    public class RequestHolder : BaseViewModel
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
        }

        private FileData fileData_;

        public FileData FileData
        {
            get { return fileData_; }
            set { fileData_ = value; OnPropertyChanged(nameof(FileData)); }
        }

        private FileUploadResponse fileUploadResponse;

        public FileUploadResponse FileUploadResponse
        {
            get { return fileUploadResponse; }
            set { fileUploadResponse = value; OnPropertyChanged(nameof(FileUploadResponse)); }
        }

        private bool success_;

        public bool Success
        {
            get { return success_; }
            set { success_ = value; OnPropertyChanged(nameof(Success)); }
        }

        private bool showCancelButton_;

        public bool ShowCancelButton
        {
            get { return showCancelButton_; }
            set { showCancelButton_ = value; OnPropertyChanged(nameof(ShowCancelButton)); }
        }

        private bool isEnabled_;

        public bool IsEnabled
        {
            get { return isEnabled_; }
            set { isEnabled_ = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        private long actionTypeId_;

        public long ActionTypeId
        {
            get { return actionTypeId_; }
            set { actionTypeId_ = value; OnPropertyChanged(nameof(ActionTypeId)); }
        }

        private string msg_;

        public string Msg
        {
            get { return msg_; }
            set { msg_ = value; OnPropertyChanged(nameof(Msg)); }
        }

        private double fontSize_;

        public double FontSize
        {
            get { return fontSize_; }
            set { fontSize_ = value; OnPropertyChanged(nameof(FontSize)); }
        }

        private bool continue_;

        public bool IsContinue
        {
            get { return continue_; }
            set { continue_ = value; OnPropertyChanged(nameof(IsContinue)); }
        }

        private bool proceed_;

        public bool Proceed
        {
            get { return proceed_; }
            set { proceed_ = value; OnPropertyChanged(nameof(Proceed)); }
        }
    }
}