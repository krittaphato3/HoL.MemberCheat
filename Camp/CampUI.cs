using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Camp
{
    public static class CampUI
    {
        private static List<List<string>> camps;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (camps == null) return;

            GUILayout.Label($"Military Camps ({camps.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? camps.Select((m, i) => new { m, i }).ToList()
                : camps.Select((m, i) => new { m, i })
                    .Where(x => CampData.GetCampName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = CampData.GetCampName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < camps.Count)
            {
                var camp = camps[selectedIndex];
                DrawCampEdit(camp);
            }
        }

        private static void DrawCampEdit(List<string> camp)
        {
            string name = CampData.GetCampName(camp);
            GUILayout.Label($"Camp: {name}", UIHelpers.BoldLabel);
            GUILayout.Label("No documented editable fields available for this data type.");
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            camps = CampData.GetCamps();
            selectedIndex = -1;
        }
    }
}
