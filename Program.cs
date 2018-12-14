using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace mysql_database
{
    /// <summary>
    /// mysql数据库增删改查事例工程
    /// 在调用数据库方法前，先要引用MySql.Data.dll类库
    /// 对于一些遗忘的sql命令，可以在mysql数据库当中做对应操作，在记录系统生成的sql命令
    /// </summary>
    class Program
    {
        //MySqlCommand command = new MySqlCommand(sqlstr, conn);
        //command.ExecuteReader();//执行查询
        //command.ExecuteNonQuery();//插入 删除
        //command.ExecuteScalar();//执行查询，返回一个单个的值
        static void Main(string[] args)
        {

            //数据库信息server地址为本机，端口号3306，用户root 密码root 数据库名称test007
            string connstr = "server=127.0.0.1;port=3306;user=root;password=root;database=vrgamedata;";

            //查询语句
            string sqlstr = "select * from musicname";

            //插入数据语句
            //string insert = "insert into user(username,password) value('liuyili','9987')";//注意严格按照sql语法书写
            string insert = "INSERT INTO `test007`.`user` (`username`, `password`) VALUES('wahahaha', '999')";//注意严格按照sql语法书写

            //更新语句
            string update = "UPDATE `test007`.`user` SET `username`='tttggg', `password`='123456' WHERE `id`='22'; ";

            //删除语句
            string deleteStr = "DELETE FROM `test007`.`user` WHERE `id`='24'";

            //创建数据库
            string createDatabase = "CREATE SCHEMA `vrgamedata`";

            MySqlConnection conn = ConnectionDatabase(connstr);//连接数据库

            SelectMySqlDatabase(sqlstr, conn);//查询数据

            //AddDatabase(insert, conn);//添加数据

            //UpdateDatabase(update, conn);//更新数据

            //DeleteDatabase(deleteStr, conn);//删除数据

            Console.ReadKey();
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connstr">建立连接的账号信息</param>>
        private static MySqlConnection ConnectionDatabase(string connstr)
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connstr);

                conn.Open();

                Console.WriteLine("已连接数据库...");

                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("错误提示:{0}", e.Message);
                return null;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        /// <summary>
        /// 删除数据库数据
        /// </summary>
        /// <param name="delete"></param>
        /// <param name="conn"></param>
        private static void DeleteDatabase(string delete, MySqlConnection conn)
        {
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(delete, conn);
                int result = command.ExecuteNonQuery();
                if (result == 1)
                {
                    Console.WriteLine("删除数据成功");
                }
                else
                {
                    Console.WriteLine("未能删除数据");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="update"></param>
        /// <param name="conn"></param>
        private static void UpdateDatabase(string update, MySqlConnection conn)
        {
            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(update, conn);
                int result = command.ExecuteNonQuery();

                if (result == 1)
                {
                    Console.WriteLine("更新{0}完成", result);
                }
                else
                {
                    Console.WriteLine("未更新");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="insert"></param>
        /// <param name="conn"></param>
        private static void AddDatabase(string insert, MySqlConnection conn)
        {
            try
            {
                conn.Open();
                MySqlCommand mySqlCommand = new MySqlCommand(insert, conn);
                int result = mySqlCommand.ExecuteNonQuery();
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sqlstr">sql语句的sring字符串</param>
        /// <param name="conn">一个意见建立的MySqlConnection连接</param>
        private static void SelectMySqlDatabase(string sqlstr, MySqlConnection conn)
        {
            MySqlCommand command = new MySqlCommand(sqlstr, conn);

            MySqlDataReader mySqlDataReader = null;

            try
            {
                conn.Open();
                mySqlDataReader = command.ExecuteReader();//执行读取
                while (mySqlDataReader.Read())//Read()方法是一行一行的读取数据，如果有数据返回true没有返回false
                {
                    Console.WriteLine("序号{0}：姓名{1}：密码{2}", mySqlDataReader[0], mySqlDataReader[1], mySqlDataReader[2]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }



        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="mySql"></param>
        private static void CloseConn(MySqlConnection mySql)
        {
            try
            {
                if (mySql != null)
                {
                    mySql.Close();
                    mySql.Dispose();
                    Console.WriteLine("已关闭数据库连接...");
                }
                GC.Collect();
            }
            catch (Exception e)
            {
                Console.WriteLine("错误信息：{0}", e.Message);
            }
        }
    }
}
