using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  // Borrowed from XUnit, licened under MS-PL
  class AssertComparer<T> : IComparer<T>, IEqualityComparer<T>
  {
    public int Compare(T x,
                       T y)
    {
        if (CanCompareEnumerable(x, y))
            return CompareEnumerable((IEnumerable)x, (IEnumerable)y);
        
        Type type = typeof(T);

      // Null?
      if (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
      {
          if (!AreBothNotNull(x, y))
          {
              return CompareNull(x, y);
          }
      }

      // Same type?
      if (x.GetType() != y.GetType())
        return -1;

      // Implements IComparable<T>?
      IComparable<T> comparable1 = x as IComparable<T>;

      if (comparable1 != null)
        return comparable1.CompareTo(y);

      // Implements IComparable?
      IComparable comparable2 = x as IComparable;

      if (comparable2 != null)
        return comparable2.CompareTo(y);

      // Implements IEquatable<T>?
      IEquatable<T> equatable = x as IEquatable<T>;

      if (equatable != null)
        return equatable.Equals(y) ? 0 : -1;

      // Last case, rely on Object.Equals
      return object.Equals(x, y) ? 0 : -1;
    }

      int CompareNull(T x, T y)
      {
          if (object.Equals(x, default(T)))
          {
              if (object.Equals(y, default(T)))
                  return 0;
              return -1;
          }

          if (object.Equals(y, default(T)))
              return -1;

          throw new NotSupportedException();
      }

      bool AreBothNotNull(T x, T y)
      {
          return !object.Equals(x, default(T)) && !object.Equals(y, default(T));
      }

      bool CanCompareEnumerable(T x, T y)
      {
          IEnumerable enumerableX = x as IEnumerable;
          IEnumerable enumerableY = y as IEnumerable;

          return enumerableX != null && enumerableY != null;
      }

      int CompareEnumerable(IEnumerable enumerableX, IEnumerable enumerableY)
      {
          IEnumerator enumeratorX = enumerableX.GetEnumerator();
          IEnumerator enumeratorY = enumerableY.GetEnumerator();

          while (true)
          {
              bool hasNextX = enumeratorX.MoveNext();
              bool hasNextY = enumeratorY.MoveNext();

              if (!hasNextX || !hasNextY)
                  return (hasNextX == hasNextY ? 0 : -1);

              if (!object.Equals(enumeratorX.Current, enumeratorY.Current))
                  return -1;
          }
      }

      public bool Equals(T x, T y)
    {
      return Compare(x, y) == 0;
    }

    public int GetHashCode(T obj)
    {
      return obj.GetHashCode();
    }
  }
}