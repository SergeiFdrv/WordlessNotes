using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Interfaces;
using Notes.Renderers;

namespace Notes.Views
{
    public abstract class CustomView : ContentView, ICustomView
    {
        public CustomView() : base()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            ParentResized += delegate { SetSize(); };
        }

        public int Index { get; set; }

        public MainPage ParentPage => ParentOfType<MainPage>();

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

        public event EventHandler ParentResized;

        public void Resize()
        {
            ParentResized(this, new EventArgs());
        }

        protected void SetSize()
        {
            TextBox.WidthRequest = DeviceDisplay.MainDisplayInfo.Width /
                                      DeviceDisplay.MainDisplayInfo.Density - 40;
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
            {
                TextBox.WidthRequest -= 100;
            }
            XButton.HeightRequest = XButton.FontSize * 2.5;
        }

        public abstract TextEditor TextBox { get; }

        public abstract Button XButton { get; }

        public string Text
        {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        public abstract string ToHTMLString();

        protected void Delete()
        {
            ParentPage?.DeleteElement(Index);
        }

        protected void Highlight()
        {
            if (ParentPage != null) ParentPage.SelectedView = this;
        }

        protected void TextChanged()
        {
            if (Text.Contains('\n') && ParentPage != null)
            {
                ParentPage.InsertElement(
                    new ParagraphView(Text.Substring(Text.LastIndexOf('\n') + 1)),
                    Index + 1);
                Text = Text.Substring(0, Text.LastIndexOf('\n'));
            }
        }
    }
}
