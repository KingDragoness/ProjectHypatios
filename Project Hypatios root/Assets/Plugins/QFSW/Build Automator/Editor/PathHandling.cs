using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace QFSW.BA
{
    /// <summary>Whether to use absolute or relative paths in all of the settings.</summary>
    public enum PathType
    {
        /// <summary>Paths will be relative to root folder.</summary>
        Relative = 0,
        /// <summary>Paths will be absolute.</summary>
        Absolute = 1
    }

    /// <summary>Various extensions to the string class.</summary>
    public static class StringExtenstion
    {
        /// <summary> Shortens a string to the desired number of characters.</summary>
        /// <param name="Text">The string to shorten.</param>
        /// <param name="MaxLength">The maximum number of characters the shortened string may contain.</param>
        /// <returns>The truncated string.</returns>
        public static string Truncate(this string Text, int MaxLength)
        {
            if (string.IsNullOrEmpty(Text) || MaxLength < 0) { return Text; }
            else { return Text.Length <= MaxLength ? Text : Text.Substring(0, MaxLength); }
        }
    }

    /// <summary>Extends the List class.</summary>
    public static class ListExtension
    {
        /// <summary>Removes and returns an element from the list.</summary>
        /// <param name="Index">The index of the desired element.</param>
        /// <returns>The element at the specified index.</returns>
        public static T Pop<T>(this List<T> Arr, int Index)
        {
            if (Index < 0 || Index >= Arr.Count || Arr.Count == 0) { throw new System.ArgumentOutOfRangeException("Index", Index, "The index specified was out of bounds of the list"); }
            T Item = Arr[Index];
            Arr.RemoveAt(Index);
            return Item;
        }

        /// <summary>Removes and returns all of the elements from the list meeting a specific criteria.</summary>
        /// <param name="PopCriteria">The lambda expression for which elements to remove.</param>
        /// <returns>The list of removed elements.</returns>
        public static List<T> PopAll<T>(this List<T> Arr, System.Func<T, bool> PopCriteria)
        {
            List<T> Items = Arr.FindAll(x => PopCriteria(x));
            for (int i = 0; i < Items.Count; i++) { Arr.Remove(Items[i]); }
            return Items;
        }
    }

    /// <summary>Extended functionality for path handling.</summary>
    public static class PathHandling
    {
        /// <summary>Combines two paths before formatting and processing them.</summary>
        /// <param name="Path1">The first part of the path.</param>
        /// <param name="Path2">The second part of the path.</param>
        /// <returns>The processed path.</returns>
        public static string Combine(string Path1, string Path2)
        {
            //Combines paths
            string ProcessedPath = Path.Combine(Path1, Path2);
            ProcessedPath = ProcessedPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            //Attempts to format
            if (!String.IsNullOrEmpty(ProcessedPath))
            {
                try { ProcessedPath = Path.GetFullPath(ProcessedPath); }
                catch { }
            }

            return ProcessedPath;
        }

        /// <summary>Determines the index where the 2 Lists diverge.</summary>
        /// <param name="StringArray1">The first list.</param>
        /// <param name="StringArray2">The second list.</param>
        /// <returns>The index of the first value that is not the same between them, -1 if the two lists are identical.</returns>
        private static int DivergenceIndex(List<string> StringArray1, List<string> StringArray2)
        {
            //Gets size of smaller list
            int SmallerStringSize = Mathf.Min(StringArray1.Count, StringArray2.Count);

            if (SmallerStringSize == 0) { return 0; }
            else
            {
                //Returns first point where lists are no longer equal
                for (int i = 0; i < SmallerStringSize; i++) { if (StringArray1[i] != StringArray2[i]) { return i; } }
                if (StringArray1.Count != StringArray2.Count) { return SmallerStringSize; }
                //Returns -1 if two lists are identical
                else { return -1; }
            }
        }

        /// <summary>Converts an absolute path into a relative path.</summary>
        /// <param name="RootPath">The root path that will be used as the reference path.</param>
        /// <param name="TargetPath">The absolute path that will be converted into a relative path.</param>
        /// <returns>A relative path from RootPath to TargetPath.</returns>
        public static string MakeRelative(string RootPath, string TargetPath)
        {
            string RelativePath = "";

            //Error validation
            if (String.IsNullOrEmpty(RootPath)) { throw new ArgumentException("RootPath", RootPath); }
            if (String.IsNullOrEmpty(TargetPath)) { throw new ArgumentException("TargetPath", TargetPath); }

            //Formats paths
            try { RootPath = Path.GetFullPath(RootPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)); }
            catch { throw new ArgumentException("RootPath", RootPath); }
            try { TargetPath = Path.GetFullPath(TargetPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)); }
            catch { throw new ArgumentException("TargetPath", TargetPath); }

            //Further Validation
            if (Path.GetPathRoot(RootPath) != Path.GetPathRoot(TargetPath)) { throw new ArgumentException("TargetPath", TargetPath); }

            //Converts to lists for each directory and removes whitespace
            List<string> RootPathList = RootPath.Split(Path.DirectorySeparatorChar).ToList();
            RootPathList.RemoveAll(x => string.IsNullOrEmpty(x));
            List<string> TargetPathList = TargetPath.Split(Path.DirectorySeparatorChar).ToList();
            TargetPathList.RemoveAll(x => string.IsNullOrEmpty(x));

            //Further Validation
            if (RootPathList.Count >= 2 && TargetPathList.Count >= 2)
            {
                if (RootPathList[0].ToLower() == "volumes" || TargetPathList[0].ToLower() == "volumes")
                {
                    if (RootPathList[1].ToLower() != TargetPathList[1].ToLower()) { throw new ArgumentException("TargetPath", TargetPath); }
                }
            }

            //Finds the point where the two paths diverge
            int SplitPoint = DivergenceIndex(TargetPathList, RootPathList);
            //Returns empty string if two paths are identical
            if (SplitPoint < 0) { return ""; }
            //Jumps up the correct number of parent directories
            if (SplitPoint < RootPathList.Count) { for (int i = 0; i < RootPathList.Count - SplitPoint; i++) { RelativePath += ".." + Path.DirectorySeparatorChar; } }

            //Reconnects the rest of the target path
            for (int i = SplitPoint; i < TargetPathList.Count; i++)
            {
                if (i > SplitPoint) { RelativePath += Path.DirectorySeparatorChar; }
                RelativePath += TargetPathList[i];
            }

            return RelativePath;
        }
    }
}
