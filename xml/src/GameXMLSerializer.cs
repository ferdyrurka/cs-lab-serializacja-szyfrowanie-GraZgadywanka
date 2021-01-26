using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GraZaDuzoZaMalo.Serializer
{
    public class GameXMLSerializer
    {
        public void Serialize<T>(T serializeObject)
        {
            FileStream fs;

            try {
                fs = new FileStream("GameState.xml", FileMode.Create);
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak! Nie można utworzyć ani otworzyć pliku z stanem gry!");
                return;
            }
            
            try
            {
                (new DataContractSerializer(typeof(T))).WriteObject(fs, serializeObject);
            }
            catch (Exception)
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
            FileStream fs;

            try {
                fs = new FileStream("GameState.xml", FileMode.Open);
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak! Nie można otworzyć pliku z stanem gry!");
                return default(T);
            }
            
            try
            {
                return (T) (new DataContractSerializer(typeof(T))).ReadObject(fs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Coś poszło nie tak, plik zapisu gry jest uszkodzony!");
                return default(T);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
