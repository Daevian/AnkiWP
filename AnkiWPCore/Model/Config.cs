using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Config
    {
        [JsonProperty("nextPos")]
        public string NextPosition { get; set; }

        [JsonProperty("estTimes")]
        public bool EstimatedTimes { get; set; }

        [JsonProperty("sortBackwards")]
        public dynamic SortBackwards { get; set; }

        [JsonProperty("SortType")]
        public string SortType { get; set; }

        [JsonProperty("timeLim")]
        public dynamic TimeLimit { get; set; }

        [JsonProperty("activeDecks")]
        public dynamic ActiveDecks { get; set; }

        [JsonProperty("addToCur")]
        public dynamic AddToCurrent { get; set; }

        [JsonProperty("curDeck")]
        public dynamic CurrentDeck { get; set; }

        [JsonProperty("collapseTime")]
        public dynamic CollapseTime { get; set; }

        [JsonProperty("activeCols")]
        public IList<dynamic> ActiveCols { get; set; }

        [JsonProperty("dueCounts")]
        public bool DueCounts { get; set; }

        [JsonProperty("curModel")]
        public dynamic CurrentModel { get; set; }

        [JsonProperty("newSpread")]
        public dynamic NewSpread { get; set; }

    }
}
