using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using WoFlagship.KancolleCommon;
using System.Windows.Controls;

namespace WoFlagship.KancolleAI
{
    public interface IKancolleAI : IKancolleAPIReceiver, IKancolleGameDataReceiver
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
