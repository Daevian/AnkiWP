using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AnkiWP.ViewModel;

namespace AnkiWP.Pages
{
    public partial class StudyPage : PhoneApplicationPage
    {
        private StudyViewModel m_viewModel;

        public StudyPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string queryStringValue = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("deck", out queryStringValue))
            {
                var deck = App.ViewModel.Decks.FirstOrDefault((item) => (string.Equals(item.Name, queryStringValue)));
                m_viewModel = new StudyViewModel(deck);
                this.DataContext = m_viewModel;

                this.ApplicationBar = Resources["FrontCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
            }
        }

        private void ShowAnswerAppBarIconButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ShowBack();
            //switch (m_viewModel.CurrentCard.CardType)
            //{
            //    case AnkiWP.Model.CardItem.LearningType.New:
            //        this.ApplicationBar = Resources["AnswerNewCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
            //        break;
            //    case AnkiWP.Model.CardItem.LearningType.Learning:
            //        this.ApplicationBar = Resources["AnswerLearningCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
            //        break;
            //    case AnkiWP.Model.CardItem.LearningType.Review:
            //        this.ApplicationBar = Resources["AnswerReviewCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
            //        break;
            //    default:
            //        this.ApplicationBar = null;
            //        break;
            //}
        }

        private void AnswerAgainAppBarIconButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ShowNextCard();
            this.ApplicationBar = Resources["FrontCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
        }

        private void AnswerHardAppBarIconButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ShowNextCard();
            this.ApplicationBar = Resources["FrontCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
        }

        private void AnswerGoodAppBarIconButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ShowNextCard();
            this.ApplicationBar = Resources["FrontCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
        }

        private void AnswerEasyAppBarIconButton_Click(object sender, EventArgs e)
        {
            m_viewModel.ShowNextCard();
            this.ApplicationBar = Resources["FrontCardAppBar"] as Microsoft.Phone.Shell.ApplicationBar;
        }
    }
}