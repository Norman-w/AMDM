#region 引用
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
#endregion
#region 主体
/// <summary>
/// dll动态加载器
/// </summary>
static class DllLoader
{
    #region 变量
    static Dictionary<string, Assembly> DllsDic = new Dictionary<string, Assembly>();
    static Dictionary<string, object> AssembliesDic = new Dictionary<string, object>();
    #endregion
    #region 公共函数,外部调用
    /// <summary>
    /// 加载
    /// </summary>
    public static void Load()
    {
        Console.WriteLine("注册dll方法调用");
        //获取Program所属程序集
        var ass = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
        //判断是否已处理
        if (AssembliesDic.ContainsKey(ass.FullName))
        {
            return;
        }
        //程序集加入已处理集合
        AssembliesDic.Add(ass.FullName, null);
        //绑定程序集加载失败事件(这里我测试了,就算重复绑也是没关系的)
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        //获取所有资源文件文件名
        var res = ass.GetManifestResourceNames();
        foreach (var r in res)
        {
            if (r.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    #region 实际应用中,不把dll解开放到某个地方 也是一样可以用的.具体根据自己的情况确定是否要解开注释内容
                    //if (r.ToLower().Contains(".dll"))
                    //{
                    //    ExtractResource2File(r, GetUnpackPatch() + @"/" + r.Substring(r.IndexOf('.') + 1));
                    //}
                    #endregion
                    var s = ass.GetManifestResourceStream(r);
                    var bts = new byte[s.Length];
                    s.Read(bts, 0, (int)s.Length);
                    var da = Assembly.Load(bts);
                    //判断加载结果
                    if (DllsDic.ContainsKey(da.FullName))
                    {
                        continue;
                    }
                    DllsDic[da.FullName] = da;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }
            }
        }
    }
    /// <summary>
    /// 解包文件
    /// </summary>
    /// <param name="resourceName">资源名</param>
    /// <param name="filename">目标文件名</param>
    #endregion
    #region 私有函数,内部使用
    static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
    {
        //程序集
        Assembly ass;
        //获取加载失败的程序集的全名
        var assName = new AssemblyName(args.Name).FullName;
        //判断Dlls集合中是否有已加载的同名程序集
        if (DllsDic.TryGetValue(assName, out ass) && ass != null)
        {
            DllsDic[assName] = null;//如果有则置空并返回
            return ass;
        }
        else
        {
            throw new DllNotFoundException(assName);//否则抛出加载失败的异常
        }
    }

    static void ExtractResource2File(string resourceName, string filename)
    {
        Console.WriteLine("解压文件:{0} {1}", resourceName, filename);
        if (!System.IO.File.Exists(filename))
        {
            using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, b.Length);
                    fs.Write(b, 0, b.Length);
                }
            }
        }
    }
    /// <summary>
    /// 获取解压目录,根据自己的需求改位置,如果解压到系统文件夹,需要有相关权限,可能会被杀毒软件误报
    /// </summary>
    /// <returns></returns>
    static string GetUnpackPatch()
    {
        //string strPath = "C:";
        string strPath = AppDomain.CurrentDomain.BaseDirectory;
        if (!Directory.Exists(strPath))
        {
            Directory.CreateDirectory(strPath);
        }
        return strPath;
    }
    #endregion
}
#endregion
