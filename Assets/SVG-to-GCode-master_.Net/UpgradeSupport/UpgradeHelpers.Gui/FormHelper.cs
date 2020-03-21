using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// The FormHelper provides some functionality for Forms, like printing forms.
    /// </summary>
    public class FormHelper
    {
        /// <summary>
        /// Prints the form layout.
        /// </summary>
        /// <param name="mform">The form to be printed.</param>
        public static void PrintForm(Form mform)
        {
            (new PrintFormClass(mform)).Print();
        }

        /// <summary>
        /// Helper class to prepare a Form to be printed.
        /// </summary>
        private class PrintFormClass : IDisposable
        {
            /// <summary>
            /// The Form to be printed.
            /// </summary>
            private Form _frm = null;
            /// <summary>
            /// The PrintDocument where to print the Form.
            /// </summary>
            private PrintDocument printDocument1 = new PrintDocument();
            /// <summary>
            /// The Dialog of PrintPreview.
            /// </summary>
            private PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
            /// <summary>
            /// The Form represented as a Bitmap.
            /// </summary>
            Bitmap imagen;

            /// <summary>
            /// Constructor of PrintFormClass.
            /// </summary>
            public PrintFormClass()
            {
            }

            /// <summary>
            /// Constructor of PrintFormClass.
            /// </summary>
            /// <param name="frm">The form to be printed.</param>
            public PrintFormClass(Form frm)
            {
                _frm = frm;
            }


            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Cleans up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected void Dispose(bool disposing)
            {
                if (disposing)
                {
                    printDocument1.Dispose();
                    printPreviewDialog1.Dispose();
                    _frm.Dispose();
                    imagen.Dispose();
                }
            }

            ~PrintFormClass()
            {
                Dispose(false);
            }
            /// <summary>
            /// Prints the form.
            /// </summary>
            public void Print()
            {
                Print(_frm);
            }

            /// <summary>
            /// Prints the form.
            /// </summary>
            /// <param name="frm">The form to be printed.</param>
            public void Print(Form frm)
            {

                printDocument1.PrintPage += new PrintPageEventHandler(PrintDocument1_PrintPage);
                CapturarPantalla(frm);
                printDocument1.Print();
            }

            /// <summary>
            /// Prints a preview of the form.
            /// </summary>
            /// <param name="frm">The form to be printed.</param>
            public void PrintPreview(Form frm)
            {
                printDocument1.PrintPage += new PrintPageEventHandler(PrintDocument1_PrintPage);
                CapturarPantalla(frm);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }



            /// <summary>
            /// Draws the Form into a Bitmap.
            /// </summary>
            /// <param name="frm">The form to draw.</param>
            private void CapturarPantalla(Form frm)
            {
                bool oldTopMostState = frm.TopMost;
                bool oldVisibleState = frm.Visible;
                frm.Visible = true;
                frm.TopMost = true;
                Application.DoEvents();

                Rectangle frmR = frm.ClientRectangle;
                Rectangle srcR = frm.RectangleToScreen(frmR);

                Graphics g = frm.CreateGraphics();
                Size s = new Size(srcR.Width, srcR.Height);
                imagen = new Bitmap(srcR.Width, srcR.Height, g);
                Graphics g2 = Graphics.FromImage(imagen);
                g2.CopyFromScreen(srcR.Location.X, srcR.Location.Y, 0, 0, s);

                frm.TopMost = oldTopMostState;
                frm.Visible = oldVisibleState;
            }

            /// <summary>
            /// PrintPage event of a PrintFocument.
            /// </summary>
            /// <param name="sender">The event sender.</param>
            /// <param name="e">The PrintPage event arguments.</param>
            private void PrintDocument1_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
            {
                e.Graphics.DrawImage(imagen, 0, 0);
            }
        }
    }
}
