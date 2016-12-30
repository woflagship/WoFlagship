using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace WoFlagship.KancolleNavigation
{

    public class Scene
    {
        public SceneTypes SceneType{get; set;} = SceneTypes.Unknown;
        public SceneStates SceneState { get; set; } = SceneStates.Unknown;
        public Scene() { }
        public Scene(SceneTypes sceneType) { this.SceneType = sceneType; SceneState = SceneStates.Unknown; }
        public Scene(SceneTypes sceneType, SceneStates sceneState) { SceneType = sceneType; SceneState = sceneState; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if(obj is SceneTypes)
            {
                return SceneType == ((SceneTypes)obj);
            }
            if(obj is Scene)
            {
                var s = obj as Scene;
                return s.SceneType == SceneType && s.SceneState == SceneState;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return SceneType.GetHashCode() + SceneState.GetHashCode();
        }

        public static bool operator ==(Scene s1, Scene s2)
        {
            if (((object)s1) == null)
                return ((object)s2) == null;
            else
                return s1.Equals(s2);
        }

        public static bool operator !=(Scene s1, Scene s2)
        {
            return !(s1 == s2);
        }

        /// <summary>
        /// s1.SceneType == s2? return true : return false
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(Scene s1, SceneTypes s2)
        {
            if (((object)s1) == null)
                return false;
            else
                return s1.Equals(s2);
        }

        public static bool operator !=(Scene s1, SceneTypes s2)
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
    public enum SceneTypes
    {
        Unknown = 0,
        Port = 0x10,
        Organize = 0x20,//编成
        Organize_ShipSelect = 0x21,//舰娘选择
        Organzie_Change_Decide = 0x22,//决定变换
        Supply = 0x30,//补给
        Remodel = 0x40,//改装
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

    public enum SceneStates
    {
        Unknown,
        Organize_SortByNew,

        Map_Start_True,
        Map_Start_False,

        Mission_Start_True,
        Mission_Start_False,
    }
}
