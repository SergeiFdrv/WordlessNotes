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
    public partial class LstView : AbsCstmView
    {
        public LstView()
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

        public static string HTMLUnorderedList(IEnumerable<LstView> list, ref int i)
        {
            if (list is null) return string.Empty;
            i = list.Last().Index;
            string content = "<ul><br>";
            foreach (LstView view in list)
            {
                content += view.ToHTMLString();
            }
            content += "</ul><br>";
            return content;
        }

        private void SetListSize()
        {
            double _margin = XButton.HeightRequest / 2 - 5;
            ListMark.Margin = new Thickness(_margin, _margin, 0, _margin);
            TextBox.WidthRequest -= (ListMark.WidthRequest + ListMark.Margin.Left + ListMark.Margin.Right);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Delete(this);
        }

        private void Item_Focused(object sender, FocusEventArgs e)
        {
            Highlight(this);
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged(this);
        }
    }
}