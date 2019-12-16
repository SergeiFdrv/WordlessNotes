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
                    Placeholder = "Header 1"
                };
            }
            else if (type == CustomViewTypes.Header2)
            {
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 18,
                    Placeholder = "Header 2"
                };
            }
            else if (type == CustomViewTypes.Header3)
            {
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 16,
                    Placeholder = "Header 3"
                };
            }
            else if (type == CustomViewTypes.Paragraph)
            {
                TextEditor = new Renderers.TextEditor
                {
                    Placeholder = "Paragraph"
                };
            }
            else if (type == CustomViewTypes.Image)
            {
                TextEditor = new Renderers.TextEditor
                {
                    FontSize = 12,
                    Placeholder = "Title",
                    TextColor = Color.Red
                };
                meat.Children.Add(new BoxView { HeightRequest = 100, BackgroundColor = Color.DarkOrchid, Margin = 0 }); // TODO: заменить тестовый BoxView обратно на Image
            }
            TextEditor.WidthRequest = 270;
            TextEditor.PlaceholderColor = Color.Gray;
            meat.Children.Add(TextEditor); // Вариант, в котором TextEditor объявлялся в xaml, приводил к тому, что весь код в этом методе не срабатывал
        }

        public int Index { get; set; }

        public Renderers.TextEditor TextEditor { get; set; }

        public CustomViewTypes Type
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        } /* TODO: размер текста должен меняться при изменении этого значения. Когда я пишу это в set, эмулятор убивает программу молча */

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