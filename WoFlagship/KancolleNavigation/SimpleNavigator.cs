using QuickGraph;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleNavigation
{
    public class SimpleNavigator : INavigator
    {
       

        private AdjacencyGraph<SceneTypes, KancolleActionEdge> navGraph = new AdjacencyGraph<SceneTypes, KancolleActionEdge>();
        private DijkstraShortestPathAlgorithm<SceneTypes, KancolleActionEdge> shortestPathAlg;
        

        public SimpleNavigator()
        {
           

            foreach (SceneTypes scene in Enum.GetValues(typeof(SceneTypes)))
            {
                navGraph.AddVertex(scene);
            }

            //配置转移图
            //母港起点
            List<KancolleActionEdge> portMainAdjs = new List<KancolleActionEdge>();
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Port_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Port_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Port_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Port_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Port_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.SallyMain, new KancolleAction(KancolleWidgetPositions.Port_Sally)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Port, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //编成
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organize_ShipSelect, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Organize_ShipList_Back)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Organzie_Change_Decide, SceneTypes.Organize_ShipSelect, new KancolleAction(KancolleWidgetPositions.Organize_Change_Decide_Back)));

            //补给
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Supply, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //改装
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Remodel, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //入渠
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.RepairMain, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //工厂
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.ArsenalMain, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //出击sally
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Sally_Map)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Sally_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Sally_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.SallyMain, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //map
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Map_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Map_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map_Decide, SceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Map_Decide_Back)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Map_Start, SceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Map_Start_Back)));

            //practice
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Practice_Map)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Practice_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Practice, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //mission远征
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Mission_Map)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Mission_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission, SceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission_Decide, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Mission_Start, SceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Mission_Start_Back)));

            //任务
            navGraph.AddEdge(new KancolleActionEdge(SceneTypes.Quest, SceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));

            shortestPathAlg = new DijkstraShortestPathAlgorithm<SceneTypes, KancolleActionEdge>(navGraph, e => 1);
            
        }

        public List<KancolleActionEdge> Navigate(SceneTypes from, SceneTypes to)
        {
            if (from == to)
                return new List<KancolleActionEdge>();
            VertexPredecessorRecorderObserver<SceneTypes, KancolleActionEdge> predecessorObserver = new VertexPredecessorRecorderObserver<SceneTypes, KancolleActionEdge>();
            predecessorObserver.Attach(shortestPathAlg);
            shortestPathAlg.Compute(from);
            IEnumerable<KancolleActionEdge> edges;
            if(predecessorObserver.TryGetPath(to, out edges))
            {
                return edges.ToList();
            }
            return null;
        }
    }
}
