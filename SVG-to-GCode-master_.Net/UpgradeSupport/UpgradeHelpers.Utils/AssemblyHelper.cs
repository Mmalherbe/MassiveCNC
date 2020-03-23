using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// The AssemblyHelper obtains the information of Assemblies.
    /// </summary>
    public class AssemblyHelper
    {
        /// <summary>
        /// Gets the title (based on the AssemblyTitle attribute) of the current executing assembly.
        /// If no AssemblyTitle attribute is found then the Filename (without extension) is used
        /// </summary>
        /// <returns>The assembly title</returns>
        public static string GetTitle()
        {
            return GetTitle(Assembly.GetExecutingAssembly());
        }
        
        /// <summary>
        /// Gets the title of the currently executing assembly.
        /// </summary>
        /// <param name="assbly">The length of the new array.</param>
        /// <returns>The assembly title.</returns>
        public static string GetTitle(Assembly assbly)
        { // Get all Title attributes on this assembly
            object[] attributes = assbly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            // If there is at least one Title attribute
            if (attributes.Length > 0)
            {
                // Select the first one
                
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                // If it is not an empty string, return it
                if (titleAttribute.Title != "")
                    return titleAttribute.Title;
            }
            
#if !PORTABLE
           // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
            return System.IO.Path.GetFileNameWithoutExtension(assbly.CodeBase);
#else
            return assbly.FullName;
#endif
        }
    }
}
