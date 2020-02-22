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
            Img.WidthRequest = DeviceDisplay.MainDisplayInfo.Width * 0.9 /
                               DeviceDisplay.MainDisplayInfo.Density;
            ListMark.WidthRequest = ListMark.HeightRequest = App.FontSize / 2;
            ListMark.Margin = new Thickness(10, App.FontSize * 5 / 6, 0, 0);
        }

        public string ToHTMLString()
        {
            if (ViewType == CustomViewType.Header1) return $"<h1>{Text}</h1><br>";
            else if (ViewType == CustomViewType.Header2) return $"<h2>{Text}</h2><br>";
            else if (ViewType == CustomViewType.Header3) return $"<h3>{Text}</h3><br>";
            else if (ViewType == CustomViewType.Image && Image != null)
                return $"<img src=\"img/{Image.Name}\"/><br><p class=\"imgdesc\">{Text}</p><br>";
            else if (ViewType == CustomViewType.Paragraph) return $"<p>{Text}</p><br>";
            return string.Empty;
        }

        #region Properties
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
                    TextEditor.WidthRequest = DeviceDisplay.MainDisplayInfo.Width * 0.9 /
                                              DeviceDisplay.MainDisplayInfo.Density - 25;
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
        #endregion

        private void Button_Clicked(object sender, EventArgs e)
        {
            ParentPage?.DeleteElement(Index);
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            if (ParentPage != null) ParentPage.SelectedView = this;
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (ParentPage != null)
            {
                ParentPage.SelectedView = this;
                ParentPage.UnsavedData = true;
            }
            List<string> options = new List<string>();
            if (CrossMedia.Current.IsPickPhotoSupported) options.Add("Memory");
            //if (CrossMedia.Current.IsTakePhotoSupported) options.Add("Camera");
            if (options.Count == 0) return;
            string choice = options.Count == 1 ? options[0] : await
                ParentPage.DisplayActionSheet("Where from?", "Cancel", null, options.ToArray()).ConfigureAwait(true);
            Plugin.Media.Abstractions.MediaFile file;
            if (choice == "Memory")
            {
                file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
                if (file == null) return;
                Image = new Models.Image { Path = file.Path, Name = file.Path.Substring(file.Path.LastIndexOf('/') + 1) };
            }
            /*else if (choice == "Camera")
            {
                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    //SaveToAlbum = true
                    Directory = "/img",
                    Name = $"local{DateTime.UtcNow}.jpg"
                }).ConfigureAwait(true);
                Image = new Models.Image { Path = file.Path, Name = file.Path.Substring(file.Path.LastIndexOf('/') + 1) };
            }*/
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