using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using AnkiWP.Model;

namespace AnkiWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        public SyncThread m_syncThread;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.DataContext = App.ViewModel;
        }

        private void DeckItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedDeck = DeckListBox.SelectedItem as Deck;
            if (selectedDeck != null)
            {
                string uri = string.Format("/Pages/StudyPage.xaml?deck={0}", selectedDeck.Name);
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
        }

        private void NewDeckAppBarButton_Click(object sender, EventArgs e)
        {
            DeckItem newDeckItem = new DeckItem
            {
                Name = DateTime.Now.TimeOfDay.ToString()
            };

            newDeckItem.Cards.Add(new CardItem { Front = "FRONT 漢字XT", Back = "BACK TEXT", CardType = CardItem.LearningType.New });
            newDeckItem.Cards.Add(new CardItem { Front = "FRONT 漢字XT", Back = "BACK TEXT", CardType = CardItem.LearningType.Learning });
            newDeckItem.Cards.Add(new CardItem { Front = "FRONT 漢字XT", Back = "BACK TEXT", CardType = CardItem.LearningType.Review });
            newDeckItem.Cards.Add(new CardItem { Front = "FRONT 漢字XT", Back = "BACK TEXT", CardType = CardItem.LearningType.Review });

            App.ViewModel.AddDeckItem(newDeckItem);
        }

        private void SyncAppBarButton_Click(object sender, EventArgs e)
        {
            m_syncThread = new SyncThread(string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}