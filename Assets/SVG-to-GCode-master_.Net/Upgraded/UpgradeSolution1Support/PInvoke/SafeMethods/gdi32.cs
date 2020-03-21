using System;
using System.Runtime.InteropServices;

namespace UpgradeSolution1Support.PInvoke.SafeNative
{
	public static class gdi32
	{

		public static int Polygon(int hDC, ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI lpPoint, int nCount)
		{
			return UpgradeSolution1Support.PInvoke.UnsafeNative.gdi32.Polygon(hDC, ref lpPoint, nCount);
		}
	}
}