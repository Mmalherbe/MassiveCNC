using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.SafeNative
{
	public static class kernel32
	{

		public static int GetCurrentProcessId()
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.kernel32.GetCurrentProcessId();
		}
		public static void Sleep(int dwMilliseconds)
		{
			UpgradeSolution1Support.PInvoke.UnsafeNative.kernel32.Sleep(dwMilliseconds);
		}
	}
}