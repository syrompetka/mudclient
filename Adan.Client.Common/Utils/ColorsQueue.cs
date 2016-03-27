using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Adan.Client.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ColorsQueue<T> : IEnumerable<RangeValuePair<T>>, IEnumerable
    {
        private IRange[] _keys;
        private T[] _values;
        private int _head;
        private int _tail;
        private int _size;
        private long _version;
        private long _offset = 0;

        /// <summary>
        /// 
        /// </summary>
        public ColorsQueue()
        {
            this._keys = new IRange[0];
            this._values = new T[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this._size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (this._head < this._tail)
            {
                Array.Clear(this._keys, this._head, this._size);
                Array.Clear(this._values, this._head, this._size);
            }
            else
            {
                Array.Clear(this._keys, this._head, this._keys.Length - this._head);
                Array.Clear(this._keys, 0, this._tail);
                Array.Clear(this._values, this._head, this._values.Length - this._head);
                Array.Clear(this._values, 0, this._tail);
            }

            this._head = 0;
            this._tail = 0;
            this._size = 0;
            this._offset = 0;
            ++this._version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Enqueue(IRange key, T value)
        {
            if (this._size > 0 && key.Low <= this.GetRange(this._size - 1).High)
                throw new ArgumentException();

            if (this._size == this._keys.Length)
            {
                int capacity = (int)((long)this._keys.Length * 2L);
                if (capacity < this._keys.Length + 4)
                    capacity = this._keys.Length + 4;

                this.SetCapacity(capacity);
            }

            this._keys[this._tail] = key;
            this._values[this._tail] = value;
            this._tail = (this._tail + 1) % this._keys.Length;
            ++this._size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ColorsQueue<T>.ColorsQueueEnumerator GetEnumerator()
        {
            return new ColorsQueue<T>.ColorsQueueEnumerator(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RangeValuePair<T> Dequeue()
        {
            if (this._size == 0)
                throw new ArgumentException();

            var obj = new RangeValuePair<T>(this.GetRange(0), this.GetValue(0));
            this._keys[this._head] = default(IRange);
            this._values[this._head] = default(T);
            this._head = (this._head + 1) % this._keys.Length;
            --this._size;
            ++this._version;
            this._offset = this._keys[this._head].Low;

            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RangeValuePair<T> Dequeue(int index)
        {
            if (this._size == 0)
                throw new ArgumentException();

            for (int i = 0; i < index; ++i)
            {
                var range = this.GetRange(i);
                range = default(IRange);
                var value = this.GetValue(i);
                value = default(T);
            }

            this._head = (this._head + index) % this._keys.Length;
            this._size -= index;
            ++this._version;

            return new RangeValuePair<T>(this.GetRange(0), this.GetValue(0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IRange GetRange(int index)
        {
            return this._keys[(this._head + index) % this._keys.Length];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetValue(int index)
        {
            return this._values[(this._head + index) % this._values.Length];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T[] KeysToArray()
        {
            T[] objArray = new T[this._size];

            if (this._size == 0)
                return objArray;

            if (this._head < this._tail)
            {
                Array.Copy(this._values, this._head, objArray, 0, this._size);
            }
            else
            {
                Array.Copy(this._values, this._head, objArray, 0, this._values.Length - this._head);
                Array.Copy(this._values, 0, objArray, this._values.Length - this._head, this._tail);
            }

            return objArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IRange[] ValuesToArray()
        {
            IRange[] objArray = new IRange[this._size];

            if (this._size == 0)
                return objArray;

            if (this._head < this._tail)
            {
                Array.Copy(this._keys, this._head, objArray, 0, this._size);
            }
            else
            {
                Array.Copy(this._keys, this._head, objArray, 0, this._keys.Length - this._head);
                Array.Copy(this._keys, 0, objArray, this._keys.Length - this._head, this._tail);
            }

            return objArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int GetIndexForElement(int element)
        {
            return FindIndexForElement(element);
        }

        private int FindIndexForElement(int element)
        {
            if (this._size == 0)
                return -1;

            int low = 0;
            int high = this._size;
            while (low <= high)
            {
                int middle = (low == high) ? low : ((low + high) / 2);
                var curRange = this.GetRange(middle);

                if (curRange.Contains(element))
                {
                    return middle;
                }
                else if (curRange.Low > element)
                {
                    high = middle - 1;
                }
                else if (curRange.High < element)
                {
                    low = middle + 1;
                }
                else
                {
                    return -middle;
                }
            }

            return -1;
        }

        private void SetCapacity(int capacity)
        {
            IRange[] keysArray = new IRange[capacity];
            T[] valuesArray = new T[capacity];

            if (this._size > 0)
            {
                if (this._head < this._tail)
                {
                    Array.Copy(this._keys, this._head, keysArray, 0, this._size);
                    Array.Copy(this._values, this._head, valuesArray, 0, this._size);
                }
                else
                {
                    Array.Copy(this._keys, this._head, keysArray, 0, this._keys.Length - this._head);
                    Array.Copy(this._keys, 0, keysArray, this._keys.Length - this._head, this._tail);
                    Array.Copy(this._values, this._head, valuesArray, 0, this._values.Length - this._head);
                    Array.Copy(this._values, 0, valuesArray, this._values.Length - this._head, this._tail);
                }
            }

            this._keys = keysArray;
            this._values = valuesArray;
            this._head = 0;
            this._tail = this._size == capacity ? 0 : this._size;
            ++this._version;
        }

        /// <summary>
        /// 
        /// </summary>
        public void TrimExcess()
        {
            if (this._size >= ((double)this._keys.Length * 0.9))
                return;

            this.SetCapacity(this._size);
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public struct ColorsQueueEnumerator : IEnumerator<RangeValuePair<T>>, IDisposable, IEnumerator
        {
            private ColorsQueue<T> _q;
            private int _index;
            private long _version;
            private RangeValuePair<T> _currentElement;

            RangeValuePair<T> IEnumerator<RangeValuePair<T>>.Current
            {
                get
                {
                    if (this._index < 0)
                    {
                        if (this._index == -1)
                            throw new ArgumentException();
                        else
                            throw new ArgumentException();
                    }

                    return this._currentElement;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this._index < 0)
                    {
                        if (this._index == -1)
                            throw new ArgumentException();
                        else
                            throw new ArgumentException();
                    }

                    return (object)this._currentElement;
                }
            }

            internal ColorsQueueEnumerator(ColorsQueue<T> q)
            {
                this._q = q;
                this._version = this._q._version;
                this._index = -1;
                this._currentElement = default(RangeValuePair<T>);
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                this._index = -2;
                this._currentElement = default(RangeValuePair<T>);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (this._version != this._q._version)
                    throw new ArgumentException();

                if (this._index == -2)
                    return false;

                ++this._index;
                if (this._index == this._q._size)
                {
                    this._index = -2;
                    this._currentElement = default(RangeValuePair<T>);
                    return false;
                }
                else
                {
                    this._currentElement = new RangeValuePair<T>(this._q.GetRange(this._index), this._q.GetValue(this._index));
                    return true;
                }
            }

            void IEnumerator.Reset()
            {
                if (this._version != this._q._version)
                    throw new ArgumentException();

                this._index = -1;
                this._currentElement = default(RangeValuePair<T>);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ColorsQueue<T>.ColorsQueueEnumerator(this);
        }

        IEnumerator<RangeValuePair<T>> IEnumerable<RangeValuePair<T>>.GetEnumerator()
        {
            return new ColorsQueue<T>.ColorsQueueEnumerator(this);
        }
    }

    /// <summary>
    /// Interface for a range.
    /// </summary>
    public interface IRange
    {
        /// <summary>
        /// The low end of the range.
        /// </summary>
        int Low
        {
            get;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        int High
        {
            get;
        }

        /// <summary>
        /// Whether or not the range contains the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Contains(int element);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        void ChangeRange(int low, int high);
    }

    /// <summary>
    /// The range structure.
    /// </summary>
    [Serializable]
    public struct Range : IRange
    {
        /// <summary>
        /// The low end of the range.
        /// </summary>
        private int _low;

        /// <summary>
        /// The high end of the range.
        /// </summary>
        private int _high;

        /// <summary>
        /// Create the given range.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public Range(int low, int high)
        {
            if (low > high)
                throw new ArgumentOutOfRangeException();

            this._low = low;
            this._high = high;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Low
        {
            get
            {
                return _low;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int High
        {
            get
            {
                return _high;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanChangeRange
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public bool Contains(int elem)
        {
            return (elem >= _low && elem <= _high);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public void ChangeRange(int low, int high)
        {
            if (low > high)
                throw new ArgumentOutOfRangeException();

            _low = low;
            _high = high;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct RangeValuePair<T>
    {
        private IRange _range;
        private T _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        public RangeValuePair(IRange range, T value)
        {
            this._range = range;
            this._value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public IRange Range
        {
            get
            {
                return _range;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                return _value;
            }
        }
    }
}