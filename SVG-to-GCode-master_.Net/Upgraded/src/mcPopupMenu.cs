using System;
using System.Runtime.InteropServices;
using UpgradeHelpers.Helpers;

namespace SVGtoGCODE
{
	internal class mcPopupMenu
	{

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetLastError();
		// Exposed Enumeration
		public enum mceItemStates
		{
			mceDisabled = 1,
			mceGrayed = 2
		}

		// Property variables
		private string psCaption = ""; // Caption of menu item (with the arrow >) if this is submenu
		private int piHwnd = 0; // Handle to Menu

		// Supporting API code
		private const int GW_CHILD = 5;
		private const int GW_HWNDNEXT = 2;
		private const int GW_HWNDFIRST = 0;
		private const int MF_BYCOMMAND = 0x0;
		private const int MF_BYPOSITION = 0x400;
		private const int MF_CHECKED = 0x8;
		private const int MF_DISABLED = 0x2;
		private const int MF_GRAYED = 0x1;
		private const int MF_MENUBARBREAK = 0x20;
		private const int MF_MENUBREAK = 0x40;
		private const int MF_POPUP = 0x10;
		private const int MF_SEPARATOR = 0x800;
		private const int MF_STRING = 0x0;
		private const int MIIM_ID = 0x2;
		private const int MIIM_SUBMENU = 0x4;
		private const int MIIM_TYPE = 0x10;
		private const int TPM_LEFTALIGN = 0x0;
		private const int TPM_RETURNCMD = 0x100;
		private const int TPM_RIGHTBUTTON = 0x2;

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", EntryPoint = "AppendMenuA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int AppendMenu(int hMenu, int wFlags, int wIDNewItem, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpNewItem);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int DestroyMenu(int hMenu);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int DeleteMenu(int hMenu, int nPosition, int uFlags);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int CreatePopupMenu();
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int EnableMenuItem(int hMenu, int wIDEnableItem, int wEnable);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure POINT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetCursorPos(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT lpPoint);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetDesktopWindow();
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetWindow(int Hwnd, int wCmd);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetWindowThreadProcessId(int Hwnd, ref int lpdwProcessId);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetCurrentProcessId();
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure RECT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetWindowRect(int Hwnd, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT lpRect);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure MENUITEMINFO may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("user32.dll", EntryPoint = "GetMenuItemInfoA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static bool GetMenuItemInfo(int hMenu, int un, bool b, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.MENUITEMINFO lpMenuItemInfo);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetFocus();
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetForegroundWindow();
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SetMenuDefaultItem(int hMenu, int uItem, int fByPos);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int TrackPopupMenuEx(int hMenu, int wFlags, int x, int y, int Hwnd, System.IntPtr lptpm);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SetMenuItemBitmaps(int hMenu, int nPosition, int wFlags, int hBitmapUnchecked, int hBitmapChecked);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int WindowFromPoint(int xPoint, int yPoint);

		public string Caption
		{
			get
			{

				return psCaption;

			}
			set
			{

				psCaption = value;

			}
		}


		public int Hwnd
		{
			get
			{

				return piHwnd;

			}
		}



		public void Remove(int iMenuPosition)
		{

			UpgradeSolution1Support.PInvoke.SafeNative.user32.DeleteMenu(piHwnd, iMenuPosition, MF_BYPOSITION);

		}

		public mcPopupMenu()
		{
			piHwnd = UpgradeSolution1Support.PInvoke.SafeNative.user32.CreatePopupMenu();
		}

		~mcPopupMenu()
		{
			UpgradeSolution1Support.PInvoke.SafeNative.user32.DestroyMenu(piHwnd);
		}

		public void Add(int iMenuID, object vMenuItem, bool bDefault = false, bool bChecked = false, mceItemStates eItemState = (mceItemStates) 0, int imgUnchecked = 0, int imgChecked = 0)
		{

			// Check to see if it's a menu item (a string) or a submenu (a class).
			string vMenuItemTyped = "";
			mcPopupMenu vMenuItemTyped2 = null;
			mcPopupMenu oSubmenu = null;
			if (vMenuItem is string)
			{
				vMenuItemTyped = ReflectionHelper.GetPrimitiveValue<string>(vMenuItem);

				if (vMenuItemTyped == "-")
				{ // Make a seperator
					string tempRefParam = "";
					UpgradeSolution1Support.PInvoke.SafeNative.user32.AppendMenu(piHwnd, MF_STRING | MF_SEPARATOR, iMenuID, ref tempRefParam);
				}
				else
				{
					string tempRefParam2 = vMenuItemTyped;
					UpgradeSolution1Support.PInvoke.SafeNative.user32.AppendMenu(piHwnd, MF_STRING | (-((bChecked) ? -1 : 0)) * MF_CHECKED, iMenuID, ref tempRefParam2);
				}

				// Menu Icons
				if (imgChecked == 0)
				{
					imgChecked = imgUnchecked;
				} // Need a value for both
				UpgradeSolution1Support.PInvoke.SafeNative.user32.SetMenuItemBitmaps(piHwnd, iMenuID, MF_BYCOMMAND, imgUnchecked, imgChecked);

				// Default item
				if (bDefault)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.SetMenuDefaultItem(piHwnd, iMenuID, 0);
				}
				// Disabled (Regular color text)
				if (eItemState == mceItemStates.mceDisabled)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.EnableMenuItem(piHwnd, iMenuID, MF_BYCOMMAND | MF_DISABLED);
				}
				// Disabled (disabled color text)
				if (eItemState == mceItemStates.mceGrayed)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.EnableMenuItem(piHwnd, iMenuID, MF_BYCOMMAND | MF_GRAYED);
				}

				// Add a submenu
			}
			else if (vMenuItem is mcPopupMenu)
			{ 
				vMenuItemTyped2 = (mcPopupMenu) vMenuItem;
				oSubmenu = vMenuItemTyped2;
				string tempRefParam3 = oSubmenu.Caption;
				UpgradeSolution1Support.PInvoke.SafeNative.user32.AppendMenu(piHwnd, MF_STRING | MF_POPUP | (-((bChecked) ? -1 : 0)) * MF_CHECKED, oSubmenu.Hwnd, ref tempRefParam3);

				if (imgChecked == 0)
				{
					imgChecked = imgUnchecked;
				} // Need a value for both
				UpgradeSolution1Support.PInvoke.SafeNative.user32.SetMenuItemBitmaps(piHwnd, oSubmenu.Hwnd, MF_BYCOMMAND, imgUnchecked, imgChecked);

				// Default item
				if (bDefault)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.SetMenuDefaultItem(piHwnd, oSubmenu.Hwnd, 0);
				}
				// Disabled (Regular color text)
				if (eItemState == mceItemStates.mceDisabled)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.EnableMenuItem(piHwnd, oSubmenu.Hwnd, MF_BYCOMMAND | MF_DISABLED);
				}
				// Disabled (disabled color text)
				if (eItemState == mceItemStates.mceGrayed)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.EnableMenuItem(piHwnd, oSubmenu.Hwnd, MF_BYCOMMAND | MF_GRAYED);
				}


				oSubmenu = null;
			}

		}

		public int Show(int iFormHwnd = -1, int x = -1, int y = -1, int iControlHwnd = -1)
		{
			int iX = 0, iHwnd = 0, iY = 0;

			// If no form is passed, use the current window
			int iCurrentID = 0, iDesktopHwnd = 0, iChildHwnd = 0, iChildID = 0;
			if (iFormHwnd == -1 || iFormHwnd == 0)
			{


				iDesktopHwnd = UpgradeSolution1Support.PInvoke.SafeNative.user32.GetDesktopWindow();
				iChildHwnd = UpgradeSolution1Support.PInvoke.SafeNative.user32.GetWindow(iDesktopHwnd, GW_CHILD);
				iCurrentID = UpgradeSolution1Support.PInvoke.SafeNative.kernel32.GetCurrentProcessId();

				while(iChildHwnd != 0)
				{
					UpgradeSolution1Support.PInvoke.SafeNative.user32.GetWindowThreadProcessId(iChildHwnd, ref iChildID);
					if (iChildID == iCurrentID)
					{
						break;
					} // Snagged
					iChildHwnd = UpgradeSolution1Support.PInvoke.SafeNative.user32.GetWindow(iChildHwnd, GW_HWNDNEXT);
				};

				if (iChildHwnd == 0)
				{ // Can't resolve a form handle. Bail out.
					return -1;
				}
				iHwnd = iChildHwnd;
			}
			else
			{
				iHwnd = iFormHwnd;
			}

			// If passed a control handle, left-bottom orient to the control.
			UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT rt = new UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT();
			UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT pt = new UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT();
			if (iControlHwnd != -1)
			{
				rt = new UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT();
				UpgradeSolution1Support.PInvoke.SafeNative.user32.GetWindowRect(iControlHwnd, ref rt);
				iX = rt.left;
				iY = rt.Bottom;
			}
			else
			{
				pt = new UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT();
				UpgradeSolution1Support.PInvoke.SafeNative.user32.GetCursorPos(ref pt);
				if (x == -1)
				{
					iX = pt.X;
				}
				else
				{
					iX = x;
				}
				if (y == -1)
				{
					iY = pt.Y;
				}
				else
				{
					iY = y;
				}
			}
			return UpgradeSolution1Support.PInvoke.SafeNative.user32.TrackPopupMenuEx(piHwnd, TPM_RETURNCMD | TPM_RIGHTBUTTON, iX, iY, iHwnd, 0);

		}
	}
}