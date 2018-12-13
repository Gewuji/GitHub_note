using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MySql.Data.MySqlClient;

namespace UnitySocketServer
{
    class Program
    {
        private static byte[] data = new byte[1024];
        private const string receiveCommand = "request_message";
        private static List<int> valueList = new List<int>();
        private static ReceiveMessage receiveMessage = new ReceiveMessage();
        private static Socket server = null;

        static void Main(string[] args)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse("192.168.1.193");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 9990);

            server.Bind(iPEndPoint);
            server.Listen(0);

            Console.WriteLine("服务器启动,正在监听客户端...");

            //异步接受连接
            server.BeginAccept(Callback_Accept, server);

            //Thread thread = new Thread(SaveData.InquireScore);
            //thread.Start();

            #region 数据库连接
            //try
            //{
            //    string connstr = "server=183.67.56.250;port=3306;user=root;password=root;database=test007";
            //    Console.WriteLine("登陆信息准备完毕");
            //    MySqlConnection conn = new MySqlConnection(connstr);
            //    Console.WriteLine("正在连接...");
            //    conn.Open();
            //    Console.WriteLine("已连接数据库...");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("错误:{0}", e.Message);
            //}
            #endregion

            Console.ReadKey();
        }

        /// <summary>
        /// 异步接收客户端的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private static void Callback_Accept(IAsyncResult ar)
        {
            Socket server = ar.AsyncState as Socket;//返回服务端socket
            Socket client = server.EndAccept(ar);//服务端socket结束accept得到客户端的socket

            Console.WriteLine("一个客户端{0}，已经连接进来...", client.LocalEndPoint);

            client.BeginReceive(
                receiveMessage.Data, receiveMessage.CannedData,
                receiveMessage.ResidueLength, SocketFlags.None, Callback_Receive, client);//客户端socket开始异步接受消息

            server.BeginAccept(Callback_Accept, server);//服务器socket再次开始异步几首客户端socket的连接
        }

        /// <summary>
        /// 异步处理信息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private static void Callback_Receive(IAsyncResult ar)
        {
            Socket client = ar.AsyncState as Socket;
            int length = client.EndReceive(ar);//结束异步receive接收消息，并得到接受到消息的长度

            //string message = Encoding.UTF8.GetString(data, 0, length);//编译消息得到string类型

            if (length == 0)
            {
                client.Close();
                return;
            }
            receiveMessage.AddCount(length);
            receiveMessage.ReadMessage();

            client.BeginReceive(
                receiveMessage.Data, receiveMessage.CannedData, receiveMessage.ResidueLength, SocketFlags.None, Callback_Receive, client);//再次接受消息
        }

        /// <summary>
        /// 向客户端返回信息
        /// </summary>
        public static void ReturnData()
        {
            byte[] allData = SaveData.ReturnData();
            server.Send(allData);
        }
    }
}
