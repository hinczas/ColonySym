using System;
using System.Drawing;

namespace ColonySym
{
    class Charge
    {
        public int chrgCnt, chrgVal;
        public int posX, posY;
        private Random randGen;
        private Props props;
        private int maxX, maxY;
        private int cntMin;
        private int cntMax;
        private int valMin;
        private int valMax;
        public int type;
        public Charge(int maxX, int maxY, long elapsed)
        {
            Init();
            this.maxX = maxX;
            this.maxY = maxY;
            posX = maxX;
            posY = maxY;
            double trig = Math.Sin(elapsed)+1;
            chrgCnt = (int) (randGen.Next(cntMin, cntMax)*trig);
            chrgVal = randGen.Next(valMin, valMax);

        }
        public Charge(int maxX, int maxY, int iniVal)
        {
            Init();
            this.maxX = maxX;
            this.maxY = maxY;
            posX = maxX;
            posY = maxY;
            chrgCnt = randGen.Next(cntMin, cntMax);
            chrgVal = randGen.Next(valMin, valMax)+iniVal;
        }

        private void Init()
        {
            props = new Props();
            randGen = new Random();
            maxX = props.GetInt("WIDTH"); 
            maxY = props.GetInt("HEIGHT");
            cntMin = props.GetInt("CHRG_CNT_MIN");
            cntMax = props.GetInt("CHRG_CNT_MAX");
            valMin = props.GetInt("CHRG_VAL_MIN");
            valMax = props.GetInt("CHRG_VAL_MAX");
            type = 0;
    }

        public void ChrgCntDown()
        {
            chrgCnt--;
            maxX = maxX <= 0 ? 1 : maxX;
            maxY = maxY <= 0 ? 1 : maxY;
            if (chrgCnt <= 0)
            {
                posX = randGen.Next(0, maxX);
                posY = randGen.Next(0, maxY);
                chrgCnt = randGen.Next(cntMin, cntMax);
            }
        }
        public void ChrgUsed()
        {
            posX = randGen.Next(0, maxX);
            posY = randGen.Next(0, maxY);
            chrgCnt = randGen.Next(cntMin, cntMax);
        }

        public void ChrgOverride(int x, int y)
        {
            posX = x;
            posY = y;
            chrgCnt = randGen.Next(cntMin, cntMax);
        }

        public Point PositionP()
        {
            Point result = new Point(posX, posY);
            return result;
        }

        public void PropsReload()
        {
            props.Reload();
        }
    }
}
