using System;
using System.Collections.Generic;

namespace Timer
{
    class Program
    {
        struct QueueEle : IComparable<QueueEle>, ETimer.IPriorityQueueElement
        {
            public int value;

            public int CompareTo(QueueEle other)
            {
                long result = value - other.value;

                if (result > 0) return 1;
                else if (result < 0) return -1;
                else return 0;
            }

            public void DeleteProcess()
            {
                value = -1;
            }

            public static QueueEle GetDefaultTopElement()
            {
                return new QueueEle() { value = -2 };
            }
        }

        static void Main(string[] args)
        {
            TestPriorityQueue();
            TestTimer();
        }

        static void TestPriorityQueue()
        {
            ETimer.PriorityQueue<QueueEle> queue = new ETimer.PriorityQueue<QueueEle>(QueueEle.GetDefaultTopElement());
            Random random = new Random();
            List<int> list = new List<int>(1500);

            for (int i = 0; i < 1500; ++i)
            {
                list.Add(i);
            }

            while (list.Count > 0)
            {
                int index = random.Next(0, list.Count);
                queue.Push(new QueueEle() { value = list[index] });
                list.RemoveAt(index);
            }

            int lastValue = -1;
            while (!queue.Empty)
            {
                if (queue.Pop(out var ele))
                {
                    if (ele.value - lastValue > 1 || ele.value < lastValue)
                    {
                        Console.WriteLine("lastValue: " + lastValue + "    " + "curValue: " + ele.value);
                    }
                    lastValue = ele.value;
                }
            }

            Console.WriteLine("Test queue end");
        }

        static void TestTimer()
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
        }
    }
}