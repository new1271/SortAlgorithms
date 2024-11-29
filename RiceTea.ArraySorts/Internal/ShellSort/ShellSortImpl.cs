﻿using InlineMethod;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RiceTea.ArraySorts.Internal.ShellSort
{
    internal static class ShellSortImpl
    {
#pragma warning disable CS8500 // 這會取得 Managed 類型的位址、大小，或宣告指向它的指標
        [Inline(InlineBehavior.Remove)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sort<T>(T[] array, int index, int count, IComparer<T> comparer)
        {
            Type type = typeof(T);
            if (type.IsPrimitive)
            {
                fixed (T* ptr = array)
                {
                    T* ptrStart = ptr + index;
                    T* ptrEnd = ptr + index + count;
                    if (comparer is null || comparer == Comparer<T>.Default)
                    {
                        ShellSortImplUnmanagedNC<T>.Sort(ptrStart, ptrEnd);
                        return;
                    }
                    ShellSortImplUnmanaged<T>.Sort(ptrStart, ptrEnd, comparer);
                }
                return;
            }
            if (comparer is null)
                comparer = Comparer<T>.Default;
            if (type.IsValueType)
            {
                fixed (T* ptr = array)
                {
                    T* ptrStart = ptr + index;
                    T* ptrEnd = ptr + index + count;
                    ShellSortImplUnmanaged<T>.Sort(ptrStart, ptrEnd, comparer);
                }
                return;
            }
            ShellSortImplManaged<T>.Sort(array, index, count, comparer);
        }

        [Inline(InlineBehavior.Remove)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sort<T>(IList<T> list, int index, int count, IComparer<T> comparer)
        {
            if (list is T[] array)
            {
                Sort(array, index, count, comparer);
                return;
            }
            ShellSortImplManaged<T>.Sort(list, index, count, comparer ?? Comparer<T>.Default);
        }
    }
}
