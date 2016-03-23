using System;

namespace AtelierXNA
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Atelier game = new Atelier())
            {
                game.Run();
            }
        }
    }
#endif
}

