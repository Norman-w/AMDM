using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



//加密步骤 将类对象序列化
//按位取反
//BASE64加密
//按位取反
//每8位为一组
//前面增加8位,第一位为1,然后第2,3,4,5,6,7,8位分别为之前的8位一组的88,7,6,5,4,3,2位的值
//组合出来的16位每段的值使用system.io.file.writeallbytes写入文件

//解密步骤
//每16个字节为一组
//去掉前面的8位
//按位取反
//得到的全部内容 使用base64解密
//按位取反
//反序列化


public static class Ciphertext
{
    public static string Encode<T>(T obj)
    {
        byte[] bytesSerialized = Serialize(obj);
        byte[] bytesSerializedFan = new List<byte>(bytesSerialized).ToArray();
        for (int i = 0; i < bytesSerialized.Length; i++)
        {
            bytesSerializedFan[i] = (byte)~bytesSerialized[i];
        }
        //base64加密
        string stringBase64Encoded = Convert.ToBase64String(bytesSerializedFan);
        byte[] bytesBase64Encoded = System.Text.Encoding.Unicode.GetBytes(stringBase64Encoded);
        //取反
        for (int i = 0; i < bytesBase64Encoded.Length; i++)
        {
            bytesBase64Encoded[i] = (byte)~bytesBase64Encoded[i];
        }
        //byte转换成8位的字符串
        List<string> bytesX2_8WaitEncode = new List<string>();
        for (int i = 0; i < bytesBase64Encoded.Length; i++)
        {
            string x2 = Convert.ToString(bytesBase64Encoded[i], 2);
            bytesX2_8WaitEncode.Add(x2);
        }
        //byte转换成bit,前面添加8位
        List<string> bytesX2_16 = new List<string>();
        for (int i = 0; i < bytesX2_8WaitEncode.Count; i++)
        {
            string x2 = bytesX2_8WaitEncode[i];
            string x2_16bit = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", 1, x2[7], x2[6], x2[5], x2[4], x2[3], x2[2], x2[1], x2[0], x2[1], x2[2], x2[3], x2[4], x2[5], x2[6], x2[7]);
            bytesX2_16.Add(x2_16bit);
        }
        //1234567812345678  这16位 再次进行修改  把原本的数据的后12345678放在16位的从2-8的规律性位置(只能到8,因为8910,11,12,13,14,15就到头了) 比如  第0个字节 放在2位置  第1个字节放在第3位置 依次循环
        //每8-2+1次为一个循环 也就是 2 3 4 5 6 7 8位的样式开始放
        /*
         * 例子
         * 0000000000000000原本
         * 0011111111000000第一个字节
         * 0001111111100000第二个字节
         * 0000111111110000第三个字节
         * ....
         * 0000000011111111第8个字节
         * 0011111111000000第9个字节
         */
        int startInsertPos = 7;
        int endInsertPos = 8;
        for (int i = 0; i < bytesX2_16.Count; i++)
        {
            string x2 = bytesX2_16[i];
            int iterater = i % (endInsertPos - startInsertPos);
            int realIterrater = iterater + startInsertPos;//真正的游标位置 按照例子,第一次是2
            x2 = x2.Substring(0, realIterrater) + bytesX2_8WaitEncode[i] + x2.Substring(realIterrater + 8);//没用的部分 真正的部分 后面的部分 可能是有用的部分的一部分 比如 没用的是(加位用的00000000+真正11111111) 有用的是11111111 组合起来是 00+11111111+111111
            bytesX2_16[i] = x2;
        }
        //将0101010101等格式转换成字符,字符拼接成字符串,写入到文件
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytesX2_16.Count; i++)
        {
            string jiemahou = jiema(bytesX2_16[i]);
            sb.Append(jiemahou);
        }
        return sb.ToString();
    }
    public static T Decode<T>(string code)
    {
        List<string> bytexX2_16_decode = new List<string>();
        for (int i = 0; i < code.Length; i++)
        {
            string current = code[i].ToString();
            string x16jiemahou = bianma(current);
            bytexX2_16_decode.Add(x16jiemahou);
        }
        ////去掉8位
        //List<string> bytesX2_8strList = new List<string>(0);
        //for (int i = 0; i < bytexX2_16_decode.Count; i++)
        //{
        //    bytesX2_8strList.Add(bytexX2_16_decode[i].Substring(8));
        //}
        //提取真正的8位
        int startInsertPos = 7;
        int endInsertPos = 8;
        List<string> bytesX2_8strList = new List<string>();
        for (int i = 0; i < bytexX2_16_decode.Count; i++)
        {
            string x2_16 = bytexX2_16_decode[i];
            int iterater = i % (endInsertPos - startInsertPos);
            int realIterrater = iterater + startInsertPos;//真正的游标位置 按照例子,第一次是2
            string x2_8 = x2_16.Substring(realIterrater, 8);
            bytesX2_8strList.Add(x2_8);
        }
        //转换成byte
        List<byte> bytesDecoded = new List<byte>();
        for (int i = 0; i < bytesX2_8strList.Count; i++)
        {
            //byte[] bs = Encoding.Unicode.GetBytes(bytesX2_8strList[i]);
            byte b = Convert.ToByte(bytesX2_8strList[i], 2);
            bytesDecoded.Add(b);
        }

        //取反
        for (int i = 0; i < bytesDecoded.Count; i++)
        {
            bytesDecoded[i] = (byte)~bytesDecoded[i];
        }
        //转换成string
        string bytesDecodedStr = System.Text.Encoding.Unicode.GetString(bytesDecoded.ToArray());
        //string bytesDecodedStr = stringBase64Encoded;
        //base64解密
        byte[] base64decoded = Convert.FromBase64String(bytesDecodedStr);

        byte[] bytesDeserialzed = base64decoded;
        for (int i = 0; i < bytesDeserialzed.Length; i++)
        {
            bytesDeserialzed[i] = (byte)~bytesDeserialzed[i];
        }
        T uh2 = Deserialize<T>(bytesDeserialzed);
        return uh2;
    }

    /// <summary>
    /// 将二进制转成字符串
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static string jiema(string s)
    {
        System.Text.RegularExpressions.CaptureCollection cs =
            System.Text.RegularExpressions.Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
        byte[] data = new byte[cs.Count];
        for (int i = 0; i < cs.Count; i++)
        {
            data[i] = Convert.ToByte(cs[i].Value, 2);
        }
        return Encoding.Unicode.GetString(data, 0, data.Length);
    }
    /// <summary>
    /// 将字符串转成二进制
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    static string bianma(string s)
    {
        byte[] data = Encoding.Unicode.GetBytes(s);
        StringBuilder result = new StringBuilder(data.Length * 8);

        foreach (byte b in data)
        {
            result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        return result.ToString();
    }
    static byte[] Serialize(object data)
    {
        if (null == data) return null;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        return Encoding.Unicode.GetBytes(json);

        //MemoryStream rems = null;
        //try
        //{
        //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    rems = new MemoryStream();
        //    formatter.Serialize(rems, data);
        //}
        //catch (Exception)
        //{
        //    throw;
        //}

        //return rems.GetBuffer();
    }
    static T Deserialize<T>(byte[] data)
    {
        object obj = null;
        string json = Encoding.Unicode.GetString(data);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        //try
        //{
        //    if (null == data) return null;
        //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    MemoryStream rems = new MemoryStream(data, 0, data.Length);
        //    obj = formatter.Deserialize(rems);
        //}
        //catch (Exception e)
        //{
        //    throw e;
        //    //MessageBox.Show(e.Message);
        //    //throw;
        //}
        //return obj;
    }
}