using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Threading;

namespace QFSW.BA
{
    /// <summary>Data store used by BuildAutomator for save data.</summary>
    public static class DataStore
    {
        /// <summary>Char used for seperating lines in the save file.</summary>
        private const char LineSeperator = '§';

        /// <summary>Internal dictionary used for key value pairs.</summary>
        private static Dictionary<string, string> DataDictionary = new Dictionary<string, string>();

        /// <summary>Path to the data file used by the data store.</summary>
        private const string DataPath = "DataStore/BA.dat";

        /// <summary>Sets a value in the DataStore.</summary>
        /// <param name="Key">Key of the item.</param>
        /// <param name="Val">Value to set.</param>
        public static void Set<T1, T2>(T1 Key, T2 Val)
        {
            string KeyStr = Key.ToString();
            if (DataDictionary.ContainsKey(KeyStr)) { DataDictionary[KeyStr] = Val.ToString(); }
            else { DataDictionary.Add(KeyStr, Val.ToString()); }
        }

        /// <summary>Gets a value from the DataStore.</summary>
        /// <param name="Key">Key of the item.</param>
        /// <param name="DefaultVal">Default value to use if value cannot be retrieved.</param>
        /// <returns>The retrieved value.</returns>
        public static T2 Get<T1, T2>(T1 Key, T2 DefaultVal = default(T2))
        {
            try
            {
                if (DataDictionary.ContainsKey(Key.ToString()))
                {
                    try { return (T2)Convert.ChangeType(DataDictionary[Key.ToString()], typeof(T2)); }
                    catch { return DefaultVal; }
                }
                else { return DefaultVal; }
            }
            catch { return DefaultVal; }
        }

        /// <summary>Cleanses the internal dictionary.</summary>
        private static void CleanseDictionary()
        {
            KeyValuePair<string, string>[] CleansedDict = DataDictionary.DistinctBy((KeyValuePair<string, string> x) => x.Key.Trim()).ToArray();
            DataDictionary = new Dictionary<string, string>();
            for (int i = 0; i < CleansedDict.Length; i++) { DataDictionary.Add(CleansedDict[i].Key.Trim(), CleansedDict[i].Value.Trim()); }
        }

        /// <summary>Saves the dictionary to the disk.</summary>
        public static void SaveToDisk()
        {
            //Creates data file
            string[] Paths = Directory.GetFiles(Application.dataPath, "DataStore.cs", SearchOption.AllDirectories);
            string Path = Paths.Where((string x) => x.Contains("Build Automator") || x.Contains("QFSW")).First().Replace("DataStore.cs", "").Replace("\\", "/") + DataPath;
            string DirectoryPath = System.IO.Path.GetDirectoryName(Path);
            if (!Directory.Exists(DirectoryPath)) { Directory.CreateDirectory(DirectoryPath); }
            StreamWriter DataFile = new StreamWriter(Path, false);

            //Writes values
            CleanseDictionary();
            foreach (KeyValuePair<string, string> Item in DataDictionary)
            {
                if (string.IsNullOrEmpty(Item.Value)) { DataFile.Write((Item.Key + ",  " + LineSeperator).Trim()); }
                else { DataFile.Write((Item.Key + "," + new string(Item.Value.Where((char x) => x != LineSeperator).ToArray()) + LineSeperator).Trim()); }
                DataFile.Write("\n");
            }

            //Closes file
            DataFile.Flush();
            DataFile.Close();
            DataFile.Dispose();
        }

        /// <summary>Loads the dictionary from the disk.</summary>
        public static bool LoadFromDisk()
        {
            try
            {
                //Opens data file
                string[] Paths = Directory.GetFiles(Application.dataPath, "DataStore.cs", SearchOption.AllDirectories);
                string Path = Paths.Where((string x) => x.Contains("Build Automator") || x.Contains("QFSW")).First().Replace("DataStore.cs", "").Replace("\\", "/") + DataPath;
                string Data = File.ReadAllText(Path);

                //Deserializes data
                string[] Items = Data.Split(LineSeperator);

                for (int i = 0; i < Items.Length; i++)
                {
                    string[] Parts = Items[i].Split(new char[1] { ',' }, 2);
                    try { Set(Parts[0], Parts[1]); }
                    catch { }
                }

                CleanseDictionary();
                return true;
            }
            catch { return false; }
        }

        /// <summary>Returns a copy of the IEnumerable with all duplicates removed.</summary>
        /// <param name="IDSelector">The lambda expression to define the unique ID of the element.</param>
        /// <returns>The filtered IEnumerable.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TID>(this IEnumerable<TSource> Arr, System.Func<TSource, TID> IDSelector)
        {
            HashSet<TID> KnownIDs = new HashSet<TID>();
            return Arr.Where(x => KnownIDs.Add(IDSelector(x)));
        }
    }
}
