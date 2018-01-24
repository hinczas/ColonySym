using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using SymClient;
using System.Collections.Generic;

namespace SeparateGUI
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Random randGen;
        private static Stopwatch watch;
        private SClient client;

        // privates

        private int milBreak, argb;
        private SmoothingMode smoothingMode;
        private TextRenderingHint textSmoothing;
        private static Object locker;
        private static Color argbCol;
        private Pen aBrush4, aBrush5;
        private string[] optionSelected;
        private string actMessage = "";

        // publics
        public static int runHashCode;

        // Init stage

        private void Init()
        {
            randGen     = new Random();
            locker      = new object();
            smoothingMode = SmoothingMode.AntiAlias;
            textSmoothing = TextRenderingHint.SystemDefault;
            optionSelected = new string[10];
            
            argbCol     = Color.DarkGray;
            argb        = Color.FromArgb(30, 37, 37, 38).ToArgb();
            aBrush4     = new Pen(Color.DarkBlue);
            aBrush5     = new Pen(Color.Gray);
        }
        

        public Form1()
        {
            Init();

            runHashCode = DateTime.UtcNow.ToString().GetHashCode();
            InitializeComponent();
            watch = System.Diagnostics.Stopwatch.StartNew();
            this.MainMenuStrip.Hide();
            this.textBox1.Hide();
            this.trackBar1.Hide();
            client = new SClient();
            Task.Factory.StartNew(() =>
                {
                    client.Init();
                });
        }
        

        private void lowMenuItem_Click(object sender, EventArgs e)
        {
            smoothingMode = SmoothingMode.HighQuality;
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
                // Solved by making console readable/writeable (type in commands)
                Console.Write("> ");
                string comm ="! "+ Console.ReadLine();
                client.SendComm(comm);


            }
            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void trackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            milBreak = trackBar1.Value;
            textBox1.Text = milBreak + " ms";
        }
       
        protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            Brush aBrush = (Brush)Brushes.Red;
            Brush aBrush2 = (Brush)Brushes.Gray;
            Pen pen = (Pen)Pens.Gray;
            Pen penRed = (Pen)Pens.DarkRed;
            //penRed.Color = Color.FromArgb(50, 200, 0, 0);
            //start an async task
            try
            {

                using (Graphics gg = Graphics.FromImage(buffer))
                {
                    gg.SmoothingMode = smoothingMode;
                    gg.TextRenderingHint = textSmoothing;
                    Dictionary<int, Point> tmpPosition = new Dictionary<int, Point>();
                    if (!actMessage.Equals(""))
                    {
                        string[] response = actMessage.Split('#');
                        gg.DrawString("ents : " + response[0].Split(';').Length, new Font(new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Gray), 5, 5);
                        gg.DrawString("chrg : " + response[1].Split(';').Length, new Font(new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Gray), 80, 5);

                        foreach (string line in response[0].Split(';'))
                        {
                            try
                            {
                                if (!line.Equals(""))
                                {
                                    string[] vals = line.Split(' ');
                                    if (!tmpPosition.ContainsKey(int.Parse(vals[0])))
                                    {
                                        try
                                        {
                                            Point tmp = new Point(int.Parse(vals[1]), int.Parse(vals[2]));
                                            tmpPosition.Add(int.Parse(vals[0]), tmp);

                                            // draw rotated
                                            RectangleF r = new RectangleF(tmp, new Size(6, 4));
                                            float angle = (float)(double.Parse(vals[3]) * (180.0 / Math.PI));
                                            RotateRectangle(gg, r, angle);
                                        }
                                        catch (Exception pd)
                                        {
                                            //Debug.WriteLine("Warning : Lost packet");
                                        }
                                    }
                                }
                            } catch (Exception dd)
                            {
                                //Debug.WriteLine("Warning : Desynchronised packet");
                            }   
                        }
                        foreach (string pnt in response[1].Trim().Split(';'))
                        {
                            try {                  
                                    if (!pnt.Equals (""))
                                {
                                    string[] pos = pnt.Split(',');
                                    Point tmp = new Point(int.Parse(pos[0]), int.Parse(pos[1]));
                                    gg.DrawEllipse(penRed, new RectangleF(tmp, new Size(2, 2)));
                                }  
                            }
                            catch (Exception fd)
                            {
                                //Debug.WriteLine("Warning : Desynchronised packet");
                            }
                        }                                   
                        //Console.Write("↓");
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
                            Thread.Sleep(10);
                        }));
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.Message);
                    }
                }
            }
            catch (Exception z)
            {
                Debug.WriteLine(z.Message);
            }
        }
        private void RotateRectangle(Graphics gg, RectangleF rr, float angle)
        {
            using (Matrix m = new Matrix())
            {
                m.RotateAt(angle, new PointF(rr.Left + (rr.Width / 2), rr.Top + (rr.Height / 2)));
                gg.Transform = m;
                gg.DrawEllipse(aBrush5, rr);
                gg.ResetTransform();
            }
        }
    }
}
