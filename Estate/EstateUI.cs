using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Estate
{
    public static class EstateUI
    {
        private static List<List<string>> estates;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (estates == null) return;

            GUILayout.Label($"Estates ({estates.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? estates.Select((m, i) => new { m, i }).ToList()
                : estates.Select((m, i) => new { m, i })
                    .Where(x => EstateData.GetEstateName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = EstateData.GetEstateName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < estates.Count)
            {
                var estate = estates[selectedIndex];
                DrawEstateEdit(estate);
            }
        }

        private static void DrawEstateEdit(List<string> estate)
        {
            string name = EstateData.GetEstateName(estate);
            GUILayout.Label($"Estate: {name}", UIHelpers.BoldLabel);
            GUILayout.Label("No documented editable fields available for this data type.");
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            estates = EstateData.GetEstates();
            selectedIndex = -1;
        }
    }
}
