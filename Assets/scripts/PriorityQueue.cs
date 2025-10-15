using System.Collections.Generic;

using System;
using System.Collections.Generic;

public class PriorityQueue<T> : IDisposable
{
    private List<(T item, float priority)> heap = new();

    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        var result = heap[0].item;
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 0)
            HeapifyDown(0);

        return result;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[index].priority >= heap[parentIndex].priority)
                break;

            (heap[index], heap[parentIndex]) = (heap[parentIndex], heap[index]);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int smallest = index;
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;

            if (leftChild < heap.Count && heap[leftChild].priority < heap[smallest].priority)
                smallest = leftChild;

            if (rightChild < heap.Count && heap[rightChild].priority < heap[smallest].priority)
                smallest = rightChild;

            if (smallest == index)
                break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }

    public void Clear()
    {
        heap.Clear();
    }

    public void Dispose()
    {
        Clear();
        heap = null;
    }
}