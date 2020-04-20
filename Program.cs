using System;
using System.Collections.Generic;

namespace Timer
{
    class Program
    {
        static void Main(string[] args)
        {
            int icount = 0;
            Dictionary<int, Action> dict = new Dictionary<int, Action>();
            List<int> list = new List<int>();
            Random random = new Random();

            for (int i = 0; i < 150; ++i)
            {
                int index = i;
                long second = random.Next(0, 100);
                Action trigger = () => { Console.WriteLine(second + ", " + index + ", " + icount++); };
                int timerID = ETimer.Timer.Instance.Add(second, trigger);
                list.Add(timerID);
                dict.Add(timerID, trigger);
            }

            for (int i = 1; i < 150; i = i + 2)
            {
                int timerID = list[i];
                ETimer.Timer.Instance.Remove(timerID, dict[timerID]);
            }

            while (true)
            {
                ETimer.Timer.Instance.Poll();
                System.Threading.Thread.Sleep(1);
            }

            Console.WriteLine("Hello World!");
        }
    }
}