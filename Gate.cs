using System.ComponentModel;
using System.Drawing;
using Newtonsoft.Json;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Gate : GameObject, IDrawable
    {
        public Image Image { get; set; }

        public Gate(float x, float y) : base(x, y) {}

        public override void Initialize()
        {
            base.Initialize();
            Image = Image.FromFile(@"image\portal.png");
        }

        public override void Draw()
        {
            ((IDrawable)this).Draw();
        }

    }
}
