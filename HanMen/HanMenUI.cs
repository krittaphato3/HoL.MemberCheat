using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.HanMen
{
    public static class HanMenUI
    {
        private static List<List<string>> members;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (members == null) return;

            GUILayout.Label($"Civilians (HanMen) ({members.Count})", GUI.skin.box);
            UIHelpers.SearchBar(ref searchText);

            var filtered = string.IsNullOrEmpty(searchText)
                ? members.Select((m, i) => new { m, i }).ToList()
                : members.Select((m, i) => new { m, i })
                    .Where(x => HanMenData.GetName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = HanMenData.GetName(item.m);
                int age = HanMenData.GetAge(item.m);
                if (GUILayout.Button($"{item.i}: {name} (Age {age})"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < members.Count)
            {
                var member = members[selectedIndex];
                DrawMemberEdit(member);
            }
        }

        private static void DrawMemberEdit(List<string> member)
        {
            string name = HanMenData.GetName(member);
            int age = HanMenData.GetAge(member);
            GUILayout.Label($"Editing: {name} (Age {age})", UIHelpers.BoldLabel);

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < member.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string val = GUILayout.TextField(UIHelpers.GetDisplayValue(member[i]), GUILayout.Width(200));
                if (val != member[i])
                {
                    member[i] = val;
                    HanMenData.SetMembers(members);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            members = HanMenData.GetMembers();
            selectedIndex = -1;
        }
    }
}
