using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 程序从验证MD5的值正式开始运行
/// 1、找到FTP服务器上的update文件
/// 2、与本地update文件进行对比
/// </summary>
public class CS_MD5Infostart : MonoBehaviour
{
    //1、找到FTP服务器上的update文件

    /// <summary>
    /// MD5信息的Web地址
    /// </summary>
    private string m_MD5MessageWebPath = "http://gewuji0127.gz01.bdysite.com/Administrator/updatemd5message/update.txt";

    private string m_MD5MessageLocalPath = "/updatemd5message/update.txt";

    private string[] m_SaveWebMD5Message;
    private string[] m_SaveLocalMD5Message;

    [SerializeField]
    private CS_downLoadAssetBundleStart cS_DownLoadAssetBundleStart;

    private void Awake()
    {
        StartCoroutine(fun_ReadMD5Message(m_MD5MessageWebPath));
    }

    IEnumerator fun_ReadMD5Message(string url)
    {
        WWW wWW = new WWW(url);

        yield return wWW;

        if (wWW.isDone)
        {
            m_SaveWebMD5Message = wWW.text.Split(':');
        }

        m_SaveLocalMD5Message = File.ReadAllText(Application.persistentDataPath + m_MD5MessageLocalPath).Split(':');

        if (m_SaveWebMD5Message[1].Trim() == m_SaveLocalMD5Message[1].Trim())//如果本地和服务器MD5匹配
        {
            fun_DisposeEqualTo();
        }
        else//如果本地和服务器MD5不匹配
        {
            Debug.Log("MD5信息不一致");
            fun_DisposeNotEqualTo(m_SaveWebMD5Message);
        }
    }

    /// <summary>
    /// 处理MD5不相等
    /// </summary>
    private void fun_DisposeNotEqualTo(string[] md5Message)
    {
        //1、删除本地的MD5文件
        File.Delete(Application.persistentDataPath + m_MD5MessageLocalPath);

        //2、下载服务器的MD5文件并保存到本地
        FileStream fileStream = new FileStream(
            Application.persistentDataPath + m_MD5MessageLocalPath, FileMode.CreateNew);
        StreamWriter streamWriter = new StreamWriter(fileStream);

        streamWriter.WriteLine(md5Message[0].Trim() + ":" + md5Message[1].Trim());

        streamWriter.Close();

        fileStream.Close();

        Debug.Log("MD5信息更新完成");

        //3、下载新的AssetBundle文件
        fun_DonwLoadAssetBundle();
    }

    /// <summary>
    /// 下载新的AssetBundle文件
    /// </summary>
    private void fun_DonwLoadAssetBundle()
    {
        cS_DownLoadAssetBundleStart.fun_DownLoadBegin();
    }

    /// <summary>
    /// 处理MD5相等
    /// </summary>
    private void fun_DisposeEqualTo()
    {
        cS_DownLoadAssetBundleStart.fun_ReadLocalAssetBundle();
    }
}
