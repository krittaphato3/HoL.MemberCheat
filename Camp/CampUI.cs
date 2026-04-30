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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? camps.Select((m, i) => new { m, i }).ToList()
                : camps.Select((m, i) => new { m, i }).Where(x => CampData.GetCampName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

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
            GUILayout.Label($"Camp: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < camp.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Index {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(camp[i], GUILayout.Width(200));
                if (val != camp[i]) camp[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            camps = CampData.GetCamps();
            selectedIndex = -1;
        }
    }
}