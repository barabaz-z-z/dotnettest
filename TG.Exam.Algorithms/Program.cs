using System;

namespace TG.Exam.Algorithms
{
    class Program
    {
        // this method includes simple functionality, so no necessity to use recursion
        // the same result can be achieved by straightforward loop
        static int Foo(int a, int b, int c)
        {
            for (var i = b; 1 < c; c--)
            {
                b += a;
                a = i;
                i = b;
            }

            return a;
        }

        static int[] Bar(int[] arr)
        {
            // or just use Array.Sort(arr) instead of inventing bycicle
            // this function is implementation of quick sorting with recursion
            // it is much faster than usual bubble sorting
            Sort(ref arr, 0, arr.Length - 1);

            return arr;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Foo result: {0}", Foo(7, 2, 8));
            Console.WriteLine("Bar result: {0}", string.Join(", ", Bar(new[] { 7, 2, 8 })));

            Console.ReadKey();
        }

        private static void Sort(ref int[] arr, int leftIndex, int rightIndex)
        {
            if (leftIndex >= rightIndex)
                return;

            var partitionIndex = CalcPartitionIndex(ref arr, leftIndex, rightIndex);

            Sort(ref arr, leftIndex, partitionIndex - 1);
            Sort(ref arr, partitionIndex + 1, rightIndex);
        }
        private static int CalcPartitionIndex(ref int[] arr, int leftIndex, int rightIndex)
        {
            var pivot = arr[rightIndex];
            var j = leftIndex;

            for (int i = leftIndex; i <= rightIndex - 1; i++)
            {
                if (arr[i] > pivot)
                    continue;

                SwapElements(ref arr, j, i);
                j++;
            }

            SwapElements(ref arr, j, rightIndex);

            return j;
        }
        private static void SwapElements(ref int[] arr, int leftIndex, int rightIndex)
        {
            if (arr[leftIndex] == arr[rightIndex])
                return;

            int temp = arr[leftIndex];
            arr[leftIndex] = arr[rightIndex];
            arr[rightIndex] = temp;
        }
    }
}
