namespace HackTheWorld
{
    public class JumpBlock : Block
    {
        private readonly Stage _stage;

        public JumpBlock(float x, float y, Stage stage) : base(x, y)
        {
            _stage = stage;
        }

        public override void Update(float dt)
        {
            if (Intersects(_stage.Player))
            {
                _stage.Player.VY = -15 * Constants.CellSize;
            }
        }

    }
}
