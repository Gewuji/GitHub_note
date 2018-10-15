using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 从下载Assetbundle文件开始运行程序(该脚本暂时冻结不做修改了2018/10/12/16：24)
/// </summary>
public class CS_downLoadAssetBundleStart : MonoBehaviour
{
    /// <summary>
    /// assetbundle目录的地址
    /// </summary>
    private string m_ManifestFilePath = "http://gewuji0127.gz01.bdysite.com/Administrator/windows_assetbundle/windows_assetbundle";
    /// <summary>
    /// assetbundle文件的基础地址
    /// </summary>
    private string m_AllAssetbundlePath = "http://gewuji0127.gz01.bdysite.com/Administrator/windows_assetbundle/";
    /// <summary>
    /// 所有AssetBundle文件名称,非路径只有名称
    /// </summary>
    private string[] m_AllAssetBundleNames;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        m_AllAssetBundleNames = new string[10];
    }


    /// <summary>
    /// 开始下载新的Assetbundle文件
    /// </summary>
    public void fun_DownLoadBegin()
    {
        StartCoroutine(fun_DownloadManifestFile(m_ManifestFilePath));
    }


    /// <summary>
    /// 读取本地的AssetBundle文件
    /// </summary>
    public void fun_ReadLocalAssetBundle()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/windows_assetbundle");

        for (int i = 0; i < directoryInfo.GetFiles().Length; i++)
        {
            m_AllAssetBundleNames[i] = Path.GetFileName(directoryInfo.GetFiles()[i].FullName);
        }

        fun_LoadAssetbundle();
    }


    /// <summary>
    /// 加载所有assetbundle资源
    /// </summary>
    private void fun_LoadAssetbundle()
    {
        #region 初级代码

        //string path1 = Application.persistentDataPath + "/windows_assetbundle/" + m_AllAssetBundleNames[0];
        //string path2 = Application.persistentDataPath + "/windows_assetbundle/" + m_AllAssetBundleNames[1];
        //string path3 = Application.persistentDataPath + "/windows_assetbundle/" + m_AllAssetBundleNames[2];

        ////print(path1);
        ////print(path2);
        ////print(path3);

        //AssetBundle asset1 = AssetBundle.LoadFromFile(path1);
        //asset1.LoadAllAssets();

        //AssetBundle asset2 = AssetBundle.LoadFromFile(path2);
        //GameObject[] objs = asset2.LoadAllAssets<GameObject>();
        //foreach (var item in objs)
        //{
        //    GameObject.Instantiate(item);
        //}

        //AssetBundle asset3 = AssetBundle.LoadFromFile(path3);
        //TextAsset[] textAssets = asset3.LoadAllAssets<TextAsset>();

        //foreach (var item in textAssets)
        //{
        //    string message = System.Text.Encoding.UTF8.GetString(item.bytes);
        //    Debug.Log(message);//这里读取到了txt文件的文字信息
        //}

        #endregion

        #region 升级后代码

        for (int i = 0; i < m_AllAssetBundleNames.Length; i++)
        {
            if (m_AllAssetBundleNames[i] == null) return;

            AssetBundle asset = AssetBundle.LoadFromFile(
                Application.persistentDataPath + "/windows_assetbundle/" + m_AllAssetBundleNames[i]);

            if (Path.GetExtension(asset.GetAllAssetNames()[0]) == ".mat")
            {
                asset.LoadAllAssets<Material>();
            }
            else if (Path.GetExtension(asset.GetAllAssetNames()[0]) == ".prefab")
            {
                GameObject[] objs = asset.LoadAllAssets<GameObject>();

                //这个回调函数用来处理加载到内存的GameObject文件
                DisposeGameObject(objs);

            }
            else if (Path.GetExtension(asset.GetAllAssetNames()[0]) == ".txt")
            {
                TextAsset[] textAssets = asset.LoadAllAssets<TextAsset>();

                //这个回调函数用来处理加载到内存的文字信息
                DisposeTextAsset(textAssets);
            }
        }

        #endregion
    }

    /// <summary>
    /// 下载AssetBundle的目录
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator fun_DownloadManifestFile(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);

        yield return unityWebRequest.SendWebRequest();

        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(unityWebRequest);

        AssetBundleManifest assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        m_AllAssetBundleNames = assetBundleManifest.GetAllAssetBundles();

        foreach (string item in m_AllAssetBundleNames)
        {
            StartCoroutine(fn_DownloadAssetBundleAndSave(m_AllAssetbundlePath + item));
        }
    }


    /// <summary>
    /// 通过www类请求到每一个ab文件的信息
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator fn_DownloadAssetBundleAndSave(string url)
    {
        WWW wWW = new WWW(url);

        yield return wWW;

        if (wWW.isDone)
        {
            fn_SaveAssetBundle(Path.GetFileName(url), wWW.bytes, wWW.bytes.Length);
        }
    }

    /// <summary>
    /// 储存为本地文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="bytes"></param>
    /// <param name="count"></param>
    void fn_SaveAssetBundle(string fileName, byte[] bytes, int count)
    {
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/windows_assetbundle/" + fileName);

        FileStream fileStream = fileInfo.Create();

        fileStream.Write(bytes, 0, count);

        fileStream.Flush();

        fileStream.Close();

        fileStream.Dispose();

        Debug.Log("下载完毕!!");
    }


    private void DisposeGameObject(GameObject[] objs)
    {
        foreach (var item in objs)
        {
            GameObject.Instantiate(item);
        }
    }

    private void DisposeTextAsset(TextAsset[] textAssets)
    {
        foreach (var item in textAssets)
        {
            Debug.Log(System.Text.Encoding.UTF8.GetString(item.bytes));
        }
    }
}
