using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Media;
using Xamarin.Essentials;

using Notes.Resources;

namespace Notes.Views
{
    public enum CustomViewType
    {
        Paragraph = 0,
        Header1 = 1,
        Header2 = 2,
        Header3 = 3,
        List = 4,
        Image = 5
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomView : ContentView
    {
        public CustomView(int index, CustomViewType type = CustomViewType.Paragraph, string text = "")
        {
            InitializeComponent();
            Index = index;
            ViewType = type;
            Text = text;
            ListMark.FontSize = App.FontSize;
        }

        public MainPage ParentPage
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent is MainPage) return parent as MainPage;
                    parent = parent.Parent;
                }
                return null;
            }
        }

        // Image
        public Models.Image Image { get; set; }
        public Image ImageBox => Img;

        // Общее
        public int Index { get; set; }
        private CustomViewType _ViewType;
        public CustomViewType ViewType
        {
            get
            {
                return _ViewType;
            }
            set
            {
                _ViewType = value;
                TextEditor.WidthRequest = DeviceDisplay.MainDisplayInfo.Width * 0.9 /
                                          DeviceDisplay.MainDisplayInfo.Density;
                TextEditor.TextColor = Color.Black;
                if (Img.IsVisible = ImgBtn.IsVisible = value == CustomViewType.Image)
                {
                    TextEditor.FontSize = App.FontSize - 2;
                    TextEditor.Placeholder = Lang.ImageDescription;
                }
                if (ListMark.IsVisible = value == CustomViewType.List)
                {
                    TextEditor.WidthRequest -= ListMark.Width + 25;
                    TextEditor.FontSize = App.FontSize;
                    TextEditor.Placeholder = Lang.ListItem;
                }
                else if (value == CustomViewType.Header1)
                {
                    TextEditor.FontSize = App.FontSize + 6;
                    TextEditor.Placeholder = $"{Lang.Header} 1";
                }
                else if (value == CustomViewType.Header2)
                {
                    TextEditor.FontSize = App.FontSize + 4;
                    TextEditor.Placeholder = $"{Lang.Header} 2";
                }
                else if (value == CustomViewType.Header3)
                {
                    TextEditor.FontSize = App.FontSize + 2;
                    TextEditor.Placeholder = $"{Lang.Header} 3";
                }
                else if (value == CustomViewType.Paragraph)
                {
                    TextEditor.FontSize = App.FontSize;
                    TextEditor.Placeholder = Lang.Paragraph;
                }
                if (ParentPage != null) ParentPage.UnsavedData = true;
            }
        }

        public string Text
        {
            get => TextEditor.Text;
            set => TextEditor.Text = value;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (ParentPage != null) ParentPage.DeleteElement(Index);
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            if (ParentPage != null) ParentPage.SelectedView = this;
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported) return;
            if (ParentPage != null)
            {
                ParentPage.SelectedView = this;
                ParentPage.UnsavedData = true;
            }
            var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
            if (file == null) return;
            Image = new Models.Image { Path = file.Path, Name = file.Path.Substring(file.Path.LastIndexOf('/') + 1) };
            await App.Database.SaveImageAsync(Image).ConfigureAwait(true);
            Img.Source = ImageSource.FromFile(Image.Path);
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text.Contains('\n'))
            {
                ParentPage.AddElement(Index, ViewType, Text.Substring(0, Text.LastIndexOf('\n')));
                Text = Text.Substring(Text.LastIndexOf('\n') + 1);
            }
        }
    }
}