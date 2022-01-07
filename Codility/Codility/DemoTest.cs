using System;
using System.Linq;

class Solution{

public int solution(int[] A)
{

int flag = 1;

Array.Sort(A);

//A = A.OrderBy(x => x).ToArray();

for (int i = 0 i < A.Lenght; i++)
{
if (A[i] <= 0)
  continue;
  else if (A[i] == flag)
  {
  flag++;
  }
  
  return flag;
}
}
}
