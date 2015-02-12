using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class DeckConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("replayq")]
        public bool Replayq { get; set; }

        [JsonProperty("lapse")]
        public LapseConfig Lapse { get; set; }

        [JsonProperty("rev")]
        public RevConfig Rev { get; set; }

        [JsonProperty("timer")]
        public dynamic Timer { get; set; }

        [JsonProperty("dyn")]
        public bool Dyn { get; set; }

        [JsonProperty("maxTaken")]
        public dynamic MaxTaken { get; set; }

        [JsonProperty("usn")]
        public dynamic Usn { get; set; }

        [JsonProperty("new")]
        public NewConfig New { get; set; }

        [JsonProperty("autoplay")]
        public bool AutoPlay { get; set; }

        [JsonProperty("id")]
        public dynamic Id { get; set; }

        [JsonProperty("mod")]
        public dynamic Mod { get; set; }

        public class LapseConfig
        {
            public enum LeechActionType
            {
                Suspend = 0,
                Tagonly = 1,
            }

            [JsonProperty("leechFails")]
            public dynamic LeechFails { get; set; }

            [JsonProperty("minInt")]
            public dynamic MinInt { get; set; }

            [JsonProperty("delays")]
            public dynamic Delays { get; set; }

            [JsonProperty("leechAction")]
            public LeechActionType LeechAction { get; set; }

            [JsonProperty("mult")]
            public ulong Mult { get; set; }
        }

        public class RevConfig
        {
            [JsonProperty("perDay")]
            public ulong PerDay { get; set; }

            [JsonProperty("ivlFct")]
            public ulong IvlFct { get; set; }

            [JsonProperty("maxIvl")]
            public ulong MaxIvl { get; set; }

            [JsonProperty("minSpace")]
            public ulong MinSpace { get; set; }

            [JsonProperty("ease4")]
            public double Ease4 { get; set; }

            [JsonProperty("fuzz")]
            public double Fuzz { get; set; }

            [JsonProperty("bury")]
            public bool Bury { get; set; }
        }

        public class NewConfig
        {
            [JsonProperty("separate")]
            public bool Separate { get; set; }

            [JsonProperty("delays")]
            public dynamic Delays { get; set; }

            [JsonProperty("perDay")]
            public ulong PerDay { get; set; }

            [JsonProperty("ints")]
            public dynamic Ints { get; set; }

            [JsonProperty("initialFactor")]
            public ulong InitialFactor { get; set; }

            [JsonProperty("order")]
            public ulong Order { get; set; }

            [JsonProperty("bury")]
            public bool Bury { get; set; }
        }
    }

    public static class DefaultDeckConfig
    {
        private static ulong NEW_CARDS_DUE = 1;

        public static DeckConfig Generate()
        {
            DeckConfig config = new DeckConfig
            {
                Name = "Default",
                New = new DeckConfig.NewConfig
                {
                    Delays = new List<int> { 1, 10 },
                    Ints = new List<int> { 1, 4, 7 },
                    InitialFactor = 2500,
                    Separate = true,
                    Order = NEW_CARDS_DUE,
                    PerDay = 20,
                    Bury = true, // may not be set on old decks
                },
                Lapse = new DeckConfig.LapseConfig
                {
                    Delays = new List<int> { 10 },
                    Mult = 0,
                    MinInt = 1,
                    LeechFails = 8,
                    LeechAction = DeckConfig.LapseConfig.LeechActionType.Suspend,
                },
                Rev = new DeckConfig.RevConfig
                {
                    PerDay = 100,
                    Ease4 = 1.3,
                    Fuzz = 0.05,
                    MinSpace = 1, // not currently used
                    IvlFct = 1,
                    MaxIvl = 36500,
                    Bury = true, // may not be set on old decks
                },
                MaxTaken = 60,
                Timer = 0,
                AutoPlay = true,
                Replayq = true,
                Mod = 0,
                Usn = 0,
            };

            return config;
        }
    }
}
