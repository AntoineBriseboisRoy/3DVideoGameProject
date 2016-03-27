using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    [Serializable]
    class ZombieInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public int Numéro { get; private set; }
        public string NomModèle { get; private set; }
        public string NomAnim { get; set; }
        public float ÉchelleInitiale { get; private set; }
        public string NomTexture { get; private set; }
        public float IntervalleMAJ { get; private set; }
        public string Grognement { get; private set; }
        public float Vitesse { get; private set; }
        public bool ShouldCreate { get; private set; }

        public ZombieInfo(Vector3 position, Vector3 rotation, int numéro, string nomModèle, float échelleInitiale,
            string nomTexture, float intervalleMAJ, string grognement, float vitesse, string nomAnim)
        {
            Position = position;
            Rotation = rotation;
            Numéro = numéro;
            NomModèle = nomModèle;
            ÉchelleInitiale = échelleInitiale;
            NomTexture = nomTexture;
            IntervalleMAJ = intervalleMAJ;
            Grognement = grognement;
            Vitesse = vitesse;
            NomAnim = nomAnim;

            Server.Envoyer("127.0.0.1", 5035, this);
        }
    }
}
