using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class BattleResult
    {
        /// <summary>
        /// 战斗结果（S，A，B，C，D），有没有E来着
        /// </summary>
        public string WinRank { get; private set; }

        internal BattleResult(api_battleresult_data battlersult_data)
        {
            WinRank = battlersult_data.api_win_rank;
        }
    }
}
