
namespace SVGtoGCODE
{
	partial class frmInterface
	{

		#region "Upgrade Support "
		private static frmInterface m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmInterface DefInstance
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
		public static frmInterface CreateInstance()
		{
			frmInterface theInstance = new frmInterface();
			theInstance.Form_Load();
			return theInstance;
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "Picture2", "_picRulers_1", "_picRulers_0", "COMDLGOpen", "COMDLGSave", "COMDLG", "List1", "Picture1", "cReBar1", "TB1", "vbalImageList1", "picRulers", "listBoxHelper1"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.PictureBox Picture2;
		private System.Windows.Forms.PictureBox _picRulers_1;
		private System.Windows.Forms.PictureBox _picRulers_0;
		public System.Windows.Forms.OpenFileDialog COMDLGOpen;
		public System.Windows.Forms.SaveFileDialog COMDLGSave;
		public UpgradeStubs.AxMSComDlg_AxCommonDialog COMDLG;
		public System.Windows.Forms.ListBox List1;
		public System.Windows.Forms.PictureBox Picture1;
		public AxvbalTBar6.AxcReBar cReBar1;
		public AxvbalTBar6.AxcToolbar TB1;
		public AxvbalIml6.AxvbalImageList vbalImageList1;
		public System.Windows.Forms.PictureBox[] picRulers = new System.Windows.Forms.PictureBox[2];
		private UpgradeHelpers.Gui.ListBoxHelper listBoxHelper1;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInterface));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.Picture2 = new System.Windows.Forms.PictureBox();
			this._picRulers_1 = new System.Windows.Forms.PictureBox();
			this._picRulers_0 = new System.Windows.Forms.PictureBox();
			this.COMDLGOpen = new System.Windows.Forms.OpenFileDialog();
			this.COMDLGSave = new System.Windows.Forms.SaveFileDialog();
			this.COMDLG = new UpgradeStubs.AxMSComDlg_AxCommonDialog();
			this.List1 = new System.Windows.Forms.ListBox();
			this.Picture1 = new System.Windows.Forms.PictureBox();
			this.cReBar1 = new AxvbalTBar6.AxcReBar();
			this.TB1 = new AxvbalTBar6.AxcToolbar();
			this.vbalImageList1 = new AxvbalIml6.AxvbalImageList();
			((System.ComponentModel.ISupportInitialize) this.cReBar1).BeginInit();
			((System.ComponentModel.ISupportInitialize) this.TB1).BeginInit();
			((System.ComponentModel.ISupportInitialize) this.vbalImageList1).BeginInit();
			this.SuspendLayout();
			this.listBoxHelper1 = new UpgradeHelpers.Gui.ListBoxHelper(this.components);
			// 
			// Picture2
			// 
			this.Picture2.AllowDrop = true;
			this.Picture2.BackColor = System.Drawing.SystemColors.Control;
			this.Picture2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Picture2.CausesValidation = true;
			this.Picture2.Dock = System.Windows.Forms.DockStyle.None;
			this.Picture2.Enabled = true;
			this.Picture2.Location = new System.Drawing.Point(44, 4000);
			this.Picture2.Name = "Picture2";
			this.Picture2.Size = new System.Drawing.Size(257, 181);
			this.Picture2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.Picture2.TabIndex = 4;
			this.Picture2.TabStop = true;
			this.Picture2.Visible = true;
			// 
			// _picRulers_1
			// 
			this._picRulers_1.AllowDrop = true;
			this._picRulers_1.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
			this._picRulers_1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._picRulers_1.CausesValidation = true;
			this._picRulers_1.Dock = System.Windows.Forms.DockStyle.None;
			this._picRulers_1.Enabled = true;
			this._picRulers_1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this._picRulers_1.Location = new System.Drawing.Point(0, 56);
			this._picRulers_1.Name = "_picRulers_1";
			this._picRulers_1.Size = new System.Drawing.Size(20, 280);
			this._picRulers_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
			this._picRulers_1.TabIndex = 3;
			this._picRulers_1.TabStop = true;
			this._picRulers_1.Visible = true;
			// 
			// _picRulers_0
			// 
			this._picRulers_0.AllowDrop = true;
			this._picRulers_0.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
			this._picRulers_0.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._picRulers_0.CausesValidation = true;
			this._picRulers_0.Dock = System.Windows.Forms.DockStyle.None;
			this._picRulers_0.Enabled = true;
			this._picRulers_0.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this._picRulers_0.Location = new System.Drawing.Point(20, 36);
			this._picRulers_0.Name = "_picRulers_0";
			this._picRulers_0.Size = new System.Drawing.Size(153, 20);
			this._picRulers_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
			this._picRulers_0.TabIndex = 2;
			this._picRulers_0.TabStop = true;
			this._picRulers_0.Visible = true;
			// 
			// List1
			// 
			this.List1.AllowDrop = true;
			this.List1.BackColor = System.Drawing.SystemColors.Window;
			this.List1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.List1.CausesValidation = true;
			this.List1.Enabled = true;
			this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
			this.List1.IntegralHeight = true;
			this.List1.Location = new System.Drawing.Point(764, 56);
			this.List1.MultiColumn = false;
			this.List1.Name = "List1";
			this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.List1.Size = new System.Drawing.Size(189, 423);
			this.List1.Sorted = false;
			this.List1.TabIndex = 1;
			this.List1.TabStop = true;
			this.List1.Visible = true;
			this.List1.DoubleClick += new System.EventHandler(this.List1_DoubleClick);
			this.List1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.List1_KeyDown);
			this.List1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.List1_MouseDown);
			this.List1.SelectedIndexChanged += new System.EventHandler(this.List1_SelectedIndexChanged);
			// 
			// Picture1
			// 
			this.Picture1.AllowDrop = true;
			this.Picture1.BackColor = System.Drawing.Color.White;
			this.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Picture1.CausesValidation = true;
			this.Picture1.Dock = System.Windows.Forms.DockStyle.None;
			this.Picture1.Enabled = true;
			this.Picture1.Location = new System.Drawing.Point(20, 56);
			this.Picture1.Name = "Picture1";
			this.Picture1.Size = new System.Drawing.Size(741, 521);
			this.Picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
			this.Picture1.TabIndex = 0;
			this.Picture1.TabStop = true;
			this.Picture1.Visible = true;
			this.Picture1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Picture1_MouseDown);
			this.Picture1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Picture1_MouseMove);
			this.Picture1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Picture1_MouseUp);
			// 
			// cReBar1
			// 
			this.cReBar1.AllowDrop = true;
			this.cReBar1.Location = new System.Drawing.Point(0, 0);
			this.cReBar1.Name = "cReBar1";
			this.cReBar1.OcxState = (System.Windows.Forms.AxHost.State) resources.GetObject("cReBar1.OcxState");
			// 
			// TB1
			// 
			this.TB1.AllowDrop = true;
			this.TB1.Location = new System.Drawing.Point(200, 0);
			this.TB1.Name = "TB1";
			this.TB1.OcxState = (System.Windows.Forms.AxHost.State) resources.GetObject("TB1.OcxState");
			this.TB1.Size = new System.Drawing.Size(153, 29);
			this.TB1.ButtonClick += new AxvbalTBar6.__cToolbar_ButtonClickEventHandler(this.TB1_ButtonClick);
			// 
			// vbalImageList1
			// 
			this.vbalImageList1.AllowDrop = true;
			this.vbalImageList1.Location = new System.Drawing.Point(64, 628);
			this.vbalImageList1.Name = "vbalImageList1";
			this.vbalImageList1.OcxState = (System.Windows.Forms.AxHost.State) resources.GetObject("vbalImageList1.OcxState");
			// 
			// frmInterface
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8, 16);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(1287, 734);
			this.Controls.Add(this.Picture2);
			this.Controls.Add(this._picRulers_1);
			this.Controls.Add(this._picRulers_0);
			this.Controls.Add(this.List1);
			this.Controls.Add(this.Picture1);
			this.Controls.Add(this.cReBar1);
			this.Controls.Add(this.TB1);
			this.Controls.Add(this.vbalImageList1);
			this.Icon = (System.Drawing.Icon) resources.GetObject("frmInterface.Icon");
			this.Location = new System.Drawing.Point(11, 37);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmInterface";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "SVG to GCODE by Avatar-X";
			listBoxHelper1.SetSelectionMode(this.List1, System.Windows.Forms.SelectionMode.MultiExtended);
			this.Closed += new System.EventHandler(this.Form_Closed);
			this.Resize += new System.EventHandler(this.Form_Resize);
			((System.ComponentModel.ISupportInitialize) this.cReBar1).EndInit();
			((System.ComponentModel.ISupportInitialize) this.TB1).EndInit();
			((System.ComponentModel.ISupportInitialize) this.vbalImageList1).EndInit();
			this.ResumeLayout(false);
		}
		void ReLoadForm(bool addEvents)
		{
			InitializepicRulers();
		}
		void InitializepicRulers()
		{
			this.picRulers = new System.Windows.Forms.PictureBox[2];
			this.picRulers[1] = _picRulers_1;
			this.picRulers[0] = _picRulers_0;
		}
		#endregion
	}
}