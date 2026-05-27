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
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? warEvents.Select((m, i) => new { m, i }).ToList()
                : warEvents.Select((m, i) => new { m, i })
                    .Where(x => WarEventData.GetEventName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

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
            GUILayout.Label($"Event: {name}", UIHelpers.BoldLabel);
            GUILayout.Label("No documented editable fields available for this data type.");
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            warEvents = WarEventData.GetWarEvents();
            selectedIndex = -1;
        }
    }
}
