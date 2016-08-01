using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static HackTheWorld.Constants;

namespace HackTheWorld
{
    /// <summary>
    /// ステージを表示しながらゲーム中のオブジェクトのコードを編集するシーン
    /// </summary>
    class EditScene : Scene
    {
        private MenuItem _backButton;
        private MenuItem _startButton;
        private Stage _stage;
        private readonly CodeBox _codebox;
        private readonly ConsoleBox _console;

        public EditScene(Stage stage)
        {
            _stage = stage;
            if (stage.EditableObjects.Count == 0) _codebox = new CodeBox();
            else _codebox = new CodeBox(stage.EditableObjects[0]);
            _console = new ConsoleBox() { Position = new Vector(CellSize * CellNumX, 300) };
        }

        public override void Cleanup()
        {
        }

        public override void Startup()
        {
            _backButton = new MenuItem(Image.FromFile(@"image\back.png")) {
                Size = new Vector(100, 50),
                Position = new Vector(0, 600)
            };
            _startButton = new MenuItem(Image.FromFile(@"image\start.png")) {
                Size = new Vector(100, 50),
                Position = new Vector(110, 600)
            };
          
        }

        public override void Update(float dt)
        {
            if (_backButton.Clicked) Scene.Pop();
            if (_startButton.Clicked) Scene.Push(new GameScene(_stage));
            if (Input.Control.Pressed && Input.W.Pushed) Application.Exit();

            if (Input.Control.Pressed)
            {
                if (Input.R.Pushed) _stage = Stage.Load("stage_1_1.json");
                if (Input.S.Pushed) _stage.Save();
            }

            foreach (var obj in _stage.EditableObjects)
            {
                if (obj.Clicked) _codebox.Focus(obj);
            }
            _codebox.Update();

            GraphicsContext.Clear(Color.White);
            DrawGrid();

            _stage.Objects.ForEach(obj => obj.Draw());

            GraphicsContext.FillRectangle(Brushes.SlateGray, 0, CellNumY * CellSize, ScreenWidth, ScreenHeight - CellNumY * CellSize);
            GraphicsContext.FillRectangle(Brushes.SlateGray, CellNumX * CellSize, 0, ScreenWidth - CellNumX * CellSize, ScreenHeight);

            GraphicsContext.FillRectangle(Brushes.WhiteSmoke, CellNumX * CellSize, 0, 100, 20);
            GraphicsContext.FillRectangle(Brushes.DarkSlateGray, CellNumX * CellSize, 280, 100, 20);
            GraphicsContext.DrawRectangle(Pens.DarkSlateGray, CellNumX * CellSize, 0, 100, 20);
            GraphicsContext.DrawRectangle(Pens.LightGray, CellNumX * CellSize, 280, 100, 20);

            GraphicsContext.DrawString("プログラム", JapaneseFont, Brushes.DarkSlateGray, CellNumX * CellSize, 0);
            GraphicsContext.DrawString("けっか", JapaneseFont, Brushes.WhiteSmoke, CellNumX * CellSize, 280);


            _codebox.Draw();
            _console.Draw();

            _backButton.Draw();
            _startButton.Draw();
        }
    }
}
