using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Deck
    {
        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("extendRev")]
        public dynamic ExtendRev { get; set; }

        [JsonProperty("usn")]
        public dynamic Usn { get; set; }

        [JsonProperty("collapsed")]
        public bool Collapsed { get; set; }

        [JsonProperty("newToday")]
        public dynamic NewToday { get; set; }

        [JsonProperty("timeToday")]
        public dynamic TimeToday { get; set; }

        [JsonProperty("dyn")]
        public dynamic Dyn { get; set; }

        [JsonProperty("extendNew")]
        public dynamic ExtendNew { get; set; }

        [JsonProperty("conf")]
        public dynamic Conf { get; set; }

        [JsonProperty("revToday")]
        public dynamic ReviseToday { get; set; }

        [JsonProperty("lrnToday")]
        public dynamic LearnToday { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("mod")]
        public dynamic Mod { get; set; }
    }
}
