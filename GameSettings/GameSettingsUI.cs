using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoLMod.MemberCheat.GameSettings
{
    public static class GameSettingsUI
    {
        private static Vector2 scroll;

        public static void Draw()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Date / Time", GUI.skin.label);
            var timeList = GetTimeList();
            if (timeList != null && timeList.Count >= 3)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Year:", GUILayout.Width(50));
                string yearStr = GUILayout.TextField(timeList[0].ToString(), GUILayout.Width(80));
                if (yearStr != timeList[0].ToString() && int.TryParse(yearStr, out int year))
                { timeList[0] = year; ApplyTimeList(timeList); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Month:", GUILayout.Width(50));
                string monthStr = GUILayout.TextField(timeList[1].ToString(), GUILayout.Width(80));
                if (monthStr != timeList[1].ToString() && int.TryParse(monthStr, out int month))
                { timeList[1] = Mathf.Clamp(month, 1, 12); ApplyTimeList(timeList); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Day:", GUILayout.Width(50));
                string dayStr = GUILayout.TextField(timeList[2].ToString(), GUILayout.Width(80));
                if (dayStr != timeList[2].ToString() && int.TryParse(dayStr, out int day))
                { timeList[2] = Mathf.Clamp(day, 1, 30); ApplyTimeList(timeList); }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        private static List<int> GetTimeList()
        {
            var field = typeof(Mainload).GetField("Time_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<int>;
        }

        private static void ApplyTimeList(List<int> timeList)
        {
            typeof(Mainload).GetField("Time_now", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, timeList);
            typeof(Mainload).GetMethod("ReadSetData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
        }
    }
}