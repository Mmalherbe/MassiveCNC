using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace UpgradeHelpers.Gui
{
    public class ImagesHelper
    {

        public static Image GetImageFromCursor(Cursor cursor)
        {
            int width = cursor.Size.Width;
            int height = cursor.Size.Height;
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                cursor.Draw(gr, new Rectangle(0, 0, width, height));
            }
            return bitmap;
        }

    }
}
