using EatWork.Mobile.Models.DataObjects;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class DocumentRequestHolder : RequestHolder
    {
        public DocumentRequestHolder()
        {
            DocumentRequestModel = new DocumentRequestModel();
            DocumentType = new SelectableListModel();
            ErrorDetails = false;
            ErrorDocumentType = false;
            ErrorReason = false;
        }

        private SelectableListModel documentType_;

        public SelectableListModel DocumentType
        {
            get { return documentType_; }
            set { documentType_ = value; RaisePropertyChanged(() => DocumentType); }
        }

        /*
        private ComboBoxObject documentType_;

        public ComboBoxObject DocumentType
        {
            get { return documentType_; }
            set { documentType_ = value; RaisePropertyChanged(() => DocumentType); }
        }
        */

        private SelectableListModel reason_;

        public SelectableListModel Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private ObservableCollection<SelectableListModel> documentsList_;

        public ObservableCollection<SelectableListModel> DocumentsList
        {
            get { return documentsList_; }
            set { documentsList_ = value; RaisePropertyChanged(() => DocumentsList); }
        }

        private ObservableCollection<SelectableListModel> reasonList_;

        public ObservableCollection<SelectableListModel> ReasonList
        {
            get { return reasonList_; }
            set { reasonList_ = value; RaisePropertyChanged(() => ReasonList); }
        }

        private DocumentRequestModel model_;

        public DocumentRequestModel DocumentRequestModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => DocumentRequestModel); }
        }

        #region validators

        private bool errorDocumentType_;

        public bool ErrorDocumentType
        {
            get { return errorDocumentType_; }
            set { errorDocumentType_ = value; RaisePropertyChanged(() => ErrorDocumentType); }
        }

        private bool errorDetails_;

        public bool ErrorDetails
        {
            get { return errorDetails_; }
            set { errorDetails_ = value; RaisePropertyChanged(() => ErrorDetails); }
        }

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        #endregion validators
    }
}