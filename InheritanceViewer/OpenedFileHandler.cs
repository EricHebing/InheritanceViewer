using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace InheritanceViewer
{
    public class OpenedFileHandler
    {
        string _OpenedFileText;

        //Opening File and Parsing Class Declarations without an error.
        bool _Success = false;
        
        List<string> _declaredClassesInFile;

        public bool Success
        {
            get { return _Success; }
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

                IVsTextManager textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
                IVsTextView textView = null;
                textManager.GetActiveView(1, null, out textView);

                var userData = textView as IVsUserData;

                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                object holder;
                userData.GetData(ref guidViewHost, out holder);
                IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;

                _OpenedFileText = viewHost.TextView.TextSnapshot.GetText();

                ParseClassesInFile();
            }
            catch(Exception e)
            {
                _Success = false;
                return;
            }
            _Success = true;
        }

        private void ParseClassesInFile()
        {
            //Lookbehind a "class" and non-capturing whitespaces. Capture all characters until next whitespace or ":" or "{"
            const string class_name_regex = @"(?<=class) *[^\s:};]* *(?=:|[ \t\n]*{|{)";

            MatchCollection DeclaredClasses = Regex.Matches(_OpenedFileText, class_name_regex);
            _declaredClassesInFile = DeclaredClasses.Cast<Match>().Select(m => m.Value.Trim()).ToList();
        }

    }
}
