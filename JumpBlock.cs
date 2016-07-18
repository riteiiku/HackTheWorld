using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Console.WriteLine("pushed");
                _stage.Player.VY = -15 * Constants.CellSize;
            }
        }

    }
}
