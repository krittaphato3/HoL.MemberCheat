using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.WarEvent
{
    public static class WarEventUI
    {
        private static List<List<string>> warEvents;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (warEvents == null) return;

            GUILayout.Label($"War Events ({warEvents.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? warEvents.Select((m, i) => new { m, i }).ToList()
                : warEvents.Select((m, i) => new { m, i }).Where(x => WarEventData.GetEventName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = WarEventData.GetEventName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < warEvents.Count)
            {
                var warEvent = warEvents[selectedIndex];
                DrawWarEventEdit(warEvent);
            }
        }

        private static void DrawWarEventEdit(List<string> warEvent)
        {
            string name = WarEventData.GetEventName(warEvent);
            GUILayout.Label($"Event: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < warEvent.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(warEvent[i], GUILayout.Width(200));
                if (val != warEvent[i]) warEvent[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            warEvents = WarEventData.GetWarEvents();
            selectedIndex = -1;
        }
    }
}