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
            if (ParentPage != null) ParentPage.Selected = this;
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
                if (value == CustomViewTypes.Image)
                {
                    TextEditor.FontSize = 12;
                    TextEditor.Placeholder = "Image description";
                    TextEditor.TextColor = Color.Red;
                    Img.HeightRequest = 100; // TODO: заменить тестовый BoxView обратно на Image
                    return;
                }
                else if (value == CustomViewTypes.List)
                {
                    TextEditor.FontSize = 14;
                    TextEditor.Placeholder = "List";
                    List.IsVisible = true;
                    return;
                }
                Img.HeightRequest = 0;
                TextEditor.TextColor = Color.Black;
                List.IsVisible = false;
                if (value == CustomViewTypes.Header1)
                {
                    TextEditor.FontSize = 20;
                    TextEditor.Placeholder = "Header 1";
                }
                else if (value == CustomViewTypes.Header2)
                {
                    TextEditor.FontSize = 18;
                    TextEditor.Placeholder = "Header 2";
                }
                else if (value == CustomViewTypes.Header3)
                {
                    TextEditor.FontSize = 16;
                    TextEditor.Placeholder = "Header 3";
                }
                else
                {
                    TextEditor.FontSize = 14;
                    TextEditor.Placeholder = "Paragraph";
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
            Console.WriteLine("--- ELEMENT TAPPED ---");
            if (ParentPage != null) ParentPage.Selected = this;
        }
    }
}