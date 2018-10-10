using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// AssetBundle下载管理类
/// </summary>
public class LoadAssetBundleManager : MonoBehaviour
{

    public MainManager mainManager;

    /// <summary>
    /// 服务器根目录
    /// </summary>
    private string webBaseURL = null;
    /// <summary>
    /// 本地资源跟目录
    /// </summary>
    private string localBaseURL = null;
    /// <summary>
    /// 服务器MD5目录
    /// </summary>
    private string webFileURL = null;
    /// <summary>
    /// 本地MD5目录
    /// </summary>
    private string localFileURL = null;
    /// <summary>
    /// 服务器MD5信息
    /// </summary>
    private string[] webFileInfo = null;
    /// <summary>
    /// 本地MD5信息
    /// </summary>
    private string[] localFileInfo = null;
    /// <summary>
    /// 主AssetBundle地址
    /// </summary>
    private string mainAssetBundleURL = null;
    private string allAssetBundle = null;

    private string webURL = null;
    private string[] assetsName;
    private bool isWebLoad;
    /// <summary>
    /// 写到硬盘的文件技术统计
    /// </summary>
    private int index;


    private SettingModle settingModle;

    private void Awake()
    {
        settingModle = gameObject.GetComponent<SettingModle>();
        isWebLoad = false;
        index = 0;
        webBaseURL = @"http://gewuji0127.gz01.bdysite.com/Files";
        localBaseURL = Application.persistentDataPath;
        webFileURL = webBaseURL + "/" + "files.txt";
        localFileURL = localBaseURL + "/files.txt";
        mainAssetBundleURL = @"http://gewuji0127.gz01.bdysite.com/UnityAssetBundleAndroid/AssetBundles";
        allAssetBundle = @"http://gewuji0127.gz01.bdysite.com/UnityAssetBundleAndroid/";
    }

    private void Start()
    {
        //开启程序
        StartProject();
    }

    /// <summary>
    /// 检查本地文件夹中是否有文件，作为整个程序的开始文件
    /// </summary>
    private void StartProject()
    {
        if (File.Exists(Application.persistentDataPath + "/files.txt"))
        {
            //下载MD5文件进行一致性对比
            StartCoroutine(DownFileMD5());
        }
        else
        {
            //直接从web服务器下载MD5文件和AssetBundle文件
            StartCoroutine(InitFunction());
        }
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitFunction()
    {
        #region 下载MD5文件并保存到本地
        WWW wwwWeb = new WWW(webFileURL);
        yield return wwwWeb;
        if (wwwWeb.isDone)
        {
            webFileInfo = wwwWeb.text.Split('|');
        }
        File.Delete(localFileURL);
        FileStream fileStream = new FileStream(localFileURL, FileMode.CreateNew);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.WriteLine(webFileInfo[0].Trim() + "|" + webFileInfo[1].Trim());
        streamWriter.Close();
        fileStream.Close();

        mainManager.uIManager.controlSlider.ChangeValue(0.3f);
        #endregion

        //下载AssetBundle文件
        StartCoroutine(LoadAssetBundleManifest());

    }


    /// <summary>
    /// 下载web服务器MD5信息并与本地MD5信息比对
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownFileMD5()
    {
        //读取web服务器MD5信息
        WWW wwwWeb = new WWW(webFileURL);
        yield return wwwWeb;
        if (wwwWeb.isDone)
        {
            webFileInfo = wwwWeb.text.Split('|');
            mainManager.uIManager.controlSlider.ChangeValue(0.15f);
        }

        //读取本地MD5信息，此处原来使用WWW类进行下载，但是在Android手机上WWW类的方法无法访问到文件，此处为巨坑...
        //后改用File.ReadAllText方法可以在Android手机上正确获取到文件信息
        localFileInfo = File.ReadAllText(Application.persistentDataPath + "/files.txt").Split('|');//读取本地MD5信息
        mainManager.uIManager.controlSlider.ChangeValue(0.15f);


        //如果MD5值一样就从本地加载资源
        if (webFileInfo[1].Trim() == localFileInfo[1].Trim())
        {
            mainManager.uIManager.controlSlider.ChangeValue(0.6f);
            LoadLocalHostAssetBundle();
        }
        //如果MD5值不一样就从服务器下载
        else
        {

            //1.将web端的files文件储存到本地
            File.Delete(localFileURL);

            FileStream fileStream = new FileStream(localFileURL, FileMode.CreateNew);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(webFileInfo[0].Trim() + "|" + webFileInfo[1].Trim());
            streamWriter.Close();
            fileStream.Close();

            mainManager.uIManager.controlSlider.ChangeValue(0.1f);
            //2.开始下载Web端AssetBundle资源保存到本地
            StartCoroutine(LoadAssetBundleManifest());
        }
    }

    /// <summary>
    /// 下载AssetBundle文件目录
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleManifest()
    {
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(mainAssetBundleURL);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);

            //下载主要AssetBundle文件
            AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            //读取所有的文件名称，并保存到一个数组里面
            assetsName = manifest.GetAllAssetBundles();

            //循环下载缓存到本地
            for (int i = 0; i < assetsName.Length; i++)
            {
                //根目录加上文件名称可循环遍历每一个AB文件
                StartCoroutine(DownLoadAssetBundleAndSave(allAssetBundle + assetsName[i], assetsName.Length));
            }

            isWebLoad = true;
        }
    }

    /// <summary>
    /// 下载缓存到本地
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator DownLoadAssetBundleAndSave(string url, int length)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone)
        {
            SaveAssetBundle(Path.GetFileName(url), www.bytes, www.bytes.Length, length);
        }
    }

    /// <summary>
    /// 保存到本地
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="bytes"></param>
    /// <param name="count"></param>
    private void SaveAssetBundle(string fileName, byte[] bytes, int count, int length)
    {
        //实例化一个文件信息类
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/" + fileName);

        //创建文件并获得一个文件流
        FileStream fileStream = fileInfo.Create();

        //向文件中写入相应信息
        fileStream.Write(bytes, 0, count);

        //并注入文件当中
        fileStream.Flush();

        //关闭文件流
        fileStream.Close();

        //处理掉文件流
        fileStream.Dispose();

        mainManager.uIManager.controlSlider.ChangeValue((0.6f / length));
        index++;
    }

    /// <summary>
    /// 从本地加载AssetBundle资源
    /// </summary>
    private void LoadLocalHostAssetBundle()
    {
        //AssetBundle.LoadFromFile(Application.persistentDataPath+"/"+"pl")

        AssetBundle ab1 = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + "player.unity3d");
        AssetBundle ab2 = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + "shader.unity3d");

        string[] materialArray = ab2.GetAllAssetNames();
        Material material = null;
        foreach (var item in materialArray)
        {
            Debug.Log("tishi:" + Path.GetFileName(item));
            if (Path.GetFileName(item) == "fenzi 1.mat")
            {
                Debug.Log(Path.GetFileName(item));
                material = ab2.LoadAsset<Material>(Path.GetFileName(item));
                Debug.Log("xianshi:" + Path.GetFileName(item));
            }
            //Debug.Log(Path.GetFileName("材质球名称:" + item));
        }

        string[] nameArray = ab1.GetAllAssetNames();

        foreach (var item in nameArray)
        {
            Debug.Log("模型名称：" + Path.GetFileName(item));
            GameObject obj = ab1.LoadAsset<GameObject>(Path.GetFileName(item));
            GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
            go.GetComponent<MeshRenderer>().material = material;
            if (go.name != "Directional Light(Clone)")
            {
                settingModle.GetModle(go);
            }
            go.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        //GameObject obj = ab1.LoadAsset<GameObject>("player");
        //GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.Euler(0, 180, 0));
        //settingModle.GetModle(go);
        mainManager.uIManager.controlSlider.ChangeValue(0.1f);
        mainManager.uIManager.CloseLoadAssetBundlePanel();
        mainManager.ChangeIsRecognition();
    }

    private void Update()
    {
        if (isWebLoad == true)
        {
            if (assetsName.Length == index)
            {
                LoadLocalHostAssetBundle();
                index = 0;
            }
        }
    }
}
