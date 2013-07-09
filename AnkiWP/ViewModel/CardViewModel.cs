using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.ViewModel
{
    public class CardViewModel
    {
        private Model.Note m_note;
        private Model.Model m_model;
        private Model.Model.Template m_template;

        public CardViewModel(Model.Note note, Model.Model model, Model.Model.Template template)
        {
            Debug.Assert(note != null);
            Debug.Assert(model != null);
            Debug.Assert(template != null);

            m_note = note;
            m_model = model;
            m_template = template;
            CompileCard();
        }

        private void CompileCard()
        {

        }
    }
}
