using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.VCProjectEngine;
using System.Collections;
using System.IO;
using System.Linq;

namespace InheritanceViewer
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class InheritanceCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4e4df52d-9eca-4898-af2e-b7f1d55c3dde");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private InheritanceCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static InheritanceCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in InheritanceCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new InheritanceCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            //Get name of selected class in active document
            OpenedFileHandler lopened_file_handler = new OpenedFileHandler();
            if (!lopened_file_handler.Success)
            {
                string message = "Extracting the name of the selected class was not successfull";
                string title = "Class name extraction failed!";
                MessageBox.Show(message, title);
            }

            //ProjectFilesFinder lprojectfilefinder = new ProjectFilesFinder();
            List<string> lall_project_files_of_open_document = GetAllProjectFilesOfActiveDocument();


            //Parse all files and build-up a Dictionary
            Inheritancefinder linheritangefinder = new Inheritancefinder();
            var inheritDictionary = linheritangefinder.BuildUpInheritanceDictionary(lall_project_files_of_open_document);

            //Build up the reverse dictionary
            Inheritedbybuilder lbyBuilder = new Inheritedbybuilder();
            var linherited_by_dictionary = lbyBuilder.BuildInheritedByList(inheritDictionary);

            //Build up nodes(classes) and edges (inheritances)
            Graphbuilder lgraphbuilder = new Graphbuilder();
            lgraphbuilder.build_up_graph(inheritDictionary, linherited_by_dictionary, lopened_file_handler.DeclaredClassesInFile);

            //Write the dgml-file by the selected class and open the DGM-File

            DgmlWriter ldgmlwriter = new DgmlWriter();
            ldgmlwriter.write_file(lgraphbuilder.Classes, lgraphbuilder.Inheritances);
            ldgmlwriter.OpenDGMLFileInEditor();
        }

        public List<string> GetAllProjectFilesOfActiveDocument()
        {
            List<string> files = new List<string>();
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            string lactive_filename = "";

            Project lproject = null;
            if (dte != null && dte.ActiveDocument != null)
            {
                Document activeDocument = dte.ActiveDocument;
                ProjectItem projectItem = activeDocument.ProjectItem;

                lactive_filename = activeDocument.FullName;

                if (projectItem != null && projectItem.ContainingProject != null)
                {
                    // Hier ist das zugehörige Projekt zu dem aktiven Dokument.
                    lproject = projectItem.ContainingProject;
                    // Du kannst das "project" Objekt jetzt verwenden, um auf Projekteigenschaften oder andere Informationen zuzugreifen.
                    var test = lproject.Name;

                    files = GetAllFilesInProject(lproject);
                }
                else
                {
                    
                }
            }

            if (lactive_filename.EndsWith(".h") && files.Count == 0)
            {//try get all FilesofactiveDocument with .cpp ending
                lactive_filename = lactive_filename.Replace(".h", ".cpp");

                OpenDocumentHandler ODH = new OpenDocumentHandler();
                ODH.openfile(lactive_filename);
                return GetAllProjectFilesOfActiveDocument();
            }

            FindHeaderForReachCppFile(lproject,ref files);

            return files;
        }

        public void FindHeaderForReachCppFile(Project project,ref List<string> filelist)
        {
            //Diesen Teil hier behalten

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


            //Diesen Teil auslagern.
            HashSet<string> list_include_directories = new HashSet<string>(additionalIncludeDirs.Split(';'));

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
                         if(matchingEntries.Count() == 1)
                        {
                            lfiles_to_add.Add(matchingEntries.First());
                        }
                    }

                }
            }
            filelist.AddRange(lfiles_to_add);
        }


        HashSet<string> find_all_header_files_in_folders(HashSet<string> include_directories)
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

        string find_header_file_in_include_directories(HashSet<string> include_directories, string header_file)
        {
            string header_path = "";
            foreach (var folder in include_directories)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                string[] files = Directory.GetFiles(folder, header_file, SearchOption.AllDirectories);

                if(files.Length > 0)
                {//Header file was found
                    if(header_path != "" || files.Length> 1)
                    {
                        throw new InvalidOperationException("Error: The Header file " + header_file + " was found nemerous times!");
                    }
                    header_path = files[0];
                }
            }
            if(header_path == "")
            {
                throw new InvalidOperationException("Error: The Header file " + header_file + " was not found but expected!");
            }
            return header_path;
        }

        public bool is_in_project(string afilename, List<string> filelist)
        {//Todo: muss noch implementiert werden
            var found_files = filelist.FindAll(str => str.Contains(afilename));
            if(found_files.Count >0)
            {
                return true;
            }
            return false;
        }

        public string extract_name_of_filename_of_cpp_file(string filename)
        {

            int index = filename.LastIndexOf("\\");
            int index_dot = filename.LastIndexOf(".");
            int llenght = filename.Length;
            if (index >= 0)
                filename = filename.Substring(index+1, index_dot-index-1);

            return filename;
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
