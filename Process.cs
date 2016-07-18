using System;

namespace HackTheWorld
{
    /// <summary>
    /// IEditable なオブジェクトに対する操作を保存する最小単位。
    /// </summary>
    public delegate void ExecuteWith(IEditable obj, float dt);
    public delegate Func<bool> Trigger(IEditable obj);

    /// <summary>
    /// IEditable なオブジェクトの Processes に格納され、
    /// どのメソッドをどれくらいの時間実行するかを保存する。
    /// </summary>
    public class Process
    {
        /// <summary>
        /// 実行する時間
        /// </summary>
        public float MilliSeconds { get; }
        /// <summary>
        /// 経過時間
        /// </summary>
        public float ElapsedTime { get; set; }
        /// <summary>
        /// 実行するメソッド
        /// </summary>
        public ExecuteWith ExecuteWith { get; }
        public Callback Callback { get; }

        /// <summary>
        /// 一度だけ実行するメソッドを格納する Process を生成する。
        /// </summary>
        public Process(ExecuteWith executeWith)
        {
            MilliSeconds = 0;
            ElapsedTime = 0;
            ExecuteWith = executeWith;
        }

        /// <summary>
        /// executeWith に 保存されたメソッドを、seconds に指定された秒数実行する。
        /// </summary>
        public Process(ExecuteWith executeWith, float seconds)
        {
            MilliSeconds = seconds * 1000;
            ElapsedTime = 0;
            ExecuteWith = executeWith;
        }

        public Process(ExecuteWith executeWith, Callback callback)
        {
            MilliSeconds = 0;
            ElapsedTime = 0;
            ExecuteWith = executeWith;
            Callback = callback;
        }

        public Process(ExecuteWith executeWith, float seconds, Callback callback)
        {
            MilliSeconds = seconds * 1000;
            ElapsedTime = 0;
            ExecuteWith = executeWith;
            Callback = callback;
        }

    }

    public class Callback
    {
        public Trigger Trigger { get; }
        public ExecuteWith ExecuteWith { get; }

        public Callback(Trigger trigger, ExecuteWith executeWith)
        {
            Trigger = trigger;
            ExecuteWith = executeWith;
        }
    }

}
