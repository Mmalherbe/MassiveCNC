using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.Gui
{
    public static class DateTimePickerHelper
    {
        static DateTimePickerHelper()
        {
            FieldInfo max = typeof(DateTimePicker).GetField("MaxDateTime", BindingFlags.Public | BindingFlags.Static);
            max.SetValue(null, DateTime.MaxValue);
        }

        public static void SetValue(this DateTimePicker dt, object value)
        {
            dt.DetachValueChangedEvent();

            if (Convert.IsDBNull(value) || value == null || Convert.ToString(value).Trim().Equals(string.Empty))
            {
                dt.Value = DateTime.Now;
                dt.Checked = false;
            }
            else
            {
                dt.Value = Convert.ToDateTime(value);
                dt.Checked = true;
            }

            dt.AttachValueChangedEvent();
        }

        public static object GetValue(this DateTimePicker dt)
        {
            if (dt.Checked)
            {
                return dt.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        private const string ValueChangedEventName = "onValueChanged";
        private static void DetachValueChangedEvent(this DateTimePicker ctrl)
        {
            var eventHandler = ctrl.GetEventHandler(ValueChangedEventName);
            if (eventHandler != null)
            {
                foreach (EventHandler item in eventHandler.GetInvocationList())
                {
                    ctrl.ValueChanged -= item;
                }
                AddEventHandlerList(ctrl, ValueChangedEventName, eventHandler);
            }
        }

        private static void AttachValueChangedEvent(this DateTimePicker ctrl)
        {
            var eventHandler = GetEventHandlerList(ctrl, ValueChangedEventName);
            if (eventHandler != null)
            {
                foreach (EventHandler item in eventHandler.GetInvocationList())
                {
                    ctrl.ValueChanged += item;
                }
                RemoveEventHandlerList(ctrl, ValueChangedEventName);
            }
        }

        private static readonly Dictionary<Control, Dictionary<string, Delegate>> EventHandlerControlCollection = new Dictionary<Control, Dictionary<string, Delegate>>();

        private static void AddEventHandlerList(Control ctrl, string eventName, Delegate eventHandler)
        {
            if (!EventHandlerControlCollection.ContainsKey(ctrl))
                EventHandlerControlCollection.Add(ctrl, new Dictionary<string, Delegate>());
            EventHandlerControlCollection[ctrl].Add(eventName, eventHandler);
        }

        private static Delegate GetEventHandlerList(Control ctrl, string eventName)
        {
            if (EventHandlerControlCollection.ContainsKey(ctrl))
                if (EventHandlerControlCollection[ctrl].ContainsKey(eventName))
                    return EventHandlerControlCollection[ctrl][eventName];
            return null;
        }

        private static void RemoveEventHandlerList(Control ctrl, string eventName)
        {
            if (EventHandlerControlCollection.ContainsKey(ctrl))
            {
                EventHandlerControlCollection[ctrl].Remove(eventName);
                if (EventHandlerControlCollection[ctrl].Count == 0)
                    EventHandlerControlCollection.Remove(ctrl);
            }
        }

        private static EventHandler GetEventHandler(this Control ctrl, string eventName)
        {
            var fieldInfo = ctrl.GetType().GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (fieldInfo != null)
            {
                var eventHandler = fieldInfo.GetValue(ctrl) as EventHandler;
                return eventHandler;
            }
            return null;
        }

        private static EventHandlerList GetEventHandlerList(this Control ctrl)
        {
            var propertyInfo = ctrl.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var eventHandlerList = propertyInfo.GetValue(ctrl, new object[] { }) as EventHandlerList;
            return eventHandlerList;
        }

        private static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }

        public static void HideForm(this Form extForm)
        {
            if (extForm.Owner != null)
            {
                extForm.Owner.Activate();
            }
            extForm.Hide();
        }
    }
}
