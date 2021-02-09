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
    public partial class ParagraphView : CustomView
    {
        public ParagraphView()
        {
            InitializeComponent();
            SetSize();
        }

        public ParagraphView(string text) : this()
        {
            Text = text;
        }

        public override TextEditor TextBox => TextEditor;
        public override Button XButton => XBtn;

        public override string ToHTMLString() => "<p>" + Text + "</p><br>";

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