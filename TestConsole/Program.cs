using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoFlagship.ToolWindows;
using WoFlagship.Wiki;
using WoFlagship.KancolleCore;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Algorithms.Observers;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AForge.Imaging;
using System.Drawing;
using WoFlagship.KancolleAI;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using SmartFormat;
using WoFlagship.KancolleQuestData;
using SmartFormat.Core.Extensions;
using WoFlagship.Utils;

namespace TestConsole
{
    class EditAction
    {
        /// <summary>
        /// 0-不动,1-修改，2-删除，没有添加
        /// </summary>
        public int EditType;

        public int Indexs;//位置

        public EditAction ea = null;

        public EditAction(int editype, int index)
        {
            this.EditType = editype;
            this.Indexs = index;
        }
    }

   

    class TestProvider : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Console.WriteLine("Fomat: " + format);
            return format;
        }

        public object GetFormat(Type formatType)
        {
            Console.WriteLine("get format " + formatType.Name);
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }
    }

    class KancolleFormatter : IFormatter
    {
        private string[] names = new[] { "Ship", "Result", "Group", "Select", "Amount" };
        public string[] Names { get { return names; } set { this.names = value; } }

        public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            if(formattingInfo.Placeholder.FormatterName == "Ship")
            {
                if(formattingInfo.CurrentValue is int[])
                {
                    var ships = formattingInfo.CurrentValue as int[];
                    if(ships.Length>0)
                    {
                        formattingInfo.Write(ships[0] + "");
                        for(int i=1; i<ships.Length; i++)
                        {
                            formattingInfo.Write("," + ships[i] );
                        }
                    }
                    else
                    {
                        formattingInfo.Write("任意船");
                    }
                }
                else if(formattingInfo.CurrentValue is int)
                {
                    var ships = (int)formattingInfo.CurrentValue;
                    formattingInfo.Write(ships+"");

                }
                else if (formattingInfo.CurrentValue is int?)
                {
                    var ships = formattingInfo.CurrentValue as int?;
                    formattingInfo.Write(ships + "");
                }
            }
            if(formattingInfo.Placeholder.FormatterName  == "Result")
            {
                if (formattingInfo.CurrentValue is string)
                {
                    if(formattingInfo.CurrentValue.ToString() == "クリア")
                    {
                        formattingInfo.Write("完成全图");
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "S")
                    {
                        formattingInfo.Write("获得S胜");
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "A")
                    {
                        formattingInfo.Write("获得A胜或以上");
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "B")
                    {
                        formattingInfo.Write("获得B胜或以上");
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "C")
                    {
                        formattingInfo.Write("获得C败或以上");
                    }
                    else
                    {
                        Console.WriteLine("未知Result！" + formattingInfo.CurrentValue);
                        return false; 
                    }

                    return true;
                }
            }
            else if(formattingInfo.Placeholder.FormatterName == "Group")
            {
                if(formattingInfo.CurrentValue is ShipRequirement[])
                {
                    var requires = formattingInfo.CurrentValue as ShipRequirement[];
                    if(requires != null)
                    {
                        foreach (var req in requires)
                        {

                            string str = Smart.Format("{Ship:Ship()} {Select:Select()}", req);
                            formattingInfo.Write(str);

                        }
                    }
                    else
                    {
                        formattingInfo.Write("无要求");
                    }
                    return true;
                }
            }
            else if (formattingInfo.Placeholder.FormatterName == "Amount")
            {
                if (formattingInfo.CurrentValue is int[])
                {
                    int[] amount  = formattingInfo.CurrentValue as int[];
                    if(amount.Length == 1)
                    {
                        formattingInfo.Write(amount[0] + "只");
                    }
                    else
                    {
                        formattingInfo.Write(amount[0]+"-"+amount[1]+ "只");
                    }
                    return true;
                }
            }
            else if (formattingInfo.Placeholder.FormatterName == "Select")
            {
                if (formattingInfo.CurrentValue is int[])
                {
                   
                    int[] select = formattingInfo.CurrentValue as int[];
                    if (select.Length == 1)
                    {
                        formattingInfo.Write("任意"+select[0] + "只");
                    }
                    else
                    {
                        formattingInfo.Write("任意" + select[0] + "-" + select[1] + "只");
                    }
                    return true;
                }
            }


            return false;
        }
    }

    class Program
    {
        static readonly int[,] dd = new int[3, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

        static void Main(string[] args)
        {

            ReadOnlyArray2<int> r = new ReadOnlyArray2<int>(dd);
            var rr = r.ToArray();
            foreach(var ar in r)
            {
                Console.WriteLine(ar);
 
            }
            foreach (var ar in rr)
            {
                Console.WriteLine(ar);
            }
            Console.Read();
        }

       
        static void ConvertToUtf8(FileInfo fi)
        {
            string text = File.ReadAllText(fi.FullName);
            File.WriteAllText(fi.FullName, text, Encoding.UTF8);
        }

        public static List<EditAction> editDistance(int[] current, int[] target)
        {
            int m = current.Length + 1;
            int n = target.Length + 1;
            int[,] edit = new int[m, n];
            int i, j;
            List<EditAction> actions = new List<EditAction>();
            if(current.Length<target.Length)
            {
                var list = new List<int>(current);
                for (i = current.Length; i < target.Length; i++)
                    list.Add(-1);
                current = list.ToArray();
            }
           
            for ( i = 0; i < m; i++) edit[i, 0] = i;
            for ( j = 0; j < n; j++) edit[0, j] = j;

            for ( i = 1; i < m; i++)
            {
                for ( j = 1; j < n; j++)
                {
                    int cost = current[i - 1] == target[j - 1] ? 0 : 1;
                    int deletion = edit[i - 1, j] + 1;
                    int substitution = edit[i - 1, j - 1] + cost;
                   if (deletion < substitution)
                    {
                        edit[i, j] = deletion;
                    }
                    else
                    {
                        edit[i, j] = substitution;
                    }
                }
            }

            for(i=0; i< m; i++)
            {
                for(j=0; j< n; j++)
                {
                    Console.Write(edit[i,j] + " ");
                }
                Console.WriteLine();
            }

            i = m - 1;j = n - 1;
            int step = 0;
            while(i>=0 && j>=0)
            {
                if(i>0 && j>0)
                {
                    if(edit[i-1,j-1] == edit[i,j])
                    {
                        actions.Add(new EditAction(0, step++));
                        i--;j--;
                    }
                    else if (edit[i - 1, j - 1] < edit[i, j-1])
                    {//替换
                        actions.Add(new EditAction(1, step++));
                        i--; j--;
                    }
                    else
                    {
                        //删除
                        actions.Add(new EditAction(2, step++));
                       j--;
                    }
                }
                if(j==0)
                {
                    actions.Add(new EditAction(2, step++));
                    i--;
                }
            }

            Console.WriteLine(edit[m-1,n-1]);
            return actions;
        }
        static void handler()
        {
            Thread.Sleep(5);
            Thread.CurrentThread.Abort();
        }

        public static void Parse(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            string content = "";
            using (StreamReader sr = new StreamReader(filePath))
            {
                content = sr.ReadToEnd();
            }
            var tableList = WikiTable.Parse(content);
            StreamWriter sw = new StreamWriter("outputtable.txt");
            foreach(var table in tableList)
            {
               foreach(var row in table.Rows)
                {
                    if(row.Count ==8)
                    {
                        Console.WriteLine(row.Count + "");
                        sw.Write(row.Count + "\t");
                        foreach(var item in row)
                        {
                            sw.Write(item + "##");
                        }
                        sw.WriteLine();
                    }
                    
                }
             
                
            }
            sw.Close();
        }
    }
}
