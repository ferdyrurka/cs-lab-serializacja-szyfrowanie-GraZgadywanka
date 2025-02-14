﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using GraZaDuzoZaMalo.Model;

namespace AppGraZaDuzoZaMaloCLI
{
    class WidokCLI
    {
        public const char ZNAK_ZAKONCZENIA_GRY = 'X';

        private KontrolerCLI kontroler;

        public WidokCLI(KontrolerCLI kontroler) => this.kontroler = kontroler;

        public void CzyscEkran() => Clear();

        public void KomunikatPowitalny() => WriteLine("Wylosowałem liczbę z zakresu ");

        public int WczytajPropozycje()
        {
            int wynik = 0;
            bool sukces = false;

            while (!sukces)
            {
                Write("Podaj swoją propozycję (lub " + KontrolerCLI.ZNAK_ZAKONCZENIA_GRY + " aby przerwać): ");
                try
                {
                    string value = ReadLine().TrimStart().ToUpper();
                    if (value.Length > 0 && value[0].Equals(ZNAK_ZAKONCZENIA_GRY)) {
                        throw new KoniecGryException();
                    }
                        

                    //UWAGA: ponizej może zostać zgłoszony wyjątek 
                    wynik = Int32.Parse(value);
                    sukces = true;
                }
                catch (KoniecGryException exception)
                {
                    throw exception;
                }
                catch (FormatException)
                {
                    WriteLine("Podana przez Ciebie wartość nie przypomina liczby! Spróbuj raz jeszcze.");
                    continue;
                }
                catch (OverflowException)
                {
                    WriteLine("Przesadziłeś. Podana przez Ciebie wartość jest zła! Spróbuj raz jeszcze.");
                    continue;
                }
                catch (Exception)
                {
                    WriteLine("Nieznany błąd! Spróbuj raz jeszcze.");
                    continue;
                }
            }
            return wynik;
        }

        public void OpisGry()
        {
            WriteLine("Gra w \"Za dużo za mało\"." + Environment.NewLine
                + "Twoimm zadaniem jest odgadnąć liczbę, którą wylosował komputer." + Environment.NewLine + "Na twoje propozycje komputer odpowiada: za dużo, za mało albo trafiłeś");
        }

        public bool ChceszKontynuowac( string prompt )
        {
            Write( prompt );
            char odp = ReadKey().KeyChar;
            WriteLine();

            return (odp == 't' || odp == 'T');
        }

        public bool ChceszKontynuowacStaraRozgrywke( string prompt )
        {
            WriteLine("Można kontynuować starą rozgrywke oto jest poprzedni stan:");

            HistoriaGry();

            Write( prompt );
            char odp = ReadKey().KeyChar;
            WriteLine();
            
            return (odp == 't' || odp == 'T');
        }

        public void HistoriaGry()
        {
            if (kontroler.ListaRuchow.Count == 0)
            {
                WriteLine("--- pusto ---");
                return;
            }

            WriteLine("Nr    Propozycja     Odpowiedź     Czas    Status");
            WriteLine("=================================================");

            int i = 1;
            DateTime previousTime = default(DateTime);
            double totalSeconds = 0;

            foreach ( var ruch in kontroler.ListaRuchow)
            {
                if (ruch.StatusGry != Gra.Status.Zawieszona) {
                    if (previousTime != default(DateTime)) {
                        TimeSpan timeSpan = ruch.Czas - previousTime;
                        totalSeconds += Math.Round(timeSpan.TotalSeconds);
                    }

                    previousTime = ruch.Czas;

                    WriteLine($"{i}     {ruch.Liczba}      {ruch.Wynik}  {totalSeconds}   {ruch.StatusGry}");
                } else {
                    WriteLine($"{i}                         {ruch.StatusGry}");
                    previousTime = default(DateTime);
                }

                i++;
            }
        }

        public void KomunikatZaDuzo()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine("Za dużo!");
            Console.ResetColor();
        }

        public void KomunikatZaMalo()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine("Za mało!");
            Console.ResetColor();
        }

        public void KomunikatTrafiono()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine("Trafiono!");
            Console.ResetColor();
        }
    }

}
