using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    public static class DragAndDropExtensions
    {
        static private Type _lastUsedType = null;
        public static void DoBeginDrag(this Control control)
        {
            _lastUsedType = control.GetType();
            control.DoDragDrop(control, DragDropEffects.All);
        }

        public static Control GetSource(this DragEventArgs args)
        {
            return _lastUsedType != null ? args.Data.GetData(_lastUsedType) as Control : null;
        }
    }
}
