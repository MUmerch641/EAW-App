using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Utils.DataAccess;
using EatWork.Mobile.Views.Shared;
using Plugin.Connectivity;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class CommonDataService : ICommonDataService
    {
        private readonly ClientSetupDataAccess clientSetupDataAccess_;
        private readonly ThemeDataAccess themeDataAccess_;
        private readonly IGenericRepository genericRepository_;
        private readonly PermissionHelper permissionHelper_;
        private readonly IDialogService dialogService_;
        private readonly INavigationService navigationService_;

        public CommonDataService(ClientSetupDataAccess clientSetupDataAccess,
            IGenericRepository genericRepository)
        {
            clientSetupDataAccess_ = clientSetupDataAccess;
            genericRepository_ = genericRepository;
            themeDataAccess_ = AppContainer.Resolve<ThemeDataAccess>();
            permissionHelper_ = AppContainer.Resolve<PermissionHelper>();
            dialogService_ = AppContainer.Resolve<IDialogService>();
            navigationService_ = AppContainer.Resolve<INavigationService>();
        }

        [Obsolete("Please use TakePhotoAsync instead.")]
        public async Task<MediaFile> TakePhoto(string prefixname = "")
        {
            MediaFile file = null;
            prefixname = (string.IsNullOrWhiteSpace(prefixname) ? "EAW" : prefixname);
            await CrossMedia.Current.Initialize();
            try
            {
                if (CrossMedia.Current.IsCameraAvailable || CrossMedia.Current.IsTakePhotoSupported)
                {
                    var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
                    var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                    if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        /*
                        storageStatus = await permissionHelper_.CheckPermissions(new CameraPermission(), "camera");
                        */
                        cameraStatus = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                    }

                    if (storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        storageStatus = await permissionHelper_.CheckPermissions(new StoragePermission(), "storage/file");
                    }

                    if (cameraStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted && storageStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            Directory = "EAW",
                            Name = string.Format("{0}_{1}.jpg", prefixname, DateTime.Now.ToString(Constants.DateFormatMMDDYYYYHHMMTT.Replace(" ", ""))),//"EAW.jpg",
                            PhotoSize = PhotoSize.Small,
                            SaveToAlbum = true
                        });
                    }
                    else if (cameraStatus == Plugin.Permissions.Abstractions.PermissionStatus.Unknown && storageStatus == Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                    {
                        throw new Exception("Camera permission denied.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return file;
        }

        public async Task<ObservableCollection<FileAttachmentListModel>> RetrieveFileAttachments(FileAttachmentParams param)
        {
            var retValue = new ObservableCollection<FileAttachmentListModel>();

            try
            {
                if (param?.FileAttachments?.Count > 0)
                {
                    retValue = new ObservableCollection<FileAttachmentListModel>(
                        param.FileAttachments.Select(x => new FileAttachmentListModel()
                        {
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileAttachmentId = x.FileAttachmentId,
                            TransactionId = x.TransactionId,
                        })
                    );

                    return retValue;
                }

                var url = await RetrieveClientUrl();
                await HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetFileAttachments}{param.ModuleFormId}/{param.TransactionId}"
                };

                var response = await genericRepository_.GetAsync<R.Responses.FileAttachmentResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    foreach (var item in response.FileAttachmentList)
                    {
                        var model = new FileAttachmentListModel();
                        PropertyCopier<R.Models.FileAttachmentList, FileAttachmentListModel>.Copy(item, model);
                        retValue.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<bool> HasInternetConnection(string url = "")
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            //Do this only if you need to and aren't listening to any other events as they will not fire.
            var connectivity = CrossConnectivity.Current;
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (!connectivity.IsConnected)
                        throw new Exception(Messages.NOINTERNETCONNECTION);

                    var link = url.Split(':')[1].Replace("/", "");
                    var port = url.Split(':')[2].Replace("/", "");

                    var reachable = await connectivity.IsRemoteReachable(link, Convert.ToInt32(port));

                    if (!reachable)
                        throw new Exception(Messages.UNREACHABLEHOST);

                    return reachable;
                }

                return connectivity.IsConnected;
            }
            finally
            {
                CrossConnectivity.Dispose();
            }
        }

        public async Task<string> RetrieveClientUrl()
        {
            var url = (await clientSetupDataAccess_.RetrieveClientSetup()).APILink.Decrypt();

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Unable to connect: NO URL FOUND");

            return url;
        }

        public async Task<bool> SaveSingleFileAttachmentAsync(FileData file, long moduleFormId, long transactionId)
        {
            bool retValue;
            try
            {
                var url = await RetrieveClientUrl();
                await HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.SaveFileAttachment
                };

                var data = new R.Requests.FileAttachmentRequest()
                {
                    FileName = file.FileName,
                    ModuleFormId = moduleFormId,
                    TransactionId = transactionId,
                    FileAttachment = Convert.ToBase64String(file.DataArray),
                    /*FileSize = string.Format("{0}", (file.DataArray.Length / 1024))*/
                    FileSize = string.Format("{0}", (file.DataArray.Length))
                };

                var response = await genericRepository_.PostAsync<R.Requests.FileAttachmentRequest, R.Responses.FileAttachmentRequestResponse>(builder.ToString(), data);

                if (!response.IsSuccess)
                    throw new Exception(response.ErrorMessage);

                retValue = response.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<FileData> AttachPhoto(MediaFile photo, string filePrefix = "")
        {
            var retValue = new FileData();
            try
            {
                if (photo != null)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var name = string.Format("{0}_{1}.jpg", filePrefix, DateTime.Now.ToString(Constants.DateFormatMMDDYYYYHHMMTT.Replace(" ", "").Replace("/", "")));
                        name = name.Replace(":", "");
                        retValue = new FileData(photo.Path, name, photo.GetStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<FileData> FileUpload()
        {
            FileData file = new FileData();

            try
            {
                var permission = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                if (permission != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    /*//permission = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();*/
                    permission = await permissionHelper_.CheckPermissions(new StoragePermission(), "storage/file");
                }

                if (permission == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var mimetypes = Newtonsoft.Json.JsonConvert.DeserializeObject<MimeType>(FormSession.MimeTypes);

                    if (mimetypes != null)
                    {
                        string[] fileTypes = null;

                        if (Device.RuntimePlatform == Device.Android)
                            fileTypes = mimetypes.MimeTypes.Android.Select(p => p.Type).ToArray();

                        if (Device.RuntimePlatform == Device.iOS)
                            fileTypes = mimetypes.MimeTypes.iOS.Select(p => p.Type).ToArray();

                        if (Device.RuntimePlatform == Device.UWP)
                            fileTypes = mimetypes.MimeTypes.UWP.Select(p => p.Type).ToArray();

                        file = await CrossFilePicker.Current.PickFile(fileTypes);
                    }
                    else
                    {
                        file = await CrossFilePicker.Current.PickFile();
                    }

                    if (file != null)
                    {
                        //==TEST FILE INFO
                        var fileName = file.FileName;
                        var fileExt = file.FilePath;
                        var filesize = file.DataArray.Length;
                        var filelength = filesize / 1024;
                        var strfilesize = string.Empty;

                        if (PreferenceHelper.MaxFileSize() > 0)
                        {
                            if (filelength > PreferenceHelper.MaxFileSize())
                            {
                                var filemb = PreferenceHelper.MaxFileSize() / 1000;
                                throw new Exception($"File must be less than {filemb} mb.");
                            }
                        }

                        strfilesize = Convert.ToString(filelength) + "KB";
                        /*if (filelength == 1024)*/
                        if (filelength >= 1024)
                        {
                            filelength = filelength / 1024;
                            strfilesize = Convert.ToString(filelength) + "MB";
                        }
                    }
                }
                else if (permission != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                {
                    throw new Exception("Storage permission denied.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
            }

            return file;
        }

        //FILE PICKER PLUGIN WILL NO LONGER SUPPORT UPDATES IN FUTURE
        public async Task<FileUploadResponse> AttachPhoto2(MediaFile photo, string filePrefix = "")
        {
            var retValue = new FileUploadResponse();

            try
            {
                if (photo != null)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var name = string.Format("{0}_{1}.jpg", filePrefix, DateTime.Now.ToString(Constants.DateFormatMMDDYYYYHHMMTT.Replace(" ", "").Replace("/", "")));
                        name = name.Replace(":", "");
                        retValue = await FileUpload2(photo.Path);
                        retValue.FileData = new FileData(photo.Path, name, photo.GetStream);

                        /*
                        retValue.FileResult = new Xamarin.Essentials.FileResult(photo.Path);

                        if (retValue.FileResult != null)
                        {
                            var stream = await retValue.FileResult.OpenReadAsync();
                            var byteArray = stream.ReadFully();
                            var fileSize = byteArray.Length;
                            var filelength = fileSize / 1024;

                            var strfilesize = Convert.ToString(filelength) + "KB";
                            if (filelength == 1024)
                            {
                                filelength = filelength / 1024;
                                strfilesize = Convert.ToString(filelength) + "MB";
                            }

                            retValue.FileName = retValue.FileResult.FileName;
                            retValue.FileSize = strfilesize;
                            retValue.MimeType = retValue.FileResult.ContentType;
                            retValue.FileDataArray = byteArray;

                            var filetype = retValue.FileResult.ContentType.Split('/')[0];
                            retValue.FileType = (filetype == "image" ? filetype : "file");
                        }
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<FileUploadResponse> FileUpload2(string path = "")
        {
            var retValue = new FileUploadResponse();

            try
            {
                var permission = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                if (permission != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    /*permission = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();*/

                    permission = await permissionHelper_.CheckPermissions(new StoragePermission(), "storage/file");
                }

                if (permission == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    Xamarin.Essentials.FileResult file = null;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        file = new Xamarin.Essentials.FileResult(path);
                    }
                    else
                    {
                        var mimetypes = Newtonsoft.Json.JsonConvert.DeserializeObject<MimeType>(FormSession.MimeTypes);
                        if (mimetypes != null)
                        {
                            var options = new PickOptions
                            {
                                FileTypes = new FilePickerFileType(
                                new Dictionary<DevicePlatform, IEnumerable<string>>
                                {
                                    { DevicePlatform.Android, mimetypes.MimeTypes.Android.Select(p => p.Type) },
                                    { DevicePlatform.iOS, mimetypes.MimeTypes.iOS.Select(p => p.Type) },
                                    { DevicePlatform.UWP, mimetypes.MimeTypes.UWP.Select(p => p.Type) },
                                }),
                                PickerTitle = "Select a file to import"
                            };

                            file = await Xamarin.Essentials.FilePicker.PickAsync(options);
                        }
                        else
                            file = await Xamarin.Essentials.FilePicker.PickAsync();
                    }

                    if (file != null)
                    {
                        var stream = await file.OpenReadAsync();
                        var byteArray = stream.ReadFully();
                        var fileSize = byteArray.Length;
                        var rawFileSize = fileSize / 1024;
                        var filelength = fileSize / 1024;
                        /*var filelength = fileSize / 1000;*/

                        if (PreferenceHelper.MaxFileSize() > 0)
                        {
                            if (filelength > PreferenceHelper.MaxFileSize())
                            {
                                var filemb = PreferenceHelper.MaxFileSize() / 1000;
                                throw new Exception($"File must be less than {filemb} mb.");
                            }
                        }

                        var strfilesize = Convert.ToString(filelength) + "KB";
                        if (filelength == 1024)
                        /*if (filelength >= 1000)*/
                        {
                            filelength = filelength / 1024;
                            /*filelength = filelength / 1000;*/
                            strfilesize = Convert.ToString(filelength) + "MB";
                        }

                        retValue.RawFileSize = rawFileSize;
                        retValue.FileResult = file;
                        retValue.FileName = file.FileName;
                        retValue.MimeType = file.ContentType;
                        retValue.FileSize = strfilesize;
                        retValue.FileDataArray = byteArray;
                        retValue.Base64String = Convert.ToBase64String(byteArray);

                        var filetype = file.ContentType.Split('/')[0];
                        retValue.FileType = (filetype == "image" ? filetype : "file");
                    }
                }
                else if (permission != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
                {
                    throw new Exception("Storage permission denied.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<bool> ApplyThemeConfig()
        {
            var retValue = false;

            try
            {
                var theme = await themeDataAccess_.RetrieveThemeSetup();

                if (theme != null)
                {
                    if (!string.IsNullOrWhiteSpace(theme.PrimaryColor))
                        Application.Current.Resources["PrimaryColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryDarkColor))
                        Application.Current.Resources["PrimaryDarkColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryDarkColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryDarkenColor))
                        Application.Current.Resources["PrimaryDarkenColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryDarkenColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryLighterColor))
                        Application.Current.Resources["PrimaryLighterColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryLighterColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryGradient))
                        Application.Current.Resources["PrimaryGradient"] = Xamarin.Forms.Color.FromHex(theme.PrimaryGradient);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryLight))
                        Application.Current.Resources["PrimaryLight"] = Xamarin.Forms.Color.FromHex(theme.PrimaryLight);

                    if (!string.IsNullOrWhiteSpace(theme.SecondaryGradient))
                        Application.Current.Resources["SecondaryGradient"] = Xamarin.Forms.Color.FromHex(theme.SecondaryGradient);

                    if (!string.IsNullOrWhiteSpace(theme.Secondary))
                        Application.Current.Resources["Secondary"] = Xamarin.Forms.Color.FromHex(theme.Secondary);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryButtonColor))
                        Application.Current.Resources["PrimaryButtonColor"] = Xamarin.Forms.Color.FromHex(theme.PrimaryButtonColor);

                    if (!string.IsNullOrWhiteSpace(theme.PrimaryButtonGradient))
                        Application.Current.Resources["PrimaryButtonGradient"] = Xamarin.Forms.Color.FromHex(theme.PrimaryButtonGradient);

                    if (!string.IsNullOrWhiteSpace(theme.LoginGradientStart))
                        Application.Current.Resources["LoginGradientStart"] = Xamarin.Forms.Color.FromHex(theme.LoginGradientStart);

                    if (!string.IsNullOrWhiteSpace(theme.LoginGradientEnd))
                        Application.Current.Resources["LoginGradientEnd"] = Xamarin.Forms.Color.FromHex(theme.LoginGradientEnd);

                    retValue = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task PreviewFileBase64(string base64, string type, string filename)
        {
            MemoryStream stream = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(base64) && base64.Contains("base64"))
                {
                    var attachment = base64.Split(';')[1];
                    var sourceString = attachment.Replace("base64,", "");

                    using (UserDialogs.Instance.Loading())
                    {
                        ImageSource source = null;
                        await Task.Delay(500);

                        if (type == FileType.Image)
                        {
                            stream = new MemoryStream(Convert.FromBase64String(sourceString));
                            source = ImageSource.FromStream(() => stream);

                            /*
                            source = ImageSource.FromStream(
                                () => new MemoryStream(Convert.FromBase64String(sourceString)));
                            */

                            await navigationService_.PushModalAsync(new PreviewImagePage(source));
                        }
                        else
                        {
                            var bytes = Convert.FromBase64String(sourceString);
                            var file = Path.Combine(FileSystem.CacheDirectory, filename);
                            File.WriteAllBytes(file, bytes);

                            await Launcher.OpenAsync(new OpenFileRequest
                            {
                                File = new ReadOnlyFile(file),
                                Title = "Open with"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name + " : " + ex.Message}");
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        public async Task<bool> SaveFileAttachmentsAsync(List<FileAttachmentParamsDto> param)
        {
            try
            {
                var url = await RetrieveClientUrl();
                await HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.SaveFileAttachment + "/multiple"
                };

                var data = new R.Requests.MultipleFileAttachmentRequest();

                foreach (var item in param)
                {
                    data.FileAttachments.Add(new R.Requests.FileAttachmentRequest()
                    {
                        FileName = item.FileName,
                        ModuleFormId = item.ModuleFormId,
                        TransactionId = item.TransactionId,
                        FileAttachment = Convert.ToBase64String(item.FileDataArray),
                        /*FileSize = string.Format("{0}", (file.DataArray.Length / 1024))*/
                        FileSize = string.Format("{0}", (item.FileDataArray.Length))
                    });
                }

                var response = await genericRepository_.PostAsync<R.Requests.MultipleFileAttachmentRequest, R.Responses.FileAttachmentRequestResponse>(builder.ToString(), data);

                if (!response.IsSuccess)
                {
                    var colletion = new ObservableCollection<string>(response.ValidationMessages);
                    var page = new ErrorPage(colletion, $"{Messages.ValidationHeaderMessage.Replace(".", "")} WHILE SAVING FILE ATTACHMENT");
                    await navigationService_.PushModalAsync(page);
                }

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }
        }

        public async Task OpenFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath),
                        Title = "Open with"
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw new Exception($"Unable to open file : {ex.Message}");
            }
        }

        public async Task<FileUploadResponse> TakePhotoAsync(string prefixname = "", bool attachFile = false)
        {
            try
            {
                if (!MediaPicker.IsCaptureSupported)
                    return null;

                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (!cameraStatus.Equals(PermissionStatus.Granted))
                    cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();

                if (cameraStatus.Equals(PermissionStatus.Granted))
                {
                    var captured = await MediaPicker.CapturePhotoAsync();

                    if (captured == null)
                        return null;

                    if (!attachFile)
                    {
                        using (var stream = await captured.OpenReadAsync())
                        {
                            var arr = stream.ReadFully();

                            var resizer = DependencyService.Get<IImageResizer>();
                            var byteArray = resizer.ScaleImage(arr, 300, 300, true);

                            var extension = Path.GetExtension(captured.FileName);
                            var fileSize = byteArray.Length;
                            var rawFileSize = fileSize / 1024;
                            var filelength = fileSize / 1024;

                            var strfilesize = Convert.ToString(filelength) + "KB";
                            if (filelength == 1024)
                            /*if (filelength >= 1000)*/
                            {
                                filelength = filelength / 1024;
                                /*filelength = filelength / 1000;*/
                                strfilesize = Convert.ToString(filelength) + "MB";
                            }

                            var filetype = captured.ContentType.Split('/')[0];

                            return new FileUploadResponse()
                            {
                                RawFileSize = rawFileSize,
                                FileResult = captured,
                                FileName = captured.FileName,
                                MimeType = captured.ContentType,
                                FileSize = strfilesize,
                                FileDataArray = byteArray,
                                Base64String = Convert.ToBase64String(byteArray),
                                FileType = (filetype == "image" ? filetype : "file"),
                            };
                        }
                    }
                    else
                        return await FileUploadAsync(captured.FullPath);
                }

                return null;
            }
            catch (FeatureNotSupportedException ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            catch (PermissionException ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        public async Task<FileUploadResponse> FileUploadAsync(string path = "")
        {
            try
            {
                Xamarin.Essentials.FileResult file = null;

                if (!string.IsNullOrWhiteSpace(path))
                {
                    file = new Xamarin.Essentials.FileResult(path);
                }
                else
                {
                    var mimetypes = Newtonsoft.Json.JsonConvert.DeserializeObject<MimeType>(FormSession.MimeTypes);
                    if (mimetypes != null)
                    {
                        var options = new PickOptions
                        {
                            FileTypes = new FilePickerFileType(
                            new Dictionary<DevicePlatform, IEnumerable<string>>
                            {
                                { DevicePlatform.Android, mimetypes.MimeTypes.Android.Select(p => p.Type) },
                                { DevicePlatform.iOS, mimetypes.MimeTypes.iOS.Select(p => p.Type) },
                                { DevicePlatform.UWP, mimetypes.MimeTypes.UWP.Select(p => p.Type) },
                            }),
                            PickerTitle = "Select a file to import"
                        };

                        file = await Xamarin.Essentials.FilePicker.PickAsync(options);
                    }
                    else
                        file = await Xamarin.Essentials.FilePicker.PickAsync();

                    if (file == null)
                        return null;

                    using (var stream = await file.OpenReadAsync())
                    {
                        var mem = new MemoryStream();
                        await stream.CopyToAsync(mem);
                        var byteArray = mem.ReadFully();
                        var extension = Path.GetExtension(file.FileName);
                        var fileSize = byteArray.Length;
                        var rawFileSize = fileSize / 1024;
                        var filelength = fileSize / 1024;

                        if (PreferenceHelper.MaxFileSize() > 0)
                        {
                            if (filelength > PreferenceHelper.MaxFileSize())
                            {
                                var filemb = PreferenceHelper.MaxFileSize() / 1000;
                                throw new Exception($"File must be less than {filemb} mb.");
                            }
                        }

                        var strfilesize = Convert.ToString(filelength) + "KB";
                        if (filelength == 1024)
                        /*if (filelength >= 1000)*/
                        {
                            filelength = filelength / 1024;
                            /*filelength = filelength / 1000;*/
                            strfilesize = Convert.ToString(filelength) + "MB";
                        }

                        var filetype = file.ContentType.Split('/')[0];

                        return new FileUploadResponse()
                        {
                            RawFileSize = rawFileSize,
                            FileResult = file,
                            FileName = file.FileName,
                            MimeType = file.ContentType,
                            FileSize = strfilesize,
                            FileDataArray = mem.ToArray(),
                            Base64String = Convert.ToBase64String(mem.ToArray()),
                            FileType = (filetype == "image" ? filetype : "file"),
                        };
                    }
                }

                return null;
            }
            catch (Xamarin.Essentials.FeatureNotSupportedException)
            {
                throw;
            }
            catch (Xamarin.Essentials.PermissionException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}