﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Interfaces;
using Notes.Renderers;

namespace Notes.Views
{
    public abstract class AbsCstmView : ContentView, ICustomView
    {
        public AbsCstmView() : base()
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

        protected static void Delete(AbsCstmView view)
        {
            view?.ParentPage?.DeleteElement(view.Index);
        }

        protected static void Highlight(AbsCstmView view)
        {
            if (view?.ParentPage != null) view.ParentPage.SelView = view;
        }

        protected static void TextChanged(AbsCstmView view)
        {
            if ((bool)(view?.Text.Contains('\n')) && view.ParentPage is ContentPage)
            {
                view.ParentPage.InsertElement(
                    new PrgView(view.Text.Substring(view.Text.LastIndexOf('\n') + 1)),
                    view.Index + 1);
                view.Text = view.Text.Substring(0, view.Text.LastIndexOf('\n'));
            }
        }
    }
}
