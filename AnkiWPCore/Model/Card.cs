using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Card
    {
        private cards m_cardData;

        public Card(cards cardData)
        {
            Debug.Assert(cardData != null);
            m_cardData = cardData;
        }

        public long Id
        {
            get { return m_cardData.id; }
        }

        public long NoteId
        {
            get { return m_cardData.nid; }
        }

        public long DeckId
        {
            get { return m_cardData.odid != 0 ? m_cardData.odid : m_cardData.did; }
        }

        public int Order
        {
            get { return m_cardData.ord; }
        }

        public cards Data
        {
            get { return m_cardData; }
        }
    }
}
