using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Note
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public int Mid { get; set; }
        public int Mon { get; set; }
        public int Usn { get; set; }
        public string Tags { get; set; }
        public List<string> Fields { get; set; }
        public int CheckSum { get; set; }
        public int Flags { get; set; }
        public string Data { get; set; }
    }
}
