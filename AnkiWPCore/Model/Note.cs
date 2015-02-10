using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Note
    {
        private notes m_noteData;
        private List<string> m_fields = new List<string>();
        private List<string> m_tags = new List<string>();

        public Note(notes noteData)
        {
            Debug.Assert(noteData != null);
            m_noteData = noteData;

            this.Fields = SplitFields(m_noteData.flds);
            this.Tags = SplitTags(m_noteData.tags);
        }

        public long Id
        {
            get { return m_noteData.id; }
        }

        public long ModelId
        {
            get { return m_noteData.mid; }
        }

        public List<string> Fields
        {
            get { return m_fields; }
            set { m_fields = value; }
        }

        public List<string> Tags
        {
            get { return m_tags; }
            set { m_tags = value; }
        }

        public notes Data
        {
            get { return m_noteData; }
        }

        public static List<string> SplitFields(string joinedFields)
        {
            return joinedFields.Split('\x1f').ToList();
        }

        public static string JoinFields(List<string> fields)
        {
            return string.Join("\x1f", fields);
        }

        public static List<string> SplitTags(string joinedTags)
        {
            return joinedTags.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
