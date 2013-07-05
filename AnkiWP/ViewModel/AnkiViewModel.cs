using AnkiWP.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AnkiWP.ViewModel
{
    public class AnkiViewModel : ViewModelBase
    {
        private Model.Collection m_collection;
        readonly public ObservableCollection<DeckItem> m_decks = new ObservableCollection<DeckItem>();

        public AnkiViewModel(Model.Collection collection)
        {
            Debug.Assert(collection != null);

            m_collection = collection;
            m_collection.PropertyChanged += Collection_PropertyChanged;
            
        }

        private void Collection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Decks":
                    RaisePropertyChanged(() => this.Decks);
                    break;
                default:
                    break;
            }
        }

        public ObservableCollection<Model.Deck> Decks
        {
            get { return m_collection.Decks; }
        }

        public ObservableCollection<DeckItem> DecksOld
        {
            get { return m_decks; }
        }

        public void AddDeckItem(DeckItem newDeckItem)
        {
            m_decks.Add(newDeckItem);
        }
    }
}
