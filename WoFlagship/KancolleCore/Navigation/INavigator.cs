using System;
using System.Collections.Generic;

namespace WoFlagship.KancolleCore.Navigation
{
    interface INavigator
    {
        /// <summary>
        /// 导航从from到to
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>可到达的行为列表，null表示不可到，空表示已经到了</returns>
        List<KancolleActionEdge> Navigate(KancolleSceneTypes from, KancolleSceneTypes to);
    }
}
