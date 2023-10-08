using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritanceViewer
{
    public class DgmlWriter
    {
        public DgmlWriter()
        {
            _name_of_file_to_write = Path.GetTempPath() + "inheritance_graph.dgml";
        }

        private string _name_of_file_to_write;

        //Template for a dgml-File with LeftToRight and Sugiyama formation. In case the DGML-Editor extension is not installed to Visual Studio it is mentioned in the opened dgml-file.
        private const string _dgml_template = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<!-- If you see this text the DGML-Editor for Visual Studio might not be installed! Check Extras->Tools and Features->DGML-Editor-->\n<DirectedGraph GraphDirection = \"LeftToRight\" Layout=\"Sugiyama\" Title=\"InheritanceGraph\" xmlns=\"http://schemas.microsoft.com/vs/2009/dgml\">\n</DirectedGraph>";

        public void write_file(List<string> classes,List<Tuple<string , string>> inheritances)
        {
            string classtext_to_insert = write_klassen(classes);
            string inheritancetext_to_insert = write_vererbungen(inheritances);

            int position_to_insert = _dgml_template.IndexOf("</DirectedGraph>");
            string modified_graph= _dgml_template.Insert(position_to_insert, classtext_to_insert + inheritancetext_to_insert);


            //Save input in file
            //TODO: File handling in try catch section
            using (StreamWriter writer = new StreamWriter(_name_of_file_to_write, false))
            {
                {
                    writer.Write(modified_graph);
                }
                writer.Close();
            }
        }
        

        string write_klassen(List<string> klassen)
        {
            string text_to_insert = "<Nodes>\n";

            foreach (var klasse in klassen)
            {
                string modified_name = klasse.Replace("<", "&lt;").Replace(">", "&gt;");

                text_to_insert = text_to_insert + "<Node Id=\"" + modified_name + "\"/>\n";
            }

            text_to_insert = text_to_insert + "</Nodes>\n";
            return text_to_insert;
        }

        string write_vererbungen(List<Tuple<string, string>> vererbungen)
        {
            string text_to_insert = "<Links>\n";

            foreach (var klasse in vererbungen)
            {
                string modifiedClassName = klasse.Item1.Replace("<", "&lt;").Replace(">", "&gt;");
                string modifiedInherited_name = klasse.Item2.Replace("<", "&lt;").Replace(">", "&gt;");
                text_to_insert = text_to_insert + "<Link Source=\"" + modifiedClassName + "\" Target=\""+modifiedInherited_name +"\"/>\n";
            }

            text_to_insert = text_to_insert + "</Links>\n";
            return text_to_insert;
        }


        public void OpenDGMLFileInEditor()
        {
            DocumentOpener Do = new DocumentOpener();
            Do.close_if_file_open(_name_of_file_to_write);
            Do.openfile(_name_of_file_to_write);
        }

    }
}
