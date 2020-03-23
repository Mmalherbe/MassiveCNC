using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
// FIX: PowerPacks
// This code, along with the code inside DoExtraBindings(object, object) was removed to eliminate
// the dependency between Helper DLLs and the PowerPacks DLL.  This commented code is used to initialize
// ControlArrayHelpers for classes that are not Controls, but Components.  The Components are added to a Control,
// which is then added to the form.  With this code commented, Components (such as Line and Shape) will not appear
// in the Form if they are part of a Control Array.  If a customer has Control Arrays for Lines or Shapes and is
// upgrading to PowerPack classes, these lines of code will need to be uncommented.
//
// A possible solution to this problem would be to add a delegate that will execute this initialization code, depending
// on which component is being used, without the need of adding the reference to PowerPacks here.
// using Microsoft.VisualBasic.PowerPacks;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// This class provides helper methods to deal with VB6 Control arrays.  The two main features are dinamic
    /// loading/unloading of a component into a control array and getting the index of a control inside a control array.
    /// </summary>
    public class ControlArrayHelper
    {

        /// <summary>
        /// Gets the index of a control when it is part of a control array.
        /// </summary>
        /// <param name="controlObject">The Control from which we should find the index.</param>
        /// <returns>The index found.</returns>
        public static int GetControlIndex(Object controlObject)
        {
            string arrayName;
            return GetControlIndex(controlObject, out arrayName);
        }

        /// <summary>
        /// Dinamically loads a control into a form, the control belongs to a control array in VB6.
        /// </summary>
        /// <param name="controlContainer">The control container where the control array is declared.</param>
        /// <param name="controlName">The name of the control to be loaded.</param>
        /// <param name="index">The index where the control should be loaded.</param>
        public static void LoadControl(Control controlContainer, String controlName, int index)
        {
            //Get the array of controls as declared in .NET
            FieldInfo fieldInfo = controlContainer.GetType().GetField(controlName);
            if ((fieldInfo != null) && fieldInfo.FieldType.IsArray)
            {
                Array controlArray = (Array)fieldInfo.GetValue(controlContainer);
                //A base control to use as template
                Object baseControl = FindBaseObject(controlArray);
                if (baseControl != null)
                {
                    if (index > controlArray.Length - 1)
                    {
                        Array redimAux = Array.CreateInstance(controlArray.GetType().GetElementType(), index + 1);
                        Array.Copy(controlArray, redimAux, Math.Min(controlArray.Length, redimAux.Length));
                        controlArray = redimAux;
                    }

                    if (controlArray.GetValue(index) != null)
                    {
                        throw new Exception("Object already loaded");
                    }

                    controlArray.SetValue(CloneObject(baseControl), index);
                    fieldInfo.SetValue(controlContainer, controlArray);
                }
            }
            else
            {
                throw new Exception("Cannot load this component");
            }
        }

        /// <summary>
        /// Unloads dinamically a control from a form, the control belongs to a control array in VB6.
        /// </summary>
        /// <param name="objectToUnload">The control to unload.</param>
        public static void UnloadControl(Object objectToUnload)
        {
            Form formContainer;
            if (objectToUnload == null)
            {
                throw new Exception("Control array element doesn't exist");
            }
            if ((formContainer = GetOwnerForm(objectToUnload)) == null)
            {
                throw new Exception("The owner form of the control couldn't be found");
            }
            string arrayName;
            int index = GetControlIndex(objectToUnload, out arrayName);
            if (index >= 0)
            {
                UnloadControl(formContainer, arrayName, index);
            }
        }

        /// <summary>
        /// Dinamically unloads a control from a form, the control belongs to a control array in VB6.
        /// </summary>
        /// <param name="controlContainer">The Control container where the control array is declared.</param>
        /// <param name="controlName">The name of the control to be unloaded.</param>
        /// <param name="index">The index where the control should be unloaded.</param>
        public static void UnloadControl(Control controlContainer, String controlName, int index)
        {
            //Get the array of controls as declared in .NET
            FieldInfo fieldInfo = controlContainer.GetType().GetField(controlName);
            if ((fieldInfo != null) && fieldInfo.FieldType.IsArray)
            {
                Array controlArray = (Array)fieldInfo.GetValue(controlContainer);
                Object objectToUnload;
                if (controlArray.Length > index)
                {
                    objectToUnload = controlArray.GetValue(index);
                }
                else
                {
                    throw new Exception("Control array element '" + index + "' doesn't exist");
                }

                if (
                    !objectToUnload.GetType().GetProperty("Name").GetValue(objectToUnload, null).ToString().Equals(
                        "DinamicallyLoadedControl", StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("Can't unload controls created at design time");
                }

                //A base control to use as template
                Object baseObject = FindBaseObject(controlArray);
                if (baseObject != null)
                {
                    CleanEventHandlers(objectToUnload, baseObject);
                    Control o = baseObject as Control;
                    if (o != null)
                    {
                        o.Parent.Controls.Remove((Control)objectToUnload);
                        controlArray.SetValue(null, index);
                        fieldInfo.SetValue(controlContainer, controlArray);
                    }
                    else
                    {
                        ToolStripItem item = baseObject as ToolStripItem;
                        if (item != null)
                        {
                            item.Owner.Items.Remove((ToolStripItem)objectToUnload);
                            controlArray.SetValue(null, index);
                            fieldInfo.SetValue(controlContainer, controlArray);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the index of a control when it is part of a control array.
        /// </summary>
        /// <param name="controlObject">The Control from which we should find the index.</param>
        /// <param name="arrayName">The name of the array if one is found.</param>
        /// <returns>The index found or -1 if no index found for this control or control is not an array.</returns>
        [DebuggerStepThrough]
        private static int GetControlIndex(Object controlObject, out string arrayName)
        {
            Form formContainer;
            arrayName = String.Empty;

            if (controlObject == null)
            {
                throw new Exception("Object Variable or With block variable not set");
            }
            if ((formContainer = GetOwnerForm(controlObject)) == null)
            {
                throw new Exception("The owner form of the control couldn't be found");
            }

            foreach (FieldInfo fieldInfo in formContainer.GetType().GetFields())
            {
                if (fieldInfo.FieldType.IsArray)
                {
                    Array formArray = (Array)fieldInfo.GetValue(formContainer);
                    if (formArray.Rank > 1)
                    {
                        continue;
                    }
                    for (int i = formArray.GetLowerBound(0); i <= formArray.GetUpperBound(0); i++)
                    {
                        object objectToCompare = formArray.GetValue(i);
                        if (objectToCompare == null)
                        {
                            continue;
                        }
                        if (!objectToCompare.GetType().Equals(controlObject.GetType()))
                        {
                            break;
                        }
                        if (objectToCompare.Equals(controlObject))
                        {
                            arrayName = fieldInfo.Name;
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Cleans the event handlers bound to a control.
        /// </summary>
        /// <param name="controlToUnload">Control to clean up.</param>
        /// <param name="baseObject">Used to get delegates from the control to unload</param>
        private static void CleanEventHandlers(object controlToUnload, object baseObject)
        {
            //Unbind the events set in the control to unload
            foreach (EventInfo eventInfo in controlToUnload.GetType().GetEvents())
            {
                Delegate[] eventDelegates = ContainerHelper.GetEventSubscribers(baseObject, eventInfo.Name);
                //The event in the new control will be bound to the same delegates of the base control
                if (eventDelegates != null)
                {
                    foreach (Delegate del in eventDelegates)
                    {
                        eventInfo.RemoveEventHandler(controlToUnload, del);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first element in the array that is different from null.
        /// </summary>
        /// <param name="controlArray">The control Array.</param>
        /// <returns>Null if no element is found.</returns>
        private static Object FindBaseObject(Array controlArray)
        {
            foreach (Object obj in controlArray)
            {
                if (obj != null && !(obj is ToolStripSeparator))
                {
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a copy of the baseControl.
        /// </summary>
        /// <param name="baseControl">The base control to use as template.</param>
        /// <returns>A copy or null.</returns>
        private static Object CloneObject(Object baseControl)
        {
            Object result = null;
            if (baseControl != null)
            {
                ICloneable control = baseControl as ICloneable;
                if (control != null)
                {
                    result = control.Clone();
                    CallISupportInitializeBeginInit(result);
                }
                else
                {
                    result = Activator.CreateInstance(baseControl.GetType());
                    Dictionary<string, object> listOfProperties = GetPropertiesToCopy(baseControl);

                    CallISupportInitializeBeginInit(result);
                    foreach (PropertyInfo propertyInfo in baseControl.GetType().GetProperties())
                    {
                        if (propertyInfo.Name.Equals("Name", StringComparison.CurrentCultureIgnoreCase))
                        {
                            propertyInfo.SetValue(result, "DinamicallyLoadedControl", null);
                        }
                        else if (listOfProperties.ContainsKey(propertyInfo.Name))
                        {
                            try
                            {
                                propertyInfo.SetValue(result, listOfProperties[propertyInfo.Name], null);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("CloneObject {0}", e.Message);
                            }
                        }
                    }
                }
                AddCloneToContainer(baseControl, result);
                CallISupportInitializeEndInit(result);

                DoExtraBindings(baseControl, result);
                //Bind the same events set in the base control to the new control
                foreach (EventInfo eventInfo in baseControl.GetType().GetEvents())
                {
                    Delegate[] eventDelegates = ContainerHelper.GetEventSubscribers(baseControl, eventInfo.Name);
                    //The event in the new control will be bound to the same delegates of the base control
                    if (eventDelegates != null)
                    {
                        foreach (Delegate del in eventDelegates)
                        {
                            try
                            {
                                eventInfo.AddEventHandler(result, del);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("CloneObject {0}", e.Message);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Performs extra bindings or property set when cloning an object.<para/>
        /// This methods does component type specific settings.
        /// </summary>
        /// <param name="baseControl">The base control used to create a new cloned one.</param>
        /// <param name="result">the clone object.</param>
        private static void DoExtraBindings(object baseControl, object result)
        {
            // FIX: PowerPacks
            // See comments above "using Microsoft.VisualBasic.PowerPacks" for more information.
            //if (baseControl is Shape)
            //{
            //    ShapeContainer shapeContainer = ((Shape) baseControl).Parent;
            //    shapeContainer.Shapes.Add((Shape) result);
            //}
        }

        /// <summary>
        /// Calls ISupportInitialize.BeginInit if the cloned object implements ISupportInitialize
        /// </summary>
        /// <param name="obj"></param>
        private static void CallISupportInitializeBeginInit(object obj)
        {
            if (obj is ISupportInitialize)
            {
                ((ISupportInitialize)obj).BeginInit();
            }
        }

        /// <summary>
        /// Calls ISupportInitialize.EndInit if the cloned object implements ISupportInitialize
        /// </summary>
        /// <param name="obj"></param>
        private static void CallISupportInitializeEndInit(object obj)
        {
            if (obj is ISupportInitialize)
            {
                ((ISupportInitialize)obj).EndInit();
            }
        }

        /// <summary>
        /// Checks if the base object is a Control and then adds the clone to the base object
        /// parent.
        /// </summary>
        /// <param name="baseObject">The object used as base to clone</param>
        /// <param name="clone">The cloned object</param>
        private static void AddCloneToContainer(object baseObject, object clone)
        {
            //Adds the control to the container
            if (baseObject is Control)
            {
                ((Control)baseObject).Parent.Controls.Add((Control)clone);
            }
            else if (baseObject is ToolStripItem)
            {
                ((ToolStripItem)baseObject).Owner.Items.Add((ToolStripItem)clone);
            }
        }

        /// <summary>
        /// Creates a list of the properties to be copied from the base control.
        /// </summary>
        /// <param name="baseControl">The control used as template.</param>
        /// <returns>A list of the propertie's values indexed by its name.</returns>
        private static Dictionary<string, object> GetPropertiesToCopy(object baseControl)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in baseControl.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsSerializable && propertyInfo.PropertyType.IsPublic
                    && propertyInfo.CanRead && propertyInfo.CanWrite
                    && !propertyInfo.Name.Equals("Visible", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!result.ContainsKey(propertyInfo.Name))
                    {
                        result.Add(propertyInfo.Name, propertyInfo.GetValue(baseControl, null));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the form owner of a control. Currently only those inherithing from Control or
        /// Menus are supported, ContextMenus are not supported.
        /// </summary>
        /// <param name="controlObject">The control.</param>
        /// <returns>Null if it couldn't be retrieve.</returns>
        private static Form GetOwnerForm(Object controlObject)
        {
            Form result = null;
            if (controlObject is Control)
            {
                result = ((Control)controlObject).FindForm();
            }
            else if (controlObject is ToolStripItem)
            {
                ToolStripItem menu = (ToolStripItem)controlObject;
                while (menu.OwnerItem != null)
                {
                    menu = menu.OwnerItem;
                }
                result = menu.Owner.FindForm();
            }
            return result;
        }

    }
}
