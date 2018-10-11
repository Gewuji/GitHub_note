using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CS_buildassetbundletool
{
    /// <summary>
    /// 打包
    /// </summary>
    [MenuItem("AssetBundle/build assetbundle for windows")]
    static void FN_buildassetbundleforwindows()
    {
        string path = Application.persistentDataPath + "/windows_assetbundle/";

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        MonoBehaviour.print("已打包AB文件到：" + path);
    }

    /// <summary>
    /// 创建MD5
    /// </summary>
    [MenuItem("AssetBundle/create md5 message")]
    static void FN_createmd5message()
    {
        string path = Application.persistentDataPath + "/windows_assetbundle/";

        string[] filename = Directory.GetFiles(path);

        foreach (var item in filename)
        {
            string md5message = CreateMD5.CreateMD5File(item);
            FN_writeInfototxt(Path.GetFileName(item) + "|" + md5message);
        }

        MonoBehaviour.print("信息写入成功,地址位于:" + path + "/update.txt");
    }

    /// <summary>
    /// 创建一个txt文件并写入信息的简单模板,
    /// </summary>
    /// <param name="value"></param>
    private static void FN_writeInfototxt(string message)
    {
        string path = Application.persistentDataPath + "/updatemd5message/";

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        StreamWriter writer;
        FileInfo file = new FileInfo(path + "update.txt");

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
    }
}
