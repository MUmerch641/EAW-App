namespace EatWork.Mobile.Models
{
    public class FileAttachmentModel
    {
        public string FileAttachment { get; set; }
        public string FileName { get; set; }
        public long ModuleFormId { get; set; }
        public long TransactionId { get; set; }
        public string FileTags { get; set; }
        public string FileSize { get; set; }
    }
}