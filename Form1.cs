﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HackTheWorld.Constants;



namespace HackTheWorld
{

    public partial class Form1 : Form
    {
        private Bitmap _bmp;
        private LinkedList<Keys> _pressedKeys;
        private LinkedList<MouseButtons> _mouseButtons;

        public Form1()
        {
            InitializeComponent();
   
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;//サイズの固定

            Shown += (sender, e) => { Task.Run(() => { MainProcess(); }); };

        }


        private void MainProcess()
        {
            _bmp = new Bitmap(ScreenWidth, ScreenHeight);

            _pressedKeys = new LinkedList<Keys>();
            _mouseButtons = new LinkedList<MouseButtons>();

            GraphicsContext = Graphics.FromImage(_bmp);
            Scene.Current = new TitleScene();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            long prevTime = stopWatch.ElapsedMilliseconds;

            while (!IsDisposed) // 毎フレーム呼ばれる処理
            {
                long currentTime = stopWatch.ElapsedMilliseconds;
                if (currentTime > 100000) stopWatch.Restart();
                float dt = (currentTime - prevTime) / 1000.0F;

                Input.Update(_pressedKeys);
                Input.Update(_mouseButtons);
                Input.Update(MousePosition, Location);

                // プレイヤーとステージをアップデート
                Scene.Current.Update(dt);

#if DEBUG
                // デバッグ用文字列
                string debugDt = "dt:" + dt*1000 + "[ms]";
                string debugFps = "FPS:" + (int)(1000 / dt);
                GraphicsContext.DrawString(debugDt, new Font("Arial", 12), Brushes.Black, ScreenWidth - 120, 0);
                GraphicsContext.DrawString(debugFps, new Font("Arial", 12), Brushes.Black, ScreenWidth - 120, 20);
#endif

                // 画面の更新
                if (InvokeRequired)
                    try { Invoke((Action)Refresh); }
                    catch (Exception) { }
                else Refresh();

                prevTime = currentTime;

            }

        }

        /// <summary>
        /// キー入力取得用。
        /// 押されたキーをpressedKeysに格納する。
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode)) _pressedKeys.AddLast(e.KeyCode);
        }

        /// <summary>
        /// キー入力取得用。
        /// キーが離されるとpressedKeysから除外する。
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _pressedKeys.Remove(e.KeyCode);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // バックスペース(\n)、SOF(\u0001)、改行(\r,\n)、タブ(\t)は除外。
            if (e.KeyChar == '\b' || e.KeyChar == '\u0001' || e.KeyChar == '\r' || e.KeyChar == '\n' || e.KeyChar == '\t') return;
            Input.KeyBoard.Append(e.KeyChar);
        }

        //押されているマウスのボタン
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_mouseButtons.Contains(e.Button)) _mouseButtons.AddLast(e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseButtons.Remove(e.Button);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_bmp, 0, 0);
        }

    }
}
