using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static HackTheWorld.Constants;


namespace HackTheWorld
{
    public class ConsoleBox : TextArea
    {
        public ConsoleBox() : base("") { }
        public ConsoleBox(string str) : base(str) { }

        public void WriteLines(string str)
        {
            Lines = str.Split('\n');
        }

        public override void Draw()
        {
            GraphicsContext.FillRectangle(Brushes.DarkSlateGray, this);
            GraphicsContext.DrawRectangle(Pens.Black, this);
            for (int i = 0; i < Lines.Length; i++)
            {
                GraphicsContext.DrawString(Lines[i], Font, Brushes.LawnGreen, X, Y + i * LineHeight - 2);
            }
        }

    }
}
