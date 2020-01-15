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
            BackgroundColor = Color.GhostWhite;
            TextColor = Color.Black;
            TextChanged += TextChange;
            FontSize = App.FontSize;
        }

        public MainPage ParentPage
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent is MainPage)
                    {
                        return parent as MainPage;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        private void TextChange(object sender, TextChangedEventArgs e)
        {
            if (ParentPage != null) ParentPage.UnsavedData = true;
        }
    }
}
