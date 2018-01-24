using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data;
using ExtraUtils;
using System.Runtime.InteropServices;
using System.IO;
using ExtensionMethods;
using SymClient;
using System.Collections.Generic;

namespace ColonySym
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Random randGen;
        //private MPC main;
        private static SQLLogger logger;
        private static Stopwatch watch;
        private Props props;
        private SClient client;
        //private static int argb = -98031576;

        // privates

        private int milBreak, a, r, g, b, argbMin, argbMax, argb, argbChrg, maxX, maxY, opt_mult;
        private static bool visible, showEnts, readMode, ERROR, keyPressed, line, kolors;
        private SmoothingMode smoothingMode;
        private TextRenderingHint textSmoothing;
        private static Object locker;
        private static Color argbCol;
        private Pen aBrush3, aBrush4, aBrush5;
        private static string ERROR_MESSAGE;
        private static int ERROR_COUNT, refresh, optionSelector, taskID;
        private string[] optionSelected;
        private double frequency;
        private static bool pause, log, borders;
        private string actMessage = "";

        // publics
        public static ColourWriter printer;
        public static int runHashCode;

        // Init stage

        private void Init()
        {
            randGen     = new Random();
            locker      = new object();
            smoothingMode = SmoothingMode.None;
            textSmoothing = TextRenderingHint.SystemDefault;
            optionSelected = new string[10];
            pause = false;

            PropsLoad();

            ERROR_MESSAGE = "";
            argbCol     = Color.DarkGray;
            argb        = Color.FromArgb(30, 37, 37, 38).ToArgb();
            aBrush3     = new Pen(Color.FromArgb(a, r, g, b));
            aBrush4     = new Pen(Color.DarkBlue);
            aBrush5     = new Pen(Color.Gray);
        }

        private void PropsLoad()
        {
            visible = props.GetInt("visible") == 1 ? true : false;
            showEnts = props.GetInt("showEnts") == 1 ? true : false;
            readMode = props.GetInt("readMode") == 1 ? true : false;
            ERROR = props.GetInt("ERROR") == 1 ? true : false;
            keyPressed = props.GetInt("keyPressed") == 1 ? true : false;
            line = props.GetInt("line") == 1 ? true : false;
            kolors = props.GetInt("kolors") == 1 ? true : false;
            log = props.GetInt("SQLLOG") == 1 ? true : false;
            borders = props.GetInt("borders") == 1 ? true : false;

            maxX = props.GetInt("WIDTH");
            maxY = props.GetInt("HEIGHT");
            milBreak = props.GetInt("BREAK");
            opt_mult = props.GetInt("OPTION_MULTIPLIER");
            a = props.GetInt("a");
            r = props.GetInt("r");
            g = props.GetInt("g");
            b = props.GetInt("b");
            argbMin = props.GetInt("argbMin");
            argbMax = props.GetInt("argbMax");
            argbChrg = props.GetInt("argbChrg");
            ERROR_COUNT = props.GetInt("ERROR_COUNT");
            refresh = props.GetInt("refresh");
            taskID = props.GetInt("taskID");
            frequency = props.GetDouble("frequency");
        }

        public Form1()
        {
            props = new ColonySym.Props();
            Init();

            optionSelector = 0;
            optionSelected = "ChrgCol speed mod refresh singleEnt Argb aRgb arGb argB freq backgrcol 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30".Split(' ');
            runHashCode = DateTime.UtcNow.ToString().GetHashCode();
            printer = new ColourWriter();
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(DoEscape);
            //main = new MPC(props.GetInt("WIDTH"), props.GetInt("HEIGHT"));
            //Task.Factory.StartNew(() =>
            //    {
            //        ConsoleInteract();
            //   });
            InitializeComponent();
            trackBar1.Value = milBreak;
            textBox1.Text = milBreak + " ms";
            watch = System.Diagnostics.Stopwatch.StartNew();
            logger = new ColonySym.SQLLogger();
            if (log) { logger.LogMessage("Start of application", null, runHashCode); }
            this.MainMenuStrip.Hide();
            this.textBox1.Hide();
            this.trackBar1.Hide();
            client = new SClient();
            Task.Factory.StartNew(() =>
                {
                    client.Init();
                });
        }

        // Event listeners below 

        private void keyPress(object sender, KeyPressEventArgs e)
        {
            string message = "";
            string comment = "KeyPress: "+e.KeyChar.ToString();
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
            {
                int key = Convert.ToInt32(e.KeyChar.ToString());
                optionSelector = key + (10 * opt_mult);
                message = "option change :" + optionSelected[key];
            }
            if (e.KeyChar==43)
            {
                switch (optionSelector) {
                    case 1:
                        milBreak++;
                        message = "milBreak " + e.KeyChar.ToString() ;
                        break;
                    case 2:
                        //main.chrg_mod++;
                        message = "chrg_mod " + e.KeyChar.ToString();
                        break;
                    case 3:
                        refresh++;
                        message = "refresh " + e.KeyChar.ToString();
                        break;
                    case 4:
                        //main.ents.Add(new LinkedL());
                        message = "ents " + e.KeyChar.ToString();
                        break;
                    case 5:
                        a = a >= 255 ? 255 : a + 1;
                        message = "alpha " + e.KeyChar.ToString();
                        break;
                    case 6:
                        r = r >= 255 ? 255 : r + 1;
                        message = "red " + e.KeyChar.ToString();
                        break;
                    case 7:
                        g = g >= 255 ? 255 : g + 1;
                        message = "green " + e.KeyChar.ToString();
                        break;
                    case 8:
                        b = b >= 255 ? 255 : b + 1;
                        message = "blue " + e.KeyChar.ToString();
                        break;
                    case 9:
                        frequency = frequency + 0.0005;
                        message = "frequency " + e.KeyChar.ToString();
                        break;
                    case 0:
                        argbChrg = argbChrg + 65793 >= argbMax ? argbMax : argbChrg + 65793;
                        message = "argbChrg " + e.KeyChar.ToString();
                        break;
                    case 10:
                        this.BackColor = this.BackColor.Increment();
                        break;

                }
                if (!logger.busy&&!keyPressed && log)
                {
                    logger.LogMessage(message, comment, runHashCode);
                }
            }
            if (e.KeyChar == 45)
            {
                switch (optionSelector)
                {
                    case 1:
                        milBreak = milBreak <= 1 ? 0 : milBreak-1;
                        message = "milBreak " + e.KeyChar.ToString();
                        break;
                    case 2:
                        //main.chrg_mod = main.chrg_mod <= 2 ? 1 : main.chrg_mod - 1;
                        message = "chrg_mod " + e.KeyChar.ToString();
                        break;
                    case 3:
                        refresh = refresh <= 1 ? 0 : refresh - 1;
                        message = "refresh " + e.KeyChar.ToString();
                        break;
                    case 4:
                        //main.ents.Remove(main.ents.First());
                        message = "ents " + e.KeyChar.ToString();
                        break;
                    case 5:
                        a = a <= 0 ? 0 : a - 1;
                        message = "alpha " + e.KeyChar.ToString();
                        break;
                    case 6:
                        r = r <= 0 ? 0 : r - 1;
                        message = "red " + e.KeyChar.ToString();
                        break;
                    case 7:
                        g = g <= 0 ? 0 : g - 1;
                        message = "green " + e.KeyChar.ToString();
                        break;
                    case 8:
                        b = b <= 0 ? 0 : b - 1;
                        message = "blue " + e.KeyChar.ToString();
                        break;
                    case 9:                        
                        frequency = frequency - 0.0005 <= 0 ? frequency : frequency - 0.0005;
                        message = "frequency " + e.KeyChar.ToString();
                        break;
                    case 0:
                        argbChrg = argbChrg - 65793 <= argbMin ? argbMin : argbChrg - 65793;
                        message = "argbChrg " + e.KeyChar.ToString();
                        break;
                    case 10:
                        this.BackColor = this.BackColor.Decrease();
                        break;
                }
                if (!logger.busy && !keyPressed && log)
                {
                    logger.LogMessage(message, comment, runHashCode);
                }
            }
            if (e.KeyChar.ToString ().Equals ("e"))
            {
                Environment.Exit(0);
            }
            if (e.KeyChar.ToString().Equals("c"))
            {
                printer.USE_COLOURS = !printer.USE_COLOURS;
            }
            if (e.KeyChar.ToString().Equals("v"))
            {
                showEnts = !showEnts;
            }
            if (e.KeyChar.ToString().Equals("k"))
            {
                r = 255;
                g = 255;
                b = 255;
                kolors = !kolors;
                r = 255;
                g = 255;
                b = 255;
            }
            if (e.KeyChar.ToString().Equals("r"))
            {
                this.Refresh();
            }
            if (e.KeyChar == 'l' && !keyPressed)
            {
                line = !line;
            }
            keyPressed = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            keyPressed = false;
        }

        private void lowMenuItem_Click(object sender, EventArgs e)
        {
            smoothingMode = SmoothingMode.None;
            textSmoothing = TextRenderingHint.SystemDefault;
            lowToolStripMenuItem.Checked = true;
            highToolStripMenuItem.Checked = false;
        }

        private void highMenuItem_Click(object sender, EventArgs e)
        {
            smoothingMode = SmoothingMode.AntiAlias;
            textSmoothing = TextRenderingHint.AntiAlias;
            highToolStripMenuItem.Checked = true;
            lowToolStripMenuItem.Checked = false;
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
            }
            if (e.Button == MouseButtons.Right)
            {
                if (visible)
                {
                    visible = false;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    this.MainMenuStrip.Hide();
                    //this.textBox1.Hide();
                    //this.trackBar1.Hide();

                } else {
                    visible = true;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    this.MainMenuStrip.Show();
                    //this.textBox1.Show();
                    //this.trackBar1.Show();
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            milBreak = trackBar1.Value;
            textBox1.Text = milBreak + " ms";
        }

        private void window_SizeSet(object sender, EventArgs e)
        {
            this.Size = new Size(maxX+17, maxY+39);
        }

        // Graphics painting

        /*protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            Brush aBrush = (Brush)Brushes.Black;
            Brush aBrush2 = (Brush)Brushes.Red;
            Pen pen = (Pen) Pens.Red;
            //start an async task
            try
            {
                lock (locker) {    
                    //Task.Factory.StartNew(() =>
                    //{
                    //    taskID = (int)Task.CurrentId;
                        if (!pause)
                        {
                            if (showEnts)
                            {
                                using (Graphics gg = Graphics.FromImage(buffer))
                                {
                                if (borders)
                                    {
                                        gg.DrawLine(new Pen(Color.DarkBlue), new Point(maxX, 0), new Point(maxX, maxY));
                                        gg.DrawLine(new Pen(Color.DarkBlue), new Point(0, maxY), new Point(maxX, maxY));
                                    }

                                    gg.SmoothingMode = smoothingMode;
                                    gg.TextRenderingHint = textSmoothing;
                                    int tmpCnt = main.EntsCount();
                                    int tmpChr = main.ChrgsCount();

                                    for (int j = 0; j < tmpChr; j++)
                                    {
                                        aBrush2 = new SolidBrush(Color.FromArgb(argbChrg));
                                        try
                                        {
                                            if (main.ChrgTypeAt(j) == 1)
                                            {
                                                gg.DrawEllipse(new Pen(Color.FromArgb(argbChrg)), new RectangleF(main.ChrgPositionAt(j), new Size(3, 3)));
                                            }
                                            else
                                            {
                                                gg.FillEllipse(aBrush2, new RectangleF(main.ChrgPositionAt(j), new Size(2, 2)));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            RaiseError(ex);
                                        }
                                    }
                                    for (int i = 0; i < tmpCnt; i++)
                                    {
                                        try
                                        {
                                            if (kolors) { Somthg(main.EntityAt(i).maxAge); }
                                            int ts = main.EntitySizeAt(i);
                                            RectangleF r = new RectangleF(main.EntityPositionAt(i), new Size(6, 3));
                                            // Grows as it gets more meat
                                            // RectangleF r = new RectangleF(main.EntityPositionAt(i), new Size(6 + ts, 3 + ts));
                                            float angle = (float)(main.AngleAt(i) * (180.0 / Math.PI));
                                            RotateRectangle(gg, r, angle, main.VisIndicatorAt(i), main.EntitySizeAt(i), main.EntityPositionAt(i), main.EntityAt(i).target, main.EntityAt(i).dLine);
                                        }
                                        catch (Exception tm)
                                        {
                                            Debug.WriteLine(tm.Message);
                                            Debug.WriteLine(tm.StackTrace);
                                            RaiseError(tm);
                                        }
                                    }
                                }
                            }
                        }                                               
                        //invoke an action against the main thread to draw the buffer to the background image of the main form.
                        if (!this.IsDisposed)
                        {
                            try {
                                this.Invoke(new Action(() =>
                                {
                                    if (!pause) { main.SingleAction(); }
                                    this.BackgroundImage = buffer;
                                    Thread.Sleep(milBreak);
                                }));
                            } catch (Exception exc)
                            {
                                RaiseError(exc);
                            }
                        }
                    //});
                }   
            } catch (Exception z)
            {
                RaiseError(z);
            }
        }*/

        protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            Brush aBrush = (Brush)Brushes.Black;
            Brush aBrush2 = (Brush)Brushes.Red;
            Pen pen = (Pen)Pens.Red;
            //start an async task
            try
            {

                using (Graphics gg = Graphics.FromImage(buffer))
                {
                    if (borders)
                    {
                        gg.DrawLine(new Pen(Color.DarkBlue), new Point(maxX, 0), new Point(maxX, maxY));
                        gg.DrawLine(new Pen(Color.DarkBlue), new Point(0, maxY), new Point(maxX, maxY));
                    }

                    gg.SmoothingMode = smoothingMode;
                    gg.TextRenderingHint = textSmoothing;
                    //int tmpCnt = main.EntsCount();
                    //int tmpChr = main.ChrgsCount();
                    Dictionary<int, Point> tmpPosition = new Dictionary<int, Point>();
                    if (!actMessage.Equals(""))
                        foreach (string line in actMessage.Split(';'))
                        {
                            string[] vals = line.Split(' ');
                            if (!tmpPosition.ContainsKey(int.Parse(vals[0])))
                            {
                                Point tmp = new Point(int.Parse(vals[1]), int.Parse(vals[2]));
                                tmpPosition.Add(int.Parse(vals[0]), tmp);
                                gg.FillEllipse(aBrush2, new RectangleF(tmp, new Size(4, 4)));
                            }
                        }
                }
                //invoke an action against the main thread to draw the buffer to the background image of the main form.
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            actMessage = client.RequestData();
                            this.BackgroundImage = buffer;
                            Thread.Sleep(milBreak);
                        }));
                    }
                    catch (Exception exc)
                    {
                        RaiseError(exc);
                    }
                }
            }
            catch (Exception z)
            {
                RaiseError(z);
            }
        }

        private void RotateRectangle(Graphics gg, RectangleF rr, float angle, int visInd, int eSiz, Point from, Point to, bool dLine)
        {
            using (Matrix m = new Matrix())
            {
                int nom = (30 * eSiz) > 250 ? 250 : (30 * eSiz);
                //aBrush3.Color = Color.FromArgb(a + visInd, r - (3 * visInd), g - (3 * visInd), b);
                if (kolors) { aBrush3.Color = Color.FromArgb(a + visInd + eSiz, r, g, b); } 
                else { aBrush3.Color = Color.FromArgb(a + visInd, r, g - nom, b - nom); }
                SolidBrush fillBrush = new SolidBrush(Color.FromArgb(argb));
                m.RotateAt(angle, new PointF(rr.Left + (rr.Width / 2), rr.Top + (rr.Height / 2)));
                gg.Transform = m;
                if (eSiz > 0 && !kolors)
                {
                    fillBrush.Color = Color.FromArgb(a + visInd + eSiz, r, g - nom, b - nom);
                    gg.FillEllipse(fillBrush, rr);
                } else
                {
                    fillBrush.Color = Color.FromArgb(argb);
                    gg.FillEllipse(fillBrush, rr);
                }                
                gg.DrawEllipse(aBrush3, rr);
                gg.ResetTransform();
                if (line && dLine)
                {
                    gg.DrawLine(aBrush3, from.X, from.Y, to.X, to.Y);
                }
            }
        }

        private void Somthg(int val)
        {
            r = (int)(Math.Sin(frequency * val + 0) * 127 + 128);
            g = (int)(Math.Sin(frequency * val + 2) * 127 + 128);
            b = (int)(Math.Sin(frequency * val + 4) * 127 + 128);
        }

        // Console window implementation below

        //private void ConsoleInteract()
        //{
        //    Console.Clear();
        //    Console.CursorVisible = true;

        //    while (readMode)
        //    {
        //        string input = "";
        //        //printer.WriteLine(""+ System.Diagnostics.Process.GetCurrentProcess().Id+" "+ Process.GetCurrentProcess().ProcessName);
        //        printer.Write(ERROR ? "! " : "> ", WriterColours.GREEN);
        //        input = printer.ReadLine();
        //        string output = "";
        //        string[] arguments = input.Split(' ');
        //        if (arguments.Length == 3 && arguments[0].Equals("set") && arguments[1].Equals("speed") && Regex.IsMatch(arguments[2], "[-+]?[0-9]*\\.?[0-9]+"))
        //        {
        //            output = milBreak + " -> " + arguments[2];
        //            printer.WriteLine(output);
        //            milBreak = Convert.ToInt32(arguments[2]);
        //            if (!logger.busy && log) { logger.LogMessage(output, input, runHashCode); }
        //        }
        //        else if (arguments.Length == 3 && arguments[0].Equals("set") && arguments[1].Equals("refresh") && Regex.IsMatch(arguments[2], "[-+]?[0-9]*\\.?[0-9]+"))
        //        {
        //            output = refresh + " -> " + arguments[2];
        //            printer.WriteLine(output);
        //            refresh = Convert.ToInt32(arguments[2]);
        //            if (!logger.busy && log)
        //            { logger.LogMessage(output, input, runHashCode); }
        //        }
        //        else if (arguments[0].Equals("stats"))
        //        {
        //            readMode = false;
        //            PrintStats();
        //        }
        //        else if (arguments[0].Equals("sql"))
        //        {
        //            if (log) { SQLMode(input); } else { Console.WriteLine("SQL is disabled"); }
        //        }
        //        else if (arguments[0].Equals("help"))
        //        {
        //            ShowHelp();
        //        }
        //        else if (arguments[0].Equals("quote"))
        //        {
        //            Quotes quoter = new Quotes();
        //            printer.WriteLine(quoter.GetQuote(), WriterColours .YELLOW);
        //        }
        //        else if (arguments[0].Equals("list"))
        //        {
        //            if (log) { ListLogs(); }
        //        }
        //        else if (arguments[0].Equals("pause"))
        //        {
        //            pause = !pause;
        //        }
        //        else if (arguments[0].Equals("props"))
        //        {
        //            PrintProps();
        //        }
        //        else if (arguments[0].Equals("reload"))
        //        {
        //            pause = true;
        //            props.Reload();
        //            PropsLoad();
        //            main.PropsReload();
        //            //PrintProps();
        //            pause = false;
        //        }
        //        else if (arguments[0].Equals("cls"))
        //        {
        //            Console.Clear();
        //            Console.SetCursorPosition(0, 0);
        //        }
        //        else if (Regex.IsMatch(arguments[0], "colou?rs?"))
        //        {
        //            printer.USE_COLOURS = !printer.USE_COLOURS;
        //            output = "colours " + printer.USE_COLOURS.ToString();
        //            if (!logger.busy && log)
        //            { logger.LogMessage(output, input, runHashCode); }
        //        }
        //        else if (Regex.IsMatch(arguments[0], "errors?"))
        //        {
        //            int hash = -1;
        //            if (arguments.Length > 1)
        //            {
        //                if (Regex.IsMatch(arguments[1], "[0-9]+"))
        //                {
        //                    hash = Convert.ToInt32(arguments[1]);
        //                }
        //            }
        //            if (log) { PrintErrors(hash); }
        //            ERROR = false;
        //            printer.MULTI_LINES = WriterColours.STANDARD;
        //            printer.INPUT_COLOUR = WriterColours.STANDARD;
        //        }
        //        else if (arguments[0].Equals("visible"))
        //        {
        //            try
        //            {
        //                this.Visible = !this.Visible;
        //            }
        //            catch (Exception e)
        //            {
        //                RaiseError(e);
        //            }
        //        }
        //        else if (arguments[0].Equals("new"))
        //        {
        //            main.ents.Add(new LinkedL());
        //            output = "new singleEnt OK";
        //            printer.WriteLine(output, WriterColours.SUCCESS);
        //            if (!logger.busy && log)
        //            { logger.LogMessage(output, input, runHashCode); }
        //        } else if (arguments[0].Equals(""))
        //        {
        //            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop-1);
        //        }
        //        else
        //        {
        //            main.ParseInput(arguments);
        //        }
        //    }
        //}

        private void PrintProps()
        {
            foreach (string key in props.Keys())
            {
                Console.Write(key.ToUpper());
                Console.SetCursorPosition(22, Console.CursorTop);
                Console.WriteLine(" = " + props.Value(key));
            }
        }

        //private void PrintStats()
        //{
        //    Console.Clear();
        //    Console.CursorVisible = false;
        //    while (!readMode)
        //    {
        //        try
        //        {
        //            int count = main.EntsCount();
        //            int count2 = main.ChrgsCount();
        //            int mod = main.chrg_mod;
        //            int colour = count < 10 ? WriterColours.WARNING : WriterColours.GREEN;
        //            int colour2 = count2 < 10 ? WriterColours.WARNING : WriterColours.GREEN;
        //            int colour3 = count < 5 ? WriterColours.RED : colour;
        //            int colour4 = count2 < 5 ? WriterColours.RED : colour2;
        //            Console.SetCursorPosition(1, 1);
        //            printer.Write("ents  : ", count + "     ", WriterColours.INFO, colour3);
        //            Console.SetCursorPosition(15, 1);
        //            printer.Write("chrgs : ", count2 + "     ", WriterColours.INFO, colour4);
        //            Console.SetCursorPosition(30, 1);
        //            printer.Write("speed : ", milBreak + " ms     ", WriterColours.INFO, WriterColours.STANDARD);

        //            Console.SetCursorPosition(1, 2);
        //            printer.Write("refr  : ", refresh + "     ", WriterColours.INFO, WriterColours.STANDARD);
        //            Console.SetCursorPosition(15, 2);
        //            printer.Write("mod   : ", mod + "     ", WriterColours.INFO, WriterColours.STANDARD);
        //            Console.SetCursorPosition(30, 2);
        //            printer.Write("freq  : ", frequency + "     ", WriterColours.INFO, WriterColours.STANDARD);
        //            Console.SetCursorPosition(30, 3);
        //            printer.Write("ERR   : ", ERROR_COUNT + "     ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.GREEN : WriterColours.ERROR);

        //            Console.SetCursorPosition(1, 3);
        //            printer.Write("hun   : ", main.hungry + "     ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.GREEN : WriterColours.ERROR);
        //            Console.SetCursorPosition(15, 3);
        //            printer.Write("fed   : ", main.fed + "    ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.GREEN : WriterColours.ERROR);
                    
        //            Console.SetCursorPosition(1, 4);
        //            printer.Write("mMeat : ", main.meatMax+ "     ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.STANDARD : WriterColours.ERROR);
        //            Console.SetCursorPosition(15, 4);
        //            printer.Write("mLmt  : ", Math.Round(main.meatMax*0.7, 2) + "     ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.STANDARD : WriterColours.ERROR);
        //            Console.SetCursorPosition(30, 4);
        //            printer.Write("mHigh : ", main.cntMeatHigh + "     ", WriterColours.INFO, ERROR_COUNT < 1 ? WriterColours.STANDARD : WriterColours.ERROR);

        //            Console.SetCursorPosition(1, 9);
        //            printer.Write("ID", WriterColours.STRONG_MAGENTA);
        //            Console.SetCursorPosition(10, Console.CursorTop);
        //            printer.Write("AGE", WriterColours.STRONG_MAGENTA);
        //            Console.SetCursorPosition(15, Console.CursorTop);
        //            printer.Write("M_AGE", WriterColours.STRONG_MAGENTA);
        //            Console.SetCursorPosition(22, Console.CursorTop);
        //            printer.Write("X", WriterColours.STRONG_MAGENTA);
        //            Console.SetCursorPosition(27, Console.CursorTop);
        //            printer.Write("Y", WriterColours.STRONG_MAGENTA);
        //            Console.SetCursorPosition(32, Console.CursorTop);
        //            printer.WriteLine("GEN", WriterColours.STRONG_MAGENTA);
        //            printer.WriteLine(new string('-', 35), WriterColours.MAGENTA);
        //            LinkedL[] two = new LinkedL[2];
        //            two[0] = main.Youngest();
        //            two[1] = main.Oldest();
        //            printer.WriteLine("");
        //            printer.WriteLine("");
        //            printer.WriteLine("");
        //            printer.WriteLine("");
        //            Console.SetCursorPosition(1, Console.CursorTop - 4);
        //            foreach (LinkedL singleEnt in two)
        //            {
        //                Console.SetCursorPosition(1, Console.CursorTop);
        //                printer.Write(singleEnt.linkID + "");
        //                Console.SetCursorPosition(10, Console.CursorTop);
        //                printer.Write(singleEnt.PARENT.maxAge + "");
        //                Console.SetCursorPosition(15, Console.CursorTop);
        //                printer.Write(singleEnt.PARENT.orgMaxAge + "");
        //                Console.SetCursorPosition(22, Console.CursorTop);
        //                printer.Write(singleEnt.PARENT.posX + "");
        //                Console.SetCursorPosition(27, Console.CursorTop);
        //                printer.Write(singleEnt.PARENT.posY + "");
        //                Console.SetCursorPosition(32, Console.CursorTop);
        //                printer.WriteLine(string.Join(" : ", singleEnt.Depth().ToString()));
        //            }
        //            Console.SetCursorPosition(1, 14);
        //            printer.WriteLine("");
        //            Console.SetCursorPosition(1, 14);
        //            string elapsed = watch.Elapsed.Hours.ToString().PadLeft(2, '0') + ":" + watch.Elapsed.Minutes.ToString().PadLeft(2, '0') + ":" + watch.Elapsed.Seconds.ToString().PadLeft(2, '0');
        //            printer.Write("Run time : ", elapsed, WriterColours.INFO);
        //            Console.SetCursorPosition(1, 16);
        //            printer.WriteLine("App ID ",""+runHashCode, WriterColours.INFO);
        //            Console.SetCursorPosition(22, 16);
        //            printer.Write("ARGB ", a + " ", WriterColours.INFO);
        //            if (kolors)
        //            {
        //                printer.WriteLine(" dynamic ".ToString().PadRight(8, ' '), WriterColours.INFO);
        //            } else
        //            {
        //                printer.WriteLine(r + " " + g + " " + b.ToString().PadRight(8, ' '), WriterColours.INFO);
        //            }
        //            Console.SetCursorPosition(22, 14);
        //            printer.Write("Opt : ", optionSelected[optionSelector].PadRight(10,' '), WriterColours.STRONG_BLUE);
        //            //Console.SetCursorPosition(1, 20);
        //            //printer.Write(actMessage, WriterColours.STANDARD);

        //            Thread.Sleep(refresh);
        //        }
        //        catch (Exception e)
        //        {
        //            RaiseError(e);
        //        }
        //    }
        //    ConsoleInteract();
        //}

        private void DoEscape(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            if (readMode)
            {
                if (!logger.busy && log)
                { logger.LogMessage("Bye", "Ctrl + C", runHashCode); }
                Console.Clear();
                Environment.Exit(0);
            } else
            {
                //statsToolStripMenuItem.Checked = false;
                readMode = true;
            }
        }

        private void ListLogs()
        {
            DataTable dataTable = logger.GetLogs();
            // Parse Results (columns and lengths
            string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            int[] cellLength = new int[columnNames.Length];

            // Set initial column lengths based on columns' names
            for (int i = 0; i < columnNames.Length; i++)
            {
                cellLength[i] = columnNames[i].Length + 3;
            }

            // Process results if any
            if (columnNames.Any())
            {
                // Determine longest column in actual row results
                foreach (DataRow row in dataTable.Rows)
                {
                    string[] result = row.ItemArray.Where(x => x != null)
                       .Select(x => x.ToString())
                       .ToArray();
                    for (int i = 0; i < result.Length; i++)
                    {
                        cellLength[i] = cellLength[i] > result[i].Length + 4 ? cellLength[i] : result[i].Length + 4;
                        cellLength[i] = cellLength[i] > 54 ? 54 : cellLength[i]; // Make sure no column is wider than 50 chars
                    }
                }

                int curLeft = 2;
                printer.WriteLine("");
                int curTop = Console.CursorTop;

                // Print column names with table
                for (int o = 0; o < columnNames.Length; o++)
                {
                    Console.SetCursorPosition(curLeft, curTop);
                    printer.Write("+" + new string('-', cellLength[o]) + "+", WriterColours.YELLOW);
                    Console.SetCursorPosition(curLeft, curTop + 1);
                    printer.Write("| ", WriterColours.YELLOW); printer.Write(columnNames[o], WriterColours.MAGENTA);
                    Console.SetCursorPosition(curLeft, curTop + 2);
                    printer.Write("+" + new string('-', cellLength[o]) + "+", WriterColours.YELLOW);
                    curLeft += cellLength[o] + 1;
                }
                Console.SetCursorPosition(curLeft, curTop + 1);
                printer.Write("|", WriterColours.YELLOW);
                Console.SetCursorPosition(curLeft, curTop + 2);
                printer.WriteLine("");

                // Print all rows (table print)
                foreach (DataRow row in dataTable.Rows)
                {
                    string[] result = row.ItemArray.Where(x => x != null)
                       .Select(x => x.ToString())
                       .ToArray();
                    curLeft = 2;
                    for (int i = 0; i < result.Length; i++)
                    {
                        Console.SetCursorPosition(curLeft, Console.CursorTop);
                        string singleCell = result[i].Length + 3 > cellLength[i] ? result[i].Substring(0, cellLength[i] - 6) + " ..." : result[i];
                        printer.Write("| ", WriterColours.YELLOW); printer.Write(singleCell);
                        curLeft += cellLength[i] + 1;
                    }
                    Console.SetCursorPosition(curLeft, Console.CursorTop);
                    printer.Write("|", WriterColours.YELLOW);
                    printer.WriteLine("");
                }
                Console.SetCursorPosition(2, Console.CursorTop);
                printer.Write("+" + new string('-', curLeft - 3) + "+", WriterColours.YELLOW);
                printer.WriteLine("");
                printer.WriteLine("");
            }
        }

        private void PrintErrors(int hash)
        {
            int pull_hash = hash == -1 ? runHashCode : hash;
            DataTable dataTable = logger.GetMessages(2, pull_hash);
            foreach (DataRow row in dataTable.Rows)
            {
                string message_text = (string)row["message_text"];
                int mType = Convert.ToInt32("" + row["message_type"]);
                var source = row["source"];
                var stack_trace = row["stack_trace"];
                var target_site = row["target_site"];
                var comment = row["comment"];
                string date_cre = row["date_cre"].ToString();
                if (mType==0)
                {
                    printer.Write(date_cre, WriterColours.WARNING);
                    printer.Write(" : ");
                    if (!DBNull.Value.Equals(row["comment"]))
                    {
                        printer.Write((string)comment, WriterColours.INFO);
                        printer.Write(" : ");
                    }
                    printer.WriteLine(message_text, WriterColours.GREEN);
                }
                if (mType == 1)
                {
                    printer.Write(date_cre, WriterColours.WARNING);
                    printer.Write(" : ");
                    if (!DBNull.Value.Equals(row["comment"]))
                    {
                        printer.Write((string)comment, WriterColours.INFO);
                        printer.Write(" : ");
                    }
                    printer.WriteLine(message_text, WriterColours.GREEN);
                    if (!DBNull.Value.Equals(row["stack_trace"]))
                    {
                        printer.WriteLine((string)stack_trace, WriterColours.ERROR);
                    }
                }
                if (mType == 2)
                {
                    printer.WriteLine(date_cre, WriterColours.WARNING);
                    if (!DBNull.Value.Equals(row["comment"]))
                    {
                        printer.WriteLine((string)comment, WriterColours.INFO);
                    }
                    printer.WriteLine(message_text, WriterColours.GREEN);
                    if (!DBNull.Value.Equals(row["stack_trace"]))
                    {
                        printer.WriteLine((string)stack_trace, WriterColours.ERROR);
                    }
                }
                printer.WriteLine("");
            }
        }

        private void SQLMode(string userInput)
        {
            // Determine if SQL statement already provided 
            string input = "";
            if (userInput.Trim().Length > 3)
            {
                input = userInput.Remove(0, 3).Trim();
            } else
            {
                printer.Write("\\ ", WriterColours.YELLOW);
                input = printer.ReadLine();
            }
            if (!logger.busy)
            { logger.LogMessage("SQL", input, runHashCode); }
            
            // Execute SQL
            string[] arguments = input.Split(' ');
            DataTable results = logger.RunCode(input);
            // Parse Results (columns and lengths
            string[] columnNames = results.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            int[] cellLength = new int[columnNames.Length];

            // Set initial column lengths based on columns' names
            for (int i = 0; i < columnNames.Length; i++)
            {
                cellLength[i] = columnNames[i].Length+3;
            }

            // Process results if any
            if (columnNames.Any())
            {
                // Determine longest column in actual row results
                foreach (DataRow row in results.Rows)
                {
                    string[] result = row.ItemArray.Where(x => x != null)
                       .Select(x => x.ToString())
                       .ToArray();
                    for (int i =0; i<result.Length;i++)
                    {
                        cellLength[i] = cellLength[i] > result[i].Length+4 ? cellLength[i] : result[i].Length+4;
                        cellLength[i] = cellLength[i] > 54 ? 54 : cellLength[i]; // Make sure no column is wider than 50 chars
                    }
                }

                int curLeft = 2;
                printer.WriteLine("");
                int curTop = Console.CursorTop;

                // Print column names with table
                for (int o = 0; o < columnNames.Length; o++)
                {
                    Console.SetCursorPosition(curLeft, curTop);
                    printer.Write("+"+new string('-', cellLength[o])+"+", WriterColours.YELLOW);
                    Console.SetCursorPosition(curLeft, curTop + 1);
                    printer.Write("| ", WriterColours.YELLOW); printer.Write(columnNames[o], WriterColours.MAGENTA);
                    Console.SetCursorPosition(curLeft, curTop + 2);
                    printer.Write("+" + new string('-', cellLength[o]) + "+", WriterColours.YELLOW);
                    curLeft += cellLength[o]+1;
                }
                Console.SetCursorPosition(curLeft, curTop + 1);
                printer.Write("|", WriterColours.YELLOW);
                Console.SetCursorPosition(curLeft, curTop + 2);
                printer.WriteLine("");
                                
                // Print all rows (table print)
                foreach (DataRow row in results.Rows)
                {
                    string[] result = row.ItemArray.Where(x => x != null)
                       .Select(x => x.ToString())
                       .ToArray();
                    curLeft = 2;
                    for (int i = 0; i < result.Length; i++)
                    {
                        Console.SetCursorPosition(curLeft, Console.CursorTop);
                        string singleCell = result[i].Length+3 > cellLength[i] ? result[i].Substring(0, cellLength[i] - 6) + " ..." : result[i];
                        printer.Write("| ", WriterColours.YELLOW); printer.Write(singleCell);
                        curLeft += cellLength[i]+1;
                    }
                    Console.SetCursorPosition(curLeft, Console.CursorTop);
                    printer.Write("|", WriterColours.YELLOW);
                    printer.WriteLine("");
                }
                Console.SetCursorPosition(2, Console.CursorTop);
                printer.Write("+"+new string('-', curLeft-3)+"+", WriterColours.YELLOW);
                printer.WriteLine("");
                printer.WriteLine("");
            }
        }

        private static void RaiseError(Exception error)
        {
            string elapsed = watch.Elapsed.Hours.ToString().PadLeft(2, '0') + ":" + watch.Elapsed.Minutes.ToString().PadLeft(2, '0') + ":" + watch.Elapsed.Seconds.ToString().PadLeft(2, '0');
            ERROR_MESSAGE += elapsed + "\n" + error.ToString() + "\n";
            ERROR = true;
            ERROR_COUNT++;
            printer.MULTI_LINES = WriterColours.ERROR;
            printer.INPUT_COLOUR  = WriterColours.ERROR;
            try
            {
                if (!logger.busy && log)
                { logger.LogException(error, runHashCode); }
            } catch (Exception sql)
            {
                ERROR_MESSAGE += elapsed + "\n" + sql.ToString() + "\n";
                ERROR_COUNT++;
            }
        }
        
        private void ShowHelp()
        {
            printer.WriteLine("You can modify any props.conf file variables");
            printer.WriteLine("");
            printer.WriteLine("Console options :");
            printer.WriteLine(" ");
            printer.WriteLine(" help                                      - prints help list");
            printer.WriteLine(" set <option> <val>                        - set new value for given option");
            printer.WriteLine("    options : speed                        - App calulation speed (ents move speed)");
            printer.WriteLine("              refresh                      - stats screen refresh frequency");
            printer.WriteLine("              mod                          - food generation frequency");
            printer.WriteLine("              width                        - original plane width");
            printer.WriteLine("              height                       - original plane eight");
            printer.WriteLine("              radius                       - X and Y max boundaries");
            printer.WriteLine(" stats                                     - enters statistics screen");
            printer.WriteLine(" show <option>                             - shows value for selected option");
            printer.WriteLine("    options : count                        - current number of ents");
            printer.WriteLine("              food                         - current number of foods");
            printer.WriteLine("              size                         - X and Y boundaries");
            printer.WriteLine("              ents                         - prints out list of all ents");
            printer.WriteLine(" kill <[id | hash] | [first | last | all]> - kills selected singleEnt(s)");
            printer.WriteLine(" sql [query]                               - allows to execute sql query");
            printer.WriteLine(" quote                                     - prints out random funny quote");
            printer.WriteLine(" list                                      - lists all sessions logged in log table");
            printer.WriteLine(" reload                                    - reloads variables from props.conf file");
            printer.WriteLine(" props                                     - prints out variables from props.conf file");
            printer.WriteLine(" cls                                       - clears console window");
            printer.WriteLine(" colours                                   - toggles console color output");
            printer.WriteLine(" errors [session ID]                       - prints errors and logs for current [selected] session");
            printer.WriteLine(" visible                                   - toggles GUI visibility (broken)");
            printer.WriteLine(" new                                       - creates new singleEnt");
            printer.WriteLine(" exit                                      - quits application");
            printer.WriteLine(" ");
            printer.WriteLine("GUI options :");
            printer.WriteLine(" ");
            printer.WriteLine(" numeric keys (0-9) [- | +]                - switches option modes");
            printer.WriteLine("    -                                      - decrease selected option");
            printer.WriteLine("    +                                      - increase selected option");
            printer.WriteLine("    1                                      - App calulation speed (ents move speed)");
            printer.WriteLine("    2                                      - food generation frequency");
            printer.WriteLine("    3                                      - stats screen refresh frequency");
            printer.WriteLine("    4                                      - add / removes 1 singleEnt");
            printer.WriteLine("    5                                      - ents alpha channel (transparency)");
            printer.WriteLine("    6                                      - Red spectrum of ents RGB colour");
            printer.WriteLine("    7                                      - Green spectrum of ents RGB colour");
            printer.WriteLine("    8                                      - Blue spectrum of ents RGB colour");
            printer.WriteLine("    9                                      - frequency of ents colour change");
            printer.WriteLine("    0                                      - food alpha channel (transparency)");
            printer.WriteLine(" c                                         - toggles console colour output");
            printer.WriteLine(" v                                         - toggles ents visibility");
            printer.WriteLine(" l                                         - toggles singleEnt destination vector");
            printer.WriteLine(" k                                         - toggle ents dynamic colour change");
            printer.WriteLine(" r                                         - refreshes GUI");
            printer.WriteLine(" e                                         - quits application");
        }
    }
}
