using AnkiWP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace AnkiWP.ViewModel
{
    public class StudyViewModel : ViewModelBase
    {
        public enum ShowMode
        {
            Front,
            Back,
        };

        private CardViewModel m_cardVM;
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

        public CardViewModel CardViewModel
        {
            get { return m_cardVM; }
        }

        public void ShowNextCard()
        {
            var card = App.Scheduler.GetCard();
            m_cardVM = new CardViewModel(card);
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
            ShowNextCard();
        }
    }
}
