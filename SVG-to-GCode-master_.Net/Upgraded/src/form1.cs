using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UpgradeHelpers.Gui;
using UpgradeHelpers.Helpers;
using UpgradeStubs;

namespace SVGtoGCODE
{
	internal partial class frmInterface
		: System.Windows.Forms.Form
	{


		public double Zoom = 0;
		public double panX = 0;
		public double panY = 0;
		float mX = 0;
		float mY = 0;
		double oX = 0;
		double oY = 0;
		bool mouseDown = false;



		//UPGRADE_NOTE: (7001) The following declaration (cmdScale_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void cmdScale_Click()
		//{
			//double maxX = 0, maxY = 0;
			//double scalar = 0;
			//
			//double w = Conversion.Val(Interaction.InputBox("Scale width to ? (inches)", "Scale", "5", 0, 0));
			//
			//SVGParse.getExtents(ref maxX, ref maxY);
			//
			//if (w > 0)
			//{
				//
				//scalar = w / maxY;
				//
				//int tempForEndVar = SVGParse.pData.GetUpperBound(0);
				//for (int i = 1; i <= tempForEndVar; i++)
				//{
					//int tempForEndVar2 = SVGParse.pData[i].Points.GetUpperBound(0);
					//for (int j = 1; j <= tempForEndVar2; j++)
					//{
						//SVGParse.pData[i].Points[j].x = scalar * SVGParse.pData[i].Points[j].x;
						//SVGParse.pData[i].Points[j].y = scalar * SVGParse.pData[i].Points[j].y;
					//}
				//}
			//}
			//
			//drawLines();
			//
			//
		//}

		//UPGRADE_NOTE: (7001) The following declaration (Command1_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command1_Click()
		//{
			//SVGParse.parseSVG(Path.GetDirectoryName(Application.ExecutablePath) + "\\wuffy-fill.svg");
			//parseSVG App.Path & "\drawing-3.svg"
			//
			//Debug.WriteLine(Support.TabLayout("Drawing ", SVGParse.pData.GetUpperBound(0).ToString()));
			//
			//drawLines();
			//
			//updateList();
			//
			//
		//}

		public frmInterface()
			: base()
		{
			if (m_vb6FormDefInstance == null)
			{
				if (m_InitializingDefInstance)
				{
					m_vb6FormDefInstance = this;
				}
				else
				{
					try
					{
						//For the start-up form, the first instance created is the default instance.
						if (System.Reflection.Assembly.GetExecutingAssembly().EntryPoint != null && System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.DeclaringType == this.GetType())
						{
							m_vb6FormDefInstance = this;
						}
					}
					catch
					{
					}
				}
			}
			//This call is required by the Windows Form Designer.
			isInitializingComponent = true;
			InitializeComponent();
			isInitializingComponent = false;
			ReLoadForm(false);
		}


		//UPGRADE_NOTE: (7001) The following declaration (cmd1_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void cmd1_Click()
		//{
			//
			//string tempRefParam = "M 402.85714,489.50504 L -94.285714,92.362183";
			//SVGParse.parsePath(ref tempRefParam, "");
			//
			//drawLines();
			//
		//}


		private void drawLines()
		{

			//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Cls was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			Picture1.Cls();

			object c = null;
			int cOrig = 0;

			// Draw the lines.

			//Debug.Print panX, panY

			UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI[] polyPoints = null;
			bool isDefocused = false;

			double lastX = -10000;
			double lastY = -10000;


			bool drawNonCut = TB1.get_ButtonChecked("noncut");


			int tempForEndVar = SVGParse.pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				//UPGRADE_ISSUE: (2064) PictureBox property Picture1.ForeColor was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				Picture1.setForeColor(Color.Black);
				//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawWidth was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				Picture1.setDrawWidth(1);
				//UPGRADE_ISSUE: (2070) Constant vbSolid was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
				//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				Picture1.setDrawStyle(UpgradeStubs.VBRUN_DrawStyleConstants.getvbSolid());
				c = SVGParse.pData[i].greyLevel * (255 / Rasterize.GREYLEVELS);
				c = ColorTranslator.ToOle(Color.FromArgb(ReflectionHelper.GetPrimitiveValue<int>(c), ReflectionHelper.GetPrimitiveValue<int>(c), ReflectionHelper.GetPrimitiveValue<int>(c)));
				//UPGRADE_WARNING: (1068) c of type Variant is being forced to int. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1068
				cOrig = ReflectionHelper.GetPrimitiveValue<int>(c);
				isDefocused = false;
				if (SVGParse.layerInfo.ContainsKey(SVGParse.pData[i].LayerID))
				{
					isDefocused = ReflectionHelper.Invoke<bool>(SVGParse.layerInfo[SVGParse.pData[i].LayerID], "Exists", new object[]{"defocused"});
				}

				//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawWidth was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				Picture1.setDrawWidth((isDefocused) ? 5 : 1);

				// Draw a line from the last point
				if (lastX != -10000 && lastY != -10000 && drawNonCut && SVGParse.pData[i].LayerID != "Cut Boxes")
				{
					if (SVGParse.pData[i].Points.GetUpperBound(0) > 0)
					{

						// Dashed line to here
						//UPGRADE_ISSUE: (2070) Constant vbDashDot was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
						//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.setDrawStyle(UpgradeStubs.VBRUN_DrawStyleConstants.getvbDashDot());
						using (Graphics g = Picture1.CreateGraphics())
						{

							g.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), Convert.ToInt32((SVGParse.pData[i].Points[1].x + panX) * Zoom), Convert.ToInt32((SVGParse.pData[i].Points[1].y + panY) * Zoom), Convert.ToInt32(lastX), Convert.ToInt32(lastY));
						}
						//UPGRADE_ISSUE: (2070) Constant vbSolid was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
						//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.setDrawStyle(UpgradeStubs.VBRUN_DrawStyleConstants.getvbSolid());
					}
				}

				int tempForEndVar2 = SVGParse.pData[i].Points.GetUpperBound(0) - 1;
				for (int j = 1; j <= tempForEndVar2; j++)
				{

					c = cOrig;
					if (SVGParse.pData[i].Points[j].noCut == 1)
					{
						c = ColorTranslator.ToOle(Color.FromArgb(150, 0, 0));
					}
					if (isDefocused)
					{
						c = ColorTranslator.ToOle(Color.FromArgb(0, 200, 0));
					}
					if (SVGParse.pData[i].LayerID == "Cut Boxes")
					{
						c = ColorTranslator.ToOle(Color.FromArgb(255, 0, 255));
						//UPGRADE_ISSUE: (2070) Constant vbDot was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
						//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.setDrawStyle(UpgradeStubs.VBRUN_DrawStyleConstants.getvbDot());
					}



					using (Graphics g = Picture1.CreateGraphics())
					{

						g.DrawLine(new Pen(ColorTranslator.FromOle(ReflectionHelper.GetPrimitiveValue<int>(c))), Convert.ToInt32((SVGParse.pData[i].Points[j].x + panX) * Zoom), Convert.ToInt32((SVGParse.pData[i].Points[j].y + panY) * Zoom), Convert.ToInt32((SVGParse.pData[i].Points[j + 1].x + panX) * Zoom), Convert.ToInt32((SVGParse.pData[i].Points[j + 1].y + panY) * Zoom));
					}


				}

				if (SVGParse.pData[i].Points.GetUpperBound(0) > 0 && SVGParse.pData[i].LayerID != "Cut Boxes")
				{
					lastX = (SVGParse.pData[i].Points[SVGParse.pData[i].Points.GetUpperBound(0)].x + panX) * Zoom;
					lastY = (SVGParse.pData[i].Points[SVGParse.pData[i].Points.GetUpperBound(0)].y + panY) * Zoom;
				}






				if (SVGParse.pData[i].Fillable && SVGParse.pData[i].ContainedBy == 0 && false)
				{
					//UPGRADE_ISSUE: (2070) Constant vbFSSolid was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
					//UPGRADE_ISSUE: (2064) PictureBox property Picture1.FillStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					Picture1.setFillStyle(UpgradeStubs.VBRUN_FillStyleConstants.getvbFSSolid());
					//UPGRADE_ISSUE: (2064) PictureBox property Picture1.ForeColor was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					Picture1.setForeColor(Color.Blue);
					//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawWidth was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					Picture1.setDrawWidth(1);
					//UPGRADE_ISSUE: (2070) Constant vbInvisible was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2070
					//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawStyle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					Picture1.setDrawStyle(UpgradeStubs.VBRUN_DrawStyleConstants.getvbInvisible());


					polyPoints = new UpgradeSolution1Support.PInvoke.UnsafeNative.Structures.POINTAPI[SVGParse.pData[i].Points.GetUpperBound(0)];
					int tempForEndVar3 = SVGParse.pData[i].Points.GetUpperBound(0);
					for (int j = 1; j <= tempForEndVar3; j++)
					{
						polyPoints[j - 1].X = Convert.ToInt32((SVGParse.pData[i].Points[j].x + panX) * Zoom);
						polyPoints[j - 1].Y = Convert.ToInt32((SVGParse.pData[i].Points[j].y + panY) * Zoom);
					}

					// Add any that are fillable.
					Polygons.addFillPolies(ref polyPoints, i);



					using (Graphics g = Picture1.CreateGraphics())
					{

						UpgradeSolution1Support.PInvoke.SafeNative.gdi32.Polygon(g.GetHdc().ToInt32(), ref polyPoints[0], polyPoints.GetUpperBound(0));
						g.ReleaseHdc();
					} //call the polygon function
				}

			}

			int A = 0;


			int tempForEndVar4 = List1.Items.Count;
			for (int i = 1; i <= tempForEndVar4; i++)
			{
				if (ListBoxHelper.GetSelected(List1, i - 1))
				{
					A = List1.GetItemData(i - 1);
					if (A > 0 && A <= SVGParse.pData.GetUpperBound(0))
					{



						//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Circle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.Circle((float) ((SVGParse.pData[A].Points[1].x + panX) * Zoom), (float) ((SVGParse.pData[A].Points[1].y + panY) * Zoom), 5, ColorTranslator.ToOle(Color.Lime), 0, 0, 0);

						int tempForEndVar5 = SVGParse.pData[A].Points.GetUpperBound(0) - 1;
						for (int j = 1; j <= tempForEndVar5; j++)
						{

							//UPGRADE_ISSUE: (2064) PictureBox property Picture1.ForeColor was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
							Picture1.setForeColor(Color.Red);
							//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawWidth was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
							Picture1.setDrawWidth(3);

							using (Graphics g = Picture1.CreateGraphics())
							{

								g.DrawLine(new Pen(new Color()), Convert.ToInt32((SVGParse.pData[A].Points[j].x + panX) * Zoom), Convert.ToInt32((SVGParse.pData[A].Points[j].y + panY) * Zoom), Convert.ToInt32((SVGParse.pData[A].Points[j + 1].x + panX) * Zoom), Convert.ToInt32((SVGParse.pData[A].Points[j + 1].y + panY) * Zoom));
							}

							//If j > 1 Then Picture1.Circle ((.Points(j).x + panX) * Zoom, (.Points(j).y + panY) * Zoom), 5, vbBlue

						}

						//UPGRADE_ISSUE: (2064) PictureBox property Picture1.DrawWidth was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.setDrawWidth(1);

						//UPGRADE_ISSUE: (2064) PictureBox method Picture1.Circle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.Circle((float) ((SVGParse.pData[A].Points[SVGParse.pData[A].Points.GetUpperBound(0)].x + panX) * Zoom), (float) ((SVGParse.pData[A].Points[SVGParse.pData[A].Points.GetUpperBound(0)].y + panY) * Zoom), 5, ColorTranslator.ToOle(Color.Red), 0, 0, 0);

					}
				}
			}


			updateRulers();


		}

		//UPGRADE_NOTE: (7001) The following declaration (Command2_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command2_Click()
		//{
			//
		//}

		//UPGRADE_NOTE: (7001) The following declaration (Command3_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command3_Click()
		//{
			//
			////
			//    ' Calculate the X and Y center of each polygon
			//    Dim i As Long
			//    Dim sorted As Boolean
			//    For i = 1 To UBound(pData)
			//        calcPolyCenter i, pData(i).xCenter, pData(i).yCenter
			//    Next
			////
			//    ' Sort!
			//    Do
			//        sorted = False
			//        For i = 1 To UBound(pData) - 1
			////
			//            If pData(i).yCenter > pData(i + 1).yCenter Then
			//                sorted = True
			//                Swap pData(i), pData(i + 1)
			//            ElseIf pData(i).yCenter = pData(i + 1).yCenter Then
			//                If pData(i).xCenter < pData(i + 1).xCenter Then
			//                    sorted = True
			//                    Swap pData(i), pData(i + 1)
			//                End If
			//            End If
			////
			//        Next
			//    Loop Until Not sorted
			////
			//    drawLines
			//    updateList
			//
			//
		//}




		//UPGRADE_NOTE: (7001) The following declaration (Command7_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command7_Click()
		//{
			//
			//rasterLinePoly Val(Text1)
			//drawLines
			//
			//double x = 0;
			//double y = 0;
			//
			//SVGParse.pData = new SVGParse.typLine[1];
			//
			//
			//newLine
			//For x = 0 To 1400 Step 1
			//    y = (Sin(x / 40) * 100) + 100
			//    addPoint x, y
			//Next
			//
			//newLine
			//For x = 0 To 1400 Step 1
			//    y = (Sin(x / 10) * 100) + 310
			//    addPoint x, y
			//Next
			//
			//circle
			//SVGParse.newLine();
			//for (double Ang = 0; Ang <= 3.14159d * 2; Ang += 0.05d)
			//{
				//x = (Math.Cos(Ang) * 200) + 250;
				//y = (Math.Sin(Ang) * 200) + 220;
				//SVGParse.addPoint(x, y);
			//}
			//
			//SVGParse.newLine();
			//for (double Ang = 0; Ang <= 3.14159d * 2; Ang += 0.1d)
			//{
				//x = (Math.Cos(Ang) * 200) + 750;
				//y = (Math.Sin(Ang) * 200) + 220;
				//SVGParse.addPoint(x, y);
			//}
			//
			//
			//SVGParse.newLine();
			//for (x = 0; x <= 1400; x += 4)
			//{
				//y = (Math.Sin(x / 160) * 100) + 530;
				//SVGParse.addPoint(x, y);
			//}
			//
			//circle
			//SVGParse.newLine();
			//for (double Ang = 0; Ang <= 3.14159d * 2; Ang += 0.01d)
			//{
				//x = (Math.Cos(Ang) * 200) + 250;
				//y = (Math.Sin(Ang) * 200) + 900;
				//SVGParse.addPoint(x, y);
			//}
			//
			//
			//double nSeg = 20;
			//double tempForEndVar5 = nSeg;
			//for (double n = 1; n <= tempForEndVar5; n += 2)
			//{
				//SVGParse.newLine();
				//
				//SVGParse.addPoint(700, 900);
				//
				//double tempForEndVar6 = ((3.14159d * 2) / nSeg) * (n + 1);
				//for (double Ang = ((3.14159d * 2) / nSeg) * n; Ang <= tempForEndVar6; Ang += 0.01d)
				//{
					//x = (Math.Cos(Ang) * 200) + 700;
					//y = (Math.Sin(Ang) * 200) + 900;
					//SVGParse.addPoint(x, y);
				//}
				//
				//SVGParse.addPoint(700, 900);
				//
				//
			//}
			//
			//
			//
			//nSeg = 70;
			//double tempForEndVar7 = nSeg;
			//for (double n = 1; n <= tempForEndVar7; n += 2)
			//{
				//SVGParse.newLine();
				//
				//SVGParse.addPoint(1200, 900);
				//
				//double tempForEndVar8 = ((3.14159d * 2) / nSeg) * (n + 1);
				//for (double Ang = ((3.14159d * 2) / nSeg) * n; Ang <= tempForEndVar8; Ang += 0.01d)
				//{
					//x = (Math.Cos(Ang) * 200) + 1200;
					//y = (Math.Sin(Ang) * 200) + 900;
					//SVGParse.addPoint(x, y);
				//}
				//
				//SVGParse.addPoint(1200, 900);
				//
				//
			//}
			//
			//
			//drawLines();
			//updateList();
			//
			//
			//
		//}

		//UPGRADE_NOTE: (7001) The following declaration (Command8_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command8_Click()
		//{
			//
			//
		//}

		//UPGRADE_NOTE: (7001) The following declaration (Command9_Click) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Command9_Click()
		//{
			//
			//
		//}

		//UPGRADE_WARNING: (2080) Form_Load event was upgraded to Form_Load method and has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2080
		private void Form_Load()
		{
			Zoom = 1;
			SVGParse.pData = new SVGParse.typLine[1];


			this.Text = "Av's SVG to GCODE v " + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMajorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMinorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FilePrivatePart.ToString();


			TB1.ImageSource = vbalTBar6.ECTBImageSourceTypes.CTBExternalImageList;
			TB1.SetImageList(vbalImageList1.hIml, vbalTBar6.ECTBImageListTypes.CTBImageListNormal);
			short tempRefParam = 24;
			bool tempRefParam2 = true;
			bool tempRefParam3 = true;
			bool tempRefParam4 = true;
			short tempRefParam5 = 0;
			TB1.CreateToolbar(ref tempRefParam, ref tempRefParam2, ref tempRefParam3, ref tempRefParam4, ref tempRefParam5);

			TB1.AddButton("Open", 0, Type.Missing, 0, "Open", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "open");
			TB1.AddButton("Scale", 1, Type.Missing, 0, "Scale", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "scale");
			TB1.AddButton("Export", 2, Type.Missing, 0, "Export", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "export");

			//.AddButton "Specify the Feed Rate used to cut your design", 10, , , "Feed Rate:", CTBAutoSize, "rate"
			//.AddControl cFeedRate.Hwnd

			TB1.AddButton("Zoom In", 3, Type.Missing, 0, "Zoom In", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "zoomin");
			TB1.AddButton("Zoom Out", 4, Type.Missing, 0, "Zoom Out", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "zoomout");

			TB1.AddButton("Display the Non-Cut Paths", 6, Type.Missing, 0, "Show NonCuts", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize | vbalTBar6.ECTBToolButtonSyle.CTBCheck, "noncut");
			//.AddButton "Plunge the Z (for engraver)", 9, , , "Z Plunge", CTBAutoSize Or CTBCheck, "zplunge"
			TB1.set_ButtonChecked("noncut", true);

			TB1.AddButton("Raster-fill the shapes by line.", 11, Type.Missing, 0, "Raster Fill", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "fill");





			TB1.AddButton("Generate a puzzle", 5, Type.Missing, 0, "Puzzle", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "puzz");
			TB1.AddButton("Split into multiple GCode files by page", 7, Type.Missing, 0, "Split by Pages", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "pages");
			TB1.AddButton("Duplicate the design in multiple rows and columns", 8, Type.Missing, 0, "Tile", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "tile");

			TB1.AddButton("Rotate 90", 12, Type.Missing, 0, "Rotate 90", vbalTBar6.ECTBToolButtonSyle.CTBAutoSize, "rotate90");


			//.AddButton "box", 5, , , "box", CTBAutoSize, "box"



			//        If browseMode = eB_Structure And Not chooserMode Then
			//            .AddButton "Top Node", 10, , , "[Application]", CTBAutoSize Or CTBDropDown, "AppDrop"
			//            .AddButton "Type Browser", 4, , , "Types", CTBAutoSize, "Types"
			//            .AddButton "Relationship Browser", 9, , , "Relationships", CTBAutoSize, "Relationships"
			//            .AddButton "New Object", 11, , , "New Object", CTBAutoSize, "NewObject"
			//
			//            .AddButton "Display tabs as web-based forms", 12, , , "Web Tabs", CTBAutoSize Or CTBCheck, "WebForms"
			//
			//
			//            .AddButton , , , , , CTBSeparator Or CTBAutoSize, "Sep"
			//            .AddButton "System", 3, , , "System", CTBAutoSize Or CTBDropDown, "System"
			//            .AddButton "About", 13, , , "About", CTBAutoSize, "About"
			//        End If
			//
			//        .AddButton "Sort", 6, , , "Sort", CTBAutoSize Or CTBDropDown, "Sort"
			//
			//        .ButtonEnabled("Back") = False
			//        .ButtonEnabled("Forward") = False





			// a) Create the rebar:
			cReBar1.ImageSource = vbalTBar6.ECRBImageSourceTypes.CRBLoadFromFile;
			cReBar1.CreateRebar(this.Handle.ToInt32());

			// b) Add the toolbar & combo boxes.
			// When you add a band, the rebar automatically sets the IdealWidth
			// to the size of the object you've added, and makes the Minimum
			// size the same.  By allowing a smaller minimum size, the rebar
			// will show a chevron when the band is reduced.

			// i) Add the 24x24 toolbar with text:
			cReBar1.AddBandByHwnd(TB1.Handle.ToInt32(), "", true, false, "Toolbar1");
			int tempRefParam6 = 24;
			cReBar1.set_BandChildMinWidth(cReBar1.BandCount - 1, ref tempRefParam6);


			//With cFeedRate
			//    .AddItem "20"
			//    .AddItem "40"
			//    .AddItem "60"
			//    .AddItem "80"
			//    .AddItem "100"
			//End With

			//cFeedRate.Text = 20




		}

		private bool isInitializingComponent;
		private void Form_Resize(Object eventSender, EventArgs eventArgs)
		{
			if (isInitializingComponent)
			{
				return;
			}

			List1.Left = Convert.ToInt32(this.ClientRectangle.Width - List1.Width);
			Picture1.Width = List1.Left - Picture1.Left;
			Picture1.Height = Convert.ToInt32(this.ClientRectangle.Height - Picture1.Top);
			List1.Height = Picture1.Height;

			cReBar1.RebarSize();



			picRulers[0].Width = Picture1.Width;
			picRulers[1].Height = Picture1.Height;

			drawLines();


		}

		private void Form_Closed(Object eventSender, EventArgs eventArgs)
		{
			Environment.Exit(0);
		}

		private void List1_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
		{
			drawLines();
			Picture1.Refresh();

		}

		private void List1_DoubleClick(Object eventSender, EventArgs eventArgs)
		{
			int A = getListLine();
			if (A > 0)
			{
				SVGParse.pData[A].Fillable = !SVGParse.pData[A].Fillable;
				updateList();
			}


		}

		public int getListLine()
		{

			int result = 0;
			int A = ListBoxHelper.GetSelectedIndex(List1);
			if (A > -1)
			{
				result = List1.GetItemData(A);
			}

			return result;
		}

		private void List1_KeyDown(Object eventSender, KeyEventArgs eventArgs)
		{
			int KeyCode = (int) eventArgs.KeyCode;
			int Shift = ((int) eventArgs.KeyData) / 65536;
			try
			{

				int A = 0;
				int j = 0;

				bool doDel = false;


				int tempForEndVar = List1.Items.Count;
				for (int lI = 1; lI <= tempForEndVar; lI++)
				{
					if (ListBoxHelper.GetSelected(List1, lI - 1))
					{
						A = List1.GetItemData(lI - 1);

						if (KeyCode == ((int) Keys.Delete))
						{
							SVGParse.pData[A].isDel = true;
							doDel = true;
						}
					}
				}

				if (doDel)
				{

					j = 0;
					int tempForEndVar2 = SVGParse.pData.GetUpperBound(0);
					for (int i = 1; i <= tempForEndVar2; i++)
					{
						if (SVGParse.pData[i].isDel)
						{
							// Skip this one
						}
						else
						{
							j++;
							SVGParse.pData[j] = SVGParse.pData[i];
						}
					}
					SVGParse.pData = ArraysHelper.RedimPreserve(SVGParse.pData, new int[]{j + 1});

					drawLines();
					updateList();
					//UPGRADE_TODO: (1065) Error handling statement (On Error Resume Next) could not be converted. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1065
					UpgradeHelpers.Helpers.NotUpgradedHelper.NotifyNotUpgradedElement("On Error Resume Next");
					ListBoxHelper.SetSelectedIndex(List1, A - 1);
				}
			}
			finally
			{
				eventArgs.Handled = KeyCode == 0;
			}

		}

		private void List1_MouseDown(Object eventSender, MouseEventArgs eventArgs)
		{
			int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
			int Shift = ((int) Control.ModifierKeys) / 65536;
			float x = eventArgs.X * 15;
			float y = eventArgs.Y * 15;

			int A = 0;
			int R = 0;

			Hashtable b = null;
			OrderedDictionary selLines = new OrderedDictionary(System.StringComparer.OrdinalIgnoreCase);
			string newLayer = "";


			mcPopupMenu mc = new mcPopupMenu();
			if (Button == 2)
			{

				int tempForEndVar = List1.Items.Count;
				for (int i = 1; i <= tempForEndVar; i++)
				{
					if (ListBoxHelper.GetSelected(List1, i - 1))
					{
						A = List1.GetItemData(i - 1);

						if (A > 0)
						{
							selLines.Add(Guid.NewGuid().ToString(), A);
						}
					}
				}

				if (selLines.Count == 1)
				{
					A = (int) selLines[0];
					mc.Add(0, "Layer: " + SVGParse.pData[A].LayerID, false, false, mcPopupMenu.mceItemStates.mceGrayed);
					mc.Add(1, "Fillable", false, SVGParse.pData[A].Fillable);


					//mc.Add 1, "",,pdata(i).

					if (!SVGParse.layerInfo.ContainsKey(SVGParse.pData[A].LayerID))
					{
						SVGParse.layerInfo.Add(SVGParse.pData[A].LayerID, new Hashtable());
					}

					b = (Hashtable) SVGParse.layerInfo[SVGParse.pData[A].LayerID];

					mc.Add(0, "-");
					mc.Add(10, "Pause before layer", false, b.ContainsKey("pausebefore"));
					mc.Add(11, "Defocused Cut Layer", false, b.ContainsKey("defocused"));
					mc.Add(12, "Move Layer to End");
					mc.Add(0, "-");
					mc.Add(20, "Remove Last Segment");
					mc.Add(30, "Set Layer");
					mc.Add(40, "DEBUG: Path Data");

					R = mc.Show();



				}
				else if (selLines.Count > 1)
				{  // Multiple lines selected

					mc.Add(100, selLines.Count.ToString() + " objects selected");
					mc.Add(0, "-");
					mc.Add(1, "Fillable", false, SVGParse.pData[(int) selLines[0]].Fillable);

					mc.Add(30, "Set Layer");

					R = mc.Show();


				}

				switch(R)
				{
					case 30 : 
						newLayer = Interaction.InputBox("Set layer to?", "Set Layer", "", 0, 0); 
						 
						break;
				}

				int tempForEndVar2 = selLines.Count;
				for (int i = 1; i <= tempForEndVar2; i++)
				{
					A = (int) selLines[i - 1];
					switch(R)
					{
						case 0 : 
							return;
						case 1 : 
							SVGParse.pData[A].Fillable = !SVGParse.pData[A].Fillable; 
							break;
						case 10 : 
							if (b.ContainsKey("pausebefore"))
							{
								b.Remove("pausebefore");
							}
							else
							{
								b.Add("pausebefore", true);
							} 
							break;
						case 11 : 
							if (b.ContainsKey("defocused"))
							{
								// turn it off
								b.Remove("defocused");
							}
							else
							{
								R = Convert.ToInt32(Conversion.Val(Interaction.InputBox("How many inches to move down?", "Defocus Cuts", "3", 0, 0)));
								if (R > 0)
								{
									b.Add("defocused", R);
								}
							} 
							break;
						case 12 :  // Move Layer to End 
							newLayer = SVGParse.pData[A].LayerID; 
							break;
						case 20 : 
							// Remove last secmet 
							SVGParse.pData[A].Points = ArraysHelper.RedimPreserve(SVGParse.pData[A].Points, new int[]{SVGParse.pData[A].Points.GetUpperBound(0)}); 
							drawLines(); 
							 
							break;
						case 30 :  // Set layer 
							SVGParse.pData[A].LayerID = newLayer; 
							 
							break;
						case 40 : 
							Clipboard.Clear(); 
							//UPGRADE_WARNING: (2081) Clipboard.SetText has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081 
							Clipboard.SetText(SVGParse.pData[A].PathCode); 
							break;
					}

					switch(R)
					{ // Outside WITH block
						case 12 :  // Move Layer to End 
							// Move all lines that are on this layer to the end. 
							SVGParse.MoveLayerToEnd(newLayer); 
							break;
					}
				}

				switch(R)
				{
					case 30 : 
						SVGParse.optimizePolys(); 
						break;
				}

				updateList();
			}

		}

		private void Picture1_MouseDown(Object eventSender, MouseEventArgs eventArgs)
		{
			int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
			int Shift = ((int) Control.ModifierKeys) / 65536;
			float x = eventArgs.X * 15;
			float y = eventArgs.Y * 15;


			mX = x;
			mY = y;
			oX = panX;
			oY = panY;

			mouseDown = true;


		}

		private void Picture1_MouseMove(Object eventSender, MouseEventArgs eventArgs)
		{
			int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
			int Shift = ((int) Control.ModifierKeys) / 65536;
			float x = eventArgs.X * 15;
			float y = eventArgs.Y * 15;

			//    Dim p1 As pointD
			//    Dim p2 As pointD
			//    Dim a As Double
			//
			//    p1.x = 200
			//    p1.y = 200
			//    p2.x = CDbl(x)
			//    p2.y = CDbl(y)
			//
			//    a = angleFromPoint(p1, p2)
			//    Debug.Print a * (180 / PI)
			//
			//    Picture1.Cls
			//    Picture1.Line (200, 200)-(x, y)
			//


			//        Dim result() As pointD
			//        Dim i As Long
			//
			//    If Button = 1 Then
			//        drawLines
			//        Picture1.ForeColor = vbBlue
			//
			//        Picture1.Line (X, Y)-(mX, mY)
			//
			//        result = lineIntersectPoly(newPoint(CDbl(X), CDbl(Y)), newPoint(CDbl(mX), CDbl(mY)), 3)
			//
			//        Debug.Print UBound(result)
			//        For i = 1 To UBound(result)
			//            Picture1.Circle (result(i).X, result(i).Y), 5
			//        Next
			//
			//        Picture1.Refresh
			//
			//    End If
			//
			//Exit Sub

			if (mouseDown)
			{
				panX = oX + ((x - mX) / Zoom);
				panY = oY + ((y - mY) / Zoom);
				drawLines();
				Picture1.Refresh();


			}
		}


		private void fillPoly(int lineID, bool useBlack)
		{

			// Get the bounds of this shape.
			double maxX = 0, maxY = 0;
			double minX = 0, minY = 0;

			SVGParse.getPolyBounds(lineID, ref minX, ref minY, ref maxX, ref maxY);

			double tempForEndVar = maxX;
			for (double x = minX; x <= tempForEndVar; x++)
			{
				double tempForEndVar2 = maxY;
				for (double y = minY; y <= tempForEndVar2; y++)
				{

					if (SVGParse.pointIsInPoly(lineID, x, y))
					{
						//UPGRADE_ISSUE: (2064) PictureBox method Picture1.PSet was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
						Picture1.PSet((float) x, (float) y, ColorTranslator.ToOle((useBlack) ? Color.Red : Color.White));

					}
				}
				Application.DoEvents();

			}


		}


		private void doFills()
		{

			// Iterate through the polygons that can be filled
			// First, figure out if any polygons are inside any other polygons.

			double matchArea = 0;
			double bestMatchArea = 0;
			int bestMatchID = 0;

			SVGParse.containList = new Hashtable();


			int tempForEndVar = SVGParse.pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (SVGParse.pData[i].Fillable)
				{



					bestMatchArea = 0;
					bestMatchID = 0;

					// Find which possible polygons might contain me
					int tempForEndVar2 = SVGParse.pData.GetUpperBound(0);
					for (int j = 1; j <= tempForEndVar2; j++)
					{
						if (i != j && SVGParse.pData[j].Fillable && SVGParse.pData[j].LayerID == SVGParse.pData[i].LayerID)
						{


							if (SVGParse.canPolyFitInside(i, j))
							{

								// Figure out how big this polygon is
								matchArea = SVGParse.getPolyArea(j);
								if (matchArea < bestMatchArea || bestMatchID == 0)
								{
									bestMatchArea = matchArea;
									bestMatchID = j;
								}
							}
						}

					}

					if (bestMatchID > 0)
					{
						// Found a match.
						SVGParse.pData[i].ContainedBy = bestMatchID;

						if (!SVGParse.containList.ContainsKey(SVGParse.pData[i].ContainedBy))
						{
							SVGParse.containList.Add(SVGParse.pData[i].ContainedBy, new OrderedDictionary(System.StringComparer.OrdinalIgnoreCase));
						}

						ReflectionHelper.Invoke(SVGParse.containList[SVGParse.pData[i].ContainedBy], "Add", new object[]{i});
					}
				}

				this.Text = "Checking inside " + i.ToString() + " / " + SVGParse.pData.GetUpperBound(0).ToString();
				if (i % 20 == 0)
				{
					Application.DoEvents();
				}

			}


			// Now that we have the shapes and who contains them, reorder the list so that the lowest level of contained ones are cut first.
			SetLevelNumber(0, 0);



			// Now sort by level number going down
			this.Text = "Sorting by level number...";
			bool sorted = false;
			do 
			{
				sorted = false;
				int tempForEndVar3 = SVGParse.pData.GetUpperBound(0) - 1;
				for (int i = 1; i <= tempForEndVar3; i++)
				{
					if (SVGParse.pData[i].LevelNumber < SVGParse.pData[i + 1].LevelNumber && SVGParse.pData[i].LayerID == SVGParse.pData[i + 1].LayerID)
					{ // Swap!
						SVGParse.SwapLine(ref SVGParse.pData[i + 1], ref SVGParse.pData[i]);
						sorted = true;
					}
				}
			}
			while(sorted);
			//fillAll 0, True


			//updateList


		}

		private void SetLevelNumber(int Container_Renamed, int LevelNum)
		{

			int tempForEndVar = SVGParse.pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (SVGParse.pData[i].ContainedBy == Container_Renamed)
				{
					SVGParse.pData[i].LevelNumber = LevelNum;
					SetLevelNumber(i, LevelNum + 1);
				}
			}



		}

		public object fillAll(int containerID, bool fillWith)
		{

			// Fill all poly's with containerID as specified
			int tempForEndVar = SVGParse.pData.GetUpperBound(0);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				if (SVGParse.pData[i].Fillable && SVGParse.pData[i].ContainedBy == containerID)
				{

					fillPoly(i, fillWith);

					fillAll(i, !fillWith);

				}
			}
			return null;
		}

		public object updateList()
		{
			string tLayer = "";

			Hashtable b = null;
			Hashtable c = null;

			try
			{

				tLayer = "---";

				List1.Items.Clear();
				int tempForEndVar = SVGParse.pData.GetUpperBound(0);
				for (int i = 1; i <= tempForEndVar; i++)
				{
					if (SVGParse.pData[i].LayerID != tLayer)
					{


						if (!SVGParse.layerInfo.ContainsKey(SVGParse.pData[i].LayerID))
						{
							c = new Hashtable();
							SVGParse.layerInfo.Add(SVGParse.pData[i].LayerID, c);
						}

						b = (Hashtable) SVGParse.layerInfo[SVGParse.pData[i].LayerID];

						List1.AddItem("[Layer " + SVGParse.pData[i].LayerID + " " + ((b.ContainsKey("pausebefore")) ? "PAUSE" : "") + "]");
						tLayer = SVGParse.pData[i].LayerID;
					}
					List1.AddItem("   Line " + i.ToString() + ": " + ((SVGParse.pData[i].Fillable) ? "F" : "") + " (" + SVGParse.pData[i].Points.GetUpperBound(0).ToString() + " segs) in " + SVGParse.pData[i].ContainedBy.ToString());
					List1.SetItemData(List1.GetNewIndex(), i);
				}
			}
			catch (System.Exception excep)
			{

				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				MessageBox.Show("Error " + Information.Err().Number.ToString() + " (" + excep.Message + ") in procedure updateList of Form frmInterface", Application.ProductName + " ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				GeneralFunctions.addToLog("[ERROR]", "In updateList of Form frmInterface", Information.Err().Number, excep.Message);
			}

			return null;
		}

		private void Picture1_MouseUp(Object eventSender, MouseEventArgs eventArgs)
		{
			int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
			int Shift = ((int) Control.ModifierKeys) / 65536;
			float x = eventArgs.X * 15;
			float y = eventArgs.Y * 15;
			mouseDown = false;

		}

		//UPGRADE_NOTE: (7001) The following declaration (Timer1_Timer) seems to be dead code More Information: https://www.mobilize.net/vbtonet/ewis/ewi7001
		//private void Timer1_Timer()
		//{
			//
			//int y = 0;
			//int lx = 0, ly = 0;
			//
			//int phase = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds * 200);
			//
			////UPGRADE_ISSUE: (2064) PictureBox method Picture1.Cls was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			//Picture1.Cls();
			//
			//for (int x = 1; x <= 200; x++)
			//{
				//y = Convert.ToInt32((Math.Sin((x + phase) / 20d) * 50) + 50);
				//using (Graphics g = Picture1.CreateGraphics())
				//{
				//
					//g.DrawLine(new Pen(new Color()), lx, ly, x, y);
				//}
				//lx = x;
				//ly = y;
			//}
			//
			//lx = 0;
			//ly = 0;
			//
			//
			//for (int x = 1; x <= 200; x++)
			//{
				//y = Convert.ToInt32((Math.Sin((x + phase) / 30d) * 50) + 50);
				//using (Graphics g = Picture1.CreateGraphics())
				//{
				//
					//g.DrawLine(new Pen(new Color()), lx, ly, x, y);
				//}
				//lx = x;
				//ly = y;
			//}
			//
		//}



		private void cmdOpenFile()
		{



			string fName = "";

			//UPGRADE_ISSUE: (6012) CommonDialog variable was not upgraded More Information: https://www.mobilize.net/vbtonet/ewis/ewi6012
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.Filter was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			//UPGRADE_WARNING: (2081) Filter has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
			COMDLGOpen.Filter = "Supported Files (*.svg, Images)|*.svg;*.bmp;*.jpg;*.gif";
			COMDLGSave.Filter = "Supported Files (*.svg, Images)|*.svg;*.bmp;*.jpg;*.gif";
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.CancelError was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			COMDLG.setCancelError(false);
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.DialogTitle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			COMDLGOpen.Title = "Open File";
			COMDLGSave.Title = "Open File";
			COMDLGOpen.ShowDialog();
			COMDLGSave.FileName = COMDLGOpen.FileName;
			//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.FileName was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			fName = COMDLGOpen.FileName;

			if (fName != "")
			{

				SVGParse.CurrentFile = fName;

				SVGParse.LastExportPath = "";


				// Reset pan and zoom settings

				switch(GeneralFunctions.getFileExten(fName))
				{
					case "svg" : 

						 
						SVGParse.layerInfo.Clear(); 

						 
						SVGParse.parseSVG(fName); 

						 
						Debug.WriteLine(Support.TabLayout("Drawing ", SVGParse.pData.GetUpperBound(0).ToString())); 
						 
						// Optimize the shapes for best drawing 
						//sortByLayers 
						 
						SVGParse.mergeConnectedLines(); 

						 
						SVGParse.optimizePolys(); 
						 
						break;
					case "jpg" : case "bmp" : case "gif" : 
						// Open and rasterize this file 
						Rasterize.rasterFile(fName); 

						 
						break;
					default:
						 
						break;
				}

				panX = 0;
				panY = 0;
				Zoom = 1;


				doFills();

				zoomToFit();

				SVGParse.getExtents(ref SVGParse.EXPORT_EXTENTS_X, ref SVGParse.EXPORT_EXTENTS_Y);



				updateList();


			}


			this.Text = "Av's SVG to GCODE v " + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMajorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileMinorPart.ToString() + "." + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FilePrivatePart.ToString();

		}

		private void cmdExport()
		{
			frmExport.DefInstance.Show(this);


		}

		public void updateRulers()
		{

			// Draw the rulers.

			int n = 0;
			double n1 = 0;

			double pixelStep = 1;
			do 
			{
				n1 = measureToRuler(pixelStep, true) - measureToRuler(0, true);
				if (n1 < 30)
				{
					pixelStep++;
				}
			}
			while(n1 < 30);

			// Calculate the inside step?
			n1 = measureToRuler(pixelStep, true) - measureToRuler(0, true);

			//addtolog "The raw pixel difference between each interval is: ", n1

			int insideStep = 20;
			double emph = 10;
			if (n1 < 70)
			{
				insideStep = 10;
				emph = 5;
			}
			if (n1 < 35)
			{
				insideStep = 5;
			}

			//Exit Sub


			// HORIZONTAL RULER
			//UPGRADE_ISSUE: (2064) PictureBox method picRulers.Cls was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			picRulers[0].Cls();
			double rStart = rulerToMeasure(0, true);
			double rEnd = rulerToMeasure(picRulers[0].Width, true);
			// Round down and up to find the nearest ruler points.
			rStart = Math.Floor(rStart / pixelStep) * pixelStep;
			rEnd = (Math.Floor(rEnd / pixelStep) + 1) * pixelStep;
			double tempForEndVar = rEnd;
			double tempForStepVar = (pixelStep / insideStep);
			for (double i = rStart; (tempForStepVar < 0) ? i >= tempForEndVar : i <= tempForEndVar; i += tempForStepVar)
			{
				n = measureToRuler(i, true);
				if ((Convert.ToInt32(i * 10000)) % (Convert.ToInt32(pixelStep * 10000)) == 0)
				{
					using (Graphics g = picRulers[0].CreateGraphics())
					{

						g.DrawLine(new Pen(new Color()), n, 0, n, 16);
					}
					//UPGRADE_ISSUE: (2064) PictureBox method picRulers.PSet was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					picRulers[0].PSet(n + 1, 2, ColorTranslator.ToOle(picRulers[0].BackColor));
					//UPGRADE_ISSUE: (2064) PictureBox method picRulers.Print was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					picRulers[0].Print(Math.Abs(Math.Round((double) i, 0)).ToString());
				}
				else
				{
					if (Convert.ToInt32(Math.Round((double) (i / (pixelStep / insideStep)), 0)) % Convert.ToInt32(emph) == 0)
					{
						using (Graphics g = picRulers[0].CreateGraphics())
						{

							g.DrawLine(new Pen(new Color()), n, 11, n, 16);
						}
					}
					else
					{
						using (Graphics g = picRulers[0].CreateGraphics())
						{

							g.DrawLine(new Pen(new Color()), n, 13, n, 16);
						}
					}

				}
			}

			// VERTICAL RULER
			//UPGRADE_ISSUE: (2064) PictureBox method picRulers.Cls was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
			picRulers[1].Cls();
			rStart = rulerToMeasure(0, false);
			rEnd = rulerToMeasure(picRulers[1].Height, false);
			// Round down and up to find the nearest ruler points.
			rStart = Math.Floor(rStart / pixelStep) * pixelStep;
			rEnd = (Math.Floor(rEnd / pixelStep) + 1) * pixelStep;
			double tempForEndVar2 = rEnd;
			double tempForStepVar2 = (pixelStep / insideStep);
			for (double i = rStart; (tempForStepVar2 < 0) ? i >= tempForEndVar2 : i <= tempForEndVar2; i += tempForStepVar2)
			{
				n = measureToRuler(i, false);
				if ((Convert.ToInt32(i * 10000)) % (Convert.ToInt32(pixelStep * 10000)) == 0)
				{
					using (Graphics g = picRulers[1].CreateGraphics())
					{

						g.DrawLine(new Pen(new Color()), 0, n, 16, n);
					}
					//UPGRADE_ISSUE: (2064) PictureBox method picRulers.PSet was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					picRulers[1].PSet(0, n + 1, ColorTranslator.ToOle(picRulers[0].BackColor));

					//UPGRADE_ISSUE: (2064) PictureBox method picRulers.Print was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
					picRulers[1].Print(addEnters(Math.Abs(Math.Round((double) i, 0)).ToString()));
				}
				else
				{
					if (Convert.ToInt32(Math.Round((double) (i / (pixelStep / insideStep)), 0)) % Convert.ToInt32(emph) == 0)
					{
						using (Graphics g = picRulers[1].CreateGraphics())
						{

							g.DrawLine(new Pen(new Color()), 11, n, 16, n);
						}
					}
					else
					{
						using (Graphics g = picRulers[1].CreateGraphics())
						{

							g.DrawLine(new Pen(new Color()), 13, n, 16, n);
						}
					}
				}
			}


			picRulers[0].Refresh();
			picRulers[1].Refresh();

		}

		public string addEnters(string inSt)
		{
			// Add a line feed after each letter
			string result = "";
			int tempForEndVar = Strings.Len(inSt);
			for (int i = 1; i <= tempForEndVar; i++)
			{
				result = result + inSt.Substring(i - 1, Math.Min(1, inSt.Length - (i - 1))) + Environment.NewLine;
			}
			return result;
		}

		public int measureToRuler(double inMeas, bool isX)
		{
			// Turn a measure value into an on-screen pixel value for the ruler.
			if (isX)
			{
				return Convert.ToInt32((inMeas + panX) * Zoom);
			}
			else
			{
				return Convert.ToInt32((inMeas + panY) * Zoom);
			}

		}

		public double rulerToMeasure(int inPx, bool isX)
		{

			// Calculate the measure at the specified ruler value, based on the scroller position and such.
			if (isX)
			{
				return (inPx / Zoom) - panX;
			}
			else
			{
				return (inPx / Zoom) - panY;
			}


		}


		public object cmdScale()
		{

			// Scale the work

			double maxX = 0, maxY = 0;
			double scalarW = 0, scalarH = 0;

			SVGParse.getExtents(ref maxX, ref maxY);

			if (maxX == 0 || maxY == 0)
			{
				return null;
			}

			frmScale tempLoadForm = frmScale.DefInstance;
			frmScale.DefInstance.originalAspect = maxX / maxY;
			frmScale.DefInstance.originalW = maxX;
			frmScale.DefInstance.originalH = maxY;
			frmScale.DefInstance.updatingValue = true;
			frmScale.DefInstance.txtWidth.Text = Math.Round((double) maxX, 5).ToString();
			frmScale.DefInstance.txtHeight.Text = Math.Round((double) maxY, 5).ToString();
			frmScale.DefInstance.updatingValue = false;
			frmScale.DefInstance.ShowDialog(this);

			double w = frmScale.DefInstance.setW;
			double H = frmScale.DefInstance.setH;
			frmScale.DefInstance.Close();



			if (w > 0 && H > 0)
			{

				scalarW = w / maxX;
				scalarH = H / maxY;

				int tempForEndVar = SVGParse.pData.GetUpperBound(0);
				for (int i = 1; i <= tempForEndVar; i++)
				{
					int tempForEndVar2 = SVGParse.pData[i].Points.GetUpperBound(0);
					for (int j = 1; j <= tempForEndVar2; j++)
					{
						SVGParse.pData[i].Points[j].x = scalarW * SVGParse.pData[i].Points[j].x;
						SVGParse.pData[i].Points[j].y = scalarH * SVGParse.pData[i].Points[j].y;
					}
				}
			}

			zoomToFit();

			return null;
		}

		public object zoomToFit()
		{

			// Fit the object on the screen
			double maxX = 0, maxY = 0;
			SVGParse.getExtents(ref maxX, ref maxY);

			if (maxX == 0 || maxY == 0)
			{
				return null;
			}

			Zoom = GeneralFunctions.Min(Picture1.Width / maxX, Picture1.Height / maxY) * 0.95d;

			// Set the pans to center it
			panY = ((Picture1.Height / 2d) - ((maxY * Zoom) / 2)) / Zoom;
			panX = ((Picture1.Width / 2d) - ((maxX * Zoom) / 2)) / Zoom;

			drawLines();


			return null;
		}

		private void TB1_ButtonClick(Object eventSender, AxvbalTBar6.__cToolbar_ButtonClickEvent eventArgs)
		{




			switch(TB1.get_ButtonKey(eventArgs.lButton))
			{
				case "open" : 
					cmdOpenFile(); 
					break;
				case "export" : 
					cmdExport(); 

					 
					break;
				case "zoomin" : 
					 
					panX -= (Picture1.Width / 4) / Zoom; 
					panY -= (Picture1.Height / 4) / Zoom; 
					 
					Zoom *= 2; 
					drawLines(); 
					 
					break;
				case "zoomout" : 

					 
					Zoom /= 2; 
					 
					panX += (Picture1.Width / 4) / Zoom; 
					panY += (Picture1.Height / 4) / Zoom; 
					 
					drawLines(); 
					 
					break;
				case "scale" : 
					cmdScale(); 
					 
					break;
				case "fill" : 
					cmdFill(); 
					 
					break;
				case "puzz" : 
					 
					//UPGRADE_ISSUE: (1046) MsgBox Parameter 'context' is not supported, and was removed. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1046 
					//UPGRADE_ISSUE: (1046) MsgBox Parameter 'helpfile' is not supported, and was removed. More Information: https://www.mobilize.net/vbtonet/ewis/ewi1046 
					if (((DialogResult) Interaction.MsgBox("Create a Puzzle design? This will erase your current design.", MsgBoxStyle.YesNo | MsgBoxStyle.Question | MsgBoxStyle.DefaultButton2, "")) == System.Windows.Forms.DialogResult.Yes)
					{
						doPuzzle();
					} 
					 
					break;
				case "pages" : 
					 
					doPages(); 
					 
					break;
				case "box" : 
					 
					//doBox 
					 
					break;
				case "tile" : 
					frmTile.DefInstance.Show(); 
					 
					break;
				case "noncut" : 
					drawLines(); 
					 
					break;
				case "rotate90" : 
					 
					 
					int tempForEndVar = SVGParse.pData.GetUpperBound(0); 
					for (int i = 1; i <= tempForEndVar; i++)
					{
						int tempForEndVar2 = SVGParse.pData[i].Points.GetUpperBound(0);
						for (int j = 1; j <= tempForEndVar2; j++)
						{
							GeneralFunctions.Swap(ref SVGParse.pData[i].Points[j].x, ref SVGParse.pData[i].Points[j].y);
							// Invert the Y
							SVGParse.pData[i].Points[j].y = 17 - SVGParse.pData[i].Points[j].y;
						}
					} 
					 
					drawLines(); 



					 
					break;
			}
		}

		public void cmdFill()
		{

			// Calculate who is in what
			doFills();


			double maxX = 0, maxY = 0;
			SVGParse.getExtents(ref maxX, ref maxY);

			if (maxX > 24 || maxY > 18)
			{
				MessageBox.Show("Your document is too big to fit on the laser. Please scale first.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

				return;
			}

			double D = Conversion.Val(Interaction.InputBox("How far apart should the raster lines be in inches?", "Raster DPI", "0.004", 0, 0));
			if (D < 0)
			{
				return;
			}


			//For i = 1 To UBound(pData)
			//    If pData(i).ContainedBy = 0 And pData(i).Fillable Then
			//        Debug.Print "Rasterizing ", i
			//        rasterLinePoly i, D, "Fill"
			//    End If
			//Next

			SVGParse.rasterDocument(D, "Fill");



			drawLines();
			updateList();

		}

		public void doPuzzle()
		{


			// Piece width and height

			double pX = 0, pY = 0;


			int pieceTypes = 6;

			// Temp defs
			double puzzW = 5;
			double puzzH = 5;
			double piecesW = 4;
			double piecesH = 4;

			double pieceW = puzzW / piecesW;
			double pieceH = puzzH / piecesH;

			SVGParse.pData = new SVGParse.typLine[1];


			VBMath.Randomize();
			double tempForEndVar = piecesH;
			for (double y = 1; y <= tempForEndVar; y++)
			{
				double tempForEndVar2 = piecesW;
				for (double x = 1; x <= tempForEndVar2; x++)
				{

					if (y == piecesH && x == piecesW)
					{
						break;
					}

					pX = x * pieceW;
					pY = (y - 1) * pieceH;

					// Go to the middle
					SVGParse.newLine();

					if (x < piecesW)
					{

						SVGParse.addPoint(pX, pY);

						drawPuzzleEdge(Convert.ToInt32(Math.Floor((double) (VBMath.Rnd() * pieceTypes)) + 1), true, VBMath.Rnd() < 0.5f, pX, pY, pieceW, pieceH);


					}
					else
					{

					}

					// Vertical pieces
					pY += pieceH;

					SVGParse.addPoint(pX, pY);

					if (y < piecesH)
					{
						// Bottom row doesn't get bottom pieces
						drawPuzzleEdge(Convert.ToInt32(Math.Floor((double) (VBMath.Rnd() * pieceTypes)) + 1), false, VBMath.Rnd() < 0.5f, pX, pY, pieceW, pieceH);
					}

					SVGParse.addPoint(pX - pieceW, pY);


				}
			}

			// Cut out the box
			SVGParse.newLine();
			SVGParse.addPoint(0, 0);
			SVGParse.addPoint(puzzW, 0);
			SVGParse.addPoint(puzzW, puzzH);
			SVGParse.addPoint(0, puzzH);
			SVGParse.addPoint(0, 0);


			SVGParse.optimizePolys();
			zoomToFit();

			updateList();


		}

		public object drawPuzzleEdge(int pShape, bool isHoriz, bool flipYes, double sX, double sY, double pW, double pH)
		{

			// Define the puzzle shapes
			string[] puzzShapes = ArraysHelper.InitializeArray<string>(9);
			string[] pont = null;

			// Shapes are defined horizontally starting from the right and protrusion going up
			// Diagonal box
			puzzShapes[1] = "40,0; 40,10; 30,20; 50,40; 70,20; 60,10; 60,0; 100,0";

			// Regular box
			puzzShapes[2] = "40,0; 40,10; 30,10; 30,30; 70,30; 70,10; 60,10; 60,0; 100,0";

			// U joint
			puzzShapes[3] = "40,0; 40,10; 30,10; 30,40; 45,40; 45,30; 55,30; 55,40; 70,40; 70,10; 60,10; 60,0; 100,0";

			// Arrow
			puzzShapes[4] = "40,0; 40,10; 30,10; 50,40; 70,10; 60,10; 60,0; 100,0";

			// Edge pieces
			puzzShapes[5] = "10,0; 10,40; 20,40; 20,-40; 10,-50; 50,-50; 40,-40; 40,40; 50,40; 50,0; 100,0";

			//
			puzzShapes[6] = "30,0; 70,40; 90,20; 80,10; 70,20; 50, 0";


			string[] puzzPieces = (string[]) puzzShapes[pShape].Split(new string[]{"; "}, StringSplitOptions.None);

			double scalar = (VBMath.Rnd() * 0.2d) + 0.4d;
			double offset = (VBMath.Rnd() * 0.4d) + 0.2d;


			// If horizontal, go down
			if (isHoriz)
			{
				for (int i = 0; i <= puzzPieces.GetUpperBound(0); i++)
				{
					pont = (string[]) puzzPieces[i].Split(',');
					// Add this point
					SVGParse.addPoint((Conversion.Val(pont[1]) / 100 * pW * scalar) * ((flipYes) ? -1 : 1) + sX, (Conversion.Val(pont[0]) / 100 * pH * scalar) + sY + (pH * scalar * offset));
				}

			}
			else
			{
				for (int i = 0; i <= puzzPieces.GetUpperBound(0); i++)
				{
					pont = (string[]) puzzPieces[i].Split(',');
					// Add this point
					SVGParse.addPoint(((100 - Conversion.Val(pont[0])) / 100 * pW * scalar) + sX - pW + (pW * scalar * offset), (Conversion.Val(pont[1]) / 100 * pH * scalar) * ((flipYes) ? -1 : 1) + sY);
				}


			}


			return null;
		}


		public object doPages()
		{

			// Look for a layer called 'Cut Boxes'
			bool hasCutBoxes = false;
			string fName = "";
			double ppW = 0, ppX = 0, ppY = 0, ppH = 0; // Not actually W and H, but X2 and Y2 really.
			int pCount = 0; // Page count

			int n = 0;
			bool lastPointInside = false;
			SVGParse.pointD intersect = new SVGParse.pointD();
			SVGParse.pointD testPoint = new SVGParse.pointD();
			SVGParse.pointD testPoint2 = new SVGParse.pointD();
			SVGParse.typLine[] pBackup = null;
			string lastLineLeft = "";
			string thisLineLeft = "";
			SVGParse.typLine[] thePage = null;

			string fileSavePath = "";

			try
			{

				int tempForEndVar = SVGParse.pData.GetUpperBound(0);
				for (int i = 1; i <= tempForEndVar; i++)
				{
					if (SVGParse.pData[i].LayerID == "Cut Boxes")
					{
						hasCutBoxes = true;
						break;
					}
				}

				if (!hasCutBoxes)
				{
					MessageBox.Show("This function will take the shapes on the layer 'Cut Boxes' and will export a GCode file for each one, with only the lines that occur inside that box.  No 'Cut Boxes' layer was found in your document.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return null;
				}



				// Ask where to save the files

				//UPGRADE_ISSUE: (6012) CommonDialog variable was not upgraded More Information: https://www.mobilize.net/vbtonet/ewis/ewi6012
				//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.FileName was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				COMDLGOpen.FileName = GeneralFunctions.getFolderNameFromPath(COMDLGOpen.FileName) + "\\" + GeneralFunctions.getFileNameNoExten(GeneralFunctions.getFileNameFromPath(COMDLGOpen.FileName)) + "-PageNN.ngc";
				COMDLGSave.FileName = GeneralFunctions.getFolderNameFromPath(COMDLGOpen.FileName) + "\\" + GeneralFunctions.getFileNameNoExten(GeneralFunctions.getFileNameFromPath(COMDLGOpen.FileName)) + "-PageNN.ngc";
				//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.Filter was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				//UPGRADE_WARNING: (2081) Filter has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				COMDLGOpen.Filter = "GCODE Files (*.ngc)|*.ngc";
				COMDLGSave.Filter = "GCODE Files (*.ngc)|*.ngc";
				//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.DialogTitle was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				COMDLGOpen.Title = "Export GCODE - NN will be replaced with page number.";
				COMDLGSave.Title = "Export GCODE - NN will be replaced with page number.";
				COMDLGSave.ShowDialog();
				COMDLGOpen.FileName = COMDLGSave.FileName;
				//UPGRADE_ISSUE: (2064) MSComDlg.CommonDialog property COMDLG.FileName was not upgraded. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2064
				fName = COMDLGOpen.FileName;

				if (fName == "")
				{
					return null;
				}
				if ((fName.IndexOf("NN.ngc", StringComparison.CurrentCultureIgnoreCase) + 1) == 0)
				{
					MessageBox.Show("The filename must end with 'NN.ngc'.  The NN will be replaced with the page number for each page saved.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}

				// Do each page in Cut Boxes.
				int tempForEndVar2 = SVGParse.pData.GetUpperBound(0);
				for (int p = 1; p <= tempForEndVar2; p++)
				{
					if (SVGParse.pData[p].LayerID == "Cut Boxes")
					{

						SVGParse.getPolyBounds(p, ref ppX, ref ppY, ref ppW, ref ppH);

						pCount++;

						// Clear the array
						thePage = new SVGParse.typLine[1];

						// Loop through each polygon and copy it onto our page.
						int tempForEndVar3 = SVGParse.pData.GetUpperBound(0);
						for (int i = 1; i <= tempForEndVar3; i++)
						{

							if (SVGParse.pData[i].LayerID != "Cut Boxes")
							{
								n = 0;
								lastPointInside = false;
								lastLineLeft = "";

								// Loop through each line segment of the polygon. If the line segment fits onto the page, copy it into our new polygon.
								int tempForEndVar4 = SVGParse.pData[i].Points.GetUpperBound(0);
								for (int j = 1; j <= tempForEndVar4; j++)
								{


									if (SVGParse.pData[i].Points[j].x >= ppX && SVGParse.pData[i].Points[j].x <= ppW && SVGParse.pData[i].Points[j].y >= ppY && SVGParse.pData[i].Points[j].y <= ppH)
									{

										if (n == 0)
										{
											// Create a new poly
											n = thePage.GetUpperBound(0) + 1;
											thePage = ArraysHelper.RedimPreserve(thePage, new int[]{n + 1});
											thePage[n].Points = new SVGParse.pointD[1];
										}

										// Was the last point NOT inside?
										if (!lastPointInside && j > 1)
										{ // Also, this can't be the first point.

											// Test 1: Top segment
											testPoint.x = ppX;
											testPoint.y = ppY;
											testPoint2.x = ppW;
											testPoint2.y = ppY;
											intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
											thisLineLeft = "T";
											if (intersect.x == -6666)
											{

												// Right side
												testPoint.x = ppW;
												testPoint.y = ppY;
												testPoint2.x = ppW;
												testPoint2.y = ppH;
												intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
												thisLineLeft = "R";
												if (intersect.x == -6666)
												{
													// Bottom
													testPoint.x = ppX;
													testPoint.y = ppH;
													testPoint2.x = ppW;
													testPoint2.y = ppH;
													intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
													thisLineLeft = "B";
													if (intersect.x == -6666)
													{
														// Left
														testPoint.x = ppX;
														testPoint.y = ppY;
														testPoint2.x = ppX;
														testPoint2.y = ppH;
														intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
														thisLineLeft = "L";
													}
												}
											}

											if (intersect.x != -6666)
											{ // Did intersect.
												if ((lastLineLeft == "T" && thisLineLeft == "L") || (lastLineLeft == "L" && thisLineLeft == "T"))
												{
													addPoint3(thePage[n], ppX, ppY);
												}
												if ((lastLineLeft == "L" && thisLineLeft == "B") || (lastLineLeft == "B" && thisLineLeft == "L"))
												{
													addPoint3(thePage[n], ppX, ppH);
												}
												if ((lastLineLeft == "B" && thisLineLeft == "R") || (lastLineLeft == "R" && thisLineLeft == "B"))
												{
													addPoint3(thePage[n], ppW, ppH);
												}
												if ((lastLineLeft == "T" && thisLineLeft == "R") || (lastLineLeft == "R" && thisLineLeft == "T"))
												{
													addPoint3(thePage[n], ppW, ppY);
												}
												addPoint2(thePage[n], intersect);
											}
										}
										addPoint2(thePage[n], SVGParse.pData[i].Points[j]);
										lastPointInside = true;
									}
									else
									{

										// Was the point previous to this one inside?
										if (lastPointInside)
										{
											// Figure out where the line between this point and the last point intersects the borders.

											// Which segment did it intersect?

											// Test 1: Top segment
											testPoint.x = ppX;
											testPoint.y = ppY;
											testPoint2.x = ppW;
											testPoint2.y = ppY;
											intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
											lastLineLeft = "T";
											if (intersect.x == -6666)
											{

												// Right side
												testPoint.x = ppW;
												testPoint.y = ppY;
												testPoint2.x = ppW;
												testPoint2.y = ppH;
												intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
												lastLineLeft = "R";
												if (intersect.x == -6666)
												{
													// Bottom
													testPoint.x = ppX;
													testPoint.y = ppH;
													testPoint2.x = ppW;
													testPoint2.y = ppH;
													intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
													lastLineLeft = "B";
													if (intersect.x == -6666)
													{
														// Left
														testPoint.x = ppX;
														testPoint.y = ppY;
														testPoint2.x = ppX;
														testPoint2.y = ppH;
														intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
														lastLineLeft = "L";
													}
												}
											}

											if (intersect.x != -6666)
											{ // Did intersect.
												addPoint2(thePage[n], intersect);
											}

										}
										lastPointInside = false;

									}

								}
							}
						}

						// Add the page outline to the document.
						n = thePage.GetUpperBound(0) + 1;
						thePage = ArraysHelper.RedimPreserve(thePage, new int[]{n + 1});
						thePage[n].Points = new SVGParse.pointD[1];

						addPoint3(thePage[n], ppX, ppY);
						addPoint3(thePage[n], ppX, ppH);
						addPoint3(thePage[n], ppW, ppH);
						addPoint3(thePage[n], ppW, ppY);
						addPoint3(thePage[n], ppX, ppY);

						// Backup the shapes
						pBackup = (SVGParse.typLine[]) ArraysHelper.DeepCopy(SVGParse.pData);
						// save out just this page
						SVGParse.pData = thePage;

						// Subtract the X and Y
						int tempForEndVar5 = SVGParse.pData.GetUpperBound(0);
						for (int i = 1; i <= tempForEndVar5; i++)
						{
							int tempForEndVar6 = SVGParse.pData[i].Points.GetUpperBound(0);
							for (int j = 1; j <= tempForEndVar6; j++)
							{
								SVGParse.pData[i].Points[j].x -= ppX;
								SVGParse.pData[i].Points[j].y -= ppY;
							}
						}

						// If the shape is taller than our laser, rotate it 90 degrees
						if (ppH - ppY > 17.5d)
						{ // Inches
							int tempForEndVar7 = SVGParse.pData.GetUpperBound(0);
							for (int i = 1; i <= tempForEndVar7; i++)
							{
								int tempForEndVar8 = SVGParse.pData[i].Points.GetUpperBound(0);
								for (int j = 1; j <= tempForEndVar8; j++)
								{
									GeneralFunctions.Swap(ref SVGParse.pData[i].Points[j].x, ref SVGParse.pData[i].Points[j].y);
									// Invert the Y
									SVGParse.pData[i].Points[j].y = (ppW - ppX) - SVGParse.pData[i].Points[j].y;
								}
							}
						}



						// Show it on screen so you can see what is being saved.
						zoomToFit();

						fileSavePath = Strings.Replace(fName, "NN.ngc", StringsHelper.Format(pCount, "000") + ".ngc", 1, -1, CompareMethod.Text);
						SVGParse.exportGCODE(fileSavePath, 20, false, false, 0, false, 0, 0);
						//MsgBox "Page " & pCount & " - Shape " & p

						Application.DoEvents();
						UpgradeSolution1Support.PInvoke.SafeNative.kernel32.Sleep(100);

						// Restore the data
						SVGParse.pData = pBackup;
						//If pCount = 3 Then Exit Function

					}
				}


				zoomToFit();
			}
			catch (System.Exception excep)
			{

				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				MessageBox.Show("Error " + Information.Err().Number.ToString() + " (" + excep.Message + ") in procedure doPages of Form frmInterface", Application.ProductName + " ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//UPGRADE_WARNING: (2081) Err.Number has a new behavior. More Information: https://www.mobilize.net/vbtonet/ewis/ewi2081
				GeneralFunctions.addToLog("[ERROR]", "In doPages of Form frmInterface", Information.Err().Number, excep.Message);
			}


			return null;
		}


		public object doPagesOld()
		{
			// Split the document up into a series of pages.

			double maxX = 0, maxY = 0;
			SVGParse.typLine[] thePage = null;

			double ppW = 0, ppX = 0, ppY = 0, ppH = 0;
			int n = 0;
			bool lastPointInside = false;
			SVGParse.pointD intersect = new SVGParse.pointD();
			SVGParse.pointD testPoint = new SVGParse.pointD();
			SVGParse.pointD testPoint2 = new SVGParse.pointD();
			int pCount = 0;
			SVGParse.typLine[] pBackup = null;

			string lastLineLeft = "";
			string thisLineLeft = "";



			// Configure for 11x17 paper
			const int pageW = 99;
			const int pageH = 250;

			// How many pages will we need?

			SVGParse.getExtents(ref maxX, ref maxY);

			int pageCountX = Convert.ToInt32(-Math.Floor((-maxX) / pageW));
			int pageCountY = Convert.ToInt32(-Math.Floor((-maxY) / pageH));

			if (MessageBox.Show("With " + pageW.ToString() + " by " + pageH.ToString() + " paper, this document will require " + pageCountX.ToString() + " by " + pageCountY.ToString() + " pages, or " + ((pageCountX * pageCountY).ToString()) + " sheets.", Application.ProductName, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{


				int tempForEndVar = pageCountY;
				for (int pY = 1; pY <= tempForEndVar; pY++)
				{
					int tempForEndVar2 = pageCountX;
					for (int pX = 1; pX <= tempForEndVar2; pX++)
					{

						// Calculate the bounds of this page.
						ppX = (pX - 1) * pageW;
						ppY = (pY - 1) * pageH;
						ppW = ppX + pageW;
						ppH = ppY + pageH;

						pCount++;

						// Clear the array
						thePage = new SVGParse.typLine[1];

						// Loop through each polygon and copy it onto our page.
						int tempForEndVar3 = SVGParse.pData.GetUpperBound(0);
						for (int i = 1; i <= tempForEndVar3; i++)
						{

							n = 0;
							lastPointInside = false;
							lastLineLeft = "";

							// Loop through each line segment of the polygon. If the line segment fits onto the page, copy it into our new polygon.
							int tempForEndVar4 = SVGParse.pData[i].Points.GetUpperBound(0);
							for (int j = 1; j <= tempForEndVar4; j++)
							{




								if (SVGParse.pData[i].Points[j].x >= ppX && SVGParse.pData[i].Points[j].x <= ppW && SVGParse.pData[i].Points[j].y >= ppY && SVGParse.pData[i].Points[j].y <= ppH)
								{

									if (n == 0)
									{
										// Create a new poly
										n = thePage.GetUpperBound(0) + 1;
										thePage = ArraysHelper.RedimPreserve(thePage, new int[]{n + 1});
										thePage[n].Points = new SVGParse.pointD[1];
									}

									// Was the last point NOT inside?
									if (!lastPointInside && j > 1)
									{ // Also, this can't be the first point.

										// Test 1: Top segment
										testPoint.x = ppX;
										testPoint.y = ppY;
										testPoint2.x = ppW;
										testPoint2.y = ppY;
										intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
										thisLineLeft = "T";
										if (intersect.x == -6666)
										{

											// Right side
											testPoint.x = ppW;
											testPoint.y = ppY;
											testPoint2.x = ppW;
											testPoint2.y = ppH;
											intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
											thisLineLeft = "R";
											if (intersect.x == -6666)
											{
												// Bottom
												testPoint.x = ppX;
												testPoint.y = ppH;
												testPoint2.x = ppW;
												testPoint2.y = ppH;
												intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
												thisLineLeft = "B";
												if (intersect.x == -6666)
												{
													// Left
													testPoint.x = ppX;
													testPoint.y = ppY;
													testPoint2.x = ppX;
													testPoint2.y = ppH;
													intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
													thisLineLeft = "L";
												}
											}
										}

										if (intersect.x != -6666)
										{ // Did intersect.


											if ((lastLineLeft == "T" && thisLineLeft == "L") || (lastLineLeft == "L" && thisLineLeft == "T"))
											{
												addPoint3(thePage[n], ppX, ppY);
											}
											if ((lastLineLeft == "L" && thisLineLeft == "B") || (lastLineLeft == "B" && thisLineLeft == "L"))
											{
												addPoint3(thePage[n], ppX, ppH);
											}
											if ((lastLineLeft == "B" && thisLineLeft == "R") || (lastLineLeft == "R" && thisLineLeft == "B"))
											{
												addPoint3(thePage[n], ppW, ppH);
											}
											if ((lastLineLeft == "T" && thisLineLeft == "R") || (lastLineLeft == "R" && thisLineLeft == "T"))
											{
												addPoint3(thePage[n], ppW, ppY);
											}

											addPoint2(thePage[n], intersect);
										}

									}


									addPoint2(thePage[n], SVGParse.pData[i].Points[j]);

									lastPointInside = true;
								}
								else
								{

									// Was the point previous to this one inside?
									if (lastPointInside)
									{
										// Figure out where the line between this point and the last point intersects the borders.

										// Which segment did it intersect?

										// Test 1: Top segment
										testPoint.x = ppX;
										testPoint.y = ppY;
										testPoint2.x = ppW;
										testPoint2.y = ppY;
										intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
										lastLineLeft = "T";
										if (intersect.x == -6666)
										{

											// Right side
											testPoint.x = ppW;
											testPoint.y = ppY;
											testPoint2.x = ppW;
											testPoint2.y = ppH;
											intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
											lastLineLeft = "R";
											if (intersect.x == -6666)
											{
												// Bottom
												testPoint.x = ppX;
												testPoint.y = ppH;
												testPoint2.x = ppW;
												testPoint2.y = ppH;
												intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
												lastLineLeft = "B";
												if (intersect.x == -6666)
												{
													// Left
													testPoint.x = ppX;
													testPoint.y = ppY;
													testPoint2.x = ppX;
													testPoint2.y = ppH;
													intersect = Polygons.lineIntersectLine(ref SVGParse.pData[i].Points[j - 1], SVGParse.pData[i].Points[j], testPoint, testPoint2);
													lastLineLeft = "L";
												}
											}
										}

										if (intersect.x != -6666)
										{ // Did intersect.
											addPoint2(thePage[n], intersect);
										}

									}
									lastPointInside = false;

								}

							}
						}

						// Backup the shapes
						pBackup = (SVGParse.typLine[]) ArraysHelper.DeepCopy(SVGParse.pData);


						// save out just this page
						SVGParse.pData = thePage;

						// Subtract the X and Y
						int tempForEndVar5 = SVGParse.pData.GetUpperBound(0);
						for (int i = 1; i <= tempForEndVar5; i++)
						{
							int tempForEndVar6 = SVGParse.pData[i].Points.GetUpperBound(0);
							for (int j = 1; j <= tempForEndVar6; j++)
							{
								SVGParse.pData[i].Points[j].x -= ppX;
								SVGParse.pData[i].Points[j].y -= ppY;
							}
						}

						zoomToFit();

						//MsgBox "This is page " & pCount

						//exportGCODE "e:\temp\sign\page" & Format(pCount, "00") & ".ngc", Val(cFeedRate.Text), TB1.ButtonChecked("zplunge")

						Application.DoEvents();
						UpgradeSolution1Support.PInvoke.SafeNative.kernel32.Sleep(100);





						// Restore the data
						SVGParse.pData = pBackup;
						//If pCount = 3 Then Exit Function

					}
				}
			}


			return null;
		}

		private object addPoint2(SVGParse.typLine theLine, SVGParse.pointD thePoint)
		{

			int n2 = theLine.Points.GetUpperBound(0) + 1;
			theLine.Points = ArraysHelper.RedimPreserve(theLine.Points, new int[]{n2 + 1});

			// Copy this point
			theLine.Points[n2] = thePoint;


			return null;
		}

		private object addPoint3(SVGParse.typLine theLine, double pX, double pY)
		{

			int n2 = theLine.Points.GetUpperBound(0) + 1;
			theLine.Points = ArraysHelper.RedimPreserve(theLine.Points, new int[]{n2 + 1});

			// Copy this point
			theLine.Points[n2].x = pX;
			theLine.Points[n2].y = pY;


			return null;
		}
		//
		//Function doBox()
		//
		//    Dim bWid As Double, bHig As Double, bDep As Double
		//    Dim matThick As Double
		//    Dim i As Long
		//    Dim nubSize As Double
		//    Dim shortestSide As Double
		//    Dim xOff As Double, yOff As Double
		//    Dim X As Double, Y As Double, n As Double
		//    Dim nubCount As Double
		//    Dim realNubSize As Double
		//
		//    ReDim pData(0)
		//
		//    ' Generate the pieces to make a box
		//    ' 0.125
		//
		//    bWid = Val(InputBox("Box Width?", "Box", 4))
		//    bHig = Val(InputBox("Box Height?", "Box", 4))
		//    bDep = Val(InputBox("Box Depth?", "Box", 4))
		//    matThick = Val(InputBox("Material Thickness?", "Box", 0.125))
		//
		//    ' What will be the size of the "nubbles"?
		//    shortestSide = Min(bWid, Min(bHig, bDep))
		//
		//    ' Make it 5 nubbles for the shortest side.
		//    nubSize = shortestSide / 10
		//
		//    ' Top side
		//    newLine
		//    xOff = 1
		//    yOff = 1
		//
		//    ' How many nubbles will fit on this side?
		//    nubCount = Int(CDbl(bHig / nubSize)) ' round down
		//    If nubCount Mod 2 = 0 Then nubCount = nubCount + 1
		//
		//
		//    ' Expand the nubbles to a whole number.
		//    realNubSize = (bHig / nubCount)
		//
		//    addPoint xOff, yOff
		//
		//    ' Side 1 (Height)
		//    doNubs matThick, nubCount, realNubSize, xOff, yOff, 0
		//
		//    ' Bottom (Width)
		//
		//
		//
		//
		//
		//
		//
		//    ' Generate four side pieces.
		//    For i = 1 To 4
		//
		//
		//
		//
		//
		//    Next
		//
		//
		//
		//    zoomToFit
		//
		//    updateList
		//
		//
		//End Function
		//
		//Function doNubs(matThick As Double, nubCount As Double, realNubSize As Double, _
		//'                xOff As Double, yOff As Double, _
		//'                direction As Long)
		//
		//    Dim i As Long
		//    For n = 1 To nubCount
		//        If n Mod 2 = 0 Then ' Down point
		//            addPoint xOff + (n * realNubSize), yOff + matThick
		//            addPoint xOff + (n * realNubSize), yOff
		//        Else ' Up point
		//            addPoint xOff + (n * realNubSize), yOff
		//            addPoint xOff + (n * realNubSize), yOff + matThick
		//        End If
		//    Next
		//
		//
		//
		//End Function


		public object goTile(int nRows, int nCols, double wOff, double hOff, double rowDiff, double colDiff)
		{

			// Tile the shape

			double maxX = 0;
			double maxY = 0;
			int count = 0;

			int upTo = SVGParse.pData.GetUpperBound(0);

			SVGParse.getExtents(ref maxX, ref maxY);



			int tempForEndVar = nRows;
			for (int y = 1; y <= tempForEndVar; y++)
			{
				int tempForEndVar2 = nCols;
				for (int x = 1; x <= tempForEndVar2; x++)
				{
					count++;

					if (count > 1)
					{ // skpi the first one
						// Copy the shapes.
						duplicateShapes(upTo, ((maxX + wOff) * (x - 1)) + (((y - 1) % 2) * colDiff), ((maxY + hOff) * (y - 1)) + (((x - 1) % 2) * rowDiff));
					}
				}
			}

			SVGParse.optimizePolys();
			zoomToFit();

			updateList();
			return null;
		}

		public object duplicateShapes(int endAt, double Xadd, double Yadd)
		{


			int n = 0;
			int tempForEndVar = endAt;
			for (int i = 1; i <= tempForEndVar; i++)
			{

				n = SVGParse.pData.GetUpperBound(0) + 1;
				SVGParse.pData = ArraysHelper.RedimPreserve(SVGParse.pData, new int[]{n + 1});

				SVGParse.pData[n] = SVGParse.pData[i];

				int tempForEndVar2 = SVGParse.pData[n].Points.GetUpperBound(0);
				for (int j = 1; j <= tempForEndVar2; j++)
				{
					SVGParse.pData[n].Points[j].x += Xadd;
					SVGParse.pData[n].Points[j].y += Yadd;
				}
			}

			return null;
		}
		[STAThread]
		static void Main()
		{
			Application.Run(CreateInstance());
		}
	}
}