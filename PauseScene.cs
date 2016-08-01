using System.Collections.Generic;
using System.Drawing;


namespace HackTheWorld
{
    /// <summary>
    /// ポーズ画面
    /// </summary>
    class PauseScene : Scene
    {
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
                Position = new Vector(400, 200)
            };
            _closeButton = new MenuItem(Image.FromFile(@"image\close1.png"), Image.FromFile(@"image\close.png"))
            {
                Size = new Vector(400, 200),
                Position = new Vector(400, 450)
            };
            _menuItem = new List<MenuItem> {_continueButton, _closeButton};
        }
        public override void Update(float dt)

        {
            if (_continueButton.Clicked)
            {
                Scene.Pop();
            }
            if (_closeButton.Clicked)
            {
                Scene.Current = new TitleScene();
            }
            foreach (var item in _menuItem)
            {
                item.Draw();
            }
        }
    }
}
