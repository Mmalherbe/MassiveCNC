using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extended Version of Scroll Bar(used for UpDown Component Support) 
    /// </summary>
    public class UpDownHelper : VScrollBar
    {

        private Control buddyControl;
        private bool syncBuddy;
        private int increment = 1; //set as default in VB6
        private int max = 10; //set as default in VB6
        private int min = 0; //set as default in VB6
        public Control BuddyControl
        {
            get
            {
                return buddyControl;
            }
            set
            {
                buddyControl = value;
            }
        }
        public bool SyncBuddy
        {
            get
            {
                return syncBuddy;
            }
            set
            {
                syncBuddy = value;
            }
        }
        public int Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value;
            }
        }
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
            }
        }
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
            }
        }
        /// <summary>
        /// Trigger Change value event if the component is SyncBuddy.
        /// </summary>
        /// <param name="sender">The form to be printed.</param>
        /// <param name="e">arg for Scroll</param>
        public static void MyEvent_Scroll(object sender, ScrollEventArgs e)
        {
            if (((UpDownHelper)sender).SyncBuddy)
            {
                ChangeValue(sender, e);
            }
        }

        /// <summary>
        /// Increment/Decrement the value of the UpDown Buddy Control.
        /// </summary>
        /// <param name="sender">The form to be printed.</param>
        /// <param name="e">arg for Scroll</param>
        public static void ChangeValue(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {

                UpDownHelper control = (UpDownHelper)sender;
                int newValue = 0;
                if (control.buddyControl.Text != "")
                {
                    int.TryParse(control.buddyControl.Text, out newValue);
                    if (e.NewValue == 0)
                    {
                        control.buddyControl.Text = newValue + control.Increment <= control.Max ? (newValue + control.Increment).ToString() : newValue.ToString();
                    }
                    else
                    {
                        control.buddyControl.Text = newValue - control.Increment >= control.Min ? (newValue - control.Increment).ToString() : newValue.ToString();

                    }

                }
                else
                {
                    control.buddyControl.Text = e.NewValue == 0 ? control.Increment.ToString() : control.Min.ToString();
                }
                e.NewValue = 0;

            }

        }
    }
}
