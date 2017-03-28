using System;

namespace WoFlagship.KancolleCore.Navigation
{
    public enum TaskPriority
    {
        Realtime,
        High,
        Normal,
        Low,
    }

    public enum TaskTypes
    {
        Organize,
        Supply,
        Repair,
        Remodel,
        BuildShip,
        BuildItem
    }

    public abstract class KancolleTask
    {
        /// <summary>
        /// 刷新数据用的任务，和new RefreshDataTask()等价，用这个的话就不用再new了，效率比较高
        /// </summary>
        public static RefreshDataTask RefreshDataTask { get; private set; } = new RefreshDataTask();

        public DateTime TimeStamp { get; private set; } = DateTime.Now;

        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
    }

    public class OrganizeTask : KancolleTask
    {
        /// <summary>
        /// 舰队号，从0开始
        /// </summary>
        public int OrganizedDeck { get; private set; }
        /// <summary>
        /// 长度必须为6
        /// </summary>
        public int[] Ships { get; private set; }

        /// <summary>
        /// 编成，organizeFleet从0开始，ships的id是ownship的id
        /// </summary>
        /// <param name="organizeDeck">编成的舰队号，从0开始算</param>
        /// <param name="ships">舰队船的id，ownShip的id，-1表示没有船</param>
        public OrganizeTask(int organizeDeck, int[] ships)
        {
            OrganizedDeck = organizeDeck;
            Ships = ships;
        }

    }

    public class SupplyTask : KancolleTask
    {
        public int SupplyDeck { get; private set; }

        /// <summary>
        /// 补给，supplyDeck从0开始
        /// </summary>
        /// <param name="supplyDeck">补给舰队，从0开始</param>
        public SupplyTask(int supplyDeck)
        {
            SupplyDeck = supplyDeck;
        }
    }

    public class QuestTask : KancolleTask
    {
        public QuestTask(int[] requireTasks)
        {

        }
    }

    /// <summary>
    /// 出击选择地图
    /// </summary>
    public class MapTask : KancolleTask
    {
        /// <summary>
        /// 出击的舰队，从0开始
        /// </summary>
        public int Fleet { get; private set; }

        /// <summary>
        /// 地图id
        /// </summary>
        public int MapId { get; private set; }


        public MapTask(int mapId)
        {
            MapId = mapId;
            Fleet = 0;
        }

        public MapTask(int deck, int mapId)
        {
            Fleet = deck;
            MapId = mapId;
        }
    }


    public abstract class BattleTask : KancolleTask
    {

    }

    public class BattleChoiceTask : BattleTask
    {
        public enum BattleChoices
        {
            /// <summary>
            /// 进击
            /// </summary>
            Next,

            /// <summary>
            /// 进击
            /// </summary>
            Return,

            /// <summary>
            /// 撤退
            /// </summary>
            Back,

            /// <summary>
            /// 夜战追击
            /// </summary>
            Night,
        }

        public BattleChoices BattleChoice { get; private set; }

        public BattleChoiceTask(BattleChoices choice)
        {
            this.BattleChoice = choice;
        }
    }

    public class BattleFormationTask : BattleTask
    {
        /// <summary>
        /// 单纵-1，复纵-2，轮形-3，梯形-4，单横-5
        /// </summary>
        public int Formation { get; private set; }

        /// <summary>
        /// 单纵-1，复纵-2，轮形-3，梯形-4，单横-5
        /// </summary>
        /// <param name="Formation"></param>
        public BattleFormationTask(int formation)
        {
            Formation = formation;
        }

    }

    /// <summary>
    /// 跳过战斗之间的画面，比如航线、出新舰娘等这些情况
    /// </summary>
    public class BattleSkipTask : BattleTask
    {

    }

    //远征
    public class MissionTask : KancolleTask
    {
        public int MissionId { get; private set; }
        public int MissionFleet { get; private set; }
        /// <summary>
        /// 默认为第2舰队远征
        /// </summary>
        /// <param name="missionId">远征id</param>
        public MissionTask(int missionId)
        {
            MissionId = missionId;
            MissionFleet = 1;
        }

        /// <summary>
        /// 远征
        /// </summary>
        /// <param name="missionId">远征id</param>
        /// <param name="MissionDeck">远征舰队，从0开始算第一舰队</param>
        public MissionTask(int missionId, int missionFleet)
        {
            MissionId = missionId;
            MissionFleet = missionFleet;
        }


    }

    public class RemodelTask : KancolleTask
    {
        /// <summary>
        /// 要改装的船所在的港口，从0开始
        /// </summary>
        public int TargetDeck { get; private set; }

        /// <summary>
        /// 要改装的船所在的未知，从0开始
        /// </summary>
        public int TargetPosition { get; private set; }

        /// <summary>
        /// 要改装的装备id，不放的位置为-1，-1不能在中间
        /// </summary>
        public int[] SlotItemNos { get; private set; }

        /// <summary>
        /// 改修
        /// </summary>
        /// <param name="targetDeck">要改装的船所在的港口，从0开始</param>
        /// <param name="targetPosition">要改装的船所在的未知，从0开始</param>
        /// <param name="slotItemNos">要改装的装备id，不放的位置为-1，-1不能在中间</param>
        public RemodelTask(int targetDeck, int targetPosition, int[] slotItemNos)
        {
            TargetDeck = targetDeck;
            TargetPosition = targetPosition;
            SlotItemNos = slotItemNos;
        }


    }

    public class RepairTask : KancolleTask
    {
        /// <summary>
        /// 需要入渠的舰娘no
        /// </summary>
        public int ShipNo { get; private set; }

        /// <summary>
        /// 入渠的位置，取值：0-3
        /// </summary>
        public int Dock { get; private set; }

        /// <summary>
        /// 是否使用高速修复
        /// </summary>
        public bool UseFastRepair { get; private set; }

        /// <summary>
        /// 入渠
        /// </summary>
        /// <param name="shipNo">需要入渠的舰娘no</param>
        /// <param name="dock">入渠的位置，取值：0-3</param>
        /// <param name="useFastRepair">是否使用高速修复</param>
        public RepairTask(int shipNo, int dock, bool useFastRepair)
        {
            ShipNo = shipNo;
            Dock = dock;
            UseFastRepair = useFastRepair;
        }
    }

    /// <summary>
    /// 用于手动刷新数据的任务
    /// </summary>
    public class RefreshDataTask : KancolleTask{
    }
}
