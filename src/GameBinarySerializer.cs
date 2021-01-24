using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GraZaDuzoZaMalo.Serializer
{
    public class GameBinarySerializer
    {
        public void Serialize<T>(T serializeObject)
        {
            FileStream fs;

            try {
                fs = new FileStream("GameState.dat", FileMode.OpenOrCreate);
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak! Nie można utworzyć ani otworzyć pliku z stanem gry!");
                return;
            }
            
            try
            {
                (new BinaryFormatter()).Serialize(fs, serializeObject);
            }
            catch (SerializationException)
            {
                Console.WriteLine("Coś poszło nie tak, stan gry nie został zapisany!");
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public T Deserialize<T>()
        {
            T deserializeObject;
            FileStream fs;

            try {
                fs = new FileStream("GameState.dat", FileMode.Open);
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak! Nie można otworzyć pliku z stanem gry!");
                return default(T);
            }
            
            try
            {
                deserializeObject = (T) (new BinaryFormatter()).Deserialize(fs);
            }
            catch (SerializationException)
            {
                Console.WriteLine("Coś poszło nie tak, plik zapisu gry jest uszkodzony!");
                return default(T);
            }
            finally
            {
                fs.Close();
            }

            return deserializeObject;
        }
    }
}
