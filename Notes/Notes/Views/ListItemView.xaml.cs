using Notes.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListItemView : CustomView
    {
        public ListItemView()
        {
            InitializeComponent();
            SetSize();
            SetListSize();
            ParentResized += delegate
            {
                SetListSize();
            };
        }

        public override TextEditor TextBox => TextEditor;

        public override Button XButton => XBtn;

        public override string ToHTMLString() => "<li>" + Text + "</li><br>";

        private void SetListSize()
        {
            double _margin = XButton.HeightRequest / 2 - 5;
            ListMark.Margin = new Thickness(_margin, _margin, 0, _margin);
            TextBox.WidthRequest -= (ListMark.WidthRequest + ListMark.Margin.Left + ListMark.Margin.Right);
        }

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
    }
}