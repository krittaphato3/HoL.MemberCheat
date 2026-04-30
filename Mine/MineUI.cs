using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Mine
{
    public static class MineUI
    {
        private static List<List<string>> mines;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (mines == null) return;

            GUILayout.Label($"Mines ({mines.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? mines.Select((m, i) => new { m, i }).ToList()
                : mines.Select((m, i) => new { m, i }).Where(x => MineData.GetMineName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = MineData.GetMineName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < mines.Count)
            {
                var mine = mines[selectedIndex];
                DrawMineEdit(mine);
            }
        }

        private static void DrawMineEdit(List<string> mine)
        {
            string name = MineData.GetMineName(mine);
            GUILayout.Label($"Mine: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < mine.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(mine[i], GUILayout.Width(200));
                if (val != mine[i]) mine[i] = val;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            mines = MineData.GetMines();
            selectedIndex = -1;
        }
    }
}