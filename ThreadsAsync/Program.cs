using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadsAsync
{
    public class Program
    {
        // Lista med alla bilar i tävlingen
        static List<Car> cars = new List<Car>();

        // Tråd lista för varje bil
        static List<Thread> threads = new List<Thread>();

        // Meddela för att veta om någon bil har vunnit
        static bool raceFinished = false;

        // Låsobjekt för att skriva säkert till konsolen
        static object consoleLock = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Biltävlingen börjar inom kort!");

            // Skapa bilar och lägg till i listan
            cars.Add(new Car("BMW"));
            cars.Add(new Car("Benz"));

            // Starta en tråd för varje bil
            foreach (Car car in cars)
            {
                Thread t = new Thread(car.Run); // Skapa tråd som kör bilens Run metod
                threads.Add(t);
                t.Start(); 
            }

            // Starta en separat tråd för att lyssna på användarens aktivitet
            Thread inputThread = new Thread(HandleInput);
            inputThread.Start();

            // Checkar om bil kliver i mål
            while (!raceFinished)
            {
                foreach (Car car in cars)
                {
                    if (car.Finished)
                    {
                        // Första bilen som går i mål vinner
                        raceFinished = true;
                        lock (consoleLock)
                        {
                            Console.WriteLine($"\n {car.Name} har vunnit racet!");
                        }
                        break;
                    }
                }

                Thread.Sleep(500); // Delay
            }

            // Vänta på att alla trådar avslutas innan den går vidare
            foreach (Thread t in threads)
            {
                t.Join();
            }

            Console.WriteLine("\nTävlingen är finito!");
        }

        // Denna metod körs i bakgrunden och väntar på att användaren trycker Enter
        static void HandleInput()
        {
            while (!raceFinished)
            {
                string input = Console.ReadLine(); // Väntar på att användaren skriver något
                if (input == "" || input.ToLower() == "status")
                {
                    // Skriv ut status för varje bil
                    lock (consoleLock)
                    {
                        Console.WriteLine("\nNuläges-RAPPORT:");
                        foreach (Car car in cars)
                        {
                            Console.WriteLine(car.GetStatus());
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
