using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace InheritanceViewer
{
    public class OpenedFileHandler
    {
        string _name_of_selected_class;
        string _opened_file_text;
        bool _success = false;

        List<string> _declaredClassesInFile;

        public bool Success
        {
            get { return _success; }   // get method
        }

        public string Name_of_selected_class
        {
            get { return _name_of_selected_class; } // get method
        }

        public List<string> DeclaredClassesInFile
        {
            get { return _declaredClassesInFile; }
        }

        public OpenedFileHandler()
        {
            try
            {
                var document = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;

                var adcitveEditor = document.SelectedItems;

                IVsTextManager textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
                IVsTextView textView = null;
                textManager.GetActiveView(1, null, out textView);

                var userData = textView as IVsUserData;

                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                object holder;
                userData.GetData(ref guidViewHost, out holder);
                IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;
                _opened_file_text = viewHost.TextView.TextSnapshot.GetText();
                var position_of_cursor = viewHost.TextView.Selection.ActivePoint.Position;

                var test_include = viewHost.TextView.TextSnapshot.GetLineNumberFromPosition(position_of_cursor.Position);

                var currentLine = viewHost.TextView.TextSnapshot.GetLineFromLineNumber(test_include);
                string lineText = currentLine.GetText().TrimStart();
                _name_of_selected_class = get_class_name_by_line(lineText);
                ParseClassesInFile();

            }
            catch(Exception e)
            {
                _success = false;
                return;
            }
            _success = true;
        }

        static public string get_class_name_by_line(string aline)
        {
            //Lookbehind a "class" and non-capturing whitespaces. Capture all characters until next whitespace or ":" or "{"
            string class_name_regex = @"(?<=class)(?: *)[^\s:}]*";

            Regex rg = new Regex(class_name_regex);

            Match matchedclassdefs = rg.Match(aline);
            if (matchedclassdefs.Success)
            {
                string class_name = matchedclassdefs.Value.Trim();
                return class_name;
            }
            else
            {
                return "";
            }
            
        }

        private void ParseClassesInFile()
        {
            //Lookbehind a "class" and non-capturing whitespaces. Capture all characters until next whitespace or ":" or "{"
            string class_name_regex = @"(?<=class)(?: *)[^\s:}]*";

            MatchCollection DeclaredClasses = Regex.Matches(_opened_file_text, class_name_regex);
            _declaredClassesInFile = DeclaredClasses.Cast<Match>().Select(m => m.Value).ToList();
        }

    }
}
