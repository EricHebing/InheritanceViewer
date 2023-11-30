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


        /// <summary>
        /// Finds and extracts C++ class declarations along with their inheritances from the specified file.
        /// </summary>
        /// <param name="afilepath">The path to the file to analyze.</param>
        /// <returns>An instance of InheritanceInformation containing class names and their respective inheritances.</returns>
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
            List<string> class_inheritance = new List<string>();
            //the class of declaration is of type: "xy : public anotherclass, ...{"

            int indexOFInheritanceSeperator = Regex.Match(ClassDeclaration, "[^:]:[^:]").Index;
            if (indexOFInheritanceSeperator <0)
            {//In case of no Inheritance (Split return initial string) return empty list as no inheritances exist
                return class_inheritance;
            }

            //2 is added because the [^:] is the first character of the matched regex
            string inheritances= ClassDeclaration.Substring(indexOFInheritanceSeperator + 2);
            inheritances = inheritances.Replace("{", "");
            inheritances = inheritances.Trim();
            string[] keywords = { "public", "private", "protected" };
            List<string> splitted = inheritances.Split(keywords, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var relation in splitted)
            {
                string inherited_class = relation.Replace(" ", "");
                //get rid of trailing "," seperating multiple inheritances
                inherited_class = inherited_class.TrimEnd(',');
                class_inheritance.Add(inherited_class);
            }

            return class_inheritance;
        }

        private List<string> GetClassDeclarations(string Text)
        {
            Namespacehandler lnamespacehandler = new Namespacehandler();

            List<NamespaceInfo> lnamespaceinfos = lnamespacehandler.GetAllNamespacesInText(Text);

            string classdefs = @"class\s+.+\s*\{";
            Regex RGClassDefs = new Regex(classdefs);

            MatchCollection matchedclassdefs = RGClassDefs.Matches(Text);

            List<string> classes = new List<string>();

            foreach(Match match in matchedclassdefs)
            {
                string classdef = match.Value.Replace("class", "").TrimStart();
                string namespaceadditionforclass = getNamespaceAddition(match.Index, lnamespaceinfos);

                classes.Add(namespaceadditionforclass + classdef);
            }
            return classes;
        }

        private string GetTextOfFile(string afilepath)
        {
            StreamReader reader = new StreamReader(afilepath);
            string filetext = reader.ReadToEnd();
            return filetext;
        }

        private string getNamespaceAddition(int startingposition, List<NamespaceInfo> namespaceinfos)
        {
            string namespaceaddition = "";

            for (int i = 0; i < namespaceinfos.Count; i++)
            {//In case of startingposition is within scope of namespace declaration add name of namespace
                if(namespaceinfos[i].Startpos < startingposition && namespaceinfos[i].Endpos > startingposition)
                {
                    namespaceaddition += namespaceinfos[i].Name;
                }
            }

            return namespaceaddition;
        }
    }
}
