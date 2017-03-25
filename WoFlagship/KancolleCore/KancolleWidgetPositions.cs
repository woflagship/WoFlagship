using System.Windows;

namespace WoFlagship.KancolleCore
{
    public class KancolleWidgetPositions
    {
        public const int Version = 1;
        public const string UpdateTime = "20161219";

        public readonly static Point Port = new Point(40, 40);

        public readonly static Point Port_Organize = new Point(195,135);
        public readonly static Point Port_Supply = new Point(75, 220);
        public readonly static Point Port_Remodel = new Point(315, 220);
        public readonly static Point Port_Repair = new Point(120, 360);
        public readonly static Point Port_Arsenal = new Point(275, 360);
        public readonly static Point Port_Sally = new Point(200, 260);

        public readonly static Point Left_Organize = new Point(20,150);
        public readonly static Point Left_Supply = new Point(20, 200);
        public readonly static Point Left_Remodel = new Point(20,260);
        public readonly static Point Left_Repair = new Point(20, 310);
        public readonly static Point Left_Arsenal = new Point(20, 360);

        //编成
        public readonly static Point[] Organize_Decks = new Point[] { new Point(135, 115), new Point(165, 115), new Point(195, 115), new Point(225, 115) };
        public readonly static Point Organize_RemoveAllExceptFirst = new Point(415, 120);
        public readonly static Point[] Organize_DetailButtons = new Point[] { new Point(325, 215), new Point(660, 215), new Point(325, 330), new Point(660, 330), new Point(325, 445), new Point(660, 445) };
        public readonly static Point[] Organize_ChangeButtons = new Point[] { new Point(405, 215), new Point(740, 215), new Point(405, 330), new Point(740, 330), new Point(405, 445), new Point(740, 445) };
        public readonly static Point Organize_Changes_Remove = new Point(500, 145);
        public readonly static Point[] Organize_Changes_ShipList = new Point[] { new Point(500, 170), new Point(500, 195), new Point(500, 225), new Point(500, 250), new Point(500, 280), new Point(500, 310), new Point(500, 335), new Point(500, 365), new Point(500, 390), new Point(500, 420) };
        public readonly static Point Organize_Changes_FirstPage = new Point(430, 450);
        public readonly static Point Organize_Changes_Next5Page = new Point(675, 450);
        public readonly static Point[] Organize_Changes_Pages = new Point[] { new Point(520, 450), new Point(550, 450), new Point(580, 450), new Point(610, 450), new Point(640, 450), };
        public readonly static Point Organize_Change_Decide = new Point(685, 440);
        public readonly static Point Organize_SortType = new Point(775, 110);
        public readonly static Point Organize_ShipList_Back = new Point(280, 280);
        public readonly static Point Organize_Change_Decide_Back = new Point(280, 280);

        //补给
        public readonly static Point Supply_SupplyDeck = new Point(120,120);
        public readonly static Point[] Supply_Decks = new Point[] { new Point(150, 115), new Point(180, 115), new Point(210, 115), new Point(240, 115) };

        //改装
        public readonly static Point[] Remodel_Decks = new Point[] { new Point(145, 115), new Point(175, 115), new Point(205, 115), new Point(235, 115) };
        public readonly static Point[] Remodel_Ships = new Point[] { new Point(210, 165), new Point(210, 220), new Point(210, 275), new Point(210, 330), new Point(210, 385), new Point(210, 440) };
        public readonly static Point[] Remodel_Items = new Point[] { new Point(430, 175), new Point(430, 210), new Point(430, 245), new Point(430, 289) };
        public readonly static Point Remodel_Items_Return = new Point(200, 260);
        public readonly static Point Remodel_ChangeItemMode = new Point(750,110);
        public readonly static Point[] Remodel_Changes_ItemList = new Point[] { new Point(570, 150), new Point(570, 180), new Point(570, 210), new Point(570, 240), new Point(570, 270), new Point(570, 300), new Point(570, 330), new Point(570, 360), new Point(570, 390), new Point(570, 420) };
        public readonly static Point Remodel_Changes_FirstPage = new Point(410, 450);
        public readonly static Point Remodel_Changes_Next5Page = new Point(650, 450);
        public readonly static Point[] Remodel_Changes_Pages = new Point[] { new Point(490, 445), new Point(520, 445), new Point(550, 445), new Point(580, 445), new Point(610, 445), };
        public readonly static Point Remodel_Change_Decide = new Point(700, 440);
        public readonly static Point Remodel_Change_Other_Decide = new Point(315, 320);
        public readonly static Point[] Remodel_RemoveItem = new Point[] { new Point(545, 175), new Point(545, 210) , new Point(545, 245) , new Point(545, 280) };

        //入渠
        public readonly static Point[] Repair_Docks = new Point[] { new Point(240, 160), new Point(240, 240), new Point(240, 320), new Point(240, 400) };
        public readonly static Point[] Repair_FastRepairs = new Point[] { new Point(750, 160), new Point(750, 240), new Point(750, 320), new Point(750, 400) };
        public readonly static Point Repair_ShipList_SortMode = new Point(770, 110);
        public readonly static Point[] Repair_ShipList = new Point[] { new Point(570, 150), new Point(570, 180), new Point(570, 210), new Point(570, 240), new Point(570, 270), new Point(570, 300), new Point(570, 330), new Point(570, 360), new Point(570, 390), new Point(570, 420) };
        public readonly static Point Repair_Ships_FirstPage = new Point(430, 460);
        public readonly static Point Repair_Ships_Next5Page = new Point(680, 460);
        public readonly static Point[] Repair_Ships_Pages = new Point[] { new Point(510, 460), new Point(545, 460), new Point(580, 460), new Point(615, 460), new Point(650, 460)};
        public readonly static Point Repair_Start = new Point(680,445);
        public readonly static Point Repair_Ships_Return = new Point(200, 260);
        public readonly static Point Repair_Start_Decide = new Point(500, 400);

        public readonly static Point Sally_Map = new Point(220, 220);
        public readonly static Point Sally_Practice = new Point(450, 220);
        public readonly static Point Sally_Mission = new Point(680, 220);
        //各海域地图位置
        public readonly static Point[] Map_Maps = new Point[] { new Point(275, 200), new Point(610, 200), new Point(275, 350), new Point(610, 350) };
        //海域位置（通常海域）
        public readonly static Point[] Map_Areas = new Point[] { new Point(150, 440), new Point(230, 440), new Point(310, 440), new Point(380, 440), new Point(460, 440), new Point(540, 440) };
        public readonly static Point Map_Practice = new Point(655,120);
        public readonly static Point Map_Mission = new Point(740,120);
        public readonly static Point Map_Decide = new Point(685, 440);
        public readonly static Point[] Map_SelectDeck = new Point[] { new Point(360, 115), new Point(390, 115), new Point(420, 115), new Point(450, 115) };
        public readonly static Point Map_Start = new Point(630, 445);
        public readonly static Point Map_Decide_Back = new Point(190, 270);
        public readonly static Point Map_Start_Back = new Point(190, 270);

        public readonly static Point Practice_Map = new Point(580, 125);
        public readonly static Point Practice_Mission = new Point(750,125);
        //演习对手
        public readonly static Point[] Pratice_Pratices = new Point[] { new Point(330, 200), new Point(330, 260), new Point(330, 310), new Point(330, 370), new Point(330, 430) };

        //远征
        public readonly static Point Mission_Map = new Point(350, 110);
        public readonly static Point Mission_Practice = new Point(430, 110);
        public readonly static Point[] Mission_Maps = new Point[] { new Point(130, 430), new Point(190, 430), new Point(250, 430), new Point(310, 430), new Point(360, 430) };
        public readonly static Point[] Mission_MissionItems = new Point[] { new Point(250, 170), new Point(250, 200), new Point(250, 230), new Point(250, 260), new Point(250, 290), new Point(250, 320), new Point(250, 350), new Point(250, 380) };
        public readonly static Point Mission_Decide = new Point(675, 440);
        public readonly static Point Mission_Start_Back = new Point(190, 280);
        public readonly static Point[] Mission_Start_Decks = new Point[] { new Point(365, 175), new Point(395, 175), new Point(425, 175), new Point(455, 175) };
        public readonly static Point Mission_Start = new Point(660,450);

        //出击
        public readonly static Point Battle_LeftChoice = new Point(295, 235);
        public readonly static Point Battle_RightChoice = new Point(505, 235);
        public readonly static Point[] Battle_Formation = new Point[] { new Point(450,185), new Point(580,185), new Point(710,185), new Point(530,345), new Point(650, 345)};
        public readonly static Rect Battle_SafeRect = new Rect(650, 330, 100, 100);


        public readonly static Point Quest = new Point(555, 50);

      

    }
}
