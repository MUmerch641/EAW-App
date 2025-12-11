using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contracts;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class StringHelper
    {
        public StringHelper()
        {
        }

        public string CreateUrl<T>(string builder, T obj)
        {
            /*
            var props = from p in obj.GetType().GetProperties()
                        where p.GetValue(obj, null) != null
                        select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
            */
            var props = from p in obj.GetType().GetProperties()
                        where p.GetValue(obj, null) != null
                        select p.Name + "=" + HttpUtility.UrlEncode
                        (
                            (p.GetValue(obj, null).ToString())
                        );

            return string.Format("{0}?{1}", builder.ToString(), String.Join("&", props.ToArray()));
        }

        public string StringBinder<T>(string str, T obj, string startTag = "", string endTag = "")
        {
            foreach (var key in obj.GetType().GetProperties())
            {
                str = str.Replace(startTag + key.Name + endTag, key.GetValue(obj, null).ToString());
            }

            return str;
        }
    }

    public static class FileHelper
    {
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    public class PermissionHelper
    {
        private readonly IDialogService dialogs_;

        public PermissionHelper()
        {
            dialogs_ = AppContainer.Resolve<IDialogService>();
        }

        public async Task<Plugin.Permissions.Abstractions.PermissionStatus> CheckPermissions(BasePermission permission, string permissionType)
        {
            var permissionStatus = await permission.CheckPermissionStatusAsync();
            bool request = false;

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await permission.RequestPermissionAsync();

                if (permissionStatus != PermissionStatus.Granted)
                {
                    var title = $"App Permission";
                    var question = $"Please enable {permissionType} permission.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }
                    return permissionStatus;
                }
            }

            if (permissionStatus == PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var title = $"App Permission";
                    var question = $"Please enable {permissionType} permission.";
                    var positive = "Settings";
                    var negative = "Maybe Later";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }

                    return permissionStatus;
                }

                request = true;
            }

            return permissionStatus;
        }
    }
}