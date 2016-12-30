using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WoFlagship.KancolleCommon;
using WoFlagship.Utils.BehaviorTree;

namespace WoFlagship.KancolleAI.SimpleAI
{
    class SimpleAI : IKancolleAI
    {
        public UserControl AIPanel
        {
            get
            {
                return null;
            }
        }

        public string Description
        {
            get
            {
                return "测试用AI";
            }
        }

        public string Name
        {
            get
            {
                return "测试用AI";
            }
        }

        public int Version
        {
            get
            {
                return 1;
            }
        }

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
           
        }

        public void OnGameDataUpdatedHandler(KancolleGameData gameData)
        {
           
        }

        public void Start()
        {
            BehaviorTreeBuilder builder = new BehaviorTreeBuilder();
            IBehaviorNode rootNode;
            builder .Selector("总任务分类")
                        .Sequence("做日常")
                        .EndComposite()
                        
                        .Sequence("练级")
                        .EndComposite()

                    .EndComposite(out rootNode);
        }

        public void Stop()
        {
            
        }
    }
}
