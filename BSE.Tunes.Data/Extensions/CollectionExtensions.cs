using BSE.Tunes.Data.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Data.Extensions
{
    public static class CollectionExtensions
    {
        public static NavigableCollection<T> ToNavigableCollection<T>(this ObservableCollection<T> collection)
        {
            NavigableCollection<T> collectionTo = null;
            if (collection != null)
            {
                collectionTo = new NavigableCollection<T>();
                foreach (T rec in collection)
                {
                    collectionTo.Add((T)rec);
                }
            }
            return collectionTo;
        }
        public static ObservableCollection<T> ToRandomCollection<T>(this ObservableCollection<T> collection)
        {
            ObservableCollection<T> randomCollection = null;
            if (collection != null)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                while (collection.Count > 0)
                {
                    int iIndex = random.Next(collection.Count);
                    if (randomCollection == null)
                    {
                        randomCollection = new ObservableCollection<T>();
                    }
                    T obj = collection[iIndex];
                    randomCollection.Add(obj);
                    collection.RemoveAt(iIndex);
                }
            }
            return randomCollection;
        }
    }
}
