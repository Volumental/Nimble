using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Nimble
{
    internal class Marshaller
    {
        public static IEnumerable<T> EnumerateStructArray<T>(IntPtr firstElement, int size)
        {
            IntPtr tmp = firstElement;
            for (int i = 0; i < size; i++)
            {
                T item = (T)Marshal.PtrToStructure(tmp, typeof(T));
                yield return item;
                tmp += Marshal.SizeOf(item);
            }
        }
    }
}
