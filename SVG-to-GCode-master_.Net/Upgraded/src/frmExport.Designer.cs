
namespace SVGtoGCODE
{
	partial class frmExport
	{

		#region "Upgrade Support "
		private static frmExport m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmExport DefInstance
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
		public static frmExport CreateInstance()
		{
			frmExport theInstance = new frmExport();
			theInstance.Form_Load();
			return theInstance;
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "cmdExport", "txtMM", "txtLoops", "chkLoop", "Label8", "Label7", "Label6", "Label5", "Frame2", "txtPPI", "chkPPI", "txtFeedRate", "chkZPlunge", "Label4", "Label3", "Label2", "Frame1", "cmdChoosePath", "txtPath", "Label1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.Button cmdExport;
		public System.Windows.Forms.TextBox txtMM;
		public System.Windows.Forms.TextBox txtLoops;
		public System.Windows.Forms.CheckBox chkLoop;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.Label Label6;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.GroupBox Frame2;
		public System.Windows.Forms.TextBox txtPPI;
		public System.Windows.Forms.CheckBox chkPPI;
		public System.Windows.Forms.TextBox txtFeedRate;
		public System.Windows.Forms.CheckBox chkZPlunge;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.Button cmdChoosePath;
		public System.Windows.Forms.TextBox txtPath;
		public System.Windows.Forms.Label Label1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExport));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.cmdExport = new System.Windows.Forms.Button();
			this.Frame2 = new System.Windows.Forms.GroupBox();
			this.txtMM = new System.Windows.Forms.TextBox();
			this.txtLoops = new System.Windows.Forms.TextBox();
			this.chkLoop = new System.Windows.Forms.CheckBox();
			this.Label8 = new System.Windows.Forms.Label();
			this.Label7 = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.txtPPI = new System.Windows.Forms.TextBox();
			this.chkPPI = new System.Windows.Forms.CheckBox();
			this.txtFeedRate = new System.Windows.Forms.TextBox();
			this.chkZPlunge = new System.Windows.Forms.CheckBox();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.cmdChoosePath = new System.Windows.Forms.Button();
			this.txtPath = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.Frame2.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdExport
			// 
			this.cmdExport.AllowDrop = true;
			this.cmdExport.BackColor = System.Drawing.SystemColors.Control;
			this.cmdExport.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cmdExport.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdExport.Location = new System.Drawing.Point(420, 216);
			this.cmdExport.Name = "cmdExport";
			this.cmdExport.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdExport.Size = new System.Drawing.Size(121, 45);
			this.cmdExport.TabIndex = 19;
			this.cmdExport.Text = "Export Now";
			this.cmdExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdExport.UseVisualStyleBackColor = false;
			this.cmdExport.Click += new System.EventHandler(this.cmdExport_Click);
			// 
			// Frame2
			// 
			this.Frame2.AllowDrop = true;
			this.Frame2.BackColor = System.Drawing.SystemColors.Control;
			this.Frame2.Controls.Add(this.txtMM);
			this.Frame2.Controls.Add(this.txtLoops);
			this.Frame2.Controls.Add(this.chkLoop);
			this.Frame2.Controls.Add(this.Label8);
			this.Frame2.Controls.Add(this.Label7);
			this.Frame2.Controls.Add(this.Label6);
			this.Frame2.Controls.Add(this.Label5);
			this.Frame2.Enabled = true;
			this.Frame2.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame2.Location = new System.Drawing.Point(268, 40);
			this.Frame2.Name = "Frame2";
			this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame2.Size = new System.Drawing.Size(273, 169);
			this.Frame2.TabIndex = 8;
			this.Frame2.Text = "Loop Cut while raising table";
			this.Frame2.Visible = true;
			// 
			// txtMM
			// 
			this.txtMM.AcceptsReturn = true;
			this.txtMM.AllowDrop = true;
			this.txtMM.BackColor = System.Drawing.SystemColors.Window;
			this.txtMM.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtMM.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtMM.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtMM.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtMM.Location = new System.Drawing.Point(16, 108);
			this.txtMM.MaxLength = 0;
			this.txtMM.Name = "txtMM";
			this.txtMM.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtMM.Size = new System.Drawing.Size(65, 21);
			this.txtMM.TabIndex = 17;
			this.txtMM.Text = "1";
			// 
			// txtLoops
			// 
			this.txtLoops.AcceptsReturn = true;
			this.txtLoops.AllowDrop = true;
			this.txtLoops.BackColor = System.Drawing.SystemColors.Window;
			this.txtLoops.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtLoops.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtLoops.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtLoops.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtLoops.Location = new System.Drawing.Point(104, 56);
			this.txtLoops.MaxLength = 0;
			this.txtLoops.Name = "txtLoops";
			this.txtLoops.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtLoops.Size = new System.Drawing.Size(65, 21);
			this.txtLoops.TabIndex = 15;
			this.txtLoops.Text = "6";
			// 
			// chkLoop
			// 
			this.chkLoop.AllowDrop = true;
			this.chkLoop.Appearance = System.Windows.Forms.Appearance.Normal;
			this.chkLoop.BackColor = System.Drawing.SystemColors.Control;
			this.chkLoop.CausesValidation = true;
			this.chkLoop.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkLoop.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.chkLoop.Enabled = true;
			this.chkLoop.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chkLoop.ForeColor = System.Drawing.SystemColors.ControlText;
			this.chkLoop.Location = new System.Drawing.Point(16, 40);
			this.chkLoop.Name = "chkLoop";
			this.chkLoop.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkLoop.Size = new System.Drawing.Size(153, 13);
			this.chkLoop.TabIndex = 13;
			this.chkLoop.TabStop = true;
			this.chkLoop.Text = "Perform job multiple times";
			this.chkLoop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkLoop.Visible = true;
			// 
			// Label8
			// 
			this.Label8.AllowDrop = true;
			this.Label8.AutoSize = true;
			this.Label8.BackColor = System.Drawing.SystemColors.Control;
			this.Label8.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label8.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label8.Location = new System.Drawing.Point(88, 112);
			this.Label8.Name = "Label8";
			this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label8.Size = new System.Drawing.Size(16, 13);
			this.Label8.TabIndex = 18;
			this.Label8.Text = "mm";
			// 
			// Label7
			// 
			this.Label7.AllowDrop = true;
			this.Label7.AutoSize = true;
			this.Label7.BackColor = System.Drawing.SystemColors.Control;
			this.Label7.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label7.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label7.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label7.Location = new System.Drawing.Point(16, 88);
			this.Label7.Name = "Label7";
			this.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label7.Size = new System.Drawing.Size(194, 13);
			this.Label7.TabIndex = 16;
			this.Label7.Text = "Raise the bed this much after each loop:";
			// 
			// Label6
			// 
			this.Label6.AllowDrop = true;
			this.Label6.AutoSize = true;
			this.Label6.BackColor = System.Drawing.SystemColors.Control;
			this.Label6.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label6.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label6.Location = new System.Drawing.Point(16, 60);
			this.Label6.Name = "Label6";
			this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label6.Size = new System.Drawing.Size(80, 13);
			this.Label6.TabIndex = 14;
			this.Label6.Text = "This many loops:";
			// 
			// Label5
			// 
			this.Label5.AllowDrop = true;
			this.Label5.AutoSize = true;
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(16, 20);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(207, 13);
			this.Label5.TabIndex = 12;
			this.Label5.Text = "Make it easy to cut through heavy plastics.";
			// 
			// Frame1
			// 
			this.Frame1.AllowDrop = true;
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.txtPPI);
			this.Frame1.Controls.Add(this.chkPPI);
			this.Frame1.Controls.Add(this.txtFeedRate);
			this.Frame1.Controls.Add(this.chkZPlunge);
			this.Frame1.Controls.Add(this.Label4);
			this.Frame1.Controls.Add(this.Label3);
			this.Frame1.Controls.Add(this.Label2);
			this.Frame1.Enabled = true;
			this.Frame1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(12, 40);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(245, 169);
			this.Frame1.TabIndex = 3;
			this.Frame1.Text = "Export Options";
			this.Frame1.Visible = true;
			// 
			// txtPPI
			// 
			this.txtPPI.AcceptsReturn = true;
			this.txtPPI.AllowDrop = true;
			this.txtPPI.BackColor = System.Drawing.SystemColors.Window;
			this.txtPPI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtPPI.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtPPI.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtPPI.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtPPI.Location = new System.Drawing.Point(76, 84);
			this.txtPPI.MaxLength = 0;
			this.txtPPI.Name = "txtPPI";
			this.txtPPI.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtPPI.Size = new System.Drawing.Size(89, 21);
			this.txtPPI.TabIndex = 11;
			this.txtPPI.Text = "111111";
			// 
			// chkPPI
			// 
			this.chkPPI.AllowDrop = true;
			this.chkPPI.Appearance = System.Windows.Forms.Appearance.Normal;
			this.chkPPI.BackColor = System.Drawing.SystemColors.Control;
			this.chkPPI.CausesValidation = true;
			this.chkPPI.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkPPI.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.chkPPI.Enabled = true;
			this.chkPPI.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chkPPI.ForeColor = System.Drawing.SystemColors.ControlText;
			this.chkPPI.Location = new System.Drawing.Point(12, 68);
			this.chkPPI.Name = "chkPPI";
			this.chkPPI.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkPPI.Size = new System.Drawing.Size(141, 13);
			this.chkPPI.TabIndex = 9;
			this.chkPPI.TabStop = true;
			this.chkPPI.Text = "PPI Mode";
			this.chkPPI.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkPPI.Visible = true;
			// 
			// txtFeedRate
			// 
			this.txtFeedRate.AcceptsReturn = true;
			this.txtFeedRate.AllowDrop = true;
			this.txtFeedRate.BackColor = System.Drawing.SystemColors.Window;
			this.txtFeedRate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtFeedRate.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtFeedRate.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtFeedRate.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtFeedRate.Location = new System.Drawing.Point(76, 20);
			this.txtFeedRate.MaxLength = 0;
			this.txtFeedRate.Name = "txtFeedRate";
			this.txtFeedRate.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtFeedRate.Size = new System.Drawing.Size(89, 21);
			this.txtFeedRate.TabIndex = 6;
			this.txtFeedRate.Text = "20";
			// 
			// chkZPlunge
			// 
			this.chkZPlunge.AllowDrop = true;
			this.chkZPlunge.Appearance = System.Windows.Forms.Appearance.Normal;
			this.chkZPlunge.BackColor = System.Drawing.SystemColors.Control;
			this.chkZPlunge.CausesValidation = true;
			this.chkZPlunge.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkZPlunge.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.chkZPlunge.Enabled = true;
			this.chkZPlunge.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.chkZPlunge.ForeColor = System.Drawing.SystemColors.ControlText;
			this.chkZPlunge.Location = new System.Drawing.Point(12, 48);
			this.chkZPlunge.Name = "chkZPlunge";
			this.chkZPlunge.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.chkZPlunge.Size = new System.Drawing.Size(217, 17);
			this.chkZPlunge.TabIndex = 4;
			this.chkZPlunge.TabStop = true;
			this.chkZPlunge.Text = "Z-plunge (for engraver)";
			this.chkZPlunge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.chkZPlunge.Visible = true;
			// 
			// Label4
			// 
			this.Label4.AllowDrop = true;
			this.Label4.AutoSize = true;
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(36, 88);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(20, 13);
			this.Label4.TabIndex = 10;
			this.Label4.Text = "PPI:";
			// 
			// Label3
			// 
			this.Label3.AllowDrop = true;
			this.Label3.AutoSize = true;
			this.Label3.BackColor = System.Drawing.SystemColors.Control;
			this.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label3.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label3.Location = new System.Drawing.Point(172, 24);
			this.Label3.Name = "Label3";
			this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label3.Size = new System.Drawing.Size(28, 13);
			this.Label3.TabIndex = 7;
			this.Label3.Text = "in/min";
			// 
			// Label2
			// 
			this.Label2.AllowDrop = true;
			this.Label2.AutoSize = true;
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(12, 24);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(54, 13);
			this.Label2.TabIndex = 5;
			this.Label2.Text = "Feed Rate:";
			// 
			// cmdChoosePath
			// 
			this.cmdChoosePath.AllowDrop = true;
			this.cmdChoosePath.BackColor = System.Drawing.SystemColors.Control;
			this.cmdChoosePath.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.cmdChoosePath.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdChoosePath.Location = new System.Drawing.Point(500, 8);
			this.cmdChoosePath.Name = "cmdChoosePath";
			this.cmdChoosePath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdChoosePath.Size = new System.Drawing.Size(45, 25);
			this.cmdChoosePath.TabIndex = 2;
			this.cmdChoosePath.Text = "...";
			this.cmdChoosePath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdChoosePath.UseVisualStyleBackColor = false;
			this.cmdChoosePath.Click += new System.EventHandler(this.cmdChoosePath_Click);
			// 
			// txtPath
			// 
			this.txtPath.AcceptsReturn = true;
			this.txtPath.AllowDrop = true;
			this.txtPath.BackColor = System.Drawing.SystemColors.Window;
			this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtPath.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtPath.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtPath.ForeColor = System.Drawing.SystemColors.WindowText;
			this.txtPath.Location = new System.Drawing.Point(84, 8);
			this.txtPath.MaxLength = 0;
			this.txtPath.Name = "txtPath";
			this.txtPath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.txtPath.Size = new System.Drawing.Size(413, 25);
			this.txtPath.TabIndex = 1;
			// 
			// Label1
			// 
			this.Label1.AllowDrop = true;
			this.Label1.AutoSize = true;
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(12, 12);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(61, 13);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Export Path:";
			// 
			// frmExport
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8, 16);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(548, 269);
			this.Controls.Add(this.cmdExport);
			this.Controls.Add(this.Frame2);
			this.Controls.Add(this.Frame1);
			this.Controls.Add(this.cmdChoosePath);
			this.Controls.Add(this.txtPath);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(3, 26);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmExport";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export GCode";
			this.Closed += new System.EventHandler(this.Form_Closed);
			this.Frame2.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}