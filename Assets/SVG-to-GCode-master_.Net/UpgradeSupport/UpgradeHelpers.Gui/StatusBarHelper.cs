using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extension class for the ToolStrip
    /// </summary>
    public static class CollectionKeySupport
    {
        /// <summary>
        /// Use XItems as alternative for the [toolbar_vb6_control].items(key) when searching an element for its key value.
        /// In .Net there's no key value so the tag property could be used instead.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="AccessibleName"></param>
        /// <returns></returns>
        public static ToolStripItem XItems(this ToolStrip instance, string accessibleName)
        {
            ToolStripItem result = null;
            foreach (ToolStripItem ts in instance.Items)
            {
                if (ts.AccessibleName != null && ts.AccessibleName.ToString() == accessibleName)
                {
                    result = ts;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// When an index is used to get the element at the index position.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ToolStripItem XItems(this ToolStrip instance, int index)
        {
            ToolStripItem result = null;
            result = instance.Items[index];
            return result;
        }

        public static ToolStripItemCollection XItems(this ToolStrip instance)
        {
            return instance.Items;
        }
    }

    public class StatusBarHelper
    {
        private static Timer _timer = null;

        static System.Collections.Generic.Dictionary<Form, StatusStrip> _forms;
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="form">Owner form</param>
        /// <param name="datePanel">Date panel</param>
        /// <param name="timePanel">Time panel</param>
        private StatusBarHelper(Form form, StatusStrip statusStrip)
        {
            if (_forms == null)
            {
                _forms = new System.Collections.Generic.Dictionary<Form, StatusStrip>();
            }
            _forms.Add(form, statusStrip);
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = 1;
                _timer.Tick += (sender, e) =>
                {
                    _timer.Enabled = false;
                    DateTime now = DateTime.Now;
                    foreach (System.Collections.Generic.KeyValuePair<Form, StatusStrip> entry in _forms)
                    {
                        StatusStrip statucStripCtrl = entry.Value;
                        ToolStripStatusLabel panelTime = statucStripCtrl.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("5"))).FirstOrDefault();
                        ToolStripStatusLabel panelDate = statucStripCtrl.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("6"))).FirstOrDefault();

                        if (panelTime != null)
                            panelTime.Text = now.ToShortTimeString();

                        if (panelDate != null)
                            panelDate.Text = now.ToShortDateString();


                        HandleSpecialStatusBarCases(statucStripCtrl);

                    }
                    _timer.Interval = 1000 - now.Millisecond; // Se ajusta el intervalo para que se refresque justo cuando cambia el segundo
                                                              //_timer.Interval = 1000;
                    _timer.Enabled = true;
                };
            }

            form.FormClosed += (sender, e) =>
            {
                _forms.Remove(form);
            };
        }
        private static void OnApplicationExit(Object sender, EventArgs eventArgs)
        {
            if (_timer != null)
            {
                _timer.Enabled = false;
                _timer.Dispose();
                _timer = null;
            }
            _forms.Clear();
        }

        /// <summary>
        /// Hooking a form
        /// </summary>
        /// <param name="form">Owner form</param>
        /// <param name="datePanel">Date panel</param>
        /// <param name="timePanel">Time panel</param>
        public static void DoHook(Form form, StatusStrip statusStrip)
        {
            bool firstTime = (_forms == null);
            StatusBarHelper updater = new StatusBarHelper(form, statusStrip);
            _timer.Enabled = true;
            if (firstTime) Application.ApplicationExit += new EventHandler(OnApplicationExit);
        }

        public static void HandleSpecialStatusBarCases(StatusStrip statusStrip1)
        {
            ToolStripStatusLabel panelCaps = statusStrip1.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("1"))).FirstOrDefault();
            ToolStripStatusLabel panelNum = statusStrip1.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("2"))).FirstOrDefault();
            ToolStripStatusLabel panelIns = statusStrip1.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("3"))).FirstOrDefault();
            ToolStripStatusLabel panelScrl = statusStrip1.Items.OfType<ToolStripStatusLabel>().Where(x => (x.Tag != null) && (x.Tag.Equals("4"))).FirstOrDefault();

            if (panelCaps != null)
            {
                if (Control.IsKeyLocked(Keys.CapsLock))
                {
                    panelCaps.Font = new Font(panelCaps.Font.FontFamily, panelCaps.Font.Size, FontStyle.Bold);
                }
                else
                {
                    panelCaps.Font = new Font(panelCaps.Font.FontFamily, panelCaps.Font.Size, FontStyle.Regular);
                }
            }

            if (panelNum != null)
            {
                if (Control.IsKeyLocked(Keys.NumLock))
                {
                    panelNum.Font = new Font(panelNum.Font.FontFamily, panelNum.Font.Size, FontStyle.Bold);
                }
                else
                {
                    panelNum.Font = new Font(panelNum.Font.FontFamily, panelNum.Font.Size, FontStyle.Regular);
                }
            }

            if (panelIns != null)
            {
                if (Control.IsKeyLocked(Keys.Insert))
                {
                    panelIns.Font = new Font(panelIns.Font.FontFamily, panelIns.Font.Size, FontStyle.Bold);
                }
                else
                {
                    panelIns.Font = new Font(panelIns.Font.FontFamily, panelIns.Font.Size, FontStyle.Regular);
                }
            }

            if (panelScrl != null)
            {
                if (Control.IsKeyLocked(Keys.Scroll))
                {
                    panelScrl.Font = new Font(panelScrl.Font.FontFamily, panelScrl.Font.Size, FontStyle.Bold);
                }
                else
                {
                    panelScrl.Font = new Font(panelScrl.Font.FontFamily, panelScrl.Font.Size, FontStyle.Regular);
                }
            }
        }
    }
}
