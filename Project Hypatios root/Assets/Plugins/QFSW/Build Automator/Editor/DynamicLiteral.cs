using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace QFSW.BA
{
    public class DynamicLiteral
    {
        public readonly string identifier;

        private readonly Func<string> LiteralGenerator;

        public DynamicLiteral(string identifier, Func<string> LiteralGenerator)
        {
            this.identifier = identifier;
            this.LiteralGenerator = LiteralGenerator;
        }

        public virtual string GenerateLiteral() { return LiteralGenerator(); }
        public virtual string GetSaveData() { return "-"; }
        public virtual void SetSaveData(string SaveData) { }
        public virtual void ShowCustomSettings() { }
    }

    public abstract class DateTimeLiteral : DynamicLiteral
    {
        private static readonly HashSet<char> bannedChars = new HashSet<char>() {'/', '\\', '>', '<', ':', '"', '|', '?', '*', ','};
        private static GUIStyle charStyle
        {
            get
            {
                if (_charStyle == null)
                {
                    _charStyle = new GUIStyle(EditorStyles.textField);
                    _charStyle.alignment = TextAnchor.MiddleCenter;
                }

                return _charStyle;
            }
        }
        private static GUIStyle _charStyle;

        protected char seperator = '-';

        protected string Join(params string[] Parts)
        {
            if (seperator == default(char)) { return string.Join(string.Empty, Parts); }
            else { return string.Join(seperator.ToString(), Parts); }
        }

        protected void ShowSeperatorGUI()
        {
            string text = EditorGUILayout.TextField(seperator.ToString(), charStyle, GUILayout.Width(20));
            if (string.IsNullOrEmpty(text)) { seperator = default(char); }
            else { seperator = text[0]; }
            if (bannedChars.Contains(seperator)) { seperator = default(char); }

        }

        protected string GetSaveData<T>(T format) where T : struct, IConvertible, IFormattable, IComparable
        {
            return ((int)(object)format).ToString() + seperator;
        }


        protected void SetSaveData<T>(ref T format, string saveData) where T : struct, IConvertible, IFormattable, IComparable
        {
            if (!string.IsNullOrEmpty(saveData))
            {
                try
                {
                    format = (T)(object)int.Parse(saveData.Substring(0, saveData.Length - 1));
                    seperator = saveData[saveData.Length - 1];
                }
                catch { }
            }
        }


        public DateTimeLiteral(string identifier, Func<string> LiteralGenerator) : base(identifier, LiteralGenerator) { }
    }

    public class DateLiteral : DateTimeLiteral
    {
        public enum DateFormat
        {
            DDMMYYYY = 0,
            MMDDYYYY = 1,
            YYYYMMDD = 2,
            YYYYDDMM = 3,
            DDYYYYMM = 4,
            MMYYYYDD = 5,
            MMYYYY = 6,
            YYYYMM = 7,
            DDMM = 8,
            MMDD = 9,
            DDYYYY = 10,
            YYYYDD = 11,
            DD = 12,
            MM = 13,
            YYYY = 14
        }

        private DateFormat format;

        public DateLiteral(string identifier, Func<string> LiteralGenerator) : base(identifier, LiteralGenerator) { }

        public override string GenerateLiteral()
        {
            DateTime date = DateTime.Now;
            string day = date.Day.ToString("D2");
            string month = date.Month.ToString("D2");
            string year = date.Year.ToString("D4");

            switch (format)
            {
                case DateFormat.DD: return Join(day);
                case DateFormat.DDMM: return Join(day, month);
                case DateFormat.DDMMYYYY: return Join(day, month, year);
                case DateFormat.DDYYYY: return Join(day, year);
                case DateFormat.DDYYYYMM: return Join(day, year, month);
                case DateFormat.MM: return Join(month);
                case DateFormat.MMDD: return Join(month, day);
                case DateFormat.MMDDYYYY: return Join(month, day, year);
                case DateFormat.MMYYYY: return Join(month, year);
                case DateFormat.MMYYYYDD: return Join(month, year, day);
                case DateFormat.YYYY: return Join(year);
                case DateFormat.YYYYDD: return Join(year, day);
                case DateFormat.YYYYDDMM: return Join(year, day, month);
                case DateFormat.YYYYMM: return Join(year, month);
                case DateFormat.YYYYMMDD: return Join(year, month, day);
                default: goto case DateFormat.DDMMYYYY;
            }
        }

        public override string GetSaveData() { return GetSaveData(format); }
        public override void SetSaveData(string saveData) { SetSaveData(ref format, saveData); }

        public override void ShowCustomSettings()
        {
            ShowSeperatorGUI();
            format = (DateFormat)EditorGUILayout.EnumPopup(format, GUILayout.Width(80));
        }
    }

    public class TimeLiteral : DateTimeLiteral
    {
        public enum TimeFormat
        {
            H24MM = 0,
            H12MM = 1,
            H24MMSS = 2,
            H12MMSS = 3
        }

        private TimeFormat format;

        public TimeLiteral(string identifier, Func<string> LiteralGenerator) : base(identifier, LiteralGenerator) { }

        public override string GenerateLiteral()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            string H24 = time.Hours.ToString("D2");
            string H12 = (time.Hours % 12).ToString();
            string H12Suffix = time.Hours > 12 ? "PM" : "AM";
            string MM = time.Minutes.ToString("D2");
            string SS = time.Seconds.ToString("D2");

            switch (format)
            {
                case TimeFormat.H12MM: return Join(H12, MM) + H12Suffix;
                case TimeFormat.H12MMSS: return Join(H12, MM, SS) + H12Suffix;
                case TimeFormat.H24MM: return Join(H24, MM);
                case TimeFormat.H24MMSS: return Join(H24, MM, SS);
                default: goto case TimeFormat.H24MM;
            }
        }

        public override string GetSaveData() { return GetSaveData(format); }
        public override void SetSaveData(string saveData) { SetSaveData(ref format, saveData); }

        public override void ShowCustomSettings()
        {
            ShowSeperatorGUI();
            format = (TimeFormat)EditorGUILayout.EnumPopup(format, GUILayout.Width(80));
        }
    }
}
