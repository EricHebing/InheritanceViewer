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
            //Build up informations of opened File.

            //Get all files of the project of the opened file
            OpenedFileHandler op = new OpenedFileHandler();
            if (op.Success == false)
            {//Show an error-message
                //TODO:
            }

            ProjectFilesFinder lprojectfilefinder = new ProjectFilesFinder();
            List<string> lall_project_files_of_open_document = lprojectfilefinder.GetAllProjectFilesOfActiveDocument();


            //Parse all files and build-up a Dictionary
            Inheritancefinder linheritangefinder = new Inheritancefinder();
            var inheritDictionary = linheritangefinder.BuildUpInheritanceDictionary(lall_project_files_of_open_document);


            //Get name of selected class in active document
            OpenedFileHandler lopened_file_handler = new OpenedFileHandler();
            if(!lopened_file_handler.Success)
            {
                string message = "Extracting the name of the selected class was not successfull";
                string title = "Class name extraction failed!";
                MessageBox.Show(message, title);
            }
            
            //Build up the reverse dictionary
            Inheritedbybuilder lbyBuilder = new Inheritedbybuilder();
            var linherited_by_dictionary = lbyBuilder.BuildInheritedByList(inheritDictionary);

            //Build up nodes(classes) and edges (inheritances)
            Graphbuilder lgraphbuilder = new Graphbuilder();
            lgraphbuilder.build_up_graph(inheritDictionary, linherited_by_dictionary, lopened_file_handler.Name_of_selected_class);

            //Write the dgml-file by the selected class and open the DGM-File

            DgmlWriter ldgmlwriter = new DgmlWriter();
            ldgmlwriter.write_file(lgraphbuilder.Classes, lgraphbuilder.Inheritances);

            ldgmlwriter.OpenDGMLFileInEditor();
        }

        private string get_selected_class()
        {//Soll nur temporär als Klassenauswähler verwendet werden.
            return "DerivedClass2";
        }
    }
}
