using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Approvals
{
    public class ApprovalHolder : ExtendedBindableObject
    {
        public ApprovalHolder()
        {
            EmployeeImage = string.Empty;
            EmployeeName = string.Empty;
            EmployeeNo = string.Empty;
            EmployeeDepartment = string.Empty;
            EmployeePosition = string.Empty;
            TransactionTypeId = 0;
            TransactionId = 0;
            ImageSource = null;
            IsSuccess = false;
            WorkflowActions = new ObservableCollection<WorkflowAction>();
            SelectedWorkflowAction = new WorkflowAction();
        }

        public string EmployeeImage { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeDepartment { get; set; }
        public string EmployeePosition { get; set; }

        //public string ImageSource { get; set; }
        public bool IsSuccess { get; set; }

        public long TransactionTypeId { get; set; }
        public long TransactionId { get; set; }
        public long StageId { get; set; }
        public string FormData { get; set; }
        public ObservableCollection<WorkflowAction> WorkflowActions { get; set; }
        public WorkflowAction SelectedWorkflowAction { get; set; }

        private Xamarin.Forms.ImageSource imageSource_;

        public Xamarin.Forms.ImageSource ImageSource
        {
            get { return imageSource_; }
            set { imageSource_ = value; RaisePropertyChanged(() => ImageSource); }
        }
    }
}