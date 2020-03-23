using System;
using System.Drawing;
using System.Windows.Forms;

namespace UpgradeStubs
{
	public class AxMSComDlg_AxCommonDialog
		: System.Windows.Forms.Control
	{

		public string getFileName()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("MSComDlg.CommonDialog.FileName");
			return "";
		}
		public void setCancelError(bool CancelError)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("MSComDlg.CommonDialog.CancelError");
		}
		public void setDialogTitle(string DialogTitle)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("MSComDlg.CommonDialog.DialogTitle");
		}
		public void setFileName(string FileName)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("MSComDlg.CommonDialog.FileName");
		}
		public void setFilter(string Filter)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("MSComDlg.CommonDialog.Filter");
		}
	} 
	public static class System_Windows_Forms_Control
	{

		public static void Print(this Control instance, string Unnamed)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.Control.Print");
		}
	} 
	public static class System_Windows_Forms_PictureBox
	{

		public static void Circle(this PictureBox instance, float X, float Y, float Radius, int Color, float Start, float End, float Aspect)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.Circle");
		}
		public static void Cls(this PictureBox instance)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.Cls");
		}
		public static int Point(this PictureBox instance, float X, float Y)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.Point");
			return 0;
		}
		public static void PSet(this PictureBox instance, float X, float Y, int Color)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.PSet");
		}
		public static void setDrawStyle(this PictureBox instance, UpgradeStubs.VBRUN_DrawStyleConstantsEnum DrawStyle)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.DrawStyle");
		}
		public static void setDrawWidth(this PictureBox instance, int DrawWidth)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.DrawWidth");
		}
		public static void setFillStyle(this PictureBox instance, UpgradeStubs.VBRUN_FillStyleConstantsEnum FillStyle)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.FillStyle");
		}
		public static void setForeColor(this PictureBox instance, Color ForeColor_Renamed)
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VB.PictureBox.ForeColor");
		}
	} 
	public class VBRUN_DrawStyleConstants
	{

		public static UpgradeStubs.VBRUN_DrawStyleConstantsEnum getvbDashDot()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VBRUN.DrawStyleConstants.vbDashDot");
			return (UpgradeStubs.VBRUN_DrawStyleConstantsEnum) VBRUN_DrawStyleConstantsEnum.vbDashDot;
		}
		public static UpgradeStubs.VBRUN_DrawStyleConstantsEnum getvbDot()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VBRUN.DrawStyleConstants.vbDot");
			return (UpgradeStubs.VBRUN_DrawStyleConstantsEnum) VBRUN_DrawStyleConstantsEnum.vbDot;
		}
		public static UpgradeStubs.VBRUN_DrawStyleConstantsEnum getvbInvisible()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VBRUN.DrawStyleConstants.vbInvisible");
			return (UpgradeStubs.VBRUN_DrawStyleConstantsEnum) VBRUN_DrawStyleConstantsEnum.vbInvisible;
		}
		public static UpgradeStubs.VBRUN_DrawStyleConstantsEnum getvbSolid()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VBRUN.DrawStyleConstants.vbSolid");
			return (UpgradeStubs.VBRUN_DrawStyleConstantsEnum) VBRUN_DrawStyleConstantsEnum.vbSolid;
		}
	} 
	public class VBRUN_FillStyleConstants
	{

		public static UpgradeStubs.VBRUN_FillStyleConstantsEnum getvbFSSolid()
		{
			UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("VBRUN.FillStyleConstants.vbFSSolid");
			return (UpgradeStubs.VBRUN_FillStyleConstantsEnum) VBRUN_FillStyleConstantsEnum.vbFSSolid;
		}
	} 
	public enum stdole_LoadPictureConstantsEnum
	{
		None = -1
	} 
	public enum VBRUN_DrawStyleConstantsEnum
	{
		vbSolid = 0,
		vbDot = 2,
		vbDashDot = 3,
		vbInvisible = 5
	} 
	public enum VBRUN_FillStyleConstantsEnum
	{
		vbFSSolid = 0
	}
}