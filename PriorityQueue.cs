using System;
using System.Collections;
using System.Collections.Generic;

namespace ETimer
{
    public interface IPriorityQueueElement
    {
        void DeleteProcess();
    }

    public class PriorityQueue<T> where T : IComparable<T>, IPriorityQueueElement
    {
        public bool Empty => iCount <= START_INDEX;
        public int Length => iCount - 1;

        public PriorityQueue(T defaultTopValue)
        {
            heap = new Element[32];
            var topEle = new Element { pos = iCount++, entity = defaultTopValue };
            heap[0] = topEle;
        }

        public IEnumerator<T> Push(T handler)
        {
            if (iCount == heap.Length) EnsureCapacity(iCount + 1);

            var result = new Element { pos = iCount, entity = handler };
            PercolateUp(iCount++, result);
            return result;
        }

        public bool Pop(out T result)
        {
            result = default(T);

            if (iCount != START_INDEX)
            {
                result = heap[START_INDEX].entity;
                heap[START_INDEX] = heap[--iCount];
                PercolateDown(START_INDEX);
                return true;
            }

            return false;
        }

        public bool Peek(out T result)
        {
            result = default(T);

            if (iCount != START_INDEX)
            {
                result = heap[START_INDEX].entity;
                return true;
            }

            return false;
        }

        public void Assign(IEnumerator<T> handler, T entity)
        {
            if (!(handler is Element ele)) throw new Exception("Wrong PriorityQueue element type");

            ele.entity = entity;
        }

        public void Delete(IEnumerator<T> handler)
        {
            if (!(handler is Element ele)) throw new Exception("Wrong PriorityQueue element type");

            ele.entity.DeleteProcess();
            PercolateUp(ele.pos, ele);
            if (!Pop(out _)) throw new Exception("Delete failed");
        }

        public void Clear()
        {
            Array.Clear(heap, START_INDEX, heap.Length);
            iCount = START_INDEX;
        }

        private void PercolateDown(int index)
        {
            int leafIndex = SelectPerfectLeaf(index, iCount);
            while (heap[index].entity.CompareTo(heap[leafIndex].entity) < 0)
            {
                leafIndex /= 2;
            }

            var temp = heap[leafIndex];
            heap[leafIndex] = heap[index];
            heap[leafIndex].pos = leafIndex;

            while (leafIndex > index)
            {
                var oldData = heap[leafIndex / 2];
                heap[leafIndex / 2] = temp;
                heap[leafIndex / 2].pos = leafIndex / 2;
                leafIndex /= 2;
                temp = oldData;
            }
        }

        private void PercolateUp(int index, Element handler)
        {
            int i;
            for (i = index; heap[i / 2].entity.CompareTo(handler.entity) > 0; i /= 2)
            {
                heap[i] = heap[i / 2];
                heap[i].pos = i;
            }

            heap[i] = handler;
            heap[i].pos = i;
        }

        private int SelectPerfectLeaf(int start, int end)
        {
            int child = start;

            while (true)
            {
                if (child + 1 >= end) break;

                int nextChild = child * 2;
                if (nextChild >= end) break;
                if (heap[nextChild].entity.CompareTo(heap[nextChild + 1].entity) > 0) nextChild = nextChild + 1;
                child = nextChild;
            }

            return child;
        }

        private void EnsureCapacity(int min)
        {
            int newCapacity = heap.Length << 1;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when list.Length overflowed thanks to the (uint) cast
            if ((uint)newCapacity > 0X7FEFFFFF) newCapacity = 0X7FEFFFFF;
            if (newCapacity < min) newCapacity = min;

            Element[] newArray = new Element[newCapacity];
            Array.ConstrainedCopy(heap, 0, newArray, 0, iCount);
            heap = newArray;
        }

        private class Element : IEnumerator<T>
        {
            public int pos;
            public T entity;
            public T Current => entity;
            object IEnumerator.Current => Current;
            
            public void Reset()
            {
                current = default(T);
            }

            public bool MoveNext()
            {
                if (current.CompareTo(entity) != 0)
                {
                    current = entity;
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
            }
            
            private T current;
        }

        private const int START_INDEX = 1;
        private int iCount;
        private Element[] heap;
    }
}