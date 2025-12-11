using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSFileService))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class iOSFileService : IFileService
    {
        public string GetStorageFolderPath()
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library");
            return libFolder;
        }
    }
}