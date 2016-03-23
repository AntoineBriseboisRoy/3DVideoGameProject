using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{

    public class Noeud
    {
        public bool Walkable { get; set; }
        public Vector3 PositionDansLeMonde { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }

        public int gridX { get; set; }
        public int gridY { get; set; }

        public Noeud parent { get; set; }
        public Noeud(bool traversable, Vector3 position, int gX, int gY)
        {
            Walkable = traversable;
            PositionDansLeMonde = position;
            gridX = gX;
            gridY = gY;
        }

    }
}