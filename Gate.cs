using System.ComponentModel;
using System.Drawing;
using Newtonsoft.Json;

namespace HackTheWorld
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Gate : GameObject, IDrawable
    {
<<<<<<< HEAD
        [JsonProperty("next", Order = 0)]
        public string NextStage{ get; set; }

=======
>>>>>>> d2c32f3605548d6b22d258ffc0b0d55f5debbfdd
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
