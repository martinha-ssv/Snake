using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public class Player
    {
        public List<List<int>> coordinates = new List<List<int>>();
        public List<int> removedCoordinates = new List<int>();
        Grid grid;
        public string initials;
        public string character = "■";
        public string headCharacter = "□";
        public int direction;
        public string orientation;
        //       _______________________
        //   ____|horizontal  |vertical|
        //   |1  |right       |down    |
        //   |-1 |left        |up      |
        //   ---------------------------

        public int score;
        public int size;
        public int moveCount;

        public Player(Grid g) {
            grid = g;
            Reset();
            
        }

        public void Reset() {
            coordinates.Clear();
            coordinates.Add(new List<int> {grid.size/2 , grid.size/2});
            orientation = "horizontal";
            direction = 1;
            size = 1;
            score = 0;
            moveCount = 0;
        }
        public void Move() {
            
            void Turn() {
                int newdir;
                string newor;
                direction = direction/Math.Abs(direction);
                switch(GameController.input.Key){
                    case ConsoleKey.UpArrow:
                        newor = "vertical";
                        newdir = -1;
                        break;
                    case ConsoleKey.DownArrow:
                        newor = "vertical";
                        newdir = 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        newor = "horizontal";
                        newdir = -1;
                        break;
                    case ConsoleKey.RightArrow:
                        newor = "horizontal";
                        newdir = 1;
                        break;
                    default:
                        newor = orientation;
                        newdir = direction;
                        break;
                }
                
                if (newor != orientation) {
                    orientation = newor;
                    direction = newdir;
                    moveCount++;
                } //else if (newdir == direction) {
                    //if (Console.KeyAvailable)
                    //GameController.deltaTime = 2*GameController.dt();
                
                //}
            }
            void Advance() {

                int Increment(int coord) {
                    return GameController.mod((coord+direction), grid.size);
                }

                int l = coordinates[^1][0];
                int c = coordinates[^1][1];
                
                if (orientation == "horizontal") {
                    c = Increment(c);
                } else if (orientation == "vertical") {
                    l = Increment(l);
                }
                coordinates.Add(new List<int> {l, c});
                removedCoordinates = coordinates[0];
                coordinates.RemoveAt(0);
            }
            Turn();
            Advance();
        }

        public Boolean isOnFruit() {
            return (coordinates[^1].SequenceEqual(GameController.fruit.coordinates));
        }
        public void Eat() {
            void Grow() {
                coordinates.Insert(0, removedCoordinates);
                size++;
            }
            if (isOnFruit()) {
                GameController.fruit.eaten = true;
                Grow();
            }
        }
    }

}