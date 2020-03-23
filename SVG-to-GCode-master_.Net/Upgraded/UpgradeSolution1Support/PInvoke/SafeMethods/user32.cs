using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.SafeNative
{
	public static class user32
	{

		public static int AppendMenu(int hMenu, int wFlags, int wIDNewItem, ref string lpNewItem)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.AppendMenu(hMenu, wFlags, wIDNewItem, ref lpNewItem);
		}
		public static int CreatePopupMenu()
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.CreatePopupMenu();
		}
		public static int DeleteMenu(int hMenu, int nPosition, int uFlags)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.DeleteMenu(hMenu, nPosition, uFlags);
		}
		public static int DestroyMenu(int hMenu)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.DestroyMenu(hMenu);
		}
		public static int EnableMenuItem(int hMenu, int wIDEnableItem, int wEnable)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.EnableMenuItem(hMenu, wIDEnableItem, wEnable);
		}
		public static int GetCursorPos(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINT lpPoint)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetCursorPos(ref lpPoint);
		}
		public static int GetDesktopWindow()
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetDesktopWindow();
		}
		public static short GetKeyState(int nVirtKey)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetKeyState(nVirtKey);
		}
		public static int GetWindow(int Hwnd, int wCmd)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetWindow(Hwnd, wCmd);
		}
		public static int GetWindowRect(int Hwnd, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.RECT lpRect)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetWindowRect(Hwnd, ref lpRect);
		}
		public static int GetWindowThreadProcessId(int Hwnd, ref int lpdwProcessId)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.GetWindowThreadProcessId(Hwnd, ref lpdwProcessId);
		}
		public static int SetMenuDefaultItem(int hMenu, int uItem, int fByPos)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.SetMenuDefaultItem(hMenu, uItem, fByPos);
		}
		public static int SetMenuItemBitmaps(int hMenu, int nPosition, int wFlags, int hBitmapUnchecked, int hBitmapChecked)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.SetMenuItemBitmaps(hMenu, nPosition, wFlags, hBitmapUnchecked, hBitmapChecked);
		}
		public static int SetWindowPos(int Hwnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.user32.SetWindowPos(Hwnd, hWndInsertAfter, x, y, cX, cY, wFlags);
		}
		public static int TrackPopupMenuEx(int hMenu, int wFlags, int x, int y, int Hwnd, int lptpm)
		{
			int result = 0;
			GCHandle handle = GCHandle.Alloc(lptpm, GCHandleType.Pinned);
			try
			{
				IntPtr tmpPtr = handle.AddrOfPinnedObject();
				result = UpgradeSolution1Support.PInvoke.UnsafeNative.user32.TrackPopupMenuEx(hMenu, wFlags, x, y, Hwnd, tmpPtr);
				lptpm = Marshal.ReadInt16(tmpPtr);
			}
			finally
			{
				handle.Free();
			}
			return result;
		}
	}
}