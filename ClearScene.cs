using System.Collections.Generic;
using static HackTheWorld.Constants;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace HackTheWorld
{
    /// <summary>
    /// コンティニュー画面
    /// </summary>
    class ClearScene : Scene
    {
        readonly Bitmap _bmp = new Bitmap(@"image\clear.png");
        private MenuItem _backButton;
        private MenuItem _continueButton;
        private MenuItem _closeButton;
        private List<MenuItem> _menuItem;
        private Bitmap con;
        private Bitmap clo;
        private Bitmap bac;
        private string[] _files;

        private readonly int _stageNo;
        //ColorMatrixオブジェクトの作成
        System.Drawing.Imaging.ColorMatrix cm;
        System.Drawing.Imaging.ImageAttributes ia;

        public ClearScene(int stageNo)
        {
            _stageNo = stageNo;
        }

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            con = new Bitmap(@"image\next.png");
            con.MakeTransparent();
            _continueButton = new MenuItem(con)
            {
                Size = new Vector(200, 100),
                Position = new Vector(300, 525)
            };
            clo = new Bitmap(@"image\end.png");
            clo.MakeTransparent();
            _closeButton = new MenuItem(clo)
            {
                Size = new Vector(200, 100),
                Position = new Vector(550, 525)
            };
            bac = new Bitmap(@"image\back.png");
            bac.MakeTransparent();
            _backButton = new MenuItem(bac)
            {
                Size = new Vector(200, 100),
                Position = new Vector(50, 525)
            };
            _menuItem = new List<MenuItem> { _backButton,_continueButton, _closeButton };

            //ColorMatrixオブジェクトの作成
            cm = new System.Drawing.Imaging.ColorMatrix();
            //ColorMatrixの行列の値を変更して、アルファ値が0.5に変更されるようにする
            cm.Matrix00 = 1;
            cm.Matrix11 = 1;
            cm.Matrix22 = 1;
            cm.Matrix33 = 0.8F;
            cm.Matrix44 = 1;
            //ImageAttributesオブジェクトの作成
            ia = new System.Drawing.Imaging.ImageAttributes();
            //ColorMatrixを設定する
            ia.SetColorMatrix(cm);

            _files = Directory.GetFiles(@".\stage\", "*.json", SearchOption.TopDirectoryOnly);

        }

        public override void Update(float dt)
        {
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            GraphicsContext.DrawImage(_bmp, new Rectangle(0, 0, _bmp.Width, _bmp.Height), 0, 0, _bmp.Width, _bmp.Height, GraphicsUnit.Pixel, ia);

            //クリックしたときの処理
            //モドル
            if (_backButton.Clicked)
            {
                Scene.Pop();
                Scene.Current.Startup();
            }
             //ススム
            if (_continueButton.Clicked)
            {
                Scene.Push(new EditScene(Stage.Load(Path.GetFileName(_files[_stageNo+1])), _stageNo+1));
            }
            //ヤメル
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