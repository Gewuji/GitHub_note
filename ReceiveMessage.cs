using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySocketServer
{
    /// <summary>
    /// 处理接收到信息的类，主要是解决数据粘包的问题
    /// </summary>
    class ReceiveMessage
    {
        private byte[] data = new byte[1024];
        private int cannedData = 0;

        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// 已存数据长度
        /// </summary>
        public int CannedData
        {
            get { return cannedData; }
        }

        /// <summary>
        /// 剩余空间长度
        /// </summary>
        public int ResidueLength
        {
            get { return data.Length - cannedData; }
        }

        public void AddCount(int count)
        {
            cannedData += count;
        }

        public void ReadMessage()
        {
            while (true)
            {
                if (CannedData <= 4) return;

                int dataLength = BitConverter.ToInt32(data, 0);//获得数据体的长度信息

                if (CannedData - 4 >= dataLength)//如果已存数据长度减去，数据表头的固定长度大于或者等于数据体的长度表示数据完整
                {
                    string message = Encoding.UTF8.GetString(data, 4, dataLength);

                    if (message == "requestMessage")//收到需要返回消息的指令
                    {
                        Program.ReturnData();
                    }
                    else
                    {
                        Console.WriteLine("解析出一条数据:" + message);
                        SaveData.AddData(message);

                        Array.Copy(data, 4 + dataLength, data, 0, CannedData - 4 - dataLength);

                        cannedData -= (dataLength + 4);
                    }
                }
                else break;
            }
        }
    }
}
