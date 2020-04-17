using System;
using System.Collections.Generic;

namespace ETimer
{
    internal struct TimerHandler : IComparable<TimerHandler>, IPriorityQueueElement
    {
        public int poolPos;
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
            int pos = -1;

            TimerNode node;
            if (head.index != 0)
            {
                pos = head.index;
                node = list[pos];
                node.callback = callback;
                node.timestamp = timestamp;
                head.index = node.index;
            }
            else
            {
                pos = list.Count;
                list.Add(new TimerNode(pos, timestamp, callback));
                node = list[pos];
                count = pos + 1;
            }

            Enqueue(pos, node);

            return pos;
        }

        public void Remove(int timerID, Action callback)
        {
            int pos = timerID;
            if (pos > 0 && pos < count)
            {
                var node = list[pos];
                if (node.timestamp == -1 || node.callback != callback)
                {
                    throw new Exception("Remove repeatedly");
                }

                Dequeue(pos, node);

                node.callback = null;
                node.timestamp = -1;
                node.nextSiblingIndex = -1;
                node.index = head.index;
                head.index = pos;
            }
            else throw new Exception("Wrong timerID");
        }

        public void Poll(long curTimestamp)
        {
            while (!queue.Empty)
            {
                var handler = queue.Peek();
                if (handler.timestamp <= curTimestamp)
                {
                    handler = queue.Pop();
                    handlerGroup.Remove(handler.timestamp);

                    int headPos = handler.poolPos;
                    var headNode = list[headPos];
                    headNode.callback?.Invoke();
                    if (headNode.nextSiblingIndex != -1)
                    {
                        var tailNode = list[headNode.nextSiblingIndex];
                        ReverseTrigger(tailNode);
                    }
                }
                else break;
            }
        }

        private void ReverseTrigger(TimerNode node)
        {
            if (node.nextSiblingIndex != -1)
            {
                var nextNode = list[node.nextSiblingIndex];
                ReverseTrigger(nextNode);

                node.callback?.Invoke();
            }
        }

        private void Enqueue(int pos, TimerNode node)
        {
            int headPos = -1;
            if (handlerGroup.TryGetValue(node.timestamp, out var handlerHead))
            {
                headPos = handlerHead.GetEnumerator().Current.poolPos;
            }

            if (headPos != -1)
            {
                var headNode = list[headPos];
                node.nextSiblingIndex = headNode.nextSiblingIndex;
                headNode.nextSiblingIndex = pos;
            }

            if (handlerHead == null)
            {
                TimerHandler newHander = new TimerHandler {timestamp = node.timestamp, poolPos = pos};
                handlerHead = queue.Push(newHander);
                handlerGroup[node.timestamp] = handlerHead;
            }
        }

        private void Dequeue(int pos, TimerNode node)
        {
            if (handlerGroup.TryGetValue(node.timestamp, out var handlerHead))
            {
                var handler = handlerHead.GetEnumerator().Current;
                if (handler.poolPos == pos)
                {
                    if (node.nextSiblingIndex == -1)
                    {
                        queue.Delete(handlerHead);
                        handlerGroup.Remove(node.timestamp);
                    }
                    else
                    {
                        var newHandler = new TimerHandler()
                            {poolPos = node.nextSiblingIndex, timestamp = node.timestamp};
                        queue.Assign(handlerHead, newHandler);
                    }
                }
            }
            else throw new Exception("Remove repeatedly");
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
            public int nextSiblingIndex;
            public long timestamp;
            public Action callback;

            public TimerNode(int index, long timestamp, Action callback)
            {
                nextSiblingIndex = -1;
                this.index = index;
                this.timestamp = timestamp;
                this.callback = callback;
            }
        }

        private static Timer _instance;

        private int count;
        private List<TimerNode> list;
        private TimerNode head;
        private PriorityQueue<TimerHandler> queue = new PriorityQueue<TimerHandler>();

        private Dictionary<long, IEnumerable<TimerHandler>> handlerGroup =
            new Dictionary<long, IEnumerable<TimerHandler>>();
    }
}