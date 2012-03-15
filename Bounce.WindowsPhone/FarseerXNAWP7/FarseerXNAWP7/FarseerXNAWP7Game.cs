using FarseerXNABase.Components;
using FarseerXNABase.ScreenSystem;
using FarseerXNAWP7.Screens;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using FarseerXNAWP7.Helpers;

namespace FarseerXNAWP7
{
    /// <summary>
    /// This is the Starting point for Game
    /// </summary>
    public class FarseerXNAWP7Game : Game
    {
        private GraphicsDeviceManager _graphics;
        public ScreenManager ScreenManager { get; set; }
        private StartScreen _startScreen;

        public FarseerXNAWP7Game()
        {
            Window.Title = "BOUNCE BALL";
            
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.PreferMultiSampling = true;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.IsFullScreen = true;
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            //TODO:Vinit - Remove from final Version
            //FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
            //frameRateCounter.DrawOrder = 101;
            //Components.Add(frameRateCounter);


            //Handle Tombstoning....
            PhoneApplicationService.Current.Deactivated += (_, __) =>
                {
                    if (_startScreen != null)
                    {
                        PhoneApplicationService.Current.State.Clear();
                        FarseerXNAWP7.Helpers.ResumeState ResumeState = _startScreen.ResumeState;
                        if (ResumeState != null)
                        {
                            PhoneApplicationService.Current.State.Add("RESUMESTATE", ResumeState);
                        }
                    }
                };

            PhoneApplicationService.Current.Activated += (_, __) =>
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("RESUMESTATE"))
                    {
                        _startScreen = new StartScreen((ResumeState)PhoneApplicationService.Current.State["RESUMESTATE"]);
                        _startScreen.ExitGame += (s, e) => { this.Exit(); };
                        ScreenManager.AddScreen(_startScreen, null);
                        _startScreen.LoadLevelScreen();
                        PhoneApplicationService.Current.State.Clear();
                    }
                    else
                    {
                        WelcomeScreen();
                    }
                };

            PhoneApplicationService.Current.Launching += (_, __) =>
                {
                    WelcomeScreen();
                };
        }

        private void WelcomeScreen()
        {
            _startScreen = new StartScreen();
            _startScreen.ExitGame += (s, e) => { this.Exit(); };
            ScreenManager.AddScreen(_startScreen, null);

            //ScreenManager.AddScreen(new temp2(), null);
        }
    }

}