using System;
using System.Collections.Generic;

namespace ETimer
{
    internal struct TimerHandler : IComparable<TimerHandler>, IPriorityQueueElement
    {
        public int pos;
        public long timestamp;

        public int CompareTo(TimerHandler other)
        {
            long result = timestamp - other.timestamp;

            if (result > 0) return 1;
            else if (result < 0) return -1;
            else return 0;
        }

        public void DeleteProcess()
        {
            timestamp = -1;
        }
    }

    public class Timer
    {
        public static Timer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Timer();
                }

                return _instance;
            }
        }

        public int Add(long timestamp, Action callback)
        {
            IEnumerable<TimerHandler> handlerHead = null;
            if (!handerGroup.TryGetValue(timestamp, out handlerHead) || handlerHead == null)
            {
                TimerHandler newHander = new TimerHandler();
                newHander.timestamp = timestamp;
                handlerHead = queue.Push(newHander);
                handerGroup[timestamp] = handlerHead;
            }

            int pos = -1;

            if (head.index != 0)
            {
                pos = head.index;
                list[pos].callback = callback;
                list[pos].timestamp = timestamp;
                head.index = list[pos].index;
            }
            else
            {
                pos = list.Count;
                list.Add(new TimerNode(pos, timestamp, callback));
                count = pos + 1;
            }

            return pos;
        }

        public void Remove(int timerID)
        {
            int pos = timerID;
            if (pos > 0 && pos < count)
            {
                Action o = list[pos].callback;
                list[pos].callback = null;
                list[pos].index = head.index;
                head.index = pos;
            }
        }

        public void Poll(long curTimestamp)
        {
            while (!queue.Empty)
            {
                var handler = queue.Peek();
                if (handler.timestamp <= curTimestamp)
                {
                    handler = queue.Pop();
                    handerGroup.Remove(handler.timestamp);
                    // handler.callback?.Invoke();
                }
                else break;
            }
        }

        private Timer()
        {
            list = new List<TimerNode>(128);
            head = new TimerNode(0, 0, null);
            list.Add(head);
            count = list.Count;
        }

        class TimerNode
        {
            public int index;
            public long timestamp;
            public Action callback;

            public TimerNode(int index, long timestamp, Action callback)
            {
                this.index = index;
                this.timestamp = timestamp;
                this.callback = callback;
            }
        }

        private static Timer _instance;

        private int count = 0;
        private List<TimerNode> list;
        private TimerNode head = null;
        private PriorityQueue<TimerHandler> queue = new PriorityQueue<TimerHandler>();
        private Dictionary<long, IEnumerable<TimerHandler>> handerGroup = new Dictionary<long, IEnumerable<TimerHandler>>();
    }
}