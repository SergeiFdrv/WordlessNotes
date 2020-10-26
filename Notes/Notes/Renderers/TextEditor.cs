using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Notes.Interfaces;

namespace Notes.Renderers
{
    public class TextEditor : Editor, ICustomView
    {
        public TextEditor ()
        {
            AutoSize = EditorAutoSizeOption.TextChanges;
            TextColor = Color.Black;
            FontSize = App.FontSize;
        }

        public event EventHandler ParentResized;

        public void Resize()
        {
            ParentResized(this, new EventArgs());
        }

        public T ParentOfType<T>() where T : VisualElement
        {
            var parent = base.Parent;
            while (parent != null)
            {
                if (parent is T) return parent as T;
                parent = parent.Parent;
            }
            return null;
        }
    }
}
