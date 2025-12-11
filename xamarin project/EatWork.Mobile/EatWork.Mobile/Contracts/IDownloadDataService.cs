using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EatWork.Mobile.Contracts
{
    public interface IDownloadDataService
    {
        Task<string> DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token, string filename);
    }
}
