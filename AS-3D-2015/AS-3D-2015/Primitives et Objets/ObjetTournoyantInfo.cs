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
        public bool ShouldRemove { get; private set; }
        public bool ShouldCreate { get; private set; }

        public ObjetTournoyantInfo(Vector3 position, Vector3 rotation, string nomModèle, bool shouldRemove, bool shouldCreate, int port, string ip)
        {
            Position = position;
            Rotation = rotation;
            NomModèle = nomModèle;
            ShouldRemove = shouldRemove;
            ShouldCreate = shouldCreate;
            Server.Envoyer(ip, port, this);
        }
    }
}
