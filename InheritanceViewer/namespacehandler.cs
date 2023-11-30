using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InheritanceViewer
{
    public struct NamespaceInfo
    {
        public string Name { get; }
        public int Startpos { get; }
        public int Endpos { get; }

        // Konstruktor
        public NamespaceInfo(string name, int startpos, int endpos)
        {
            Name= name;
            Startpos = startpos;
            Endpos = endpos;
        }
    }

    public class Namespacehandler
    {//Constructor
        public Namespacehandler()
        {

        }

        //returns a List of Namespaceinfos found in the given text. The List is sorted by the startingpos of the NamespaceInfo
        //precondition: the given text is not allowed to have comments as they can cause errors.
        public List<NamespaceInfo> GetAllNamespacesInText(string TextWithNamespaces)
        {
            List<NamespaceInfo> namespaces = new List<NamespaceInfo>();
            

            string namespacepattern = @"(?:namespace)\s*.*\s*{";
            var namespaces_regex = Regex.Matches(TextWithNamespaces, namespacepattern);
            
            foreach(Match match in namespaces_regex)
            {
                //Get name and start- end position of the namespace
                string nameOfNamespace = NameOfNamespacematch(match);
                int startingpos = match.Index;
                //Die Endposition muss noch mit einer besonderen Funktion geholt werden um die schließende Klammer des namespaces zu finden.
                int endpos = EndOfNamespace(TextWithNamespaces, startingpos);

                namespaces.Add(new NamespaceInfo(nameOfNamespace, startingpos, endpos));
            }   

            namespaces.Sort((x, y) => x.Startpos.CompareTo(y.Startpos));

            return namespaces;
        }

        int EndOfNamespace(string text, int startingPosition)
        {
            int openBraceCount = 0;
            int textLength = text.Length;

            for (int i = startingPosition; i < textLength; i++)
            {
                if (text[i] == '{')
                {
                    openBraceCount++;
                }
                else if (text[i] == '}')
                {
                    if (--openBraceCount == 0)
                    {
                        return i;
                    }
                }
            }

            //In case of negative Index no matching closing brace was bound. An error-message should be thrown.
            //TODO: Show error-message
            return -1;
        }

        string NameOfNamespacematch(Match match)
        {
            string text = match.Value;
            return text
                .Replace("namespace", "")
                .Replace("{", "")
                .Trim();
        }
    }


}
