using System.Windows.Forms;
using System.Drawing;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Helper to support painting in controls/forms.
    /// </summary>
    public static partial class PictureBoxHelper
    {
        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1)
        {
            Image image = picture as Image;
            if (image != null)
            {
                Image img = image;
               // PaintPicture(mpicture, Picture, X1, Y1, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(img.Width), Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(img.Height));
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1)
        {
            Image image = picture as Image;
            if (image != null)
            {
                Image img = image;
                PaintPicture(mpicture, image, x1, y1, width1, SupportHelper.Support.PixelsToTwipsY(img.Height));
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1)
        {
            //Rectangle rec = new Rectangle((int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(X1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Y1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Width1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Height1));
            Rectangle rec = new Rectangle((int)SupportHelper.Support.TwipsToPixelsX(x1), (int)SupportHelper.Support.TwipsToPixelsY(y1), (int)SupportHelper.Support.TwipsToPixelsX(width1), (int)SupportHelper.Support.TwipsToPixelsY(height1));

            Image image = picture as Image;
            if (image != null)
            {
                Image img = image;
                mpicture.CreateGraphics().DrawImage(img, rec);

                //mpicture.CreateGraphics().drawi
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        /// <param name="x2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1, double x2)
        {
            if (picture is Image)
            {
                PaintPicture(mpicture, picture, x1, y1, width1, height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        /// <param name="x2">This argument is discarded.</param>
        /// <param name="y2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1, double x2, double y2)
        {
            if (picture is Image)
            {
                PaintPicture(mpicture, picture, x1, y1, width1, height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        /// <param name="x2">This argument is discarded.</param>
        /// <param name="y2">This argument is discarded.</param>
        /// <param name="width2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1, double x2, double y2, double width2)
        {
            if (picture is Image)
            {
                PaintPicture(mpicture, picture, x1, y1, width1, height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        /// <param name="x2">This argument is discarded.</param>
        /// <param name="y2">This argument is discarded.</param>
        /// <param name="width2">This argument is discarded.</param>
        /// <param name="height2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1, double x2, double y2, double width2, double height2)
        {
            if (picture is Image)
            {
                PaintPicture(mpicture, picture, x1, y1, width1, height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="picture">The image to paint.</param>
        /// <param name="x1">The position in the X axis.</param>
        /// <param name="y1">The position in the Y axis.</param>
        /// <param name="width1">The width used to paint the image.</param>
        /// <param name="height1">The height used to paint the image.</param>
        /// <param name="x2">This argument is discarded.</param>
        /// <param name="y2">This argument is discarded.</param>
        /// <param name="width2">This argument is discarded.</param>
        /// <param name="height2">This argument is discarded.</param>
        /// <param name="opcode">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object picture, double x1, double y1, double width1, double height1, double x2, double y2, double width2, double height2, int opcode)
        {
            if (picture is Image)
            {
                PaintPicture(mpicture, picture, x1, y1, width1, height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }
    }
}
