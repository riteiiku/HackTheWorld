using System;
using System.Drawing;
using System.IO;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    class StageSelectScene : Scene
    {
        private string[] _files;
        private Image _title;
        private MenuItem[] _menuItems;
        
        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _menuItems = new MenuItem[8];

            for (int i = 0; i < 8; i++)
            {
                _menuItems[i] = new MenuItem(Image.FromFile(@".\image\stage" + (i + 1) + ".png"))
                {
                    Position = new Vector(300*(i%4) + 50, 220*(i/4) + 200),
                    Size = new Vector(280, 200)
                };
            }

            _files = Directory.GetFiles(@".\stage\", "*.json", SearchOption.TopDirectoryOnly);
            _title = Image.FromFile(@"image\stageSelect.png");
        }

        public override void Update(float dt)
        {

            for (int i=0; i<8; i++)
            {
                if(_menuItems[i].Clicked) Scene.Push(new EditScene(Stage.Load(Path.GetFileName(_files[i]))));
            }

            GraphicsContext.Clear(Color.White);
            GraphicsContext.DrawImage(_title, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                var m = _menuItems[i];
                if (m.IsSelected) GraphicsContext.FillRectangle(Brushes.Red, m.X-3, m.Y-3, m.W+6, m.H+6);
                _menuItems[i].Draw();
            }
        }
    }
}
