using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AnkiWP.Model
{
    public class DeckItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private ObservableCollection<CardItem> m_cards = new ObservableCollection<CardItem>();

        private string m_name;

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int DueCount
        {
            get { return 80; }
        }

        public int NewCount
        {
            get { return 30; }
        }

        public ObservableCollection<CardItem> Cards
        {
            get { return m_cards; }
            set
            {
                m_cards = value;
                NotifyPropertyChanged("Cards");
            }
        }

        public CardItem CurrentCard
        {
            get { return m_cards[0]; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}
