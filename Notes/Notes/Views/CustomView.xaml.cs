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

namespace Notes.Views
{
    public enum CustomViewTypes
    {
        Header1 = 0,
        Header2 = 1,
        Header3 = 2,
        Paragraph = 3,
        List = 4,
        Image = 5
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomView : ContentView
    {
        public CustomView(int index, CustomViewTypes type = CustomViewTypes.Paragraph)
        {
            InitializeComponent();
            Index = index;
            Type = type;
            (Content as Grid).ColumnDefinitions[0].Width = DeviceDisplay.MainDisplayInfo.Width * 0.9 / DeviceDisplay.MainDisplayInfo.Density;
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

        // List
        public CustomListView ListV => List;

        // Image
        public Models.Image Image { get; set; }
        public Image ImageBox => Img;

        // Общее
        public int Index { get; set; }
        private CustomViewTypes ViewType;
        public CustomViewTypes Type
        {
            get
            {
                return ViewType;
            }
            set
            {
                ViewType = value;
                List.IsVisible = value == CustomViewTypes.List;
                TextEditor.IsVisible = !(List.IsVisible = value == CustomViewTypes.List);
                TextEditor.TextColor = Color.Black;
                if (Img.IsVisible = ImgBtn.IsVisible = value == CustomViewTypes.Image)
                {
                    TextEditor.FontSize = App.FontSize - 2;
                    TextEditor.Placeholder = "Image description";
                }
                else if (List.IsVisible && !(TextEditor.Text == string.Empty || List.StackL.Children.Any()))
                {
                    List.StackL.Children.Add(new CustomListViewCell(TextEditor.Text));
                }
                else if (value == CustomViewTypes.Header1)
                {
                    TextEditor.FontSize = App.FontSize + 6;
                    TextEditor.Placeholder = "Header 1";
                }
                else if (value == CustomViewTypes.Header2)
                {
                    TextEditor.FontSize = App.FontSize + 4;
                    TextEditor.Placeholder = "Header 2";
                }
                else if (value == CustomViewTypes.Header3)
                {
                    TextEditor.FontSize = App.FontSize + 2;
                    TextEditor.Placeholder = "Header 3";
                }
                else
                {
                    TextEditor.FontSize = App.FontSize;
                    TextEditor.Placeholder = "Paragraph";
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
            if (ParentPage != null) ParentPage.Selected = this;
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported) return;
            if (ParentPage != null)
            {
                ParentPage.Selected = this;
                ParentPage.UnsavedData = true;
            }
            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null) return;
            Image = new Models.Image { Path = file.Path, Name = file.Path.Substring(file.Path.LastIndexOf('/') + 1) };
            await App.Database.SaveImageAsync(Image);
            Img.Source = ImageSource.FromFile(Image.Path);
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text.Contains('\n'))
            {
                CustomView customView = new CustomView(Index, Type)
                {
                    Text = Text.Substring(0, Text.LastIndexOf('\n'))
                };
                ParentPage.AddElement(Index, customView);
                Text = Text.Substring(Text.LastIndexOf('\n') + 1);
            }
        }
    }
}