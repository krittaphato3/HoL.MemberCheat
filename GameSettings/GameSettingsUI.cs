using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HoLMod.MemberCheat.GameSettings
{
    public static class GameSettingsUI
    {
        private static Vector2 scroll;
        private const string TimeField = "Time_now";

        public static void Draw()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            UIHelpers.Section("Date / Time");
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

                GUILayout.Space(10);
                UIHelpers.Section("Quick Date Presets");
                GUI.color = Color.green;
                UIHelpers.ActionButtons(
                    ("+1 Day", () => { timeList[2] = Mathf.Clamp(timeList[2] + 1, 1, 30); ApplyTimeList(timeList); }),
                    ("+1 Month", () => { timeList[1] = Mathf.Clamp(timeList[1] + 1, 1, 12); ApplyTimeList(timeList); }),
                    ("+1 Year", () => { timeList[0]++; ApplyTimeList(timeList); }),
                    ("-1 Day", () => { timeList[2] = Mathf.Clamp(timeList[2] - 1, 1, 30); ApplyTimeList(timeList); }),
                    ("-1 Month", () => { timeList[1] = Mathf.Clamp(timeList[1] - 1, 1, 12); ApplyTimeList(timeList); }),
                    ("-1 Year", () => { timeList[0] = Math.Max(1, timeList[0] - 1); ApplyTimeList(timeList); })
                );
                GUI.color = Color.white;
            }
            GUILayout.EndScrollView();
        }

        private static List<int> GetTimeList()
        {
            var field = typeof(Mainload).GetField(TimeField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field?.GetValue(null) as List<int>;
        }

        private static void ApplyTimeList(List<int> timeList)
        {
            var field = typeof(Mainload).GetField(TimeField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, timeList);
            UIHelpers.InvokeReadSetData();
        }
    }
}
