using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.ViewModel
{
    public class CardViewModel : ViewModelBase
    {
        public enum CardSide
        {
            Question,
            Answer,
        };

        private Model.Card m_card;
        private Model.Note m_note;
        private Model.Model m_model;
        private Model.Model.Template m_template;
        private Model.Deck m_deck;
        private Dictionary<CardSide, string> m_qa;
        private string m_question = string.Empty;
        private string m_answer = string.Empty;
        private string m_css = string.Empty;

        public CardViewModel(Model.Card card)
        {
            Debug.Assert(card != null);
            m_card = card;

            m_note = App.Collection.Notes.FirstOrDefault(note => note.Id == m_card.NoteId);
            Debug.Assert(m_note != null);

            m_model = App.Collection.Models.FirstOrDefault(model => model.Id == m_note.ModelId);
            Debug.Assert(m_model != null);

            switch (m_model.Type)
            {
                case AnkiWP.Model.Model.ModelType.Standard:
                    m_template = m_model.Templates[m_card.Order];
                    break;
                default:
                    m_template = m_model.Templates[0];
                    break;
            }

            Debug.Assert(m_template != null);

            long deckId = m_card.DeckId;
            m_deck = App.Collection.Decks.FirstOrDefault(deck => deck.Id == deckId);
            Debug.Assert(m_deck != null);

            CompileCard();
        }

        public string Question
        {
            get { return m_question; }
            set
            {
                m_question = value;
                RaisePropertyChanged(() => this.Question);
            }
        }

        public string Answer
        {
            get { return m_answer; }
            set
            {
                m_answer = value;
                RaisePropertyChanged(() => this.Answer);
            }
        }

        public string Css
        {
            get { return m_css; }
            set
            {
                m_css = value;
                RaisePropertyChanged(() => this.Css);
            }
        }

        private void CompileCard(bool browser = false)
        {
            RenderQaData data = new RenderQaData
            {
                CardId = m_card.Id,
                NoteId = m_note.Id,
                ModelId = m_model.Id,
                DeckId = m_card.DeckId,
                Ord = m_card.Order,
                Tags = m_note.Tags.Distinct().ToList(),
                Fields = m_note.Fields,
            };

            data.Tags.Sort();

            BrowserFormat browserFormat = null;
            if (browser)
            {
                browserFormat = new BrowserFormat
                {
                    BrowserQuestionFormat = m_template.BrowserQuestionFormat,
                    BrowserAnswerFormat = m_template.BrowserAnswerFormat,
                };
            }

            m_qa = RenderQa(data, browserFormat);

            this.Css = string.Format("<style>{0}</style>", m_model.Css);
            this.Question = this.Css + m_qa[CardSide.Question];
            this.Answer = this.Css + m_qa[CardSide.Answer];
        }

        private Dictionary<CardSide, string> RenderQa(RenderQaData data, BrowserFormat browserFormat = null)
        {
            var fields = new Dictionary<string, string>();

            {
                var fieldMap = m_model.Fields.ToDictionary(field => field.Name);
                foreach (var field in fieldMap)
                {
                    fields[field.Key] = data.Fields[field.Value.Order];
                }
            }

            fields["Tags"] = Model.Note.JoinFields(data.Tags);
            fields["Type"] = m_model.Name;
            fields["Deck"] = m_deck.Name;
            fields["Card"] = m_template.Name;
            fields["c" + (m_card.Order + 1)] = "1";

            string questionFormat = m_template.QuestionFormat;
            string answerFormat = m_template.AnswerFormat;

            if (browserFormat != null)
            {
                questionFormat = browserFormat.BrowserQuestionFormat;
                answerFormat = browserFormat.BrowserAnswerFormat;
            }

            var generatedSides = new Dictionary<CardSide, string>();

            var cardSides = new Dictionary<CardSide, string>();
            cardSides[CardSide.Question] = questionFormat;
            cardSides[CardSide.Answer] = answerFormat;
            foreach (var side in cardSides)
            {
                var format = side.Value;
                switch (side.Key)
                {
                    case CardSide.Question:
                        format = format.Replace("{{cloze:", string.Format("{{cq:{0}", m_card.Order + 1));
                        format = format.Replace("<%cloze:", string.Format("<%cq:{0}", m_card.Order + 1));
                        break;
                    case CardSide.Answer:
                        format = format.Replace("{{cloze:", string.Format("{{ca:{0}", m_card.Order + 1));
                        format = format.Replace("<%cloze:", string.Format("<%ca:{0}", m_card.Order + 1));
                        //fields["FrontSide"] = stripSounds(d['q'])
                        break;
                }

                var html = format;
                generatedSides[side.Key] = html;

                //fields = runFilter("mungeFields", fields, model, data, self)
                //html = anki.template.render(format, fields)
                //d[type] = runFilter(
                //    "mungeQA", html, type, fields, model, data, self)
                //# empty cloze?
                //if type == 'q' and model['type'] == MODEL_CLOZE:
                //    if not self.models._availClozeOrds(model, data[6], False):
                //        d['q'] += ("<p>" + _(
                //    "Please edit this note and add some cloze deletions. (%s)") % (
                //    "<a href=%s#cloze>%s</a>" % (HELP_SITE, _("help"))))
            }
            
            return generatedSides;
        }

        private class RenderQaData
        {
            public long CardId { get; set; }
            public long NoteId { get; set; }
            public long ModelId { get; set; }
            public long DeckId { get; set; }
            public long Ord { get; set; }
            public List<string> Tags { get; set; }
            public List<string> Fields { get; set; }
        }

        private class BrowserFormat
        {
            public string BrowserQuestionFormat { get; set; }
            public string BrowserAnswerFormat { get; set; }
        }
    }
}
