using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Fief
{
    public static class FiefUI
    {
        private static List<List<string>> fiefs;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (fiefs == null) return;

            GUILayout.Label($"Fiefs ({fiefs.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? fiefs.Select((m, i) => new { m, i }).ToList()
                : fiefs.Select((m, i) => new { m, i }).Where(x => FiefData.GetFiefName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = FiefData.GetFiefName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < fiefs.Count)
            {
                var fief = fiefs[selectedIndex];
                DrawFiefEdit(fief);
            }
        }

        private static void DrawFiefEdit(List<string> fief)
        {
            string name = FiefData.GetFiefName(fief);
            GUILayout.Label($"Fief: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < fief.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Index {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(fief[i], GUILayout.Width(200));
                if (val != fief[i]) fief[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            fiefs = FiefData.GetFiefs();
            selectedIndex = -1;
        }
    }
}