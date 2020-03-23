using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Control created to convert the images of a VB6 ImageList into an internal .NET ImageList.
    /// This control can be used in design time or runtime
    ///     - Design Time: Add the control to the form with the VB6 ImageList, 
    ///       use the property VB6ImageList to indicate the name of the VB6 ImageList and 
    ///       use the property NETImageList to access the .NET ImageList with the images migrated
    ///     - Runtime: Creates an instance of this control and use the method LoadVB6ImageList 
    ///       with a VB6 ImageList as parameter to migrate it in runtime, again, 
    ///       use the property NETImageList to access the .NET ImageList with the images migrated
    /// note:
    ///     Some images may not be converted into the .NET ImageList so in such cases 
    ///     a default error image is used, the workaround is to change the original image 
    ///     in the VB6 ImageList to another format.
    /// </summary>
    public partial class ImageListHelper : UserControl
    {
        internal static class ImageListHelperNativeMethods
        {
            /// <summary>
            /// DestroyIcon external function from user32.dll.
            /// </summary>
            /// <param name="handle">The handle of the icon.</param>
            /// <returns>True if successful, false otherwise.</returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern bool DestroyIcon(IntPtr handle);
        }
        /// <summary>
        /// Constructor for ImageListHelper.
        /// </summary>
        public ImageListHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Indicates if the ImageList has been loaded and initialized.
        /// </summary>
        private bool _internalImageListInitialized;

        /// <summary>
        /// Returns a .NET version of the VB6 ImageList.
        /// </summary>
        [Category("ImageListProperties")]
        public ImageList NETImageList
        {
            get
            {
                if ((!_internalImageListInitialized) && (!string.IsNullOrEmpty(_vb6ImageList)))
                {
                    _internalImageListInitialized = true;
                    LoadVb6ImageListFromControlName(_vb6ImageList);
                }

                return internalImageList;
            }
            set
            {
                internalImageList = value;
            }
        }

        /// <summary>
        /// Stores the VB6 ImageList control name, this property should be used only in design time.
        /// </summary>
        private string _vb6ImageList = string.Empty;

        /// <summary>
        /// Returns the VB6 ImageList control name.
        /// </summary>
        [Category("ImageListProperties")]
        public string VB6ImageList
        {
            get
            {
                return _vb6ImageList;
            }
            set
            {
                _vb6ImageList = value;
            }
        }

        /// <summary>
        /// Tries to find the form containing this instance and loads the image list of VB6.
        /// </summary>
        /// <param name="vb6ImageListName">The name of the VB6 ImageList.</param>
        /// <returns>True if image list was successfully loaded.</returns>
        private bool LoadVb6ImageListFromControlName(string vb6ImageListName)
        {
            Form parentForm = FindForm();

            if (parentForm != null)
            {
                Control ctrl = ContainerHelper.Controls(ParentForm)[vb6ImageListName];
                if (ctrl != null)
                    return LoadVB6ImageList(ctrl);
            }

            return false;
        }

        /// <summary>
        /// Loads the VB6ImageList into the internal instance of the .NET ImageList.
        /// </summary>
        /// <param name="vb6ImageListControl">The original VB6 ImageList.</param>
        /// <returns>True if image list was successfully loaded.</returns>
        public bool LoadVB6ImageList(object vb6ImageListControl)
        {
            try
            {
                _internalImageListInitialized = true;
                if (IsValid(vb6ImageListControl))
                {
                    internalImageList.Images.Clear();
                    LoadBasicProperties(vb6ImageListControl);
                    LoadImages(vb6ImageListControl);
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// Loads the list of images from the original VB6 ImageList to the new .NET ImageList.
        /// </summary>
        /// <param name="vb6ImageList">The original VB6 ImageList.</param>
        private void LoadImages(object vb6ImageList)
        {
            Icon vb6Icon = null;
            string key;
            object images = Helpers.ReflectionHelper.GetMember(vb6ImageList, "ListImages");

            if (images == null)
                throw new InvalidCastException("Couldn't retrieve internal VB6 image list");

            for (int i = 1; i <= Convert.ToInt32(Helpers.ReflectionHelper.GetMember(images, "Count")); i++)
            {
                vb6Icon = null;
                object img = Helpers.ReflectionHelper.Invoke(images, "get_item", new object[] { i });
                if (img == null)
                    throw new InvalidCastException("Couldn't retrieve internal VB6 image item");

                key = Convert.ToString(Helpers.ReflectionHelper.GetMember(img, "Key"));
                Image netImg;
                try
                {
                    netImg = SupportHelper.Support.IPictureToImage(Helpers.ReflectionHelper.GetMember(img, "Picture"));
                }
                catch
                {
                    try
                    {
                        //In the case that the image is an Icon, this will convert it into a Bitmap
                        object iconHandle = Helpers.ReflectionHelper.GetMember(Helpers.ReflectionHelper.Invoke(img, "ExtractIcon", new object[] { }), "Handle");
                        IntPtr iconPtr = new IntPtr(Convert.ToInt32(iconHandle));
                        vb6Icon = Icon.FromHandle(iconPtr);
                        netImg = vb6Icon.ToBitmap();
                    }
                    catch
                    {
                        netImg = new Bitmap(errorImageList.Images[0], internalImageList.ImageSize);
                    }
                    finally
                    {
                        try
                        {
                            if (vb6Icon != null)
                                ImageListHelperNativeMethods.DestroyIcon(vb6Icon.Handle);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (string.IsNullOrEmpty(key))
                    internalImageList.Images.Add(netImg);
                else
                    internalImageList.Images.Add(key, netImg);
            }
        }

        /// <summary>
        /// Loads the basic properties of the VB6 ImageList into the new .NET ImageList, 
        /// like Height and Width.
        /// </summary>
        /// <param name="vb6ImageList">The original VB6 ImageList.</param>
        private void LoadBasicProperties(object vb6ImageList)
        {
            Size imgSize = new Size();

            imgSize.Height = Convert.ToInt32(Helpers.ReflectionHelper.GetMember(vb6ImageList, "ImageHeight"));
            imgSize.Width = Convert.ToInt32(Helpers.ReflectionHelper.GetMember(vb6ImageList, "ImageWidth"));
            internalImageList.ImageSize = imgSize;

            bool useMaskColor = (bool)Helpers.ReflectionHelper.GetMember(vb6ImageList, "UseMaskColor");
            if (useMaskColor)
                internalImageList.TransparentColor = (Color)Helpers.ReflectionHelper.GetMember(vb6ImageList, "MaskColor");

            internalImageList.Tag = Helpers.ReflectionHelper.GetMember(vb6ImageList, "Tag");
        }

        /// <summary>
        /// Validates that the object is a VB6 ImageList.
        /// </summary>
        /// <param name="ctrl">The original VB6 ImageList.</param>
        /// <returns>True if this object represents a VB6 ImageList.</returns>
        public static bool IsValid(object ctrl)
        {
            return ctrl != null && (ctrl is AxHost) && ctrl.GetType().Name.Equals("AxImageList", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Event handled to ensure that the control is only visible in design time.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ImageListHelper_VisibleChanged(object sender, EventArgs e)
        {
            if (!DesignMode && Visible)
                Visible = false;
        }
    }
}
