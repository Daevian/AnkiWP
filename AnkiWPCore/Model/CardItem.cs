using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnkiWP.Model
{
    public class CardItem
    {
        private string m_front;
        private string m_back;
        private LearningType m_learningType;
        private TimeSpan m_nextReviewDate;

        public enum LearningType
        {
            New,
            Learning,
            Review
        };

        public string Front
        {
            get { return "FRONT 漢字XT"; }
            set { m_front = value; }
        }

        public string Back
        {
            get { return "BACK TEXT"; }
            set { m_back = value; }
        }

        public LearningType CardType
        {
            get { return m_learningType; }
            set { m_learningType = value; }
        }

        public TimeSpan NextReview
        {
            get { return m_nextReviewDate; }
            set { m_nextReviewDate = value; }
        }
    }
}
