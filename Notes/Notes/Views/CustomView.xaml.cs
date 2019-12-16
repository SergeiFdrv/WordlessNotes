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
        Header1 = 0,
        Header2 = 1,
        Header3 = 2,
        Paragraph = 3,
        Image = 4
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomView : ContentView
    {
        public CustomView(int index, CustomViewTypes type = CustomViewTypes.Paragraph)
        {
            InitializeComponent();
            Index = index;
            Type = type;
            if (type == CustomViewTypes.Header1)
            {
                View = new Renderers.TextEditor
                {
                    FontSize = 20,
                    Placeholder = "Header 1",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Header2)
            {
                View = new Renderers.TextEditor
                {
                    FontSize = 18,
                    Placeholder = "Header 2",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Header3)
            {
                View = new Renderers.TextEditor
                {
                    FontSize = 16,
                    Placeholder = "Header 3",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Paragraph)
            {
                View = new Renderers.TextEditor
                {
                    Placeholder = "Paragraph",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Image)
            {
                View = new Image();
            }
            View.WidthRequest = 270;
            (Content as FlexLayout).Children.Insert(0, View);
        }

        public int Index { get; set; }

        public View View { get; set; }

        public CustomViewTypes Type
        {
            get => Type;
            set
            {
                Type = value;
                (View as Renderers.TextEditor).FontSize = 20 - 2 * (int)value;
            }
        }

        public string Text
        {
            get {
                if (Type == CustomViewTypes.Image)
                {
                    return "image";
                }
                return ((Content as FlexLayout).Children.First() as CustomView).Text;
            }
            set {
                if (Type == CustomViewTypes.Paragraph)
                {
                    ((Content as FlexLayout).Children.First() as CustomView).Text = value;
                }
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            (Application.Current.MainPage as MainPage).DelEl(Index);
        }

        private void FlexLayout_Focused(object sender, FocusEventArgs e)
        {
            (Application.Current.MainPage as MainPage).Selected.Index = Index;
        }
    }
}