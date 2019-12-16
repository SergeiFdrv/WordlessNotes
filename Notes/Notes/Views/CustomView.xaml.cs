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
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 20,
                    Placeholder = "Header 1",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Header2)
            {
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 18,
                    Placeholder = "Header 2",
                    PlaceholderColor = Color.Gray
                };
            }
            else if (type == CustomViewTypes.Header3)
            {
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 16,
                    Placeholder = "Header 3",
                    PlaceholderColor = Color.Gray
                };
            }
            else
            {
                TextEditor = new Renderers.TextEditor
                {
                    Placeholder = "|",
                    PlaceholderColor = Color.Gray
                };
            }
            TextEditor.WidthRequest = 270;
            if (type == CustomViewTypes.Image)
            {
                meat.Children.Insert(0, new BoxView { HeightRequest = 100, BackgroundColor = Color.DarkOrchid}); // TODO: заменить тестовый BoxView обратно на Image
                TextEditor.FontSize = 12;
                TextEditor.TextColor = Color.Red;
            }
        }

        public int Index { get; set; }

        //public Renderers.TextEditor TextEditor { get; set; }

        public CustomViewTypes Type { get; set; } /* TODO: размер текста должен меняться при изменении этого значения. Когда я пишу это в set, эмулятор убивает программу молча */

        public string Text
        {
            get => TextEditor.Text;
            set => TextEditor.Text = value;
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