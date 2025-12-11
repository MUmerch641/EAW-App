using EatWork.Mobile.Contracts;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(EatWork.Mobile.Droid.Helpers.ReadWriteStoragePermission))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class ReadWriteStoragePermission : Xamarin.Essentials.Permissions.BasePlatformPermission, IReadWritePermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.ReadExternalStorage, true),
            (Android.Manifest.Permission.WriteExternalStorage, true)
        }.ToArray();
    }
}