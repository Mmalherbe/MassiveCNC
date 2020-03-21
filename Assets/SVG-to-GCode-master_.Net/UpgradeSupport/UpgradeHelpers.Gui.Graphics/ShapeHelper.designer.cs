namespace UpgradeHelpers.Gui
{
    public partial class ShapeHelper
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (shapePen != null)
                {
                    shapePen.Dispose();
                }
                if (_shapeBrush != null)
                {
                    _shapeBrush.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ShapeHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ShapeHelper";
            this.Resize += new System.EventHandler(this.ShapeHelper_Resize);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ShapeHelper_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
