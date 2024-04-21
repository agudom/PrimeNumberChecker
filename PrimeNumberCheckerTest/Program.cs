using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumberCheckerTest
{
    internal class Program
    {
        /*
         * [] Evento
         * [] async/await
         * https://primes.utm.edu/curios/index.php?start=20&stop=24
         */

  
        private static void PrimeNumberChecker_ResultCompleted(ulong value, bool isPrime)
        {
             string strPrime = isPrime ? "" : "not ";
            Console.WriteLine($"{value} is {strPrime}prime.");            
        }

        private static void Checking()
        {
            Console.Write("Checking");
            while (PrimeNumberChecker.IsWorking)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Done!");
        }
        static async Task Main(string[] args)
        {
            Stopwatch timeMeasure = new Stopwatch();

            PrimeNumberChecker.ResultCompleted += PrimeNumberChecker_ResultCompleted;

            timeMeasure.Start();
            Task<bool> taskPrime = PrimeNumberChecker.IsPrimeAsync(18446744073709551557); //(10089886811898868001); //13790155021 //18446744073709551611
            Checking();
            await taskPrime;
            timeMeasure.Stop();

            Console.WriteLine($"Time: {timeMeasure.Elapsed.TotalSeconds}s ({Stopwatch.IsHighResolution})");

            //for (ulong i = 2; i <= 1000; i++)
            //{
            //    if (PrimeNumberChecker.IsPrimeAsync(i).Result)
            //        Console.WriteLine(i);
            //}         
            Console.ReadKey();
        }

        
    }
}
