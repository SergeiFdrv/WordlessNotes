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
                    if (parent is MainPage)
                    {
                        return parent as MainPage;
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        public CustomListView ListV => List;

        public string ImgPath { get; set; }

        public int ImgID { get; set; }

        public Image ImageBox => Img;

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
                if (Img.IsVisible = value == CustomViewTypes.Image)
                {
                    Img.HeightRequest = 200;
                    TextEditor.FontSize = App.FontSize - 2;
                    TextEditor.Placeholder = "Image description";
                    TextEditor.TextColor = Color.Red;
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
                if (ParentPage != null)
                {
                    ParentPage.UnsavedData = true;
                }
            }
        }

        public string Text
        {
            get => TextEditor.Text;
            set => TextEditor.Text = value;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (ParentPage != null) ParentPage.DelEl(Index);
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Console.WriteLine("--- ELEMENT FOCUSED ---");
            if (ParentPage != null) ParentPage.Selected = this;
        }

        private void Image_Tapped(object sender, EventArgs e)
        {
            Console.WriteLine("--- IMAGE FOCUSED ---");
            if (!CrossMedia.Current.IsPickPhotoSupported) return;
            if (ParentPage != null)
            {
                ParentPage.Selected = this;
                ParentPage.UnsavedData = true;
            }
            var file = CrossMedia.Current.PickPhotoAsync();
            if (file == null) return;
            Img.Source = ImageSource.FromStream(() =>
            {
                Models.Image image = new Models.Image { Path = file.Result.Path };
                ImgPath = file.Result.Path;
                ImgID = App.Database.SaveImageAsync(image).Result;
                var stream = file.Result.GetStream();
                return stream;
            });
        }
    }
}