using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidFileService))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class AndroidFileService : IFileService
    {
        public string GetStorageFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}