namespace WoFlagship.KancolleCore.Navigation
{

    public class KancolleScene
    {
        public KancolleSceneTypes SceneType{get; set;} = KancolleSceneTypes.Unknown;
        public KancolleSceneStates SceneState { get; set; } = KancolleSceneStates.Unknown;
        public KancolleScene() { }
        public KancolleScene(KancolleSceneTypes sceneType) { this.SceneType = sceneType; SceneState = KancolleSceneStates.Unknown; }
        public KancolleScene(KancolleSceneTypes sceneType, KancolleSceneStates sceneState) { SceneType = sceneType; SceneState = sceneState; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if(obj is KancolleSceneTypes)
            {
                return SceneType == ((KancolleSceneTypes)obj);
            }
            if(obj is KancolleScene)
            {
                var s = obj as KancolleScene;
                return s.SceneType == SceneType && s.SceneState == SceneState;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SceneType.GetHashCode() + SceneState.GetHashCode();
        }

        public static bool operator ==(KancolleScene s1, KancolleScene s2)
        {
            if (((object)s1) == null)
                return ((object)s2) == null;
            else
                return s1.Equals(s2);
        }

        public static bool operator !=(KancolleScene s1, KancolleScene s2)
        {
            return !(s1 == s2);
        }

        /// <summary>
        /// s1.SceneType == s2? return true : return false
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(KancolleScene s1, KancolleSceneTypes s2)
        {
            if (((object)s1) == null)
                return false;
            else
                return s1.Equals(s2);
        }

        public static bool operator !=(KancolleScene s1, KancolleSceneTypes s2)
        {
            return !(s1 == s2);
        }

        public override string ToString()
        {
            return SceneType.ToString()+" " + SceneState.ToString();
        }
    }

    /// <summary>
    /// 16进制下，总共2位，低位表示某个场景下的小场景，高位表示场景
    /// </summary>
    public enum KancolleSceneTypes
    {
        Unknown = 0,
        Port = 0x10,
        Organize = 0x20,//编成
        Organize_ShipSelect = 0x21,//舰娘选择
        Organzie_Change_Decide = 0x22,//决定变换

        Supply = 0x30,//补给

        Remodel = 0x40,//改装
        Remodel_ItemList = 0x41,//装备列表
        Remodel_ItemList_Decide = 0x42,//装备详细，决定按钮

        RepairMain = 0x50,//入渠
        ArsenalMain = 0x60,//工厂,
        SallyMain = 0x70,//任务选择

        Map = 0x80,//出击地图
        Map_Decide = 0x81,//地图决定
        Map_Start = 0x82,//出击决定



        Mission = 0x90,//远征
        Mission_Decide = 0x91,//远征决定
        Mission_Start= 0x92,//远征开始
        Practice = 0xA0,//演习
        Quest = 0xB0,//任务


        Battle_NextChoice = 0xC0,//进击的画面（进击/回港）
        Battle_NightChoice = 0xC1,//撤退/夜战突入
        Battle_Compass = 0xC2,//罗盘娘
        Battle_Formation = 0xC3,//阵型选择
    }

    public enum KancolleSceneStates
    {
        Unknown,
        Organize_SortByNew,

        Remodel_ItemList_Normal,//装备列表（一般情况）
        Remodel_ItemList_Other,//装备列表（已被别人装备）

        Map_Start_True,
        Map_Start_False,

        Mission_Start_True,
        Mission_Start_False,
    }
}
