using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Item : IComparable<Item>
    {
        private int index;
        private Coordinate location;
        private Boolean side;
        private int order;
        private Boolean picked;

        public int Index
        {
            get
            {
                return index;
            }
            protected set
            {
                index = value;
            }
        }

        public int AInfo
        {
            get
            {
                return location.Y;
            }
            protected set
            {
                location.Y = value;
            }
        }

        public int BInfo
        {
            get
            {
                return location.X;
            }
            protected set
            {
                location.X = value;
            }
        }

        public int CInfo
        {
            get
            {
                return Convert.ToInt32(side);
            }
            protected set
            {
                side = Convert.ToBoolean(value);
            }
        }

        public int DInfo
        {
            get
            {
                return order;
            }
            protected set
            {
                order = value;
            }
        }

        public bool Picked
        {
            get
            {
                return picked;
            }
            protected set
            {
                picked = value;
            }
        }

        public Item()
        {
            this.index = 0;
            this.location = new Coordinate(1, 1);
            this.side = false;
            this.order = 1;
            this.picked = false;
        }

        public Item(int index, int aInfo, int bInfo, int cInfo, int dInfo)
        {
            this.index = index;
            this.location = new Coordinate(aInfo, bInfo);
            this.CInfo = cInfo;
            this.order = dInfo;
            this.picked = false;
        }

        //public int index;
        //public int a_info;
        //public int b_info;
        //public int c_info;
        //public int d_info;

        //public bool picked_during_sshape = false;

        int IComparable<Item>.CompareTo(Item other) //compares coordinatesof items DESCENDING
        {
            if (other.BInfo > this.BInfo)
                return -1;
            else if (other.BInfo == this.BInfo)
                //return 0;
                {
                    if (other.CInfo > this.CInfo)
                        return -1;
                    else if (other.CInfo == this.CInfo)
                        //return 0;
                        {
                            if (other.AInfo > this.AInfo)
                                return -1;
                            else if (other.AInfo == this.AInfo)
                                //return 0;
                                {
                                    if (other.DInfo > this.DInfo)
                                        return -1;
                                    else if (other.DInfo == this.DInfo)
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
