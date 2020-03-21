using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Class provided to add lost functionality to the TreeViews .NET.
    /// </summary>
    public class TreeViewHelper
    {
        /// <summary>
        /// Enum to handle the different properties and custom behaviors supplied by this Helper.
        /// </summary>
        private enum NewPropertiesEnum
        {
            ExpandedImage = 0,
            NormalImage = 1
        }

        /// <summary>
        /// List of events to be corrected for this provider.
        /// </summary>
        private static readonly Dictionary<string, Delegate> EventsToCorrect = new Dictionary<string, Delegate>();
        /// <summary>
        /// List of events to be patched for this provider.
        /// </summary>
        private static readonly WeakDictionary<TreeView, Dictionary<String, List<Delegate>>> EventsPatched = new WeakDictionary<TreeView, Dictionary<string, List<Delegate>>>();
        /// <summary>
        /// List of properties and values that are supplied by this Helper.
        /// </summary>
        private static readonly WeakDictionary<TreeView, Dictionary<NewPropertiesEnum, object>> NewProperties = new WeakDictionary<TreeView, Dictionary<NewPropertiesEnum, object>>();

        private const string AfterCollapseEventName = "AfterCollapse";
        private const string AfterExpandEventName = "AfterExpand";

        /// <summary>
        /// Constructor.
        /// </summary>
        static TreeViewHelper()
        {
            //Initializes the list of events that should be patched
            EventsToCorrect.Add(AfterCollapseEventName, new TreeViewEventHandler(TreeView_AfterCollapse));
            EventsToCorrect.Add(AfterExpandEventName, new TreeViewEventHandler(TreeView_AfterExpand));
        }


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the ExpandedImage property for a TreeNode (Key|Index).
        /// </summary>
        /// <param name="nItem">The TreeNode to get.</param>
        /// <returns>The current value.</returns>
        public static object GetTreeNodeExpandedImageProperty(TreeNode nItem)
        {
            TreeView treeView = nItem.TreeView;

            if (CheckForProperty(treeView, NewPropertiesEnum.ExpandedImage))
            {
                Dictionary<TreeNode, object> treeNodeExpandedImageList = (Dictionary<TreeNode, object>)NewProperties[treeView][NewPropertiesEnum.ExpandedImage];
                if (treeNodeExpandedImageList.ContainsKey(nItem))
                    return treeNodeExpandedImageList[nItem];
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// Sets the ExpandedImage property for a TreeNode (Key|Index).
        /// </summary>
        /// <param name="nItem">The TreeNode to set.</param>
        /// <param name="value">The new value to set.</param>
        public static void SetTreeNodeExpandedImageProperty(TreeNode nItem, object value)
        {
            TreeView treeView = nItem.TreeView;

            if ((value is string) || (value is Int32))
            {
                if (CheckForProperty(treeView, NewPropertiesEnum.ExpandedImage))
                {
                    ValidateImageIndex(treeView, value);

                    Dictionary<TreeNode, object> treeNodeExpandedImageList = (Dictionary<TreeNode, object>)NewProperties[treeView][NewPropertiesEnum.ExpandedImage];

                    if (!treeNodeExpandedImageList.ContainsKey(nItem))
                        treeNodeExpandedImageList.Add(nItem, value);
                    else
                        treeNodeExpandedImageList[nItem] = value;

                    if (!EventsPatched.ContainsKey(treeView))
                        CorrectEventsForTreeView(treeView);

                    CheckExpandedImageForTreeNode(nItem);
                }

                return;
            }

            throw new InvalidCastException("Invalid property value");
        }

        /// <summary>
        /// Creates a drag image using the associated image to a treeNode. 
        /// This image is typically used in drag-and-drop operations
        /// </summary>
        /// <param name="treeNode">The base node.</param>
        /// <returns>An image that can be used for Drag and Drop operations.</returns>
        public static Image CreateDragImage(TreeNode treeNode)
        {
            Image res = Resources.UpgradeHelpers_VB6.DefaultDragImage.ToBitmap();
            if ((treeNode.TreeView != null) && (treeNode.TreeView.ImageList != null))
            {
                if (!string.IsNullOrEmpty(treeNode.ImageKey) && treeNode.TreeView.ImageList.Images.ContainsKey(treeNode.ImageKey))
                    res = treeNode.TreeView.ImageList.Images[treeNode.ImageKey];

                if ((treeNode.ImageIndex >= 0) && (treeNode.ImageIndex < treeNode.TreeView.ImageList.Images.Count))
                    res = treeNode.TreeView.ImageList.Images[treeNode.ImageIndex];
            }

            return res;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// STATIC PROPERTIES DEFINITION ////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this TreeView.
        /// </summary>
        /// <param name="treeView">The TreeView.</param>
        /// <param name="prop">The newPropertiesEnum to search.</param>
        /// <returns>true if successful, false otherwise.</returns>
        private static bool CheckForProperty(TreeView treeView, NewPropertiesEnum prop)
        {
            if (treeView == null)
                return false;

            CheckNewProperties(treeView);
            if (!NewProperties[treeView].ContainsKey(prop))
                NewProperties[treeView][prop] = GetDefaultValueForProperty(prop);

            return true;
        }

        /// <summary>
        /// Checks if a TreeView is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="treeView">The TreeView.</param>
        private static void CheckNewProperties(TreeView treeView)
        {
            if (!NewProperties.ContainsKey(treeView))
            {
                NewProperties[treeView] = new Dictionary<NewPropertiesEnum, object>();
                treeView.Disposed += TreeView_Disposed;
            }
        }

        /// <summary>
        /// Event handler for the Disposed event of a TreeView so it can be cleaned it.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void TreeView_Disposed(object sender, EventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            if (NewProperties.ContainsKey(treeView))
                NewProperties.Remove(treeView);

            if (EventsPatched.ContainsKey(treeView))
                EventsPatched.Remove(treeView);
        }

        /// <summary>
        /// Returns a default value for the specified property.
        /// </summary>
        /// <param name="prop">The property requesting a default value.</param>
        /// <returns>A default value casted as object.</returns>
        private static object GetDefaultValueForProperty(NewPropertiesEnum prop)
        {
            switch (prop)
            {
                case NewPropertiesEnum.ExpandedImage:
                case NewPropertiesEnum.NormalImage:
                    return new Dictionary<TreeNode, object>();
            }

            return null;
        }

        /// <summary>
        /// Patches the events for a specific treeview.
        /// </summary>
        /// <param name="treeView">The TreeView.</param>
        private static void CorrectEventsForTreeView(TreeView treeView)
        {
            if (EventsPatched.ContainsKey(treeView))
                throw new InvalidOperationException("Events for this TreeView has been previously patched: '" + treeView.Name + "'");

            EventsPatched.Add(treeView, new Dictionary<string, List<Delegate>>());
            foreach (string eventName in EventsToCorrect.Keys)
            {
                EventInfo eventInfo = treeView.GetType().GetEvent(eventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + eventName + "' could not be found");

                EventsPatched[treeView].Add(eventName, new List<Delegate>());
                Delegate[] eventDelegates = ContainerHelper.GetEventSubscribers(treeView, eventName);
                if (eventDelegates != null)
                {

                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[treeView][eventName].Add(del);
                        eventInfo.RemoveEventHandler(treeView, del);
                    }
                }
                eventInfo.AddEventHandler(treeView, EventsToCorrect[eventName]);
            }
        }

        /// <summary>
        /// Event handler for the event AfterCollapse.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The TreeView event arguments.</param>
        private static void TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            TreeView source = (TreeView)sender;
            try
            {
                CheckExpandedImageForTreeNode(e.Node);

            }
            catch
            {
            }
            finally
            {
                try
                {
                    InvokeEvents(source, AfterCollapseEventName, new object[] { sender, e });
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Event handler for the event AfterExpand.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The TreeView event arguments.</param>
        private static void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeView source = (TreeView)sender;
            try
            {
                CheckExpandedImageForTreeNode(e.Node);

            }
            catch
            {
            }
            finally
            {
                try
                {
                    InvokeEvents(source, AfterExpandEventName, new object[] { sender, e });
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Checks if the image for a treenode should be changed based on 
        /// if an ExpandedImage has been defined for the TreeNode.
        /// </summary>
        /// <param name="nItem">The TreeNode.</param>
        private static void CheckExpandedImageForTreeNode(TreeNode nItem)
        {
            TreeView treeView = nItem.TreeView;
            object expandedImageID = null;
            object normalImageID = null;

            if (CheckForProperty(treeView, NewPropertiesEnum.ExpandedImage))
            {
                Dictionary<TreeNode, object> treeNodeExpandedImageList = (Dictionary<TreeNode, object>)NewProperties[treeView][NewPropertiesEnum.ExpandedImage];
                if (treeNodeExpandedImageList.ContainsKey(nItem))
                {
                    if (CheckForProperty(treeView, NewPropertiesEnum.NormalImage))
                    {
                        expandedImageID = treeNodeExpandedImageList[nItem];
                        ValidateImageIndex(treeView, expandedImageID);

                        Dictionary<TreeNode, object> treeNodeNormalImageList = (Dictionary<TreeNode, object>)NewProperties[treeView][NewPropertiesEnum.NormalImage];

                        if (nItem.IsExpanded)
                        {
                            if (!treeNodeNormalImageList.ContainsKey(nItem))
                            {
                                if (!string.IsNullOrEmpty(nItem.ImageKey))
                                    treeNodeNormalImageList.Add(nItem, nItem.ImageKey);
                                else
                                    treeNodeNormalImageList.Add(nItem, nItem.ImageIndex);

                            }

                            if (expandedImageID is string)
                            {
                                nItem.ImageIndex = nItem.SelectedImageIndex = -1;
                                nItem.ImageKey = nItem.SelectedImageKey = expandedImageID as string;
                            }
                            else
                            {
                                nItem.ImageKey = nItem.SelectedImageKey = string.Empty;
                                nItem.ImageIndex = nItem.SelectedImageIndex = Convert.ToInt32(expandedImageID);
                            }
                        }
                        else
                        {
                            if (treeNodeNormalImageList.ContainsKey(nItem))
                            {
                                normalImageID = treeNodeNormalImageList[nItem];
                                if (normalImageID is string)
                                {
                                    nItem.ImageIndex = nItem.StateImageIndex = -1;
                                    nItem.ImageKey = nItem.SelectedImageKey = normalImageID as string;
                                }
                                else
                                {
                                    nItem.ImageKey = nItem.SelectedImageKey = string.Empty;
                                    nItem.ImageIndex = nItem.StateImageIndex = Convert.ToInt32(normalImageID);
                                }

                                treeNodeNormalImageList.Remove(nItem);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates if the ImageIndex can be used with the images of the TreeView.
        /// </summary>
        /// <param name="treeView">The TreeView source.</param>
        /// <param name="imageId">The Image ID.</param>
        private static void ValidateImageIndex(TreeView treeView, object imageId)
        {
            if (!(imageId is string) && !(imageId is int))
                throw new InvalidCastException("Invalid type for an image index");

            if (treeView.ImageList == null)
                throw new InvalidOperationException("ImageList must be initialized before it can be used");

            if (imageId is string)
            {
                string imageKey = imageId as string;
                if (!treeView.ImageList.Images.ContainsKey(imageKey))
                    throw new KeyNotFoundException("Element not found");
            }

            if (imageId is int)
            {
                int imageIndex = Convert.ToInt32(imageId);
                if ((imageIndex < 0) || (imageIndex >= treeView.ImageList.Images.Count))
                    throw new IndexOutOfRangeException("Index out of bounds");
            }
        }

        /// <summary>
        /// Allows to invoke the patched events for a TreeView.
        /// </summary>
        /// <param name="source">The TreeView to invoke the event.</param>
        /// <param name="eventName">The event name to be invoked.</param>
        /// <param name="args">The arguments used to invoke the event.</param>
        private static void InvokeEvents(TreeView source, string eventName, object[] args)
        {
            if (EventsPatched.ContainsKey(source) && EventsPatched[source].ContainsKey(eventName))
            {
                foreach (Delegate del in EventsPatched[source][eventName])
                {
                    del.DynamicInvoke(args);
                }
            }
        }
    }
}
