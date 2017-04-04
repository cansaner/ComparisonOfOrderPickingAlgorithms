using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TBLST;
using TBSRCH;
using ConsoleApplication1;

namespace TBLST
{
    public class TabuList
    {
        public int[,] tabuList;//!!!!!!!!! public olmayınca sorun yaratıyor!

        public TabuList(int numCities)
        {
            tabuList = new int[numCities, numCities]; //city 0 is not used here, but left for simplicity
        }//end of TabuList

        public void tabuMove(int city1, int city2)
        { //tabus the swap operation
            tabuList[city1, city2] += ConsoleApplication1.Program.tabuLength; //Why 5 but not 10? 5+5=10??? (1,2)=5 ve (2,1)=5 ve toplamda mı 10?
            tabuList[city2, city1] += ConsoleApplication1.Program.tabuLength; //Why 5?
        }//end of tabuMove

        public void decrementTabu()
        {
            for (int i = 0; i < tabuList.GetLength(1) ; i++)
            {
                for (int j = 0; j < tabuList.GetLength(0) ; j++)
                {
                    //tabuList[i, j] -= tabuList[i, j] <= 0 ? 0 : 1;
                    if (tabuList[i, j] <= 0)
                    tabuList[i, j] = (tabuList[i, j] - 0);
                    else //if(tabuList[i, j] > 0)
                        tabuList[i, j] = (tabuList[i, j] - 1);

                }
            }
        }//end of decrementTabu()

    }//end of class TabuList

}
