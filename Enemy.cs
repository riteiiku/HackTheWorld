﻿using System.Drawing;

namespace HackTheWorld
{
    /// <summary>
    /// 敵用のクラス。
    /// </summary>
    public class Enemy : GameObject, IDrawable
    {
        public Enemy(float x, float y) : base(x, y) { }

        public Enemy(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Enemy(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public Image Image { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Image = Image.FromFile(@"image\needle.png");
        }

        public override void Draw()
        {
            if (!IsAlive) return;
            //GraphicsContext.FillRectangle(Brushes.HotPink, X, Y, Width, Height);
            GraphicsContext.FillPie(Brushes.HotPink, X, Y, Width, Height, 0, 360);
            GraphicsContext.DrawRectangle(Pens.Magenta, X, Y, Width, Height);
            ((IDrawable)this).Draw();
        }

    }
}
