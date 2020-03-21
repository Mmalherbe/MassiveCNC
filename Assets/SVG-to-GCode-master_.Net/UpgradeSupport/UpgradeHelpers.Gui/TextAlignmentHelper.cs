using System;
using System.Drawing;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    public static class TextAlignmentHelper
    {

        static public ContentAlignment ToContentMiddle(HorizontalAlignment horizontalAlignment)
        {
            return (horizontalAlignment == HorizontalAlignment.Left) ? ContentAlignment.MiddleLeft : (horizontalAlignment == HorizontalAlignment.Right) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleCenter;
        }

        static public ContentAlignment ToContentMiddle(ContentAlignment contentAlignment)
        {
            switch (contentAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return ContentAlignment.MiddleLeft;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return ContentAlignment.MiddleCenter;
                default:
                    return ContentAlignment.MiddleRight;
            }
        }

        static public ContentAlignment ToContentTop(HorizontalAlignment horizontalAlignment)
        {
            return (horizontalAlignment == HorizontalAlignment.Left) ? ContentAlignment.TopLeft : (horizontalAlignment == HorizontalAlignment.Right) ? ContentAlignment.TopRight : ContentAlignment.TopCenter;
        }

        static public ContentAlignment ToContentTop(ContentAlignment contentAlignment)
        {
            switch (contentAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return ContentAlignment.TopLeft;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return ContentAlignment.TopCenter;
                default:
                    return ContentAlignment.TopRight;
            }
        }

        static public HorizontalAlignment ToHorizontal(ContentAlignment contentAlignment)
        {
            switch (contentAlignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return HorizontalAlignment.Left;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return HorizontalAlignment.Center;
                default:
                    return HorizontalAlignment.Right;
            }
        }

    }
}
