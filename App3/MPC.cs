using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SymServer;
using System.Threading.Tasks;

namespace ColonySym
{
    class MPC
    {
        Random randGen;
        Entity singleEnt;
        Props props;
        public List<LinkedL> ents;
        List<Charge> chrgs;
        List<int> list, listDead;
        public Charge singleChrg;
        private int WIDTH, HEIGHT, counter, treshold, reproduction;
        public int meatMax, cntMeatLow, cntMeatHigh;
        public int chrg_mod;
        private SQLLogger logger;
        public bool locked;
        private bool log;
        private Stopwatch watch;
        public int hungry;
        public int fed;
        private SServer server;
        string message = "";
        //public KdTree<float, Point> theTree = new KdTree<int, Point>(2, new IFloatMath());

#pragma warning disable CS0168 // Variable is declared but never used
        // Initialize
        private void Init()
        {
            server = new SServer();
            Task.Factory.StartNew(() =>
            {
                server.StartServer();
            });            
            randGen = new Random();
            singleEnt = new Entity();
            props = new Props();
            ents = new List<LinkedL>();
            chrgs = new List<Charge>();
            list = new List<int>();
            listDead = new List<int>();
            WIDTH = props.GetInt("WIDTH");
            HEIGHT = props.GetInt("HEIGHT");
            counter = props.GetInt("counter");
            meatMax = props.GetInt("meatMax");
            cntMeatLow = props.GetInt("cntMeatLow");
            cntMeatHigh = props.GetInt("cntMeatHigh");
            chrg_mod = props.GetInt("CHRG_RESPAWN_MOD");
            treshold = props.GetInt("ENT_REP_TRESHOLD");
            reproduction = props.GetInt("ENT_REP_AGE");
            log = props.GetInt("SQLLOG") == 1 ? true : false;
            locked = false;
            watch = new Stopwatch();
            hungry = props.GetInt("hungry");
            fed = props.GetInt("fed");
        }
        public MPC(int sizeX, int sizeY)
        {
            Init();
            WIDTH = sizeX;
            HEIGHT = sizeY;
            logger = new SQLLogger();
            watch.Start();
            singleChrg = new Charge(WIDTH, HEIGHT, watch.ElapsedMilliseconds);
            int chrgWidth = WIDTH/ props.GetInt("CHRG_INIT");
            int chrgHei = HEIGHT / props.GetInt("CHRG_INIT");
            for (int i = 0; i < props.GetInt("ENT_INIT"); i++)
            {
               //double angle = (randGen.Next(0,100)/100)*6.28;
               ents.Add(new LinkedL(randGen.Next(0, WIDTH), randGen.Next(0, HEIGHT), randGen.NextDouble()*i, randGen.Next(props.GetInt("ENT_CNT_MIN"), props.GetInt("ENT_CNT_MAX")), randGen.Next(props.GetInt("ENT_AGE_MIN"), props.GetInt("ENT_AGE_MAX"))));
           }
            for (int i = 0; i < props.GetInt("CHRG_INIT"); i++)
            {
                chrgs.Add(new Charge(randGen.Next(chrgWidth * i, WIDTH), randGen.Next(chrgHei * i, HEIGHT), watch.ElapsedMilliseconds));
            }            
        }

        public void SingleAction()
        {
            message = "";
            counter++;
            hungry = 0;
            fed = 0;
            cntMeatHigh = 0;
            meatMax = 0;
            // Iterate through all entities performing single action on each (heart beat)
            // Console.Write(ents.Count + " ");
            for (int i = 0; i < ents.Count; i++)
            {
                Entity iterEnt = ents.ElementAt(i).PARENT;
                if (iterEnt.hungry) { hungry++; } else { fed++; }
                // Performs single entity operations
                // Returns closest Charge if available
                int[] result = iterEnt.PerformSingleIteration(chrgs, watch.ElapsedMilliseconds, ents, meatMax);
                // Delete Charge if taken by enity
                if (result[0] > -1)
                {
                    if (result[1]==1)
                    {
                        chrgs.RemoveAt(result[0]);
                    }
                    if (result[1] == 2 && result[2]==-1)
                    {
                        listDead.Add(result[0]);
                    }
                    if (result[1] == 2 && result[2] == 1)
                    {
                       ents.ElementAt(result[0]).PARENT.entCnt = ents.ElementAt(result[0]).PARENT.entCnt - props.GetInt("ENT_EAT_WEAK");
                    }
                    
                    
                }
                // SERVER message prep
                message += iterEnt.hasH + " " + iterEnt.posX + " " + iterEnt.posY + " " + iterEnt.curAngle + ";";

                // Dead conditions met
                if (iterEnt.entCnt < 0 || iterEnt.maxAge <=0)
                {
                    listDead.Add(i);
                }
                // Reproduction conditions met
                if (iterEnt.entRep > treshold && iterEnt.entCnt > reproduction)
                {
                    list.Add(i);
                    ents.ElementAt(i).PARENT.entRep = randGen.Next(0, 50);
                }
                meatMax = ents.ElementAt(i).PARENT.extraSize > meatMax ? ents.ElementAt(i).PARENT.extraSize : meatMax;
                cntMeatHigh = ents.ElementAt(i).PARENT.extraSize > (meatMax * 0.7) ? cntMeatHigh + 1 : cntMeatHigh;

            }
            // Create new entities (reproduction list)
            for (int x = list.Count - 1; x >= 0; x--)
            {
                double rdb = randGen.NextDouble() * 5;
                Entity tmp = ents.ElementAt(list.ElementAt(x)).PARENT;
                int mAge = (ents.ElementAt(list.ElementAt(x)).Depth() * props.GetInt("ENT_GEN_MULTIPLIER")) + ents.ElementAt(list.ElementAt(x)).PARENT.orgMaxAge;
                ents.Add(new LinkedL(tmp.posX, tmp.posY, rdb, ents.ElementAt(list.ElementAt(x))));
                list.RemoveAt(x);
            }
            // Remove dead entities (dead list)
            for (int x = listDead.Count - 1; x >= 0; x--)
            {
                if (listDead.ElementAt(x)<=(ents.Count-1))
                {
                    //int tmpx = ents.ElementAt(listDead.ElementAt(x)).PARENT.posX;
                    //int tmpy = ents.ElementAt(listDead.ElementAt(x)).PARENT.posY;
                    //Charge singleChrg = new Charge(tmpx, tmpy, (int)(ents.ElementAt(listDead.ElementAt(x)).PARENT.maxAge * 0.01));
                    //singleChrg.type = 1;
                    //chrgs.Add(singleChrg);
                    ents.RemoveAt(listDead.ElementAt(x));
                    listDead.RemoveAt(x);
                } //else
                //{
                //    Debug.WriteLine("Cannot remove dead singleEnt - already removed");
                //}
            }
            // Re-create instances if all deleted
            if (ents.Count==0)
            {
                for (int i = 0; i < 10; i++)
                {
                    //Console.WriteLine("Ooops... all died!");
                    ents.Add(new LinkedL(randGen.Next(0, WIDTH), randGen.Next(0, HEIGHT), 
                                    randGen.NextDouble(), randGen.Next(props.GetInt("ENT_CNT_MIN"), props.GetInt("ENT_CNT_MAX")),
                                    randGen.Next(props.GetInt("ENT_AGE_MIN"), props.GetInt("ENT_AGE_MAX"))));
                }
            }
            // SERVER message prep
            message += "#";

            // Counter down single charge item
            for (int c = 0; c < chrgs.Count; c++)
            {
                // SERVER message prep
                message += chrgs.ElementAt(c).posX + "," + chrgs.ElementAt(c).posY + ";";

                chrgs.ElementAt(c).ChrgCntDown();
            }            
            // Adds new single charge every (n)th iteration
            if (counter%100==chrg_mod)
            {
                int startWid = randGen.Next(0, WIDTH);
                int startHei = randGen.Next(0, HEIGHT);
                chrgs.Add(new Charge(startWid, startHei, watch.ElapsedMilliseconds));
                //chrgs.Add(new Charge(randGen.Next(startWid, WIDTH), randGen.Next(startHei, HEIGHT), watch.ElapsedMilliseconds));
                counter = 0;
            }

            //
            // SERVER below
            //
            //Console.WriteLine("SERVER : ready " + server.ready.ToString() + " , dataRequested " + server.dataRequested.ToString() + " , commandMode " + server.commandMode.ToString());
            if (server.dataRequested)
            {
                //foreach (LinkedL ent in ents)
                //{
                //    message += ent.PARENT.hasH + " " + ent.PARENT.posX + " " + ent.PARENT.posY +" "+ ent.PARENT.curAngle + ";";
                //}
                //message += "#";
                //foreach (Charge chrg in chrgs)
                //{
                //    message += chrg.posX + "," + chrg.posY + ";";
                //}
                server.SendString(message.Length+"");
                //Console.WriteLine(message);
            }
            if (server.ready&& !server.dataRequested)
            {                
                server.SendString(message);
            }
            if (server.commandMode)
            {
                Console.WriteLine("[colony] i\tserver.commandMode : true : " + server.commandString);
                string[] comms = server.commandString.Split(' ');
                List<string> tmpList = comms.ToList();
                tmpList.RemoveAt(0);
                ParseInput(tmpList.ToArray());
                server.commandMode = false;
            }
        }

        public void AddChargesOnClick(int x, int y)
        {

            for (int i = 0; i < 5; i++)
            {
                int randx = randGen.Next(-10, 10);
                int randy = randGen.Next(-10, 10);
                chrgs.Add(new Charge(x+randx, y+randy, watch.ElapsedMilliseconds));
            }
        }

        private void OverrideRadius(int val)
        {
            foreach(LinkedL ent in ents)
            {
                ent.PARENT.OverrideRadius(val, val);
            }
        }

        public int EntsCount()
        {
            return ents.Count;
        }

        public int ChrgsCount()
        {
            return chrgs.Count;
        }

        public Entity EntityAt(int index)
        {
            return ents.ElementAt(index).PARENT;
        }

        public int EntitySizeAt(int index)
        {
            return ents.ElementAt(index).PARENT.extraSize;
        }

        public int VisIndicatorAt(int index)
        {
            return ents.ElementAt(index).PARENT.visibleIndicator;
        }

        //public LinkedL LinkAt(int index)
        //{
        //    return ents.ElementAt(index);
        //}

        public Point EntityPositionAt(int index)
        {
            Point tmp = new Point();
            try
            {
                Entity entTmp = ents.ElementAt(index).PARENT;
                tmp.X = entTmp.posX;
                tmp.Y = entTmp.posY;
            } catch (Exception e)
            {
                if (!logger.busy && log) { logger.LogException(e, Form1.runHashCode, "Entity Position At failed (MPC)"); }
            }
            return tmp;
        }

        public Point ChrgPositionAt(int index)
        {
            Charge chrTmp = chrgs.ElementAt(index);
            Point tmp = new Point();
            tmp.X = chrTmp.posX;
            tmp.Y = chrTmp.posY;
            return tmp;
        }

        public int ChrgTypeAt(int index)
        {
            Charge chrTmp = chrgs.ElementAt(index);
            int tmp = chrTmp.type;
            return tmp;
        }

        public double AngleAt(int index)
        {
            double result = ents.ElementAt(index).PARENT.curAngle;
            return result;
        }

        public void ParseInput(string[] args)
        {
            //if (!logger.busy && log)
            //{ logger.LogMessage(string.Join(" ", args), "Parse Input", Form1.runHashCode); }
            Console.WriteLine("[colony] i\tParseInput : " + string.Join(" ",args));
            if (args.Length >=1)
            {
                switch (args[0])
                {
                    case "stats":
                        Console.Clear();
                        Console.SetCursorPosition(1, 1);
                        Console.Write("1");
                        break;
                    case "set":
                        if (args.Length ==3)
                        {
                            SetParam(args[1], args[2]);
                        }                        
                        break;
                    case "show":
                        if (args.Length>=2)
                        {
                            Show(args[1], args.Length==3?args[2]:null);
                        }
                        break;
                    case "add":
                        if (args.Length == 3)
                        {
                            ParseAdd(args[1], args[2]);
                        }
                        break;
                    case "kill":
                        if (args.Length==2)
                        {
                            try
                            {
                                if (args[1].Equals ("all"))
                                {
                                    ents.Clear();
                                } else
                                if (args[1].Equals("last"))
                                {
                                    ents.RemoveAt(ents.Count-1);
                                }
                                else
                                if (args[1].Equals("first"))
                                {
                                    ents.RemoveAt(0);
                                }
                                else
                                {
                                    bool kill = KillByIndex(Convert.ToInt32(args[1]));
                                    if (!kill)
                                    {
                                        KillByID(Convert.ToInt32(args[1]));
                                    }
                                }
                            } catch (Exception e)
                            {
                                string manualComment = "[colony] !\tError while converting ID";
                                Console.WriteLine(manualComment);
                                if (!logger.busy && log)
                                { logger.LogException(e, Form1.runHashCode, manualComment); }
                            }
                        }
                        break;
                    case "exit":
                        if (!logger.busy && log)
                        { logger.LogMessage("Bye", args[0], Form1.runHashCode); }
                        Thread.Sleep(500);
                        Console.Clear();
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private LinkedL FindByID(int id)
        {
            LinkedL index = null;
            foreach (LinkedL ent in ents)
            {
                if (ent.linkID == id)
                {
                    index = ent;
                }
            }
            return index;
        }

        private LinkedL FindByIndex(int index)
        {
            LinkedL tmp = null;
            tmp = ents.ElementAt(index);
            return tmp;
        }

        private bool KillByID(int id)
        {
            bool result = false;
            LinkedL index = FindByID(id);
            result = ents.Contains(index) ? true : false;
            ents.Remove(index);
            string message = index.linkID + " is no more. It had " + (index.Depth() - 1) + " children";
            Console.WriteLine(message);
            if (!logger.busy && log)
            { logger.LogMessage(message, "kill " + id, Form1.runHashCode); }
            return result;
        }

        private bool KillByIndex(int index)
        {
            bool result = false;
            try
            {
                LinkedL ent = ents.ElementAt(index);
                result = ents.Contains(ent) ? true : false;
                ents.Remove(ent);
                string message = ent.linkID + " is no more. It had " + (ent.Depth() - 1) + " children";
                Console.WriteLine(message);
                if (!logger.busy && log)
                { logger.LogMessage(message, "kill " + index, Form1.runHashCode); }
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        private void ParseAdd(string param, string value)
        {
            Console.WriteLine("[colony] i\tParseAdd : param " + param+" : value "+value);
            string message = "";
            if (Regex.IsMatch(value, "[-+]?[0-9]*\\.?[0-9]+"))
            {
                switch (param)
                {
                    case "ent":
                        Console.Write("[colony] i\tents before : " + ents.Count);
                        for (int i = 0; i < int.Parse(value); i++) {
                            ents.Add(new LinkedL(randGen.Next(1, 300), randGen.Next(1, 300), randGen.NextDouble ()));
                        }
                        Console.WriteLine(" after : " + ents.Count);
                        break;
                    case "food":
                        for (int i = 0; i < int.Parse(value); i++)
                        {
                            chrgs.Add(new Charge(randGen.Next(WIDTH), randGen.Next(HEIGHT), 18));
                        }
                        break;
                }
                //if (!logger.busy && log)
                //{ logger.LogMessage(message, "set " + param + " " + value, Form1.runHashCode); }
            }
            else
            {
                message = "[colony] !\tInvalid VALUE param";
                Console.WriteLine(message);
                //if (!logger.busy && log)
                //{ logger.LogMessage(message, param + " " + value, Form1.runHashCode); }
            }
        }


        private void SetParam(string param, string value)
        {
            string message = "";
            Console.WriteLine("[colony] i\tSetParam : param " + param + " : value " + value);
            if (Regex.IsMatch(value, "[-+]?[0-9]*\\.?[0-9]+"))
            {
                switch (param)
                {
                    case "mod":
                        message = chrg_mod + " -> " + value;
                        //Console.WriteLine(message);
                        chrg_mod = Convert.ToInt32(value);
                        break;
                    case "width":
                        message = WIDTH + " -> " + value;
                        Console.WriteLine(message);
                        WIDTH = Convert.ToInt32(value);
                        break;
                    case "height":
                        message = HEIGHT + " -> " + value;
                        Console.WriteLine(message);
                        HEIGHT = Convert.ToInt32(value);
                        break;
                    case "radius":
                        message = props.GetInt("MAX_RADIUS") + "-> " + value;
                        Console.WriteLine(message);
                        OverrideRadius(Convert.ToInt32(value));
                        break;
                }
                //if (!logger.busy && log)
                //{ logger.LogMessage(message, "set " + param + " " + value, Form1.runHashCode); }
            } else
            {
                message = "[colony] !\tInvalid VALUE param";
                Console.WriteLine(message);
                //if (!logger.busy && log)
                //{ logger.LogMessage(message, param + " " + value, Form1.runHashCode); }
            }
        }

        private void Show(string input, string value)
        {
            string message = "";
            if (Regex.IsMatch(input, "^\\d+$"))
            {
                int max = ents.Count-1;
                int val = Convert.ToInt32(input);
                val = val > max ? max : val;
                LinkedL tmp = FindByID(val);
                if (tmp == null)
                {
                    tmp = FindByIndex(val);
                }
                if (tmp != null)
                {
                    message = tmp.PARENT.EntStat() + " " + string.Join(" : ", tmp.PreviousList());
                    Console.WriteLine(message);
                    Console.WriteLine("maxAge: " + tmp.PARENT.maxAge);
                    Console.WriteLine("entCnt: " + tmp.PARENT.entCnt);
                    Console.WriteLine("entRep: " + tmp.PARENT.entRep);
                    Console.WriteLine("hungry: " + tmp.PARENT.hungry.ToString());
                }
            } else
            {
                switch (input)
                {
                    case "count":
                        Console.SetCursorPosition(20, Console.CursorTop - 1);
                        message = ents.Count + "";
                        Console.WriteLine(message);
                        break;
                    case "food":
                        Console.SetCursorPosition(20, Console.CursorTop - 1);
                        message = chrgs.Count + "";
                        Console.WriteLine(message);
                        break;
                    case "size":
                        Console.SetCursorPosition(20, Console.CursorTop - 1);
                        message = WIDTH + "x" + HEIGHT;
                        Console.WriteLine(message);
                        break;
                    case "ents":
                        Console.SetCursorPosition(2, Console.CursorTop);
                        Console.Write("Index");
                        Console.SetCursorPosition(10, Console.CursorTop);
                        Console.WriteLine("ID");
                        List<LinkedL> temps = new List<LinkedL>();
                        temps = ents.Take(ents.Count).ToList();
                        foreach (LinkedL ent in temps)
                        {
                            Console.SetCursorPosition(2, Console.CursorTop);
                            Console.Write(ents.IndexOf(ent));
                            Console.SetCursorPosition(10, Console.CursorTop);
                            Console.WriteLine(ent.linkID);
                        }
                        temps.Clear();
                        break;
                }
            }
            if (!logger.busy && log)
            { logger.LogMessage(message, "show " + input + " " + value, Form1.runHashCode); }

        }

        public LinkedL Youngest()
        {
            LinkedL[] result = new LinkedL[ents.Count];
            ents.CopyTo(result);
            LinkedL tmp = result.FirstOrDefault();
            foreach (LinkedL ent in result)
            {
                if (ent.Depth() < tmp.Depth ())
                {
                    tmp = ent;
                }
            }
            return tmp;
        }

        public LinkedL Oldest()
        {
            LinkedL[] result = new LinkedL[ents.Count];
            ents.CopyTo(result);
            LinkedL tmp = result.FirstOrDefault();
            foreach (LinkedL ent in result)
            {
                if (ent.Depth() > tmp.Depth())
                {
                    tmp = ent;
                }
            }
            return tmp;
        }

        //public void ConsoleInteract()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        while (true)
        //        {
        //            Console.Write("> ");
        //            string input = Console.ReadLine();
        //            string[] arguments = input.Split(' ');
        //            ParseInput(arguments);
        //        }
        //    });
        //}

        public void PropsReload()
        {
            props.Reload();

            WIDTH = props.GetInt("WIDTH");
            HEIGHT = props.GetInt("HEIGHT");
            chrg_mod = props.GetInt("CHRG_RESPAWN_MOD");
            treshold = props.GetInt("ENT_REP_TRESHOLD");
            reproduction = props.GetInt("ENT_REP_AGE");
            log = props.GetInt("SQLLOG") == 1 ? true : false;

            foreach (LinkedL lEnt in ents)
            {
                lEnt.PARENT.PropsReload();
            }
        }

        ///*
        public static void Main(string[] args)
        {
            MPC main = new ColonySym.MPC(300, 300);
            //Task.Factory.StartNew(() =>
            //{
                while (true)
                {
                    main.SingleAction();
                    Thread.Sleep(30);
                }
            //});

            //while (!gotInput)
            //{
            //    Console.Write("> ");
            //    string input = Console.ReadLine();
            //    string[] arguments = input.Split(' ');
            //    main.ParseInput(arguments);
            //}
        }
        //*/

    }
}
