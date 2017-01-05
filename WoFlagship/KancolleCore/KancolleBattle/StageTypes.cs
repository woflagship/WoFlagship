using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
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
}
