
namespace SVGtoGCODE
{
	partial class frmScale
	{

		#region "Upgrade Support "
		private static frmScale m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmScale DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = CreateInstance();
					m_InitializingDefInstance = false;
				}
				return m_vb6FormDefInstance;
			}
			set
			{
				m_vb6FormDefInstance = value;
			}
		}

		#endregion
		#region "Windows Form Designer generated code "
		public static frmScale CreateInstance()
		{
			frmScale theInstance = new frmScale();
			return theInstance;
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "txtScale", "cmdCancel", "cmdApply", "chkAspect", "txtHeight", "txtWidth", "Label5", "Label4", "Label3", "Label2", "Label1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.TextBox txtScale;
		public System.Windows.Forms.Button cmdCancel;
		public System.Windows.Forms.Button cmdApply;
		public System.Windows.Forms.CheckBox chkAspect;
		public System.Windows.Forms.TextBox txtHeight;
		public System.Windows.Forms.TextBox txtWidth;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.Label Label1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScale));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.txtScale = new System.Windows.Forms.TextBox();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.cmdApply = new System.Windows.Forms.Button();
			this.chkAspect = new System.Windows.Forms.CheckBox();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtScale
			// 
			this.txtScale.AcceptsReturn = true;
			this.txtScale.AllowDrop = true;
			this.txtScale.BackColor = System.Drawing.SystemColors.Window;
			this.txtScale.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtScale.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtScale.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtScale.Location = new System.Drawing.Point(84, 72);
			this.txtScale.MaxLength = 0;
			this.txtScale.Name = "txtScale";
			this.txtScale.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtScale.Size = new System.Drawing.Size(89, 21);
			this.txtScale.TabIndex = 7;
			this.txtScale.TextChanged += new System.EventHandler(this.txtScale_TextChanged);
			// 
			// cmdCancel
			// 
			this.cmdCancel.AllowDrop = true;
			this.cmdCancel.BackColor = System.Drawing.SystemColors.Control;
			this.cmdCancel.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdCancel.Location = new System.Drawing.Point(24, 128);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdCancel.Size = new System.Drawing.Size(97, 29);
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdCancel.UseVisualStyleBackColor = false;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// cmdApply
			// 
			this.cmdApply.AllowDrop = true;
			this.cmdApply.BackColor = System.Drawing.SystemColors.Control;
			this.cmdApply.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cmdApply.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdApply.Location = new System.Drawing.Point(140, 128);
			this.cmdApply.Name = "cmdApply";
			this.cmdApply.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdApply.Size = new System.Drawing.Size(97, 29);
			this.cmdApply.TabIndex = 5;
			this.cmdApply.Text = "Apply";
			this.cmdApply.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdApply.UseVisualStyleBackColor = false;
			this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
			// 
			// chkAspect
			// 
			this.chkAspect.AllowDrop = true;
			this.chkAspect.Appearance = System.Windows.Forms.Appearance.Normal;
			this.chkAspect.BackColor = System.Drawing.SystemColors.Control;
			this.chkAspect.CausesValidation = true;
			this.chkAspect.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkAspect.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAspect.Enabled = true;
			this.chkAspect.ForeColor = System.Drawing.SystemColors.ControlText;
			this.chkAspect.Location = new System.Drawing.Point(84, 100);
			this.chkAspect.Name = "chkAspect";
			this.chkAspect.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkAspect.Size = new System.Drawing.Size(125, 17);
			this.chkAspect.TabIndex = 4;
			this.chkAspect.TabStop = true;
			this.chkAspect.Text = "Keep Aspect Ratio";
			this.chkAspect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkAspect.Visible = true;
			// 
			// txtHeight
			// 
			this.txtHeight.AcceptsReturn = true;
			this.txtHeight.AllowDrop = true;
			this.txtHeight.BackColor = System.Drawing.SystemColors.Window;
			this.txtHeight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtHeight.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtHeight.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtHeight.Location = new System.Drawing.Point(84, 44);
			this.txtHeight.MaxLength = 0;
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtHeight.Size = new System.Drawing.Size(89, 21);
			this.txtHeight.TabIndex = 3;
			this.txtHeight.TextChanged += new System.EventHandler(this.txtHeight_TextChanged);
			// 
			// txtWidth
			// 
			this.txtWidth.AcceptsReturn = true;
			this.txtWidth.AllowDrop = true;
			this.txtWidth.BackColor = System.Drawing.SystemColors.Window;
			this.txtWidth.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtWidth.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtWidth.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtWidth.Location = new System.Drawing.Point(84, 16);
			this.txtWidth.MaxLength = 0;
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtWidth.Size = new System.Drawing.Size(89, 21);
			this.txtWidth.TabIndex = 1;
			this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
			// 
			// Label5
			// 
			this.Label5.AllowDrop = true;
			this.Label5.AutoSize = true;
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(180, 48);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(19, 13);
			this.Label5.TabIndex = 10;
			this.Label5.Text = "inch";
			// 
			// Label4
			// 
			this.Label4.AllowDrop = true;
			this.Label4.AutoSize = true;
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(180, 20);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(19, 13);
			this.Label4.TabIndex = 9;
			this.Label4.Text = "inch";
			// 
			// Label3
			// 
			this.Label3.AllowDrop = true;
			this.Label3.BackColor = System.Drawing.SystemColors.Control;
			this.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label3.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label3.Location = new System.Drawing.Point(12, 76);
			this.Label3.Name = "Label3";
			this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label3.Size = new System.Drawing.Size(65, 17);
			this.Label3.TabIndex = 8;
			this.Label3.Text = "Scale:";
			// 
			// Label2
			// 
			this.Label2.AllowDrop = true;
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(12, 48);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(65, 17);
			this.Label2.TabIndex = 2;
			this.Label2.Text = "Height:";
			// 
			// Label1
			// 
			this.Label1.AllowDrop = true;
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(12, 20);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(65, 17);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Width:";
			// 
			// frmScale
			// 
			this.AcceptButton = this.cmdApply;
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8, 16);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(257, 171);
			this.Controls.Add(this.txtScale);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdApply);
			this.Controls.Add(this.chkAspect);
			this.Controls.Add(this.txtHeight);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.Label5);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = (System.Drawing.Icon) resources.GetObject("frmScale.Icon");
			this.Location = new System.Drawing.Point(3, 29);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmScale";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Scale";
			this.Closed += new System.EventHandler(this.Form_Closed);
			this.ResumeLayout(false);
		}
		#endregion
	}
}