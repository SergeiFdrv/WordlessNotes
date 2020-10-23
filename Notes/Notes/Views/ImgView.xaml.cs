using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Renderers;
using Plugin.Media;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImgView : AbsCstmView
    {
        public ImgView()
        {
            InitializeComponent();
            SetSize();
            ImageBox.WidthRequest = TextBox.WidthRequest;
            TextBox.FontSize -= 4;
        }

        public override TextEditor TextBox => TextEditor;
        public override Button XButton => XBtn;
        public Models.Image Image { get; set; }
        public Image ImageBox => Img;

        public override string ToHTMLString() => $"<img src=\"img/{Image.Name}\"/><br><p class=\"imgdesc\">{Text}</p><br>";

        private void Button_Clicked(object sender, EventArgs e)
        {
            Delete(this);
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Highlight(this);
            ImgBtn.IsVisible = true;
        }

        private void Item_Unfocused(object sender, FocusEventArgs e)
        {
            ImgBtn.IsVisible = false;
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(this);
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (ParentPage != null)
            {
                ParentPage.SelView = this;
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
    }
}