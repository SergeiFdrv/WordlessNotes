using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Notes.Renderers
{
    public class TextEditor : Editor
    {
        public TextEditor ()
        {
            AutoSize = EditorAutoSizeOption.TextChanges;
            HorizontalOptions = LayoutOptions.StartAndExpand;
            BackgroundColor = Color.GhostWhite;
            TextColor = Color.Black;
        }
    }
}
