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
        public ulong Usn { get; set; }

        [JsonProperty("collapsed")]
        public bool Collapsed { get; set; }

        [JsonProperty("newToday")]
        public dynamic NewToday { get; set; }

        [JsonProperty("timeToday")]
        public dynamic TimeToday { get; set; }

        [JsonProperty("dyn")]
        public ulong Dyn { get; set; }

        [JsonProperty("extendNew")]
        public dynamic ExtendNew { get; set; }

        [JsonProperty("conf")]
        public ulong Conf { get; set; }

        [JsonProperty("revToday")]
        public dynamic ReviseToday { get; set; }

        [JsonProperty("lrnToday")]
        public dynamic LearnToday { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("mod")]
        public ulong Mod { get; set; }
    }

    public static class DefaultDeck
    {
        public static Deck Generate()
        {
            Deck deck = new Deck
            {
                NewToday = new List<int> { 0, 0 },
                ReviseToday = new List<int> { 0, 0 },
                LearnToday = new List<int> { 0, 0 },
                TimeToday = new List<int> { 0, 0 },
                Conf = 1,
                Usn = 0,
                Desc = string.Empty,
                Dyn = 0,
                Collapsed = false,
                ExtendNew = 10,
                ExtendRev = 50,
            };

            return deck;
        }
    }
}
