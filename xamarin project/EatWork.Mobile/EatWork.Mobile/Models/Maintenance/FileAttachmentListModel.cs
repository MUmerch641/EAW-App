using System.Collections.Generic;

namespace EatWork.Mobile.Models
{
    public class FileAttachmentListModel
    {
        public long FileAttachmentId { get; set; }
        public long? ModuleFormId { get; set; }
        public long? TransactionId { get; set; }
        public string Attachment { get; set; }
        public string FileName { get; set; }
        public string FileTags { get; set; }
        public string FileSize { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedDate { get; set; }
        public string FileUpload { get; set; }
        public string FileType { get; set; }
    }

    public class FileAttachmentParams
    {
        public long ModuleFormId { get; set; }

        public long TransactionId { get; set; }

        public string TransactionIds { get; set; }

        public string FileTags { get; set; }
        public List<FileAttachmentListModel> FileAttachments { get; set; }
        public string URI { get; set; }
    }

    public class FileAttachmentParamsDto : FileAttachmentParams
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public int RawFileSize { get; set; }
        public byte[] FileDataArray { get; set; }
    }
}