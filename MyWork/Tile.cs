using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWork
{
    public class Tile
    {
        public char Letter { get; set; }
        public int Score { get; set; }

        public Tile(char letter, int score)
        {
            Letter = letter;
            Score = score;
        }
    }
}
