
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumberCheckerTest
{
    public static class PrimeNumberChecker
    {
        public delegate void result(ulong value, bool isPrime);
        public static event result ResultCompleted;

        public static bool IsWorking { get; private set; }

        private static IEnumerable<ulong> GetNext(ulong first, ulong last)
        {
            for (ulong i = first; i <= last; i += 10)
                yield return i; 
        }

        private static void IsPossiblePrime(ulong value, ulong first, ulong last, AutoResetEvent success, AutoResetEvent failure)
        {
            foreach (ulong v in GetNext(first, last))
                if (value % v == 0)
                    failure.Set();

            success.Set();
        }

        public static async Task<bool> IsPrimeAsync(ulong value)
        {
            IsWorking = true;

            await Task.Yield();

            bool isPrime = false;

            if (value < 2) throw new ArgumentOutOfRangeException("value");
            else if (value == 2 || value == 3 || value == 5 || value == 7) isPrime = true;
            else if (value % 2 == 0 || value % 5 == 0) isPrime = false;       
            else
            {          
                ulong boundary = (ulong)Math.Floor(Math.Sqrt(value));
                                
                AutoResetEvent success = new AutoResetEvent(false);
                AutoResetEvent failure = new AutoResetEvent(false);
                AutoResetEvent succeeded3 = new AutoResetEvent(false);
                AutoResetEvent succeeded7 = new AutoResetEvent(false);
                AutoResetEvent succeeded9 = new AutoResetEvent(false);
                AutoResetEvent succeeded11 = new AutoResetEvent(false);

                List<Thread> threads = new List<Thread>();
                threads.Add(new Thread(() => IsPossiblePrime(value, 3, boundary, succeeded3, failure)));
                threads.Add(new Thread(() => IsPossiblePrime(value, 7, boundary, succeeded7, failure)));
                threads.Add(new Thread(() => IsPossiblePrime(value, 9, boundary, succeeded9, failure)));
                threads.Add(new Thread(() => IsPossiblePrime(value, 11, boundary, succeeded11, failure)));

                threads.Add(new Thread(() => {
                                                AutoResetEvent.WaitAll(new WaitHandle[] { succeeded3, succeeded7, succeeded9, succeeded11 }); 
                                                success.Set();
                                             }));

                foreach (Thread t in threads)                
                    t.Start();

                isPrime = Convert.ToBoolean(AutoResetEvent.WaitAny(new WaitHandle[] { failure, success }));

                foreach (Thread t in threads)
                {
                    try { t.Abort(); } catch (Exception) { }
                }                
            }

            IsWorking = false;

            if (ResultCompleted != null)
                ResultCompleted.Invoke(value, isPrime);

            return isPrime;
        }
    }
}

/*
 
using System;
using System.Collections.Generic;
using System.Threading;

namespace PrimeNumberCheckerTest
{
    public static class PrimeNumberChecker
    {
        private static IEnumerable<ulong> GetNext(ulong first, ulong last)
        {
            for (ulong i = first; i <= last; i += 10)
                yield return i; 
        }

        private static bool IsPossiblePrime(ulong value, ulong first, ulong last)
        {
            foreach (ulong v in GetNext(first, last))
                if (value % v == 0)
                    return false;

            return true;
        }

        public static bool IsPrime(ulong value)
        {
            if (value < 2) throw new ArgumentOutOfRangeException("value");
            else if (value == 2 || value == 3 || value == 5 || value == 7) return true;
            else if (value % 2 == 0 || value % 5 == 0) return false;       
            else
            {
                ulong boundary = (ulong)Math.Floor(Math.Sqrt(value));

                bool isPossiblePrime3 = false, isPossiblePrime7 = false, isPossiblePrime9 = false, isPossiblePrime11 = false;
                
                List<Thread> threads = new List<Thread>();
                threads.Add(new Thread(() => isPossiblePrime3 = IsPossiblePrime(value, 3, boundary)));
                threads.Add(new Thread(() => isPossiblePrime7 = IsPossiblePrime(value, 7, boundary)));
                threads.Add(new Thread(() => isPossiblePrime9 = IsPossiblePrime(value, 9, boundary)));
                threads.Add(new Thread(() => isPossiblePrime11 = IsPossiblePrime(value, 11, boundary)));

                foreach (Thread t in threads)                
                    t.Start();

                foreach (Thread t in threads)                   
                    t.Join();
     
                return isPossiblePrime3 && isPossiblePrime7 && isPossiblePrime9 && isPossiblePrime11;

                //return (
                //    IsPossiblePrime(value, 3, boundary) &&
                //    IsPossiblePrime(value, 7, boundary) &&
                //    IsPossiblePrime(value, 9, boundary) &&
                //    IsPossiblePrime(value, 11, boundary)
                //    );
            }
            
        }
    }
}
*/