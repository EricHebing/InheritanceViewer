using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace InheritanceViewer
{
    public class Inheritancefinder
    {
        //Standardkonstruktor
        public Inheritancefinder()
        {

        }

        public Dictionary<string, List<string>> BuildUpInheritanceDictionary(List<string> alistoffiles)
        {
            Dictionary<string, List<string>> linheritancedictionary = new Dictionary<string, List<string>>();

            foreach (var file in alistoffiles)
            {
                var classes_of_file = Findclassesandinheritance(file);
                foreach (var class_info in classes_of_file)
                {
                    if(!linheritancedictionary.ContainsKey(class_info.Item1))
                    {
                        linheritancedictionary.Add(class_info.Item1, new List<string>());
                    }
                    linheritancedictionary[class_info.Item1].AddRange(class_info.Item2);
                }
            }

            return linheritancedictionary;
        }


        //
        public List<Tuple<string, List<string>>> Findclassesandinheritance(string afilepath)
        {
            List<Tuple<string, List<string>>> foundclasses = new List<Tuple<string, List<string>>>();

            //Öffnen des files

            if (!File.Exists(afilepath))
            {
                //check if file exists and get string
                return foundclasses;
            }

            StreamReader reader = new StreamReader(afilepath);
            string filetext = reader.ReadToEnd();
            //Hole über Regex alle Klassendefinitionen

            //Es wird zuerst nach "class" gesucht. "\s+.+\s*" Es dürfen beliebige Zeichen und linebreaks kommen.
            //"\{" es muss eine öffnende geschwiefte Klammer auftauchen."(?:.|n)*?" Es dürfen beliebig viele Zeichen folgen(? so wenig wie mögich). Am Ende muss ein linebreack
            //sich schließende geschwiefte Klammern und ein Semikolon kommen.
            string classdefs = @"class\s+.+\s*\{(?:.|\n)*?\n\};";

            Regex rg = new Regex(classdefs);

            MatchCollection matchedclassdefs = rg.Matches(filetext);
            List<string> classes = matchedclassdefs.Cast<Match>().Select(m => m.Value).ToList();
            //(?<=:|,) *(public|protected|private)* *.*?(?=,|{)
            string inheritances = @"(?<=:|,) *(public|protected|private)* *.*?(?=,|{)";
            //Holfe für jede Klassendefinition die Inheritance
            Regex rg_inheritance = new Regex(inheritances);
            foreach (var lclass in classes)
            {//Finde die Inheritance
                MatchCollection matchedinheritances = rg_inheritance.Matches(lclass);
                List<string> inh = matchedinheritances.Cast<Match>().Select(m => m.Value).ToList();

                string classnamepattern = @"(?<=class) *.*?(?=:|{)";
                string class_name = Regex.Match(lclass, classnamepattern).Value;
                class_name = class_name.Trim();

                Tuple<string, List<string>> class_inheritance = Tuple.Create(class_name, new List<string>());
                


                foreach (var relation in inh)
                {
                    string inherited_class = CaseInsenstiveReplace(relation, "public", "");
                    inherited_class = CaseInsenstiveReplace(inherited_class, "private", "");
                    inherited_class = CaseInsenstiveReplace(inherited_class, "protected", "");
                    inherited_class = inherited_class.Replace(" ", "");

                    class_inheritance.Item2.Add(inherited_class);

                }

                foundclasses.Add(class_inheritance);
            }



            return foundclasses;

        }

        string CaseInsenstiveReplace(string originalString, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue,
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(originalString, newValue);
        }

    }
}
