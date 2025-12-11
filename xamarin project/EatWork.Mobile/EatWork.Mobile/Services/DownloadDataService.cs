using EatWork.Mobile.Contracts;
using EatWork.Mobile.Utils;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EatWork.Mobile.Services
{
    public class DownloadDataService : IDownloadDataService
    {
        private HttpClient client_;
        private readonly IFileService fileService_;
        private int bufferSize = 4095;

        public DownloadDataService()
        {
            client_ = new HttpClient();
            fileService_ = DependencyService.Get<IFileService>();

            /*ADDED BY AGC 03.19.2020*/
            if (!string.IsNullOrEmpty(FormSession.TokenBearer))
            {
                client_.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", FormSession.TokenBearer);
            }
        }

        public async Task<string> DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token, string filename)
        {
            try
            {
                var filePath = string.Empty;

                await System.Threading.Tasks.Task.Delay(10);

                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                if (storageStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    storageStatus = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                }

                if (storageStatus == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var response = await client_.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
                    }

                    // Step 2 : Filename
                    var fileName = response.Content.Headers?.ContentDisposition?.FileName ?? filename;

                    // Step 3 : Get total of data
                    var totalData = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                    var canSendProgress = totalData != -1L && progress != null;

                    // Step 4 : Get total of data
                    filePath = Path.Combine(fileService_.GetStorageFolderPath(), fileName);

                    // Step 5 : Download data
                    using (var fileStream = OpenStream(filePath))
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var totalRead = 0L;
                            var buffer = new byte[bufferSize];
                            var isMoreDataToRead = true;

                            do
                            {
                                token.ThrowIfCancellationRequested();

                                var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                                if (read == 0)
                                {
                                    isMoreDataToRead = false;
                                }
                                else
                                {
                                    // Write data on disk.
                                    await fileStream.WriteAsync(buffer, 0, read);

                                    totalRead += read;

                                    if (canSendProgress)
                                    {
                                        progress.Report((totalRead * 1d) / (totalData * 1d) * 100);
                                    }
                                }
                            } while (isMoreDataToRead);
                        }
                    }
                }
                else
                {
                    throw new Exception("Please allow to access file storage.");
                }

                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Opens the stream.
        /// </summary>
        /// <returns>The stream.</returns>
        /// <param name="path">Path.</param>
        private Stream OpenStream(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize);
        }
    }
}