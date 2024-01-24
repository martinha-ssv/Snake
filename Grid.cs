using System.Collections.Generic;
using System;

namespace Snake
{
    public class Grid
    {
        public string character = " ";
        public int size;
        public Grid(int s) 
        {
            size = s;
        }

        public int setSize(int s) {
            size = s;
            return size;
        }

    }
}