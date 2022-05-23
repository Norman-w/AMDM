using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FakeHISServer
{
    public delegate string OnReciveMessageEventHandler(string msg, object requestRef, object responseRef);
    public class HTTPServer
    {
        HttpListener httpobj;
        public void Start(string address)
        {
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            httpobj = new HttpListener();
            //定义url及端口号，通常设置为配置文件
            httpobj.Prefixes.Add(address);
            //启动监听器
            httpobj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpobj.BeginGetContext(Result, null);
            Console.WriteLine("服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
            //Console.ReadKey();
        }

        public void Stop()
        {
            httpobj.Close();
            //httpobj.Prefixes.Clear();
            Console.WriteLine("http服务已停止");
        }


        private void Result(IAsyncResult ar)
        {
            //当接收到请求后程序流会走到这里
            if (httpobj.IsListening == false)
            {
                Console.WriteLine("已不再连接状态时接收到请求");
                return;
            }

            //继续异步监听
            httpobj.BeginGetContext(Result, null);
            var guid = Guid.NewGuid().ToString();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("接到新的请求:{guid},时间：{DateTime.Now.ToString()}");
            //获得context对象
            var context = httpobj.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            ////如果是js的ajax请求，还可以设置跨域的ip地址与参数
            //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件
            var requestEncoding = context.Request.ContentEncoding;

            #region 显示请求的encoding
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("HTTP Server Result 收到请求的编码格式是:{0}", context.Request.ContentEncoding.EncodingName);
            Console.ResetColor();
            #endregion

            //context.Response.ContentType = string.Format("application/json;charset={0}", requestEncoding.ToString());// "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            //context.Response.AddHeader("Content-type", "application/json");//添加响应头信息
            //context.Response.ContentEncoding = requestEncoding;// Encoding.UTF8;

            
            string returnObj = null;//定义返回客户端的信息
            List<string> allowMethod = new List<string>();
            allowMethod.Add("POST");
            allowMethod.Add("GET");
            //if (request.HttpMethod == "POST" && request.InputStream != null)
            if (allowMethod.Contains(request.HttpMethod) && request.InputStream != null)
            {
                //处理客户端发送的请求并返回处理信息
                returnObj = HandleRequest(request, response);
            }
            else
            {
                Dictionary<string, object> rspParam = new Dictionary<string, object>();
                rspParam.Add("Success", false);
                rspParam.Add("ErrMsg", "invalid method. should be POST only");
                rspParam.Add("ErrCode", 403);
                returnObj = JsonConvert.SerializeObject(rspParam);
                //returnObj = "这里需要实现post函数";
            }
            response.ContentEncoding = Encoding.UTF8;
            var returnByteArr =returnObj == null? null: Encoding.UTF8.GetBytes(returnObj);// Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("网络崩了：{0}", ex.ToString());
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("请求处理完成：{guid},时间：{ DateTime.Now.ToString()}\r\n");
        }

        private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string data = null;
            try
            {
                if (request.HttpMethod == "POST")
                {
                    var byteList = new List<byte>();
                    var byteArr = new byte[2048];
                    int readLen = 0;
                    int len = 0;
                    //接收客户端传过来的数据并转成字符串类型
                    do
                    {
                        readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                        len += readLen;
                        byteList.AddRange(byteArr);
                    } while (readLen != 0);
                    data = request.ContentEncoding.GetString(byteList.ToArray(), 0, len);// Encoding.GetEncoding("GBK").GetString(byteList.ToArray(),0, len);

                    //获取得到数据data可以进行其他操作

                    if (this.OnReciveMessage != null)
                    {
                        return this.OnReciveMessage(data, request, response);
                    }
                }
                else if(request.HttpMethod == "GET")
                {
                    if (this.OnReciveMessage != null)
                    {
                        return this.OnReciveMessage(null, request, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                Console.ForegroundColor = ConsoleColor.Red;
                string errMsg = string.Format("在接收数据时发生错误:{0}", ex.ToString());
                Console.WriteLine(errMsg);
                return errMsg;//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
            }
            response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
            response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
            Console.ForegroundColor = ConsoleColor.Green;
            string successLog = string.Format("接收数据完成,未进行逻辑处理:{0},时间：{1}", data.Trim(), DateTime.Now.ToString());
            Console.WriteLine(successLog);
            //return "got some request";
            return successLog;
        }

        public event OnReciveMessageEventHandler OnReciveMessage;
    }
}
