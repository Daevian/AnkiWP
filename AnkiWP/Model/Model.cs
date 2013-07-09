﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnkiWP.Model
{
    public class Model
    {
        public enum ModelType
        {
            Standard = 0,
            Cloze = 1
        };

        [JsonProperty("vers")]
        public dynamic Vers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tags")]
        public dynamic Tags { get; set; }

        [JsonProperty("did")]
        public dynamic Did { get; set; }

        [JsonProperty("usn")]
        public dynamic Usn { get; set; }

        [JsonProperty("req")]
        public dynamic Req { get; set; }

        [JsonProperty("flds")]
        public IList<Field> Fields { get; set; }

        [JsonProperty("sortf")]
        public dynamic Sortf { get; set; }

        [JsonProperty("tmpls")]
        public IList<Template> Templates { get; set; }

        [JsonProperty("mod")]
        public dynamic Mod { get; set; }

        [JsonProperty("latexPost")]
        public string LatexPost { get; set; }

        [JsonProperty("type")]
        public ModelType Type { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("css")]
        public string Css { get; set; }

        [JsonProperty("latexPre")]
        public string LatexPre { get; set; }

        public class Field
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("media")]
            public dynamic Media { get; set; }

            [JsonProperty("sticky")]
            public bool Sticky { get; set; }

            [JsonProperty("rtl")]
            public bool Rtl { get; set; }

            [JsonProperty("ord")]
            public int Order { get; set; }

            [JsonProperty("font")]
            public string Font { get; set; }

            [JsonProperty("size")]
            public int Size { get; set; }
        }

        public class Template
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("afmt")]
            public string AnswerFormat { get; set; }

            [JsonProperty("qfmt")]
            public string QuestionFormat { get; set; }

            [JsonProperty("bafmt")]
            public string BrowserAnswerFormat { get; set; }

            [JsonProperty("bqfmt")]
            public string BrowserQuestionFormat { get; set; }

            [JsonProperty("did")]
            public dynamic Did { get; set; }

            [JsonProperty("ord")]
            public dynamic Ord { get; set; }
        }
    }
}
