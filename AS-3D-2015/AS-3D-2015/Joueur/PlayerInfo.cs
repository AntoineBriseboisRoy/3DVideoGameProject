using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    [Serializable]
    public class PlayerInfo
    {
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Vector3 PreviousPosition { get; private set; }

        public PlayerInfo(Vector3 position, Vector3 rotation, Vector3 previousPosition, int port, string ip)
        {
            Position = position;
            Rotation = rotation;
            PreviousPosition = previousPosition;
            Server.Envoyer(ip, port, this);
        }
    }
}
