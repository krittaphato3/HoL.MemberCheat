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
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? mines.Select((m, i) => new { m, i }).ToList()
                : mines.Select((m, i) => new { m, i })
                    .Where(x => MineData.GetMineName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

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
            GUILayout.Label($"Mine: {name}", UIHelpers.BoldLabel);
            GUILayout.Label("No documented editable fields available for this data type.");
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            mines = MineData.GetMines();
            selectedIndex = -1;
        }
    }
}
