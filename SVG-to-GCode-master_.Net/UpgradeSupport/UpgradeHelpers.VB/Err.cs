using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;

namespace UpgradeHelpers.VB
{
    /// <summary>
    /// Class to assist the management of errors/exceptions into the instance Microsoft.VisualBasic.Information.Err
    /// </summary>
    public class Err
    {
        /// <summary>
        /// Loads the values stored in the exception into the class Microsoft.VisualBasic.Information.Err
        /// </summary>
        /// <param name="e">The current exception</param>
        public static void LoadError(Exception e)
        {
            Information.Err().Clear();

            if (e != null)
            {
                System.Runtime.InteropServices.COMException comEx = e as System.Runtime.InteropServices.COMException;
                if (comEx != null)
                {
                    Information.Err().Number = (short)comEx.ErrorCode;
                    Information.Err().Description = comEx.Message;
                    Information.Err().Source = comEx.Source;
                    Information.Err().HelpFile = comEx.HelpLink;
                }
                else
                {
                    //For some known types of exceptions there is a number that used to be used in VB6
                    if (e is IndexOutOfRangeException)
                        Information.Err().Number = 9;

                    Information.Err().Description = e.Message;
                    Information.Err().Source = e.Source;
                }
            }
        }
    }
}
