using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace monoSlime2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        public static int window_w = 1377;
        public static int window_h = 786;
        public static int img_w = 1366;
        public static int img_h = 768;
        public DirectBitmap Dbmp;
        public long framecounter = 0;

        //opencl stuff
        public OpenCLTemplate.CLCalc.Program.Kernel test_kernal;
        public OpenCLTemplate.CLCalc.Program.Kernel draw_kernal;
        public OpenCLTemplate.CLCalc.Program.Kernel blurr_kernal;
        public OpenCLTemplate.CLCalc.Program.Variable[] test_args;
        public OpenCLTemplate.CLCalc.Program.Variable img_rgb_PackedInts_CL;
        public OpenCLTemplate.CLCalc.Program.Variable img_rgb_ValuesInts_CL;
        public OpenCLTemplate.CLCalc.Program.Variable img_trails_rgb_ValuesInts_CL;
        public OpenCLTemplate.CLCalc.Program.Variable img_pixel_RgbValueStartIndex_CL;
        public OpenCLTemplate.CLCalc.Program.Variable agent_pos_x_CL;
        public OpenCLTemplate.CLCalc.Program.Variable agent_pos_y_CL;
        public OpenCLTemplate.CLCalc.Program.Variable agent_bearing_CL;
        public OpenCLTemplate.CLCalc.Program.Variable agent_mag_CL;
        public OpenCLTemplate.CLCalc.Program.Variable agent_pixelID_CL;
        public OpenCLTemplate.CLCalc.Program.Variable imageInfoArray_CL;
        public OpenCLTemplate.CLCalc.Program.Variable settingsArray_CL;
        public OpenCLTemplate.CLCalc.Program.Variable debugOuts_CL;

        public int workers_test;
        public int workers_draw;
        public int workers_blurr;
        public int[] workers;
        public int[] draw_workers;
        public int[] blurr_workers;
        public int[] img_rgb_PackedInts = new int[img_w * img_h];
        public int[] img_rgb_ValuesInts = new int[img_w * img_h * 4];
        public int[] img_trails_rgb_ValuesInts = new int[img_w * img_h * 4];
        public int[] img_pixel_RgbValueStartIndex = new int[img_w * img_h];
        public int[] imageInfoArray = new int[3] { img_w, img_h, 0 };
        public float[] settingsArray = new float[2] { 30, 45 };
        public float[] debugOuts = new float[11] {0,0,0,0,0,0,0,0,0,0,0};



        string debugData;
        string debugData2;
        Texture2D agents_texture;
        Texture2D button_texture;
        Texture2D singleButton_texture;
        Texture2D textbox_texture;
        Texture2D slider_texture;
        Texture2D sliderToggle_texture;
 
        MouseState mouseState;
        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        //Agents stuff
        int agentsCount = 1000000; // (int)((img_w*img_h) * 0.05f);
        float[] agent_pos_x;
        float[] agent_pos_y;
        float[] agent_bearing;
        float[] agent_mag;
        int[] agent_pixelID;

        //button stuff  
        private List<Component> _allComponents;
        private List<Button> _buttonComponents;
        private List<Textbox> _textboxComponents;
        

        Stopwatch w = new Stopwatch();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = window_w;
            _graphics.PreferredBackBufferHeight = window_h;
            // _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;

        }



         

        public void InitializeAgents()
        {
            agent_pos_x = new float[agentsCount];
            agent_pos_y = new float[agentsCount];
            agent_bearing = new float[agentsCount];
            agent_mag = new float[agentsCount];
            agent_pixelID = new int[agentsCount];

            for (int i = 0; i < agentsCount; i++)
            {
                agent_pos_x[i] = img_w / 2;// (float)(ThreadSafeRandom.NextDouble(15, img_w - 15)); //
                agent_pos_y[i] = img_h / 2;//(float)(ThreadSafeRandom.NextDouble(15, img_h - 15)); //
                agent_pixelID[i] = ((img_w * (int)Math.Round(agent_pos_y[i])) + (int)Math.Round(agent_pos_x[i]));
                agent_bearing[i] = (float)ThreadSafeRandom.NextDouble(0, 360);
                agent_mag[i] = 1f;

            }

        }

        public void InitializeImage()
        {
            for (int i = 0; i < img_rgb_PackedInts.Length; i++)
            {
                img_rgb_PackedInts[i] = Abgr_packedint(255,0,0,0);
            }
            int pxCount = 0;
            for (int i = 0; i < img_rgb_ValuesInts.Length; i++)
            {
                if (i%4 == 0)
                {
                    img_pixel_RgbValueStartIndex[pxCount] = i;
                    pxCount++;
                }
                img_rgb_ValuesInts[i] = 0;
                img_trails_rgb_ValuesInts[i] = 0;
            }
            agents_texture = new Texture2D(GraphicsDevice, img_w, img_h, false, SurfaceFormat.Color);
            agents_texture.SetData<int>(img_rgb_PackedInts, 0, img_rgb_PackedInts.Length);
           
        }
        public void InitializeOpenCL()
        {
            

            OpenCLTemplate.CLCalc.InitCL();

            OpenCLTemplate.CLCalc.Program.Compile(new string[] { Kernals.test, Kernals.draw , Kernals.blurr });
            test_kernal = new OpenCLTemplate.CLCalc.Program.Kernel("Test");
            draw_kernal = new OpenCLTemplate.CLCalc.Program.Kernel("draw");
            blurr_kernal = new OpenCLTemplate.CLCalc.Program.Kernel("blurr");


            workers_test = agentsCount;
            workers_draw = img_w * img_h;
            workers_blurr = img_w * img_h;
            workers = new int[] { workers_test };
            draw_workers = new int[] { workers_draw };
            blurr_workers = new int[] { workers_blurr };

            img_rgb_PackedInts_CL = new OpenCLTemplate.CLCalc.Program.Variable(img_rgb_PackedInts);
            img_rgb_ValuesInts_CL = new OpenCLTemplate.CLCalc.Program.Variable(img_rgb_ValuesInts);
            img_trails_rgb_ValuesInts_CL = new OpenCLTemplate.CLCalc.Program.Variable(img_trails_rgb_ValuesInts);
            img_pixel_RgbValueStartIndex_CL = new OpenCLTemplate.CLCalc.Program.Variable(img_pixel_RgbValueStartIndex);
            agent_pos_x_CL = new OpenCLTemplate.CLCalc.Program.Variable(agent_pos_x);
            agent_pos_y_CL = new OpenCLTemplate.CLCalc.Program.Variable(agent_pos_y);
            agent_bearing_CL = new OpenCLTemplate.CLCalc.Program.Variable(agent_bearing);
            agent_mag_CL = new OpenCLTemplate.CLCalc.Program.Variable(agent_mag);
            agent_pixelID_CL = new OpenCLTemplate.CLCalc.Program.Variable(agent_pixelID);
            imageInfoArray_CL = new OpenCLTemplate.CLCalc.Program.Variable(imageInfoArray);
            settingsArray_CL = new OpenCLTemplate.CLCalc.Program.Variable(settingsArray);
            debugOuts_CL = new OpenCLTemplate.CLCalc.Program.Variable(debugOuts);

            test_args = new OpenCLTemplate.CLCalc.Program.Variable[] { 
                img_rgb_PackedInts_CL,
                img_rgb_ValuesInts_CL,
                img_trails_rgb_ValuesInts_CL,
                img_pixel_RgbValueStartIndex_CL,
                agent_pos_x_CL,
                agent_pos_y_CL,
                agent_bearing_CL,
                agent_mag_CL,
                agent_pixelID_CL,
                imageInfoArray_CL,
                settingsArray_CL,
                debugOuts_CL
            };


        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            InitializeAgents();
            InitializeImage();
            InitializeOpenCL();

        }

        public int Abgr_packedint(int a, int b, int g, int r)
        {
            int Ccolor = 0;
            Ccolor |= a << 24;
            Ccolor |= b << 16;
            Ccolor |= g << 8;
            Ccolor |= r;
            return Ccolor;
        }

        protected override void LoadContent()
        {

            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("defaultFont");
            button_texture = Content.Load<Texture2D>("Button");
            singleButton_texture = Content.Load<Texture2D>("singleButton");
            textbox_texture = Content.Load<Texture2D>("textbox");
            slider_texture = Content.Load<Texture2D>("slider");
            sliderToggle_texture = Content.Load<Texture2D>("sliderToggle");

            var buttonOne = new Button(button_texture, font)
            {
                PositionScale = new Vector2(0.1f,0.97f),
                //Position = new Vector2(300, Window.ClientBounds.Height - 50),
                Text = "Button",
            };
            buttonOne.Click += ButtonOne_Click;

            var Quitbutton = new Button(button_texture, font);
            Quitbutton.PositionScale = new Vector2(0f, 0.97f);
            Quitbutton.Text = "Quit";
            Quitbutton.Click += Quitbutton_Click;

            var singlebutton = new Button(singleButton_texture, font)
            {
                PositionScale = new Vector2(0f, 0.5f),
                // Position = new Vector2(600, 500),
                Text = "+",
            };
            singlebutton.Click += Singlebutton_Click;
            
            var textbox = new Textbox(textbox_texture, font, "Textbox1")
            {
                PositionScale = new Vector2(0f, 0f),
                // Position = new Vector2(600, 500),
                Text = "TurnDeg: " + settingsArray[0].ToString(),
            };
            textbox.UpdateText += Textbox_UpdateText;

            var turnslider = new Slider(slider_texture, sliderToggle_texture, font, "Slider1")
            {
                PositionScale = new Vector2(0.15f, 0.01f),
                ToggleValue = 0,
                MinMaxValues = new Vector2(0,180),
                // Position = new Vector2(600, 500),
                //Text = settingsArray[0].ToString(),
            };
            turnslider.UpdateValue += Turnslider_UpdateValue;

            var sensAngletextbox = new Textbox(textbox_texture, font, "Textbox2")
            {
                PositionScale = new Vector2(0f, 0.05f),
                // Position = new Vector2(600, 500),
                Text = "TurnDeg: " + settingsArray[0].ToString(),
            };
            sensAngletextbox.UpdateText += SensAngletextbox_UpdateText; ;

            var sensAngleslider = new Slider(slider_texture, sliderToggle_texture, font, "Slider2")
            {
                PositionScale = new Vector2(0.15f, 0.06f),
                ToggleValue = 0,
                MinMaxValues = new Vector2(0, 45),
                // Position = new Vector2(600, 500),
                //Text = settingsArray[0].ToString(),
            };
            sensAngleslider.UpdateValue += SensAngleslider_UpdateValue; ;

            _allComponents = new List<Component>
            {
                buttonOne,
                Quitbutton,
               // singlebutton,
                textbox,
                turnslider,
                sensAngleslider,
                sensAngletextbox
            };
            _buttonComponents = new List<Button>
            {
                buttonOne,
                Quitbutton,
               // singlebutton,              
            };
            _textboxComponents = new List<Textbox>
            {
                textbox,
                //sensAngletextbox
            };
        
        }

        private void SensAngleslider_UpdateValue(object sender, Slider e)
        {
            settingsArray[1] = e.ToggleValue;
        }

        private void SensAngletextbox_UpdateText(object sender, Textbox e)
        {
            e.Text = "SensAngle: " + settingsArray[1];
        }

        private void Turnslider_UpdateValue(object sender, Slider e)
        {
            settingsArray[0] = e.ToggleValue;
        }

        private void Textbox_UpdateText(object sender, Textbox e)
        {
            e.Text = "TurnDeg: " + settingsArray[0].ToString();                        
        }

        private void Singlebutton_Click(object sender, EventArgs e)
        {
            
            settingsArray[0] += 1;
            if (settingsArray[0] > 180)
            {
                settingsArray[0] = 0;
            }
        }

        private void Quitbutton_Click(object sender, EventArgs e)
        {
            agents_texture.Dispose();
            img_rgb_PackedInts_CL.Dispose();
            img_rgb_ValuesInts_CL.Dispose();
            img_trails_rgb_ValuesInts_CL.Dispose();
            img_pixel_RgbValueStartIndex_CL.Dispose();
            agent_pos_x_CL.Dispose();
            agent_pos_y_CL.Dispose();
            agent_bearing_CL.Dispose();
            agent_mag_CL.Dispose();
            agent_pixelID_CL.Dispose();
            imageInfoArray_CL.Dispose();
            debugOuts_CL.Dispose();
            Exit();
        }

        private void ButtonOne_Click(object sender, EventArgs e)
        {
           
        }

        public bool IsKeyPressed(Keys key, bool oneShot)
        {
            if (!oneShot) return keyboardState.IsKeyDown(key);
            return keyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }

        protected override void Update(GameTime gameTime)
        {

            mouseState = Mouse.GetState();
            previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                agents_texture.Dispose();
                img_rgb_PackedInts_CL.Dispose();
                img_rgb_ValuesInts_CL.Dispose();
                img_trails_rgb_ValuesInts_CL.Dispose();
                img_pixel_RgbValueStartIndex_CL.Dispose();
                agent_pos_x_CL.Dispose();
                agent_pos_y_CL.Dispose();
                agent_bearing_CL.Dispose();
                agent_mag_CL.Dispose();
                agent_pixelID_CL.Dispose();
                imageInfoArray_CL.Dispose();
                settingsArray_CL.Dispose();
                debugOuts_CL.Dispose();
                Exit();
            }



       
            if (IsKeyPressed(Keys.LeftAlt, false) && IsKeyPressed(Keys.Enter, true))
            {
                _graphics.ToggleFullScreen();
               /* if (_graphics.IsFullScreen)
                {
                    
                    _graphics.ToggleFullScreen();

                }
                else
                {
                 
                    _graphics.ToggleFullScreen();
                }*/

               
            }
            foreach (var button in _allComponents)
            {
                button.Update(gameTime, Window);
              //  button.posUpdate();
            }
            
            // TODO: Add your update logic here
            if (framecounter%2 == 0)
            {
                settingsArray_CL.WriteToDevice(settingsArray);

                blurr_kernal.Execute(test_args, blurr_workers);
                test_kernal.Execute(test_args, workers);
                draw_kernal.Execute(test_args, draw_workers);
                
                img_rgb_PackedInts_CL.ReadFromDeviceTo(img_rgb_PackedInts);
                debugOuts_CL.ReadFromDeviceTo(debugOuts);

                agents_texture.SetData<int>(img_rgb_PackedInts, 0, img_rgb_PackedInts.Length);
            }
            
            base.Update(gameTime);
            framecounter++;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(agents_texture, new Microsoft.Xna.Framework.Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), new Microsoft.Xna.Framework.Rectangle(0,0,agents_texture.Width,agents_texture.Height), Microsoft.Xna.Framework.Color.White,0,new Vector2(0,0),SpriteEffects.None,1);

            // _spriteBatch.DrawString(font, "Mouse: X: " + mouseState.X + " Y: " + mouseState.Y, new Vector2(100, 0), Microsoft.Xna.Framework.Color.White);
            // _spriteBatch.DrawString(font, "Window Width: W:" + Window.ClientBounds.Width + " H: " + Window.ClientBounds.Height, new Vector2(100, 50), Microsoft.Xna.Framework.Color.White);
            /* _spriteBatch.DrawString(font, "Sensor 1 value: " + debugOuts[1], new Vector2(0, 50), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Sensor 2 value: " + debugOuts[2], new Vector2(0, 100), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Sensor 3 value: " + debugOuts[3], new Vector2(0, 150), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "TargetSum value: " + debugOuts[4], new Vector2(0, 200), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Targetx: " + debugOuts[5], new Vector2(0, 250), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Targety: " + debugOuts[6], new Vector2(0, 300), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Currentx: " + debugOuts[7], new Vector2(0, 350), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "Currenty: " + debugOuts[8], new Vector2(0, 400), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "TargetID: " + debugOuts[9], new Vector2(0, 450), Microsoft.Xna.Framework.Color.Yellow);
             _spriteBatch.DrawString(font, "CurrentID: " + debugOuts[10], new Vector2(0, 500), Microsoft.Xna.Framework.Color.Yellow);*/
            foreach (var button in _allComponents)
            {
                button.Draw(gameTime, _spriteBatch);
            }
            
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
