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
            TextBox.WidthRequest -= ListMark.WidthRequest;
            ListMark.Margin = new Thickness(0, XButton.HeightRequest / 2 - 5);
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
                content += "<li>" + view.Text + "</li><br>";
            }
            content += "</ul><br>";
            return content;
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