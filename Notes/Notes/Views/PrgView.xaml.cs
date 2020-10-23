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
    public partial class PrgView : AbsCstmView
    {
        public PrgView()
        {
            InitializeComponent();
            SetSize();
        }

        public PrgView(string text) : this()
        {
            Text = text;
        }

        public PrgView(int index, string text = "") : this(text)
        {
            Index = index;
        }

        public override TextEditor TextBox => TextEditor;
        public override Button XButton => XBtn;

        public override string ToHTMLString() => "<p>" + Text + "</p><br>";

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