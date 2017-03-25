using QuickGraph;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WoFlagship.KancolleCore.Navigation
{
    public class SimpleNavigator : INavigator
    {
       

        private AdjacencyGraph<KancolleSceneTypes, KancolleActionEdge> navGraph = new AdjacencyGraph<KancolleSceneTypes, KancolleActionEdge>();
        private DijkstraShortestPathAlgorithm<KancolleSceneTypes, KancolleActionEdge> shortestPathAlg;
        

        public SimpleNavigator()
        {
           

            foreach (KancolleSceneTypes scene in Enum.GetValues(typeof(KancolleSceneTypes)))
            {
                navGraph.AddVertex(scene);
            }

            //配置转移图
            //母港起点
            List<KancolleActionEdge> portMainAdjs = new List<KancolleActionEdge>();
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Port_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Port_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Port_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Port_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Port_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.SallyMain, new KancolleAction(KancolleWidgetPositions.Port_Sally)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Port, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //编成
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organize_ShipSelect, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Organize_ShipList_Back)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Organzie_Change_Decide, KancolleSceneTypes.Organize_ShipSelect, new KancolleAction(KancolleWidgetPositions.Organize_Change_Decide_Back)));

            //补给
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Supply, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //改装
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel_ItemList, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Remodel_Items_Return)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Remodel_ItemList_Decide, KancolleSceneTypes.Remodel_ItemList, new KancolleAction(KancolleWidgetPositions.Remodel_Items_Return)));

            //入渠
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.RepairMain, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Repair_ShipList, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Repair_Ships_Return)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Repair_Start, KancolleSceneTypes.Repair_ShipList, new KancolleAction(KancolleWidgetPositions.Repair_Ships_Return)));

            //工厂
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.ArsenalMain, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //出击sally
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Sally_Map)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Sally_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Sally_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.SallyMain, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //map
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Map_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Map_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map_Decide, KancolleSceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Map_Decide_Back)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Map_Start, KancolleSceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Map_Start_Back)));

            //practice
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Practice_Map)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Practice_Mission)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Practice, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));

            //mission远征
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Organize, new KancolleAction(KancolleWidgetPositions.Left_Organize)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Supply, new KancolleAction(KancolleWidgetPositions.Left_Supply)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Remodel, new KancolleAction(KancolleWidgetPositions.Left_Remodel)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.RepairMain, new KancolleAction(KancolleWidgetPositions.Left_Repair)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.ArsenalMain, new KancolleAction(KancolleWidgetPositions.Left_Arsenal)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Map, new KancolleAction(KancolleWidgetPositions.Mission_Map)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Practice, new KancolleAction(KancolleWidgetPositions.Mission_Practice)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission, KancolleSceneTypes.Quest, new KancolleAction(KancolleWidgetPositions.Quest)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission_Decide, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Mission_Start, KancolleSceneTypes.Mission, new KancolleAction(KancolleWidgetPositions.Mission_Start_Back)));

            //任务
            navGraph.AddEdge(new KancolleActionEdge(KancolleSceneTypes.Quest, KancolleSceneTypes.Port, new KancolleAction(KancolleWidgetPositions.Port)));

            shortestPathAlg = new DijkstraShortestPathAlgorithm<KancolleSceneTypes, KancolleActionEdge>(navGraph, e => 1);
            
        }

        public List<KancolleActionEdge> Navigate(KancolleSceneTypes from, KancolleSceneTypes to)
        {
            if (from == to)
                return new List<KancolleActionEdge>();
            VertexPredecessorRecorderObserver<KancolleSceneTypes, KancolleActionEdge> predecessorObserver = new VertexPredecessorRecorderObserver<KancolleSceneTypes, KancolleActionEdge>();
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
