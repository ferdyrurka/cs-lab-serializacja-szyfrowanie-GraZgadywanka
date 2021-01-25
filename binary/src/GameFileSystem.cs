using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GraZaDuzoZaMalo.FileSystem
{
    public class GameFileSystem
    {
        public void Delete() {
            try {
                File.Delete("GameState.dat");
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak!. Zapis nie został usunięty.");
            }
        }

        public bool IsExist() {
            try {
                return File.Exists("GameState.dat");
            } catch (Exception) {
                Console.WriteLine("Coś poszło nie tak!. Nie da się sprawdzić czy plik istnieje!");
                return false;
            }
        }
    }
}
