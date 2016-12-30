using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleBattle
{
    public enum StageTypes
    {
        Engagement,//索敌
        Support,//支援
        LandBase,//路基
        Aerial,//航空战
        Torpedo,//雷击
        Shelling,//炮击

        //subtypes
        SUndefined,
        SMain,//第一舰队
        SEscort,//第二舰队
        SNight,//夜战
        SOpening,//开幕雷，开幕反潜
        SClosing

    }

    public class Stage
    {
        public StageTypes StageType { get; set; }
        public StageTypes SubType { get; set; }
        public List<Attack> Attacks { get; set; }

        public override string ToString()
        {
            string str = string.Format("Stage:{0}\tSubStage:{1}\n", StageType.ToString(), SubType.ToString());
            if (Attacks != null)
            {
                foreach (var attack in Attacks)
                {
                    str += attack.ToString() + "\n";
                }
            }

            return str;
        }
    }
}
