using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03
{
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _elements = new();

        public void Add(T element) => _elements.Add(element);

        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetReverseEnumerator()
        {
            for (int index = _elements.Count - 1; index >= 0; index--)
            {
                yield return _elements[index];
            }
        }

        public static IEnumerable<int> GenerateSequence(int first, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Количество элементов не должно быть отрицательным.");
            }

            for (int i = 0; i < amount; i++)
            {
                yield return first + i;
            }
        }

        public IEnumerable<T> FilterAndSort(Func<T, bool> filterCondition, Func<T, IComparable> sortKey)
        {
            if (filterCondition == null)
            {
                throw new ArgumentNullException(nameof(filterCondition), "Условие фильтрации не может быть null.");
            }

            if (sortKey == null)
            {
                throw new ArgumentNullException(nameof(sortKey), "Ключ для сортировки не может быть null.");
            }

            return _elements.Where(filterCondition).OrderBy(sortKey);
        }
    }
}