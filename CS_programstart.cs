using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 程序正式开始运行了
/// 1、找到FTP服务器上的update文件
/// 2、与本地update文件进行对比
/// </summary>
public class CS_programstart : MonoBehaviour
{
    //1、找到FTP服务器上的update文件

    private string basePath = @"http://gewuji0127.gz01.bdysite.com/Administrator/";
    private string md5messagePath = "/updatemd5message/";
    private string assetbundlePath = "windows_assetbundle";

    private string[] servermd5message;

    private void Awake()
    {
        //StartCoroutine(FN_download());

        var str = File.ReadAllLines(Application.persistentDataPath + "/updatemd5message/update.txt");
        print(str.Length);

        foreach (var item in str)
        {
            print(item);
        }
    }

    private IEnumerator FN_download()
    {
        WWW wWW = new WWW("http://gewuji0127.gz01.bdysite.com/Administrator/updatemd5message/update.txt");

        yield return wWW;

        if (wWW.isDone == false)
        {
            print(wWW.error);
            yield break;
        }
        else
        {
            //servermd5message = wWW.text.Split('|');
            //servermd5message = wWW.text.Split(' ');

        }

        print(servermd5message.Length);

        for (int i = 0; i < servermd5message.Length; i++)
        {
            print(servermd5message[i]);
        }
    }

}
