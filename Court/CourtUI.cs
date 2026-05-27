using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Court
{
    public static class CourtUI
    {
        private static List<List<string>> ministers;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (ministers == null) return;

            GUILayout.Label($"Court Ministers ({ministers.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? ministers.Select((m, i) => new { m, i }).ToList()
                : ministers.Select((m, i) => new { m, i })
                    .Where(x => CourtData.GetMinisterName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = CourtData.GetMinisterName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < ministers.Count)
            {
                var minister = ministers[selectedIndex];
                DrawMinisterEdit(minister);
            }
        }

        private static void DrawMinisterEdit(List<string> minister)
        {
            string name = CourtData.GetMinisterName(minister);
            GUILayout.Label($"Editing: {name}", UIHelpers.BoldLabel);

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < minister.Count; i++)
            {
                if (!CourtData.FieldLabels.ContainsKey(i)) continue;
                string label = CourtData.FieldLabels[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{label}:", GUILayout.Width(100));
                string val = GUILayout.TextField(minister[i], GUILayout.Width(200));
                if (val != minister[i])
                {
                    minister[i] = val;
                    CourtData.SetMinisters(ministers);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            ministers = CourtData.GetMinisters();
            selectedIndex = -1;
        }
    }
}
