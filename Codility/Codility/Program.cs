using System;

namespace Codility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ElevatorStops.checkElevatorStops(new int[] { 60, 80, 40 }, new int[] { 2, 3, 5 }, 5, 2, 200);

            CheckSorted.checkSorted(new int[] { 60, 80, 40 });
            
            LinkedListLength.CalculateLinkedListLength(new int[] { 60, 80, 40 });
        }
    }
}
