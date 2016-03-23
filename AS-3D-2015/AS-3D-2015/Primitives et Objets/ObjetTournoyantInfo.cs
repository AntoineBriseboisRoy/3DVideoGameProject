using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    [Serializable]
    class ObjetTournoyantInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public string NomModèle { get; private set; }

        public ObjetTournoyantInfo(Vector3 position, Vector3 rotation, string nomModèle)
        {
            Position = position;
            Rotation = rotation;
            NomModèle = nomModèle;
            Server.Envoyer("127.0.0.1", 5035, this);
        }
    }
}
