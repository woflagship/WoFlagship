using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.Utils.BehaviorTree;

namespace WoFlagship.KancolleCore.Navigation
{
    public class KancolleBehaviorFactory
    {
        public static BehaviorTreeBuilder SimpleBattle(int battleDeck)
        {
            BehaviorTreeBuilder behaviorTree = new BehaviorTreeBuilder()
                .Sequence("Check")
                    .Condition("CheckGameData", async () => await Task.Run(() => KancolleGameData.Instance != null))
                    .Condition("IsInBattle", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene.IsBattleScene()))
                    .Selector("BatleType")
                        .Sequence("BattleCompass")
                            //如果是罗盘娘，则空点跳过
                            .Condition("IsInBattleCompass", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene == KancolleSceneTypes.Battle_Compass))
                            .Do("DoBattleCompass", async () =>
                            {
                                await KancolleTaskExecutor.Instance.DoTaskAsync(KancolleTask.BattleSkipTask);
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()
                        .Sequence("BattleFormation")
                            //如果是阵型选择
                            .Condition("IsBattleFormation", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene == KancolleSceneTypes.Battle_Formation))
                            .Do("DoBattleFormation", async () =>
                            {
                                await KancolleTaskExecutor.Instance.DoTaskAsync(new BattleFormationTask(1));
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()
                        .Sequence("BattleNextChoice")
                            //如果是进击选项
                            .Condition("IsBattleNextChoice", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene == KancolleSceneTypes.Battle_NextChoice))
                            .Do("DoBattleNextChoice", async()=>
                            {
                                if (KancolleGameData.Instance.HasBigBrokenShip(battleDeck))
                                {
                                    //存在大破，则撤退
                                    await KancolleTaskExecutor.Instance.DoTaskAsync(new BattleChoiceTask( BattleChoiceTask.BattleChoices.Return));
                                }
                                else
                                {
                                    await KancolleTaskExecutor.Instance.DoTaskAsync(new BattleChoiceTask(BattleChoiceTask.BattleChoices.Next));
                                }
                                
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()
                        .Sequence("BattleNightChoice")
                            //如果是夜战选项
                            .Condition("IsBattleNightChoice", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene == KancolleSceneTypes.Battle_NightChoice))
                            .Do("DoBattleNightChoice", async () =>
                            {
                                await KancolleTaskExecutor.Instance.DoTaskAsync(new BattleChoiceTask(BattleChoiceTask.BattleChoices.Back));
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()
                    .EndComposite()
                .EndComposite();

            return behaviorTree;
        }
    }
}
