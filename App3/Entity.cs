using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ColonySym
{
    class Entity
    {
        private Props props;
        public int posX, posY, entCnt, maxAge, orgMaxAge, extraSize, generation;
        public int entRep=0, visibleIndicator;
        public double dPosX, dPosY, curAngle, angChange;
        public bool hungry, xFlipped, yFlipped, dLine;
        public Point target;
        private int maxX,
                    maxY,
                    cntMin,
                    cntMax,
                    ageMin,
                    ageMax,
                    dist,
                    locID;
        Random randGen;
        public int hasH;

        private void Init()
        {
            randGen = new Random();
            props = new ColonySym.Props();
            extraSize = 0;
            entRep = 0;
            visibleIndicator = 0;
            hungry = false;
            xFlipped = false;
            yFlipped = false;
            dLine = false;
            target = new Point(0, 0);
            maxX = props.GetInt("WIDTH");
            maxY = props.GetInt("HEIGHT");
            cntMin = props.GetInt("ENT_CNT_MIN");
            cntMax = props.GetInt("ENT_CNT_MAX");
            ageMin = props.GetInt("ENT_AGE_MIN");
            ageMax = props.GetInt("ENT_AGE_MAX");
            dist = props.GetInt("HUNGRY_DIST");
            locID = -1;
            hasH = this.GetHashCode();
        }

        // Constructors
        public Entity()
        {
            Init();
            posX = randGen.Next(1, maxX);
            posY = randGen.Next(1, maxY);
            curAngle = 180 / Math.PI;
            dPosX = posX;
            dPosY = posY;
            entCnt = randGen.Next(cntMin, cntMax);
            maxAge = randGen.Next(ageMin, ageMax);
            orgMaxAge = maxAge;
            target.X = posX;
            target.Y = posY;
        }
        public Entity(int x, int y)
        {
            Init();
            posX = x;
            posY = y;
            curAngle = 180 / Math.PI;
            dPosX = x;
            dPosY = y;
            entCnt = randGen.Next(cntMin, cntMax);
            maxAge = randGen.Next(ageMin, ageMax);
            orgMaxAge = maxAge;
            target.X = posX;
            target.Y = posY;
        }
        public Entity(int x, int y, double db)
        {
            Init();
            posX = x;
            posY = y;
            curAngle = (180 / Math.PI) * db ;
            dPosX = x;
            dPosY = y;
            entCnt = randGen.Next(cntMin, cntMax);
            maxAge = randGen.Next(ageMin, ageMax);
            orgMaxAge = maxAge;
            target.X = posX;
            target.Y = posY;
        }
        public Entity(int x, int y, double curAng, int lif, int mAge)
        {
            Init();
            posX = x;
            posY = y;
            curAngle = curAng;
            dPosX = posX;
            dPosY = posY;
            entCnt = lif;
            maxAge = mAge;
            orgMaxAge = maxAge;
            target.X = posX;
            target.Y = posY;
        }

        public void OverrideRadius(int x, int y)
        {
            maxX = x;
            maxY = y;
        }
        // Methods
        private void EntCntDown()
        {
            entCnt--;
            if (entCnt < props.GetInt("HUNGRY_MIN"))
            {
                hungry = true;
            }
            if (entCnt >= props.GetInt("HUNGRY_MAX"))
            {
                hungry = false;
                dLine = false;
            }
            maxAge--;
            entRep++;
        }
        
        // Recalculate next position
        private int[] EntRecChrgPos(List<Charge> chrgs, double mathSin, List<LinkedL> ents, int mm)
        {

            //curAngle = Math.Abs(curAngle) % 6.28;
            curAngle = curAngle % 6.28;
            List<Charge> temps = chrgs;
            List<LinkedL> tmpEnts = ents;
            Point selected = new Point();
            int result= -1;
            double step = 1;
            int fType = -1;
            dist = props.GetInt("HUNGRY_DIST");
            locID = -1;
            visibleIndicator = 0;
            double rads = 0;
            // If hungry locate closest charge
            if (hungry)
            {                
                step = 1.5;
                for (int i = 0; i < chrgs.Count; i++)
                {
                    int tmpdist = (int)DistanceTo(temps.ElementAt(i).posX, temps.ElementAt(i).posY);
                    if (tmpdist < (dist-1))
                    {
                        dist = tmpdist;
                        visibleIndicator = (props.GetInt("HUNGRY_DIST") - dist) / 2;
                        locID = i;
                        fType = 1;
                    }
                }
                if (locID == -1 && (entCnt < 100 || extraSize >= (mm*0.7)) && generation > 3)
                {
                    for (int i = 0; i < ents.Count; i++)
                    {
                        int tmpdist = (int)DistanceTo(tmpEnts.ElementAt(i).PARENT.posX, tmpEnts.ElementAt(i).PARENT.posY);
                        if (tmpdist < (dist - 1) && tmpEnts.ElementAt(i).PARENT.hasH!=this.hasH)
                        {
                            dist = tmpdist;
                            visibleIndicator = (props.GetInt("HUNGRY_DIST") - dist) / 2;
                            locID = i;
                            fType = 2;
                        }
                    }
                }

            }
            // Food not found close enough
            if (locID == -1)
            {               
                angChange = props.GetDouble("ANGLE_CHANGE") * randGen.Next(-1, 2);
                curAngle = curAngle + angChange;
                dLine = false;
            }
            // Food found
            else
            {
                angChange = props.GetDouble("ANGLE_CHANGE");
                dLine = true;
                if (fType == 1)
                {
                    selected = chrgs.ElementAt(locID).PositionP();
                } else
                {
                    selected = ents.ElementAt(locID).PositionP();
                }
                
                target.X = selected.X;
                target.Y = selected.Y;
                //atan2 for angle
                var radians = Math.Atan2((selected.Y - posY), (selected.X - posX));
                radians = radians % 6.28;

                // Check which angle is smaller (current or destination to food)
                var smaller = radians > curAngle ? curAngle : radians;
                var bigger = radians > curAngle ? radians : curAngle;
                var difference = bigger - smaller;

                // Turn straight to food if small angle change required
                if (difference < (angChange + 0.1) || dist <= 2)
                {
                    curAngle = radians;
                } else
                {
                    // Decide which way to turn
                    if (difference < 3.14)
                    {
                        curAngle = curAngle == smaller ? curAngle + angChange : curAngle - angChange;
                    }
                    else
                    {
                        curAngle = curAngle == smaller ? curAngle - angChange : curAngle + angChange;
                    }
                }
                rads = radians;
            }
            
            // Bounce from walls
            if (dPosX < 0 && !xFlipped)
            {
                curAngle = curAngle<=3.14 ? 3.14 - curAngle : 6.28-(curAngle-3.14);
                xFlipped = true;
                dPosX = dPosX + step * Math.Cos(curAngle);
                posX = (int)dPosX;
            }
            else
            {
                if (dPosX > maxX && !xFlipped)
                {
                    curAngle = curAngle <= 3.14 ? 3.14 - curAngle : 6.28 - (curAngle - 3.14);
                    xFlipped = true;
                    dPosX = dPosX + step * Math.Cos(curAngle);
                    posX = (int)dPosX;
                }
                else
                {
                    dPosX = dPosX + step * Math.Cos(curAngle);
                    posX = (int)dPosX;
                }
                if (dPosX < maxX && dPosX > 0)
                {
                    xFlipped = false;
                }
            }
            if (dPosY < 0 && !yFlipped)
            {
                curAngle = 6.28 - curAngle;
                yFlipped = true;
                dPosY = dPosY + step * Math.Sin(curAngle);
                posY = (int)dPosY;
            }
            else
            {
                if (dPosY > maxY && !yFlipped)
                {
                    curAngle = 6.28 - curAngle;
                    yFlipped = true;
                    dPosY = dPosY + step * Math.Sin(curAngle);
                    posY = (int)dPosY;
                }
                else
                {                    
                    dPosY = dPosY + step * Math.Sin(curAngle);
                    posY = (int)dPosY;
                }
                if (dPosY < maxY && dPosY > 0)
                {
                    yFlipped = false;
                }
            }
            int fight = -1;
            // If close to food pick it up
            if (dist < 2)
            {
                result = locID;
                if (fType == 1)
                {
                    entCnt += chrgs.ElementAt(locID).chrgVal;
                }
                if (fType == 2)
                {
                    int tmpVal = ents.ElementAt(result).PARENT.entCnt;
                    if (this.extraSize<ents.ElementAt(locID).PARENT.extraSize)
                    {
                        entCnt += tmpVal / 2;
                        maxAge += tmpVal / 2;
                        fight = 1;
                    } else
                    {
                        entCnt += tmpVal;
                        maxAge += tmpVal;
                    }                    
                    //Debug.WriteLine("Ent " + this.hasH + " just ate singleEnt " + ents.ElementAt(locID).PARENT.hasH);
                    extraSize++;
                }
            }
            int[] res = new int[3];
            res[0] = result;
            res[1] = fType;
            res[2] = fight;
            return res;
        }
        
        // Print stats
        public string EntStat()
        {
            string sposX = "     " + posX;
            string sposY = "     " + posY;
            string sentCnt = entCnt + "          ";
            string sAng = curAngle.ToString()+"     ";
            //string sang = "     " + sAng.Substring(0, sAng.IndexOf('.')+3);
            string sang = "     " + sAng.Substring(0, 5);
            return ("AGE : X : Y : Ang ( " + sentCnt.Substring(0, 7) + " : " + sposX.Substring(sposX.Length-4, 4) + " : " + sposY.Substring(sposY.Length - 4, 4)+" : "+sang.Substring(sang.Length-5, 5)+" )");
        }

        public double DistanceTo(int x, int y)
        {
            var deltaX = Math.Pow((x - posX), 2);
            var deltaY = Math.Pow((y - posY), 2);
            //pythagoras theorem for distance
            var distance = Math.Sqrt(deltaY + deltaX);
            return distance;
        }
        public int[] PerformSingleIteration(List<Charge> chrgs, long elapsed, List<LinkedL> ents, int mm) 
        {
            double mathSin = Math.Sin(elapsed);
            int[] result = EntRecChrgPos(chrgs, mathSin, ents, mm);          
            EntCntDown();
            //EntStat();
            return result;
        }

        public void PropsReload()
        {
            props.Reload();
            maxX = props.GetInt("WIDTH");
            maxY = props.GetInt("HEIGHT");
            cntMin = props.GetInt("ENT_CNT_MIN");
            cntMax = props.GetInt("ENT_CNT_MAX");
            ageMin = props.GetInt("ENT_AGE_MIN");
            ageMax = props.GetInt("ENT_AGE_MAX");
            dist = props.GetInt("HUNGRY_DIST");
        }
        
    }
}
