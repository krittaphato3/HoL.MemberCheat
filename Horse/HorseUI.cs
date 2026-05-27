using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Horse
{
    public static class HorseUI
    {
        private static List<List<string>> horses;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (horses == null) return;

            GUILayout.Label($"Horses ({horses.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? horses.Select((m, i) => new { m, i }).ToList()
                : horses.Select((m, i) => new { m, i })
                    .Where(x => HorseData.GetHorseName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = HorseData.GetHorseName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < horses.Count)
            {
                var horse = horses[selectedIndex];
                DrawHorseEdit(horse);
            }
        }

        private static void DrawHorseEdit(List<string> horse)
        {
            string name = HorseData.GetHorseName(horse);
            GUILayout.Label($"Horse: {name}", UIHelpers.BoldLabel);
            GUILayout.Label("No documented editable fields available for this data type.");
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            horses = HorseData.GetHorses();
            selectedIndex = -1;
        }
    }
}
