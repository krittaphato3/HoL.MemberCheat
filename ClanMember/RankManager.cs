using System.Collections.Generic;
using System.Linq;

namespace HoLMod.MemberCheat.ClanMember
{
    public static class RankManager
    {
        public class RankPreset
        {
            public string Code;
            public string Label;
            public RankPreset(string code, string label) { Code = code; Label = label; }
        }

        public class RankPresetCategory
        {
            public string CategoryName;
            public List<RankPreset> Presets;
            public RankPresetCategory(string name, List<RankPreset> presets) { CategoryName = name; Presets = presets; }
        }

        public static List<RankPresetCategory> Presets = new List<RankPresetCategory>
        {
            new RankPresetCategory("None / Special", new List<RankPreset>
            {
                new RankPreset("0@0@0", "None"),
                new RankPreset("1@7@0", "Empress Dowager"),
            }),
            new RankPresetCategory("Official – No Position", new List<RankPreset>
            {
                new RankPreset("5@0@0", "Rank-7 No Firm"),
                new RankPreset("5@1@0", "Rank-6 No Firm"),
                new RankPreset("5@2@0", "Rank-5 No Firm"),
                new RankPreset("5@3@0", "Rank-4 No Firm"),
                new RankPreset("5@4@0", "Rank-3 No Firm"),
                new RankPreset("5@5@0", "Rank-2 No Firm"),
                new RankPreset("5@6@0", "Rank-1 No Firm"),
            }),
            new RankPresetCategory("Military", new List<RankPreset>
            {
                new RankPreset("5@0@2", "County Marshal"),
                new RankPreset("5@1@2", "Gui'de Officer"),
                new RankPreset("5@2@2", "Provincial Marshal"),
                new RankPreset("5@2@4", "General of Cavalry"),
                new RankPreset("5@3@2", "General of Xuanwei"),
                new RankPreset("5@3@3", "General of Zuolin"),
                new RankPreset("5@3@4", "General of Youlin"),
                new RankPreset("5@3@5", "General of Zuoxiao"),
                new RankPreset("5@3@6", "General of Youxiao"),
                new RankPreset("5@3@7", "General of Zuowu"),
                new RankPreset("5@3@8", "General of Youwu"),
                new RankPreset("5@3@9", "General of Zuotun"),
                new RankPreset("5@3@10", "General of Youtun"),
                new RankPreset("5@3@11", "General of Zuohou"),
                new RankPreset("5@3@12", "General of Youhou"),
                new RankPreset("5@3@13", "General of Zuoyu"),
                new RankPreset("5@3@14", "General of Youyu"),
            }),
            new RankPresetCategory("Government", new List<RankPreset>
            {
                new RankPreset("5@0@1", "Deputy Magistrate"),
                new RankPreset("5@0@3", "Yihui Officer"),
                new RankPreset("5@1@1", "Magistrate"),
                new RankPreset("5@2@1", "Deputy Governor"),
                new RankPreset("5@2@3", "Chief Censor"),
                new RankPreset("5@3@1", "Provincial Governor"),
                new RankPreset("5@4@1", "Justice Minister"),
                new RankPreset("5@4@2", "Civil Minister"),
                new RankPreset("5@4@3", "Revenue Minister"),
                new RankPreset("5@4@4", "Rites Minister"),
                new RankPreset("5@4@5", "Industry Minister"),
                new RankPreset("5@4@6", "War Minister"),
                new RankPreset("5@5@1", "Grand Minister"),
                new RankPreset("5@5@2", "Imperial Censor"),
                new RankPreset("5@6@1", "Chancellor"),
            }),
            new RankPresetCategory("Censors (Regional)", new List<RankPreset>
            {
                new RankPreset("5@4@7", "Nan, Sanchuan, Shu"),
                new RankPreset("5@4@8", "Danyang, Chenliu, Changsha"),
                new RankPreset("5@4@9", "Kuaiji, Guangling, Taiyuan"),
                new RankPreset("5@4@10", "Yizhou, Nanhai, Yunnan"),
            }),
            new RankPresetCategory("Royal", new List<RankPreset>
            {
                new RankPreset("6@0@0", "Feudal Princess"),
                new RankPreset("6@1@0", "Feudal Prince"),
                new RankPreset("6@2@0", "Prince Royal"),
                new RankPreset("6@3@0", "Princess"),
                new RankPreset("6@5@0", "Emperor"),
            }),
        };

        public static int FindRankIndex(List<string> member)
        {
            for (int i = 0; i < member.Count; i++)
                if (member[i].Contains("@") && member[i].Count(c => c == '@') >= 2)
                    return i;
            return -1;
        }
    }
}