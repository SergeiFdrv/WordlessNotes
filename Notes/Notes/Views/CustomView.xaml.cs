using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    public enum CustomViewTypes
    {
        Paragraph = 0,
        Image = 1
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomView : ContentView
    {
        public CustomView()
        {
            InitializeComponent();
            Type = CustomViewTypes.Paragraph;
            Renderers.TextEditor TXT = new Renderers.TextEditor
            {
                WidthRequest = 270
            };
            (Content as FlexLayout).Children.Insert(0, TXT);
        }

        public CustomView(int index, CustomViewTypes type = CustomViewTypes.Paragraph)
        {
            InitializeComponent();
            Index = index;
            Type = type;
            if (type == CustomViewTypes.Paragraph)
            {
                Renderers.TextEditor TXT = new Renderers.TextEditor
                {
                    WidthRequest = 270
                };
                (Content as FlexLayout).Children.Insert(0, TXT);
            }
            else if (type == CustomViewTypes.Image)
            {
                Image image = new Image
                {
                    WidthRequest = 270
                };
                (Content as FlexLayout).Children.Insert(0, image);
            }
        }

        public int Index { get; set; }

        public string Text
        {
            get {
                if (Type == CustomViewTypes.Paragraph)
                {
                    return ((Content as FlexLayout).Children.First() as CustomView).Text;
                }
                else if (Type == CustomViewTypes.Image)
                {
                    return "image";
                }
                return string.Empty;
            }
            set {
                if (Type == CustomViewTypes.Paragraph)
                {
                    ((Content as FlexLayout).Children.First() as CustomView).Text = value;
                }
            }
        }

        public CustomViewTypes Type { get; set; }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (Application.Current.MainPage as MainPage).DelEl(Index);
        }
    }
}