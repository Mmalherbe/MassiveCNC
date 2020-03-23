using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using UpgradeHelpers.Helpers;

namespace UpgradeHelpers.Gui
{
    /// <summary>
    /// Extender Provider for handling CommandButton properties which are not present in .NET buttons.
    /// </summary>
    [ProvideProperty("DownPicture", typeof(Button))]
    [ProvideProperty("Style", typeof(Button))]
    [ProvideProperty("DisabledPicture", typeof(Button))]
    [ProvideProperty("MaskColor", typeof(Button))]
    [ProvideProperty("CorrectEventsBehavior", typeof(Button))]
    public partial class CommandButtonHelper : Component, IExtenderProvider, ISupportInitialize
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public CommandButtonHelper()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with container.
        /// </summary>
        /// <param name="container">The container where the button is included.</param>
        public CommandButtonHelper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Determinates which controls can use these extra properties.
        /// </summary>
        /// <param name="extender">The object to test.</param>
        /// <returns>True if the object can extend the properties.</returns>
        public bool CanExtend(object extender)
        {
            return extender is Button;
        }

        private enum NewPropertiesEnum
        {
            CorrectEventsBehavior = 0
        }

        static CommandButtonHelper()
        {
            //Initializes the list of events that should be patched
            EventsToCorrect.Add("Click", new EventHandler(Button_Click));
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////// STATIC VARIABLES TO MANAGE EXTRA PROPERTIES //////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Variables for the management of the property DownPicture.
        /// </summary>
        private static readonly object ObjLockEvents = new object();
        private static readonly WeakDictionary<Button, Image> DownPictures = new WeakDictionary<Button, Image>();
        private static readonly WeakDictionary<Button, Image> UpPictures = new WeakDictionary<Button, Image>();
        private static readonly WeakDictionary<Button, Image> DisabledPictures = new WeakDictionary<Button, Image>();
        private static readonly WeakDictionary<Button, Color> MaskColor = new WeakDictionary<Button, Color>();
        private static readonly List<Button> OnMouseDown = new List<Button>();
        private static readonly Queue<KeyValuePair<Button, int>> SetStylePendingList = new Queue<KeyValuePair<Button, int>>();
        private static readonly WeakDictionary<Button, Dictionary<NewPropertiesEnum, object>> NewProperties = new WeakDictionary<Button, Dictionary<NewPropertiesEnum, object>>();
        private static readonly WeakDictionary<Button, Dictionary<String, List<Delegate>>> EventsPatched = new WeakDictionary<Button, Dictionary<string, List<Delegate>>>();
        private static readonly Dictionary<string, Delegate> EventsToCorrect = new Dictionary<string, Delegate>();

        /// <summary>
        /// Variable for the management of the property Style.
        /// </summary>
        private static readonly WeakDictionary<Button, int> Styles = new WeakDictionary<Button, int>();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////// STATIC VARIABLES TO MANAGE EXTRA PROPERTIES //////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////// INSTANCE IMPLEMENTATION FOR EXTRA PROPERTIES //////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Instance - Gets the disabled picture bound to this button.
        /// </summary>
        /// <param name="button">The button bound to the disabled picture.</param>
        /// <returns>The image bound for disable picture of this button.</returns>
        [Description("Returns/sets a graphic to be displayed when the button is disabled, if Style is set to 1")]
        public Image GetDisabledPicture(Button button)
        {
            return GetDisabledPictureProperty(button);
        }
        /// <summary>
        /// Instance - Sets the disabled picture for a button.
        /// </summary>
        /// <param name="button">The button to bind the disabled picture.</param>
        /// <param name="image">The iamge to use as disable picture.</param>
        public void SetDisabledPicture(Button button, Image image)
        {
            SetDisabledPictureProperty(button, image);
        }

        /// <summary>
        /// Instance - Gets the down picture bound to this button.
        /// </summary>
        /// <param name="button">The button bound to the down picture.</param>
        /// <returns>The image bound for down picture of this button.</returns>
        [Description("Returns/sets a graphic to be displayed when the button is in the down position, if Style is set to 1")]
        public Image GetDownPicture(Button button)
        {
            return GetDownPictureProperty(button);
        }
        /// <summary>
        /// Instance - Sets the down picture for a button.
        /// </summary>
        /// <param name="button">The button to bind the down picture.</param>
        /// <param name="image">The image to use as down picture.</param>
        public void SetDownPicture(Button button, Image image)
        {
            SetDownPictureProperty(button, image);
        }

        /// <summary>
        /// Instance - Gets the current value of the property Style.
        /// </summary>
        /// <param name="button">The button to get the property.</param>
        /// <returns>The current value.</returns>
        [Description("Returns/sets the appearance of the control, whether standard (standard Windows style) or graphical (with a custom picture)")]
        public int GetStyle(Button button)
        {
            return GetStyleProperty(button);
        }
        /// <summary>
        /// Instance - Sets the value of the property Style.
        /// </summary>
        /// <param name="button">The button to set the property.</param>
        /// <param name="style">The style to set.</param>
        public void SetStyle(Button button, int style)
        {
            SetStyleProperty(DesignMode, button, style);
        }

        /// <summary>
        /// Instance - Gets the property MaskColor for the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The current MaskColor for this button.</returns>
        [Description("Returns or sets a color in a button's picture to be a 'mask' (that is, transparent), if Style is set to 1")]
        public Color GetMaskColor(Button button)
        {
            return GetMaskColorProperty(button);
        }
        /// <summary>
        /// Instance - Sets the property MaskColor for the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="maskColor">The new value for MaskColor of this button.</param>
        public void SetMaskColor(Button button, Color maskColor)
        {
            SetMaskColorProperty(button, maskColor);
        }

        /// <summary>
        /// Gets property for CorrectEventsBehavior property.
        /// </summary>
        /// <param name="button">The button for which to get the CorrectEventsBehavior property.</param>
        /// <returns>A flag to indicate that some events of the button should be patched so they behave more like VB6.</returns>
        [Description("Returns/sets a flag to indicate that some events of the button should be patched so they behave more like VB6")]
        public bool GetCorrectEventsBehavior(Button button)
        {
            if (CheckForProperty(button, NewPropertiesEnum.CorrectEventsBehavior))
                return Convert.ToBoolean(NewProperties[button][NewPropertiesEnum.CorrectEventsBehavior]);
            return false;
        }

        /// <summary>
        /// Sets property for CorrectEventsBehavior property.
        /// </summary>
        /// <param name="btn">The button for which to set the CorrectEventsBehavior property.</param>
        /// <param name="value">The value that will be set.</param>
        public void SetCorrectEventsBehavior(Button btn, bool value)
        {
            if (CheckForProperty(btn, NewPropertiesEnum.CorrectEventsBehavior))
                NewProperties[btn][NewPropertiesEnum.CorrectEventsBehavior] = value;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////// INSTANCE IMPLEMENTATION FOR EXTRA PROPERTIES //////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////// STATIC IMPLEMENTATION FOR EXTRA PROPERTIES ///////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Check if the property 'newPropertiesEnum' is already defined for this button.
        /// </summary>
        /// <param name="btn">The button.</param>
        /// <param name="prop">newPropertiesEnum</param>
        /// <returns>true if successful, false otherwise.</returns>
        private static bool CheckForProperty(Button btn, NewPropertiesEnum prop)
        {
            if (btn == null)
                return false;

            CheckNewProperties(btn);
            if (!NewProperties[btn].ContainsKey(prop))
                NewProperties[btn][prop] = GetDefaultValueForProperty(prop);

            return true;
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
                case NewPropertiesEnum.CorrectEventsBehavior:
                    return true;
            }

            return null;
        }

        /// <summary>
        /// Checks if the btn is controlled by the newProperties Dictionary.
        /// </summary>
        /// <param name="btn">The button</param>
        private static void CheckNewProperties(Button btn)
        {
            if (!NewProperties.ContainsKey(btn))
                NewProperties[btn] = new Dictionary<NewPropertiesEnum, object>();
        }



        /// <summary>
        /// Static - Gets the disabled picture bound to this button.
        /// </summary>
        /// <param name="button">The button bound to the disabled picture.</param>
        /// <returns>The image bound for disable picture of this button.</returns>
        public static Image GetDisabledPictureProperty(Button button)
        {
            if (!DisabledPictures.ContainsKey(button))
                return null;
            return DisabledPictures[button];
        }

        /// <summary>
        /// Static - Sets the disabled picture for a button.
        /// </summary>
        /// <param name="button">The button to bind the disabled picture.</param>
        /// <param name="image">The image to use as disable picture.</param>
        public static void SetDisabledPictureProperty(Button button, Image image)
        {
            button.EnabledChanged -= Button_EnabledChanged;

            //button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            DisabledPictures[button] = image;

            if (image != null)
                button.EnabledChanged += Button_EnabledChanged;
        }

        /// <summary>
        /// Static - Gets the down picture bound to this button.
        /// </summary>
        /// <param name="button">The button bound to the down picture.</param>
        /// <returns>The image bound for down picture of this button.</returns>
        public static Image GetDownPictureProperty(Button button)
        {
            if (!DownPictures.ContainsKey(button))
                return null;
            return DownPictures[button];
        }

        /// <summary>
        /// Static - Sets the down picture for a button.
        /// </summary>
        /// <param name="button">The button to bind the down picture.</param>
        /// <param name="image">The image to use as down picture.</param>
        public static void SetDownPictureProperty(Button button, Image image)
        {
            button.MouseDown -= Button_MouseDown;
            button.MouseUp -= Button_MouseUp;

            //FSQSABORIO 20080730. Changed because it changes the user settings
            //button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            DownPictures[button] = image;

            if (image != null)
            {
                button.MouseDown += Button_MouseDown;
                button.MouseUp += Button_MouseUp;
            }
        }

        /// <summary>
        /// Static - Gets the current value of the property Style.
        /// </summary>
        /// <param name="button">The button to get the property.</param>
        /// <returns>The current value.</returns>
        public static int GetStyleProperty(Button button)
        {
            if (!Styles.ContainsKey(button))
                return 0;
            return Styles[button];
        }

        /// <summary>
        /// Static - Sets the value of the property Style.
        /// </summary>
        /// <param name="button">The button to set the property.</param>
        /// <param name="style">The style to set.</param>
        public static void SetStyleProperty(Button button, int style)
        {
            SetStyleProperty(false, button, style);
        }
        /// <summary>
        /// Static - Sets the value of the property Style.
        /// </summary>
        /// <param name="designMode">To indicate if the operation is done in design mode.</param>
        /// <param name="button">The button to set the property.</param>
        /// <param name="style">The style to set.</param>
        public static void SetStyleProperty(bool designMode, Button button, int style)
        {
            if (OnMouseDown.Contains(button))
            {
                SetStylePendingList.Enqueue(new KeyValuePair<Button, int>(button, style));
                return;
            }

            Styles[button] = (style == 0) ? 0 : 1;

            if (designMode)
                return;

            button.Paint -= Button_Paint;

            if ((Styles[button] == 0) && (button.Image != null))
            {
                UpPictures[button] = button.Image;
                button.Image = null;
            }
            else if ((Styles[button] == 1) && UpPictures.ContainsKey(button))
                button.Image = UpPictures[button];

            button.Paint += Button_Paint;
        }

        /// <summary>
        /// Static - Gets the property MaskColor for the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>The current MaskColor for this button.</returns>
        public static Color GetMaskColorProperty(Button button)
        {
            if (!MaskColor.ContainsKey(button))
                MaskColor.Add(button, Color.Silver);

            return MaskColor[button];
        }
        /// <summary>
        /// Static - Sets the property MaskColor for the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="maskColor">The new value for MaskColor of this button.</param>
        public static void SetMaskColorProperty(Button button, Color maskColor)
        {
            MaskColor[button] = maskColor;

            if (button.Image != null)
            {
                Bitmap bmp = new Bitmap(button.Image);
                bmp.MakeTransparent(maskColor);
                button.Image = bmp;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////// STATIC IMPLEMENTATION FOR EXTRA PROPERTIES ///////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Paint event management so when the style is set to 0 and the button has a graphic, 
        /// this is not displayed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Button_Paint(object sender, PaintEventArgs e)
        {
          Button button = (Button)sender;
        if (Styles.ContainsKey(button) && (Styles[button] == 0) && (button.Image != null))
        {
          if (!UpPictures.ContainsKey(button))
           UpPictures[button] = button.Image;
            button.Image = null;
            }
       }

        /// <summary>
        /// Event handler to change the current button image when the button is enabled or disabled.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Enabled)
            {
                if (!(Styles.ContainsKey(button) && Styles[button] == 0))
                {
                    button.Image = UpPictures[button];
                }
            }
            else
            {
                if (Styles.ContainsKey(button) && Styles[button] == 0)
                    return;

                if (OnMouseDown.Contains(button))
                    Button_MouseUp(button, null);

                UpPictures[button] = button.Image;
                button.Image = DisabledPictures[button];
            }

        }

        /// <summary>
        /// Event handler to change the current button image for the down picture.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Button_MouseDown(object sender, MouseEventArgs e)
        {
            
            Button button = (Button)sender;
            OnMouseDown.Add(button);

            if (Styles.ContainsKey(button) && Styles[button] == 0)
                return;

            UpPictures[button] = button.Image;
            button.Image = DownPictures[button];
        }

        /// <summary>
        /// Event handler to change back to the original button image.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            if (!OnMouseDown.Contains(button))
                return;

            OnMouseDown.Remove(button);

            if (!(Styles.ContainsKey(button) && Styles[button] == 0))
            {
                button.Image = UpPictures[button];
            }

            while (SetStylePendingList.Count > 0)
            {
                KeyValuePair<Button, int> styleParameters = SetStylePendingList.Dequeue();
                SetStyleProperty(styleParameters.Key, styleParameters.Value);
            }
        }


        /// <summary>
        ///Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
        }

        /// <summary>
        ///Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (!DesignMode)
            {
                CleanDeadReferences();
                CorrectEventsBehavior();
            }
        }

        /// <summary>
        /// It will clean the internal dictionaries from old references of buttons already disposed.
        /// </summary>
        private void CleanDeadReferences()
        {
            try
            {
                List<Button> toClean = new List<Button>();
                foreach (Button btn in NewProperties.Keys)
                {
                    if (btn.IsDisposed)
                        toClean.Add(btn);
                }
                foreach (Button btn in toClean)
                {
                    NewProperties.Remove(btn);
                }

                toClean.Clear();
                foreach (Button btn in EventsPatched.Keys)
                {
                    if (btn.IsDisposed)
                        toClean.Add(btn);
                }
                foreach (Button btn in toClean)
                {
                    EventsPatched.Remove(btn);
                }
            }
            catch
            {
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////// FUNCTIONS TO PATCH THE EVENTS ///////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////
        /* This is how this path of events is going to work:
         *  When in design code the property "CorrectEventsBehavior" is set to true for a specific 
         *  Button, the following code will be applied at the end of execution of InitializeComponent,
         *  that means at the end of the design code.
         *  This code will:
         *      - Remove the event handlers for certains event as they were specified in design time
         *      - Add a custom event handler for the specific event being patch (defined below)
         *      - The custome events defined here will decide how and under what circunstances the
         *          original events will be called
         * 
         *  This mean that we will remove the events defined by the user and add our owns and we decide
         *  how and when to call the user defined events.
         * 
         *  Restrictions:
         *      This will path the events defined in design time, if the user specify another events in
         *      runtime code they will not be patched.
         */

        /// <summary>
        /// Deattach some events for the buttons in order to be managed internally.
        /// </summary>
        private static void CorrectEventsBehavior()
        {
            List<Button> btnsToCorrects = new List<Button>();
            lock (ObjLockEvents)
            {
                foreach (Button btn in NewProperties.Keys)
                {
                    if (NewProperties[btn].ContainsKey(NewPropertiesEnum.CorrectEventsBehavior)
                        && Convert.ToBoolean(NewProperties[btn][NewPropertiesEnum.CorrectEventsBehavior]))
                    {
                        btnsToCorrects.Add(btn);
                        CorretEventsForButton(btn);
                    }
                }

                foreach (Button btn in btnsToCorrects)
                {
                    NewProperties[btn].Remove(NewPropertiesEnum.CorrectEventsBehavior);
                }
            }
        }

        /// <summary>
        /// Patches the events for a specific button.
        /// </summary>
        /// <param name="btn">The button to patch.</param>
        private static void CorretEventsForButton(Button btn)
        {
            if (EventsPatched.ContainsKey(btn))
                throw new InvalidOperationException("Events for this button has been previously patched: '" + btn.Name + "'");

            EventsPatched.Add(btn, new Dictionary<string, List<Delegate>>());
            foreach (string eventName in EventsToCorrect.Keys)
            {
                EventInfo eventInfo = btn.GetType().GetEvent(eventName);
                if (eventInfo == null)
                    throw new InvalidOperationException("Event info for event '" + eventName + "' could not be found");

                EventsPatched[btn].Add(eventName, new List<Delegate>());
                Delegate[] eventDelegates = ContainerHelper.GetEventSubscribers(btn, eventName);
                if (eventDelegates != null)
                {

                    foreach (Delegate del in eventDelegates)
                    {
                        EventsPatched[btn][eventName].Add(del);
                        eventInfo.RemoveEventHandler(btn, del);
                    }
                }
                eventInfo.AddEventHandler(btn, EventsToCorrect[eventName]);
            }
        }

        /// <summary>
        /// Event handler for the Click event of a Button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Button_Click(object sender, EventArgs e)
        {
            bool defaultButton = false;
            Button source = (Button)sender;
            if (source.UseMnemonic)
            {
                Form parentForm = source.FindForm();
                if (parentForm != null)
                    defaultButton = source.Equals(parentForm.AcceptButton) || source.Equals(parentForm.CancelButton);

                if (source.Focused || defaultButton || ((Control.ModifierKeys & Keys.Alt) == Keys.Alt))
                {
                    InvokeEvents(source, "Click", new object[] { sender, e });
                }
            }
            else
            {
                InvokeEvents(source, "Click", new object[] { sender, e });
            }
        }

        /// <summary>
        /// Allows to invoke the patched events for a Button.
        /// </summary>
        /// <param name="source">The control for which to call the patched event.</param>
        /// <param name="eventName">The name of the event to call.</param>
        /// <param name="args">The arguments to pass to the event.</param>
        private static void InvokeEvents(Button source, string eventName, object[] args)
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
