using System;
using System.Collections.Generic;

namespace Snake
{
    public class Fruit 
    {
        public List<int> coordinates = new List<int>();
        public string character = "*";
        public Boolean eaten = true;

        Grid grid;

        public Fruit(Grid g) {
            grid = g;
        }

        public void GetNewCoordinates() {
            
            int l;
            int c;
            Random rd = new Random();
            if (eaten)
                do {
                l = rd.Next(grid.size);
                c = rd.Next(grid.size);
                coordinates = new List<int> {l, c};
                } while(GameController.CoordinatesTaken(coordinates));
                eaten = false;
                
        }

        

    }
}