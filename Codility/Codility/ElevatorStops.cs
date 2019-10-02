using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codility
{
    class ElevatorStops
    {


        public static int checkElevatorStops(int[] A, int[] B, int M, int X, int Y)
        {

            int totalNumberStops = 0;
            long totalWeightPerTrip = 0;
            int maxPeople = 0;
            List<int> lstFloors = new List<int>();
            int currentPerson = 0;
            bool start = false;
            while (currentPerson < A.Length)
            {

                if ((totalWeightPerTrip + A[currentPerson]) <= Y && (maxPeople + 1) <= X)
                {
                    totalWeightPerTrip += A[currentPerson];
                    maxPeople++;
                    lstFloors.Add(B[currentPerson]);

                    if (currentPerson == A.Length - 1)
                        start = true;

                    currentPerson++;
                }
                else
                {
                    start = true;
                }

                if (start)
                {
                    totalNumberStops += lstFloors.Distinct().Count() + 1;

                    lstFloors.Clear();
                    maxPeople = 0;
                    totalWeightPerTrip = 0;
                    start = false;
                }
            }

            return totalNumberStops;
        }

    }
}
