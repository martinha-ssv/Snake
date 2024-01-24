using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Snake
{
    public class GameController
    {
        public static Grid grid;
        public static Player player;
        public static Fruit fruit;
        public static int deltaTime;

        public static Boolean gameLost;
        public static ConsoleKeyInfo input;

        //Listas abaixo: guardam dados do .txt file e escrevem lá.
        public static List<string> fullText;
        public static List<Result> localResults = new List<Result>();
        
    
        public static string logo = @"
  ██████  ███▄    █  ▄▄▄       ██ ▄█▀▓█████ 
▒██    ▒  ██ ▀█   █ ▒████▄     ██▄█▒ ▓█   ▀ 
░ ▓██▄   ▓██  ▀█ ██▒▒██  ▀█▄  ▓███▄░ ▒███   
  ▒   ██▒▓██▒  ▐▌██▒░██▄▄▄▄██ ▓██ █▄ ▒▓█  ▄ 
▒██████▒▒▒██░   ▓██░ ▓█   ▓██▒▒██▒ █▄░▒████▒
▒ ▒▓▒ ▒ ░░ ▒░   ▒ ▒  ▒▒   ▓▒█░▒ ▒▒ ▓▒░░ ▒░ ░
░ ░▒  ░ ░░ ░░   ░ ▒░  ▒   ▒▒ ░░ ░▒ ▒░ ░ ░  ░
░  ░  ░     ░   ░ ░   ░   ▒   ░ ░░ ░    ░   
      ░           ░       ░  ░░  ░      ░  ░
                                            
";
        public static ConsoleColor logoColor = ConsoleColor.Red;
        public static ConsoleColor textColor = ConsoleColor.DarkRed;
        public static int mod(int a, int b) {
            return (Math.Abs(a * b) + a) % b;
        }

        // Preps the game to open
        public static void CreateScene(int s) {
            
            grid = new Grid(s);
            fruit = new Fruit(grid);
            player = new Player(grid);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = logoColor;
            fruit.GetNewCoordinates();

        }
        
        // Returns true if the coordinate is part of the player's coordinate array
        public static Boolean CoordinatesTaken(List<int> coord) {
            return (player.coordinates.Any(x => x.SequenceEqual(coord))); 
        }
        
        // Runs when the game round starts
        public static void StartGame(int s) {
            Console.CursorVisible = false;
            gameLost = false;
            GetHighScores();
            CreateScene(s);
        }
        public static int GameModeSelector(string a = "") {
            string gameMode = "Select GAMEMODE: grid size\n";
            return Int32.Parse(Menu(new List<string>() {"10", "20"+a, "30"+a}, gameMode, new List<Action> {Console.WriteLine, Console.WriteLine, Console.WriteLine }));
        }

        public static void HighScoreScreen() {
            //int gs;
            //Menu(new List<string>() {"10", "20", "30", "40"}, "lol", new List<Action> {Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine });
            Menu(new List<string>() {"BACK", "QUIT"}, HighScoreText(GameModeSelector()), new List<Action>() {StartMenu, Quit});
        }
        public static void SaveScore() {
            localResults.Add(new Result(player.score, player.initials.ToUpper(), player.size, player.moveCount, grid.size));
        }
        // On start game, is called to retrieve the data from the high scores file into a working memory list
        public static void GetHighScores() {
            void ReadBlock(int ind) {
                int trueJ;
                localResults.Add(new Result());
                for (int j=0; j<Result.propertyCount; j++) {
                    trueJ = j+ind;
                    switch (j) {
                        case 0:
                            //write to score
                            localResults[ind/Result.propertyCount].score = Int32.Parse(fullText[trueJ]);
                            break;
                        case 1:
                            //write to initials
                            localResults[ind/Result.propertyCount].initials = fullText[trueJ];
                            break;
                        case 2:
                            //write to moveCount
                            localResults[ind/Result.propertyCount].moveCount = Int32.Parse(fullText[trueJ]);
                            break;
                        case 3:
                            //write to size
                            localResults[ind/Result.propertyCount].size = Int32.Parse(fullText[trueJ]);
                            break;
                        case 4:
                            //write to gridsize
                            localResults[ind/Result.propertyCount].gridSize = Int32.Parse(fullText[trueJ]);
                            break;
                    }
                }
            } 
            using (FileStream s = File.Open("HighScores.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                fullText = File.ReadAllLines(s.Name).ToList<string>();
            }
            for (int i = 0; i<fullText.Count; i+= Result.propertyCount) {
                ReadBlock(i);
            }

        }
        public static void FilterHighScores() {
            void WriteBlock(StreamWriter w, Result res) {
                w.WriteLine(res.score);
                w.WriteLine(res.initials);
                w.WriteLine(res.moveCount);
                w.WriteLine(res.size);
                w.WriteLine(res.gridSize);
            }
            localResults = localResults.OrderBy(res=>res.score).ToList<Result>();
            localResults.Reverse();

            using (FileStream s = File.Create("HighScores.txt", 3)) {
                using (StreamWriter w = new StreamWriter(s)) {
                    for (int k = 0; k<5; k++) {
                        if (localResults.Count > k)
                        WriteBlock(w, localResults[k]);
                    }
                }
            }
        }
    
        public static string HighScoreText(int gs){
            string scores = "";
            double avgMovePerSize= 0;
            double avgSize= 0;
            int scoresCount = 0;
            int modeCount = localResults.Count(res => res.gridSize == gs);
            localResults = localResults.OrderBy(res=>res.score).ToList<Result>();
            localResults.Reverse();
            for (int i = 0; i<localResults.Count; i++) {
                if (scoresCount<5 && localResults[i].gridSize == gs) {
                    scores = scores + (scoresCount+1) + " - " + localResults[i].score + ": "+ localResults[i].initials +"\n";
                    scoresCount++;
                    avgMovePerSize += localResults[i].moveCount/localResults[i].size;
                    avgSize += localResults[i].size;
                }
                
            }
            if (modeCount != 0) {
                avgMovePerSize /= modeCount;
                avgSize /= modeCount;
            }

            return $"HIGH SCORES => grid: {gs} \n\n"+scores+"\nMOVES PER FRUIT: "+avgMovePerSize+"\nAVERAGE SIZE: "+avgSize+ "\n";
        }

        public static void Quit() {
            FilterHighScores();
            System.Environment.Exit(0);
        }
        // Multiple choice
        public static string Menu(List<String> opts, string stscreen, List<Action> res) {
            string sideSpacer = "   ";
            int option = 0;
            Boolean selection = false;
            ConsoleColor selectionColor = ConsoleColor.Cyan;
            //ConsoleKeyInfo input;
            do {
                do {
                    Console.Clear();
                    Console.WriteLine(logo);
                    Console.WriteLine(stscreen);
                    for (int i = 0; i<opts.Count; i++) {
                        if (i == option) {
                            Console.ForegroundColor = selectionColor;
                            Console.WriteLine(sideSpacer[..^2]+"> "+opts[i]+" <");
                            Console.ForegroundColor = logoColor;
                        } else {
                            Console.WriteLine(sideSpacer+opts[i]);
                        }
                    }
                
                    input = Console.ReadKey();
                    switch (input.Key) {
                        case (ConsoleKey.DownArrow):
                            option = mod(option+1, opts.Count);
                            break;
                        case (ConsoleKey.UpArrow):
                            option = mod(option-1, opts.Count);
                            break;
                        case (ConsoleKey.Enter):
                            selection = true;
                            break;
                    }
                } while (Console.KeyAvailable);
                
            } while (!selection);
            res[option]();
            return opts[option][..2];
        }
        // Menu screen
        public static string startScreen = "Are you ready for your doom? \n\n";
        public static void OnStartPress() {
            grid.setSize(GameModeSelector(" (please increase window size)"));
            GameRound();
        }
        public static void StartMenu() {
            
            Menu(new List<string> {"START","HIGH SCORES", "QUIT"}, startScreen, new List<Action> {OnStartPress, HighScoreScreen, Quit});

        }
        public static void ResetRound() {
            gameLost = false;
            player.Reset();
            fruit.GetNewCoordinates();
        }
        // Game running
        public static void GameRound() {
            ResetRound();
            while (!CheckGameOver()){
                if (Console.KeyAvailable) { input = Console.ReadKey();}
                else { input = new ConsoleKeyInfo();}
                GameController.GetFrame();
                Console.Clear();
                GameController.DrawFrame();
                System.Threading.Thread.Sleep(dt());
            }
            OnGameOver();
        }
        // When the round ends
        public static void OnGameOver() {
            InitialsScreen();
            SaveScore();
            input = new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
            Menu(new List<string> {"RETRY","HIGH SCORES", "QUIT"}, ScoreScreen(), new List<Action> {OnStartPress, HighScoreScreen, Quit});

        }
        public static void InitialsScreen() {
            string sideSpacer = "     ";
            Console.Clear();
            Console.WriteLine(logo);
            Console.WriteLine(sideSpacer+"Enter your initials.\n");
            Console.Write(sideSpacer+" > ");
            player.initials = Console.ReadLine();
        }
        // Score screen
        public static string ScoreScreen() {
            string extraText;
            if (gameLost) {
                extraText = "YOUR TIME HAS COME. YOUR POISON HAS TURNED ON YOURSELF.\n\n\n";
            } else { 
                extraText = "Gone so soon? Dare to retry?\n\n\n";
            }
            return "\n             FINAL SCORE:    "+player.score+"\n              final size:    "+player.size+"\n\n"+extraText;
        }
        // Generates a frame
        public static void GetFrame() {
            
            player.Move();
            player.Eat();
            fruit.GetNewCoordinates();
        }
        // Returns true if the game is over
        public static Boolean CheckGameOver() {
            List<int> snakeHead = player.coordinates[^1];
            List<List<int>> snakeBody = player.coordinates.GetRange(0, player.coordinates.Count-1);
            if (snakeBody.Any(x => x.SequenceEqual(snakeHead))) {
                gameLost = true;
                return true;
            } else if (input.Key == ConsoleKey.Escape) {
                gameLost = false;
                return true;
            } else { 
                return false;
            }
        }
        // Returns the time interval between frames, function of the player's score and the gridsize.
        public static int dt() {
            double x = player.size/grid.size;
            return Convert.ToInt32(Math.Pow(1/Math.E, 0.6*x-5.8));
        }
        // Stat widget
        public static string Stats() {
            player.score = player.size*100-player.moveCount;
            return ("SIZE: "+player.size+"\ntotal moves: "+player.moveCount+"\nscore: "+player.score);
        }
        // Prints the frame gotten by GetFrame()
        public static void DrawFrame() {
            string sideSpacer = "         ";
            void InsertCharacter(ConsoleColor k, string ch) {
                Console.ForegroundColor = k;
                Console.Write(ch);
                Console.ForegroundColor = logoColor;
            }
            void InsertLine(string ch) {
                Console.Write(sideSpacer + " ");
                for (int i=0; i<grid.size*2+1; i++) Console.Write(ch);
                Console.WriteLine();
            }
            InsertLine("_");
            for (int j = 0; j<grid.size; j++) {
                Console.Write(sideSpacer+"||");
                for (int i = 0; i<grid.size; i++) {
                    List<int> coord = new List<int>() {j, i};
                    if (coord.SequenceEqual(player.coordinates[^1])) { //if it's the last coordinate in the player coordinates array
                        InsertCharacter(ConsoleColor.DarkGreen, player.headCharacter);
                    } else if (CoordinatesTaken(coord)) { //if it's in the player's body
                        InsertCharacter(ConsoleColor.DarkGreen, player.character);
                    } else if (fruit.coordinates.SequenceEqual(coord)) { // if it's a fruit
                        InsertCharacter(ConsoleColor.Yellow, fruit.character);
                    } else {
                        InsertCharacter(ConsoleColor.Magenta, grid.character);
                    }
                    if (i != grid.size -1) Console.Write(" ");
                }
                Console.WriteLine("||");
            }
            InsertLine("⎺");
            Console.WriteLine(Stats());
        }
        
    }
}


