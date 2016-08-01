using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using static HackTheWorld.Constants;
using System.Collections;
using System.Text.RegularExpressions;

namespace HackTheWorld
{
    /// <summary>
    /// 編集可能インターフェース。
    /// CodeBox で編集するオブジェクトに継承させる。
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public interface IEditable
    {
        /// <summary>
        /// 何番目の Process が実行されているか。
        /// </summary>
        int ProcessPtr { get; set; }
        /// <summary>
        /// 編集時に表示される名前。
        /// </summary>
        [JsonProperty("name", Order = 10, DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("No name")]
        string Name { get; set; }
        /// <summary>
        /// 自身のコード。
        /// </summary>
        [JsonProperty("code", Order = 11, DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        string Code { get; set; }
        /// <summary>
        /// 自身の動作を格納する。
        /// </summary>
        List<Process> Processes { get; set; }
        /// <summary>
        /// true のとき Update() で Process が実行されるようになる。
        /// </summary>
        bool CanExecute { get; set; }

        // GameObject 由来のプロパティ
        float X { get; set; }
        float Y { get; set; }
        float VX { get; set; }
        float VY { get; set; }
        float W { get; set; }
        float H { get; set; }
        float MinX { get; set; }
        float MinY { get; set; }
        float MidX { get; set; }
        float MidY { get; set; }
        float MaxX { get; set; }
        float MaxY { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        Vector Position { get; set; }
        Vector Velocity { get; set; }
        Vector Size { get; set; }
        bool Clicked { get; }
        bool IsAlive { get; }
        void Move(float dt);
        void Rotate(double degree);
        void Accelerate(double a);
        bool Contains(Point p);
        bool Contains(GameObject obj);
        bool Intersects(GameObject obj);
        bool CollidesWith(GameObject obj);
        bool StandOn(GameObject obj);
        bool HitHeadOn(GameObject obj);
        bool Nearby(GameObject obj);
        bool InWindow();
    }

    static partial class Extensions
    {
        public static void SetProcesses(this IEditable self, Process[] processes)
        {
            self.Processes = processes.ToList();
        }

        public static void SetProcesses(this IEditable self, List<Process> processes)
        {
            self.Processes = processes;
        }

        public static void AddProcess(this IEditable self, Process process)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(process);
        }

        public static void AddProcess(this IEditable self, ExecuteWith executeWith, float seconds)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(new Process(executeWith, seconds));
        }

        public static void AddProcess(this IEditable self, ExecuteWith executeWith)
        {
            if (self.Processes == null) self.Processes = new List<Process>();
            self.Processes.Add(new Process(executeWith));
        }

        public static void Update(this IEditable self, float dt)
        {
            if (!self.CanExecute || self.Processes == null || self.Processes.Count == 0) return;

            var process = self.Processes[self.ProcessPtr];
            if (process.ElapsedTime * 1000 <= process.MilliSeconds)
            {
                process.ExecuteWith(self, dt);
                process.ElapsedTime += dt;
                if (process.Callback != null)
                {
                    if (process.Callback.Trigger(self)())
                    {
                        process.Callback.ExecuteWith(self, dt);
                    }
                }
            }
            else if (self.ProcessPtr + 1 < self.Processes.Count)
            {
                self.ProcessPtr++;
            }
        }

        public static void Execute(this IEditable self)
        {
            self.ProcessPtr = 0;
            self.CanExecute = true;
        }

        public static void Compile(this IEditable self, Stage stage)
        {
            string str = self.Code;
            // ここにstring型をProcess型に変換する処理を書く。
            // CodeParserで生成されたArrayListの中身は<size,1,1><wait,1><move,1,1,2>の形

            //CodeParser.ConvertCodebox(str);

            #region CodeParser.ConvertCodebox(str)をProcess型に変換する処理


            //以下のリストの中身("move, x, y")を小集合とする

            //動作テスト用配列
            //            var procedure = new List<string> { "size,1,1", "wait,1", "move,1,1,2", "touch,jump" };
            //            var procedure = new List<string> { "wait,2", "touch,move,1,1,1", "ontop,jump", "nearby,shoot" };


            //ぺいぺいすまんな
            //開いた結果を表示したい
            var expansioned = CodeParser.ConvertCodebox(str);
            //ここでなんか表示する

            var procedure = ConvertDirectionToInt(expansioned).ToList();
            //本実行用配列
            //var procedure = CodeParser.ConvertCodebox(str).Cast<string>().ToList();

            //各小集合に対して、以下の分割処理を行う。
            foreach (var elements in procedure)
            {
                //小集合を要素に分割して、要素数1-4程度の配列を作成
                string[] commands = elements.Split(',');

                Callback callback;
                switch (commands[0])
                {
                    case "touch":
                        switch (commands[1])
                        {
                            //プレイヤーが触れたら大きさを変更
                            case "size":
                                callback = new Callback(obj => () => obj.Intersects(stage.Player), 
                                                       (obj, dt) => { self.Size = new Vector(float.Parse(commands[2]), float.Parse(commands[3]));
                                });
                                self.AddOnce(callback);
                                break;

                            case "jump":
                                callback = new Callback(obj => () => obj.HitHeadOn(stage.Player), (obj, dt) => { stage.Player.VY = -CellSize * 15; });
                                self.AddOnce(callback);
                                break;

                            //プレイヤーが触れたら待機
                            case "wait":
                                self.AddWait(float.Parse(commands[2]));
                                break;

                            //プレイヤーが触れたら移動
                            case "move":
                                self.AddMove(float.Parse(commands[2]), float.Parse(commands[3]), float.Parse(commands[4]));
                                break;
                        }
                        break;
                    case "ontop":
                        switch (commands[1])
                        {
                            case "jump":
                                callback = new Callback(obj => () => obj.HitHeadOn(stage.Player), (obj, dt) => { stage.Player.VY = -CellSize * 15; });
                                self.AddOnce(callback);
                                break;
                        }
                        break;
                    case "nearby":
                        switch (commands[1])
                        {
                            case "jump":
                                callback = new Callback(obj => () => obj.Nearby(stage.Player), (obj, dt) => { stage.Player.VY = -CellSize * 15; });
                                self.AddOnce(callback);
                                break;
                            case "move":
                                self.AddMove(float.Parse(commands[2]), float.Parse(commands[3]), float.Parse(commands[4]));
                                break;
                            case "shoot":
                                callback = new Callback(obj => () => obj.Nearby(stage.Player), (obj, dt) =>
                                {
                                    var b = new Bullet(self.X, self.MidY, -50, 0, 10, 10);
                                    stage.Bullets.Add(b);
                                    stage.Objects.Add(b);
                                });
                                self.AddOnce(callback);
                                break;
                        }
                        break;
                    case "wait":
                        self.AddWait(float.Parse(commands[1]));
                        break;
                    case "move":
                        self.AddMove(float.Parse(commands[1]), float.Parse(commands[2]), float.Parse(commands[3]));
                        break;

                }

            }

            #endregion
        }

        public static void AddWait(this IEditable self, float sec)
        {
            self.AddProcess(new Process((obj, dt) =>
            {
                obj.VX = 0.0f;
                obj.VY = 0.0f;
            }));
            self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, sec));
        }

        public static void AddMove(this IEditable self, float vx, float vy, float sec)
        {
            self.AddChangeVelocity(vx, vy);
            self.AddProcess((obj, dt) => { obj.Move(dt); }, sec);
            self.AddStop();
        }

        public static void AddOnce(this IEditable self, Callback callback)
        {
            self.AddProcess(new Process((obj, dt) => { }, callback));
        }

        public static void AddWait(this IEditable self, float sec, Callback callback)
        {
            self.AddStop();
            self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, sec, callback));
        }

        public static void AddMove(this IEditable self, float vx, float vy, float sec, Callback callback)
        {
            self.AddChangeVelocity(vx, vy);
            self.AddProcess(new Process((obj, dt) => { obj.Move(dt); }, sec, callback));
            self.AddStop();
        }

        public static void AddStop(this IEditable self)
        {
            self.AddProcess(new Process((obj, dt) =>
            {
                obj.VX = 0.0f;
                obj.VY = 0.0f;
            }));
        }

        public static void AddChangeVelocity(this IEditable self, float vx, float vy)
        {
            self.AddProcess(new Process((obj, dt) =>
            {
                obj.VX = CellSize * vx;
                obj.VY = -CellSize * vy;
            }));
        }

        public static void AddChangeSize(this IEditable self, float width, float height)
        {
            self.AddProcess((obj, dt) =>
            {
                obj.W = CellSize * width;
                obj.H = CellSize * height;
            });
        }

        static IEnumerable<string> ConvertDirectionToInt(ArrayList sArray)
        {
            string result = "";
            

            for(int i = 0;i < sArray.Count;i++)
            {
                string s = (string)sArray[i];
                if(s.StartsWith("size"))
                {
                    Regex reg = new Regex(@"\s*size\s*\(\s*(?<a>[\d|\.]+)\s*,\s*(?<b>[\d|\.]+)\)");
                    Match mat = reg.Match(s);
                    result = "size," + mat.Groups["a"].Value + "," + mat.Groups["b"].Value;
                    sArray[i] = result;
                }
                if(Regex.IsMatch(s,@"^(player.ontop,|player.nearby,|player.touch,)*wait"))
                {
                    Regex reg = new Regex(@"(?<head>\s*.*,*wait)\s*\(\s*(?<a>[\d|\.]+)\s*\)");
                    Match mat = reg.Match(s);
                    result = mat.Groups["head"].Value + "," + mat.Groups["a"].Value;
                    sArray[i] = result;
                }
                if(Regex.IsMatch(s,@"^(player.ontop,|player.nearby,|player.touch,)*move"))
                {

                    Regex reg = new Regex(@"(?<head>\s*.*,*move)\s*\(\s*(?<a>\w+)");
                    Match mat = reg.Match(s);
                    result = mat.Groups["head"].Value + ",";

                    switch(mat.Groups["a"].Value.ToString())
                    {
                        //0123→右下左上
                        case "right":
                            result += "1,0,1";
                            break;
                        case "down":
                            result += "0,-1,1";
                            break;
                        case "left":
                            result += "-1,0,1";
                            break;
                        case "up":
                            result += "0,1,1";
                            break;
                    }
                    sArray[i] = result;
                }
            }
            return sArray.Cast<string>();
        }
        
    }

}
