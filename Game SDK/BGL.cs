using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace fesbGameSDK
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {

        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        public static List<Sprite> allSprites = new List<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Sprite sprite in allSprites)
            {
                //if (sprite.bmp != null && sprite.show == true)
                if (sprite != null)
                    if (sprite.Show == true)
                    {
                        //lock (sprite.CurrentCostume)
                        g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                    }
            }
            //Ispis(g, bodovi);
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();

        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
            //dodano za pucanje
            if (sensing.Key == "Space")
            {
                space = true;
            }
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }
        
        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */




        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        MyCharacter debeli;
        MyCharacter oruzje;
        JunkFood jf;
        Fruit f;
        JunkFood jf2;
        Fruit f2;

        // za klona
        bool space;

        //za restartat igru
        bool PrvaIgra = true;
        //bool start;
        int highscore;

        /* Initialization */
        private void SetupGame()
        {

       

            //1. setup stage
            SetStageTitle("Food Mania");
            setBackgroundColor(Color.WhiteSmoke);
            setBackgroundPicture("background/zid.jpg");
            setPictureLayout("stretch");


            //2. add sprites

            debeli = new MyCharacter("sprites/trciD.png", 0, 360);
            Game.AddSprite(debeli);
            debeli.SetSize(100);
            debeli.RotationStyle = "AllAround";
            debeli.AddCostumes("sprites/trciD.png", "sprites/trciDD.png", "sprites/trciL.png", "sprites/trciLL.png");
            debeli.SetVisible(false);

            oruzje = new MyCharacter("sprites/vatra.png", 0, 360);
            Game.AddSprite(oruzje);
            oruzje.SetSize(50);
            oruzje.RotationStyle = "AllAround";
            oruzje.SetVisible(false);

            jf = new JunkFood("sprites/cola.png", 0, 0);
            Game.AddSprite(jf);
            jf.SetSize(70);
            jf.AddCostumes("sprites/cola.png", "sprites/pomfrit.png", "sprites/pizza.png", "sprites/cheese.png");
            jf.SelectCostume(SlucajanBr(1, 4));
            jf.X = SlucajanBr(0, GameOptions.RightEdge - jf.Width);
            jf.Y = 0 - jf.Heigth;
            jf.SetVisible(false);

            jf2 = new JunkFood("sprites/cola.png", 0, 0);
            Game.AddSprite(jf2);
            jf2.SetSize(70);
            jf.AddCostumes("sprites/cola.png", "sprites/pomfrit.png", "sprites/pizza.png", "sprites/cheese.png");
            jf2.SelectCostume(SlucajanBr(1, 4));
            jf2.X = SlucajanBr(0, GameOptions.RightEdge - jf2.Width);
            jf2.Y = 0 - jf2.Heigth;
            jf2.SetVisible(false);

            f = new Fruit("sprites/kruska.png", 0, 0);
            Game.AddSprite(f);
            f.SetSize(70);
            f.AddCostumes("sprites/kruska.png", "sprites/jabuka.png", "sprites/banana.png", "sprites/ananas.png");
            f.SelectCostume(SlucajanBr(1, 4));
            f.X = SlucajanBr(0, GameOptions.RightEdge - f.Width);
            f.Y = 0 - f.Heigth;
            f.SetVisible(false);

            f2 = new Fruit("sprites/kruska.png", 0, 0);
            Game.AddSprite(f2);
            f2.SetSize(70);
            f2.AddCostumes("sprites/kruska.png", "sprites/jabuka.png", "sprites/banana.png", "sprites/ananas.png");
            f2.SelectCostume(SlucajanBr(1, 4));
            f2.X = SlucajanBr(0, GameOptions.RightEdge - f2.Width);
            f2.Y = 0 - f2.Heigth;
            f2.SetVisible(false);
        }

        private void Ispis()
        {
            ISPIS = "Lives: " + debeli.lives + " Health: " + debeli.health;
        }
        private int SlucajanBr(int min, int max)
        {
            Random r = new Random();
            int br = r.Next(min, max + 1);
            return br;
        }
        private void StartGame()
        {
            debeli.health = 0;
            debeli.lives = 3;

            //START = true;
            START = true;

            if (PrvaIgra)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button1.Visible = false;
                button2.Visible = false;
                label1.Visible = false;
            }

            debeli.SetVisible(true);
            f.SetVisible(true);
            jf.SetVisible(true);

            setBackgroundPicture("background/getready.jpg");
            setPictureLayout("stretch");

            Game.StartScript(CharacterMove);
            Game.StartScript(CharacterAnimation);
            Game.StartScript(Clone);
            Game.StartScript(FruitFall);
            Game.StartScript(JunkFoodFall);
        }
        private void GameOver()
        {
            //START = false;
            START = false;
            PrvaIgra = false;

            ISPIS = "";
            debeli.SetVisible(false);
            f.SetVisible(false);
            f2.SetVisible(false);
            jf.SetVisible(false);
            jf2.SetVisible(false);


            setBackgroundPicture("background/gameover.jpg");
            setPictureLayout("stretch");
            Highscore forma = new Highscore();
            if (debeli.health > highscore)
            {
                highscore = debeli.health;
                forma.tekst = "Čestitam! \nTvoj novi highscore je: " + debeli.health;
                forma.ShowDialog();
            }
            else
            {
                forma.tekst = "Bodovi: " + debeli.health + "\nHighscore: " + highscore;
                forma.ShowDialog();
            }

            //START = true;
            StartGame();
        }

        /* Scripts */
        private int CharacterMove()
        {
            while (START)
            {
                if (sensing.KeyPressed("Right"))
                {
                    debeli.X += 6;

                }
                if (sensing.KeyPressed("Left"))
                {
                    debeli.X -= 6;
                }

            }
            return 0;
        }

        private int CharacterAnimation()
        {
            while (START)
            {
                if (sensing.KeyPressed("Right"))
                {
                    debeli.SelectCostume(2);
                    Wait(0.1);
                    debeli.SelectCostume(1);
                    Wait(0.1);
                }
                if (sensing.KeyPressed("Left"))
                {
                    debeli.SelectCostume(4);
                    Wait(0.1);
                    debeli.SelectCostume(3);
                    Wait(0.1);
                }
            }
            return 0;
        }

        //NOT OK
        private int Clone()
        {
            while (START)
            {
                if (space)
                {
                    oruzje.SetVisible(true);
                    oruzje.Y = debeli.Y;
                    oruzje.X = debeli.X;

                    while (true)
                    {
                        oruzje.Y--;
                        Wait(0.01);

                        if (oruzje.Y == GameOptions.UpEdge - oruzje.Heigth)
                        {
                            space = false;
                            oruzje.SetVisible(false);
                            break;
                        }
                    }
                }
            }
            return 0;
        }

        private int FruitFall()
        {
            bool Skripta2Zapoceta = false;
            bool PozadinaNamjestena = false;
            while (START)
            {
                //početak igre
                if (f.Show && !PozadinaNamjestena)
                {
                    setBackgroundPicture("background/zid.jpg");
                    setPictureLayout("stretch");
                    Ispis();
                    PozadinaNamjestena = true;
                    f.Show = true;
                    if (false) f.Y = 0;
                }
                Wait(0.02);
                f.Y += 2;
                //možda u prvih par krugova neće padat drugo voće jer neće pogodit slučajan broj
                //al to je ok jer počinje od manje stvari koje padaju pa je lakše za početak
                if (f.Y == SlucajanBr(100, 200) && !Skripta2Zapoceta)
                {
                    Skripta2Zapoceta = true;
                    Game.StartScript(Fruit2Fall);
                }
                //ako je miki uhvatio voće
                if (debeli.TouchingSprite(f) && f.Show ) 
                {
                    f.SetVisible(false);
                    debeli.health += f.vitamins;
                    Ispis();
                }
                   //ako je voće palo, a da ga debeli nije skupio
                if (f.Y == GameOptions.DownEdge - f.Heigth && f.Show)
                {
                    debeli.lives--;
                    Ispis();
                    if (debeli.lives == 0)
                    {
                        GameOver();
                        break;
                    }
                }
            }
            return 0;
        }

        private int Fruit2Fall()
        {
            while (START)
            {
                f2.Y += 2;
                Wait(0.02);

                if (debeli.TouchingSprite(f2) && f2.Show)
                {
                    f2.SetVisible(false);
                    debeli.health += f2.vitamins;
                    Ispis();
                }
                //ako je voće palo, a da ga miki nije skupio
                if (f2.Y == GameOptions.DownEdge - f2.Heigth && f2.Show)
                {
                    debeli.lives--;
                    Ispis();
                    if (debeli.lives == 0)
                    {
                        GameOver();
                        break;
                    }
                }
            }
            return 0;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private int JunkFoodFall()
        {
            bool Skripta2Zapoceta = false;
            while (START)
            {
                jf.Y += 2;
                Wait(0.02);
                if (jf.Y == SlucajanBr(100, 200) && !Skripta2Zapoceta)
                {
                    Skripta2Zapoceta = true;
                    Game.StartScript(JunkFood2Fall);
                }

                if (debeli.TouchingSprite(jf) && jf.Show)
                {
                    jf.SetVisible(false);
                    debeli.health -= jf.Damage; //gubi health ovisno koji ga je fast food dotakao
                    Ispis();
                    //ako je health manji od 0, gubi život
                    if (debeli.health < 0)
                    {
                        debeli.lives--;
                        debeli.health = 0;
                        Ispis();
                        if (debeli.lives == 0)
                        {
                            GameOver();
                            break;
                        }
                    }
                }

                if(oruzje.TouchingSprite(jf) && oruzje.Show && jf.Show)
                {
                    jf.SetVisible(false);
                    debeli.health += jf.Damage;
                    Ispis();
                }
            }
            return 0;
        }
        
        private int JunkFood2Fall()
        {
            while (START)
            {
                jf2.Y += 2;
                Wait(0.02);
                if (debeli.TouchingSprite(jf2) && jf2.Show ) 
                {
                    jf2.SetVisible(false);
                    debeli.health -= jf2.Damage;
                    Ispis();
                    //ako je health manji od 0, gubi život
                    if (debeli.health < 0)
                    {
                        debeli.lives--;
                        debeli.health = 0;
                        Ispis();
                        if (debeli.lives == 0)
                        {
                            GameOver();
                            break;
                        }
                    }

                }

                if(oruzje.TouchingSprite(jf2) && oruzje.Show && jf2.Show)
                {
                    jf2.SetVisible(false);
                    debeli.health += jf2.Damage;
                    Ispis();
                }
            }
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Help forma = new Help();
            forma.ShowDialog();
        }




        /* ------------ GAME CODE END ------------ */


    }
}

