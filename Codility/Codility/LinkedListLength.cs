using System;
using System.Collections.Generic;
using System.Text;

namespace Codility
{
    class LinkedListLength
    {

        public static int CalculateLinkedListLength(int[] A)
        {
            LinkedList llist = new LinkedList();
            //   Array.Reverse(A);


            llist.push(A[0], A);
            return llist.getCount();
        }


         class LinkedList
        {
            Node head; // head of list 

            /* Inserts a new Node at front of the list. */
            public void push(int current, int[] array)
            {



                //Node new_node = new Node(array[current]);
                Node new_node = new Node();

                // var a= Array.IndexOf(array, current);

                new_node.next = getNext(current, array);

                /* 4. Move the head to point to new Node */
                head = new_node;
            }


            public Node getNext(int current, int[] array)
            {

                if (current < 0)
                {
                    return new Node
                    {
                        data = current,
                        next = null
                    };
                }
                else
                {
                    return new Node
                    {
                        data = current,
                        next = getNext(array[current], array)
                    };
                }
            }





            /* Returns count of nodes in linked list */
            public int getCount()
            {
                Node temp = head;
                int count = 0;
                while (temp != null)
                {
                    if (temp.data == -1)
                        return count;
                    count++;
                    temp = temp.next;
                }
                return count;
            }
        }


        public class Node
        {
            public int data;
            public Node next;


            public Node()
            {

            }
            public Node(int d)
            {
                data = d; next = null;
            }
        }


    }




    

}
