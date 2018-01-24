using ColonySym;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonySym
{
    class LinkedL
    {
        public int linkID;
        public Entity PARENT;
        private LinkedL LINK;
        // private DNAString DNA;

        // Constructors
        public LinkedL()
        {
            PARENT = new Entity();
            Init();
        }
        public LinkedL(int x, int y, double db)
        {
            PARENT = new Entity(x, y, db);
            Init();
        }
        public LinkedL(int x, int y, double rdb, int entCnt, int mAge)
        {
            PARENT = new Entity(x, y, rdb, entCnt, mAge);
            Init();
        }
        public LinkedL(int x, int y, double rdb, LinkedL entity)
        {
            PARENT = new Entity(x, y, rdb);
            LINK = entity;
            PARENT.extraSize = (int)entity.PARENT.extraSize / 3;
            Init();
        }
        public LinkedL(LinkedL entity)
        {
            
            PARENT = new Entity();
            LINK = entity;
            Init();
        }

        private void Init()
        {
            linkID = this.GetHashCode();
            PARENT.generation = Depth();
        }
        // Methods
        public bool HasNext()
        {
            bool hasNext=false;
            if (LINK!=null)
            {
                hasNext = true;
            }
            return hasNext;
        }
        public LinkedL GetNext()
        {
            return LINK;
        }
        public void AddLink(LinkedL enitity)
        {
            LINK = enitity;
        }

        //
        // Summary:
        //     Gets the number of elements contained in the Current Linked List.
        //
        // Returns:
        //     The number of elements contained in the Current Linked List.
        public int Depth()
        {
            int counter = 1;
            LinkedL tmp = LINK;
            if (tmp!=null) {
                counter++;
                while (tmp.HasNext())
                {
                    tmp = tmp.GetNext();
                    counter++;
                }
            }
            return counter;
        }

        public bool OldEnough()
        {
            bool res = false;
            res = Depth() > 3 ? true : false;
            return res;
        }
        public Point PositionP()
        {
            Point result = new Point(PARENT.posX, PARENT.posY);
            return result;
        }
        //
        // Summary:
        //     Gets the list of elements contained in the Current Linked List.
        //
        // Returns:
        //     The list of elements contained in the Current Linked List.
        public string[] PreviousList()
        {
            string[] list = new string[this.Depth()];
            int counter = 0;
            LinkedL tmp = LINK;
            list[counter] = (this.linkID.ToString().PadLeft(8,' '));
            if (tmp != null)
            {
                counter++;
                list[counter] = (tmp.linkID.ToString().PadLeft(8, ' '));
                while (tmp.HasNext())
                {
                    counter++;
                    tmp = tmp.GetNext();
                    list[counter] = (tmp.linkID.ToString().PadLeft(8, ' '));
                }
            }
            return list;
        }
    }
}
