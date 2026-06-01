using System;
using System.Collections.Generic;

namespace IT23575608_CoreDeveloper
{
    /// <summary>
    /// Custom Min-Heap Priority Queue implementation.
    /// This satisfies the Core Developer assignment requirement for a Priority Queue.
    /// </summary>
    /// <typeparam name="T">The type of the element being stored.</typeparam>
    public class MinHeap<T>
    {
        private class Node
        {
            public T Item { get; set; }
            public float Priority { get; set; }

            public Node(T item, float priority)
            {
                Item = item;
                Priority = priority;
            }
        }

        private List<Node> elements = new List<Node>();
        // Using a dictionary to track item indices allows O(log n) priority updates
        private Dictionary<T, int> itemIndices = new Dictionary<T, int>();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            // If item already exists, we simply update its priority
            if (itemIndices.ContainsKey(item))
            {
                UpdatePriority(item, priority);
                return;
            }

            elements.Add(new Node(item, priority));
            int currentIndex = elements.Count - 1;
            itemIndices[item] = currentIndex;
            HeapifyUp(currentIndex);
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Heap is empty");

            T rootItem = elements[0].Item;
            itemIndices.Remove(rootItem);

            int lastIndex = elements.Count - 1;
            if (lastIndex > 0)
            {
                elements[0] = elements[lastIndex];
                itemIndices[elements[0].Item] = 0;
                elements.RemoveAt(lastIndex);
                HeapifyDown(0);
            }
            else
            {
                elements.RemoveAt(0);
            }

            return rootItem;
        }

        public bool Contains(T item)
        {
            return itemIndices.ContainsKey(item);
        }

        private void UpdatePriority(T item, float newPriority)
        {
            if (!itemIndices.TryGetValue(item, out int index))
                return;

            float oldPriority = elements[index].Priority;
            elements[index].Priority = newPriority;

            if (newPriority < oldPriority)
                HeapifyUp(index);
            else if (newPriority > oldPriority)
                HeapifyDown(index);
        }

        private void HeapifyUp(int index)
        {
            int currentIndex = index;
            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;
                if (elements[currentIndex].Priority >= elements[parentIndex].Priority)
                    break;

                Swap(currentIndex, parentIndex);
                currentIndex = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            int currentIndex = index;
            int lastIndex = elements.Count - 1;

            while (true)
            {
                int leftChildIndex = 2 * currentIndex + 1;
                int rightChildIndex = 2 * currentIndex + 2;
                int smallestIndex = currentIndex;

                if (leftChildIndex <= lastIndex && elements[leftChildIndex].Priority < elements[smallestIndex].Priority)
                    smallestIndex = leftChildIndex;

                if (rightChildIndex <= lastIndex && elements[rightChildIndex].Priority < elements[smallestIndex].Priority)
                    smallestIndex = rightChildIndex;

                if (smallestIndex == currentIndex)
                    break;

                Swap(currentIndex, smallestIndex);
                currentIndex = smallestIndex;
            }
        }

        private void Swap(int indexA, int indexB)
        {
            Node temp = elements[indexA];
            elements[indexA] = elements[indexB];
            elements[indexB] = temp;

            itemIndices[elements[indexA].Item] = indexA;
            itemIndices[elements[indexB].Item] = indexB;
        }
    }
}
