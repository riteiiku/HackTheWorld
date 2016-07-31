using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    public class TextArea : GameObject
    {
        protected int FontSize;
        protected Font Font;
        protected Pen Pen;
        protected float FontWidth;
        protected int LineHeight;
        protected int Cols;
        protected int LineLimit;
        public string[] Text;

        public override void Initialize()
        {
            FontSize = 12;
            Font = new Font("Courier New", FontSize);
            Pen = new Pen(Color.Black, 30);
            Cols = 40;
            // 画面の解像度には非対応
            FontWidth = FontSize * 0.83f;
            LineHeight = FontSize;
            LineLimit = 20;

            Width = FontWidth * Cols;
            Height = LineHeight * LineLimit + 4;

        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.Gray, this);
            GraphicsContext.DrawRectangle(Pens.SteelBlue, this);
        }

    }
    }
