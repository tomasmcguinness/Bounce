using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerXNAWP7.Helpers;
using FarseerXNABase.HighScore;
using Microsoft.Xna.Framework.GamerServices;
using FarseerXNABase.Components;

namespace FarseerXNAWP7
{
    public static class Assets
    {
        //Application level Constants
        public const int CELLSIZE = 32;
        public static float CELLSIZE_FARSEER = FarseerXNABase.ConvertUnits.ToSimUnits(CELLSIZE);
        public const int DENSITY = 10;
        public const float RESTITUTION = 0.5f;
        public const float FRICTION = 0.0f;
        public const int DIAMONDPOINTS = 300;

        //---------------TEXTURES---------------
        //Start Screen
        public const string MAIN_BACKGROUND = "Game/Background/StartScreenBG";
        public const string GAME_LOGO = "Game/Images2/Logo6";
        public const string BG_CLOUDS = "Game/Images2/CloudStrip";
        public const string STARTGAME_NORMAL = "Game/Images2/startgame_off";
        public const string STARTGAME_CLICKED = "Game/Images2/startgame_on";
        public const string INSTRUCTIONS_NORMAL = "Game/Images2/instructions_off";
        public const string INSTRUCTIONS_CLICKED = "Game/Images2/instructions_on";
        public const string HIGHSCORE_NORMAL = "Game/Images2/highscore_off";
        public const string HIGHSCORE_CLICKED = "Game/Images2/highscore_on";

        //Instructions Screen
        public const string INSTRUCTION_HOWTOPLAY = "Game/BlackLevelEntity/howtoplay";

        //GameOver Screen
        public const string GAMEOVER_TITLE = "Game/BlackLevelEntity/title_gameover";
        public const string TOTALSCORE_BG = "Game/BlackLevelEntity/yourresult";

        //Background
        public const string SIMPLE_BACKGROUND = "Game/Background/MainBG";

        //Common
        public const string GOTIT_NORMAL = "Game/Images2/gotit_off";
        public const string GOTIT_CLICKED = "Game/Images2/gotit_on";
        public const string MENU_NORMAL = "Game/Images2/menu_off";
        public const string MENU_CLICKED = "Game/Images2/menu_on";
        public const string BLANK = "Common/blank";
        public const string WOODPLANK_NORMAL = "Game/Images2/WoodPlank";
        public const string WOODPLANK_CLICKED = "Game/Images2/WoodPlankClicked";
        public const string HANGING_WOOD_PLANK = "Game/Images2/HangingWoodPlank";
        public const string RETRYLEVEL_NORMAL = "Game/Images2/RetryLevel_off";
        public const string RETRYLEVEL_CLICKED = "Game/Images2/RetryLevel_on";
        public const string CONITNUE_NORMAL = "Game/Images2/continue_off";
        public const string CONITNUE_CLICKED = "Game/Images2/continue_on";

        //------------LEVEL DATA-----------------
        //Level XML Files
        public const string GAMEDATA = "Levels/Game.xml";


        //------------COMMON METHODS--------------
        public static Random rand = new Random();
        public static string Obstacle()
        {
            return "Game/LevelEntity/Obstacle/obs" + rand.Next(1, 7);
        }


        //----------BLACK LEVEL ENTITIES----------
        //Obstacles
        public const string OBSTACLE1 = "Game/BlackLevelEntity/Obstacle/Obs1";
        public const string OBSTACLE2 = "Game/BlackLevelEntity/Obstacle/Obs2";
        public const string OBSTACLE3 = "Game/BlackLevelEntity/Obstacle/Obs3";
        public const string OBSTACLE4 = "Game/BlackLevelEntity/Obstacle/Obs4";
        public const string OBSTACLE5 = "Game/BlackLevelEntity/Obstacle/Obs5";
        public const string OBSTACLE6 = "Game/BlackLevelEntity/Obstacle/Obs6";
        //Spikes
        public const string SPIKE_LEFT_WOOD = "Game/BlackLevelEntity/Spike/SpikeLeft";
        public const string SPIKE_RIGHT_WOOD = "Game/BlackLevelEntity/Spike/SpikeRight";
        public const string SPIKE_UP_WOOD = "Game/BlackLevelEntity/Spike/SpikeUp";
        public const string SPIKE_DOWN_WOOD = "Game/BlackLevelEntity/Spike/SpikeDown";
        public const string SPIKE_STRIP_WOOD = "Game/BlackLevelEntity/Spike/SpikeStrip";
        //Floor
        public const string FLOORLEFT = "Game/BlackLevelEntity/Floor/floorLeft";
        public const string FLOORMIDDLE = "Game/BlackLevelEntity/Floor/floorMiddle";
        public const string FLOORRIGHT = "Game/BlackLevelEntity/Floor/floorRight";

        //Obstacle
        public static string Obstacle(FarseerXNAWP7.Helpers.EntityType Type)
        {
            if (Type == Helpers.EntityType.Obstacle1)
                return OBSTACLE1;
            if (Type == Helpers.EntityType.Obstacle2)
                return OBSTACLE2;
            if (Type == Helpers.EntityType.Obstacle3)
                return OBSTACLE3;
            if (Type == Helpers.EntityType.Obstacle4)
                return OBSTACLE4;
            if (Type == Helpers.EntityType.Obstacle5)
                return OBSTACLE5;
            if (Type == Helpers.EntityType.Obstacle6)
                return OBSTACLE6;
            return "";
        }

        public const string WOODBALL = "Game/BlackLevelEntity/Ball";
        public const string WOOD_DIAMOND = "Game/BlackLevelEntity/WoodDiamond";
        public const string EXPLOSION2 = "Game/BlackLevelEntity/explosion2";
        public const string SCORE_BOARD = "Game/BlackLevelEntity/score_board";


        //--------------LEVEL DATA XML PARSED------------
        private static List<Level> _levels;
        public static List<Level> Levels
        {
            get
            {
                if (_levels == null)
                    _levels = SerializeHelper.DeserializeParams<Level>(GAMEDATA);
                return _levels;
            }
        }
        

        //-----------MATH HELPER-------------------------
        public static float ToRadian(this float degree)
        {
            return (float)(degree * Math.PI / 180);
        }

        //----------HIGH SCORE--------------------------
        private static HighScores HighScores = new HighScores();
        private static int _score;
        private static string _levelName;
        private static SettingsManager SettingsManager = new SettingsManager();
        
        /// <summary>
        /// Initializes the scores data with level data.
        /// </summary>
        /// <param name="lvls">The Levels.</param>
        public static void InitializeScores(List<Level> lvls)
        {
            foreach (Level item in lvls)
            {
                HighScores.InitializeTable(item.LevelName, 5);
            }
            HighScores.LoadScores();
        }


        /// <summary>
        /// Determines whether [is new highscore] [the specified score].
        /// </summary>
        /// <param name="Score">The score.</param>
        /// <param name="LevelName">Name of the level.</param>
        /// <returns><c>true</c> if [is new highscore] [the specified score]; otherwise, <c>false</c>.</returns>
        public static bool IsNewHighscore(int Score, string LevelName)
        {
            return HighScores.GetTable(LevelName).ScoreQualifies(Score);
        }


        /// <summary>
        /// Adds the level score.
        /// </summary>
        /// <param name="Score">The score.</param>
        /// <param name="LevelName">Name of the level.</param>
        public static void AddLevelScore(int Score, string LevelName)
        {
            if (HighScores.GetTable(LevelName).ScoreQualifies(Score))
                //if (string.IsNullOrEmpty(SettingsManager.GetValue("PlayerName", "")))
                //{
                //Show Keyboard and get username
                if (!(Guide.IsVisible))
                {
                    _score = Score;
                    _levelName = LevelName;

                    // Show the input dialog to get text from the user
                    Guide.BeginShowKeyboardInput(Microsoft.Xna.Framework.PlayerIndex.One, "High Score Achieved", "Please enter your name",
                        SettingsManager.GetValue("PlayerName", ""),
                        (IAsyncResult result) =>
                        {
                            string sipContent = Guide.EndShowKeyboardInput(result);

                            // Did we get some input from the user?
                            if (!string.IsNullOrEmpty(sipContent))
                            {
                                // Add the name to the highscore
                                HighScores.GetTable(_levelName).AddEntry(sipContent, _score);
                                // Save the scores
                                HighScores.SaveScores();
                                // Store the name for later use
                                SettingsManager.SetValue("PlayerName", sipContent);
                            }

                            _score = 0;
                            _levelName = string.Empty;

                        }, null);
                }
            //}
            //else
            //{
            //    //Add Score Entry
            //    HighScores.GetTable(LevelName).AddEntry(SettingsManager.GetValue("PlayerName", ""), Score);
            //    // Save the scores
            //    HighScores.SaveScores();
            //}
        }

        /// <summary>
        /// Gets the level score.
        /// </summary>
        /// <param name="LevelName">Name of the level.</param>
        public static List<HighScoreEntry> GetLevelScore(string LevelName)
        {
            List<HighScoreEntry> lst = HighScores.GetTable(LevelName).Entries.ToList();
            List<HighScoreEntry> returnList = new List<HighScoreEntry>();
            foreach (HighScoreEntry item in lst)
            {
                if (item.Score != 0)
                    returnList.Add(item);
            }
            return returnList;
        }
    }
}
