using System.Collections;
using UnityEngine;
using System.IO;

/// <summary>
/// 从FTP服务器下载ab文件的简单模板
/// </summary>
public class CS_newdownloadfile : MonoBehaviour
{
    void Start()
    {
        //StartCoroutine(FN_loadassetBundle());
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FN_ReadFileInfo();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            FN_writeInfototxt(testString);
        }
    }

    /// <summary>
    /// 下载的函数
    /// </summary>
    /// <returns></returns>
    private IEnumerator FN_loadassetBundle()
    {
        string url = "http://gewuji0127.gz01.bdysite.com/Administrator/HomeFile/modle.unity3d";

        WWW wWW = new WWW(url);

        yield return wWW;

        if (wWW.isDone == false)
        {
            MonoBehaviour.print(wWW.error);
            yield break;
        }

        byte[] vs = new byte[1024];

        //Application.persistentDataPath一直指向以这个项目名命名的文件夹，在这个文件夹下的的子文件夹里的文件
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/programab_asset/newModle.unity3d");

        FileStream fileStream = fileInfo.Create();

        fileStream.Write(vs, 0, 1024);

        fileStream.Flush();

        fileStream.Close();

        fileStream.Dispose();

        MonoBehaviour.print("文件下载完毕!");
    }

    /// <summary>
    /// 读取txt文件的简单模板
    /// </summary>
    private void FN_ReadFileInfo()
    {
        string path = Application.persistentDataPath + "/newMessage.txt";

        StreamReader sr = new StreamReader(path);

        string txtinfo = sr.ReadToEnd();

        string[] strArray = txtinfo.Split('|');//通过|字符来分离字符串，标题和内容分离，增加可读性

        print(txtinfo);

        foreach (var item in strArray)
        {
            print(item);
        }

        string indexStr = strArray[1].Trim();

        print(indexStr);
    }

    private StreamWriter writer;
    private string testString = "【环球网军事10月10日报道】据俄罗斯卫星新闻通讯社报道，美国海军驻欧洲和非洲|司令詹姆斯·法戈9日称，美军“哈里·杜鲁门”号航母将参加北|约今秋在挪威举行的近年最大|规模演习。" +
        "法戈称：“以‘哈里·杜鲁门’号为首|的航母群将参加军演，人员数量将达5000人";


    /// <summary>
    /// 创建一个txt文件并写入信息的简单模板
    /// </summary>
    /// <param name="value"></param>
    private void FN_writeInfototxt(string message)
    {
        FileInfo file = new FileInfo(Application.persistentDataPath + "/newMessage.txt");

        if (!file.Exists)
        {
            writer = file.CreateText();
        }
        else
        {
            writer = file.AppendText();
        }

        writer.WriteLine(message);

        writer.Flush();
        writer.Close();
        writer.Dispose();

        print("信息写入成功,地址位于:" + Application.persistentDataPath + "/newMessage.txt");
    }
}
