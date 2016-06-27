﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;
using System.Text.RegularExpressions;


namespace HackTheWorld
{
    public static class CodeParser
    {


        public static ArrayList yomitori(string s1)
        {
            Hashtable hash = new Hashtable();

            ICollection valuecall = hash.Values;
            //連続で入力してデバックしたいからいる奴ら
            hash.Clear();

            //行で分割
            //char[ ] delimiterChars = { ' ' , ':' , '\t' , '\n' };
            char[] delimiterChars = { '\n' };

            //分割した文を入れるリストと結果を入れるリスト
            ArrayList sArray = new ArrayList();
            ArrayList result = new ArrayList();


            string[] s2 = s1.Split(delimiterChars);
            for(int i = 0;i < s2.Length;i++)
            {
                if(s2[i] != "") sArray.Add(s2[i]);
            }

            string str = "";
            if(!checkfunction(sArray))
            {
                result.Clear();
                result.Add("へんな書きかた");
                str = "";
                for(int i = 0;i < result.Count;i++)
                {
                    str += (string)result[i] + "\n";
                }
                MessageBox.Show(str);
                return result;
            }
            if(!isValidScript(sArray))
            {

                result.Clear();
                result.Add("構文エラー");
                str = "";
                for(int i = 0;i < result.Count;i++)
                {
                    str += (string)result[i] + "\n";
                }
                MessageBox.Show(str);
                return result;
            }
            //割り振る
            warifuri(sArray,result,hash);

            str = "";
            for(int i = 0;i < result.Count;i++)
            {
                str += (string)result[i] + "\n";
            }
            MessageBox.Show(str);
            return result;
        }


        public static bool isValidScript(ArrayList sArray)
        {
            int countfunction = 0;
            int countend = 0;
            int kakko = 0;
            for(int i = 0;i < sArray.Count;i++)
            {
                //関数の数を数える(今はforとif)
                if(firstfor(sArray,i) || firstif(sArray,i) || firstwhile(sArray,i)) countfunction++;
                if(firstend(sArray,i)) countend++;
            }
            if(countfunction != countend)
            {
                Console.WriteLine("関数とendの数が違う");
            }
            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                int j = 0;
                //i行目がforで始まってるかどうか
                if(firstfor(sArray,i))
                {
                    kakko++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        //endがいたらへらす
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            if(!boolfor(sArray,i))
                            {
                                MessageBox.Show("forとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countfunction--;
                            break;
                        }
                    }

                }
                //似たようなもん
                if(firstif(sArray,i))
                {
                    kakko++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            if(!boolif(sArray,i))
                            {
                                MessageBox.Show("ifとendはいるみたいだけど\n文の中身が違う");
                                return false;
                            }
                            countfunction--;
                            break;
                        }
                    }
                }
                if(firstwhile(sArray,i))
                {
                    kakko++;
                    for(j = i + 1;j < sArray.Count;j++)
                    {
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            countfunction--;
                            break;
                        }
                    }
                }
                if(kakko != 0)
                {
                    Console.WriteLine("end多い");
                    return false;
                }
                //関数っぽいのが全部なくなったら
                if(countfunction == 0) return true;

            }
            //ループ回り切ったらだめ
            MessageBox.Show("関数とendの対応がだめ");
            return false;
        }

        public static bool checkfunction(ArrayList sArray)
        {
            string message = "";
            string s = "";
            int count = 0;
            int size = 14;
            //"size,1,1", "wait,1", "move,1,1,2"
            Regex[] reg = new Regex[size];
            reg[0] = new Regex(@"\s*size\s*,\s*[\w+|\+|\-|\*|\/]+\s*,\s*[\w+|\+|\-|\*|\/]+");
            reg[1] = new Regex(@"\s*wait\s*,\s*\w+");
            reg[2] = new Regex(@"\s*move\s*,\s*[\w+|\+|\-|\*|\/]+\s*,\s*[\w+|\+|\-|\*|\/]+,\s*[\w+|\+|\-|\*|\/]+");
            reg[3] = new Regex(@"\s*\w+\s*=\s*[\w+|\+|\-|\*|\/]+\s*");
            reg[4] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*=");
            reg[5] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\+");
            reg[6] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\-");
            reg[7] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\+\=");
            reg[8] = new Regex(@"\s*(?<name>[a-zA-z]+)\s*\-\=");
            reg[9] = new Regex(@"for");
            reg[10] = new Regex(@"if");
            reg[11] = new Regex(@"while");
            reg[12] = new Regex(@"end");
            reg[13] = new Regex(@"else");




            Match[] mat = new Match[size];

            for(int i = 0;i < sArray.Count;i++)
            {
                count = 0;
                s = sArray[i].ToString();
                for(int j = 0;j < size;j++)
                {
                    mat[j] = reg[j].Match(s);
                }

                if(s.Contains("size") && mat[0].Length == 0) message = "size";
                if(s.Contains("wait") && mat[1].Length == 0) message = "wait";
                if(s.Contains("move") && mat[2].Length == 0) message = "move";

                for(int j = 0;j < size;j++)
                {
                    if(mat[j].Length != 0) count++;
                }
                if(message.Length != 0)
                {
                    MessageBox.Show(message + "の書き方がまちがってます");
                    return false;
                }
                if(count == 0)
                {
                    MessageBox.Show("知らない形の文が" + (i + 1) + "行目にあります");
                    return false;
                }
            }
            return true;
        }
        //最初が何で始まるかわかるといちいち書かなくていいからべんり
        public static int bunki(ArrayList sArray,int i)
        {
            if(firstfor(sArray,i)) return 1;
            if(firstif(sArray,i)) return 2;
            if(firstwhile(sArray,i)) return 3;
            return 0;
        }


        public static void warifuri(ArrayList sArray,ArrayList result,Hashtable hash)
        {
            //基本1行ずつ読む
            for(int i = 0;i < sArray.Count;i++)
            {
                Has(sArray,i,hash);
                //i行目が関数で始まってるかどうか
                switch(bunki(sArray,i))
                {
                    case 1:
                        For(sArray,result,i,hash);
                        int kakko = 1;
                        int k = 0;
                        for(k = i + 1;k < sArray.Count;k++)
                        {
                            if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                            if(firstend(sArray,k)) kakko--;
                            if(kakko == 0)
                            {
                                break;
                            }
                        }
                        i = k;
                        break;
                    case 2:
                        If(sArray,result,i,hash);
                        kakko = 1;
                        k = 0;
                        for(k = i + 1;k < sArray.Count;k++)
                        {
                            if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                            if(firstend(sArray,k)) kakko--;
                            if(kakko == 0)
                            {
                                break;
                            }
                        }
                        i = k;
                        break;
                    case 3:
                        While(sArray,result,i,hash);
                        kakko = 1;
                        k = 0;
                        for(k = i + 1;k < sArray.Count;k++)
                        {
                            if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                            if(firstend(sArray,k)) kakko--;
                            if(kakko == 0)
                            {
                                break;
                            }
                        }
                        i = k;
                        break;
                    default:
                        dainyu(sArray,i,hash);
                        result.Add(sArray[i]);
                        break;
                }
            }
            //見た目だけの話
            ArrayList result2 = new ArrayList();
            for(int i = 0;i < result.Count;i++)
            {
                string s = (string)result[i];
                if(!s.Contains("=") && !s.Contains("+")) result2.Add(result[i]);
            }
            result.Clear();
            for(int i = 0;i < result2.Count;i++)
            {
                result.Add(result2[i]);
            }
        }
        public static Tuple<int,int>[] forset(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] s2 = s.Split('\n');
            for(int i = 0;i < s2.Length;i++)
            {
                if(s2[i] != "") sArray.Add(s2[i]);
            }

            ArrayList forArray = new ArrayList();

            int kakko = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(firstfor(sArray,i))
                {
                    kakko++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        //endがいたらへらす
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            forArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] t = new Tuple<int,int>[forArray.Count];
            for(int i = 0;i < forArray.Count;i++)
            {
                t[i] = (Tuple<int,int>)forArray[i];
            }
            return t;
        }
        public static Tuple<int,int>[] ifset(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] s2 = s.Split('\n');
            for(int i = 0;i < s2.Length;i++)
            {
                if(s2[i] != "") sArray.Add(s2[i]);
            }
            ArrayList ifArray = new ArrayList();

            int kakko = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(firstif(sArray,i))
                {
                    kakko++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        //endがいたらへらす
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            //tupleの中に行番号を入れる
                            ifArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] t = new Tuple<int,int>[ifArray.Count];
            for(int i = 0;i < ifArray.Count;i++)
            {
                t[i] = (Tuple<int,int>)ifArray[i];
            }
            return t;
        }
        public static Tuple<int,int>[] whileset(string s)
        {
            ArrayList sArray = new ArrayList();
            string[] s2 = s.Split('\n');
            for(int i = 0;i < s2.Length;i++)
            {
                if(s2[i] != "") sArray.Add(s2[i]);
            }
            ArrayList whileArray = new ArrayList();

            int kakko = 0;

            //初めのほうから順番に見ていく
            for(int i = 0;i < sArray.Count;i++)
            {
                //i行目がforで始まってるかどうか
                if(firstwhile(sArray,i))
                {
                    kakko++;
                    for(int j = i + 1;j < sArray.Count;j++)
                    {
                        //閉じる前にまた関数っぽいのがいたらカウント増やす
                        if(firstfor(sArray,j) || firstif(sArray,j) || firstwhile(sArray,j)) kakko++;
                        //endがいたらへらす
                        if(firstend(sArray,j)) kakko--;
                        if(kakko == 0)
                        {
                            //tupleの中に行番号を入れる
                            whileArray.Add(new Tuple<int,int>(i,j));
                            break;
                        }
                    }
                }
            }
            Tuple<int,int>[] t = new Tuple<int,int>[whileArray.Count];
            for(int i = 0;i < whileArray.Count;i++)
            {
                t[i] = (Tuple<int,int>)whileArray[i];
            }
            return t;
        }
        public static Tuple<int,int>[] allset(string s)
        {
            ArrayList allArray = new ArrayList();

            Tuple<int,int>[] t1 = forset(s);
            Tuple<int,int>[] t2 = ifset(s);
            Tuple<int,int>[] t3 = whileset(s);

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

            Tuple<int,int>[] t = new Tuple<int,int>[allArray.Count];
            for(int i = 0;i < allArray.Count;i++)
            {
                t[i] = (Tuple<int,int>)allArray[i];
            }
            return t;
        }
        public static void Has(ArrayList sArray,int i,Hashtable hash)
        {
            string m1, m2, m3;
            ICollection keycall = hash.Keys;
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>\w+)\s*=\s*(?<uhen>[(?<value>\w+)|\+|\-|\*|\/|\.]+)\s*"))
            {
                string s = (string)sArray[i];
                Regex reg = new Regex(@"\s*(?<name>\w+)\s*=\s*(?<uhen>.*)");
                Match mat = reg.Match(s);
                m1 = mat.Groups["uhen"].Value;

                foreach(string k in keycall)
                {
                    if(m1.Contains(k))
                    {
                        string pattern = k;
                        string replacement = hash[k].ToString();
                        Regex r = new Regex(pattern);
                        s = r.Replace(s,replacement);
                    }
                }
                if(m1.Contains(@"\w+")) return;

                reg = new Regex(@"\s*(?<name>\w+)\s*=\s*(?<uhen>[\d+|\+|\-|\*|\/|\.]+)\s*");
                mat = reg.Match(s);
                m2 = mat.Groups["uhen"].Value;
                m2 = FourOperations(m2);

                m3 = mat.Groups["name"].Value;
                hash[m3] = m2;
                return;

            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\+\+"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\+");
                Match m = r.Match((string)sArray[i]);
                m1 = m.Groups["name"].Value;
                hash[m1] = Convert.ToInt32(hash[m1]) + 1;
                return;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\-\-"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\-\-");
                Match m = r.Match((string)sArray[i]);
                m1 = m.Groups["name"].Value;
                hash[m1] = Convert.ToInt32(hash[m1]) - 1;
                return;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)");
                Match m = r.Match((string)sArray[i]);
                m1 = m.Groups["name"].Value;
                m2 = m.Groups["value"].Value;
                hash[m1] = Convert.ToInt32(hash[m1]) + int.Parse(m2);
                return;
            }
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[i],@"\s*(?<name>[a-zA-z]+)\s*\-\=\s*(?<value>\d+)"))
            {
                Regex r = new Regex(@"(?<name>[a-zA-z]+)\s*\+\=\s*(?<value>\d+)");
                Match m = r.Match((string)sArray[i]);
                m1 = m.Groups["name"].Value;
                m2 = m.Groups["value"].Value;
                hash[m1] = Convert.ToInt32(hash[m1]) - int.Parse(m2);
                return;
            }

        }

        public static void dainyu(ArrayList sArray,int x,Hashtable hash)
        {
            ICollection keycall = hash.Keys;
            //hashになにか入ってたら
            if(keycall.Count > 0)
            {
                //hashのkeyごとに
                foreach(string k in keycall)
                {
                    string input = (string)sArray[x];
                    int foundIndex = input.IndexOf(k);
                    
                    while(0 <= foundIndex)
                    {
                        bool booldainyu = true;
                        string s;

                        //余計な文字がついている
                        if(foundIndex == 0)
                        {
                            s = input.Substring(foundIndex,k.Length + 1);
                            if(Regex.IsMatch(s,@"[a-zA-Z]$")) booldainyu = false;
                        }
                        else if(foundIndex!=input.Length-1)
                        {
                            s = input.Substring(foundIndex - 1,k.Length + 2);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]")|| Regex.IsMatch(s,@"[a-zA-Z]$")) booldainyu = false;
                        }
                        else
                        {
                            s = input.Substring(foundIndex-1,k.Length+1);
                            if(Regex.IsMatch(s,@"^[a-zA-Z]")) booldainyu = false;
                        }
                        if(booldainyu)
                        {
                            char[] c = input.ToCharArray();
                            char[] ckey = hash[k].ToString().ToCharArray();
                            List<char> clist = new List<char>();
                            if(foundIndex != 0)
                            {
                                for(int i = 0;i < foundIndex;i++)
                                {
                                    clist.Add(c[i]);
                                }
                            }
                            for(int i = 0;i < ckey.Length;i++)
                            {
                                clist.Add(ckey[i]);
                            }
                            if(foundIndex+k.Length<c.Length)
                            {
                                for(int i = foundIndex + k.Length;i < c.Length;i++)
                                {
                                    clist.Add(c[i]);
                                }
                            }
                            string converteds="";
                            for(int i = 0;i < clist.Count;i++)
                            {
                                converteds += clist[i];
                            }
                            sArray[x] = converteds;
                        }
                        input = (string)sArray[x];
                        //次の検索開始位置
                        int nextIndex = foundIndex + k.Length;
                        if(nextIndex < input.Length)
                        {
                            //次の位置を探す
                            foundIndex = input.IndexOf(k,nextIndex);
                        }
                        else
                        {
                            //最後まで検索したときは終わる
                            break;
                        }
                        
                    }
                    
                }
            }




        }




        public static void For(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {

            int type = 0;
            //sArray[home]はforから始まっていて繰り返し回数を指定している行
            //どんな書き方をしているかの正規表現を用いた場合分けをしたい
            //いちいち間に\s*(0個以上の空白文字を示す)を入れて間に空白が入っても読めるようにする
            //typeを3つ作ることにする

            //for(i=0;i<5;i++)がtype1
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|(<=)|(>=)|(==)" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)")) type = 1;
            //for i=0 to 3がtype2
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+\s*=\s*\w+\s*to\s*\w+")) type = 2;
            //for 2とかをtype3とする
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"for\s*\w+")) type = 3;

            switch(type)
            {
                case 1:
                    Regex re1_1 = new Regex(@"(?<start>\w+\s*\=\s*\w+)");
                    Regex re1_2 = new Regex(@"(?<jouken>\w+\s*(<|>|(<=)|(>=)|(==))\s*\w+)");
                    Regex re1_3 = new Regex(@"(?<update>\w+(\+\+)|(\-\-)|(\+=\w+)|(\-=\w+))");
                    Match m1_1 = re1_1.Match((string)sArray[home]);
                    Match m1_2 = re1_2.Match((string)sArray[home]);
                    Match m1_3 = re1_3.Match((string)sArray[home]);
                    ArrayList tArray = new ArrayList();
                    ArrayList uArray = new ArrayList();
                    bool yesbreak = false;

                    //homeまで読む(条件取り終わったから)
                    Has(sArray,home,hash);


                    //初期条件をHasにぶち込んでリセット
                    tArray.Add(m1_1.Groups["start"].Value);
                    Has(tArray,0,hash);

                    //判定式に入れるとき数字＜数字とかになってないと困るじゃん、と思った
                    uArray.Add(m1_2.Groups["jouken"].Value);
                    dainyu(uArray,0,hash);
                    //ここからループ
                    while(hantei((string)uArray[0]))
                    {
                        //reset
                        tArray.Clear();
                        int i = 1;
                        int kakko = 1;
                        while(true)
                        {
                            if(firstfor(sArray,home + i) || firstif(sArray,home + i) || firstwhile(sArray,home + i)) kakko++;
                            if(firstend(sArray,home + i)) kakko--;

                            tArray.Add(sArray[home + i]);

                            if(kakko == 0) break;
                            i++;

                        }
                        tArray.Insert(tArray.Count - 1,m1_3.Groups["update"].Value);
                        //1行ずつ読む
                        for(i = 0;i < tArray.Count;i++)
                        {
                            while(!firstend(tArray,i))
                            {
                                //home+jまで同じことをする
                                Has(tArray,i,hash);


                                switch(bunki(tArray,i))
                                {
                                    case 1:
                                        For(tArray,result,i,hash);
                                        kakko = 1;
                                        int k = 0;
                                        for(k = i + 1;k < tArray.Count;k++)
                                        {
                                            if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                            if(firstend(tArray,k)) kakko--;
                                            if(kakko == 0)
                                            {
                                                break;
                                            }
                                        }
                                        i = k + 1;
                                        break;
                                    case 2:
                                        If(tArray,result,i,hash);
                                        kakko = 1;
                                        k = 0;
                                        for(k = i + 1;k < tArray.Count;k++)
                                        {
                                            if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                            if(firstend(tArray,k)) kakko--;
                                            if(kakko == 0)
                                            {
                                                break;
                                            }
                                        }
                                        i = k + 1;
                                        break;
                                    case 3:
                                        While(tArray,result,i,hash);
                                        kakko = 1;
                                        k = 0;
                                        for(k = i + 1;k < tArray.Count;k++)
                                        {
                                            if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                            if(firstend(tArray,k)) kakko--;
                                            if(kakko == 0)
                                            {
                                                break;
                                            }
                                        }
                                        i = k + 1;
                                        break;
                                    default:
                                        dainyu(tArray,i,hash);
                                        result.Add(tArray[i]);
                                        i++;
                                        break;
                                }
                                if(result.Count != 0)
                                {
                                    if(result[result.Count - 1].ToString() == "break")
                                    {
                                        yesbreak = true;
                                        result.RemoveAt(result.Count - 1);
                                        break;
                                    }
                                }
                            }
                            if(yesbreak) break;
                        }
                        if(yesbreak) break;
                        uArray.Clear();
                        uArray.Add(m1_2.Groups["jouken"].Value);
                        dainyu(uArray,0,hash);
                    }
                    return;
                case 3:
                    Regex re3 = new Regex(@"for\s*(?<repeat>\w+)");
                    Match m3 = re3.Match((string)sArray[home]);
                    int n = 0;
                    if(hash.ContainsKey(m3.Groups["repeat"].Value)) n = Convert.ToInt32(hash[m3.Groups["repeat"].Value]);
                    else if(!int.TryParse((string)m3.Groups["repeat"].Value,out n))
                    {
                        MessageBox.Show("(For type3)数字代入してますか？");
                        return;
                    }
                    yesbreak = false;
                    tArray = new ArrayList();
                    tArray.Clear();

                    //homeまで読んでhash登録、代入、forとendの対応の取り直し
                    Has(sArray,home,hash);
                    for(int i = 0;i < n;i++)
                    {
                        int j = 1;
                        int kakko = 1;
                        tArray.Clear();
                        while(true)
                        {
                            if(firstfor(sArray,home + j) || firstif(sArray,home + j) || firstwhile(sArray,home + j)) kakko++;
                            if(firstend(sArray,home + j)) kakko--;

                            tArray.Add(sArray[home + j]);

                            if(kakko == 0) break;
                            j++;

                        }
                        j = 0;
                        while(!firstend(tArray,j))
                        {
                            //home+jまで同じことをする
                            Has(tArray,j,hash);

                            switch(bunki(tArray,j))
                            {
                                case 1:
                                    For(tArray,result,j,hash);
                                    kakko = 1;
                                    int k = 0;
                                    for(k = j + 1;k < tArray.Count;k++)
                                    {
                                        if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                        if(firstend(tArray,k)) kakko--;
                                        if(kakko == 0)
                                        {
                                            break;
                                        }
                                    }
                                    j = k + 1;
                                    break;
                                case 2:
                                    If(tArray,result,j,hash);
                                    kakko = 1;
                                    k = 0;
                                    for(k = j + 1;k < tArray.Count;k++)
                                    {
                                        if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                        if(firstend(tArray,k)) kakko--;
                                        if(kakko == 0)
                                        {
                                            break;
                                        }
                                    }
                                    j = k + 1;
                                    break;
                                case 3:
                                    While(tArray,result,j,hash);
                                    kakko = 1;
                                    k = 0;
                                    for(k = j + 1;k < tArray.Count;k++)
                                    {
                                        if(firstfor(tArray,k) || firstif(tArray,k) || firstwhile(tArray,k)) kakko++;
                                        if(firstend(tArray,k)) kakko--;
                                        if(kakko == 0)
                                        {
                                            break;
                                        }
                                    }
                                    j = k + 1;
                                    break;
                                default:
                                    dainyu(tArray,j,hash);
                                    result.Add(tArray[j]);
                                    j++;
                                    break;
                            }
                            if(result.Count != 0)
                            {
                                if(result[result.Count - 1].ToString() == "break")
                                {
                                    yesbreak = true;
                                    result.RemoveAt(result.Count - 1);
                                    break;
                                }
                            }
                        }
                        if(yesbreak) break;
                    }

                    return;
                default:
                    result.Add("ここのfor boolfor通過しといてうまくいってない");
                    return;
            }
        }


        public static void If(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            //homeまで読んでhash登録、代入、forとendの対応の取り直し
            Has(sArray,home,hash);

            ArrayList tArray = new ArrayList();
            //条件を抜き出す
            string s = (string)sArray[home];
            Regex r = new Regex(@"(?<jouken>\w+\s*(<|>|(<=)|(>=)|(==))\s*\d+)");
            Match m = r.Match(s);

            tArray.Add(m.Value);
            dainyu(tArray,0,hash);

            if(hantei((string)tArray[0]))
            {
                int i = 1;
                while(!firstend(sArray,home + i) && !firstbreak(sArray,home + i - 1))
                {
                    if(firstelse(sArray,home + i)) break;
                    //home+iまで同じことをする
                    Has(sArray,home + i,hash);
                    dainyu(sArray,home + i,hash);
                    switch(bunki(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            int kakko = 1;
                            int k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            kakko = 1;
                            k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            kakko = 1;
                            k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        default:
                            dainyu(sArray,home + i,hash);
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }
            else
            {
                int tmp = 0;
                int i = 0;
                while(true)
                {
                    if(firstelse(sArray,home + tmp))
                    {
                        i = tmp + 1;
                        break;
                    }
                    else tmp++;
                    if(home + tmp >= sArray.Count) return;
                }
                while(!firstend(sArray,home + i) && !firstbreak(sArray,home + i - 1))
                {
                    //home+iまで同じことをする
                    Has(sArray,home + i,hash);

                    switch(bunki(sArray,home + i))
                    {
                        case 1:
                            For(sArray,result,home + i,hash);
                            int kakko = 1;
                            int k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        case 2:
                            If(sArray,result,home + i,hash);
                            kakko = 1;
                            k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        case 3:
                            While(sArray,result,home + i,hash);
                            kakko = 1;
                            k = 0;
                            for(k = home + i + 1;k < sArray.Count;k++)
                            {
                                if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                if(firstend(sArray,k)) kakko--;
                                if(kakko == 0)
                                {
                                    break;
                                }
                            }
                            i = k;
                            break;
                        default:
                            dainyu(sArray,home + i,hash);
                            result.Add(sArray[home + i]);
                            i++;
                            break;
                    }
                }
            }


            return;
        }

        public static void While(ArrayList sArray,ArrayList result,int home,Hashtable hash)
        {
            ArrayList tArray = new ArrayList();
            //条件を抜き出す
            string s = (string)sArray[home];
            Regex r = new Regex(@"(?<jouken>\w+\s*(<|>|(<=)|(>=)|(==))\s*\d+)");
            Match m = r.Match(s);
            bool yesbreak = false;

            //homeまで読んでhash登録、代入、forとendの対応の取り直し
            Has(sArray,home,hash);


            while(!yesbreak)
            {
                //条件式だけを入れてdainyu hanteiに入れられるように
                tArray.Clear();
                tArray.Add(m.Value);
                dainyu(tArray,0,hash);
                if(hantei((string)tArray[0]))
                {
                    //dainyu後はもとの文でリセットしないと4++などはi++扱いされない
                    //(あとでHasじゃないところに目印つけまくる場所を作ればいい気がする)
                    //とりあえずsArray(何回か読むから書き換わると困る)→tArray(dainyu用)
                    tArray.Clear();
                    for(int j = 0;j < sArray.Count;j++)
                    {
                        tArray.Add(sArray[j]);
                    }


                    int i = 1;
                    while(!firstend(sArray,home + i) && !firstbreak(sArray,home + i - 1))
                    {
                        //home+iまで同じことをする
                        Has(sArray,home + i,hash);

                        switch(bunki(sArray,home + i))
                        {
                            case 1:
                                For(sArray,result,home + i,hash);
                                int kakko = 1;
                                int k = 0;
                                for(k = home + i + 1;k < sArray.Count;k++)
                                {
                                    if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                    if(firstend(sArray,k)) kakko--;
                                    if(kakko == 0)
                                    {
                                        break;
                                    }
                                }
                                i = k;
                                break;
                            case 2:
                                If(sArray,result,home + i,hash);
                                kakko = 1;
                                k = 0;
                                for(k = home + i + 1;k < sArray.Count;k++)
                                {
                                    if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                    if(firstend(sArray,k)) kakko--;
                                    if(kakko == 0)
                                    {
                                        break;
                                    }
                                }
                                i = k;
                                break;
                            case 3:
                                While(sArray,result,home + i,hash);
                                kakko = 1;
                                k = 0;
                                for(k = home + i + 1;k < sArray.Count;k++)
                                {
                                    if(firstfor(sArray,k) || firstif(sArray,k) || firstwhile(sArray,k)) kakko++;
                                    if(firstend(sArray,k)) kakko--;
                                    if(kakko == 0)
                                    {
                                        break;
                                    }
                                }
                                i = k;
                                break;
                            default:
                                dainyu(tArray,home + i,hash);
                                result.Add(tArray[home + i]);
                                i++;
                                break;
                        }
                        if(result.Count != 0)
                        {
                            if(result[result.Count - 1].ToString() == "break")
                            {
                                yesbreak = true;
                                result.RemoveAt(result.Count - 1);
                                break;
                            }
                        }
                    }

                }
                else break;
            }
        }




        public static string FourOperations(string s)
        {
            if(System.Text.RegularExpressions.Regex.IsMatch(s,@"\d+|[\+|\-|\*|\/]+") && !s.StartsWith(@"[\+|\-|\*|\/]") && !s.EndsWith(@"[\+|\-|\*|\/]") && !s.Contains(@"[\+\+|\-\-|\*\*|\/\/]"))
            {
                //ここで計算
                System.Data.DataTable dt = new System.Data.DataTable();

                //Type t = dt.Compute(s,"").GetType();

                return dt.Compute(s,"").ToString();
            }
            return "四則演算が変";
        }





        //こっから下で判定してあってるかあってないか出す奴をやりたい

        //文字数のカウントをする
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


        //For()に突っ込んでいいのか判断するためのbool
        public static bool boolfor(ArrayList sArray,int home)
        {
            //一致してるかは知りたいけどうしろに余計なのがついてたらはじきたい
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"^for\s*\(\s*\w+\s*\=\s*\w+\s*;\s*\w+\s*" + @"<|>|<=|>=" + @"\s*\w+\s*;\s*\w+[\+\+|\-\-|\+=\w+|\-=\w+]\)\s*$")) return true;
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*=\s*\w+\s*to\s*\w+\s*$")) return true;
            if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"^for\s*\w+\s*$")) return true;
            return false;
        }


        public static bool boolif(ArrayList sArray,int home)
        {
            string[] re = new string[5];
            re[0] = @"[0-9a-zA-Z]+\s*\<\s*[0-9a-zA-Z]+";
            re[1] = @"[0-9a-zA-Z]+\s*\>\s*[0-9a-zA-Z]+";
            re[2] = @"[0-9a-zA-Z]+\s*\<\s*\=\s*[0-9a-zA-Z]+";
            re[3] = @"[0-9a-zA-Z]+\s*\>\s*\=\s*[0-9a-zA-Z]+";
            re[4] = @"[0-9a-zA-Z]+\s*\=\s*\=\s*[0-9a-zA-Z]+";

            for(int i = 0;i < re.Length;i++)
            {
                if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + re[i] + @"\)\s*"))
                {
                    if(System.Text.RegularExpressions.Regex.IsMatch((string)sArray[home],@"if\s*\(" + re[i] + @"\)\s*."))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }


        //この辺はifの中に書くとき短くしたいからいる
        public static bool firstfor(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("for")) return true;
            return false;
        }
        public static bool firstif(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("if")) return true;
            return false;
        }
        public static bool firstwhile(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("while")) return true;
            return false;
        }
        public static bool firstend(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("end")) return true;
            return false;
        }
        public static bool firstelse(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("else")) return true;
            return false;
        }
        public static bool firstbreak(ArrayList sArray,int i)
        {
            if(sArray[i].ToString().StartsWith("break")) return true;
            return false;
        }

        //ifのための判定群
        public static bool hantei(string s)
        {
            //大小判定したい
            Regex re1 = new Regex(@"(?<sahen>\d+)\s*<\s*(?<uhen>\d+)");
            Match m1 = re1.Match(s);

            Regex re2 = new Regex(@"(?<sahen>\d+)\s*>\s*(?<uhen>\d+)");
            Match m2 = re2.Match(s);

            Regex re3 = new Regex(@"(?<sahen>\d+)\s*<\s*=\s*(?<uhen>\d+)");
            Match m3 = re3.Match(s);

            Regex re4 = new Regex(@"(?<sahen>\d+)\s*>\s*=\s*(?<uhen>\d+)");
            Match m4 = re4.Match(s);

            Regex re5 = new Regex(@"(?<sahen>\d+)\s*=\s*=\s*(?<uhen>\d+)");
            Match m5 = re5.Match(s);

            if(m1.Length > 0)
            {
                int sahen = int.Parse(m1.Groups["sahen"].Value);
                int uhen = int.Parse(m1.Groups["uhen"].Value);
                if(sahen < uhen) return true;
                else return false;
            }
            if(m2.Length > 0)
            {
                int sahen = int.Parse(m2.Groups["sahen"].Value);
                int uhen = int.Parse(m2.Groups["uhen"].Value);
                if(sahen > uhen) return true;
                else return false;
            }
            if(m3.Length > 0)
            {
                int sahen = int.Parse(m3.Groups["sahen"].Value);
                int uhen = int.Parse(m3.Groups["uhen"].Value);
                if(sahen <= uhen) return true;
                else return false;
            }
            if(m4.Length > 0)
            {
                int sahen = int.Parse(m4.Groups["sahen"].Value);
                int uhen = int.Parse(m4.Groups["uhen"].Value);
                if(sahen >= uhen) return true;
                else return false;
            }
            if(m5.Length > 0)
            {
                int sahen = int.Parse(m5.Groups["sahen"].Value);
                int uhen = int.Parse(m5.Groups["uhen"].Value);
                if(sahen == uhen) return true;
                else return false;
            }
            return false;
        }
    }
}
