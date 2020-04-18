using System;

namespace Timer
{
    class Program
    {
        static void Main(string[] args)
        {
            int icount = 0;
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            for (int i = 0; i < 150; ++i)
            {
                int index = i;
                ETimer.Timer.Instance.Add((long)ts.TotalSeconds + i, () =>
                {
                    Console.WriteLine(index);
                    Console.WriteLine(icount++);
                });
            }

            while (icount < 150)
            {
                ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                ETimer.Timer.Instance.Poll((long)ts.TotalSeconds);
                System.Threading.Thread.Sleep(1);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
