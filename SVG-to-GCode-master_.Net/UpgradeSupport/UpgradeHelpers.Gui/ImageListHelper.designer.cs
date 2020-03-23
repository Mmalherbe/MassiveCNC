namespace UpgradeHelpers.Gui
{
    public partial class ImageListHelper
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageListHelper));
            this.internalImageList = new System.Windows.Forms.ImageList(this.components);
            this.errorImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // internalImageList
            // 
            this.internalImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.internalImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.internalImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // errorImageList
            // 
            this.errorImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorImageList.ImageStream")));
            this.errorImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.errorImageList.Images.SetKeyName(0, "Error.ICO");
            // 
            // ImageListHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(40, 40);
            this.MinimumSize = new System.Drawing.Size(40, 40);
            this.Name = "ImageListHelper";
            this.Size = new System.Drawing.Size(38, 38);
            this.VisibleChanged += new System.EventHandler(this.ImageListHelper_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary> 
        /// Stores VB6 image list into a .NET image list
        /// </summary>
        private System.Windows.Forms.ImageList internalImageList;
        /// <summary> 
        /// Stores VB6 error image list into a .NET image list
        /// </summary>
        private System.Windows.Forms.ImageList errorImageList;
    }
}
