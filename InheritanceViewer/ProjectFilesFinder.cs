using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using System.Linq;

namespace InheritanceViewer
{
    public class ProjectFilesFinder
    { 
        public ProjectFilesFinder()
        {
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
                    Project ProjectOfActiveDocument = projectItem.ContainingProject;


                    files = GetAllFilesInProject(ProjectOfActiveDocument);
                }
                else
                {
                    throw new InvalidOperationException("Error: Active File is not part of any Project!");
                }
            }
            return files;
        }

        private List<string> GetAllFilesInProject(Project project)
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

        //Given a ProjectItem adds file to List or searches for files in given folder
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

        public HashSet<string> getIncludeDirectoriesOfProject(Project project)
        {
            string additionalIncludeDirs = "";

            VCProject vcProject = project.Object as VCProject;
            IEnumerable projectConfigurations = vcProject.Configurations as IEnumerable;
            foreach (Object objectProjectConfig in projectConfigurations)
            {
                VCConfiguration vcProjectConfig = objectProjectConfig as VCConfiguration;
                IEnumerable projectTools = vcProjectConfig.Tools as IEnumerable;

                string includeDirs = vcProjectConfig.Evaluate("$(IncludePath)");
                foreach (Object objectProjectTool in projectTools)
                {
                    VCCLCompilerTool compilerTool = objectProjectTool as VCCLCompilerTool;
                    if (compilerTool != null)
                    {
                        additionalIncludeDirs = compilerTool.AdditionalIncludeDirectories;
                        break;
                    }
                }
            }

            HashSet<string> list_include_directories = new HashSet<string>(additionalIncludeDirs.Split(';'));
            return list_include_directories;
        }

        public HashSet<string> find_all_header_files_in_folders(HashSet<string> include_directories)
        {
            HashSet<string> found_files = new HashSet<string>();

            foreach (var folder in include_directories)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                string[] files = Directory.GetFiles(folder, "*.h", SearchOption.AllDirectories);

                found_files.UnionWith(files);
            }

            return found_files;
        }

        public void FindHeaderForReachCppFile(Project project, ref List<string> filelist)
        {

            HashSet<string> list_include_directories = getIncludeDirectoriesOfProject(project);

            HashSet<string> all_header_files = find_all_header_files_in_folders(list_include_directories);

            List<string> lfiles_to_add = new List<string>();
            foreach (var file in filelist)
            {
                if (file.EndsWith(".cpp"))
                {
                    string filename_header = extract_name_of_filename_of_cpp_file(file) + ".h";

                    if (is_in_project(filename_header, filelist))
                    {
                        continue;
                    }
                    else
                    {

                        IEnumerable<string> matchingEntries = all_header_files.Where(entry => entry.EndsWith(filename_header));
                        //string header_path = find_header_file_in_include_directories(list_include_directories, filename_header);
                        if (matchingEntries.Count() == 1)
                        {
                            lfiles_to_add.Add(matchingEntries.First());
                        }
                    }

                }
            }
            filelist.AddRange(lfiles_to_add);
        }

        private bool is_in_project(string afilename, List<string> filelist)
        {
            var found_files = filelist.FindAll(str => str.Contains(afilename));
            if (found_files.Count > 0)
            {
                return true;
            }
            return false;
        }

        private string extract_name_of_filename_of_cpp_file(string filename)
        {
            int index = filename.LastIndexOf("\\");
            int index_dot = filename.LastIndexOf(".");
            if (index >= 0)
                filename = filename.Substring(index + 1, index_dot - index - 1);

            return filename;
        }
    }
}
