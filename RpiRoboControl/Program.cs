using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpiRoboControl
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);

        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
        {
            return 0 != (GetAsyncKeyState((int)vKey) & 0x8000);
        }

        static int up = 0;

        static void Main(string[] args)
        {
            new Thread(() =>
            {
                while(true)
                {
                    try
                    {
                        string ip = "";
                        using (FileStream f = new FileStream("config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(f))
                            {
                                ip = sr.ReadLine();
                            }
                        }
                            TcpClient c = new TcpClient(ip, 9090);
                        Console.WriteLine("START");
                        using (StreamWriter sw = new StreamWriter(c.GetStream()))
                        {
                            while (c.Connected)
                            {
                                Thread.Sleep(100);
                                if(IsKeyPushedDown(Keys.W))
                                {
                                    if(up != 0)
                                    {
                                        sw.WriteLine("w-down");
                                        sw.Flush();
                                        up = 0;
                                    }
                                }
                                else
                                {
                                    if (IsKeyPushedDown(Keys.S))
                                    {
                                        if (up != 0)
                                        {
                                            sw.WriteLine("s-down");
                                            sw.Flush();
                                            up = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (IsKeyPushedDown(Keys.A))
                                        {
                                            if (up != 0)
                                            {
                                                sw.WriteLine("a-down");
                                                sw.Flush();
                                                up = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (IsKeyPushedDown(Keys.D))
                                            {
                                                if (up != 0)
                                                {
                                                    sw.WriteLine("d-down");
                                                    sw.Flush();
                                                    up = 0;
                                                }
                                            }
                                            else
                                            {
                                                if (up == 0)
                                                {
                                                    up = 1;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (IsKeyPushedDown(Keys.Right))
                                {
                                    sw.WriteLine("head-right");
                                    sw.Flush();
                                }

                                if (IsKeyPushedDown(Keys.Left))
                                {
                                    sw.WriteLine("head-left");
                                    sw.Flush();
                                }

                                if (up == 1)
                                {
                                    up = 3;
                                    sw.WriteLine("up");
                                    sw.Flush();
                                }
                            }
                        }
                    }
                    catch { }
                }
            })
            {
                IsBackground = true
            }.Start();

            Console.ReadLine();
        }
    }
}
