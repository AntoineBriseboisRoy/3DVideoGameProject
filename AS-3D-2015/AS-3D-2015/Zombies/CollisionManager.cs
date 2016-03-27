using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AtelierXNA
{
    public class CollisionManager : Microsoft.Xna.Framework.GameComponent, IGame
    {
        public CollisionManager(Game jeu)
            : base(jeu)
        { }

        #region CollisionsDuJoueur
        //public bool CollisionJoueurs(Caméra joueurA, Caméra joueurB)
        //{
        //    Vector3 distance = joueurA.ZoneCollision.Center - joueurB.ZoneCollision.Center;
        //    float normeDist = distance.Length();
        //    float somme = joueurA.ZoneCollision.Radius + joueurB.ZoneCollision.Radius;

        //    return somme > normeDist;
        //}

        public bool CollisionJoueurMur(BoundingSphere zoneCollision, CubeColoré mur)
        {
            bool estEnCollision = false;
            Vector3 distance = zoneCollision.Center - mur.ZoneVerifCollision.Center;

            float normeDist = distance.Length();
            float somme = zoneCollision.Radius + mur.ZoneVerifCollision.Radius;
            if (somme > normeDist)
            {
                estEnCollision = zoneCollision.Intersects(mur.ZoneCollision);
            }

            return estEnCollision;
        }

        //public bool Collision JoueurMonstre(Caméra joueur, Monstre zombie)
        //{
        //    Vector3 distance = joueur.ZoneCollision.Center - zombie.ZoneCollision.Center;
        //    float normeDist = distance.Length();
        //    float somme = joueur.ZoneCollision.Radius + zombie.ZoneCollision.Radius;

        //    return somme > normeDist;
        //}

        public bool CollisionJoueurObjet(BoundingSphere obj, BoundingSphere joueur)
        {
            Vector3 distance = joueur.Center - obj.Center;
            float normeDist = distance.Length();
            float somme = joueur.Radius + obj.Radius;

            return somme > normeDist;
        }

        #endregion

        #region CollisionsDuMonstre
        public bool CollisionMonstreMur(BoundingSphere zoneCollZombie, CubeColoré mur)
        {
            bool estEnCollision = false;
            Vector3 distance = zoneCollZombie.Center - mur.ZoneCollisionZombie.Center;

            float normeDist = distance.Length();
            float somme = zoneCollZombie.Radius + mur.ZoneCollisionZombie.Radius;

            if (somme > normeDist)
            {
                estEnCollision = zoneCollZombie.Intersects(mur.ZoneCollision);
            }

            return estEnCollision;
        }

        public bool CollisionMonstreMonstre(BoundingSphere zoneCollZombieA, BoundingSphere zoneCollZombieB)
        {
            Vector3 distance = zoneCollZombieA.Center - zoneCollZombieB.Center;

            float normeDist = distance.Length();
            float somme = zoneCollZombieA.Radius + zoneCollZombieB.Radius;

            return somme > normeDist;
        }

        public bool DétectionJoueur(BoundingSphere zoneOuieZombie, BoundingSphere zoneBruitJoueur)
        {
            Vector3 distance = zoneOuieZombie.Center - zoneBruitJoueur.Center;

            float normeDist = distance.Length();
            float somme = zoneOuieZombie.Radius + zoneBruitJoueur.Radius;

            return somme > normeDist;
        }
        #endregion

    }
}
