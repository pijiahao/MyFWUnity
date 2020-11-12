﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common.Encrypt
{
    public class EncryptManager
    {
        public static string Encode(string data)
        {
            return Encode(data, "cpcpcpcp", "cpcpcpcp");
        }

        public static string Decode(string data)
        {
            return Decode(data, "cpcpcpcp", "cpcpcpcp");
        }

        public static string Encode(string data, string Key_64, string Iv_64)
        {
            string KEY_64 = Key_64;// "VavicApp";
            string IV_64 = Iv_64;// "VavicApp";
            try
            {
                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cst);
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public static string Decode(string data, string Key_64, string Iv_64)
        {
            string KEY_64 = Key_64;// "VavicApp";密钥
            string IV_64 = Iv_64;// "VavicApp"; 向量
            try
            {
                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
                byte[] byEnc;
                byEnc = Convert.FromBase64String(data); //把需要解密的字符串转为8位无符号数组
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }
        public static string GetMD5(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}
