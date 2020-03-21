namespace UpgradeHelpers.Helpers
{

    /// <summary>
    /// The NotUpgradedHelper is a miscellaneous class to handle notifications for the not-upgraded members or statements.
    /// Each not-upgraded member generates a stub declaration in the target application.
    /// When one of these stub declarations is invoked the NotUpgradedHelper notifies the missing functionality to the application user.
    /// </summary>
    public class NotUpgradedHelper
    {


        private const string Title = "Not-Upgraded Element";

        private const string Message1 = "The not-upgraded element: '";

        private const string Message2 = "' is being invoked.\n" + "The application behavior might be affected depending on how critical this element is.\n\n" + "Do you want to ignore this issue and continue running the application?\n\n" + "[Yes]    = Ignore this occurrence and continue the program execution\n" + "[No]     = Stop the execution and debug\n" + "[Cancel] = Cancel the not-upgraded element notifications and ignore any potential behavior difference\n";

#if WINFORMS
        private static bool _performNotifications = true;
        private static bool _reporting;

        /// <summary>
        /// Notifies the usage of a not-upgraded element to the user.
        /// <param name="notUpgradedElementDescription">The name of the not-upgraded VB6 member.</param>
        /// <param name="elements">Source code elements that conformed the original statement.</param>
        /// </summary>
        public static void NotifyNotUpgradedElement(string notUpgradedElementDescription, params object[] elements)
        {
            if (_performNotifications && !_reporting)
            {
                _reporting = true;
                System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show(Message1 + notUpgradedElementDescription + Message2, Title + notUpgradedElementDescription, System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                _reporting = false;
                switch (res)
                {
                    case  System.Windows.Forms.DialogResult.Yes:
                        // Do nothing
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        System.Diagnostics.Debugger.Break();
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        _performNotifications = false;
                        break;
                }
            }
        }
#endif

#if WPF
        private static bool _performNotifications = true;
        private static bool _reporting;

        /// <summary>
        /// Notifies the usage of a not-upgraded element to the user.
        /// <param name="notUpgradedElementDescription">The name of the not-upgraded VB6 member.</param>
        /// <param name="elements">Source code elements that conformed the original statement.</param>
        /// </summary>
        public static void NotifyNotUpgradedElement(string notUpgradedElementDescription, params object[] elements)
        {
            if (_performNotifications && !_reporting)
            {
                _reporting = true;
                System.Windows.MessageBoxResult res = System.Windows.MessageBox.Show(Message1 + notUpgradedElementDescription + Message2, Title + notUpgradedElementDescription, System.Windows.MessageBoxButton.YesNoCancel);
                _reporting = false;
                switch (res)
                {
                    case System.Windows.MessageBoxResult.Yes:
                        // Do nothing
                        break;
                    case System.Windows.MessageBoxResult.No:
                        System.Diagnostics.Debugger.Break();
                        break;
                    case System.Windows.MessageBoxResult.Cancel:
                        _performNotifications = false;
                        break;
                }
            }
        }
#endif

    }
}
