using AnkiWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AnkiWP.ViewModel
{
    public class StudyViewModel : INotifyPropertyChanged
    {
        public enum ShowMode
        {
            Front,
            Back,
        };

        private Model.Deck m_deck;
        private ShowMode m_currentShowMode = ShowMode.Front;

        public event PropertyChangedEventHandler PropertyChanged;

        public Model.Deck Deck
        {
            get { return m_deck; }
            set
            {
                m_deck = value;
                OnDeckChanged(m_deck);
            }
        }

        public string DeckName
        {
            get { return m_deck.Name; }
        }

        public string CardFront
        {
            get
            {
                return "test text";
            }
        }

        //public CardItem CurrentCard
        //{
        //    get { return m_deck.CurrentCard; }
        //}

        public void ShowBack()
        {
            m_currentShowMode = ShowMode.Back;
            NotifyPropertyChanged("CardText");
        }

        public void ShowNextCard()
        {
            m_currentShowMode = ShowMode.Front;

            //m_deckItem.CurrentCard.NextReview = DateTime.Now.TimeOfDay + TimeSpan.FromMinutes(10.0);

            //m_deckItem.Cards = new ObservableCollection<CardItem>(m_deckItem.Cards.OrderBy(item => item.NextReview));
            

            NotifyPropertyChanged("CardText");
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnDeckChanged(Model.Deck deck)
        {
            
            var note = App.Collection.Notes[0];
            
            
            //App.
        }

        private void CompileCard()
        {

        }
    }
}
