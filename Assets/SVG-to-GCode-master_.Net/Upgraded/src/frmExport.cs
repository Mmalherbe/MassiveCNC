using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace SVGtoGCODE
{
	internal partial class frmExport
		: System.Windows.Forms.Form
	{

		public frmExport()
			: base()
		{
			if (m_vb6FormDefInstance == null)
			{
				if (m_InitializingDefInstance)
				{
					m_vb6FormDefInstance = this;
				}
				else
				{
					try
					{
						//For the start-up form, the first instance created is the default instance.
						if (System.Reflection.Assembly.GetExecutingAssembly().EntryPoint != null && System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType == this.GetType())
						{
							m_vb6FormDefInstance = this;
						}
					}
					catch
					{
					}
				}
			}
			//This call is required by the Windows Form Designer.
			InitializeComponent();
		}





		private void cmdChoosePath_Click(Object eventSender, EventArgs eventArgs)
		{



			//UPGRADE_ISSUE: (6012) CommonDialog variable was not upgraded More Information: https://www.mobilize.net/vbtonet/ewis/ewi6012

			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.FileName was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.COMDLGSave.FileName = GeneralFunctions.getFolderNameFromPath(frmInterface.DefInstance.COMDLGSave.FileName) + "\\" + GeneralFunctions.getFileNameNoExten(GeneralFunctions.getFileNameFromPath(frmInterface.DefInstance.COMDLGSave.FileName)) + ".ngc";

			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.Filter was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			//UPGRADE_WARNING: (2081) Filter has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
			frmInterface.DefInstance.COMDLGSave.Filter = "GCODE Files (*.ngc)|*.ngc";

			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.DialogTitle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.COMDLGSave.Title = "Export GCODE";
			frmInterface.DefInstance.COMDLGSave.ShowDialog();
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.CancelError was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			frmInterface.DefInstance.COMDLG.setCancelError(false);
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.FileName was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			txtPath.Text = frmInterface.DefInstance.COMDLGSave.FileName;



		}

		private void cmdExport_Click(Object eventSender, EventArgs eventArgs)
		{
			if (txtPath.Text == "")
			{
				MessageBox.Show("Please specify an export path.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (GeneralFunctions.myDir(txtPath.Text) != "")
			{
				//UPGRADE_ISSUE: (1046) MsgBox Parameter 'context' is not supported, and was removed. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1046
				//UPGRADE_ISSUE: (1046) MsgBox Parameter 'helpfile' is not supported, and was removed. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1046
				if (((DialogResult) Interaction.MsgBox("The file already exists.  Overwrite?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "")) != System.Windows.Forms.DialogResult.Yes)
				{
					return;
				}
			}

			// Save everything.
			SetSet("FeedRate", txtFeedRate.Text);
			SetSet("ZPlunge", (GeneralFunctions.FromCheck(chkZPlunge)) ? "Y" : "N");
			SetSet("PPI", (GeneralFunctions.FromCheck(chkPPI)) ? "Y" : "N");
			SetSet("PPI Rate", txtPPI.Text);
			SetSet("Loop", (GeneralFunctions.FromCheck(chkLoop)) ? "Y" : "N");
			SetSet("Loops", txtLoops.Text);
			SetSet("RaiseDist", txtMM.Text);
			SVGParse.LastExportPath = txtPath.Text;


			SVGParse.exportGCODE(txtPath.Text, Conversion.Val(txtFeedRate.Text), GeneralFunctions.FromCheck(chkZPlunge), GeneralFunctions.FromCheck(chkPPI), Convert.ToInt32(Conversion.Val(txtPPI.Text)), GeneralFunctions.FromCheck(chkLoop), Convert.ToInt32(Conversion.Val(txtLoops.Text)), Conversion.Val(txtMM.Text));

			MessageBox.Show("Export complete!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);


		}

		private object SetSet(string Sett, string Value)
		{
			Interaction.SaveSetting("Av's SVG to GCode", "Export", Sett, Value);
			return null;
		}
		private string GetSet(string Sett, string DefaultValue = "")
		{
			return Interaction.GetSetting("Av's SVG to GCode", "Export", Sett, DefaultValue);
		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load method and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
		private void Form_Load()
		{

			txtFeedRate.Text = GetSet("FeedRate", "20");
			if (GetSet("ZPlunge") == "Y")
			{
				chkZPlunge.CheckState = CheckState.Checked;
			}
			if (GetSet("PPI") == "Y")
			{
				chkPPI.CheckState = CheckState.Checked;
			}
			if (GetSet("Loop") == "Y")
			{
				chkLoop.CheckState = CheckState.Checked;
			}
			txtPPI.Text = GetSet("PPI", "111111");
			txtLoops.Text = GetSet("Loops", "6");
			txtMM.Text = GetSet("RaiseDist", "0.5");
			txtPath.Text = SVGParse.LastExportPath;


			if (txtPath.Text == "")
			{
				txtPath.Text = GeneralFunctions.getFolderNameFromPath(SVGParse.CurrentFile) + "\\" + GeneralFunctions.getFileNameNoExten(GeneralFunctions.getFileNameFromPath(SVGParse.CurrentFile)) + ".ngc";

			}

		}
		private void Form_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}