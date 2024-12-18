﻿
using InlineMethod;

using RiceTea.ArraySorts.Internal.BinaryInsertionSort;
using RiceTea.ArraySorts.Internal.ShellSort;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RiceTea.ArraySorts.Internal.InPlaceMergeSort
{
#pragma warning disable CS8500 // 這會取得 Managed 類型的位址、大小，或宣告指向它的指標
    internal static unsafe class InPlaceMergeSortImplUnmanaged<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort(T* ptr, T* ptrEnd, IComparer<T> comparer)
        {
            long count = ptrEnd - ptr;
            if (count < 2L || SortUtils.OptimizeSort(ptr, ptrEnd, count, comparer))
                return;
            SortCore(ptr, ptrEnd, count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortWithoutCheck(T* ptr, T* ptrEnd, long count, IComparer<T> comparer)
        {
            SortCore(ptr, ptrEnd, count, comparer);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SortInternal(T* ptr, T* ptrEnd, IComparer<T> comparer)
        {
            long count = ptrEnd - ptr;
            if (count < 2L || SortUtils.OptimizeSort(ptr, ptrEnd, count, comparer))
                return;
            SortCore(ptr, ptrEnd, count, comparer);
        }

        [Inline(InlineBehavior.Remove)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SortCore(T* ptr, T* ptrEnd, long count, IComparer<T> comparer)
        {
            T* pivot = ptr + (count >> 1);
            SortInternal(ptr, pivot, comparer);
            SortInternal(pivot, ptrEnd, comparer);
            Merge(ptr, pivot, ptrEnd, comparer);
        }

        [Inline(InlineBehavior.Remove)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Merge(T* ptr, T* pivot, T* ptrEnd, IComparer<T> comparer)
        {
            T left = *(pivot - 1);
            T right = *pivot;
            if (comparer.Compare(left, right) < 0)
                return;
            long count = ptrEnd - ptr;
            if (comparer.Compare(*ptr, *(ptrEnd - 1)) > 0)
            {
                long countRight = ptrEnd - pivot;
                long countLeft = count - countRight;
                if (countLeft == countRight) //均等情況
                {
                    for (T* leftIterator = ptr, rightIterator = pivot; leftIterator < pivot; leftIterator++, rightIterator++)
                    {
                        (*leftIterator, *rightIterator) = (*rightIterator, *leftIterator);
                    }
                }
                else //不均等情況
                {
                    for (T* leftIterator = pivot - 1, rightIterator = ptrEnd - 1; leftIterator >= ptr; leftIterator--, rightIterator--)
                    {
                        T temp = *rightIterator;
                        *rightIterator = *leftIterator;
                        *(leftIterator + 1) = temp;
                    }
                    *ptr = right;
                }
                return;
            }
            do
            {
                bool isOdd = (count & 0b01) == 0b01;
                count >>= 1;
                if (isOdd)
                    count++;
                ShellSortImplUnmanaged<T>.SortOnce(ptr, ptrEnd, comparer, count);
            } while (count > 1);
        }
    }
}

