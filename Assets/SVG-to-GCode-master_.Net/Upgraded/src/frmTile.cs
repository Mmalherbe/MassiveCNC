using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace SVGtoGCODE
{
	internal partial class frmTile
		: System.Windows.Forms.Form
	{

		public frmTile()
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
			ReLoadForm(false);
		}


		private void cmdGo_Click(Object eventSender, EventArgs eventArgs)
		{

			// Save
			int tempForEndVar = txtInput.GetUpperBound(0);
			for (int i = 0; i <= tempForEndVar; i++)
			{
				Interaction.SaveSetting("SVG to GCODE", "Tile", i.ToString(), txtInput[i].Text);
			}

			frmInterface.DefInstance.goTile(Convert.ToInt32(Conversion.Val(txtInput[0].Text)), Convert.ToInt32(Conversion.Val(txtInput[1].Text)), Conversion.Val(txtInput[2].Text), Conversion.Val(txtInput[3].Text), Conversion.Val(txtInput[4].Text), Conversion.Val(txtInput[5].Text));


		}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load method and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
		private void Form_Load()
		{
			// Load last values
			string AA = "";
			int tempForEndVar = txtInput.GetUpperBound(0);
			for (int i = 0; i <= tempForEndVar; i++)
			{
				AA = Interaction.GetSetting("SVG to GCODE", "Tile", i.ToString(), "");
				if (AA != "")
				{
					txtInput[i].Text = AA;
				}
			}
		}
		private void Form_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}