using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Wiki
{

    public enum WikiTableTokenTypes
    {
        TableStart,
        TableEnd,
        TableTitle,
        NewLine,
        ItemSplit,
        Item
    }

    public class WikiTable
    {
        private List<List<string>> tableItems = new List<List<string>>();
        private int currentLine = -1;

        public int RowLength { get { return currentLine + 1; } }

        public static List<WikiTable> Parse(string wikiTableContent)
        {
            List<WikiTableToken> tokenList = LexicalAnalysis(wikiTableContent);
            List<WikiTable> tableList = new List<WikiTable>();
            WikiTable t = null;
            foreach(var token in tokenList)
            {
                switch (token.WikiTableTokenType)
                {
                    case WikiTableTokenTypes.TableStart:
                        if(t != null)
                            throw new FormatException("未配对TableEnd");
                        t = new WikiTable();
                        break;
                    case WikiTableTokenTypes.TableEnd:
                        if (t == null)
                            throw new FormatException("未配对TableStart");
                        tableList.Add(t);
                        t = null;
                        break;
                    case WikiTableTokenTypes.Item:
                        if (t == null)
                            throw new FormatException("未配对TableStart");
                        t.AddItem(token.Value);
                        break;
                    case WikiTableTokenTypes.NewLine:
                        if (t == null)
                            throw new FormatException("未配对TableStart");
                        t.AddLine();
                        break;
                }
               
            }

            return tableList;
        }

        private void AddLine()
        {
            tableItems.Add(new List<string>());
            currentLine++;
        }

        private void AddItem(string item)
        {
            tableItems[currentLine].Add(item);
        }

        public static List<WikiTableToken> LexicalAnalysis(string content)
        {
            List<WikiTableToken> tokenList = new List<WikiTableToken>();
            int state = 0;//初始状态
            int temp = 0;
            bool inTable = false;
            for(int i=0; i<content.Length; i++)
            {
                char ch = content[i];
                switch (state)
                {
                    case 0:
                        if (ch == '{')//开始
                        { state = 1; temp = i; }
                        if (!inTable)
                            break;
                        if (ch == '|')
                        { state = 2; temp = i; }
                        else if (ch == '!')//列标题
                        { state = 5; temp = i; }
                        break;
                    case 1://判断是否为{|
                        if (ch == '|')
                        {
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.TableStart };
                            tokenList.Add(token);
                            temp = i;
                            inTable = true;
                        }
                        state = 0;
                        break;
                    case 2://以|开头
                        if(ch == '+')
                        {
                            temp = i;
                            state = 3;//标题开始
                        }
                        else if(ch == '-')//换行
                        {
                            temp = i;
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.NewLine };
                            tokenList.Add(token);
                            state = 0;
                        }
                        else if(ch == '|')//单元格划分
                        {
                            temp = i;
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.ItemSplit };
                            tokenList.Add(token);
                            state = 4;//字符内容
                        }
                        else if(ch == '}')//表格结束
                        {
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.TableEnd };
                            tokenList.Add(token);
                            state = 0;
                            inTable = false;
                        }
                        else
                        {
                            //字符内容
                            state = 4;
                            temp = i-1;
                            
                        }
                        break;
                    case 3://标题
                        if(ch == '\n' || ch == '|')//标题结束
                        {
                            state = 0;
                            string title = content.Substring(temp + 1, i - temp - 1);
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.TableTitle, Value = title };
                            tokenList.Add(token);
                            state = 0;
                        }
                        break;
                    case 4:
                        if (ch == '\n' )//单元格结束
                        {
                            /*string item = content.Substring(temp + 1, i - temp - 1);
                            WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.Item, Value = item };
                            tokenList.Add(token);
                            temp = i;
                            state = 0;*/
                        }
                        else if(ch == '|')
                        {
                            if(i+1<content.Length && (content[i+1]=='}' || content[i+1]=='-' || content[i+1]=='|'))
                            {
                                string item = content.Substring(temp + 1, i - temp - 1);
                                WikiTableToken token = new WikiTableToken() { WikiTableTokenType = WikiTableTokenTypes.Item, Value = item };
                                tokenList.Add(token);
                                temp = i;
                                state = 2;
                            }
                        }
                        break;
                    case 5://列标题，暂时不解析
                        if(ch == '\n')
                        {
                            state = 0;
                        }
                        break;
                }

            }

            return tokenList;
        }

        public List<List<string>> Rows { get { return tableItems; } }

        
    }

    public class WikiTableToken
    {
        public WikiTableTokenTypes WikiTableTokenType { get; set; }
        public string Value { get; set; }
    }
}

