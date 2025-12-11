namespace EatWork.Mobile.Models
{
    public class LeaveRequestDocumentModel
    {
        public LeaveRequestDocumentModel()
        {
            IsSaved = 1;
            ShowCommand = true;
        }

        public long LeaveRequestDocumentId { get; set; }
        public long cmbLeaveTypeDocumentId { get; set; }
        public long LeaveRequestId { get; set; }
        public string FileName { get; set; }
        public string FileBytes { get; set; }
        public string FileType { get; set; }
        public short IsSaved { get; set; }
        public string DocumentName { get; set; }
        public long LeaveRequestHeaderId { get; set; }
        public bool ShowCommand { get; set; }
    }
}