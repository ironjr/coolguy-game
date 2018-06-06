using System.Collections.Generic;
using System;

public class BinaryHeap<T>
{
    protected List<T> _items;

    // _comparer determines the type of this heap. Default is minheap.
    protected IComparer<T> _comparer;

    public int Count
    {
        get
        {
            return _items.Count;
        }
    }

    public BinaryHeap() : this(Comparer<T>.Default) { }

    public BinaryHeap(IComparer<T> comparer)
    {
        _items = new List<T>();
        _comparer = comparer;
    }

    public void Insert(T item)
    {
        // Insert the item to the end of the heap.
        _items.Add(item);

        // Do promotion of the inserted item until it reaches the appropriate
        // position.
        int i = _items.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_comparer.Compare(_items[i], _items[parent]) > 0)
            {
                // Swap.
                T temp = _items[i];
                _items[i] = _items[parent];
                _items[parent] = temp;

                // Promote.
                i = parent;
            }
            else
            {
                // Promotion finished.
                break;
            }
        }
    }

    public T RemoveRoot()
    {
        // Handle basic exceptions.
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("The heap is empty.");
        }

        // Backup root value of the heap.
        T root = _items[0];

        // Remove the root value.
        int len = _items.Count - 1;
        _items[0] = _items[len];
        _items.RemoveAt(len);

        // Rearrange the heap if needed.
        if (len > 0)
        {
            int i = 0;
            while (true)
            {
                int leftChild = (i << 1) + 1;
                int rightChild = (i << 1) + 2;
                int largest = i;
                if (leftChild < len && _comparer.Compare(_items[leftChild], _items[largest]) > 0)
                {
                    largest = leftChild;
                }
                if (rightChild < len && _comparer.Compare(_items[rightChild], _items[largest]) > 0)
                {
                    largest = rightChild;
                }
                
                // If changes has occurred.
                if (largest != i)
                {
                    // Swap.
                    T temp = _items[i];
                    _items[i] = _items[largest];
                    _items[largest] = temp;

                    // Promote.
                    i = largest;
                }
                else
                {
                    // Promotion finished.
                    break;
                }
            }
        }

        // Return the removed root value of the heap.
        return root;
    }

    public T Peek()
    {
        // Handle basic exceptions.
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("The heap is empty.");
        }
        return _items[0];
    }

    public bool IsEmpty()
    {
        if (_items.Count == 0) return true;
        else return false;
    }

    public void Clear()
    {
        _items.Clear();
    }
}