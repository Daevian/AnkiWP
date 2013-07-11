using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnkiWP
{
    using System.Diagnostics;
    using System.Reflection;
    using TagModifier = Func<string, Dictionary<string, string>, string>;
    using FieldModifier = Func<Template.FieldModifierArgs, string>;

    public static class Template
    {
        public class FieldModifierArgs
        {
            public string Modifier { get; set; }
            public string Extra { get; set; }
            public string Text { get; set; }
            public string TagName { get; set; }
        };

        private enum ClozeSide
        {
            Answer,
            Question,
        };

        private static Model.Model.Template m_template;
        private static string otag = "{{";
        private static string ctag = "}}";
        private static string m_clozeReg = @"{{c{0}::(.*?)(::(.*?))?}}";
        private static Regex m_sectionRe;
        private static Regex m_tagRe;
        private static Dictionary<string, TagModifier> m_modifiers = new Dictionary<string, TagModifier>();
        private static Dictionary<string, FieldModifier> m_fieldModifiers = new Dictionary<string, FieldModifier>();

        static Template()
        {
            CompileRegExps();

            m_modifiers["{"] = RenderTag;
            m_modifiers["!"] = RenderComment;
            m_modifiers[""] = RenderUnescaped;
            m_modifiers["="] = RenderDelimiter;

            AddFieldModifier("text", FieldModifierText);
            AddFieldModifier("type", FieldModifierType);
            AddFieldModifier("ca", FieldModifierCloze);
            AddFieldModifier("cq", FieldModifierCloze);
        }

        public static void AddFieldModifier(string modifierName, FieldModifier modifier)
        {
            m_fieldModifiers[modifierName] = modifier;
        }

        private static void CompileRegExps()
        {
            var tags = new Dictionary<string, string>();
            tags["otag"] = Regex.Escape(otag);
            tags["ctag"] = Regex.Escape(ctag);

            var section = string.Format(@"{0}[\#|^]([^\}}]*){1}(.+?){0}/\1{1}", tags["otag"], tags["ctag"]);
            m_sectionRe = new Regex(section, RegexOptions.Multiline | RegexOptions.Singleline);

            var tag = string.Format(@"{0}(#|=|&|!|>|\{{)?(.+?)\1?{1}+", tags["otag"], tags["ctag"]);
            m_tagRe = new Regex(tag, RegexOptions.Multiline | RegexOptions.Singleline);
        }

        public static string Render(string template, Dictionary<string, string> context)
        {
            var result = RenderSections(template, context);
            result = RenderTags(result, context);

            //if encoding is not None:
            //    result = result.encode(encoding)
            
            return result;
        }

        private static string RenderSections(string template, Dictionary<string, string> context)
        {
            var matches = m_sectionRe.Matches(template);

            return template;
        }

        private static string RenderTags(string template, Dictionary<string, string> context)
        {
            var matches = m_tagRe.Matches(template);

            foreach (Match match in matches)
            {
                var tag = match.Groups[0].ToString();
                var tagType = match.Groups[1].ToString();
                var tagName = match.Groups[2].ToString();
                tagName = tagName.Trim();

                TagModifier modifier;
                string replacement = "{{invalid template}}";
                if (m_modifiers.TryGetValue(tagType, out modifier))
                {
                    replacement = modifier(tagName, context);
                }

                template = template.Replace(tag, replacement);
            }

            return template;
        }

        private static string RenderTag(string tagName, Dictionary<string, string> context)
        {
            return RenderUnescaped(tagName, context);
        }

        private static string RenderComment(string tagName, Dictionary<string, string> context)
        {
            return string.Empty;
        }

        private static string RenderUnescaped(string tagName, Dictionary<string, string> context)
        {
            string text = GetFieldOrProperty(tagName, context);
            if (text != null)
            {
                return text;
            }

            string modifier = string.Empty;
            string tag = string.Empty;
            string extra = string.Empty;
            
            // Split tag into modifiers and actual tag
            var parts = tagName.Split(new char[] {':'}, 3);
            if (parts.Length > 0 && string.IsNullOrWhiteSpace(parts[0]) ||
                parts.Length == 1)
            {
                return string.Format("{{unknown field {0}}}", tagName);
            }
            else if (parts.Length == 2)
            {
                modifier = parts[0];
                tag = parts[1];
            }
            else if (parts.Length == 3)
            {
                modifier = parts[0];
                extra = parts[1];
                tag = parts[2];
            }

            text = GetFieldOrProperty(tag, context);

            // Built-in modifiers
            FieldModifier fieldModifier;
            if (m_fieldModifiers.TryGetValue(modifier, out fieldModifier))
            {
                if (fieldModifier != null)
                {
                    return fieldModifier(new FieldModifierArgs
                        {
                            Modifier = modifier,
                            Extra = extra,
                            Text = text,
                            TagName = tagName,
                        });
                }
            }

            return string.Format("{{unknown field {0}}}", tagName);
        }

        private static string RenderDelimiter(string tagName, Dictionary<string, string> context)
        {
            throw new NotImplementedException();
            return string.Empty;
        }

        private static string FieldModifierText(FieldModifierArgs args)
        {
            throw new NotImplementedException();
        }

        private static string FieldModifierType(FieldModifierArgs args)
        {
            return string.Format("[[{0}]]", args.TagName);
        }

        private static string FieldModifierCloze(FieldModifierArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.Text) &&
                !string.IsNullOrWhiteSpace(args.Extra))
            {
                ClozeSide type = ClozeSide.Answer;
                switch (args.Modifier)
                {
                    case "cq":
                        type = ClozeSide.Question;
                        break;
                    case "ca":
                        type = ClozeSide.Answer;
                        break;
                }

                return ClozeText(args.Text, args.Extra, type);
            }
            else
            {
                return string.Empty;
            }
        }

        private static string ClozeText(string text, string order, ClozeSide type)
        {
            throw new NotImplementedException();

            //string reg = string.Format(m_clozeReg, order);
            //var match = Regex.Match(text, reg);


            //def clozeText(self, txt, ord, type):
            //    reg = clozeReg
            //    if not re.search(reg%ord, txt):
            //        return ""
            //    def repl(m):
            //        # replace chosen cloze with type
            //        if type == "q":
            //            if m.group(3):
            //                return "<span class=cloze>[%s]</span>" % m.group(3)
            //            else:
            //                return "<span class=cloze>[...]</span>"
            //        else:
            //            return "<span class=cloze>%s</span>" % m.group(1)
            //    txt = re.sub(reg%ord, repl, txt)
            //    # and display other clozes normally
            //    return re.sub(reg%".*?", "\\1", txt)

            return string.Empty;
        }

        private static string GetFieldOrProperty(string tagName, Dictionary<string, string> context, string defaultString = null)
        {
            string text = string.Empty;
            if (context.TryGetValue(tagName, out text))
            {
                return text;
            }
            else
            {
                text = GetProperty(context, tagName);
                if (string.IsNullOrWhiteSpace(text))
                {
                    return defaultString;
                }
            }

            return text;
        }

        private static string GetProperty(object obj, string name)
        {
            Debug.Assert(obj != null);
            Type type = obj.GetType();

            var property = type.GetProperty(name);
            if (property != null)
            {
                return property.GetValue(obj).ToString();
            }

            return null;
        }
    }
}
