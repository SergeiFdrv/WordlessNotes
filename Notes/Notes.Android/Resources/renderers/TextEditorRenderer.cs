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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Notes.Renderers;

[assembly: ExportRenderer(typeof(TextEditor), typeof(Notes.Droid.Resources.renderers.TextEditorRenderer))]
namespace Notes.Droid.Resources.renderers
{
    public class TextEditorRenderer : EditorRenderer
    {
        public TextEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetBackgroundColor(global::Android.Graphics.Color.GhostWhite);
                Control.SetTextColor(global::Android.Graphics.Color.Black);
            }
        }
    }
}