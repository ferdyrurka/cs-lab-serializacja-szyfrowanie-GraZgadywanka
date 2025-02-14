﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GraZaDuzoZaMalo.Model;
using static GraZaDuzoZaMalo.Model.Gra.Odpowiedz;
using GraZaDuzoZaMalo.Serializer;
using GraZaDuzoZaMalo.FileSystem;

namespace AppGraZaDuzoZaMaloCLI
{
    public class KontrolerCLI
    {
        public const char ZNAK_ZAKONCZENIA_GRY = 'X';

        private Gra gra;
        private WidokCLI widok;

        public int MinZakres { get; private set; } = 1;
        public int MaxZakres { get; private set; } = 100;

        public IReadOnlyList<Gra.Ruch> ListaRuchow {
            get
            { return gra.ListaRuchow;  }
 }

        public KontrolerCLI()
        {
            gra = new Gra();
            widok = new WidokCLI(this);
        }

        public void Uruchom()
        {
            widok.OpisGry();
    
            while(widok.ChceszKontynuowac("Czy chcesz kontynuować aplikację (t/n)? ")) {
                UruchomRozgrywke();
            }
        }

        public void UruchomRozgrywke()
        {
            widok.CzyscEkran();

            GameFileSystem gameFileSystem = new GameFileSystem();

            if (gameFileSystem.IsExist()) {
                GameXMLSerializer gameXMLSerializer = new GameXMLSerializer();
                gra = gameXMLSerializer.Deserialize<Gra>();

                if (gra == null || ListaRuchow.Count == 0 || !widok.ChceszKontynuowacStaraRozgrywke("Czy chcesz kontynuować starą rozgrywke (t/n)? ")) {
                    if (gra != null) {
                        gra.Zakoncz();
                    }
                    
                    gra = new Gra(MinZakres, MaxZakres);
                }

                gameFileSystem.Delete();
            } else {
                gra = new Gra(MinZakres, MaxZakres);
            }

            do
            {
                //wczytaj propozycję
                int propozycja = 0;
                try
                {
                    if (gra.StatusGry == Gra.Status.Zawieszona) {
                        gra.Wznow();
                    }
                    
                    propozycja = widok.WczytajPropozycje();
                }
                catch(KoniecGryException)
                {
                    GameXMLSerializer gameXMLSerializer = new GameXMLSerializer();
                    
                    gra.Przerwij();

                    gameXMLSerializer.Serialize<Gra>(gra);
                }

                Console.WriteLine(propozycja);

                if (gra.StatusGry == Gra.Status.Poddana) break;

                //Console.WriteLine( gra.Ocena(propozycja) );
                //oceń propozycję, break
                switch( gra.Ocena(propozycja) )
                {
                    case ZaDuzo:
                        widok.KomunikatZaDuzo();
                        break;
                    case ZaMalo:
                        widok.KomunikatZaMalo();
                        break;
                    case Trafiony:
                        widok.KomunikatTrafiono();
                        break;
                    default:
                        break;
                }
                widok.HistoriaGry();
            }
            while (gra.StatusGry == Gra.Status.WTrakcie);

            Console.WriteLine(gra.StatusGry);
                      
            //if StatusGry == Przerwana wypisz poprawną odpowiedź
            //if StatusGry == Zakończona wypisz statystyki gry
        }

        ///////////////////////

        public void UstawZakresDoLosowania(ref int min, ref int max)
        {

        }

        public int LiczbaProb() => gra.ListaRuchow.Count();

        public void ZakonczGre()
        {
            //np. zapisuje stan gry na dysku w celu późniejszego załadowania
            //albo dopisuje wynik do Top Score
            //sprząta pamięć
            gra = null;
            widok.CzyscEkran(); //komunikat o końcu gry
            widok = null;
            System.Environment.Exit(0);
        }

        public void ZakonczRozgrywke()
        {
            gra.Przerwij();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <exception cref="KoniecGryException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <returns></returns>
        public int WczytajLiczbeLubKoniec(string value, int defaultValue )
        {
            if( string.IsNullOrEmpty(value) )
                return defaultValue;

            value = value.TrimStart().ToUpper();
            if ( value.Length>0 && value[0].Equals(ZNAK_ZAKONCZENIA_GRY))
                throw new KoniecGryException();

            //UWAGA: ponizej może zostać zgłoszony wyjątek 
            return Int32.Parse(value);
        }
    }

    [Serializable]
    internal class KoniecGryException : Exception
    {
        public KoniecGryException()
        {
        }

        public KoniecGryException(string message) : base(message)
        {
        }

        public KoniecGryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KoniecGryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
