using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleMap;
using WoFlagship.ViewModels;

namespace WoFlagship.ToolWindows
{
    /// <summary>
    /// MapEditor.xaml 的交互逻辑
    /// </summary>
    public partial class MapEditor : Window
    {
        List<MapInfoItem> mapItems = new List<MapInfoItem>();
        MapEditorViewModel model = new MapEditorViewModel();


        public MapEditor()
        {
            InitializeComponent();
            Lb_Maps.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source=model, Path = new PropertyPath("MapInfos") });
            Lb_Maps.SetBinding(ListBox.SelectedItemProperty, new Binding() { Source=model, Path = new PropertyPath("SelectedMap") });
            Lb_Routes.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.Routes") });
            Lb_Routes.SetBinding(ListBox.SelectedItemProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.SelectedRoute") });
            Lb_Conditions.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.SelectedRoute.Conditions") });
            Lb_Conditions.SetBinding(ListBox.SelectedItemProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.SelectedRoute.SelectedCondition") });
            Lb_Ships.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.SelectedRoute.SelectedCondition.Constraints") });
            Lb_Ships.SetBinding(ListBox.SelectedItemProperty, new Binding() { Source = model, Path = new PropertyPath("SelectedMap.SelectedRoute.SelectedCondition.SelectedConstraint") });

        }

        private void Btn_LoadFromPOIFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                FileInfo fi = new FileInfo(ofd.FileName);
                string content = "";
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    content = sr.ReadToEnd();
                    UpdateMap(content);
                    model.MapInfos.Clear();
                    foreach (var itm in mapItems)
                    {
                        model.MapInfos.Add(new MapInfoItemViewModel(itm));
                    }
                }
            }
        }

        private void Btn_LoadFromJsonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                FileInfo fi = new FileInfo(ofd.FileName);
                string content = "";
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    content = sr.ReadToEnd();
                    mapItems = new List<MapInfoItem>(JsonConvert.DeserializeObject<KancolleMapInfo>(content).MapInfos);
                    model.MapInfos.Clear();
                    foreach (var itm in mapItems)
                    {
                        model.MapInfos.Add(new MapInfoItemViewModel(itm));
                    }

                }
            }
        }

        private void UpdateMap(string content)
        {
            JObject root = JsonConvert.DeserializeObject(content) as JObject;
            CreateMapItems(root["data"]);
        }

        private void CreateMapItems(JToken data)
        {
            mapItems.Clear();
            foreach(var map in data)
            {
                MapInfoItem item = new MapInfoItem();
                string mapStr = (map as JProperty).Name;
                item.Map = mapStr;
                string[] ms = mapStr.Trim().Split(new char[] { '-'});
                int h = int.Parse(ms[0]);
                int l = int.Parse(ms[1]);
                item.MapId = h * 10 + l;
                item.Routes = getRoutes((map as JProperty).Value["route"]);
                item.Spots = getSpots((map as JProperty).Value["spots"]);
                mapItems.Add(item);

            }
        }

        private MapRoute[] getRoutes(JToken route)
        {
            List<MapRoute> mrs = new List<MapRoute>();
            foreach(var r in route)
            {
               
                string[] spots = (r as JProperty).Value.ToObject<string[]>();
                if(spots[0] != null && spots[1]!= null)
                {
                    MapRoute mr = new MapRoute();
                    mr.From = spots[0];
                    mr.To = spots[1];
                    mrs.Add(mr);
                }
            }

            return mrs.ToArray();
        }

        private Spot[] getSpots(JToken spots)
        {
            List<Spot> spList = new List<Spot>();
            foreach(var spot in spots)
            {
                Spot s = new Spot();
                s.Id = (spot as JProperty).Name;
                JArray value = (spot as JProperty).Value as JArray;
                s.X = value[0].ToObject<int>() / 20;
                s.Y = value[1].ToObject<int>() / 18;
                spList.Add(s);
;            }
            return spList.ToArray();
        }

        private void Btn_SaveToJson_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json文件(*.json)|*.json";
            if (sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                SaveMapListToJson(fileName);
            }
        }

        private void SaveMapListToJson(string jsonFile)
        {
            using (StreamWriter sw = File.CreateText(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                KancolleMapInfo qi = new KancolleMapInfo()
                {
                    Version = 1,
                    UpdateTime = DateTime.Now.ToString("yyyyMMdd"),
                    MapInfos = mapItems.ToArray(),
                };
                serializer.Serialize(sw, qi);
            }
        }




        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            cb.ItemsSource = KancolleAPIs.ShipTypeDic.Keys.ToArray();
        }

        private void Btn_AddCondition_Click(object sender, RoutedEventArgs e)
        {
            model.SelectedMap?.SelectedRoute?.Conditions.Add(new ShipConditionViewModel());
        }

        private void Btn_DeleteCondition_Click(object sender, RoutedEventArgs e)
        {
            if(model.SelectedMap?.SelectedRoute?.SelectedCondition != null)
                model.SelectedMap?.SelectedRoute?.Conditions.Remove(model.SelectedMap?.SelectedRoute?.SelectedCondition);
        }

        private void Btn_AddShipRequirement_Click(object sender, RoutedEventArgs e)
        {
            model.SelectedMap?.SelectedRoute?.SelectedCondition?.Constraints.Add(new ShipConstraintViewModel());
        }

        private void Btn_DeleteShip_Click(object sender, RoutedEventArgs e)
        {
            if (model.SelectedMap?.SelectedRoute?.SelectedCondition?.SelectedConstraint!= null)
                model.SelectedMap?.SelectedRoute?.SelectedCondition?.Constraints.Remove(model.SelectedMap?.SelectedRoute?.SelectedCondition?.SelectedConstraint);

        }


    }

    
}
