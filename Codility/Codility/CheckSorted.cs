using System;
using System.Collections.Generic;
using System.Text;

namespace Codility
{
    class CheckSorted
    {


        public static bool checkSorted(int[] arr)
        {

            int n = arr.Length;
            return checkNumberOfMinimalSorts( n,  arr);
        }

        static bool checkNumberOfMinimalSorts(int n, int[] arr)
        {
            // Create a sorted copy of original array 
            int[] b = new int[n];
            for (int i = 0; i < n; i++)
                b[i] = arr[i];
            Array.Sort(b);

            // Check if 0 or 1 swap required to 
            // get the sorted array 
            int ct = 0;
            for (int i = 0; i < n; i++)
                if (arr[i] != b[i])
                    ct++;
            if (ct == 0 || ct == 2)
                return true;
            else
                return false;
        }
    }
}
