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

        private string _dgml_template = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<!-- If you see this text the DGML-Editor for Visual Studio might not be installed! Check Extras->Tools and Features->DGML-Editor-->\n<DirectedGraph GraphDirection = \"LeftToRight\" Layout=\"Sugiyama\" Title=\"InheritanceGraph\" xmlns=\"http://schemas.microsoft.com/vs/2009/dgml\">\n</DirectedGraph>";

        public void write_file(List<string> klassen,List<Tuple<string , string>> vererbungen)
        {
            string classtext_to_insert = write_klassen(klassen);
            string inheritancetext_to_insert = write_vererbungen(vererbungen);

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
                text_to_insert = text_to_insert + "<Node Id=\"" + klasse + "\"/>\n";
            }

            text_to_insert = text_to_insert + "</Nodes>\n";
            return text_to_insert;
        }

        string write_vererbungen(List<Tuple<string, string>> vererbungen)
        {
            string text_to_insert = "<Links>\n";

            foreach (var klasse in vererbungen)
            {
                text_to_insert = text_to_insert + "<Link Source=\"" + klasse.Item1 + "\" Target=\""+klasse.Item2 +"\"/>\n";
            }

            text_to_insert = text_to_insert + "</Links>\n";
            return text_to_insert;
        }


        public void OpenDGMLFileInEditor()
        {
            OpenDocumentHandler ODH = new OpenDocumentHandler();
            ODH.close_if_file_open(_name_of_file_to_write);
            ODH.openfile(_name_of_file_to_write);
        }

    }
}
