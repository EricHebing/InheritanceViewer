using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InheritanceViewerShared
{
    public class CommentRemover
    {
        //remove all c++-style Line- and Block-comments in the given Text
        public CommentRemover()
        {

        }

        public string removeCommentsInString(string text)
        {
            //
            MatchCollection single_line_comments = Regex.Matches(text, @"\s*//.*");
            MatchCollection block_comments = Regex.Matches(text, @"/\*(.|\n)*?\*/");

            foreach(Match m in single_line_comments)
            {
                text = text.Replace(m.Value, "");
            }
            foreach(Match m in block_comments)
            {
                text = text.Replace(m.Value, "");
            }

            return text;
        }

    }
}
