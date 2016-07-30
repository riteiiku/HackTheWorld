using System.Collections.Generic;
using static HackTheWorld.Constants;
using System.Drawing;
using System.Windows.Forms;


namespace HackTheWorld
{
    /// <summary>
    /// コンティニュー画面
    /// </summary>
    class ContinueScene : Scene
    {
        //画像を読み込む
        readonly Bitmap _bmp = new Bitmap(@"image\gameover.bmp");

        private MenuItem _continueButton;
        private MenuItem _closeButton;
        private List<MenuItem> _menuItem;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _continueButton = new MenuItem(Image.FromFile(@"image\continue.png"), Image.FromFile(@"image\continue1.png"))
            {
                Size = new Vector(400, 200),
                Position = new Vector(800, 200)
            };
            _closeButton = new MenuItem(Image.FromFile(@"image\close.png"), Image.FromFile(@"image\close1.png"))
            {
                Size = new Vector(400, 200),
                Position = new Vector(800, 450)
            };
            _menuItem = new List<MenuItem> {_continueButton, _closeButton};

        }

        public override void Update(float dt)
        {
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            //背景を透明にする
            _bmp.MakeTransparent();
            GraphicsContext.DrawImage(_bmp,  0, 0);

            //クリックしたときの処理
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
            _bmp.MakeTransparent();
            GraphicsContext.DrawImage(_bmp, 0, 0);

            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}