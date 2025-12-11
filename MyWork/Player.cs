using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWork
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public List<Tile> Tiles { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            Tiles = new List<Tile>();
        }
    }
}
