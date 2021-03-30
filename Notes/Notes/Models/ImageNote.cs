using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public class ImageNote
    {
        [ForeignKey(typeof(Image))]
        public int ImageId { get; set; }

        [ForeignKey(typeof(Note))]
        public int NoteId { get; set; }
    }
}
