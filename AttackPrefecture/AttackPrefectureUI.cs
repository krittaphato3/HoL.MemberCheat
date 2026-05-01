using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.AttackPrefecture
{
    public static class AttackPrefectureUI
    {
        private static List<int> prefectureIDs;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (prefectureIDs == null) return;

            GUILayout.Label($"Hostile Prefectures ({prefectureIDs.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? prefectureIDs.Select((id, i) => new { id, i }).ToList()
                : prefectureIDs.Select((id, i) => new { id, i }).Where(x => x.id.ToString().Contains(searchText)).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                if (GUILayout.Button($"{item.i}: Prefecture {item.id}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < prefectureIDs.Count)
            {
                int currentID = prefectureIDs[selectedIndex];
                DrawPrefectureEdit(currentID);
            }
        }

        private static void DrawPrefectureEdit(int id)
        {
            GUILayout.Label($"Prefecture: {id}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(100));
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID:", GUILayout.Width(40));
            string val = GUILayout.TextField(id.ToString(), GUILayout.Width(80));
            if (int.TryParse(val, out int newID) && newID >= 0)
                prefectureIDs[selectedIndex] = newID;
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            prefectureIDs = AttackPrefectureData.GetAttackPrefectures();
            selectedIndex = -1;
        }
    }
}