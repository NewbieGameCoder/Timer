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

        public bool Empty => iCount > 0;

        public IEnumerable<T> Push(T handler)
        {
            if (iCount == heap.Length) EnsureCapacity(iCount + 1);

            var result = new Element { pos = iCount, entity = handler };
            PercolateUp(iCount++, result);
            return result;
        }

        public T Pop()
        {
            T result = default(T);

            if (iCount != 0)
            {
                result = heap[0].entity;
                heap[0] = heap[--iCount];
                PercolateDown(0);
            }

            return result;
        }

        public T Peek()
        {
            T result = default(T);

            if (iCount != 0)
            {
                result = heap[0].entity;
            }

            return result;
        }

        public void Assign(IEnumerable<T> handler, T entity)
        {
            if (!(handler is Element ele)) throw new Exception("Wrong PriorityQueue element type");
            
            ele.entity = entity;
        }

        public void Delete(IEnumerable<T> handler)
        {
            if (!(handler is Element ele)) throw new Exception("Wrong PriorityQueue element type");
            
            ele.entity.DeleteProcess();
            PercolateUp(ele.pos, ele);
            Pop();
        }

        private void PercolateDown(int index)
        {
            int leafIndex = SelectMinLeaf(index, iCount);
            while (heap[index].entity.CompareTo(heap[leafIndex].entity) < 0)
            {
                leafIndex /= 2;
            }

            var temp = heap[leafIndex];
            heap[leafIndex] = heap[index];
            heap[index].pos = leafIndex;

            while (leafIndex > index)
            {
                var oldData = heap[leafIndex / 2];
                heap[leafIndex / 2] = temp;
                temp.pos = leafIndex / 2;
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
                heap[i / 2].pos = i;
            }

            heap[i] = handler;
            handler.pos = i;
        }

        private int SelectMinLeaf(int start, int end)
        {
            int child = start * 2 + 1;

            while (child < end)
            {
                if (child + 1 > end) break;
                else if (heap[child].entity.CompareTo(heap[child + 1].entity) > 0) child = child * 2 + 2;
                else child = child * 2 + 1;
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

        private class Element : IEnumerable<T>
        {
            public int pos;
            public T entity;

            public IEnumerator<T> GetEnumerator()
            {
                yield return entity;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                yield return entity;
            }
        }

        private int iCount;
        private Element[] heap = new Element[32];
    }
}