using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace HackTheWorld
{
    public static class CodeParser
    {

        //つらを
        #region メイン
        public static ArrayList ConvertCodebox(string originStr)
        {
            originStr = RemoveSpace(originStr);
            

            //行で分割
            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[] delimiterChars = { '\n' };

            //分割した文を入れるリストと結果を入れるリスト
            ArrayList sArray = new ArrayList();
            ArrayList resultArray = new ArrayList();

            string[] tmp = originStr.Split(delimiterChars);
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }

            string strResult = "";
            //try
            //{
            if(!isFunction(sArray))
            {
                resultArray.Clear();
                resultArray.Add("へんな書きかた");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }

            if(!isValidScript(sArray))
            {
                resultArray.Clear();
                resultArray.Add("構文エラー");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }
            ConvertDirectionToNumbers(sArray);

            JumpToFunction(sArray,resultArray);

            strResult = ConvertArrayToString(resultArray);
            Console.WriteLine(strResult);
            //}
            //catch
            //{
            //Console.WriteLine("どこかがうまくいってない");
            //}
            if(LastCheck(strResult))
            {
                return resultArray;
            }
            resultArray.Clear();
            Console.WriteLine("最終チェックアウト");
            return resultArray;

        }
        public static ArrayList ConvertCodebox(string originStr,int maxMove,int maxSize,int maxWait)
        {
            originStr = RemoveSpace(originStr);

            //行で分割
            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[] delimiterChars = { '\n' };

            //分割した文を入れるリストと結果を入れるリスト
            ArrayList sArray = new ArrayList();
            ArrayList resultArray = new ArrayList();

            string[] tmp = originStr.Split(delimiterChars);
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }

            string strResult = "";
            //try
            //{
            if(!isFunction(sArray))
            {
                resultArray.Clear();
                resultArray.Add("へんな書きかた");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }

            if(!isValidScript(sArray))
            {
                resultArray.Clear();
                resultArray.Add("構文エラー");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }
            if(OverLimitString(originStr,maxMove,maxSize,maxWait))
            {
                resultArray.Clear();
                resultArray.Add("関数が多すぎる");
                strResult = ConvertArrayToString(resultArray);
                Console.WriteLine(strResult);
                return resultArray;
            }
            ConvertDirectionToNumbers(sArray);

            JumpToFunction(sArray,resultArray);

            strResult = ConvertArrayToString(resultArray);
            Console.WriteLine(strResult);
            //}
            //catch
            //{
            //Console.WriteLine("どこかがうまくいってない");
            //}
            if(LastCheck(strResult))
            {
                return resultArray;
            }
            resultArray.Clear();
            Console.WriteLine("最終チェックアウト");
            return resultArray;

        }
        public static void JumpToFunction(ArrayList sArray,ArrayList resultArray)
        {
            Hashtable hash = new Hashtable();

            ICollection valuecall = hash.Values;
            //連続で入力してデバックしたい
            hash.Clear();
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目が関数で始まってるかどうか
                switch(ReadSentenceHead(sArray,i))
                {
                    case 1:
                        For(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    case 2:
                        If(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    case 3:
                        While(sArray,resultArray,i,hash);
                        i = EndOfFunction(sArray,i);
                        break;
                    default:
                        UpdateHash(sArray,i,hash);
                        AssignmentHashValue(sArray,i,hash);
                        resultArray.Add(SearchAndAssignment((string)sArray[i]));
                        break;
                }
            }
            //「3=3」や「4++」を消したい
            ArrayList resultArray2 = new ArrayList();
            for(int i = 0;i < resultArray.Count;i++)
            {
                string s = (string)resultArray[i];
                if(!s.Contains("=") && !s.Contains("++")&&!s.Contains("--")) resultArray2.Add(resultArray[i]);
            }
            resultArray.Clear();
            for(int i = 0;i < resultArray2.Count;i++)
            {
                resultArray.Add(resultArray2[i]);
            }
            ConvertForYokouchi(resultArray);
        }
        #endregion

        #region チェック
        static string RemoveSpace(string s)
        {
            string space = " ";
            s = s.Replace(space,"");
            return s;
        }
        public static bool isValidScript(ArrayList sArray)
        {
            //全体の関数(for,if,while)の数
            int countFunction = CounterOfFunction(sArray);
            //閉じるためのendの数
            int countEnd = CounterOfEnd(sArray);
            //カウント(forの中にforがいたら次のendが終わりじゃないので数えたい)
            int count = 0;

            if(countFunction != countEnd)
            {
                Console.WriteLine("関数とendの数が違う");
            }
            if(countFunction == 0) return true;
            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                int j = 0;
                //i行目がforで始まってるかどうか
                if(FirstFor(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            if(!isFor(sArray,i))
                            {
                                Console.WriteLine("forとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countFunction--;
                            break;
                        }
                    }

                }
                //似たようなもん
                if(FirstIf(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(FirstFunction(sArray,j)) count++;
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            if(!isIf(sArray,i))
                            {
                                //WindowContext.Invoke((Action)(() => {
                                //    Console.WriteLine("ifとendはいるみたいだけど\n文の中身が違う");
                                //}));
                                Console.WriteLine("ifとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countFunction--;
                            break;
                        }
                    }
                }
                if(FirstWhile(sArray,i))
                {
                    count++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(FirstFunction(sArray,j)) count++;
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            countFunction--;
                            break;
                        }
                    }
                }
                if(count != 0)
                {
                    Console.WriteLine("endが多い");
                    return false;
                }
                //関数が全部なくなったら
                if(countFunction == 0) return true;
            }
            //ループ回り切ったらだめ
            Console.WriteLine("関数とendの対応がだめ");
            return false;
        }

        public static bool isFunction(ArrayList sArray)
        {
            string msg = "";
            string s = "";
            int count = 0;

            //意味ない言葉が混ざっていないか見たい
            //"size,1,1", "wait,1", "move,1,1,2"
            List<Regex> reg = new List<Regex>();
            reg.Add(new Regex(@"\s*size\s*\(\s*[\w+|\+|\-|\*|\/|\.|\%|\(|\)]+\s*,\s*[\w+|\+|\-|\*|\/|\.|\%|\(|\)]+\)"));
            reg.Add(new Regex(@"\s*wait\s*\(\s*[\w+|\+|\-|\*|\/|\.|\%|\(|\)]+\)"));
            reg.Add(new Regex(@"\s*move\([\w+|\+|\-|\*|\/|\.|\%|\(|\)]+\)"));
            reg.Add(new Regex(@"\s*\w+\s*=\s*[\w+|\+|\-|\*|\/|\%|\(|\)]+\s*"));
            reg.Add(new Regex(@"\s*(?<name>[a-zA-z]+)\s*="));
            reg.Add(new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\+"));
            reg.Add(new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\-"));
            reg.Add(new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\="));
            reg.Add(new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\="));
            reg.Add(new Regex(@"for"));
            reg.Add(new Regex(@"if"));
            reg.Add(new Regex(@"while"));
            reg.Add(new Regex(@"end"));
            reg.Add(new Regex(@"else"));
            reg.Add(new Regex(@"break"));
            reg.Add(new Regex(@"player.ontop"));


            Match[] mat = new Match[reg.Count];

            for(int i = 0;i < sArray.Count;i++)
            {
                count = 0;
                s = sArray[i].ToString();
                //matchの配列の初期化
                for(int j = 0;j < reg.Count;j++)
                {
                    mat[j] = reg[j].Match(s);
                }
                //エラーメッセージを吐いてほしい
                if(s.Contains("size") && mat[0].Length == 0) msg = "size";
                if(s.Contains("wait") && mat[1].Length == 0) msg = "wait";
                if(s.Contains("move") && mat[2].Length == 0) msg = "move";

                if(msg.Length != 0)
                {
                    Console.WriteLine(msg + "の書き方がまちがってます");
                    return false;
                }
                for(int j = 0;j < reg.Count;j++)
                {
                    if(mat[j].Length != 0) count++;
                }

                //すべてのmatchに当てはまらなかった
                if(count == 0)
                {
                    Console.WriteLine("知らない形の文が" + (i + 1) + "行目にあります");
                    return false;
                }
            }
            return true;
        }
        static bool OverLimitString(string s,int maxMove,int maxSize,int maxWait)
        {
            Regex[] limited = new Regex[3];
            limited[0] = new Regex(@"move");
            limited[1] = new Regex(@"size");
            limited[2] = new Regex(@"wait");
            int[] count = new int[3];
            for(int i = 0;i < 3;i++)
            {
                count[i] = limited[i].Matches(s).Count;
            }
            if(count[0] > maxMove || count[1] > maxSize || count[2] > maxWait) return true;
            return false;
        }
        static bool LastCheck(string s)
        {
            string[] sArray = s.Split('\n');
            int size = 3;

            Regex[] reg = new Regex[size];
            Match[] mat = new Match[size];

            reg[0] = new Regex(@"move,,,");
            reg[1] = new Regex(@"size,,");
            reg[2] = new Regex(@"wait,$");

            for(int i = 0;i < sArray.Length;i++)
            {
                for(int j = 0;j < size;j++)
                {
                    mat[j] = reg[j].Match(sArray[i]);
                    if(mat[j].Length != 0) return false;
                }
            }
            return true;
        }
        #endregion

        #region 繰り返し出てくる処理
        //sArray[home]がforなどである場合、そのforに対応したendの次の行が何行目なのか知りたい
        static int EndOfFunction(ArrayList sArray,int home)
        {
            int count = 1;
            int j = 0;
            for(j = home + 1;j < sArray.Count;j++)
            {
                if(FirstFunction(sArray,j)) count++;
                if(FirstEnd(sArray,j)) count--;
                if(count == 0)
                {
                    break;
                }
            }
            return j;
        }
        //最初が何で始まるかに応じて1か2か3で返す
        public static int ReadSentenceHead(ArrayList sArray,int i)
        {
            if(FirstFor(sArray,i)) return 1;
            if(FirstIf(sArray,i)) return 2;
            if(FirstWhile(sArray,i)) return 3;
            return 0;
        }
        //四則演算を行う
        public static string FourOperations(string s)
        {
            //かっこ優先
            int counter = 0;
            while(System.Text.RegularExpressions.Regex.IsMatch(s,@"\([\d+|\+|\-|\*|\/|\(|\)|\%]+\)")&&counter<100)
            {
                Regex regInside = new Regex(@"\((?<inside>[\d+|\+|\-|\*|\/|\%]+)\)");
                Regex regInside2 = new Regex(@"\([\d+|\+|\-|\*|\/|\%]+\)");
                Match mat = regInside.Match(s);
                string ans = FourOperations(mat.Groups["inside"].Value);
                s = regInside2.Replace(s,ans,1);
                counter++;
            }

            //Pow（累乗）をする(dt.computeで対応してないっぽい)
            while(System.Text.RegularExpressions.Regex.IsMatch(s,@"[\d|\(|\)|\-]+\^[\d|\(|\)|\-]+"))
            {
                Regex reg = new Regex(@"(?<left_hand>[\d|\(|\)|\-]+)\^(?<right_hand>[\d|\(|\)|\-]+)");
                Match mat = reg.Match(s);
                double left = Convert.ToDouble(FourOperations(mat.Groups["left_hand"].ToString()));
                double right = Convert.ToDouble(FourOperations(mat.Groups["right_hand"].ToString()));
                string ans = Math.Pow(left,right).ToString();
                s = reg.Replace(s,ans,1);
            }

            if(System.Text.RegularExpressions.Regex.IsMatch(s,@"\d+|[\+|\-|\*|\/|\(|\)|\%]+") && !s.StartsWith(@"[\+|\-|\*|\/]") && !s.EndsWith(@"[\+|\-|\*|\/]") && !s.Contains(@"[\+\+|\-\-|\*\*|\/\/]"))
            {
                //ここで計算
                DataTable dt = new DataTable();

                //Type t = dt.Compute(s,"").GetType();

                s= dt.Compute(s,"").ToString();
                double a = Convert.ToDouble(s.ToString());
                int b = (int)a;
                s = b.ToString();
            }

            if(System.Text.RegularExpressions.Regex.IsMatch(s,@"[\d+|\-]+")) return s;
            
            return "四則演算が変";
        }

        //ArrayListを\nで区切りながらstringに入れる
        static string ConvertArrayToString(ArrayList sArray)
        {
            string str = "";
            for(int i = 0;i < sArray.Count;i++)
            {
                str += (string)sArray[i] + "\n";
            }
            return str;
        }
        //ArrayListの中の関数の数を数える
        static int CounterOfFunction(ArrayList sArray)
        {
            int count = new int();
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(FirstFunction(sArray,i)) count++;
            }
            return count;
        }
        //ArrayListの中のendの数を数える
        static int CounterOfEnd(ArrayList sArray)
        {
            int count = new int();
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(FirstEnd(sArray,i)) count++;
            }
            return count;
        }
        //forなどにおいてhome行目の関数からendまでを抜き出して別のarraylistに入れたいときに使う
        static ArrayList CopyArrayList(ArrayList sArray,int home)
        {
            int i = 1;
            int count = 1;
            ArrayList tArray = new ArrayList();
            while(true)
            {
                if(FirstFunction(sArray,home + i)) count++;
                if(FirstEnd(sArray,home + i)) count--;

                tArray.Add(sArray[home + i]);

                if(count == 0) break;
                i++;
            }
            return tArray;
        }
        static void ConvertDirectionToNumbers(ArrayList sArray)
        {
            Regex[] reg = new Regex[4];
            reg[0] = new Regex(@"right"); 
            reg[1] = new Regex(@"down");
            reg[2] = new Regex(@"left");
            reg[3] = new Regex(@"up");

            Match[] mat = new Match[4];
            for(int i = 0;i < sArray.Count;i++)
            {
                string s = sArray[i].ToString();
                for(int j = 0;j < 4;j++)
                {
                    mat[j] = reg[j].Match(s);
                    while(mat[j].Length > 0)
                    {
                        s = reg[j].Replace(s,j.ToString(),1);
                        mat[j] = reg[j].Match(s);
                    }
                }
                sArray[i] = s;
            }
        }

        static void ConvertForYokouchi(ArrayList sArray)
        {
            for(int i = 0;i < sArray.Count;i++)
            {
                string s = (string)sArray[i];

                if(Regex.IsMatch(s,@"^(player.ontop,|player.nearby,|player.touch,)*move"))
                {

                    Regex reg = new Regex(@"(?<head>\s*.*,*move)\s*\(\s*(?<a>[\-|\d|\.]+)");
                    Match mat = reg.Match(s);
                    string result = mat.Groups["head"].Value;
                    int dir = 0;

                    if(Convert.ToInt32(mat.Groups["a"].Value) >= 0) dir=Convert.ToInt32(mat.Groups["a"].Value) % 4;
                    else dir=4+ Convert.ToInt32(mat.Groups["a"].Value) % 4;


                    switch(dir)
                    {
                        //0123→右下左上
                        case 0:
                        case 4:
                            result += "(right)";
                            break;
                        case 1:
                            result += "(down)";
                            break;
                        case 2:
                            result += "(left)";
                            break;
                        case 3:
                            result += "(up)";
                            break;
                    }
                    sArray[i] = result;
                }
            }
        }
        static string SearchAndAssignment(string s)
        {
            if(Regex.IsMatch(s,@"wait")|| Regex.IsMatch(s,@"size") || Regex.IsMatch(s,@"move"))
            {
                int counter = 0;
                while(System.Text.RegularExpressions.Regex.IsMatch(s,@"wait\([\d+|\+|\-|\*|\/|\(|\)|\%]+\)") && counter < 100)
                {
                    Regex regInside = new Regex(@"wait\((?<inside>[\d+|\+|\-|\*|\/|\%|\(|\)]+)\)");
                    Regex regInside2 = new Regex(@"wait\([\d+|\+|\-|\*|\/|\%|\(|\)]+\)");
                    Match matInside = regInside.Match(s);
                    string ans = "wait("+FourOperations(matInside.Groups["inside"].Value)+")";
                    s = regInside2.Replace(s,ans,1);
                    counter++;
                }
                counter = 0;
                while(System.Text.RegularExpressions.Regex.IsMatch(s,@"size\([\d+|\+|\-|\*|\/|\(|\)|\%]+,[\d+|\+|\-|\*|\/|\(|\)|\%]+\)") && counter < 100)
                {
                    Regex regInside = new Regex(@"size\((?<left>[\d+|\+|\-|\*|\/|\(|\)|\%]+),(?<right>[\d+|\+|\-|\*|\/|\(|\)|\%]+)\)");
                    Regex regInside2 = new Regex(@"size\([\d+|\+|\-|\*|\/|\(|\)|\%]+,");
                    Regex regInside3 = new Regex(@",[\d+|\+|\-|\*|\/|\(|\)|\%]+\)");
                    Match matInside = regInside.Match(s);

                    string ansLeft = "size("+FourOperations(matInside.Groups["left"].Value)+",";
                    string ansRight = FourOperations(matInside.Groups["right"].Value)+")";

                    s = regInside2.Replace(s,ansLeft,1);
                    s = regInside3.Replace(s,ansRight,1);
                    counter++;
                }
                counter = 0;
                while(System.Text.RegularExpressions.Regex.IsMatch(s,@"move\([\d+|\+|\-|\*|\/|\(|\)|\%]+\)") && counter < 100)
                {
                    Regex regInside = new Regex(@"move\((?<inside>[\d+|\+|\-|\*|\/|\%|\(|\)]+)\)");
                    Regex regInside2 = new Regex(@"move\([\d+|\+|\-|\*|\/|\%|\(|\)]+\)");
                    Match matInside = regInside.Match(s);
                    string ans = "move(" + FourOperations(matInside.Groups["inside"].Value) + ")";
                    s = regInside2.Replace(s,ans,1);
                    counter++;
                }
            }
            Regex reg = new Regex(@"\d+[\+|\-|\*|\/|\%]+[\d+|\+|\-|\*|\/|\%]*\d+");
            Match mat = reg.Match(s);
            while(mat.Length > 0)
            {
                string ans = FourOperations(mat.Value);
                s = reg.Replace(s,ans,1);
                mat = reg.Match(s);
            }
            return s;
        }
        static void AddPrefix(ArrayList sArray,ArrayList result,int home,int i)
        {
            string prefix = "";
            switch(i)
            {
                case 0:
                    prefix = "player.ontop,";
                    break;
                case 1:
                    prefix = "player.nearby,";
                    break;
                case 2:
                    prefix = "player.touch,";
                    break;
            }

            for(int j = home + 1;j < EndOfFunction(sArray,home);j++)
            {
                string s = (string)sArray[j];
                if(s.StartsWith("move") || s.StartsWith("wait"))
                {
                    result.Add(s.Insert(0,prefix));
                }
            }
        }
        #endregion

        #region 関数の位置をタプルで返す
        public static Tuple<int,int>[] RowNumberOfFor(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }

            ArrayList forArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstFor(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            forArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[forArray.Count];
            for(int i = 0;i < forArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)forArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfIf(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }
            ArrayList ifArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstIf(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            //tupleの中に行番号を入れる
                            ifArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[ifArray.Count];
            for(int i = 0;i < ifArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)ifArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfWhile(string s)
        {
            ArrayList sArray = new ArrayList();

            string[] tmp = s.Split('\n');
            for(int i = 0;i < tmp.Length;i++)
            {
                if(tmp[i] != "") sArray.Add(tmp[i]);
            }
            ArrayList whileArray = new ArrayList();

            int count = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(FirstWhile(sArray,i))
                {
                    count++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(FirstFunction(sArray,j)) count++;
                        //endがいたらへらす
                        if(FirstEnd(sArray,j)) count--;
                        if(count == 0)
                        {
                            //tupleの中に行番号を入れる
                            whileArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] result = new Tuple<int,int>[whileArray.Count];
            for(int i = 0;i < whileArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)whileArray[i];
            }
            return result;
        }
        public static Tuple<int,int>[] RowNumberOfFunction(string s)
        {
            ArrayList allArray = new ArrayList();

            Tuple<int,int>[] t1 = RowNumberOfFor(s);
            Tuple<int,int>[] t2 = RowNumberOfIf(s);
            Tuple<int,int>[] t3 = RowNumberOfWhile(s);

            for(int i = 0;i < t1.Length;i++)
            {
                allArray.Add(t1[i]);
            }
            for(int i = 0;i < t2.Length;i++)
            {
                allArray.Add(t2[i]);
            }
            for(int i = 0;i < t3.Length;i++)
            {
                allArray.Add(t3[i]);
            }

            Tuple<int,int>[] result = new Tuple<int,int>[allArray.Count];
            for(int i = 0;i < allArray.Count;i++)
            {
                result[i] = (Tuple<int,int>)allArray[i];
            }
            return result;
        }
        #endregion

        #region hash関係
        public static void UpdateHash(ArrayList sArray,int i,Hashtable hash)
        {
            string str1, str2, str3;
            ICollection keycall = hash.Keys;
            string input = (string)sArray[i];
            Regex equals = new Regex(@"\=|\+\=|\-\=|\+\+|\-\-");
            Match equalsMatch = equals.Match(input);

            Regex[] regSeparate = new Regex[5];
            regSeparate[0] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*=\s*(?<right_hand>[(?<value>\w+)|\+|\-|\*|\/|\.|\(|\)]+)\s*");
            regSeparate[1] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\+");
            regSeparate[2] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\-");
            regSeparate[3] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>[\d|\w|\+|\-|\*|\/|\.|\(|\)]+)");
            regSeparate[4] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\=\s*(?<value>[\d|\w|\+|\-|\*|\/|\.|\(|\)]+)");

            int stepforward = 0;

            while(0 != equalsMatch.Length)
            {
                if(regSeparate[0].IsMatch(input,stepforward))
                {
                    Regex reg = new Regex(@"(?<name>\w+)\s*=\s*(?<right_hand>[\d|\w|\+|\-|\*|\/|\.|\(|\)]+)");
                    Match mat = reg.Match(input,stepforward);
                    str1 = mat.Groups["right_hand"].Value;
                    

                    foreach(string k in keycall)
                    {
                        //右辺にすでにハッシュに入れたものがいる
                        while(str1.Contains(k))
                        {
                            string pattern = k;
                            string replacement = hash[k].ToString();
                            Regex r = new Regex(pattern);
                            input = r.Replace(input,replacement);
                            mat = reg.Match(input);
                            str1 = mat.Groups["right_hand"].Value;
                        }
                    }
                    //右辺に今までハッシュに登録されていない文字がいる
                    if(str1.Contains(@"\w+"))
                    {
                        Console.WriteLine("ここ");
                        return;
                    }
                    reg = new Regex(@"\s*(?<name>\w+)\s*=\s*(?<right_hand>[\d+|\+|\-|\*|\/|\.|\%|\^|\,|\(|\)]+)\s*");
                    mat = reg.Match(input,stepforward);
                    str2 = mat.Groups["right_hand"].Value;
                    //右辺の文字は数字に置換されたはずなので四則演算の関数に入れてよい
                    str2 = FourOperations(str2);

                    //hashへの登録
                    str3 = mat.Groups["name"].Value;
                    if(str3=="right"|| str3 == "down"||str3 == "left"||str3 == "up")
                    {
                        return;
                    }
                    hash[str3] = str2;

                }
                //ここから＋＋とか＋＝とかの部分
                if(regSeparate[1].IsMatch(input,stepforward))
                {
                    Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\+");
                    Match m = r.Match(input,stepforward);
                    str1 = m.Groups["name"].Value;
                    if(str1 == "right" || str1 == "down" || str1 == "left" || str1 == "up")
                    {
                        return;
                    }
                    hash[str1] = Convert.ToInt32(hash[str1]) + 1;
                }
                if(regSeparate[2].IsMatch(input,stepforward))
                {
                    Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\-\-");
                    Match m = r.Match(input,stepforward);
                    str1 = m.Groups["name"].Value;
                    if(str1 == "right" || str1 == "down" || str1 == "left" || str1 == "up")
                    {
                        return;
                    }
                    hash[str1] = Convert.ToInt32(hash[str1]) - 1;
                }
                if(regSeparate[3].IsMatch(input,stepforward))
                {
                    Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>[\d|\w|\+|\-|\*|\/|\.|\(|\)]+)");
                    Match m = r.Match(input,stepforward);
                    str1 = m.Groups["name"].Value;
                    str2 = FourOperations(m.Groups["value"].Value);
                    if(str1 == "right" || str1 == "down" || str1 == "left" || str1 == "up")
                    {
                        return;
                    }
                    hash[str1] = Convert.ToInt32(hash[str1]) + int.Parse(str2);
                }
                if(regSeparate[4].IsMatch(input,stepforward))
                {
                    Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\-\=\s*(?<value>[\d|\w|\+|\-|\*|\/|\.|\(|\)]+)");
                    Match m = r.Match(input,stepforward);
                    str1 = m.Groups["name"].Value;
                    str2 = FourOperations(m.Groups["value"].Value);
                    if(str1 == "right" || str1 == "down" || str1 == "left" || str1 == "up")
                    {
                        return;
                    }
                    hash[str1] = Convert.ToInt32(hash[str1]) - int.Parse(str2);
                }

                int nextIndex = equalsMatch.Index + equalsMatch.Value.Length;
                if(nextIndex < input.Length)
                {
                    //次の位置を探す

                    Regex blanc = new Regex(@"\s");
                    Match matchBlanc = blanc.Match(input,nextIndex);
                    stepforward = matchBlanc.Index;
                    equalsMatch = equals.Match(input,nextIndex);
                }
                else
                {
                    //最後まで検索したので終わる
                    break;
                }
            }

        }

        public static void AssignmentHashValue(ArrayList sArray,int x,Hashtable hash)
        {
            ICollection keycall = hash.Keys;
            //hashになにか入ってたら
            if(keycall.Count > 0)
            {
                //hashのkeyごとに
                foreach(string key in keycall)
                {
                    string input = (string)sArray[x];
                    int foundIndex = input.IndexOf(key);
                    //inputのどこかにハッシュのキーがいる限り回る
                    while(0 <= foundIndex)
                    {
                        bool canAssignment = true;
                        string s;

                        //左右に余計な文字がついている:一番はじめや一番後ろでマッチしたときは見る場所がずれる
                        if(foundIndex == 0)
                        {
                            s = input.Substring(foundIndex,key.Length + 1);
                            if(Regex.IsMatch(s,@"[a-zA-Z]$")) canAssignment = false;
                        }
                        else if(input.Length > foundIndex + key.Length + 1)
                        {
                            //abcde
                            s = input.Substring(foundIndex - 1,key.Length + 2);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]") || Regex.IsMatch(s,@"[a-zA-Z]$")) canAssignment = false;
                        }
                        else
                        {
                            s = input.Substring(foundIndex - 1,key.Length + 1);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]")) canAssignment = false;
                        }

                        if(canAssignment)
                        {
                            //charの配列をlistに追加していき、最後にstringにしたい
                            char[] c = input.ToCharArray();
                            char[] cHashValue = hash[key].ToString().ToCharArray();
                            List<char> cList = new List<char>();

                            //はじめにkeyがいないときはもともとの文が最初に入る
                            if(foundIndex != 0)
                            {
                                for(int i = 0;i < foundIndex;i++)
                                {
                                    cList.Add(c[i]);
                                }
                            }
                            //hashの中身の値をリストに加える
                            for(int i = 0;i < cHashValue.Length;i++)
                            {
                                cList.Add(cHashValue[i]);
                            }
                            //keyの後ろの残りの文を入れる
                            if(foundIndex + key.Length < c.Length)
                            {
                                for(int i = foundIndex + key.Length;i < c.Length;i++)
                                {
                                    cList.Add(c[i]);
                                }
                            }
                            string result = "";
                            for(int i = 0;i < cList.Count;i++)
                            {
                                result += cList[i];
                            }

                            //hashの値を代入した後の文に差し替える
                            sArray[x] = result;
                        }

                        //次の検索開始位置を決める
                        input = (string)sArray[x];
                        int nextIndex = foundIndex + key.Length;
                        if(nextIndex < input.Length)
                        {
                            //次の位置を探す
                            foundIndex = input.IndexOf(key,nextIndex);
                        }
                        else
                        {
                            //最後まで検索したので終わる
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region for if while
        public static void For(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            int typeOfFor = 0;
            //sArray[home]はforから始まっていて繰り返し回数を指定している行
            //どんな書き方をしているかの正規表現を用いた場合分けをしたい
            //いちいち間に\s*(0個以上の空白文字を示す)を入れて間に空白が入っても読めるようにする
            //typeを3つ作ることにする

            //for(i=0;i<5;i++)がtype1
            if(Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|(<=)|(>=)|(==)" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)")) typeOfFor = 1;
            //for i=0 to 3がtype2
            if(Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\w+\s*to\s*\w+")) typeOfFor = 2;
            //for 2とかをtype3とする
            if(Regex.IsMatch((string)sArray[home],@"for\s*\w+")) typeOfFor = 3;

            switch(typeOfFor)
            {
                case 1:
                    Regex reg1_1 = new Regex(@"(?<start>\w+\s*\=\s*\w+)");
                    Regex reg1_2 = new Regex(@"(?<condition>\w+\s*(<|>|(<=)|(>=)|(==))\s*\w+)");
                    Regex reg1_3 = new Regex(@"(?<update>\w+((\+\+)|(\-\-)|(\+=\w+)|(\-=\w+)))");
                    Match m1_1 = reg1_1.Match((string)sArray[home]);
                    Match m1_2 = reg1_2.Match((string)sArray[home]);
                    Match m1_3 = reg1_3.Match((string)sArray[home]);
                    //代入して条件式などが失われると困るので別のArrayListに入れておきたい
                    ArrayList tArray = new ArrayList();
                    ArrayList uArray = new ArrayList();
                    bool breakIsExists = false;

                    //初期条件を別のArrayListに入れ、hashを更新する
                    tArray.Add(m1_1.Groups["start"].Value);
                    UpdateHash(tArray,0,hash);

                    //大小判定をしたいので、条件式に代入する
                    uArray.Add(m1_2.Groups["condition"].Value);
                    AssignmentHashValue(uArray,0,hash);
                    //ここからループ、条件式の大小判定があっているか確かめる
                    while(SizeComparing((string)uArray[0]))
                    {
                        tArray = CopyArrayList(sArray,home);
                        //最後に更新式をつけておく
                        tArray.Insert(tArray.Count - 1,m1_3.Groups["update"].Value);

                        //1行ずつ読む
                        for(int i = 0;i < tArray.Count;i++)
                        {
                            while(!FirstEnd(tArray,i))
                            {
                                switch(ReadSentenceHead(tArray,i))
                                {
                                    case 1:
                                        For(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    case 2:
                                        If(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    case 3:
                                        While(tArray,result,i,hash);
                                        i = EndOfFunction(tArray,i) + 1;
                                        break;
                                    default:
                                        UpdateHash(tArray,i,hash);
                                        AssignmentHashValue(tArray,i,hash);
                                        result.Add(SearchAndAssignment((string)tArray[i]));
                                        i++;
                                        break;
                                }
                                //breakが結果に入っていたらbreakしたい
                                if(result.Count != 0)
                                {
                                    if(result[result.Count - 1].ToString() == "break")
                                    {
                                        breakIsExists = true;
                                        result.RemoveAt(result.Count - 1);
                                        break;
                                    }
                                }
                            }
                            if(breakIsExists) break;
                        }
                        if(breakIsExists) break;
                        //条件式をまた入れ、最新の値で代入しておく
                        uArray.Clear();
                        uArray.Add(m1_2.Groups["condition"].Value);
                        AssignmentHashValue(uArray,0,hash);
                    }
                    return;

                case 3:
                    Regex reg3 = new Regex(@"for\s*(?<repeat>\w+)");
                    Match m3 = reg3.Match((string)sArray[home]);

                    int repeatCount = 0;
                    //hashの値を用いるか、数字として読むかして繰り返し回数を決める
                    if(hash.ContainsKey(m3.Groups["repeat"].Value)) repeatCount = Convert.ToInt32(hash[m3.Groups["repeat"].Value]);
                    else if(!int.TryParse(m3.Groups["repeat"].Value,out repeatCount))
                    {
                        Console.WriteLine("(For type3)数字を代入していますか？");
                        return;
                    }
                    breakIsExists = false;

                    //これも代入によって原文が書き換わらないようにするためのarraylist
                    tArray = new ArrayList();

                    UpdateHash(sArray,home,hash);
                    for(int i = 0;i < repeatCount;i++)
                    {
                        tArray = CopyArrayList(sArray,home);
                        int j = 0;
                        while(!FirstEnd(tArray,j))
                        {
                            UpdateHash(tArray,j,hash);

                            switch(ReadSentenceHead(tArray,j))
                            {
                                case 1:
                                    For(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                case 2:
                                    If(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                case 3:
                                    While(tArray,result,j,hash);
                                    j = EndOfFunction(tArray,j) + 1;
                                    break;
                                default:
                                    AssignmentHashValue(tArray,j,hash);
                                    result.Add(SearchAndAssignment((string)tArray[j]));
                                    j++;
                                    break;
                            }
                            if(result.Count != 0)
                            {
                                if(result[result.Count - 1].ToString() == "break")
                                {
                                    breakIsExists = true;
                                    result.RemoveAt(result.Count - 1);
                                    break;
                                }
                            }
                        }
                        if(breakIsExists) break;
                    }

                    return;
                default:
                    result.Add("whileがうまくいっていない");
                    return;
            }
        }


        public static void If(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            UpdateHash(sArray,home,hash);

            ArrayList tArray = new ArrayList();

            //条件を抜き出してarraylistへ
            string s = (string)sArray[home];

            Regex reg1 = new Regex(@"^if\((?<condition>.+)\)$");
            Match m1 = reg1.Match(s);

            Regex[] reg = new Regex[3];
            Match[] mat = new Match[3];
            reg[0] = new Regex(@"player.ontop");
            reg[1] = new Regex(@"player.nearby");
            reg[2] = new Regex(@"player.touch");
            for(int i = 0;i < mat.Length;i++)
            {
                mat[i] = reg[i].Match(s);
                if(mat[i].Length != 0)
                {
                    AddPrefix(sArray,result,home,i);
                    return;
                }
            }

            tArray.Add(m1.Groups["condition"].Value);
            AssignmentHashValue(tArray,0,hash);
            tArray[0] = SearchAndAssignment((string)tArray[0]);


            if(SizeComparing((string)tArray[0]))
            {
                int i = 1;
                //行のはじめにendがくるかbreakがくるまで読む
                while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                {
                    if(FirstElse(sArray,home + i)) break;
                    UpdateHash(sArray,home + i,hash);

                    switch(ReadSentenceHead(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        default:
                            AssignmentHashValue(sArray,home + i,hash);
                            result.Add(SearchAndAssignment((string)sArray[home + i]));
                            i++;
                            break;
                    }
                }
            }
            else
            {
                //if文がfalseになったら
                int tmp = 0;
                int i = 0;
                //elseがいるか探す、いなかったらこのif文はやることがないのでreturnする
                while(true)
                {
                    if(FirstElse(sArray,home + tmp))
                    {
                        i = tmp + 1;
                        break;
                    }
                    tmp++;
                    if(home + tmp >= sArray.Count) return;
                }
                //elseの行からまた読み直す
                while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                {
                    //home+iまで同じことをする
                    UpdateHash(sArray,home + i,hash);

                    switch(ReadSentenceHead(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            i = EndOfFunction(sArray,home + i);
                            break;
                        default:
                            AssignmentHashValue(sArray,home + i,hash);
                            result.Add(SearchAndAssignment((string)sArray[home + i]));
                            i++;
                            break;
                    }
                }
            }
        }

        public static void While(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            ArrayList tArray = new ArrayList();
            ArrayList uArray = new ArrayList();
            int whileCounter = 0;

            //条件を抜き出す
            string s = (string)sArray[home];
            Regex r = new Regex(@"(?<condition>\w+\s*(<|>|(<=)|(>=)|(==))\s*\d+)");
            Match m = r.Match(s);
            bool breakIsExists = false;

            //homeまで読んでhash登録、代入、forとendの対応の取り直し
            UpdateHash(sArray,home,hash);

            while(!breakIsExists && whileCounter < 100)
            {
                //uArrayは条件式だけをいれるためにいる
                //ループが回るたびに条件式を入れなおし、代入しなおす
                uArray.Clear();
                uArray.Add(m.Value);
                AssignmentHashValue(uArray,0,hash);

                if(SizeComparing((string)uArray[0]))
                {
                    //tArrayにsArrayのすべてをコピー
                    tArray.Clear();
                    for(int j = 0;j < sArray.Count;j++)
                    {
                        tArray.Add(sArray[j]);
                    }


                    int i = 1;
                    while(!FirstEnd(sArray,home + i) && !FirstBreak(sArray,home + i - 1))
                    {
                        //home+iまで同じことをする
                        UpdateHash(sArray,home + i,hash);

                        switch(ReadSentenceHead(sArray,home + i))
                        {
                            case 1:
                                For(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home + i);
                                break;
                            case 2:
                                If(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home + i);
                                break;
                            case 3:
                                While(sArray,result,home + i,hash);
                                i = EndOfFunction(sArray,home + i);
                                break;
                            default:
                                AssignmentHashValue(tArray,home + i,hash);
                                result.Add(SearchAndAssignment((string)tArray[home + i]));
                                i++;
                                break;
                        }
                        if(result.Count != 0)
                        {
                            if(result[result.Count - 1].ToString() == "break")
                            {
                                breakIsExists = true;
                                result.RemoveAt(result.Count - 1);
                                break;
                            }
                        }
                    }
                    whileCounter++;
                }
                else break;
            }
        }

        #endregion

        #region bool群
        public static bool isFor(ArrayList sArray,int home)
        {
            //一致してるかは知りたいけどうしろに余計なのがついてたらはじきたい
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)\s*$")) return true;
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*=\s*\w+\s*to\s*\w+\s*$")) return true;
            if(Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*$")) return true;
            return false;
        }

        public static bool isIf(ArrayList sArray,int home)
        {
            List<string> reg = new List<string>();
            reg.Add(@"[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+\s*\<\s*[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+");
            reg.Add(@"[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+\s*\>\s*[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+");
            reg.Add(@"[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+\s*\<\s*\=\s*[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+");
            reg.Add(@"[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+\s*\>\s*\=\s*[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+");
            reg.Add(@"[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+\s*\=\s*\=\s*[\d+|\w+|\+|\-|\*|\/|\(|\)|\%]+");
            reg.Add(@"player.ontop");

            for(int i = 0;i < reg.Count;i++)
            {
                if(Regex.IsMatch((string)sArray[home],@"if\s*\(" + reg[i] + @"\)\s*"))
                {
                    if(Regex.IsMatch((string)sArray[home],@"if\s*\(" + reg[i] + @"\)\s*."))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        static bool FirstFunction(ArrayList sArray,int i)
        {
            if(FirstFor(sArray,i) || FirstIf(sArray,i) || FirstWhile(sArray,i)) return true;
            return false;
        }
        //この辺はifの中に書くとき短くしたいからいる
        public static bool FirstFor(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("for")) return true;
            return false;
        }
        public static bool FirstIf(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("if")) return true;
            return false;
        }
        public static bool FirstWhile(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("while")) return true;
            return false;
        }
        public static bool FirstEnd(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("end")) return true;
            return false;
        }
        public static bool FirstElse(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("else")) return true;
            return false;
        }
        public static bool FirstBreak(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("break")) return true;
            return false;
        }

        //ifのための判定群
        static bool SizeComparing(string s)
        {
            string result = SizeComparingMain(s);
            if(result == "true") return true;
            else return false;
        }
        public static string SizeComparingMain(string s)
        {
            //大小を比較している部分すべてに「true」[false」を叩き込む
            //かっこを先に抜き出して、かつの判定をすべて行ったのちまたはの判定をおこなう
            //かっこがおわったら別のかっこをさがす、なかったらかつ→または
            //＆と棒がなくなったらおわり
            Regex reg = new Regex(@"\d+[\d|\+|\-|\*|\/]+\d+");
            Match mat = reg.Match(s);
            Regex onlyNumber = new Regex(@"^\d+$");
            Match matchNumber = onlyNumber.Match(mat.Value);
            while(mat.Length > 0 && matchNumber.Length == 0)
            {
                string ans = FourOperations(mat.Value);
                s = reg.Replace(s,ans,1);
                mat = reg.Match(s);
                matchNumber = onlyNumber.Match(mat.Value);
            }

            string TrueOrFalse = "";
            //大小判定したい

            Regex reg1 = new Regex(@"(?<left_hand>\d+)\s*<\s*(?<right_hand>\d+)");
            Match m1 = reg1.Match(s);

            Regex reg2 = new Regex(@"(?<left_hand>\d+)\s*>\s*(?<right_hand>\d+)");
            Match m2 = reg2.Match(s);

            Regex reg3 = new Regex(@"(?<left_hand>\d+)\s*<\s*=\s*(?<right_hand>\d+)");
            Match m3 = reg3.Match(s);

            Regex reg4 = new Regex(@"(?<left_hand>\d+)\s*>\s*=\s*(?<right_hand>\d+)");
            Match m4 = reg4.Match(s);

            Regex reg5 = new Regex(@"(?<left_hand>\d+)\s*=\s*=\s*(?<right_hand>\d+)");
            Match m5 = reg5.Match(s);

            while(m1.Length > 0 || m2.Length > 0 || m3.Length > 0 || m4.Length > 0 || m5.Length > 0)
            {
                if(m1.Length > 0)
                {
                    int left_hand = int.Parse(m1.Groups["left_hand"].Value);
                    int right_hand = int.Parse(m1.Groups["right_hand"].Value);
                    if(left_hand < right_hand) TrueOrFalse = "true";
                    else TrueOrFalse = "false";
                    s = reg1.Replace(s,TrueOrFalse,1);
                    m1 = reg1.Match(s);
                }
                if(m2.Length > 0)
                {
                    int left_hand = int.Parse(m2.Groups["left_hand"].Value);
                    int right_hand = int.Parse(m2.Groups["right_hand"].Value);
                    if(left_hand > right_hand) TrueOrFalse = "true";
                    else TrueOrFalse = "false";
                    s = reg2.Replace(s,TrueOrFalse,1);
                    m2 = reg2.Match(s);
                }
                if(m3.Length > 0)
                {
                    int left_hand = int.Parse(m3.Groups["left_hand"].Value);
                    int right_hand = int.Parse(m3.Groups["right_hand"].Value);
                    if(left_hand <= right_hand) TrueOrFalse = "true";
                    else TrueOrFalse = "false";
                    s = reg3.Replace(s,TrueOrFalse,1);
                    m3 = reg3.Match(s);
                }
                if(m4.Length > 0)
                {
                    int left_hand = int.Parse(m4.Groups["left_hand"].Value);
                    int right_hand = int.Parse(m4.Groups["right_hand"].Value);
                    if(left_hand >= right_hand) TrueOrFalse = "true";
                    else TrueOrFalse = "false";
                    s = reg4.Replace(s,TrueOrFalse,1);
                    m4 = reg4.Match(s);
                }
                if(m5.Length > 0)
                {
                    int left_hand = int.Parse(m5.Groups["left_hand"].Value);
                    int right_hand = int.Parse(m5.Groups["right_hand"].Value);
                    if(left_hand == right_hand) TrueOrFalse = "true";
                    else TrueOrFalse = "false";
                    s = reg5.Replace(s,TrueOrFalse,1);
                    m5 = reg5.Match(s);
                }
            }
            reg = new Regex(@"\((?<result>true|false)\)");
            mat = reg.Match(s);
            while(mat.Length > 0)
            {
                s = reg.Replace(s,mat.Groups["result"].Value,1);
                mat = reg.Match(s);
            }

            reg = new Regex(@"\(.+\)");
            mat = reg.Match(s);
            while(mat.Length > 0)
            {
                string insideParentheses = FourOperations(mat.Value);
                SizeComparing(insideParentheses);
                mat = reg.Match(s);
            }

            //「かつ」優先
            reg = new Regex(@"(?<left>true|false)\&\&(?<right>true|false)");
            mat = reg.Match(s);
            while(mat.Length > 0)
            {
                string result = "";
                if(mat.Groups["left"].Value == "true" && mat.Groups["right"].Value == "true") result = "true";
                else result = "false";
                s = reg.Replace(s,result,1);
                mat = reg.Match(s);
            }
            //「または」
            reg = new Regex(@"(?<left>true|false)\|\|(?<right>true|false)");
            mat = reg.Match(s);
            while(mat.Length > 0)
            {
                string result = "";
                if(mat.Groups["left"].Value == "true" || mat.Groups["right"].Value == "true") result = "true";
                else result = "false";
                s = reg.Replace(s,result,1);
                mat = reg.Match(s);
            }
            //おわり
            reg = new Regex(@"^true|false$");
            mat = reg.Match(s);
            if(mat.Length > 0)
            {
                return mat.Value;
            }
            return "false";
        }
        #endregion

        #region 使ってない
        //消すのが惜しかった
        public static int CountChar(string s,char c)
        {
            return s.Length - s.Replace(c.ToString(),"").Length;
        }
        //ノーマルかっこと閉じかっこ、中かっこと中閉じかっこの数が同じかどうか(いらない気がしてきた)
        public static bool kakkocounter(ArrayList sArray)
        {
            int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
            for(int i = 0;i < sArray.Count;i++)
            {
                count1 += CountChar(sArray[i].ToString(),'(');
                count2 += CountChar(sArray[i].ToString(),')');
                count3 += CountChar(sArray[i].ToString(),'{');
                count4 += CountChar(sArray[i].ToString(),'}');
            }
            if(count1 == count2 && count3 == count4) return true;
            return false;
        }
        #endregion


    }
}
