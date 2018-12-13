using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySocketServer
{
    /// <summary>
    /// 自定义的一个保存客户端发来数据的类，用两个字典保存模拟数据
    /// 但是这种方法不利于数据的解析回传，正尝试用mysql数据库进行
    /// 数据保存
    /// </summary>
    class SaveData
    {
        private static List<string> cacheData = new List<string>();
        private static Dictionary<string, Dictionary<string, int>> musicAndPlayer
            = new Dictionary<string, Dictionary<string, int>>();

        public static void AddData(string data)
        {
            cacheData.Add(data);
            if (cacheData.Count == 3)
            {
                JoinDictionary(cacheData);
            }
        }

        private static void JoinDictionary(List<string> list)
        {
            if (!musicAndPlayer.ContainsKey(list[0]))//没有歌曲名
            {
                Dictionary<string, int> playerAndScore = new Dictionary<string, int>();
                playerAndScore.Add(list[1], int.Parse(list[2]));
                musicAndPlayer.Add(list[0], playerAndScore);
                //Console.WriteLine("什么数据都没有的情况已保存");
            }
            else//已有歌曲名
            {
                if (!musicAndPlayer[list[0]].ContainsKey(list[1]))//已有歌曲名中没有玩家名字
                {
                    musicAndPlayer[list[0]].Add(list[1], int.Parse(list[2]));
                    //Console.WriteLine("有歌曲没玩家的情况已保存");
                }
                else//已有歌曲名中有玩家名字
                {
                    if (musicAndPlayer[list[0]][list[1]] < int.Parse(list[2]))
                    {
                        musicAndPlayer[list[0]][list[1]] = int.Parse(list[2]);
                    }
                    //Console.WriteLine("有歌曲，有玩家的情况已更新数据");
                }
            }
            list.Clear();
            Console.WriteLine("已经将数据信息保存到字典中...");
        }

        public static void InquireScore()
        {
            while (true)
            {
                string musicName = Console.ReadLine();
                string playerName = Console.ReadLine();

                try
                {
                    Dictionary<string, int> vs = musicAndPlayer[musicName];
                    int score = vs[playerName];
                    Console.WriteLine("分数是：{0}", score);
                }
                catch (Exception e)
                {
                    Console.WriteLine("无法查询，错误信息：" + e.Message);
                }
                finally
                {
                    if (!musicAndPlayer.ContainsKey(musicName))
                    {
                        Console.WriteLine("没有主键");
                    }
                    else
                    {
                        if (!musicAndPlayer[musicName].ContainsKey(playerName))
                        {
                            Console.WriteLine("没有次键");
                        }
                        else
                        {
                            Console.WriteLine("没有分数");
                        }
                    }
                }
            }
        }

        internal static byte[] ReturnData()
        {

            return new byte[1024];
        }
    }
}
