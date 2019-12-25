using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Notes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime DateTime { get; set; }
    }
}
