using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat.Prop
{
    public static class PropUI
    {
        private static List<List<string>> props;
        private static int selectedIndex = -1;
        private static Vector2 scrollList, scrollEdit;
        private static bool needsRefresh = true;
        private static string searchText = "";

        public static void RequestRefresh() => needsRefresh = true;

        public static void Draw()
        {
            if (needsRefresh) { Refresh(); needsRefresh = false; }
            if (props == null) return;

            GUILayout.Label($"Items / Props ({props.Count})", GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = GUILayout.TextField(searchText, GUILayout.Width(120));
            if (GUILayout.Button("Clear")) searchText = "";
            GUILayout.EndHorizontal();

            var filtered = string.IsNullOrEmpty(searchText)
                ? props.Select((m, i) => new { m, i }).ToList()
                : props.Select((m, i) => new { m, i }).Where(x => PropData.GetPropName(x.m).ToLower().Contains(searchText.ToLower())).ToList();

            scrollList = GUILayout.BeginScrollView(scrollList, GUILayout.Height(150));
            for (int j = 0; j < filtered.Count; j++)
            {
                var item = filtered[j];
                string name = PropData.GetPropName(item.m);
                if (GUILayout.Button($"{item.i}: {name}"))
                    selectedIndex = item.i;
            }
            GUILayout.EndScrollView();

            if (selectedIndex >= 0 && selectedIndex < props.Count)
            {
                var prop = props[selectedIndex];
                DrawPropEdit(prop);
            }
        }

        private static void DrawPropEdit(List<string> prop)
        {
            string name = PropData.GetPropName(prop);
            GUILayout.Label($"Item: {name}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 13 });

            scrollEdit = GUILayout.BeginScrollView(scrollEdit, GUILayout.Height(500));

            for (int i = 0; i < prop.Count; i++)
            {
                string val = prop[i];
                string displayVal = val;
                if (float.TryParse(val, out float fv))
                    displayVal = Mathf.RoundToInt(fv).ToString();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"Field {i}:", GUILayout.Width(80));
                string newVal = GUILayout.TextField(displayVal, GUILayout.Width(120));
                if (newVal != displayVal) prop[i] = newVal;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private static void Refresh()
        {
            scrollList = Vector2.zero;
            scrollEdit = Vector2.zero;
            props = PropData.GetProps();
            selectedIndex = -1;
        }
    }
}