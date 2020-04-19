using System;
using System.Collections.Generic;

namespace Timer
{
    class Program
    {
        static void Main(string[] args)
        {
            int icount = 0;
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            Dictionary<int, Action> dict = new Dictionary<int, Action>();
            List<int> list = new List<int>();

            for (int i = 0; i < 150; ++i)
            {
                int index = i;
                Action trigger = () =>
                {
                    Console.WriteLine(index);
                    Console.WriteLine(icount++);
                };
                int timerID = ETimer.Timer.Instance.Add((long)ts.TotalSeconds + i, trigger);
                list.Add(timerID);
                dict.Add(timerID, trigger);
            }

            for (int i = 1; i < 150; i = i + 2)
            {
                int timerID = list[i];
                ETimer.Timer.Instance.Remove(timerID, dict[timerID]);
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
