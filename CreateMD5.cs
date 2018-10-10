using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// 生成MD5值的工具类
/// </summary>
public class CreateMD5
{
    /// <summary>
    /// 生成文件的MD5值
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string CreateMD5File(string file)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        MD5 mD5 = new MD5CryptoServiceProvider();
        byte[] retVal = mD5.ComputeHash(fs);
        fs.Close();

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            stringBuilder.Append(retVal[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}
