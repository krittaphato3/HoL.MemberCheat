using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoLMod.MemberCheat
{
    public static class UIHelpers
    {
        private static GUIStyle _boldLabel;
        private static GUIStyle _sectionHeader;
        private static GUIStyle _headerLabel;

        public static GUIStyle BoldLabel =>
            _boldLabel ??= new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 };

        public static GUIStyle SectionHeader =>
            _sectionHeader ??= new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };

        public static GUIStyle HeaderLabel =>
            _headerLabel ??= new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 12 };

        public static void SearchBar(ref string searchText, Action onClear = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            string newSearch = GUILayout.TextField(searchText, GUILayout.Width(150));
            if (newSearch != searchText) searchText = newSearch;
            if (GUILayout.Button("Clear", GUILayout.Width(50))) { searchText = ""; onClear?.Invoke(); }
            GUILayout.EndHorizontal();
        }

        public static void Label(string text, GUIStyle style = null)
        {
            GUILayout.Label(text, style ?? GUI.skin.label);
        }

        public static void Section(string title)
        {
            GUILayout.Label($"── {title} ──", SectionHeader);
        }

        public static bool IntField(string label, List<string> data, int idx, int width = 60, Action onChange = null)
        {
            if (idx >= data.Count) return false;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            string val = GUILayout.TextField(data[idx], GUILayout.Width(width));
            if (val != data[idx] && int.TryParse(val, out int iVal))
            {
                data[idx] = iVal.ToString();
                onChange?.Invoke();
                GUILayout.EndHorizontal();
                return true;
            }
            GUILayout.EndHorizontal();
            return false;
        }

        public static void IntFieldWithButtons(string label, List<string> data, int idx, int maxVal, Action onChange = null)
        {
            if (idx >= data.Count) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            string val = GUILayout.TextField(data[idx], GUILayout.Width(50));
            if (val != data[idx] && int.TryParse(val, out int iVal)) { data[idx] = iVal.ToString(); onChange?.Invoke(); }
            if (int.TryParse(data[idx], out int cur))
            {
                if (GUILayout.Button("-", GUILayout.Width(25))) { data[idx] = Mathf.Max(0, cur - 1).ToString(); onChange?.Invoke(); }
                if (GUILayout.Button("+", GUILayout.Width(25))) { data[idx] = (cur + 1).ToString(); onChange?.Invoke(); }
                if (GUILayout.Button($"Max", GUILayout.Width(40))) { data[idx] = maxVal.ToString(); onChange?.Invoke(); }
            }
            GUILayout.EndHorizontal();
        }

        public static void FloatFieldWithButtons(string label, List<string> data, int idx, int maxVal, Action onChange = null)
        {
            if (idx >= data.Count) return;
            float.TryParse(data[idx], out float curFloat);
            int cur = Mathf.RoundToInt(curFloat);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(90));
            string newVal = GUILayout.TextField(cur.ToString(), GUILayout.Width(50));
            if (int.TryParse(newVal, out int newInt) && newInt != cur)
            {
                data[idx] = newInt.ToString();
                onChange?.Invoke();
            }
            float.TryParse(data[idx], out curFloat);
            cur = Mathf.RoundToInt(curFloat);
            if (GUILayout.Button("-", GUILayout.Width(25))) { data[idx] = Mathf.Max(0, cur - 1).ToString(); onChange?.Invoke(); }
            if (GUILayout.Button("+", GUILayout.Width(25))) { data[idx] = (cur + 1).ToString(); onChange?.Invoke(); }
            if (GUILayout.Button("Max", GUILayout.Width(40))) { data[idx] = maxVal.ToString(); onChange?.Invoke(); }
            GUILayout.EndHorizontal();
        }

        public static void TextField(string label, List<string> data, int idx, int labelWidth = 100, int fieldWidth = 200, Action onChange = null)
        {
            if (idx >= data.Count) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(labelWidth));
            string val = GUILayout.TextField(data[idx], GUILayout.Width(fieldWidth));
            if (val != data[idx]) { data[idx] = val; onChange?.Invoke(); }
            GUILayout.EndHorizontal();
        }

        public static void DropdownButtons<T>(string label, string currentLabel, Dictionary<T, string> options, Action<T> onSelect, int labelWidth = 60, int buttonWidth = 80)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(labelWidth));
            if (currentLabel != null)
                GUILayout.Label(currentLabel, GUILayout.Width(buttonWidth));
            foreach (var opt in options)
                if (GUILayout.Button(opt.Value, GUILayout.Width(buttonWidth)))
                    onSelect(opt.Key);
            GUILayout.EndHorizontal();
        }

        public static void DropdownButtonsWrapped<T>(string label, string currentLabel, Dictionary<T, string> options, Action<T> onSelect, int cols = 5, int labelWidth = 60)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(labelWidth));
            if (currentLabel != null)
                GUILayout.Label(currentLabel, GUILayout.Width(80));
            GUILayout.EndHorizontal();

            var items = options.ToList();
            for (int i = 0; i < items.Count; i += cols)
            {
                GUILayout.BeginHorizontal();
                if (i == 0) GUILayout.Label("", GUILayout.Width(labelWidth));
                else GUILayout.Label("", GUILayout.Width(labelWidth));
                for (int j = i; j < i + cols && j < items.Count; j++)
                {
                    var opt = items[j];
                    if (GUILayout.Button(opt.Value))
                        onSelect(opt.Key);
                }
                GUILayout.EndHorizontal();
            }
        }

        public static void ActionButtons(params (string label, Action action)[] buttons)
        {
            GUILayout.BeginHorizontal();
            foreach (var btn in buttons)
                if (GUILayout.Button(btn.label))
                    btn.action();
            GUILayout.EndHorizontal();
        }

        public static void DangerButton(string label, Action action)
        {
            GUI.color = Color.red;
            if (GUILayout.Button(label))
                action();
            GUI.color = Color.white;
        }

        public static bool TextFieldChanged(string label, ref string value, int labelWidth = 50, int fieldWidth = 120)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", GUILayout.Width(labelWidth));
            string newVal = GUILayout.TextField(value, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();
            if (newVal != value) { value = newVal; return true; }
            return false;
        }

        public static bool Checkbox(string label, ref bool value, int labelWidth = 200)
        {
            bool newVal = GUILayout.Toggle(value, label, GUILayout.Width(labelWidth));
            if (newVal != value) { value = newVal; return true; }
            return false;
        }

        public static void WriteBackSetData(List<List<string>> list, string fieldName, Action refresh = null)
        {
            SetStaticField(fieldName, list);
            InvokeReadSetData();
            refresh?.Invoke();
        }

        public static void WriteBackFlatData(List<string> list, string fieldName, Action refresh = null)
        {
            SetStaticField(fieldName, list);
            InvokeReadSetData();
            refresh?.Invoke();
        }

        public static void WriteBackIntList(List<int> list, string fieldName, Action refresh = null)
        {
            SetStaticField(fieldName, list);
            InvokeReadSetData();
            refresh?.Invoke();
        }

        private static void SetStaticField(string fieldName, object value)
        {
            var field = typeof(Mainload).GetField(fieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field?.SetValue(null, value);
        }

        public static void InvokeReadSetData()
        {
            typeof(Mainload).GetMethod("ReadSetData",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, null);
        }

        public static List<List<string>> GetStaticListField(string fieldName)
        {
            var field = typeof(Mainload).GetField(fieldName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (field?.GetValue(null) as List<List<string>>) ?? new List<List<string>>();
        }

        public static string GetDisplayValue(string val)
        {
            if (float.TryParse(val, out float fv))
                return Mathf.RoundToInt(fv).ToString();
            return val;
        }
    }
}
