using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Collection : ViewModelBase
    {
        private ObservableCollection<Model> m_models = new ObservableCollection<Model>();
        private ObservableCollection<Deck> m_decks = new ObservableCollection<Deck>();
        private Dictionary<string, dynamic> m_tags = new Dictionary<string, dynamic>();
        private ObservableCollection<DeckConfig> m_deckConfigs = new ObservableCollection<DeckConfig>();
        private Config m_config = new Config();

        public ObservableCollection<Model> Models
        {
            get { return m_models; }
            set
            {
                m_models = value;
                RaisePropertyChanged(() => this.Models);
            }
        }

        public ObservableCollection<Deck> Decks
        {
            get { return m_decks; }
            set
            {
                m_decks = value;
                RaisePropertyChanged(() => this.Decks);
            }
        }

        public Dictionary<string, dynamic> Tags
        {
            get { return m_tags; }
            set
            {
                m_tags = value;
                RaisePropertyChanged(() => this.Tags);
            }
        }

        public ObservableCollection<DeckConfig> DeckConfigs
        {
            get { return m_deckConfigs; }
            set
            {
                m_deckConfigs = value;
                RaisePropertyChanged(() => this.DeckConfigs);
            }
        }

        public Config Config
        {
            get { return m_config; }
            set
            {
                m_config = value;
                RaisePropertyChanged(() => this.Config);
            }
        }
    }
}
