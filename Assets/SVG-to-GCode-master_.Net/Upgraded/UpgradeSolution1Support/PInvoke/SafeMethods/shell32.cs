using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.SafeNative
{
	public static class shell32
	{

		public static int SHFileOperation(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.SHFILEOPSTRUCT lpFileOp)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.shell32.SHFileOperation(ref lpFileOp);
		}
	}
}