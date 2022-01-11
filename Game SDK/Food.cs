using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fesbGameSDK
{
    class Food:Sprite
    {


        public Food(string putanja, int osX, int osY):base(putanja,osX,osY)
        {
        }
        private int SlucajanBr(int min, int max)
        {
            Random r = new Random();
            int br = r.Next(min, max + 1);
            return br;
        }
        public override int Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                //ako je hrana pala dole
                if (value > GameOptions.DownEdge)
                {

                    this.SelectCostume(SlucajanBr(1, 4));
                    y = GameOptions.UpEdge - this.Heigth;
                    x = SlucajanBr(0, GameOptions.RightEdge - this.Width);
                    this.SetVisible(true);
                }
                else
                {
                    base.Y = value;
                }
            }
        }
    }
    class JunkFood : Food
    {

        private int damage;
        public int Damage
        {
            get { return damage; }
         
        }
        
        public JunkFood(string putanja, int osX, int osY):base(putanja,osX,osY)
        {
        }
        public override int CostumeIndex
        {
            get
            {
                return base.CostumeIndex;
            }
            set
            {
                if (value == 0)
                    this.damage = 1;
                else if (value == 1)
                    this.damage = 2;
                else if(value==2)
                    this.damage = 3;
                else if (value == 3)
                    this.damage = 4;
                base.CostumeIndex = value;
            }
        }

    }
    class Fruit : Food
    {
        public int vitamins;
        public int Vitamins
        {
            get { return vitamins; }
          
        }
        public Fruit(string putanja, int osX, int osY):base(putanja,osX,osY)
        {
        }
        public override int CostumeIndex
        {
            get
            {
                return base.CostumeIndex;
            }
            set
            {
                if (value == 1)
                    this.vitamins = 1;
                else if (value == 2)
                    this.vitamins = 2;
                else if(value==3)
                    this.vitamins = 3;
                else if (value == 4)
                    this.vitamins = 4;
                base.CostumeIndex = value;
            }
        }
    }
}
