using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMDMClientSDK
{
    /// <summary>
    /// 中文汉字转换拼音
    /// </summary>
    public class CNChar2PinyinIndexesConverter
    {
        public CNChar2PinyinIndexesConverter()
        {
            #region 简单的构造和使用

            //string text = "覃说乐";
            //Dictionary<char, List<string>> charsFullPinyins = new Dictionary<char, List<string>>();
            //Dictionary<char, List<string>> charsFirstPinyins = new Dictionary<char, List<string>>();
            //for (int i = 0; i < text.Length; i++)
            //{
            //    char current = text[i];
            //    charsFullPinyins.Add(current, new List<string>());
            //    charsFirstPinyins.Add(current, new List<string>());

            //    ChineseChar cnschar = new ChineseChar(current);
            //    List<string> firstPinyins = new List<string>();
            //    for (int j = 0; j < cnschar.PinyinCount; j++)
            //    {
            //        //没有声调的拼音全拼
            //        string fullPinyinNoTone = cnschar.Pinyins[j].Substring(0, cnschar.Pinyins[j].Length - 1);
            //        if (charsFullPinyins[current].Contains(fullPinyinNoTone) == false)
            //        {
            //            charsFullPinyins[current].Add(fullPinyinNoTone);
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //        #region 给出拼音首字母
            //        if (fullPinyinNoTone.StartsWith("ZH")
            //            || fullPinyinNoTone.StartsWith("CH") || fullPinyinNoTone.StartsWith("SH"))
            //        {
            //            //额外添加翘舌音
            //            firstPinyins.Add(fullPinyinNoTone.Substring(0, 2));
            //        }
            //        firstPinyins.Add(fullPinyinNoTone.Substring(0, 1));
            //        #endregion
            //    }
            //    #region 由于系统的函数判断的首字母的话只是ZCS这样的 所以平翘舌的时候要单独的添加ZH CH SH 到待生成的数据里面去 然后再去和
            //    for (int j = 0; j < firstPinyins.Count; j++)
            //    {
            //        string currentFirstPinyin = firstPinyins[j];
            //        if (charsFirstPinyins[current].Contains(firstPinyins[j]) == false)
            //        {
            //            charsFirstPinyins[current].Add(firstPinyins[j]);
            //        }
            //    }
            //    #endregion

            //}
            //MakeFullPinyinCombains(text[0], charsFullPinyins, text);
            //MakeFirstPinyinCombains(text[0], charsFirstPinyins, text);
            #endregion
        }
        #region 全拼迭代
        //迭代器
        List<string> fullPinyinIterator = new List<string>();
        //组合好的可能
        List<List<string>> fullPinyinCombainResult = new List<List<string>>();
        /// <summary>
        /// 递归制作拼音的可能性
        /// </summary>
        /// <param name="current"></param>
        /// <param name="charsFullPinyins"></param>
        /// <param name="text"></param>
        //void MakeFullPinyinCombains(char current, Dictionary<char, List<string>> charsFullPinyins, string text)
        void MakeFullPinyinCombains(int currentCharPos, List<List<string>> charsFullPinyins, string text)
        {
            //当前字符的拼音
            List<string> currentPinyins = charsFullPinyins[currentCharPos];
            //这个字符后面还有没有其他的字符
            bool hasNext = (currentCharPos + 1) >= text.Length ? false : true;
            for (int i = 0; i < currentPinyins.Count; i++)
            {
                fullPinyinIterator.Add(currentPinyins[i]);
                //如果还有下一个字符,进入下一个字符
                if (hasNext)
                {
                    //下一个字符
                    char next = text[currentCharPos + 1];
                    MakeFullPinyinCombains(currentCharPos + 1, charsFullPinyins, text);
                }
                else
                {
                    //如果是最后一个字符了, 那就添加这种已经迭代好的可能.
                    //新建一个cantChangedPinyin是为了迭代器会随着进退的情况改变,也会改变combains里面的数据,所以要单独的一个数组保存
                    List<string> cantChangedPinyin = new List<string>();
                    cantChangedPinyin.AddRange(fullPinyinIterator);
                    //添加拼音信息进去,这个信息的保存结构是不会变的.如果使用iterator直接保存就会改变
                    fullPinyinCombainResult.Add(cantChangedPinyin);
                    //当前完成,回归到上一位
                    fullPinyinIterator.RemoveAt(fullPinyinIterator.Count - 1);
                }
            }
            if (fullPinyinIterator.Count > 0)
            {
                //完成当前循环以后,迭代到上一位
                fullPinyinIterator.RemoveAt(fullPinyinIterator.Count - 1);
            }
        }
        #endregion
        #region 首字母迭代
        //迭代器
        List<string> firstPinyinIterator = new List<string>();
        //组合好的可能
        List<List<string>> firstPinyinCombainResult = new List<List<string>>();
        /// <summary>
        /// 递归制作拼音的可能性
        /// </summary>
        /// <param name="current"></param>
        /// <param name="charsfirstPinyins"></param>
        /// <param name="text"></param>
        //        void MakeFirstPinyinCombains(char current, Dictionary<char, List<string>> charsfirstPinyins, string text)
        void MakeFirstPinyinCombains(int currentCharPos, List<List<string>> charsfirstPinyins, string text)
        {
            //当前字符的拼音
            List<string> currentPinyins = charsfirstPinyins[currentCharPos];
            //这个字符后面还有没有其他的字符
            bool hasNext = (currentCharPos + 1) >= text.Length ? false : true;
            for (int i = 0; i < currentPinyins.Count; i++)
            {
                firstPinyinIterator.Add(currentPinyins[i]);
                //如果还有下一个字符,进入下一个字符
                if (hasNext)
                {
                    //下一个字符
                    char next = text[currentCharPos + 1];
                    MakeFirstPinyinCombains(currentCharPos + 1, charsfirstPinyins, text);
                }
                else
                {
                    //如果是最后一个字符了, 那就添加这种已经迭代好的可能.
                    //新建一个cantChangedPinyin是为了迭代器会随着进退的情况改变,也会改变combains里面的数据,所以要单独的一个数组保存
                    List<string> cantChangedPinyin = new List<string>();
                    cantChangedPinyin.AddRange(firstPinyinIterator);
                    //添加拼音信息进去,这个信息的保存结构是不会变的.如果使用iterator直接保存就会改变
                    firstPinyinCombainResult.Add(cantChangedPinyin);
                    //当前完成,回归到上一位
                    firstPinyinIterator.RemoveAt(firstPinyinIterator.Count - 1);
                }
            }
            if (firstPinyinIterator.Count > 0)
            {
                //完成当前循环以后,迭代到上一位
                firstPinyinIterator.RemoveAt(firstPinyinIterator.Count - 1);
            }
        }
        #endregion

        /// <summary>
        /// 将汉子转换出来可以组合的拼音可能
        /// </summary>
        /// <param name="value"></param>
        /// <param name="getFullPinyin"></param>
        /// <param name="getFirstPinyin"></param>
        /// <returns></returns>
        public List<string> ToPinyin(string value, bool getFullPinyin, bool getFirstPinyin)
        {
            List<string> allResult = new List<string>();
            if ((getFirstPinyin == false && getFullPinyin == false) || value == null || value.Length < 1)
            {
                return allResult;
            }
            //Dictionary<char, List<string>> charsFullPinyins = new Dictionary<char, List<string>>();
            List<List<string>> charsFullPinyins = new List<List<string>>();
            //Dictionary<char, List<string>> charsFirstPinyins = new Dictionary<char, List<string>>();
            List<List<string>> charsFirstPinyins = new List<List<string>>();
            for (int i = 0; i < value.Length; i++)
            {
                //char current = value[i];
                //charsFullPinyins.Add(current, new List<string>());
                //charsFirstPinyins.Add(current, new List<string>());
                charsFullPinyins.Add(new List<string>());
                charsFirstPinyins.Add(new List<string>());

                if (ChineseChar.IsValidChar(value[i]) == false)
                {
                    charsFullPinyins[i].Add(value[i].ToString());
                    charsFirstPinyins[i].Add(value[i].ToString());
                    continue;
                }

                ChineseChar cnschar = new ChineseChar(value[i]);
                List<string> firstPinyins = new List<string>();
                for (int j = 0; j < cnschar.PinyinCount; j++)
                {
                    //没有声调的拼音全拼
                    string fullPinyinNoTone = cnschar.Pinyins[j].Substring(0, cnschar.Pinyins[j].Length - 1);
                    if (charsFullPinyins[i].Contains(fullPinyinNoTone) == false)
                    {
                        charsFullPinyins[i].Add(fullPinyinNoTone);
                    }
                    else
                    {
                        continue;
                    }
                    #region 给出拼音首字母
                    if (fullPinyinNoTone.StartsWith("ZH")
                        || fullPinyinNoTone.StartsWith("CH") || fullPinyinNoTone.StartsWith("SH"))
                    {
                        //额外添加翘舌音
                        firstPinyins.Add(fullPinyinNoTone.Substring(0, 2));
                    }
                    firstPinyins.Add(fullPinyinNoTone.Substring(0, 1));
                    #endregion
                }
                #region 由于系统的函数判断的首字母的话只是ZCS这样的 所以平翘舌的时候要单独的添加ZH CH SH 到待生成的数据里面去 然后再去和
                for (int j = 0; j < firstPinyins.Count; j++)
                {
                    string currentFirstPinyin = firstPinyins[j];
                    if (charsFirstPinyins[i].Contains(firstPinyins[j]) == false)
                    {
                        charsFirstPinyins[i].Add(firstPinyins[j]);
                    }
                }
                #endregion

            }
            if (getFullPinyin)
            {
                List<string> fullPinyinResult = new List<string>();
                //MakeFullPinyinCombains(value[0], charsFullPinyins, value);
                MakeFullPinyinCombains(0, charsFullPinyins, value);
                for (int i = 0; i < fullPinyinCombainResult.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < fullPinyinCombainResult[i].Count; j++)
                    {
                        sb.Append(fullPinyinCombainResult[i][j]);
                    }
                    fullPinyinResult.Add(sb.ToString());
                }
                allResult.AddRange(fullPinyinResult);
            }
            if (getFirstPinyin)
            {
                List<string> firstPinyinResult = new List<string>();
                MakeFirstPinyinCombains(0, charsFirstPinyins, value);
                for (int i = 0; i < firstPinyinCombainResult.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < firstPinyinCombainResult[i].Count; j++)
                    {
                        sb.Append(firstPinyinCombainResult[i][j]);
                    }
                    firstPinyinResult.Add(sb.ToString());
                }
                allResult.AddRange(firstPinyinResult);
            }

            return allResult;
        }
    }
}
