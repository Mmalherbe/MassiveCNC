using System;
using System.Collections;
using System.Collections.Specialized;

namespace UpgradeHelpers.Helpers
{
    /// <summary>
    /// The CollectionHelper contains a specific functionality to support VB6.Collection using
    /// System.Collections.Specialized.OrderedDictionary .Net native class.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Searchs an element in Dictionary via a key and returns the index of the element.
        /// </summary>
        /// <param name="dict">Dictionary where to search the element.
        /// </param>
        /// <param name="key">Key of the element being searched.</param>
        /// <returns>Returns the index of the found element or -1 if element is not found.</returns>
        public static int GetIndex(this OrderedDictionary dict, object key)
        {
            int index = -1;
            foreach (DictionaryEntry elem in dict)
            {
                index++;
                if (elem.Key == key) return index;
            }
            return -1;
        }

        /// <summary>
        /// Used when the collection access cannot be resolved in static analysis and requires a runtime check to determine the internal type of the key.
        /// If it is an integer the element will be located by position, otherwise by key.
        /// </summary>
        /// <param name="keyOrPosition">The key or position to locate the desired element </param>
        /// <returns>Returns the desired element.</returns>
        public static object GetItem(this OrderedDictionary dict, object keyOrPosition)
        {
            string key = keyOrPosition as string;
            if (key == null)
                return dict[Convert.ToInt32(keyOrPosition) - 1];
            else
                return dict[key];
        }

        /// <summary>
        /// Used when the collection removal cannot be resolved in static analysis and requires a runtime check to determine the internal type of the key.
        /// If it is an integer the element will be located by position, otherwise by key.
        /// </summary>
        /// <param name="keyOrPosition">The key or position to locate the desired element </param>
        public static void RemoveItem(this OrderedDictionary dict, object keyOrPosition)
        {
            string key = keyOrPosition as string;
            if (key == null)
                dict.RemoveAt(Convert.ToInt32(keyOrPosition) - 1);
            else
                dict.Remove(key);
        }
    }
}
