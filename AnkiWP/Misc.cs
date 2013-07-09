using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnkiWP
{
    static class Misc
    {
        public static void Render(Model.Model.Template template, string context, string encoding)
        {
            //template = template or self.template
            //context = context or self.context
            //template = RenderSections(template);
            var result = RenderTags(template);
            //if encoding is not None:
            if (encoding != string.Empty)
            {
                result = Encode(result, encoding);
            }
        }

        public static string RenderTags(Model.Model.Template template)
        {
            return string.Empty;
        }

        public static string Encode(string text, string encoding)
        {
            return text;
        }
    }

    public class Template
    {
        private Model.Model.Template m_template;
        private string otag = "{{";
        private string ctag = "}}";
        private Regex m_sectionRe;
        private Regex m_tagRe;

        public Template(Model.Model.Template template)
        {
            m_template = template;

            //CompileRegExps();
        }

        public void CompileRegExps()
        {
            var tags = new Dictionary<string, string>();
            tags["otag"] = Regex.Escape(otag);
            tags["ctag"] = Regex.Escape(ctag);

            var section = string.Format(@"%{0}s[\#|^]([^\}}]*)%{1}s(.+?)%{0}s/\1%{1}s", tags["otag"], tags["ctag"]);
            m_sectionRe = new Regex(section, RegexOptions.Multiline | RegexOptions.Compiled);

            var tag = string.Format(@"%{0}s(#|=|&|!|>|\{{)?(.+?)\1?%{1}s+", tags["otag"], tags["ctag"]);
            m_tagRe = new Regex(tag, RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public Model.Model.Template RenderSections(Model.Model.Template template)
        {


            return template;
        }

    }
}
