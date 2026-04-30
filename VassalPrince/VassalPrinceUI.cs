using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.VassalPrince
{
    public static class VassalPrinceUI
    {
        private static List<List<string>> princes;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (princes == null) return;

            GUILayout.Label($"Vassal Princes ({princes.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? princes.Select((m, i) => new { m, i }).ToList()
                : princes.Select((m, i) => new { m, i }).Where(x => VassalPrinceData.GetPrinceName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = VassalPrinceData.GetPrinceName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < princes.Count)
            {
                var prince = princes[selectedIndex];
                DrawPrinceEdit(prince);
            }
        }

        private static void DrawPrinceEdit(List<string> prince)
        {
            string name = VassalPrinceData.GetPrinceName(prince);
            GUILayout.Label($"Prince: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < prince.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(prince[i], GUILayout.Width(200));
                if (val != prince[i]) prince[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            princes = VassalPrinceData.GetPrinces();
            selectedIndex = -1;
        }
    }
}