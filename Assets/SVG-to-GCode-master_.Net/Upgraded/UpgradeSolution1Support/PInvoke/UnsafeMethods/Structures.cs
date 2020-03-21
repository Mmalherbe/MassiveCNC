using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class Structures
	{

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int tOp;
			public int Right;
			public int Bottom;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct MENUITEMINFO
		{
			public int cbSize;
			public int fMask;
			public int fType;
			public int fState;
			public int wID;
			public int hSubMenu;
			public int hbmpChecked;
			public int hbmpUnchecked;
			public int dwItemData;
			public string dwTypeData;
			public int cch;
			private static void InitStruct(ref MENUITEMINFO result, bool init)
			{
				if (init)
				{
					result.dwTypeData = String.Empty;
				}
			}
			public static MENUITEMINFO CreateInstance()
			{
				MENUITEMINFO result = new MENUITEMINFO();
				InitStruct(ref result, true);
				return result;
			}
			public MENUITEMINFO Clone()
			{
				MENUITEMINFO result = this;
				InitStruct(ref result, false);
				return result;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINTAPI
		{
			public int X;
			public int Y;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEOPSTRUCT
		{
			public int Hwnd;
			public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			public int fAnyOperationsAborted;
			public int hNameMappings;
			public string lpszProgressTitle;
			private static void InitStruct(ref SHFILEOPSTRUCT result, bool init)
			{
				if (init)
				{
					result.pFrom = String.Empty;
					result.pTo = String.Empty;
					result.lpszProgressTitle = String.Empty;
				}
			}
			public static SHFILEOPSTRUCT CreateInstance()
			{
				SHFILEOPSTRUCT result = new SHFILEOPSTRUCT();
				InitStruct(ref result, true);
				return result;
			}
			public SHFILEOPSTRUCT Clone()
			{
				SHFILEOPSTRUCT result = this;
				InitStruct(ref result, false);
				return result;
			}
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
		}
	}
}