using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Notes.Droid;
using Notes.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformSpecific))]
namespace Notes.Droid
{
    public class PlatformSpecific : IPlatformSpecific
    {
        public string GetDocsDirectory()
        {
            string path = System.IO.Path.Combine
                (Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments);
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            return path;
        }

        public string GetAppFilesDirectory()
        {
            Context context = Android.App.Application.Context;
            string path = context.GetExternalFilesDir(null).AbsolutePath;
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            return path;
        }

        public void SayLong(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
            });
        }

        public void SayShort(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
            });
        }
    }
}