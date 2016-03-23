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

        public ZombieInfo(Vector3 position, Vector3 rotation, int numéro)
        {
            Position = position;
            Rotation = rotation;
            Numéro = numéro;
            Server.Envoyer("127.0.0.1", 5035, this);
        }
    }
}
