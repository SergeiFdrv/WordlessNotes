using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Notes.Interfaces
{
    public interface ICustomView
    {
        string Text { get; set; }

        event EventHandler ParentResized;

        T ParentOfType<T>() where T : VisualElement;
    }
}
