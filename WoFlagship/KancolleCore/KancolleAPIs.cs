using System;
using System.Collections.Generic;
using WoFlagship.Properties;

namespace WoFlagship.KancolleCore
{
    /// <summary>
    /// 舰娘游戏中的API信息
    /// </summary>
    public class KancolleAPIs
    {
        /// <summary>
        /// Kancolle API 版本
        /// </summary>
        public const int Version = 1;

        /// <summary>
        /// Kancolle API 更新日期
        /// </summary>
        public const string UpdateTime = "20161212";

        /// <summary>
        /// 获得URL当中的API信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAPI(string url)
        {
            int index = url.IndexOf(DMMUrls.KanColleAPIKeyword);
            if(index > 0)
            {
                return url.Substring(index + DMMUrls.KanColleAPIKeyword.Length);
            }
            return "";
        }

        /// <summary>
        /// 头衔
        /// </summary>
        public static string[] RankText { get; } = { "", "元帥", "大将", "中将", "少将", "大佐", "中佐", "新米中佐", "少佐", "中堅少佐", "新米少佐" };

        /// <summary>
        /// 舰娘种类
        /// </summary>
        public static string[] ShipTypeText { get; } = { Resources.unkown, Resources.dd, Resources.cl, Resources.clt, Resources.ca, Resources.cav, Resources.cvl, Resources.fbb, Resources.bb, Resources.bbv,
            Resources.cv, Resources.unkown, Resources.ss, Resources.ssv, Resources.av_e, Resources.av, Resources.ls, Resources.acv, Resources.ws, Resources.ssm, Resources.pcl, Resources.sp};

        /// <summary>
        /// 舰娘种类编号
        /// </summary>
        public static Dictionary<string, int> ShipTypeDic { get; } = new Dictionary<string, int>()
        {
            { "艦",0}, { "他の艦",0}, { "駆逐",1}, { "軽巡",2}, {"重雷装巡洋舰", 3 }, { "重巡",4}, { "航巡",5}, { "轻母",6 }, { "軽母", 6}, {"高速战舰",7 },  {"高速战艦",7 }, { "高速艦", 7},{"戦艦",8 }, {"航戦",9 }, { "空母",10}, { "轻母/空母", 10}, {"11(未知)",11 },
            { "潜水艦",12}, {"潜水空母",13 }, {"水上机母舰",14 }, {"水母",15 }, { "扬陆舰",16}, { "装甲空母",17}, { "工作舰",18}, {"潜水母艦",19 }, { "练习巡洋舰",20}, { "补给舰" ,21 },
            {"大和型", 100 }, {"長門型",101 }, {"伊勢型",102 }, {"扶桑型",103 }
        };


    }


    #region api structure

    internal class get_incentive
    {
        public int api_result { get; set; }
        public string api_result_mgs { get; set; }
        public dynamic api_data { get; set; }
    }

    #endregion


    internal class svdata
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
        public object api_data { get; set; }
        //下面属性只有在api_req_hokyu/charge中有用
        public api_ship_item[] api_ship { get; set; }
        public int[] api_material { get; set; }
        public int api_use_bou { get; set; }

    }

    #region api_start2


    internal class api_start_data
    {
        public api_mst_ship_item[] api_mst_ship { get; set; }
        public api_mst_shipgraph_item[] api_mst_shipgraph { get; set; }
        public api_mst_slotitem_equiptype_item[] api_mst_slotitem_equiptype { get; set; }
        public int[] api_mst_equip_exslot { get; set; }
        public api_mst_stype_item[] api_mst_stype { get; set; }
        public api_mst_slotitem_item[] api_mst_slotitem { get; set; }
        //api_mst_furniture
        //api_mst_furnituregraph
        //useitem
        //payitem
        //item_shtop
        public api_mst_maparea_item[] api_mst_maparea { get; set; }
        public api_mst_mapinfo_item[] api_mst_mapinfo { get; set; }
        //mapbgm
        public api_mst_mission_item[] api_mst_mission { get; set; }
        //const
        public api_mst_shipupgrade_item[] api_mst_shipupgrade { get; set; }
        //bgm
    }

    internal class api_mst_ship_item
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public string api_yomi { get; set; }
        public int api_stype { get; set; }
        public int api_afterlv { get; set; }
        public string api_aftershippid { get; set; }
        public int[] api_taik { get; set; }
        public int[] api_souk { get; set; }
        public int[] api_houg { get; set; }
        public int[] api_raig { get; set; }
        public int[] api_tyku { get; set; }
        public int[] api_luck { get; set; }
        public int api_soku { get; set; }
        public int api_leng { get; set; }
        public int api_slot_num { get; set; }
        public int[] api_maxeq { get; set; }
        public int api_buildtime { get; set; }
        public int[] api_broken { get; set; }
        public int[] api_powup { get; set; }
        public int api_backs { get; set; }
        public string api_getmes { get; set; }
        public int api_afterfuel { get; set; }
        public int api_afterbull { get; set; }
        public int api_fuel_max { get; set; }
        public int api_bull_max { get; set; }
        public int api_voice { get; set; }
    }



    internal class api_mst_shipgraph_item
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_filename { get; set; }
        public string[] api_version { get; set; }
        public int[] api_boko_n { get; set; }
        public int[] api_boko_d { get; set; }
        public int[] api_kaisyu_n { get; set; }
        public int[] api_kaisyu_d { get; set; }
        public int[] api_kaizo_n { get; set; }
        public int[] api_kaizo_d { get; set; }
        public int[] api_map_n { get; set; }
        public int[] api_map_d { get; set; }
        public int[] api_ensyuf_n { get; set; }
        public int[] api_ensyuf_d { get; set; }
        public int[] pi_ensyue_n { get; set; }
        public int[] api_battle_n { get; set; }
        public int[] api_battle_d { get; set; }
        public int[] api_weda { get; set; }
        public int[] api_wedb { get; set; }
    }

    internal class api_mst_slotitem_equiptype_item
    {
        public int api_id { get; set; }
        public string api_name { get; set; }
        public int api_show_flg { get; set; }
    }

    internal class api_mst_stype_item
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public int api_scnt { get; set; }
        public int api_kcnt { get; set; }
        public Dictionary<string, int> api_equip_type { get; set; }
    }

    internal class api_mst_slotitem_item
    {
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        public string api_name { get; set; }
        public int[] api_type { get; set; }
        public int api_taik { get; set; }
        public int api_souk { get; set; }
        public int api_houg { get; set; }
        public int api_raig { get; set; }
        public int api_soku { get; set; }
        public int api_baku { get; set; }
        public int api_tyku { get; set; }
        public int api_tais { get; set; }
        public int api_atap { get; set; }
        public int api_houm { get; set; }
        public int api_raim { get; set; }
        public int api_houk { get; set; }
        public int api_raik { get; set; }
        public int api_bakk { get; set; }
        public int api_saku { get; set; }
        public int api_sakb { get; set; }
        public int api_luck { get; set; }
        public int api_leng { get; set; }
        public int api_rare { get; set; }
        public int[] api_broken { get; set; }
        public string api_info { get; set; }
        public string api_usebull { get; set; }
    }

    internal class api_mst_maparea_item
    {
        public int api_id { get; set; }
        public string api_name { get; set; }
        public int api_type { get; set; }
    }

    internal class api_mst_mapinfo_item
    {
        public int api_id { get; set; }
        public int api_maparea_id { get; set; }
        public int api_no { get; set; }
        public string api_name { get; set; }
        public int api_level { get; set; }
        public string api_infotext { get; set; }
        public int[] api_item { get; set; }
        public Nullable<int> api_max_maphp { get; set; }
        public Nullable<int> api_required_defeat_count { get; set; }
        public int[] api_sally_flat { get; set; }
    }

    internal class api_mst_mission_item
    {
        public int api_id { get; set; }
        public int api_maparea_id { get; set; }
        public string api_name { get; set; }
        public string api_details { get; set; }
        public int api_time { get; set; }
        public int api_difficulty { get; set; }
        public double api_use_fuel { get; set; }
        public double api_use_bull { get; set; }
        public int[] api_win_item1 { get; set; }
        public int[] api_win_item2 { get; set; }
        public int api_return_flag { get; set; }
    }

    internal class api_mst_shipupgrade_item
    {
        public int api_id { get; set; }
        public int api_current_ship_id { get; set; }
        public int api_original_ship_id { get; set; }
        public int api_upgrade_type { get; set; }
        public int api_upgrade_level { get; set; }
        public int api_drawing_count { get; set; }
        public int api_catapult_count { get; set; }
        public int api_sortno { get; set; }
    }
    #endregion

    #region api_port/port


    internal class api_port_data
    {
        public api_material_item[] api_material { get; set; }
        public api_deck_port_item[] api_deck_port { get; set; }
        //都是4个元素，没开的渠state为-1，开了的为0
        public api_ndock_item[] api_ndock { get; set; }
        public api_ship_item[] api_ship { get; set; }
        public api_basic api_basic { get; set; }
        public api_log_item[] api_log { get; set; }
        public int api_p_bgm_id { get; set; }
        public int api_parallel_quest_count { get; set; }
    }



    internal class api_deck_port_item
    {
        public int api_member_id { get; set; }
        public int api_id { get; set; }
        public string api_name { get; set; }
        public string api_name_id { get; set; }
        public long[] api_mission { get; set; }
        public string api_flagship { get; set; }
        public int[] api_ship { get; set; }
    }

    /// <summary>
    /// 入渠情况
    /// </summary>
    internal class api_ndock_item
    {
        public int api_member_id { get; set; }
        public int api_id { get; set; }
        /// <summary>
        /// -1:未拥有；0：已拥有，且空着；1：已拥有，但是被占用
        /// </summary>
        public int api_state { get; set; }
        public int api_ship_id { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public long api_complete_time { get; set; }
        /// <summary>
        /// 日本时间
        /// </summary>
        public string api_complete_time_str { get; set; }
        public int api_item1 { get; set; }
        public int api_item2 { get; set; }
        public int api_item3 { get; set; }
        public int api_item4 { get; set; }
    }



    internal class api_basic
    {
        public string api_member_id { get; set; }
        public string api_nickname { get; set; }
        public string api_nickname_id { get; set; }
        public int api_active_flag { get; set; }
        public long api_starttime { get; set; }
        public int api_level { get; set; }
        public int api_rank { get; set; }
        public int api_experience { get; set; }
        public string api_fleetname { get; set; }
        public string api_comment { get; set; }
        public string api_comment_id { get; set; }
        public int api_max_chara { get; set; }
        public int api_max_slotitem { get; set; }
        public int api_max_kagu { get; set; }
        public int api_playtime { get; set; }
        public int api_tutorial { get; set; }
        public int[] api_furniture { get; set; }
        public int api_count_deck { get; set; }
        //工厂数
        public int api_count_kdock { get; set; }
        //入渠数
        public int api_count_ndock { get; set; }
        public int api_fcoin { get; set; }
        public int api_st_win { get; set; }
        public int api_st_lose { get; set; }
        public int api_ms_count { get; set; }
        public int api_ms_success { get; set; }
        public int api_pt_win { get; set; }
        public int api_pt_lose { get; set; }
        public int api_pt_challenged { get; set; }
        public int api_pt_challenged_win { get; set; }
        public int api_firstflag { get; set; }
        public int api_tutorial_progress { get; set; }
        public int[] api_pvp { get; set; }
        public int api_medals { get; set; }
        public int api_large_dock { get; set; }
    }

    internal class api_log_item
    {
        public int api_no { get; set; }
        public string api_type { get; set; }
        public string api_state { get; set; }
        public string api_message { get; set; }
    }


    #endregion

    #region api_get_member/material

    internal class api_material_item
    {
        public int api_member_id { get; set; }
        public int api_id { get; set; }
        public int api_value { get; set; }
    }

    #endregion

    #region api_req_hokyu/charge
    //在svdata中
    #endregion

    #region api_get_member/require_info

    internal class api_requireinfo_data
    {
        public api_basic api_basic { get; set; }
        public api_slot_item_item[] api_slot_item { get; set; }
        //unsetslot
        public api_kdock_item[] api_kdock { get; set; }
        //furniture
    }

    #endregion

    #region api_req_kousyou/createitem

    internal class api_createitem_data
    {
        public int api_create_flag { get; set; }
        public int api_shizai_flag { get; set; }
        public api_slot_item_item api_slot_item { get; set; }
        public int[] api_material { get; set; }
        public string api_fdata { get; set; }
        public int api_type3 { get; set; }
        public int[] api_unsetslot { get; set; }
    }

    #endregion

    #region api_req_kousyou/destroyitem2
    namespace destroyitem2_api
    {
        internal class svdata
        {
            public int api_result { get; set; }
            public string api_result_msg { get; set; }
            public api_data api_data { get; set; }
        }

        internal class api_data
        {
            public int[] api_get_material { get; set; }
        }
    }
    #endregion

    #region api_get_member/questlist


    internal class api_questlist_data
    {
        public int api_count { get; set; }
        public int api_completed_kind { get; set; }
        public int api_page_count { get; set; }
        public int api_disp_page { get; set; }
        //由于任务界面可能出现不满5个情况，而不满的数据是-1，而不是空对象，所以只能用object表示，再读取的时候再做判断
        public object[] api_list { get; set; }
        public int api_exec_count { get; set; }
        public int api_exec_type { get; set; }
    }

    [Serializable]
    internal class api_questlist_item
    {
        public api_questlist_item() { }
        // public api_questlist_item(int i) { }
        public int api_no { get; set; }
        public int api_category { get; set; }
        public int api_type { get; set; }
        //state=3表示完成，state=2表示任务接受 state=1表示未接收
        public int api_state { get; set; }
        public string api_title { get; set; }
        public string api_detail { get; set; }
        public int[] api_get_material { get; set; }
        public int api_bonus_flag { get; set; }
        //0-0% 1-50% 2-80%
        public int api_progress_flag { get; set; }
        public int api_invalid_flag { get; set; }
    }

    #endregion

    #region api_get_member/ship3
    internal class api_ship3_data
    {
        public api_ship_item[] api_ship_data { get; set; }
        public api_deck_item[] api_deck_data { get; set; }
        //public api_slot_item_item api_slot_data { get; set; }
    }

    #endregion

    #region api_req_kaisou/slot_deprive
    internal class api_slot_deprive_data
    {
        public api_slot_deprive_ship_data api_ship_data { get; set; }
        public api_slot_deprive_unset_list api_unset_list { get; set; }
    }

    internal class api_slot_deprive_ship_data
    {
        public api_ship_item api_unset_ship { get; set; }
        public api_ship_item api_set_ship { get; set; }
    }

    internal class api_slot_deprive_unset_list {
        public int api_type3No { get; set; }
        public int[] api_slot_list { get; set; }
    }

    #endregion

    internal class api_ship_item
    {
        /// <summary>
        /// ownedShip id，按照获得的先后顺序排序，后获得的id大
        /// </summary>
        public int api_id { get; set; }
        public int api_sortno { get; set; }
        /// <summary>
        /// 数据库中船的id
        /// </summary>
        public int api_ship_id { get; set; }
        public int api_lv { get; set; }
        public int[] api_exp { get; set; }
        public int api_nowhp { get; set; }
        public int api_maxhp { get; set; }
        public int api_leng { get; set; }
        public int[] api_slot { get; set; }
        public int[] api_onslot { get; set; }
        public int api_slot_ex { get; set; }
        /// <summary>
        /// 已强化数值
        /// </summary>
        public int[] api_kyouka { get; set; }
        public int api_backs { get; set; }
        /// <summary>
        /// 燃料量
        /// </summary>
        public int api_fuel { get; set; }
        /// <summary>
        /// 弹药量
        /// </summary>
        public int api_bull { get; set; }
        /// <summary>
        /// 可用装备栏位
        /// </summary>
        public int api_slotnum { get; set; }
        public int api_ndock_time { get; set; }
        public int[] api_ndock_item { get; set; }
        public int api_srate { get; set; }
        /// <summary>
        /// 疲劳值
        /// </summary>
        public int api_cond { get; set; }
        /// <summary>
        /// 火力
        /// </summary>
        public int[] api_karyoku { get; set; }
        /// <summary>
        /// 雷装
        /// </summary>
        public int[] api_raisou { get; set; }
        /// <summary>
        /// 对空
        /// </summary>
        public int[] api_taiku { get; set; }
        /// <summary>
        /// 装甲
        /// </summary>
        public int[] api_soukou { get; set; }
        /// <summary>
        /// 回避
        /// </summary>
        public int[] api_kaihi { get; set; }
        /// <summary>
        /// 对潜
        /// </summary>
        public int[] api_taisen { get; set; }
        /// <summary>
        /// 索敌
        /// </summary>
        public int[] api_sakuteki { get; set; }
        /// <summary>
        /// 运
        /// </summary>
        public int[] api_lucky { get; set; }
        public int api_locked { get; set; }
        public int api_locked_equip { get; set; }

    }


    internal class api_slot_item_item
    {
        public int api_id { get; set; }
        public int api_slotitem_id { get; set; }
        public int api_locked { get; set; }
        public int api_level { get; set; }
    }

    internal class api_kdock_item
    {
        public int api_id { get; set; }
        public int api_state { get; set; }
        public int api_created_ship_id { get; set; }
        public long api_complete_time { get; set; }
        public string api_complete_time_str { get; set; }
        public int api_item1 { get; set; }
        public int api_item2 { get; set; }
        public int api_item3 { get; set; }
        public int api_item4 { get; set; }
        public int api_item5 { get; set; }
    }

    #region api_get_member/mission
    internal class api_mission_item
    {
        public int api_mission_id { get; set; }
        public int api_state { get; set; }
    }
    #endregion

    #region api_req_map/start
    //同api_mapnext_data
    #endregion

    #region api_req_map/next
    internal class api_mapnext_data
    {
        public object api_cell_data { get; set; }
        public int api_rashin_flg { get; set; }
        public int api_rashin_id { get; set; }
        public int api_maparea_id { get; set; }
        public int api_mapinfo_no { get; set; }
        public int api_no { get; set; }
        public int api_color_no { get; set; }
        public int api_event_id { get; set; }
		public int api_event_kind{ get; set; }
		public int api_next { get; set; }
        public int api_bosscell_no { get; set; }
		public int api_bosscomp{ get; set; }
		public int api_comment_kind { get; set; }
        public int api_production_kind { get; set; }
        public object api_airsearch { get; set; }
        public int api_from_no { get; set; }
    }
    #endregion

    #region api_req_sortie/battle
    internal class api_battle_data
    {
        //出击的编队号
        public int api_dock_id { get; set; }

        //敌舰队的舰船id，第0位不使用,数字和字符串都有可能是里面的元素
        public object[] api_ship_ke { get; set; }
        public object[] api_ship_ke_combined { get; set; }
        //敌舰队等级，第0位不使用
        public int[] api_ship_lv { get; set; }

        //第0位不用，1-6友，7-12敌
        public int[] api_nowhps { get; set; }
        public int[] api_maxhps { get; set; }

        //是否夜战，0-否，1-是，将会出现夜战分歧选择
        public int api_midnight_flag { get; set; }

        //敌方舰队的插槽装备名单
        public int[][] api_eSlot { get; set; }
        //敌方舰队的强化状况清单
        public int[,] api_eKyouka { get; set; }
        //自艦队的战斗参数列表排列
        public int[,] api_fParam { get; set; }
        //敌艦队的战斗参数列表排列
        public int[,] api_eParam { get; set; }
        //索敵結果排列
        public int[] api_search { get; set; }
        //舰队阵型
        public int[] api_formation { get; set; }
        //开幕式航空战的序列信息,1表示有，0表示无
        public int[] api_stage_flag { get; set; }
        //航空站信息
        public api_kouku api_kouku { get; set; }
        //支援攻击有无的标志，1支援，0
        public int api_support_flag { get; set; }
        //支援舰队的攻击序列数据。内容不明，但炮击战阶段，航空战阶段，雷击战阶段，有可能。支援舰队编成。
        public dynamic api_support_info { get; set; }
        
        public int api_opening_taisen_flag { get; set; }
        public dynamic api_opening_taisen { get; set; }
        //开幕式攻击无标志，1下就进行了开幕，0，没有开幕攻击。
        public int api_opening_flag { get; set; }
        //开幕雷击的顺序信息,只有一个t
        public dynamic api_opening_atack { get; set; }
        //炮雷击战的顺序信息0-3
        //[0]第一顺序炮击 [1]第二顺序炮击 [2]第三顺序炮击（也许未实装）[3]雷击的发生有无表示
        public int[] api_hourai_flag { get; set; }

        public api_hougeki api_hougeki1 { get; set; }
        public api_hougeki api_hougeki2 { get; set; }
        public api_hougeki api_hougeki3 { get; set; }
        public api_raigeki api_raigeki { get; set; }
        public int[][] api_eSlot_combined { get; set; }
        public int[] api_maxhps_combined { get; set; }
        public int[] api_nowhps_combined { get; set; }
        public int[] api_ship_lv_combined { get; set; }
        public api_kouku[] api_air_base_attack { get; set; }
        public api_kouku api_kouku2 { get; set; }
        public api_hougeki api_hougeki { get; set; }
        public int[] api_active_deck { get; internal set; }
    }

    internal class api_kouku
    {
        //自军，敌军的舰载机発艦舰艇座次排列,[[n],[n]]的二维数组方式
        public int[][] api_plane_from { get; set; }
        //制空权关联的参加舰载机和击落状况
        public api_plane_stage api_stage1 { get; set; }
        //航空雷击和航空轰炸的参加舰载机击落状况
        public api_plane_stage api_stage2 { get; set; }
        //伤害阶段
        public api_plane_stage3 api_stage3 { get; set; }

        public api_plane_stage3 api_stage3_combined { get; set; }
    }

    internal class api_plane_stage
    {
        public int api_f_cout { get; set; }
        public int api_f_lostcount { get; set; }
        public int api_e_count { get; set; }
        public int api_e_lostcount { get; set; }
        //制空权
        public int api_disp_seiku { get; set; }
        public int[] api_touch_plane { get; set; }
    }

    internal class api_plane_stage3
    {
        public int[] api_ebak_flag { get; set; }
        public int[] api_ecl_flag { get; set; }
        public double[] api_edam { get; set; }
        public int[] api_erai_flag { get; set; }
        public int[] api_fbak_flag { get; set; }
        public int[] api_fcl_flag { get; set; }
        public double[] api_fdam { get; set; }
        public int[] api_frai_flag { get; set; }
    }

    //炮击战
    internal class api_hougeki
    {
        //攻击顺序表，0不使用[1-12]敌方伙伴的攻击顺序排列在舰队序列号码上，双方都混在一起
        public int[] api_at_list { get; set; }
        public int[] api_at_type { get; set; }
        //以下4个都是这个形式，第一个元素为-1，其余均为数组，例如：[-1, [7], [4], [7], [7]],
        //被攻击顺序表，敌方伙伴的被攻击顺序排列在舰队序列号码上，双方都混在一起
        public dynamic[] api_df_list { get; set; }
        //攻击方军舰的使用武器名单，该攻击顺序的舰艇使用武器的攻击号码排列设定，-1为未使用或航空攻撃。
        public dynamic[] api_si_list { get; set; }
        //设置攻击顺序的临界的发生无
        public dynamic[] api_cl_list { get; set; }
        //设置攻击顺序发生的伤害数
        public dynamic[] api_damage { get; set; }

        public int[] api_at_eflag { get; set; }

        public int[] api_sp_list { get; set; }
    }

    //雷击战
    internal class api_raigeki
    {
        //第0位都不用！
        //自军的雷击对方座次排列，敌军只席次
        public int[] api_frai { get; set; }
        //敌军的雷击对方座次排列，自军只席次
        public int[] api_erai { get; set; }
        //我军受到的伤害
        public int[] api_fdam { get; set; }
        public int[] api_edam { get; set; }
        //自军不可？貌似是我军造成的伤害
        public double[] api_fydam { get; set; }
        //敌军造成的伤害？
        public double[] api_eydam { get; set; }
        //自军的临界产生有无
        public int[] api_fcl { get; set; }
        //敌军的临界产生有无
        public int[] api_ecl { get; set; }
    }
    #endregion

    #region api_req_sortie/battleresult
    internal class api_battleresult_data
    {
        public int[] api_ship_id { get; set; }
        public string api_win_rank { get; set; }
        public int api_get_exp { get; set; }
        public int api_mvp { get; set; }
        public int api_member_lv { get; set; }
        public int api_member_exp { get; set; }
        public int api_get_base_exp { get; set; }
        public int[] api_get_ship_exp { get; set; }
        public int[][] api_get_exp_lvup { get; set; }
        public int api_dests { get; set; }
        public int api_destsf { get; set; }
        public int[] api_lost_flag { get; set; }
        public string api_quest_name { get; set; }
        public int api_quest_level { get; set; }
        //api_enemy_info
        public int api_first_clear { get; set; }
        public int api_mapcell_incentive { get; set; }
        public int[] api_get_flag { get; set; }
        public api_get_ship api_get_ship { get; set; }
        public int api_get_eventflag { get; set; }
        public int api_get_exmap_rate { get; set; }
        public int api_get_exmap_useitem_id { get; set; }
    }

    internal class api_get_ship
    {
        public int api_ship_id { get; set; }
        public string api_ship_type { get; set; }
        public string api_ship_name { get; set; }
        public string api_ship_getmes { get; set; }
    }
    #endregion

    #region api_req_battle_midnight/battle
    //和api_battle_data一样

    #endregion

    #region api_get_member/ship_deck
    internal class api_shipdeck_data
    {
        public api_ship_item[] api_ship_data { get; set; }
        public api_deck_item[] api_deck_data { get; set; }
    }

    internal class api_deck_item
    {
        public int api_member_id { get; set; }
        public int api_id { get; set; }
        public string api_name { get; set; }
        public string api_name_id { get; set; }
        public int[] api_mission { get; set; }
        public string api_flagship { get; set; }
        public int[] api_ship { get; set; }
    }
    #endregion
}