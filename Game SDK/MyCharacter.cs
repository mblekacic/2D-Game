using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fesbGameSDK
{
    class MyCharacter:Sprite
    {
        public int health=0;
        public int lives = 3;
        public MyCharacter(string putanja, int osX, int osY):base(putanja,osX,osY)
        {
        }
        public override int X
        {
            get
            {
                return base.X;
            }
            set
            {
                if (value < 0)
                {
                    x = 0;
                }

                else if (value > GameOptions.RightEdge - this.Width)
                {
                    x = GameOptions.RightEdge - this.Width;
                }

                else
                {
                    base.X = value;
                }
            }
        }
    }
}
