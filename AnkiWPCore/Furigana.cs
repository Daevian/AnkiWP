using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnkiWP
{
    public static class Furigana
    {
        private static string m_regex = @"' ?([^ >]+?)\[(.+?)\]'";
        private static string m_ruby = @"<ruby><rb>\1</rb><rt>\2</rt></ruby>";

        public static void Install()
        {
            Template.AddFieldModifier("kanji", FieldModifierKanji);
            Template.AddFieldModifier("kana", FieldModifierKana);
            Template.AddFieldModifier("furigana", FieldModifierFurigana);
        }

        private static string FieldModifierKanji(Template.FieldModifierArgs args)
        {
            var match = Regex.Replace(Munge(args.Text), m_regex, NoSound(@"\1"));
            return string.Empty;
        }

        private static string FieldModifierKana(Template.FieldModifierArgs args)
        {
            var match = Regex.Replace(Munge(args.Text), m_regex, NoSound(@"\2"));
            return string.Empty;
        }

        private static string FieldModifierFurigana(Template.FieldModifierArgs args)
        {
            //var match = Regex.Replace(Munge(args.Text), m_regex, NoSound(m_ruby));
            var match = Regex.Match(Munge(args.Text), m_regex);
            return string.Empty;
        }

        private static MatchEvaluator NoSound(string replacement)
        {
            MatchEvaluator func = (match) =>
            {
                if (match.Groups[2].Value.StartsWith("sound:"))
                {
                    return match.Groups[0].Value;
                }
                else
                {
                    return Regex.Replace(match.Groups[0].Value, m_regex, replacement);
                }
            };

            return func;
        }

        private static string Munge(string text)
        {
            // 0XA0 == &nbsp;
            var result = text.Replace((char)0XA0, ' ');
            return result;
        }
    }
}
