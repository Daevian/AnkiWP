using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnkiWP
{
    public static class Template
    {
        private static Model.Model.Template m_template;
        private static string otag = "{{";
        private static string ctag = "}}";
        private static Regex m_sectionRe;
        private static Regex m_tagRe;

        static Template()
        {
            CompileRegExps();
        }

        public static void CompileRegExps()
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
                

                //try:
                //    func = modifiers[tag_type]
                //    replacement = func(self, tag_name, context)
                //    template = template.replace(tag, replacement)
                //except (SyntaxError, KeyError):
                //    return u"{{invalid template}}"
            }

            return template;
        }

    }
}
