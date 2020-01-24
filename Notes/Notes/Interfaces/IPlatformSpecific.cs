using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Interfaces
{
    public interface IPlatformSpecific
    {
        string GetDocsDirectory();

        void SayLong(string message);

        void SayShort(string message);
    }
}
