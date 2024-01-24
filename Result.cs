namespace Snake
{
    public class Result
    {
        static public int propertyCount=5;
        public int score;
        public string initials;
        public int size;
        public int moveCount;
        public int gridSize;
        public Result(int scr = 0, string init = "", int sz = 1, int mC = 0, int gs = 10) {
            score = scr;
            initials = init;
            size = sz;
            moveCount = mC;
            gridSize = gs;
        }
    }
}