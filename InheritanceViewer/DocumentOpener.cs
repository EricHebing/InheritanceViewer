using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace InheritanceViewer
{
    public class DocumentOpener
    {
        public DocumentOpener()
        {

        }

        public void openfile(string filename)
        {
            IVsUIShellOpenDocument openDoc = Package.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            if (openDoc != null)
            {
                Guid logicalView = VSConstants.LOGVIEWID_Designer;
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp;
                IVsUIHierarchy hierarchy;
                uint itemID;
                IVsWindowFrame frame;

                int hr = openDoc.OpenDocumentViaProject(filename, logicalView, out sp, out hierarchy, out itemID, out frame);

                if (hr == VSConstants.S_OK)
                {
                    frame.Show();
                }
            }
        }

        public void close_if_file_open(string filename)
        {
            IVsUIShellOpenDocument openDoc = Package.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            if (openDoc != null)
            {
                Guid logicalView = VSConstants.LOGVIEWID_Designer;
                IVsUIHierarchy hierarchy;
                uint itemID;
                IVsWindowFrame frame;
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp;
                uint[] targetId = new uint[1];
                int is_open = 0;
                int success = openDoc.IsDocumentOpen(null, 0, filename, logicalView, 0, out hierarchy, targetId, out frame, out is_open);

                if (is_open == 1)
                {
                    frame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
                }
            }
        }
    }
}
