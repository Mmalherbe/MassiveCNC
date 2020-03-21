using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class user32
	{

		[DllImport("user32.dll", EntryPoint = "AppendMenuA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int AppendMenu(int hMenu, int wFlags, int wIDNewItem, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpNewItem);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int CreatePopupMenu();
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int DeleteMenu(int hMenu, int nPosition, int uFlags);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int DestroyMenu(int hMenu);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int EnableMenuItem(int hMenu, int wIDEnableItem, int wEnable);
		//UPGRADE_TODO: (1050) Structure POINT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetCursorPos(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT lpPoint);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetDesktopWindow();
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetFocus();
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetForegroundWindow();
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static short GetKeyState(int nVirtKey);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetMenu(int Hwnd);
		//UPGRADE_TODO: (1050) Structure MENUITEMINFO may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		[DllImport("user32.dll", EntryPoint = "GetMenuItemInfoA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static bool GetMenuItemInfo(int hMenu, int un, bool b, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.MENUITEMINFO lpMenuItemInfo);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetSubMenu(int hMenu, int nPos);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetWindow(int Hwnd, int wCmd);
		//UPGRADE_TODO: (1050) Structure RECT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetWindowRect(int Hwnd, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT lpRect);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int GetWindowThreadProcessId(int Hwnd, ref int lpdwProcessId);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int SetMenuDefaultItem(int hMenu, int uItem, int fByPos);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int SetMenuItemBitmaps(int hMenu, int nPosition, int wFlags, int hBitmapUnchecked, int hBitmapChecked);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int SetWindowPos(int Hwnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int TrackPopupMenuEx(int hMenu, int wFlags, int x, int y, int Hwnd, System.IntPtr lptpm);
		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int WindowFromPoint(int xPoint, int yPoint);
	}
}