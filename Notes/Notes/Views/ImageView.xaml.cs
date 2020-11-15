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
    public partial class ImageView : CustomView
    {
        public ImageView()
        {
            InitializeComponent();
            SetSize();
            ImageBox.WidthRequest = TextBox.WidthRequest;
            TextBox.FontSize -= 4;
            ParentResized += delegate
            {
                ImageBox.WidthRequest = TextBox.WidthRequest;
            };
        }

        public override TextEditor TextBox => TextEditor;
        public override Button XButton => XBtn;
        public Models.Image Image { get; set; }
        public ImageButton ImageBox => Img;

        public override string ToHTMLString() => $"<img src=\"img/{Image.Name}\"/><br><p class=\"imgdesc\">{Text}</p><br>";

        private void Button_Clicked(object sender, EventArgs e)
        {
            Delete();
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Highlight();
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void Image_SizeChanged(object sender, EventArgs e)
        {
            ImgBtn.IsVisible = ImageBox.Source is null;
        }

        private async void ImgBtn_Clicked(object sender, EventArgs e)
        {
            TextBox.Focus();
            if (ParentPage != null)
            {
                ParentPage.SelectedView = this;
                ParentPage.UnsavedData = true;
            }
            List<string> options = new List<string>();
            if (CrossMedia.Current.IsPickPhotoSupported) options.Add("Memory");
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported) options.Add("Camera");
            if (options.Count == 0) return;
            string choice = options.Count == 1 ? options[0] : await
                ParentPage.DisplayActionSheet("Where from?", "Cancel", null, options.ToArray()).ConfigureAwait(true);
            Plugin.Media.Abstractions.MediaFile file =
                choice == "Memory" ?
                    await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true) :
                choice == "Camera" ?
                    await CrossMedia.Current
                    .TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions())
                    .ConfigureAwait(true) : null;
            if (file == null) return;
            Image = new Models.Image { Path = file.Path, Name = file.Path.Substring(file.Path.LastIndexOf('/') + 1) };
            await App.Database.SaveImageAsync(Image).ConfigureAwait(true);
            Img.Source = ImageSource.FromFile(Image.Path);
        }
    }
}