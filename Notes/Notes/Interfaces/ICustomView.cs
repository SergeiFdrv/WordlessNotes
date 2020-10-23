using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Interfaces
{
    public interface ICustomView
    {
        int Index { get; set; }
        string Text { get; set; }
        string ToHTMLString();
    }
}
