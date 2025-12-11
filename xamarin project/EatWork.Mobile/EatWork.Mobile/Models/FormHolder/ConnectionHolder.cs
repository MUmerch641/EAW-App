using EatWork.Mobile.Models.DataAccess;
using EatWork.Mobile.ViewModels;

namespace EatWork.Mobile.Models.FormHolder
{
    public class ConnectionHolder : BaseViewModel
    {
        public ConnectionHolder()
        {
            ClientSetupModel = new ClientSetupModel();
            Success = false;
            ErrorClientCode = false;
            ErrorPassKey = false;

            ErrorClientCodeMessage = "Client Code is required";
        }

        private ClientSetupModel model;

        public ClientSetupModel ClientSetupModel
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(() => ClientSetupModel);}
        }

        private bool success_;

        public bool Success
        {
            get { return success_; }
            set { success_ = value; RaisePropertyChanged(() => Success);}
        }

        private bool errorClientCode_;

        public bool ErrorClientCode
        {
            get { return errorClientCode_; }
            set { errorClientCode_ = value; RaisePropertyChanged(() => ErrorClientCode);}
        }

        private string errorClientCodeMessage_;

        public string ErrorClientCodeMessage
        {
            get { return errorClientCodeMessage_; }
            set { errorClientCodeMessage_ = value; RaisePropertyChanged(() => ErrorClientCodeMessage);}
        }

        private bool errorPassKey_;

        public bool ErrorPassKey
        {
            get { return errorPassKey_; }
            set { errorPassKey_ = value; RaisePropertyChanged(() => ErrorPassKey);}
        }
    }
}