﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackTheWorld.Constants;
using System.Drawing;


namespace HackTheWorld
{
    class ContinueScene : Scene
    {
        //画像を読み込む
        Bitmap bmp = new Bitmap(@"image\gameover.bmp");

        private MenuItem _continueButton;
        private MenuItem _closeButton;
        private List<MenuItem> _menuItem;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _continueButton = new MenuItem(Image.FromFile(@"image\continue.bmp"), Image.FromFile(@"image\continue1.bmp"))
            {
                Size = new Vector(400, 100),
                Position = new Vector(800, 200)
            };
            _closeButton = new MenuItem(Image.FromFile(@"image\close1.bmp"), Image.FromFile(@"image\close.bmp"))
            {
                Size = new Vector(400, 100),
                Position = new Vector(800, 300)
            };
            _menuItem = new List<MenuItem> {_continueButton, _closeButton};

        }

        public override void Update(float dt)
        {
            foreach (var button in _menuItem)
            {
                button.IsSelected = button.Contains(Input.Mouse.Position);
            }
            if (_continueButton.Clicked)
            {
                Scene.Pop();
                Scene.Current.Startup();
            }
            if (_closeButton.Clicked)
            {
                Scene.Current = new TitleScene();
            }

            //背景を透明にする
            bmp.MakeTransparent();
            GraphicsContext.DrawImage(bmp, 0, 0);

            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}