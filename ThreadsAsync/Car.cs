using System;
using System.Threading;

namespace ThreadsAsync
{
    public class Car
    {
        // Namn på bilen
        public string Name { get; set; }

        // Hastigheten i km i timmen. Startar på 120
        public double Speed { get; set; } = 120.0;

        // Hur långt bilen har kört i km
        public double DistanceTraveled { get; set; } = 0.0;

        // True om bilen har nått mållinjen, annars spinner den vidare med false
        public bool Finished { get; set; } = false;

        // Slumpgenerator. Måste va static för att samarbeta riktigt med flera trådar.
        private static Random random = new Random();

        // Skyddar data vid flera trådar med lås objekt
        private object lockObj = new object();

        public Car(string name)
        {
            Name = name;
        }

        public void Run()
        {
            Console.WriteLine($"{Name} har startat!");

            int secondsPassed = 0;

            // kör sålänge den e under 5km
            while (DistanceTraveled < 5.0)
            {
                Thread.Sleep(1000); 
                secondsPassed++;

                // Lås såd det ej blir problem om flera trådar ändrar samtidigt
                lock (lockObj)
                {
                    // Lägg till sträckan bilen kört på en sekund
                    DistanceTraveled += Speed / 3600.0; // km per sekund
                }

                // Var 10e sekund ska den slumpa en händelse
                if (secondsPassed % 10 == 0)
                {
                    HandleRandomEvent();
                }
            }

            // När bilen kört klart skickas meddelande till användaren 
            Finished = true;
            Console.WriteLine($"{Name} har gått i mål!");
        }

        // Här slumpas fram om bilen får ett problem
        public void HandleRandomEvent()
        {
            int chance;

            // Lås för att göra Random trådsäker
            lock (random)
            {
                chance = random.Next(1, 51); // slump generator
            }

            // Olika problem beroende på slumptalet enligt instruktion
            if (chance == 1)
            {
                Console.WriteLine($"{Name} fick slut på bensin! Stannar i 15 sekunder.");
                Thread.Sleep(15000);
            }
            else if (chance <= 3)
            {
                Console.WriteLine($"{Name} fick punktering! Stannar i 10 sekunder.");
                Thread.Sleep(10000);
            }
            else if (chance <= 8)
            {
                Console.WriteLine($"{Name} fick en fågel på vindrutan! Stannar i 5 sekunder.");
                Thread.Sleep(5000);
            }
            else if (chance <= 18)
            {
                Speed = Math.Max(1, Speed - 1); // Minska farten, men aldrig under 1 km/h
                Console.WriteLine($"{Name} fick motorfel! Hastigheten sänks till {Speed} km/h.");
            }
        }

        // Metod för att skriva ut status, hur långt o snabbt
        public string GetStatus()
        {
            return $"{Name}: {DistanceTraveled:F2} km, {Speed} km/h";
        }
    }
}
