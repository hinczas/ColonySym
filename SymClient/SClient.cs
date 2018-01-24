using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;

namespace SymClient
{
    public class SClient
    {
        private string command = "";
        private string actualMessage = "";
        private string remoteIP;
        private static Stopwatch watch;
        private TcpClient tcpClient;
        private IPAddress ipAdress;
        private int messageLength;
        public bool clientMode = false;

        public SClient()
        {
            

            //while (!command.Equals("zegnaj"))
            //{
            //    SendMessage();
            //}

        }

        public void Init()
        {
            tcpClient = new TcpClient();
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                remoteIP = endPoint.Address.ToString();
                socket.Close();
            }
            ipAdress = IPAddress.Parse(remoteIP);

            OpenConnection();
        }

        private void OpenConnection()
        {
            try
            {
                 Console.WriteLine("Connecting.....");

                tcpClient.Connect(ipAdress, 8000);
                // Use the ipaddress as in the server program

                Console.WriteLine("Connected to "+remoteIP+":8000...");
                //Console.Write("Enter the string to be sent: ");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ops! Error! " + e.StackTrace);
            }
        }

        public void SendMessage()
        {
            //Console.Write("> ");
            String str = Console.ReadLine();
            while (str.Equals (""))
            {
                //Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                //Console.Write("> ");
                str = Console.ReadLine();
            }
            Stream stm = tcpClient.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(str);
            stm.Write(ba, 0, ba.Length);

            if (str.Equals("get"))
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                WaitForSize();
            } else
            {
                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);
                string mLength = System.Text.Encoding.UTF8.GetString(bb).Replace('\0', ' ').Trim();
                command = mLength;
                //Console.WriteLine(" ↓ " + DateTime.Now.ToString("HH:mm:ss")+ " : " + mLength);
                if (command.Equals ("zegnaj"))
                {
                    CloseConnection();
                }
            }
            
        }

        public string RequestData()
        {
            Stream stm = tcpClient.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes("get");
            stm.Write(ba, 0, ba.Length);
            
            WaitForSize();

            return actualMessage;
        }

        public void SendComm(string comm)
        {
            Stream stm = tcpClient.GetStream();

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(comm);
            stm.Write(ba, 0, ba.Length);
        }

        private void WaitForSize()
        {
            //Console.WriteLine("Wait For Size...");
            Stream stm = tcpClient.GetStream();
            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);
            string mLength = System.Text.Encoding.UTF8.GetString(bb).Replace('\0', ' ').Trim();
            //Console.WriteLine(" ↓ " + DateTime.Now.ToString("HH:mm:ss")+ " : " + mLength);
            int length;

            if (int.TryParse(mLength, out length))
            {
                messageLength = length;
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes("ready");
                stm.Write(ba, 0, ba.Length);
                //Console.WriteLine(" ↑ " + DateTime.Now.ToString("HH:mm:ss") + " : ready");
                WwaitForMessage();
            }
        }

        private void WwaitForMessage()
        {
            //Console.WriteLine("Wait For Data...");
            Stream stm = tcpClient.GetStream();
            byte[] bb = new byte[messageLength];
            int k = stm.Read(bb, 0, messageLength);
            string message = System.Text.Encoding.UTF8.GetString(bb).Replace('\0', ' ').Trim();
            //foreach (string line in message.Split(';'))
            //    Console.WriteLine(" ↓ " + " : " + line);
            //Console.WriteLine(" ↓ " + DateTime.Now.ToString("HH:mm:ss")+ " : request " + messageLength+ " received "+message.Length);
            actualMessage = message;
            //string elapsed = watch.Elapsed.Milliseconds.ToString();
            //Console.WriteLine("Pull time : " + elapsed);
        }
        private void CloseConnection()
        {
            try
            {

                Console.WriteLine("");

                tcpClient.Close();

                Console.WriteLine("Connection closed.");
                Console.WriteLine("Press Enter to exit...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ops! Error! " + e.StackTrace);
            }
        }

        static void Main(string[] args)
        {
            new SClient();
            Console.ReadLine();
        }
    }
}
