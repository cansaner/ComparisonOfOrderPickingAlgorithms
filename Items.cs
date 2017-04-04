using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITEMDEFINITION
{
    public class Item : IComparable<Item>
    {
        public int index;
        public int A_info;
        public int B_info;
        public int C_info;
        public int D_info;

        public bool picked_during_sshape = false;

        int IComparable<Item>.CompareTo(Item other) //compares coordinatesof items DESCENDING
        {
            if (other.B_info > this.B_info)
                return -1;
            else if (other.B_info == this.B_info)
                //return 0;
                {
                    if (other.C_info > this.C_info)
                        return -1;
                    else if (other.C_info == this.C_info)
                        //return 0;
                        {
                            if (other.A_info > this.A_info)
                                return -1;
                            else if (other.A_info == this.A_info)
                                //return 0;
                                {
                                    if (other.D_info > this.D_info)
                                        return -1;
                                    else if (other.D_info == this.D_info)
                                        return 0;
                                    else //if (other.index < this.index)
                                        return 1;
                                 }
                            else //if (other.index < this.index)
                                return 1;
                        }
                   else //if (other.index < this.index)
                        return 1;
                }
            else //if (other.B_info < this.B_info)
                return 1;
         }//end of Int IComparable

    }//end of class Item

}//end of namespace

//---
//if (other.B_info > this.B_info)
//                return -1;
//            else if (other.B_info == this.B_info)
//                //return 0;
//            {
//                if (other.index > this.index)
//                    return -1;
//                else if (other.index == this.index)
//                    return 0;
//                else //if (other.index < this.index)
//                    return 1;
//            }
//            else //if (other.B_info < this.B_info)
//                return 1;
