using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

using Microsoft.VisualStudio.Shell;

using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell.Services;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Linq;
//using Microsoft.VisualStudio.VCProjectEngine;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio;


namespace InheritanceViewer
{
    public class ProjectFilesFinder
    { 
        public ProjectFilesFinder()
        {//Standard constructor

        }

        public List<string> GetAllProjectFilesOfActiveDocument()
        {
            List<string> files = new List<string>(); 
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            if (dte != null && dte.ActiveDocument != null)
            {
                Document activeDocument = dte.ActiveDocument;
                ProjectItem projectItem = activeDocument.ProjectItem;

                if (projectItem != null && projectItem.ContainingProject != null)
                {
                    // Hier ist das zugehörige Projekt zu dem aktiven Dokument.
                    Project project = projectItem.ContainingProject;
                    // Du kannst das "project" Objekt jetzt verwenden, um auf Projekteigenschaften oder andere Informationen zuzugreifen.
                    var test = project.Name;

                    files = GetAllFilesInProject(project);
                }
                else
                {
                    // Active Document is no part of a project or unsaved.
                    //TODO: Show error-message
                }
            }
            return files;
        }

        public List<string> GetAllFilesInProject(Project project)
        {
            List<string> fileList = new List<string>();
            if (project != null)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    // Collect all files recursively
                    GetAllFilesRecursive(item, fileList);
                }
            }
            return fileList;
        }


        private void GetAllFilesRecursive(ProjectItem projectItem, List<string> fileList)
        {
            if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
            {
                // Projectitem is a file
                fileList.Add(projectItem.FileNames[1]);
            }
            else if (projectItem.ProjectItems != null)
            {
                // Porjecitem is a folder
                foreach (ProjectItem item in projectItem.ProjectItems)
                {
                    GetAllFilesRecursive(item, fileList);
                }
            }
        }
    }
}
