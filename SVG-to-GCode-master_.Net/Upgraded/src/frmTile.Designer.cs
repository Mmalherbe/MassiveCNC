
namespace SVGtoGCODE
{
	partial class frmTile
	{

		#region "Upgrade Support "
		private static frmTile m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmTile DefInstance
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
		public static frmTile CreateInstance()
		{
			frmTile theInstance = new frmTile();
			theInstance.Form_Load();
			return theInstance;
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "cmdGo", "_txtInput_0", "_txtInput_5", "_txtInput_4", "_txtInput_3", "_txtInput_2", "_txtInput_1", "Label6", "Label5", "Label4", "Label3", "Label2", "Label1", "txtInput"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.Button cmdGo;
		private System.Windows.Forms.TextBox _txtInput_0;
		private System.Windows.Forms.TextBox _txtInput_5;
		private System.Windows.Forms.TextBox _txtInput_4;
		private System.Windows.Forms.TextBox _txtInput_3;
		private System.Windows.Forms.TextBox _txtInput_2;
		private System.Windows.Forms.TextBox _txtInput_1;
		public System.Windows.Forms.Label Label6;
		public System.Windows.Forms.Label Label5;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.TextBox[] txtInput = new System.Windows.Forms.TextBox[6];
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTile));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.cmdGo = new System.Windows.Forms.Button();
			this._txtInput_0 = new System.Windows.Forms.TextBox();
			this._txtInput_5 = new System.Windows.Forms.TextBox();
			this._txtInput_4 = new System.Windows.Forms.TextBox();
			this._txtInput_3 = new System.Windows.Forms.TextBox();
			this._txtInput_2 = new System.Windows.Forms.TextBox();
			this._txtInput_1 = new System.Windows.Forms.TextBox();
			this.Label6 = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cmdGo
			// 
			this.cmdGo.AllowDrop = true;
			this.cmdGo.BackColor = System.Drawing.SystemColors.Control;
			this.cmdGo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdGo.Location = new System.Drawing.Point(96, 156);
			this.cmdGo.Name = "cmdGo";
			this.cmdGo.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdGo.Size = new System.Drawing.Size(69, 25);
			this.cmdGo.TabIndex = 12;
			this.cmdGo.Text = "Go";
			this.cmdGo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdGo.UseVisualStyleBackColor = false;
			this.cmdGo.Click += new System.EventHandler(this.cmdGo_Click);
			// 
			// _txtInput_0
			// 
			this._txtInput_0.AcceptsReturn = true;
			this._txtInput_0.AllowDrop = true;
			this._txtInput_0.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_0.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_0.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_0.Location = new System.Drawing.Point(96, 8);
			this._txtInput_0.MaxLength = 0;
			this._txtInput_0.Name = "_txtInput_0";
			this._txtInput_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_0.Size = new System.Drawing.Size(69, 21);
			this._txtInput_0.TabIndex = 11;
			this._txtInput_0.Text = "5";
			// 
			// _txtInput_5
			// 
			this._txtInput_5.AcceptsReturn = true;
			this._txtInput_5.AllowDrop = true;
			this._txtInput_5.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_5.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_5.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_5.Location = new System.Drawing.Point(96, 128);
			this._txtInput_5.MaxLength = 0;
			this._txtInput_5.Name = "_txtInput_5";
			this._txtInput_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_5.Size = new System.Drawing.Size(69, 21);
			this._txtInput_5.TabIndex = 10;
			this._txtInput_5.Text = "0";
			// 
			// _txtInput_4
			// 
			this._txtInput_4.AcceptsReturn = true;
			this._txtInput_4.AllowDrop = true;
			this._txtInput_4.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_4.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_4.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_4.Location = new System.Drawing.Point(96, 104);
			this._txtInput_4.MaxLength = 0;
			this._txtInput_4.Name = "_txtInput_4";
			this._txtInput_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_4.Size = new System.Drawing.Size(69, 21);
			this._txtInput_4.TabIndex = 8;
			this._txtInput_4.Text = "0";
			// 
			// _txtInput_3
			// 
			this._txtInput_3.AcceptsReturn = true;
			this._txtInput_3.AllowDrop = true;
			this._txtInput_3.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_3.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_3.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_3.Location = new System.Drawing.Point(96, 80);
			this._txtInput_3.MaxLength = 0;
			this._txtInput_3.Name = "_txtInput_3";
			this._txtInput_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_3.Size = new System.Drawing.Size(69, 21);
			this._txtInput_3.TabIndex = 6;
			this._txtInput_3.Text = "0.1";
			// 
			// _txtInput_2
			// 
			this._txtInput_2.AcceptsReturn = true;
			this._txtInput_2.AllowDrop = true;
			this._txtInput_2.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_2.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_2.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_2.Location = new System.Drawing.Point(96, 56);
			this._txtInput_2.MaxLength = 0;
			this._txtInput_2.Name = "_txtInput_2";
			this._txtInput_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_2.Size = new System.Drawing.Size(69, 21);
			this._txtInput_2.TabIndex = 4;
			this._txtInput_2.Text = "0.1";
			// 
			// _txtInput_1
			// 
			this._txtInput_1.AcceptsReturn = true;
			this._txtInput_1.AllowDrop = true;
			this._txtInput_1.BackColor = System.Drawing.SystemColors.Window;
			this._txtInput_1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._txtInput_1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this._txtInput_1.ForeColor = System.Drawing.SystemColors.WindowText;
			this._txtInput_1.Location = new System.Drawing.Point(96, 32);
			this._txtInput_1.MaxLength = 0;
			this._txtInput_1.Name = "_txtInput_1";
			this._txtInput_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._txtInput_1.Size = new System.Drawing.Size(69, 21);
			this._txtInput_1.TabIndex = 2;
			this._txtInput_1.Text = "5";
			// 
			// Label6
			// 
			this.Label6.AllowDrop = true;
			this.Label6.BackColor = System.Drawing.SystemColors.Control;
			this.Label6.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label6.Location = new System.Drawing.Point(8, 132);
			this.Label6.Name = "Label6";
			this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label6.Size = new System.Drawing.Size(77, 17);
			this.Label6.TabIndex = 9;
			this.Label6.Text = "Column Offset";
			// 
			// Label5
			// 
			this.Label5.AllowDrop = true;
			this.Label5.BackColor = System.Drawing.SystemColors.Control;
			this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label5.Location = new System.Drawing.Point(8, 108);
			this.Label5.Name = "Label5";
			this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label5.Size = new System.Drawing.Size(77, 17);
			this.Label5.TabIndex = 7;
			this.Label5.Text = "Row Offset";
			// 
			// Label4
			// 
			this.Label4.AllowDrop = true;
			this.Label4.BackColor = System.Drawing.SystemColors.Control;
			this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label4.Location = new System.Drawing.Point(8, 84);
			this.Label4.Name = "Label4";
			this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label4.Size = new System.Drawing.Size(77, 17);
			this.Label4.TabIndex = 5;
			this.Label4.Text = "Height Gap";
			// 
			// Label3
			// 
			this.Label3.AllowDrop = true;
			this.Label3.BackColor = System.Drawing.SystemColors.Control;
			this.Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label3.Location = new System.Drawing.Point(8, 60);
			this.Label3.Name = "Label3";
			this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label3.Size = new System.Drawing.Size(77, 17);
			this.Label3.TabIndex = 3;
			this.Label3.Text = "Width Gap";
			// 
			// Label2
			// 
			this.Label2.AllowDrop = true;
			this.Label2.BackColor = System.Drawing.SystemColors.Control;
			this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label2.Location = new System.Drawing.Point(8, 36);
			this.Label2.Name = "Label2";
			this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label2.Size = new System.Drawing.Size(77, 17);
			this.Label2.TabIndex = 1;
			this.Label2.Text = "Columns (Width)";
			// 
			// Label1
			// 
			this.Label1.AllowDrop = true;
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(8, 12);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(77, 17);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Rows (Height)";
			// 
			// frmTile
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8, 16);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(171, 185);
			this.Controls.Add(this.cmdGo);
			this.Controls.Add(this._txtInput_0);
			this.Controls.Add(this._txtInput_5);
			this.Controls.Add(this._txtInput_4);
			this.Controls.Add(this._txtInput_3);
			this.Controls.Add(this._txtInput_2);
			this.Controls.Add(this._txtInput_1);
			this.Controls.Add(this.Label6);
			this.Controls.Add(this.Label5);
			this.Controls.Add(this.Label4);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Location = new System.Drawing.Point(3, 21);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTile";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ShowInTaskbar = false;
			this.Text = "Tile";
			this.Closed += new System.EventHandler(this.Form_Closed);
			this.ResumeLayout(false);
		}
		void ReLoadForm(bool addEvents)
		{
			InitializetxtInput();
		}
		void InitializetxtInput()
		{
			this.txtInput = new System.Windows.Forms.TextBox[6];
			this.txtInput[0] = _txtInput_0;
			this.txtInput[5] = _txtInput_5;
			this.txtInput[4] = _txtInput_4;
			this.txtInput[3] = _txtInput_3;
			this.txtInput[2] = _txtInput_2;
			this.txtInput[1] = _txtInput_1;
		}
		#endregion
	}
}