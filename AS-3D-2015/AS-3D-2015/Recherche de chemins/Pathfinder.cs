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
    public class Pathfinder : Microsoft.Xna.Framework.GameComponent, IGame
    {
        Noeud Départ { get; set; }
        Noeud Cible { get; set; }
        GrilleCollision Grille { get; set; }
        List<Noeud> OpenSet { get; set; }
        HashSet<Noeud> ClosedSet { get; set; }
        public List<Noeud> Chemin { get; set; }

        public Pathfinder(Game jeu) : base(jeu) { }

        public void CalculerChemin(Vector3 startPos, Vector3 targetPos)
        {
            Grille = Game.Services.GetService(typeof(GrilleCollision)) as GrilleCollision;
            Départ = Grille.NodeFromWorldPoint(startPos);
            Cible = Grille.NodeFromWorldPoint(targetPos);
            OpenSet = new List<Noeud>();
            ClosedSet = new HashSet<Noeud>();
            OpenSet.Add(Départ);

            while (OpenSet.Count > 0)
            {
                Noeud CurrentNode = OpenSet[0];

                for (int i = 0; i < OpenSet.Count; ++i)
                {
                    if (OpenSet[i].FCost < CurrentNode.FCost || OpenSet[i].FCost == CurrentNode.FCost && OpenSet[i].HCost < CurrentNode.HCost)
                    {
                        CurrentNode = OpenSet[i];
                    }
                }
                OpenSet.Remove(CurrentNode);
                ClosedSet.Add(CurrentNode);

                if (CurrentNode == Cible)
                {
                    RetracePath(Départ, Cible);
                    break;
                }

                foreach (Noeud voisin in Grille.GetNeighboors(CurrentNode))
                {
                    if (!voisin.Walkable || ClosedSet.Contains(voisin))
                    {
                        continue;
                    }

                    int newMovementCost = CurrentNode.GCost + GetDistance(CurrentNode, voisin);
                    if (newMovementCost < voisin.GCost || !OpenSet.Contains(voisin))
                    {
                        voisin.GCost = newMovementCost;
                        voisin.HCost = GetDistance(voisin, Cible);
                        voisin.parent = CurrentNode;

                        if (!OpenSet.Contains(voisin))
                        {
                            OpenSet.Add(voisin);
                        }
                    }
                }
            }
        }


        void RetracePath(Noeud Start, Noeud End)
        {
            Chemin = new List<Noeud>();
            Noeud currentNode = End;
            while (currentNode != Start)
            {
                Chemin.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Chemin.Reverse();


        }
        int GetDistance(Noeud A, Noeud B)
        {
            int distX = Math.Abs(A.gridX - B.gridX);
            int distY = Math.Abs(A.gridY - B.gridY);
            return 10 * distX + 10 * distY;


        }

    }
}

