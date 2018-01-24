using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SymServer
{
    public class SServer
    {
        private string command = "";
        private IPAddress ipAdress;// = IPAddress.Parse("10.40.61.14");
        private TcpListener tcpListener;
        private Socket socket;
        public bool dataRequested = false;
        public bool ready = false;
        public bool commandMode = false;
        public string commandString = "";
        string localIP;

        public SServer()
        {
            using (Socket socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket2.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket2.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                socket2.Close();
            }
            ipAdress = IPAddress.Parse(localIP);
            tcpListener = new TcpListener(ipAdress, 8000);
            
        }

        public void StartServer()
        {
            try
            {
                // Start Listeneting at the specified port
                tcpListener.Start();

                Console.WriteLine("Server running - Port: 8000");
                Console.WriteLine("Local end point: " + tcpListener.LocalEndpoint);
                Console.WriteLine("Waiting for connections...");

                socket = tcpListener.AcceptSocket();
                // When accepted
                Console.WriteLine("Connection accepted from " + socket.RemoteEndPoint+" ["+ready.ToString()+" "+dataRequested .ToString()+" "+commandMode.ToString ()+"]");
                Listen();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
                dataRequested = false;
                ready = false;
            }
        }
        private void Listen()
        {
            try
            {
                while (!command.Equals("exit"))
                {
                    if (!dataRequested) {
                        byte[] b = new byte[100];
                        int k = socket.Receive(b);
                        command = System.Text.Encoding.UTF8.GetString(b).Replace('\0', ' ').Trim();
                        //Console.Write(" ↓ ");
                        string curTimeReceive = DateTime.Now.ToString("HH:mm:ss");
                        //Console.Write(curTimeReceive + " : ");
                        //Console.WriteLine(command);

                        ASCIIEncoding asen = new ASCIIEncoding();
                        if (command.Equals("exit"))
                        {
                            string curTimeSend = DateTime.Now.ToString("HH:mm:ss");
                            //Console.WriteLine(" ↑ " + curTimeSend + " : zegnaj");
                            socket.Send(asen.GetBytes("zegnaj"));
                            CloseConnection();
                        }
                        else
                        if (command.Equals("get"))
                        {
                            //string curTimeSend = DateTime.Now.ToString("HH:mm:ss");
                            //Console.WriteLine(" ↑ " + curTimeSend + " : wait");
                            //socket.Send(asen.GetBytes("wait"));
                            dataRequested = true;
                        }
                        else if (command.StartsWith("!"))
                        {
                            string curTimeSend = DateTime.Now.ToString("HH:mm:ss");
                            Console.Write("[server] ↓\t" + curTimeSend + " : "+command);
                            //socket.Send(asen.GetBytes("wait"));
                            commandMode = true;
                            commandString = command;
                            Console.WriteLine(" : commandMode " + commandMode.ToString());
                        }
                        else
                        {
                            //Console.WriteLine(" ↑ "+ curTimeReceive + " : ok");
                            socket.Send(asen.GetBytes("ok"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[server] !\tConnection lost... ");
                Console.WriteLine("[server] i\tWaiting for connections...");
                dataRequested = false;
                ready = false;
                try {
                    socket = tcpListener.AcceptSocket();
                    Console.WriteLine("[server] i\tConnection accepted from " + socket.RemoteEndPoint +" [" + ready.ToString() + " " + dataRequested.ToString() + " " + commandMode.ToString() + "]");
                    Listen();
                } catch (Exception extraInside) {
                    Console.WriteLine("[server] !\tError..... " + extraInside.StackTrace);
                }
            }
        }

        public void CloseConnection()
        {
            try
            {
                Console.WriteLine("");

                socket.Close();
                tcpListener.Stop();

                Console.WriteLine("[server] i\tConnection closed.");
                Console.WriteLine("[server] i\tPress Enter to exit...");
            }
            catch (Exception e)
            {
                Console.WriteLine("[server] !\tError..... " + e.StackTrace);
                dataRequested = false;
                ready = false;
            }
        }

        public void SendString(string message)
        {
            try
            {
                ASCIIEncoding asen = new ASCIIEncoding();
                string curTimeSend = DateTime.Now.ToString("HH:mm:ss");
                int length;
                if (ready)
                {
                    //Console.WriteLine(" ↑ " + curTimeSend + " : data...");
                    //Console.WriteLine(" ↑ " + curTimeSend + " : "+message);
                    socket.Send(asen.GetBytes(message));
                    //Console.Write("↑");
                    ready = false;
                } else {
                    if (int.TryParse(message, out length))
                    {
                        //Console.WriteLine(" ↑ " + curTimeSend + " : size...");
                        socket.Send(asen.GetBytes(message));
                        byte[] b = new byte[100];
                        int k = socket.Receive(b);
                        string response = System.Text.Encoding.UTF8.GetString(b).Replace('\0', ' ').Trim();
                        //Console.WriteLine(" ↓ " + curTimeSend + " : "+response);
                        if (response.Equals("ready"))
                        {
                            ready = true;
                        }

                    }
                }                
                dataRequested = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[server] !\tFailed to send data...");
                dataRequested = false;
                ready = false;
                //try
                //{
                //    tcpListener.Stop();
                //    tcpListener.Start();
                //    socket.Dispose();
                //    socket.Close();
                //    socket = tcpListener.AcceptSocket();
                //    Console.WriteLine("[server] i\tConnection accepted from " + socket.RemoteEndPoint + " [" + ready.ToString() + " " + dataRequested.ToString() + " " + commandMode.ToString() + "]");
                //    Listen();
                //}
                //catch (Exception extraInside)
                //{
                //    Console.WriteLine("[server] !\tError..... " + extraInside.StackTrace);
                //}
            }
        }
        static void Main(string[] args)
        {
            new SServer();
            Console.ReadLine();
        }
    }
}
