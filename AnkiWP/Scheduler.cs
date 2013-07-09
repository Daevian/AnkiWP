using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP
{
    public class Scheduler
    {
        public Model.Card GetCard()
        {
            return App.Collection.Cards[10000];
        }
    }
}
