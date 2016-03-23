using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace AtelierXNA
{
    public static class Serializer
    {
        public static BinaryFormatter Formatter { get; private set; }
        static MemoryStream Stream { get; set; }

        public static MemoryStream SerializeToStream(object toSave)
        {
            try
            {
                Stream = new MemoryStream();
                Formatter = new BinaryFormatter();
                Formatter.Serialize(Stream, toSave);
                return Stream;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new MemoryStream();
            }
        }

        public static T DeserializeFromStream<T>(MemoryStream stream)
        {
            Formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            return (T)Formatter.Deserialize(stream);
        }

        public static byte[] GetBytes(object o)
        {
            MemoryStream s = new MemoryStream();
            Formatter = new BinaryFormatter();

            Formatter.Serialize(s, o);

            return s.GetBuffer();
        }

        public static T GetObjectFromBytes<T>(byte[] buffer)
        {
            Stream = new MemoryStream(buffer);
            Formatter = new BinaryFormatter();

            return (T)Formatter.Deserialize(Stream);
        }
    }
}
