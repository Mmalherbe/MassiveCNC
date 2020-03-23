using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class imagehlp
	{

		[DllImport("IMAGEHLP.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int MakeSureDirectoryPathExists([MarshalAs(UnmanagedType.VBByRefStr)] ref string DirPath);
	}
}