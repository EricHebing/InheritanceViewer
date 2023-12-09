using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InheritanceViewer
{
    public class CommentRemover
    {
        //remove all c++-style Line- and Block-comments in the given Text
        public CommentRemover()
        {

        }

        //removes all C++-Style comments in given text. Highly optimized for Performance
        public string RemoveCommentsInString(string text)
        {
            StringBuilder result = new StringBuilder(text.Length);
            bool inSingleLineComment = false;
            bool inMultiLineComment = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (inSingleLineComment)
                {
                    if (text[i] == '\n')
                    {
                        inSingleLineComment = false;
                        result.Append('\n');
                    }
                }
                else if (inMultiLineComment)
                {
                    if (text[i] == '*' && i < text.Length - 1 && text[i + 1] == '/')
                    {
                        inMultiLineComment = false;
                        i++; // Skip the '*' character
                    }
                }
                else
                {
                    if (i < text.Length - 1 && text[i] == '/' && text[i + 1] == '/')
                    {
                        inSingleLineComment = true;
                        i++; // Skip the second '/' character
                    }
                    else if (i < text.Length - 1 && text[i] == '/' && text[i + 1] == '*')
                    {
                        inMultiLineComment = true;
                        i++; // Skip the second '/' character
                    }
                    else
                    {
                        result.Append(text[i]);
                    }
                }
            }

            return result.ToString();
        }

    }
}