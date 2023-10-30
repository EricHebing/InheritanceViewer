using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace InheritanceViewer
{
    using InheritanceInformation = Dictionary<string, List<string>>;

    public class Inheritancefinder
    {
        //Standardkonstruktor
        public Inheritancefinder()
        {

        }

        //Each file of the given list is searched for class declarations. Classes and inheritance informations retrieved are returned in a dictionary
        public InheritanceInformation BuildUpInheritanceDictionary(List<string> listoffiles)
        {
            InheritanceInformation linheritancedictionary = new InheritanceInformation();

            foreach (var file in listoffiles)
            {
                var classes_of_file = Findclassesandinheritance(file);
                foreach (var class_info in classes_of_file)
                {
                    if (!linheritancedictionary.ContainsKey(class_info.Key))
                    {
                        linheritancedictionary.Add(class_info.Key, new List<string>());
                    }
                    linheritancedictionary[class_info.Key].AddRange(class_info.Value);
                }
            }

            return linheritancedictionary;
        }


        //
        public InheritanceInformation Findclassesandinheritance(string afilepath)
        {
            InheritanceInformation foundclasses = new InheritanceInformation();

            if (!File.Exists(afilepath))
            {//check if file exists
                return foundclasses;
            }
            string filetext = GetTextOfFile(afilepath);

            CommentRemover ComRemover = new CommentRemover();
            string FiletextWithoutComments = ComRemover.RemoveCommentsInString(filetext);

            List<string> ClassDeclarations = GetClassDeclarations(FiletextWithoutComments);

            foreach (var lclass in ClassDeclarations)
            {//Find all Inheritances
                List<string> Inheritances = GetInheritancesOfClass(lclass);
                string ClassName = GetClassNameByDeclaration(lclass);
                foundclasses[ClassName] = Inheritances;
            }

            return foundclasses;

        }

        private string GetClassNameByDeclaration(string ClassDeclaration)
        {
            string classnamepattern = @"(?<=class) *.*?(?=:|{)";
            string class_name = Regex.Match(ClassDeclaration, classnamepattern).Value;
            class_name = class_name.Trim();
            return class_name;
        }

        private List<string> GetInheritancesOfClass(string ClassDeclaration)
        {
            //the class of declaration is of type: "class xy : public anotherclass, ...{"
            ClassDeclaration = ClassDeclaration.Substring(ClassDeclaration.IndexOf(':') + 1);
            ClassDeclaration = ClassDeclaration.Replace("{", "");
            ClassDeclaration = ClassDeclaration.Trim();
            string[] keywords = { "public", "private", "protected" };
            List<string> splitted = ClassDeclaration.Split(keywords, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<string> class_inheritance = new List<string>();

            foreach (var relation in splitted)
            {
                string inherited_class = relation.Replace(" ", "");
                class_inheritance.Add(inherited_class);
            }

            return class_inheritance;
        }

        private List<string> GetClassDeclarations(string Text)
        {
            string classdefs = @"class\s+.+\s*\{";
            Regex RGClassDefs = new Regex(classdefs);

            MatchCollection matchedclassdefs = RGClassDefs.Matches(Text);
            List<string> classes = matchedclassdefs.Cast<Match>().Select(m => m.Value).ToList();
            return classes;
        }

        private string GetTextOfFile(string afilepath)
        {
            StreamReader reader = new StreamReader(afilepath);
            string filetext = reader.ReadToEnd();
            return filetext;
        }

        //Replaces all occurences of <oldvalue> within the originalString by <newValue>
        string CaseInsensitiveReplace(string originalString, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(originalString, newValue);
        }

    }
}
