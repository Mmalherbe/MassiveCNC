using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using UpgradeHelpers.Gui;
using UpgradeHelpers.Helpers;

namespace SVGtoGCODE
{
	internal static class GeneralFunctions
	{

		//General functions that could be used in any project.
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static void Sleep(int dwMilliseconds);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetMenu(int Hwnd);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int GetSubMenu(int hMenu, int nPos);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SetMenuDefaultItem(int hMenu, int uItem, int fByPos);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static short GetKeyState(int nVirtKey);
		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SetWindowPos(int Hwnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags);

		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		//[DllImport("IMAGEHLP.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int MakeSureDirectoryPathExists([MarshalAs(UnmanagedType.VBByRefStr)] ref string DirPath);

		public const int HWND_TOPMOST = -1;
		public const int SWP_NOMOVE = 0x2;
		public const int SWP_NOSIZE = 0x1;
		static readonly public int TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct typGenSizeInfo
		{
			public int numFiles;
			public int totalSize;
		}

		public static bool debug_noLog = false;


		//UPGRADE_NOTE: (2041) The following line was commented. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2041
		////UPGRADE_TODO: (1050) Structure SHFILEOPSTRUCT may require marshalling attributes to be passed as an argument in this Declare statement. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1050
		//[DllImport("shell32.dll", EntryPoint = "SHFileOperationA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		//extern public static int SHFileOperation(ref UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.SHFILEOPSTRUCT lpFileOp);

		public const int FO_DELETE = 0x3;
		public const int FO_MOVE = 0x1;
		public const int FOF_ALLOWUNDO = 0x40;
		public const int FOF_CONFIRMMOUSE = 0x2;
		public const int FOF_FILESONLY = 0x80; //  on *.*, do only files
		public const int FOF_MULTIDESTFILES = 0x1;
		public const int FOF_NOCONFIRMATION = 0x10; //  Don't prompt the user.
		public const int FOF_NOCONFIRMMKDIR = 0x200; //  don't confirm making any needed dirs
		public const int FOF_RENAMEONCOLLISION = 0x8;
		public const int FOF_SILENT = 0x4; //  don't create progress/report
		public const int FOF_SIMPLEPROGRESS = 0x100; //  means don't show names of files
		public const int FOF_WANTMAPPINGHANDLE = 0x20; //  Fill in SHFILEOPSTRUCT.hNameMappings
		public const int FOF_NOERRORUI = 0x400;


		// FILE AND DISK

		// Get file extension.
		internal static string getFileExten(string inName)
		{
			string result = "";
			int e = inName.LastIndexOf(".") + 1;
			if (e > 0)
			{
				result = inName.Substring(Math.Max(inName.Length - (Strings.Len(inName) - e), 0)).ToLower();
			}
			return result;
		}


		internal static string getFileNameFromPath(string inPth)
		{
			// Return the file name from the path

			int e = inPth.LastIndexOf("\\") + 1;
			if (e == 0)
			{
				e = inPth.LastIndexOf("/") + 1;
			}

			if (e > 0)
			{
				return inPth.Substring(Math.Max(inPth.Length - (Strings.Len(inPth) - e), 0));
			}
			else
			{
				return inPth;
			}

		}

		internal static string getFileNameNoExten(string inName)
		{
			string result = "";
			int e = inName.LastIndexOf(".") + 1;
			if (e > 0)
			{
				result = inName.Substring(0, Math.Min(e - 1, inName.Length));
			}
			return result;
		}

		internal static string getFolderNameFromPath(string inPth)
		{
			// Return the path from the path and file

			string result = "";
			int e = inPth.LastIndexOf("\\") + 1;
			if (e == 0)
			{
				e = inPth.LastIndexOf("/") + 1;
			}

			if (e > 0)
			{
				result = inPth.Substring(0, Math.Min(e - 1, inPth.Length));
			}
			return result;
		}

		// DOES NOT RECURSE
		// TODO: Make it take a file mask.
		internal static typGenSizeInfo getSizeOfFolder(string pth)
		{

			typGenSizeInfo result = new typGenSizeInfo();
			string A = myDir(pth + "\\*.*");

			while(A != "")
			{
				switch(getFileExten(A).ToLower())
				{
					case "jpg" : case "jpeg" : case "png" : case "gif" : case "wav" : 
						result.totalSize += ((int) (new FileInfo(pth + "\\" + A)).Length); 
						result.numFiles++; 
						break;
				}
				A = FileSystem.Dir();
			};
			return result;
		}




		// GENERAL
		internal static object Swap(ref double in1, ref double in2)
		{
			double b = in1;
			in1 = in2;
			in2 = b;
			return null;
		}

		// COLOR

		internal static int makeVBColor(ref string inHex)
		{
			// Turns FF0033 into the VB color.
			int G = 0, R = 0, b = 0;

			if (Strings.Len(inHex) < 6)
			{
				inHex = Strings.Replace(new String(' ', 6 - Strings.Len(inHex)), " ", "0", 1, -1, CompareMethod.Binary) + inHex;
			}

			if (Strings.Len(inHex) == 6)
			{
				R = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(0, Math.Min(2, inHex.Length))));
				G = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(2, Math.Min(2, inHex.Length - 2))));
				b = Convert.ToInt32(Conversion.Val("&H" + inHex.Substring(4, Math.Min(2, inHex.Length - 4))));

				return (b * 256 * 256) + (G * 256) + R;
			}
			else
			{
				return 0;
			}


		}

		internal static string myTrim(string inSt, ref string trimChar)
		{
			// Trim with a specified character.
			int i = 0;
			int j = 0;

			if (trimChar == "")
			{
				trimChar = " ";
			}

			int tempForEndVar = Strings.Len(inSt);
			for (i = 1; i <= tempForEndVar; i++)
			{
				if (inSt.Substring(i - 1, Math.Min(1, inSt.Length - (i - 1))) != trimChar)
				{
					break;
				}
			}

			int tempForEndVar2 = i;
			for (j = Strings.Len(inSt); j >= tempForEndVar2; j--)
			{
				if (inSt.Substring(j - 1, Math.Min(1, inSt.Length - (j - 1))) != trimChar)
				{
					break;
				}
			}

			return inSt.Substring(i - 1, Math.Min(j - i + 1, inSt.Length - (i - 1)));

		}

		internal static string myTrim(string inSt)
		{
			string tempRefParam = "";
			return myTrim(inSt, ref tempRefParam);
		}

		internal static string Escape(string inTxt)
		{

			//Escape = vbcurl_string_escape(inTxt, Len(inTxt))

			//Exit Function


			// Escape the text.
			string result = "";

			result = inTxt;

			result = Strings.Replace(result, "%", "%25", 1, -1, CompareMethod.Binary);
			for (int i = 1; i <= 255; i++)
			{
				if (i == 37)
				{
					// skip %
				}
				else if (i >= 65 && i <= 90)
				{ 
					// A-Z
				}
				else if (i >= 97 && i <= 122)
				{ 
					// a-z
				}
				else if (i >= 48 && i <= 57)
				{ 
					// 0-9
				}
				else
				{
					result = Strings.Replace(result, Strings.Chr(i).ToString(), "%" + fixZeros(i.ToString("X")), 1, -1, CompareMethod.Binary);
				}
			}




			// Old code
			//For i = 1 To Len(inTxt)
			//    b = Mid(inTxt, i, 1)
			//    If (b >= "A" And b <= "Z") Or (b >= "a" And b <= "z") Or (b >= "0" And b <= "9") Then
			//        outText = outText & b
			//    Else
			//        outText = outText & "%" & fixZeros(Hex(Asc(b)))
			//    End If
			//Next

			//Escape = outText

			return result;
		}

		internal static string Unescape(string inSt)
		{

			// Unescape %20 and stuff like that.
			string result = "";
			int e = 0;
			string nTwo = "";
			int nVal = 0;
			//UPGRADE_TODO: (1069) Error handling statement (On Error Resume Next) was converted to a pattern that might have a different behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1069
			try
			{

				do 
				{
					e = Strings.InStr(e + 1, inSt, "%", CompareMethod.Binary);
					if (e > 0)
					{

						// get next two characters.
						if (e + 2 <= Strings.Len(inSt))
						{
							nTwo = inSt.Substring(e, Math.Min(2, inSt.Length - e));

							// Convert hex to number
							nVal = 0;
							nVal = Convert.ToInt32(Conversion.Val("&H" + nTwo));

							if (nVal > 0)
							{
								inSt = inSt.Substring(0, Math.Min(e - 1, inSt.Length)) + Strings.Chr(nVal).ToString() + inSt.Substring(Math.Max(inSt.Length - (Strings.Len(inSt) - e - 2), 0));
							}
						}
					}
				}
				while(e != 0);

				result = inSt;
			}
			catch (Exception exc)
			{
				NotUpgradedHelper.NotifyNotUpgradedElement("Resume in On-Error-Resume-Next Block");
			}

			return result;
		}

		internal static string fixZeros(string inSt)
		{
			// Adds a 0 to the front if needed.
			string result = "";
			result = inSt;
			if (Strings.Len(result) == 1)
			{
				result = "0" + result;
			}
			return result;
		}


		internal static double Max(double n1, double n2)
		{
			if (n1 > n2)
			{
				return n1;
			}
			else
			{
				return n2;
			}
		}

		internal static double Min(double n1, double n2)
		{
			if (n1 < n2)
			{
				return n1;
			}
			else
			{
				return n2;
			}
		}

		internal static bool isIn(object checkFor, params object[] checkIn)
		{
			// See if the value is in one of the items
			int tempForEndVar = checkIn.GetUpperBound(0);
			for (int i = 0; i <= tempForEndVar; i++)
			{
				if (checkFor.Equals(checkIn[i]))
				{
					return true;
				}
			}


			return false;
		}

		internal static bool isInArray(object checkFor, object[] checkIn)
		{
			// See if the value is in one of the items
			foreach (object checkIn_item in checkIn)
			{
				if (checkFor.Equals(checkIn_item))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool separateURL(string inURL, ref string Host, ref string path, ref int port, ref string UserN, ref string PassW, ref string protocol)
		{
			// Turn the URL:
			// http://www.site.com/path/to/file.pat?text
			// ftp://user:pass@www.site.com:port/path/to/file

			// into host and path
			bool result = false;
			string userpass = "";
			string URL = inURL;

			int f = 0;

			int e = (URL.IndexOf("://") + 1);
			if (e > 0)
			{

				// remove http://
				protocol = URL.Substring(0, Math.Min(e + 2, URL.Length));
				URL = URL.Substring(Math.Max(URL.Length - (Strings.Len(URL) - e - 2), 0));


				e = (URL.IndexOf('/') + 1);
				if (e > 0)
				{
					Host = URL.Substring(0, Math.Min(e - 1, URL.Length));
					path = URL.Substring(Math.Max(URL.Length - (Strings.Len(URL) - e + 1), 0));
				}
				else
				{
					Host = URL;
				}

				e = Host.LastIndexOf(":") + 1;
				f = Host.LastIndexOf("@") + 1;
				if (e > 0 && e > f)
				{
					port = Convert.ToInt32(Conversion.Val(Host.Substring(Math.Max(Host.Length - (Strings.Len(Host) - e), 0))));
					Host = Host.Substring(0, Math.Min(e - 1, Host.Length));
				}

				e = (Host.IndexOf('@') + 1);
				if (e > 0)
				{
					userpass = Host.Substring(0, Math.Min(e - 1, Host.Length));
					Host = Host.Substring(Math.Max(Host.Length - (Strings.Len(Host) - e), 0));

					e = (userpass.IndexOf(':') + 1);
					if (e > 0)
					{
						// user and pass
						UserN = userpass.Substring(0, Math.Min(e - 1, userpass.Length));
						PassW = userpass.Substring(Math.Max(userpass.Length - (Strings.Len(userpass) - e), 0));
					}
					else
					{
						UserN = userpass;
					}
				}

				result = true;
			}
			return result;
		}

		internal static bool separateURL(string inURL, ref string Host, ref string path, ref int port, ref string UserN, ref string PassW)
		{
			string tempRefParam2 = "";
			return separateURL(inURL, ref Host, ref path, ref port, ref UserN, ref PassW, ref tempRefParam2);
		}

		internal static bool separateURL(string inURL, ref string Host, ref string path, ref int port, ref string UserN)
		{
			string tempRefParam3 = "";
			string tempRefParam4 = "";
			return separateURL(inURL, ref Host, ref path, ref port, ref UserN, ref tempRefParam3, ref tempRefParam4);
		}

		internal static bool separateURL(string inURL, ref string Host, ref string path, ref int port)
		{
			string tempRefParam5 = "";
			string tempRefParam6 = "";
			string tempRefParam7 = "";
			return separateURL(inURL, ref Host, ref path, ref port, ref tempRefParam5, ref tempRefParam6, ref tempRefParam7);
		}

		internal static bool separateURL(string inURL, ref string Host, ref string path)
		{
			int tempRefParam8 = 0;
			string tempRefParam9 = "";
			string tempRefParam10 = "";
			string tempRefParam11 = "";
			return separateURL(inURL, ref Host, ref path, ref tempRefParam8, ref tempRefParam9, ref tempRefParam10, ref tempRefParam11);
		}

		internal static bool setComboBoxToTextItem(ComboBox checkComboBox, string matchString)
		{
			// Set a combobox to the item

			int tempForEndVar = checkComboBox.Items.Count - 1;
			for (int i = 0; i <= tempForEndVar; i++)
			{
				if (matchString == checkComboBox.GetListItem(i))
				{
					checkComboBox.SelectedIndex = i;
					return true;
				}
			}
			checkComboBox.SelectedIndex = -1;
			return false;
		}

		internal static string myDir(string Pathname, FileAttribute Attributes)
		{

			// Dir to avoid errors
			//UPGRADE_TODO: (1069) Error handling statement (On Error Resume Next) was converted to a pattern that might have a different behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1069
			string result = "";
			try
			{
				if (Pathname == "")
				{
					result = "";
				}
				else
				{
					result = FileSystem.Dir(Pathname, Attributes);
				}
			}
			catch (Exception exc)
			{
				NotUpgradedHelper.NotifyNotUpgradedElement("Resume in On-Error-Resume-Next Block");
			}

			return result;
		}

		internal static string myDir(string Pathname)
		{
			return myDir(Pathname, FileAttribute.Normal);
		}

		internal static string myDir()
		{
			return myDir("", FileAttribute.Normal);
		}


		internal static object addToLog(params object[] entries)
		{

			if (debug_noLog)
			{
				return null;
			}

			//UPGRADE_TODO: (1069) Error handling statement (On Error Resume Next) was converted to a pattern that might have a different behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1069
			try
			{

				StringBuilder t = new StringBuilder();
				t.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:SS") + "." + StringsHelper.Format(Math.Floor(getDecimal(DateTime.Now.TimeOfDay.TotalSeconds) * 1000), "000") + "] ");

				foreach (string x in entries)
				{
					t.Append(x + "\t");
				}

				int f = FileSystem.FreeFile();

				//Open debugLogPath For Append As f
				//    Print #f, t
				//Close f

				Debug.WriteLine(t.ToString());
			}
			catch (Exception exc)
			{
				NotUpgradedHelper.NotifyNotUpgradedElement("Resume in On-Error-Resume-Next Block");
			}





			return null;
		}

		internal static string secsToTime(double inSecs)
		{
			// Convert 3232.587329 to a nice number like 4:56:22.31
			double S = inSecs;

			int H = Convert.ToInt32(Math.Floor(S / 3600));
			S -= (H * 3600);

			int M = Convert.ToInt32(Math.Floor(S / 60));
			S -= (M * 60);

			return H.ToString() + ":" + StringsHelper.Format(M, "00") + ":" + StringsHelper.Format(Math.Round((double) S, 2), "00.00");
		}

		internal static string secsToTime2(double inSecs)
		{
			// Convert 3232.587329 to a nice number like 4:56:22.31
			double S = inSecs;

			int H = Convert.ToInt32(Math.Floor(S / 3600));
			S -= (H * 3600);

			int M = Convert.ToInt32(Math.Floor(S / 60));
			S -= (M * 60);

			return H.ToString() + ":" + StringsHelper.Format(M, "00") + ":" + StringsHelper.Format(Math.Round((double) S, 2), "00");
		}
		internal static string secsToTime3(double inSecs)
		{
			// Convert 3232.587329 to a nice number like 4:56:22.31
			double S = inSecs;

			int H = Convert.ToInt32(Math.Floor(S / 3600));
			S -= (H * 3600);

			int M = Convert.ToInt32(Math.Floor(S / 60));
			S -= (M * 60);

			return H.ToString() + " hours, " + M.ToString() + " minutes";
		}

		internal static string Filerize(string inSt)
		{

			// Turn any string of text into a valid filename.
			string result = "";
			int b = 0;
			int tempForEndVar = Strings.Len(inSt);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				b = Strings.Asc(inSt.Substring(i - 1, Math.Min(1, inSt.Length - (i - 1)))[0]);
				if ((b >= 65 && b <= 90) || (b >= 48 && b <= 57) || (b >= 97 && b <= 122) || (b >= 35 && b <= 41) || (b >= 44 && b <= 46) || b == 32 || b == 95)
				{

					result = result + Strings.Chr(b).ToString();
				}
				else
				{
					result = result + "_";
				}
			}

			return result;
		}


		internal static bool GCD_Of(double First_Int, double Second_Int, ref int Numerator, ref int Denominator)
		{
			bool result = false;

			//UPGRADE_TODO: (1069) Error handling statement (On Error Resume Next) was converted to a pattern that might have a different behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1069
			try
			{


				double Q = 1d; // Initialize quotient as DECIMAL variable type
				double R = Q; // Initialize remainder



				// Convert input arguments into DECIMAL variable type
				if (Math.Floor(First_Int) != First_Int || Math.Floor(Second_Int) != Second_Int)
				{
					First_Int *= 100;
					Second_Int *= 100;
				}

				First_Int = (double) First_Int;
				Second_Int = (double) Second_Int;

				// Read the input argument values
				double x = First_Int;
				double y = Second_Int;

				// Make sure both arguments are absolute values
				x = Math.Abs(x);
				y = Math.Abs(y);

				// Report error if either argument is zero
				if (x == 0 || y == 0)
				{
					return result;
				}

				// Swap argument values, if necessary, so that X > Y
				if (x < y)
				{
					Q = x;
					x = y;
					y = Q;
				}

				// Perform Euclid's algorithm to find GCD of X and Y
				while (R != 0)
				{
					Q = x / y;
					//i = InStr(Q, ".")
					//If i > 0 Then Q = Left(Q, i - 1)

					// Truncate decimal.

					Q = Math.Floor(Q);


					R = x - Q * y;
					x = y;
					y = R;
				}

				// Return the result
				result = true;
				//Debug.Print "result: ", X

				Numerator = Convert.ToInt32(First_Int / x);
				Denominator = Convert.ToInt32(Second_Int / x);
			}
			catch (Exception exc)
			{
				NotUpgradedHelper.NotifyNotUpgradedElement("Resume in On-Error-Resume-Next Block");
			}



			return result;
		}


		internal static void MakeTopMost(int Hwnd)
		{
			UpgradeSolution1Support.PInvoke.SafeNative.user32.SetWindowPos(Hwnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
		}

		internal static int ceiling(double Number)
		{
			return Convert.ToInt32(-Math.Floor(-Number));
		}

		internal static string makeHTMLCodes(string ST)
		{

			ST = Strings.Replace(ST, "&", "&amp;", 1, -1, CompareMethod.Binary);

			ST = Strings.Replace(ST, "\"", "&quot;", 1, -1, CompareMethod.Binary);
			ST = Strings.Replace(ST, "<", "&lt;", 1, -1, CompareMethod.Binary);
			ST = Strings.Replace(ST, ">", "&gt;", 1, -1, CompareMethod.Binary);
			ST = Strings.Replace(ST, "'", "&apos;", 1, -1, CompareMethod.Binary);


			return ST;
		}


		internal static string toJSONString(string inSt)
		{

			string result = "";
			result = inSt;
			result = Strings.Replace(result, "\\", "\\\\", 1, -1, CompareMethod.Binary);
			result = Strings.Replace(result, Strings.Chr(8).ToString(), "\\b", 1, -1, CompareMethod.Binary);
			result = Strings.Replace(result, "\"", "\\\"", 1, -1, CompareMethod.Binary);
			result = Strings.Replace(result, Strings.Chr(12).ToString(), "\\f", 1, -1, CompareMethod.Binary);
			result = Strings.Replace(result, "\n", "\\n", 1, -1, CompareMethod.Binary);
			result = Strings.Replace(result, "\r", "\\r", 1, -1, CompareMethod.Binary);
			return Strings.Replace(result, "\t", "\\t", 1, -1, CompareMethod.Binary);

		}


		internal static bool isKeyDown(int KeyCode)
		{

			return Math.Abs(UpgradeSolution1Support.PInvoke.SafeNative.user32.GetKeyState(KeyCode)) > 1;

		}

		internal static bool MyIsNumeric(string Expression)
		{
			// Deals with bugs in the real IsNumeric

			if (Information.VarType(Expression) == VariantType.String)
			{
				if (safeRight(Expression.Trim(), 1) == "+")
				{
					return false;
				}
				if (safeRight(Expression.Trim(), 1) == "-")
				{
					return false;
				}
			}

			return Information.IsNumeric(Expression);



		}

		internal static string safeLeft(string inSt, int numChr)
		{
			if (Strings.Len(inSt) >= numChr)
			{
				return inSt.Substring(0, Math.Min(numChr, inSt.Length));
			}
			else
			{
				return inSt;
			}
		}


		internal static string safeRight(string inSt, int numChr)
		{
			if (Strings.Len(inSt) >= numChr)
			{
				return inSt.Substring(Math.Max(inSt.Length - numChr, 0));
			}
			else
			{
				return inSt;
			}
		}

		internal static string fileIntoMemory(string tPath)
		{
			// Quick load a file into memory

			//UPGRADE_TODO: (1069) Error handling statement (On Error Resume Next) was converted to a pattern that might have a different behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1069
			string result = "";
			try
			{


				string G = new String(' ', (int) (new FileInfo(tPath)).Length);
				int f = FileSystem.FreeFile();
				FileSystem.FileOpen(f, tPath, OpenMode.Binary, OpenAccess.Default, OpenShare.Default, -1);
				//UPGRADE_WARNING: (2080) Get was upgraded to FileGet and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
				FileSystem.FileGet(f, ref G, -1);
				FileSystem.FileClose(f);

				result = G;
			}
			catch (Exception exc)
			{
				NotUpgradedHelper.NotifyNotUpgradedElement("Resume in On-Error-Resume-Next Block");
			}

			return result;
		}

		internal static double getDecimal(double inNum)
		{
			// Return the number after the decimal point
			return inNum - Math.Floor(inNum);
		}

		internal static string GetFile(string inPath)
		{
			// Load this file into memory.


			int f = 0;
			string G = "";
			try
			{

				f = FileSystem.FreeFile();
				G = new String(' ', (int) (new FileInfo(inPath)).Length);

				FileSystem.FileOpen(f, inPath, OpenMode.Binary, OpenAccess.Default, OpenShare.Default, -1);
				//UPGRADE_WARNING: (2080) Get was upgraded to FileGet and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
				FileSystem.FileGet(f, ref G, -1);
				FileSystem.FileClose(f);


				return G;
			}
			catch (System.Exception excep)
			{

				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				MessageBox.Show("Error " + Information.Err().Number.ToString() + " (" + excep.Message + ") in procedure GetFile of Module GeneralFunctions", "LIVELAYOUT ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				addToLog("[ERROR]", "In GetFile of Module GeneralFunctions", Information.Err().Number, excep.Message);
			}
			return "";
		}


		internal static object MoveFile(string origPath, string newPath)
		{

			// Move the files
			UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.SHFILEOPSTRUCT WinType_SFO = UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.SHFILEOPSTRUCT.CreateInstance();

			WinType_SFO.wFunc = FO_MOVE;
			WinType_SFO.pFrom = origPath + Strings.Chr(0).ToString();
			WinType_SFO.pTo = newPath + Strings.Chr(0).ToString();
			WinType_SFO.fFlags = (short) (FOF_MULTIDESTFILES | FOF_NOCONFIRMMKDIR | FOF_NOERRORUI | FOF_SILENT);

			int lRet = UpgradeSolution1Support.PInvoke.SafeNative.shell32.SHFileOperation(ref WinType_SFO);

			return null;
		}

		internal static string addCredentialsToPath(string inPath, string sUser, string sPass)
		{
			// Add these credentials to the path.
			string aPath = "", aHost = "", aProtocol = "";
			int aPort = 0;

			string tempRefParam = "";
			string tempRefParam2 = "";
			separateURL(inPath, ref aHost, ref aPath, ref aPort, ref tempRefParam, ref tempRefParam2, ref aProtocol);

			return aProtocol + sUser + ":" + sPass + "@" + aHost + ((aPort > 0) ? ":" + aPort.ToString() : "") + aPath;

		}


		internal static object HandleError(string ErrLine, string ErrLocation, string ErrNum, string ErrDesc)
		{
			addToLog("[ERROR]", ErrLocation, ErrNum, ErrDesc, "Line " + ErrLine);
			MessageBox.Show("Error " + ErrNum + " (" + ErrDesc + ") " + ErrLocation + " (Line " + ErrLine + ")", Application.ProductName + " ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return null;
		}

		internal static bool FromCheck(CheckBox inCheck)
		{
			return inCheck.CheckState == CheckState.Checked;
		}
	}
}