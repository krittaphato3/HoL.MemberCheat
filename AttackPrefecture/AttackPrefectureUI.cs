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
            UIHelpers.SearchBar(ref searchText);

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
                DrawPrefectureEdit();
            }
        }

        private static void DrawPrefectureEdit()
        {
            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(100));
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID:", GUILayout.Width(40));
            string val = GUILayout.TextField(prefectureIDs[selectedIndex].ToString(), GUILayout.Width(80));
            if (int.TryParse(val, out int newID) && newID >= 0 && newID != prefectureIDs[selectedIndex])
            {
                prefectureIDs[selectedIndex] = newID;
                AttackPrefectureData.SetAttackPrefectures(prefectureIDs);
            }
            GUILayout.EndHorizontal();
            UIHelpers.DangerButton("Remove Selected", () =>
            {
                prefectureIDs.RemoveAt(selectedIndex);
                AttackPrefectureData.SetAttackPrefectures(prefectureIDs);
                selectedIndex = -1;
                Refresh();
            });
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
