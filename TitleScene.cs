using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;


namespace HackTheWorld
{
    /// <summary>
    /// タイトルシーン
    /// </summary>
    class TitleScene : Scene
    {
        private Image[] _menuImages;
        private MenuItem[] _menu;
        private Image _bgImage;

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _menuImages = new Image[10];
            _menu = new MenuItem[5];

            _menuImages[0] = Image.FromFile(@".\image\10.png");
            _menuImages[1] = Image.FromFile(@".\image\3.png");

            _menu[0] = new MenuItem(Image.FromFile(@".\image\play.png"),Image.FromFile(@".\image\play1.png")) {Position = new Vector(100, 400),Size=new Vector(180,60)};
            _menu[1] = new MenuItem(_menuImages[0], _menuImages[1]) {Position = new Vector(100, 500)};
            _menu[2] = new MenuItem(_menuImages[0], _menuImages[1]) {Position = new Vector(100, 600)};
            _menu[3] = new MenuItem(_menuImages[0], _menuImages[1]) {Position = new Vector(500, 400)};
            _menu[4] = new MenuItem(Image.FromFile(@".\image\esc.png"), Image.FromFile(@".\image\esc1.png")) {Position = new Vector(500, 500), Size = new Vector(180, 60) };
            _bgImage = Image.FromFile(@"image\title.png");

        }

        public override void Update(float dt)
        {

            for (int i = 0; i < 5; i++)
            {
                if (_menu[i].Clicked)
                {
                    switch (i)
                    {
                        case 0:
                            Scene.Push(new StageSelectScene());
                            break;
                        case 1:
                            Scene.Push(new MapEditScene());
                            break;
                        case 2:
                            Scene.Push(new EditScene(Stage.Load("stage1.json")));
                            break;
                        case 3:
                            Scene.Push(new GameScene(Stage.Load("stage1.json")));
                            break;
                        case 4:
                            Application.Exit();
                            break;
                    }
                }
            }

            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_bgImage, 0, 0);
            foreach (var item in _menu)
            {
                item.Draw();
            }

        }
    }
}
