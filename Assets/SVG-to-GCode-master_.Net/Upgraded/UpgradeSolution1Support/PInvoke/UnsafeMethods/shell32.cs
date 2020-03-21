using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.UnsafeNative
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	public static class shell32
	{

		//UPGRADE_TODO: (1050) Structure SHFILEOPSTRUCT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		[DllImport("shell32.dll", EntryPoint = "SHFileOperationA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		extern public static int SHFileOperation(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.SHFILEOPSTRUCT lpFileOp);
	}
}