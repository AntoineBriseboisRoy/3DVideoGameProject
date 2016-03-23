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
    public class GrilleCollision : Microsoft.Xna.Framework.GameComponent, IGame
    {
        public Vector2 GridWorldSize { get; set; }
        public float NodeRadius { get; set; }
        Noeud[,] Grid { get; set; }
        float NodeDiameter { get; set; }
        int GridSizeX { get; set; }
        int GridSizeY { get; set; }
        Color[] DataTexture { get; set; }
        Texture2D CarteTerrain { get; set; }
        float HauteurY { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        string NomCarte { get; set; }

        public GrilleCollision(Game jeu, Vector2 gridWorldSize, float nodeRayon, string nomCarte, float hauteurY)
            : base(jeu)
        {
            GridWorldSize = gridWorldSize;
            NomCarte = nomCarte;
            HauteurY = hauteurY;
        }

        public override void Initialize()
        {
            base.Initialize();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            CarteTerrain = GestionnaireDeTextures.Find(NomCarte);
            DataTexture = new Color[CarteTerrain.Width * CarteTerrain.Height];
            CarteTerrain.GetData<Color>(DataTexture);
            GridSizeX = CarteTerrain.Width;
            GridSizeY = CarteTerrain.Height;
            NodeRadius = (GridWorldSize.X / 2f) / GridSizeX;
            NodeDiameter = 2 * NodeRadius;
            CreateGrid();

        }

        void CreateGrid()
        {
            Grid = new Noeud[GridSizeX, GridSizeY];

            Vector3 worldTopLeft = new Vector3(0, HauteurY, 0) - Vector3.Right * GridWorldSize.X / 2f + Vector3.Forward * GridWorldSize.Y / 2f;
            int k = 0;
            for (int y = 0; y < GridSizeY; ++y)
            {
                for (int x = 0; x < GridSizeX; ++x)
                {
                    Vector3 worldPoint = worldTopLeft + Vector3.Right * (x * NodeDiameter) - Vector3.Forward * (y * NodeDiameter);
                    bool walkable = DataTexture[k].R == 0 ? false : true;

                    Grid[x, y] = new Noeud(walkable, worldPoint, x, y);
                    ++k;
                }
            }
        }
        public Noeud NodeFromWorldPoint(Vector3 worldPosition)
        {
            worldPosition += new Vector3(NodeRadius, 0, NodeRadius);
            float percentX = (worldPosition.X + (GridWorldSize.X / 2f)) / GridWorldSize.X;
            float percentY = (worldPosition.Z + (GridWorldSize.Y / 2f)) / GridWorldSize.Y;

            percentX = MathHelper.Clamp(percentX, 0, 1);
            percentY = MathHelper.Clamp(percentY, 0, 1);

            int x = (int)(GridSizeX * percentX);
            int y = (int)(GridSizeY * percentY);
            return Grid[x, y];
        }

        public List<Noeud> GetNeighboors(Noeud node)
        {
            List<Noeud> voisins = new List<Noeud>();
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (Math.Abs(x) == Math.Abs(y))
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < GridSizeX && checkY >= 0 && checkY < GridSizeY)
                    {
                        voisins.Add(Grid[checkX, checkY]);
                    }
                }
            }

            return voisins;
        }
    }
}