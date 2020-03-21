using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;

namespace SVGtoGCODE
{
	internal partial class frmScale
		: System.Windows.Forms.Form
	{

		public bool updatingValue = false;
		public double originalAspect = 0;

		public double setW = 0;
		public double setH = 0;
		public double originalW = 0;
		public double originalH = 0;
		public frmScale()
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




		private void cmdApply_Click(Object eventSender, EventArgs eventArgs)
		{
			setW = Conversion.Val(txtWidth.Text);
			setH = Conversion.Val(txtHeight.Text);

			this.Close();
		}

		private void cmdCancel_Click(Object eventSender, EventArgs eventArgs)
		{
			this.Close();


		}

		private void txtHeight_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			if (updatingValue)
			{
				return;
			}

			// Calculate the new wodtj
			if (chkAspect.CheckState == CheckState.Checked)
			{
				updatingValue = true;
				txtWidth.Text = Math.Round((double) (Conversion.Val(txtHeight.Text) * originalAspect), 5).ToString();
				txtScale.Text = Math.Round((double) (Conversion.Val(txtWidth.Text) / originalW * 100), 2).ToString();
				updatingValue = false;


			}

		}

		private void txtWidth_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			if (updatingValue)
			{
				return;
			}

			// Calculate the new height
			if (chkAspect.CheckState == CheckState.Checked)
			{
				updatingValue = true;
				txtHeight.Text = Math.Round((double) (Conversion.Val(txtWidth.Text) / originalAspect), 5).ToString();
				txtScale.Text = Math.Round((double) (Conversion.Val(txtWidth.Text) / originalW * 100), 2).ToString();
				updatingValue = false;


			}

		}


		private void txtScale_TextChanged(Object eventSender, EventArgs eventArgs)
		{
			if (updatingValue)
			{
				return;
			}

			// Calculate the new height
			updatingValue = true;
			txtHeight.Text = Math.Round((double) (originalH * (Conversion.Val(txtScale.Text) / 100)), 5).ToString();
			txtWidth.Text = Math.Round((double) (originalW * (Conversion.Val(txtScale.Text) / 100)), 5).ToString();
			updatingValue = false;



		}
		private void Form_Closed(Object eventSender, EventArgs eventArgs)
		{
		}
	}
}