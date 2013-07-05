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
        public bool Autoplay { get; set; }

        [JsonProperty("id")]
        public dynamic Id { get; set; }

        [JsonProperty("mod")]
        public dynamic Mod { get; set; }

        public class LapseConfig
        {
            [JsonProperty("leechFails")]
            public dynamic leechFails { get; set; }

            [JsonProperty("minInt")]
            public dynamic minInt { get; set; }

            [JsonProperty("delays")]
            public dynamic delays { get; set; }

            [JsonProperty("leechAction")]
            public dynamic leechAction { get; set; }

            [JsonProperty("mult")]
            public dynamic mult { get; set; }
        }

        public class RevConfig
        {
            [JsonProperty("perDay")]
            public dynamic PerDay { get; set; }

            [JsonProperty("ivlFct")]
            public dynamic IvlFct { get; set; }

            [JsonProperty("maxIvl")]
            public dynamic MaxIvl { get; set; }

            [JsonProperty("minSpace")]
            public dynamic MinSpace { get; set; }

            [JsonProperty("ease4")]
            public dynamic Ease4 { get; set; }

            [JsonProperty("fuzz")]
            public dynamic Fuzz { get; set; }
        }

        public class NewConfig
        {
            [JsonProperty("separate")]
            public bool Separate { get; set; }

            [JsonProperty("delays")]
            public dynamic Delays { get; set; }

            [JsonProperty("perDay")]
            public dynamic PerDay { get; set; }

            [JsonProperty("ints")]
            public dynamic Ints { get; set; }

            [JsonProperty("initialFactor")]
            public dynamic InitialFactor { get; set; }

            [JsonProperty("order")]
            public dynamic Order { get; set; }
        }
    }
}
