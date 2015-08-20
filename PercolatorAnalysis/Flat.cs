using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percolator.AnalysisServices
{
    internal class Flat
    {
        public Dictionary<string, string> PositionValues { get; set; }
        public Dictionary<string, object> MeasureValues { get; set; }

        Flat()
        {
            MeasureValues = new Dictionary<string, object>();
            PositionValues = new Dictionary<string, string>();
        }

        public static IEnumerable<Flat> Flatten(CellSet cellSet)
        {
            List<string> valueMeasures = new List<string>();
            var axes = cellSet.Axes.Cast<Microsoft.AnalysisServices.AdomdClient.Axis>().ToList();
            var members = new List<MemberPair>();
                      
            for (int i = 0; i < cellSet.Axes.Count; i++)
                for(int j = 0; j < cellSet.Axes[i].Positions.Count; j++)
                    for (int k = 0; k < cellSet.Axes[i].Positions[j].Members.Count; k++)
                    {
                        var levelName = cellSet.Axes[i].Positions[j].Members[k].ParentLevel.Caption;
                        if (levelName == "MeasuresLevel")
                        {
                            var name = cellSet.Axes[i].Positions[j].Members[i].Caption;
                            if(!valueMeasures.Contains(name))
                                valueMeasures.Add(name);
                        }
                        else
                        {
                            var caption = cellSet.Axes[i].Positions[j].Members[k].Caption;
                            members.Add(new MemberPair(levelName, caption));
                        }
                    }
            
            var valueCount = valueMeasures.Count;
            var memberCount = members.Select(x => x.Level).Distinct().Count();
            var totalLoops = valueCount == 0 ? 0 : cellSet.Cells.Count / valueCount;
            for (int i = 0; i < totalLoops; i++)
            {
                var flat = new Flat();
                for(int j = 0; j < valueCount; j++)
                {
                    flat.MeasureValues.Add(valueMeasures[j], cellSet[j + (i * valueCount)].Value);
                }
                for(int j = 0; j < memberCount; j++)
                {
                    flat.PositionValues.Add(members[j].Level, members[j + (i * memberCount)].Member);
                }
                yield return flat;
            }
        }
        struct MemberPair
        {
            public string Level { get; set; }
            public string Member { get; set; }

            public MemberPair(string level, string member)
                : this()
            {
                Level = level;
                Member = member;
            }
        }
    }
}
