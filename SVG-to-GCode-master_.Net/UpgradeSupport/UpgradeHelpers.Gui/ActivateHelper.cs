using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Holds the static field 'myActiveForm' to be used in Activate and Activated event declarations
    /// when MDI forms are being used in the original project.
    /// </summary>
    public class ActivateHelper
    {
        /// <summary>
        /// References the active form when MDI forms are being used to reproduce the same behavior as in VB6 in activation events.
        /// </summary>
        public static Form myActiveForm;
    }
}
