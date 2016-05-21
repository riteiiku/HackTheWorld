﻿using System.Drawing;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// アイテム
    /// </summary>
    public class Item : GameObject
    {
        public Item(float x, float y) : base(x, y) { }

        public Item(float x, float y, float vx, float vy) : base(x, y, vx, vy) { }

        public Item(float x, float y, float vx, float vy, float w, float h) : base(x, y, vx, vy, w, h) { }

        public override void Initialize()
        {
            base.Initialize();
            X += CellSize / 4;
            Y += CellSize / 2;
            W = CellSize / 2;
            H = CellSize / 2;
            ObjectType = ObjectType.Item;
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.GreenYellow, X, Y, Width, Height);
            GraphicsContext.DrawRectangle(Pens.ForestGreen, X, Y, Width, Height);
        }
    }
}
