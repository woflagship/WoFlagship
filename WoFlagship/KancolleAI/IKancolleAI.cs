using WoFlagship.KancolleCore;
using System.Windows.Controls;
using WoFlagship.KancolleCore.Navigation;
using System;

namespace WoFlagship.KancolleAI
{
    public interface IKancolleAI : IKancolleGameDataReceiver, IKancolleSceneRevceiver
    {
        /// <summary>
        /// ai名字
        /// </summary>
        string Name {get; }

        /// <summary>
        /// ai介绍
        /// </summary>
        string Description{ get;}

        int Version { get; }

        void Start();

        void Stop();

        UserControl AIPanel { get; }
    }
}
