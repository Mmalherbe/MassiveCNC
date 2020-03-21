using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    public class LabelHelper : Label
    {
        private bool _AutoSize;
        private const double dpiDefault = 96.0;

        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }

            set
            {
                this._AutoSize = value;
                base.AutoSize = false;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                double width;
                double height;
                using (var g = this.CreateGraphics())
                {
                    height = g.MeasureString(value, this.Font).Height;
                    width = g.MeasureString(value, this.Font).Width + this.Padding.Left + this.Padding.Right + this.Margin.Left + this.Margin.Right;
                    width = width * g.DpiX / dpiDefault;
                }
                if (this.Parent != null) this.Parent.SuspendLayout();
                this.Left = this.Right - (int)width;
                this.Width = (int)width;
                this.Height = (int)height;
                if (this.Parent != null) this.Parent.ResumeLayout();
                base.Text = value;

            }
        }
    }
}
