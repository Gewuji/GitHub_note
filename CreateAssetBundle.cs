using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 编辑器扩展类
/// </summary>
public class CreateAssetBundle
{
    [MenuItem("AssetBundle/Build AssetBundle")]
    public static void BuildAssetBundle()
    {
        string path = "AssetBundles";

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
        Debug.Log("AssetBundle打包成功");
    }

    [MenuItem("AssetBundle/Create MD5 Files")]
    public static void CreateFiles()
    {
        #region 路径要在使用的时候根据情况修改
        string filesPath = @"D:\UnityWorkFile\ARForAndroid\AssetBundlesMD5File\files.txt";
        string fenziPath = @"D:\UnityWorkFile\ARForAndroid\AssetBundles\AssetBundles";

        #endregion

        if (File.Exists(filesPath))
        {
            File.Delete(filesPath);
        }

        FileStream fileStream = new FileStream(filesPath, FileMode.CreateNew);
        StreamWriter streamWriter = new StreamWriter(fileStream);

        string modleMD5 = CreateMD5.CreateMD5File(fenziPath);

        streamWriter.WriteLine("AssetBundle" + "|" + modleMD5);

        streamWriter.Close();
        fileStream.Close();

        Debug.Log("MD5文件创建完毕");
    }
}
