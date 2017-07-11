using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Item : IComparable<Item>, ICloneable
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
            set
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
            set
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
            set
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
            set
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
            set
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
            set
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
            this.location = new Coordinate(bInfo, aInfo);
            this.CInfo = cInfo;
            this.order = dInfo;
            this.picked = false;
        }

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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

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
