using Notes.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Notes.Views
{
    public class HeaderView : ParagraphView
    {
        public HeaderView(byte level = 1, string text = "") : base(text)
        {
            Level = level;
            TextBox.FontAttributes = FontAttributes.Bold;
        }

        private byte LevelPvt;
        public byte Level
        {
            get => LevelPvt;
            set
            {
                LevelPvt = value;
                TextBox.Placeholder = Lang.Header + ' ' + LevelPvt;
                TextBox.FontSize = App.FontSize + 12 - 3 * LevelPvt;
            }
        }

        public override string ToHTMLString() => $"<h{Level}>" + Text + $"</h{Level}><br>";
    }
}
