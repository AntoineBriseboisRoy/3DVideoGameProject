using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class Server : Microsoft.Xna.Framework.GameComponent
    {
        private static Thread TthEcoute { get; set; }
        bool IsServer { get; set; }
        Player JoueurDeux { get; set; }
        int Port { get; set; }
        Game Jeu { get; set; }

        public Server(Game game, int port)
            :base(game)
        {
            Port = port;
            Jeu = game;
            
            TthEcoute = new Thread(new ThreadStart(Ecouter));
            TthEcoute.Start();
        }

        void Ecouter()
        {
            UdpClient listener = new UdpClient(Port);
            listener.EnableBroadcast = true;
            IPEndPoint client = new IPEndPoint(IPAddress.Any, Port);

            while (true)
            {
                byte[] data = listener.Receive(ref client);
                if (client.Address.ToString() != "127.0.0.1")
                {
                    object c = Serializer.GetObjectFromBytes<object>(data);

                    if (c is PlayerInfo)
                    {
                        PlayerInfo cam = c as PlayerInfo;
                        Console.WriteLine("Données reçues en provenance de {0} : {1}.", client.Address, client.Port);
                        if (JoueurDeux == null)
                        {
                            JoueurDeux = new Player(Jeu, "Superboy", "Default Take", "Lambent_Femal", 1, new Vector3(-MathHelper.PiOver2, 0, 0), new Vector3(-5, 0.5f, -5), 15f, "jump", "landing", "walk", "walk_slow", PlayerIndex.One, false, 5001, "");
                            Jeu.Components.Add(JoueurDeux);
                        }
                        JoueurDeux.Position = cam.Position;
                        JoueurDeux.PreviousPosition = cam.PreviousPosition;
                        JoueurDeux.Rotation = cam.Rotation;
                    }
                    //if(c is ZombieInfo)
                    //{
                    //    ZombieInfo z = c as ZombieInfo;
                    //}
                    //if (c is ObjetTournoyantInfo)
                    //{
                    //    ObjetTournoyantInfo o = c as ObjetTournoyantInfo;
                    //}
                }
            }
        }

        public static void Envoyer(string adresse, int port, object o)
        {
            byte[] msg = Serializer.GetBytes(o);
            UdpClient udpClient = new UdpClient();
            udpClient.Send(msg, msg.Length, adresse, port);
            udpClient.Close();
        }
    }
}
