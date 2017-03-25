using AForge.Imaging;
using System;
using System.Drawing;
using static WoFlagship.Properties.Resources;

namespace WoFlagship.KancolleCore.Navigation
{
    public class SceneRecognizer
    {
        public const string ComparisonFolder = "Comparison";

        private Rectangle Rect_WholeScreen { get; set; } = new Rectangle(0, 0, 800, 480);
        private Rectangle Rect_MainPort_Title { get; set; } = new Rectangle(0, 0, 800, 70);
        private Rectangle Rect_MainPort_Left { get; set; } = new Rectangle(0, 0, 13, 400);
        private Rectangle Rect_LeftToolbar { get; set; } = new Rectangle(0, 120, 100, 280);
        private Rectangle Rect_Sally { get; set; } = new Rectangle(100, 70, 700, 70);
        private Rectangle Rect_Quest { get; set; } = new Rectangle(100, 70, 700, 30);

        private Rectangle Rect_Organize_ShipSelect { get; set; } = new Rectangle(350, 100, 400, 20);
        private Rectangle Rect_Organize_Change_Decide { get; set; } = new Rectangle(630, 425, 120, 35);
        private Rectangle Rect_Organize_Sort { get; set; } = new Rectangle(750, 105, 45, 15);

        private Rectangle Rect_Remodel_Remodel = new Rectangle(600, 410, 110, 70);
        private Rectangle Rect_Remodel_ItemList = new Rectangle(355, 70, 55, 25);
        private Rectangle Rect_Remodel_ItemList_Mode = new Rectangle(710, 100, 80, 20);
        private Rectangle Rect_Remodel_ItemList_Decide = new Rectangle(640, 420, 110, 60);
        private Rectangle Rect_Remodel_ItemList_Other_Decide = new Rectangle(250, 300, 130, 40);

        private Rectangle Rect_Repair_ShipSelect = new Rectangle(400, 100, 310, 20);
        private Rectangle Rect_Repair_Sort = new Rectangle(750, 105, 45, 15);
        private Rectangle Rect_Repair_Start = new Rectangle(600, 410, 170, 70);

        private Rectangle Rect_Mission_Decide { get; set; } = new Rectangle(620, 420, 120, 60);
        private Rectangle Rect_Mission_Start { get; set; } = new Rectangle(530, 420, 170, 60);

        private Rectangle Rect_Map_Decide { get; set; } = new Rectangle(630, 420, 110, 60);
        private Rectangle Rect_Map_Start { get; set; } = new Rectangle(530, 420, 170, 60);
        private Rectangle Rect_Map_Start2 { get; set; } = new Rectangle(120, 105, 100, 25);//这块区域用于区分Mission_start（主要是false的情况）,因为两者比较像

        private Rectangle Rect_Battle_LeftChoice { get; set; } = new Rectangle(250, 190, 90, 90);
        private Rectangle Rect_Battle_RightChoice { get; set; } = new Rectangle(460, 190, 90, 90);
        private Rectangle Rect_Battle_Compass = new Rectangle(300, 140, 200, 200);
        private Rectangle Rect_Battle_Formation = new Rectangle(400, 170, 95, 25);

        public const float Threshold = 0.9f;//必须大于0

        //private Bitmap bm_sally, bm_map, bm_mission, bm_practice;
        //private Bitmap bm_map_decide, bm_map_start_true, bm_map_start_false, bm_map_start_false2;
        //private Bitmap bm_mission_decide, bm_mission_start_true, bm_mission_start_false;
        //private Bitmap bm_mainport_title, bm_mainport_left;
        //private Bitmap bm_organize, bm_organize_shipselect,bm_organize_change_decide_true, bm_organize_change_decide_false,  bm_organize_sortbynew;
        //private Bitmap bm_supply;
        //private Bitmap bm_remodel;
        //private Bitmap bm_repair;
        //private Bitmap bm_arsenal;
        //private Bitmap bm_quest;

        //private Bitmap bm_battle_next, bm_battle_back, bm_battle_return, bm_battle_night, bm_battle_compass;
        //private Bitmap bm_battle_formation;

        private ExhaustiveTemplateMatching matching = new ExhaustiveTemplateMatching(Threshold);

        public SceneRecognizer()
        {
            
            //bm_mainport_title = new Bitmap(Path.Combine(ComparisonFolder, "MainPort_Title.bmp"));
            //bm_mainport_left = new Bitmap(Path.Combine(ComparisonFolder, "MainPort_Left.bmp"));
            //bm_sally = new Bitmap(Path.Combine(ComparisonFolder, "Sally.bmp"));
            //bm_map = new Bitmap(Path.Combine(ComparisonFolder, "Map.bmp"));
            //bm_mission = new Bitmap(Path.Combine(ComparisonFolder, "Mission.bmp"));
            //bm_practice = new Bitmap(Path.Combine(ComparisonFolder, "Practice.bmp"));

            //bm_map_decide = new Bitmap(Path.Combine(ComparisonFolder, "Map_Decide.bmp"));
            //bm_map_start_true = new Bitmap(Path.Combine(ComparisonFolder, "Map_Start_True.bmp"));
            //bm_map_start_false = new Bitmap(Path.Combine(ComparisonFolder, "Map_Start_False.bmp"));
            //bm_map_start_false2 = new Bitmap(Path.Combine(ComparisonFolder, "Map_Start_False2.bmp"));

            //bm_mission_decide = new Bitmap(Path.Combine(ComparisonFolder, "Mission_Decide.bmp"));
            //bm_mission_start_true = new Bitmap(Path.Combine(ComparisonFolder, "Mission_Start_True.bmp"));
            //bm_mission_start_false = new Bitmap(Path.Combine(ComparisonFolder, "Mission_Start_False.bmp"));

            //bm_organize = new Bitmap(Path.Combine(ComparisonFolder, "Organize.bmp"));
            //bm_organize_shipselect = new Bitmap(Path.Combine(ComparisonFolder, "Organize_ShipSelect.bmp"));
            //bm_organize_change_decide_true = new Bitmap(Path.Combine(ComparisonFolder, "Organize_Change_Decide_True.bmp"));
            //bm_organize_change_decide_false = new Bitmap(Path.Combine(ComparisonFolder, "Organize_Change_Decide_False.bmp"));
            //bm_organize_sortbynew = new Bitmap(Path.Combine(ComparisonFolder, "Organize_SortByNew.bmp"));


            //bm_supply = new Bitmap(Path.Combine(ComparisonFolder, "Supply.bmp"));
            //bm_remodel = new Bitmap(Path.Combine(ComparisonFolder, "Remodel.bmp"));
            //bm_repair = new Bitmap(Path.Combine(ComparisonFolder, "Repair.bmp"));
            //bm_arsenal = new Bitmap(Path.Combine(ComparisonFolder, "Arsenal.bmp"));

            //bm_quest = new Bitmap(Path.Combine(ComparisonFolder, "Quest.bmp"));

            //bm_battle_back = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Back.bmp"));
            //bm_battle_night = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Night.bmp"));
            //bm_battle_next = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Next.bmp"));
            //bm_battle_return = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Return.bmp"));
            //bm_battle_compass = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Compass.bmp"));
            //bm_battle_formation = new Bitmap(Path.Combine(ComparisonFolder, "Battle_Formation.bmp"));
        }

        //以下所有的判断逻辑可能存在不同的写法...
        public KancolleScene GetSceneTypeFromBitmap(Bitmap screenBitmap)
        {
            KancolleScene scene = new KancolleScene();
            double maxSim = 0, sim;

            //需要特殊处理的
            //装备从别人那拿过来决定，remodel衍生出来的场景
            if (SceneSimilarity(screenBitmap, Rect_Remodel_ItemList_Other_Decide, Remodel_ItemList_Other_Decide) > Threshold)
            {
                scene.SceneType = KancolleSceneTypes.Remodel_ItemList_Other_Decide;
                return scene;
            }

            //判断是否存在港口的title
            sim = SceneSimilarity(screenBitmap, Rect_MainPort_Title, MainPort_title);
            if (sim > Threshold)//如果是，则表示未出击，属于平常状态
            {
                //优先判断各个场景中的小场景

                if (SceneSimilarity(screenBitmap, Rect_MainPort_Left, MainPort_Left) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Port;
                    return scene;
                }

                //在编成、改装等界面选取列表时，左侧图片会变暗，所以识别会有些问题，因此就在这里识别列表选取的相关场景
                //编成，修改舰娘
                if (SceneSimilarity(screenBitmap, Rect_Organize_ShipSelect, Organize_ShipSelect) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Organize_ShipSelect;
                    maxSim = 0;
                    sim = SceneSimilarity(screenBitmap, Rect_Organize_Sort, Organize_SortByNew);
                    if (sim > maxSim && sim > Threshold) { maxSim = sim; scene.SceneState = KancolleSceneStates.Organize_SortByNew; }

                    return scene;
                }

                //入渠，选择舰娘
                if (SceneSimilarity(screenBitmap, Rect_Repair_ShipSelect, Repair_ShipSelect) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Repair_ShipList;
                    maxSim = 0;
                    sim = SceneSimilarity(screenBitmap, Rect_Repair_Sort, Repair_SortByNew);
                    if (sim > maxSim && sim > Threshold) { maxSim = sim; scene.SceneState = KancolleSceneStates.Repair_SortByNew; }

                    return scene;
                }
                else if (SceneSimilarity(screenBitmap, Rect_Repair_Start, Repair_Start_True) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Repair_Start;
                    scene.SceneState = KancolleSceneStates.Repair_Start_True;
                    return scene;

                }
                else if (SceneSimilarity(screenBitmap, Rect_Repair_Start, Repair_Start_False) > Threshold)
                {

                    scene.SceneType = KancolleSceneTypes.Repair_Start;
                    scene.SceneState = KancolleSceneStates.Repair_Start_False;
                    return scene;
                }

                if (SceneSimilarity(screenBitmap, Rect_Organize_Change_Decide, Organize_Change_Decide_True) > Threshold || SceneSimilarity(screenBitmap, Rect_Organize_Change_Decide, Organize_Change_Decide_False) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Organzie_Change_Decide;
                    return scene;
                }


                //远征的决定按钮
                if (SceneSimilarity(screenBitmap, Rect_Mission_Decide, Mission_Decide) > Threshold && SceneSimilarity(screenBitmap, Rect_Sally, Mission) > Threshold)
                {
                        scene.SceneType = KancolleSceneTypes.Mission_Decide;
                    return scene;
                }

                //海域的决定按钮
                if (SceneSimilarity(screenBitmap, Rect_Map_Decide, Map_Decide) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Map_Decide;
                    return scene;
                }

                //远征开始
                if (SceneSimilarity(screenBitmap, Rect_Mission_Start, Mission_Start_True) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Mission_Start;
                    scene.SceneState = KancolleSceneStates.Mission_Start_True;
                    return scene;
                }

                //海域出击开始
                if (SceneSimilarity(screenBitmap, Rect_Map_Start, Map_Start_True) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Map_Start;
                    scene.SceneState = KancolleSceneStates.Map_Start_True;
                    return scene;
                }

                //海域出击开始,由于在不可出击状态下和远征开始很像，所以需要bm_map_start_false2作辅助判断
                if (SceneSimilarity(screenBitmap, Rect_Map_Start, Map_Start_False) > Threshold && SceneSimilarity(screenBitmap, Rect_Map_Start2, Map_Start_False2) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Map_Start;
                    scene.SceneState = KancolleSceneStates.Map_Start_False;
                    return scene;
                }

                if (SceneSimilarity(screenBitmap, Rect_Mission_Start, Mission_Start_False) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Mission_Start;
                    scene.SceneState = KancolleSceneStates.Mission_Start_False;
                    return scene;
                }

              

                //判断是否为出击、演习、远征选择
                if (SceneSimilarity(screenBitmap, Rect_WholeScreen, Sally) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.SallyMain;
                    return scene;
                }
                maxSim = 0;
                sim = SceneSimilarity(screenBitmap, Rect_Sally, Map);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Map; }
                sim = SceneSimilarity(screenBitmap, Rect_Sally, Mission);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Mission; }
                sim = SceneSimilarity(screenBitmap, Rect_Sally, Practice);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Practice; }
                if (maxSim > Threshold)
                    return scene;


                //判断是否为编成、补给、改装、入渠、工厂中的一个
                maxSim = 0;
                sim = SceneSimilarity(screenBitmap, Rect_LeftToolbar, Organize);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Organize; }
                sim = SceneSimilarity(screenBitmap, Rect_LeftToolbar, Supply);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Supply; }
                sim = SceneSimilarity(screenBitmap, Rect_LeftToolbar, Remodel);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.Remodel; }
                sim = SceneSimilarity(screenBitmap, Rect_LeftToolbar, Repair);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.RepairMain; }
                sim = SceneSimilarity(screenBitmap, Rect_LeftToolbar, Arsenal);
                if (sim > maxSim) { maxSim = sim; scene.SceneType = KancolleSceneTypes.ArsenalMain; }
                if (maxSim > Threshold)
                {
                    if (scene.SceneType == KancolleSceneTypes.Remodel)
                    {
                        //装备决定
                        if (SceneSimilarity(screenBitmap, Rect_Remodel_ItemList_Decide, Remodel_ItemList_Decide_True) > Threshold)
                        {
                            scene.SceneType = KancolleSceneTypes.Remodel_ItemList_Decide;
                            return scene;
                        }

                        //装备列表
                        if (SceneSimilarity(screenBitmap, Rect_Remodel_ItemList, Remodel_ItemList) > Threshold)
                        {
                            scene.SceneType = KancolleSceneTypes.Remodel_ItemList;
                            if (SceneSimilarity(screenBitmap, Rect_Remodel_ItemList_Mode, Remodel_ItemList_Normal) > Threshold)
                                scene.SceneState = KancolleSceneStates.Remodel_ItemList_Normal;
                            else
                                scene.SceneState = KancolleSceneStates.Remodel_ItemList_Other;
                            return scene;
                        }
                       
                    }
                    return scene;
                }

                if (SceneSimilarity(screenBitmap, Rect_Quest, Quest) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Quest;
                    return scene;
                }
            }
            else
            {//出击模式下
                //罗盘娘
                if (SceneSimilarity(screenBitmap, Rect_Battle_Compass, Battle_Compass) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Battle_Compass;
                    return scene;
                }

                //进击
                if (SceneSimilarity(screenBitmap, Rect_Battle_LeftChoice, Battle_Next)>Threshold &&
                    SceneSimilarity(screenBitmap, Rect_Battle_RightChoice, Battle_Return) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Battle_NextChoice;
                    return scene;
                }

                //夜战
                if (SceneSimilarity(screenBitmap, Rect_Battle_LeftChoice, Battle_Back) > Threshold &&
                   SceneSimilarity(screenBitmap, Rect_Battle_RightChoice, Battle_Night) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Battle_NightChoice;
                    return scene;
                }

                if (SceneSimilarity(screenBitmap, Rect_Battle_Formation, Battle_Formation) > Threshold)
                {
                    scene.SceneType = KancolleSceneTypes.Battle_Formation;
                    return scene;
                }



            }

            return scene;
        }


        private double SceneSimilarity(Bitmap screenBitmap, Rectangle matchRect, Bitmap matchTemplate)
        {
            Bitmap cutImag = new Bitmap((int)matchRect.Width, (int)matchRect.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(cutImag))
            {
                g.DrawImage(screenBitmap, 0, 0, matchRect, GraphicsUnit.Pixel);
            }
            
            var results = matching.ProcessImage(cutImag, matchTemplate);
            if (results.Length == 0)
                return 0;
            double sim = results[0].Similarity;
            for (int i = 1; i < results.Length; i++)
            {
                sim = Math.Max(sim, results[i].Similarity);
            }
            return sim;
        }

    }

}
