using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EatWork.Mobile.Contracts
{
    public interface ICommonDataService
    {
        [Obsolete("Please use TakePhotoAsync instead.")]
        Task<MediaFile> TakePhoto(string prefixname = "");

        Task<FileData> FileUpload();

        [Obsolete("Use FileUploadAsync instead.")]
        Task<FileUploadResponse> FileUpload2(string path = "");

        Task<FileData> AttachPhoto(MediaFile photo, string filePrefix = "");

        [Obsolete("FILE PICKER PLUGIN WILL NO LONGER SUPPORT UPDATES IN FUTURE")]
        Task<FileUploadResponse> AttachPhoto2(MediaFile photo, string filePrefix = "");

        Task<ObservableCollection<FileAttachmentListModel>> RetrieveFileAttachments(FileAttachmentParams param);

        Task<bool> SaveSingleFileAttachmentAsync(FileData file, long moduleFormId, long transactionId);

        Task<bool> SaveFileAttachmentsAsync(List<FileAttachmentParamsDto> param);

        Task<bool> HasInternetConnection(string url = "");

        Task<string> RetrieveClientUrl();

        Task<bool> ApplyThemeConfig();

        Task PreviewFileBase64(string base64, string type, string filename);

        Task OpenFile(string filePath);

        Task<FileUploadResponse> TakePhotoAsync(string prefixname = "", bool attachFile = false);

        Task<FileUploadResponse> FileUploadAsync(string path = "");
    }
}