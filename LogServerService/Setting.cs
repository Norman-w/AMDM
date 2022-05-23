using EasyNamedPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogServerService
{
    #region 支持xml序列化的dictionary
    /// <summary>   
    /// 支持XML序列化的泛型 Dictionary   
    /// </summary>   
    /// <typeparam name="TKey"></typeparam>   
    /// <typeparam name="TValue"></typeparam>   
    [XmlRoot("SerializableDictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {

        #region 构造函数
        public SerializableDictionary()
            : base()
        {
        }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>   
        /// 从对象的 XML 表示形式生成该对象   
        /// </summary>   
        /// <param name="reader"></param>   
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /**/
        /// <summary>   
        /// 将对象转换为其 XML 表示形式   
        /// </summary>   
        /// <param name="writer"></param>   
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }
    #endregion

    #region 日志服务器设置类和相关枚举类型定义
    /// <summary>
    /// 日志服务器设置,通过序列化保存到文件.
    /// </summary>
    public class Setting
    {
        public Setting()
        {
            PerHeartbeatsValidSpanMS = 5000;
            RetryCheckValidHeartbeatsDelayMS = 1000;
            RetryLoadSettingDelayMS = 5000;
            RetryWaitFirstHeartbeatsDelayMS = 1000;
            RetryWaitFriendlyClosedDelayMS = 1000;
            RetryWaitProcessClosedDelayMS = 1000;
            StopOneAppByCloseMainWindowMethodTimeoutMS = 5000;
            WaitFirstHeartbeatsTimeout = 10000;
            WaitClientStartedTimeout = 30;//30秒就应该能把程序启动
            RetryWaitClientDelayMS = 1000;

            #region 压力测试模式
            //this.PerHeartbeatsValidSpanMS = 1;
            //this.RetryCheckValidHeartbeatsDelayMS = 1;
            //this.RetryLoadSettingDelayMS = 1;
            //this.RetryWaitClientDelayMS = 1;
            //this.RetryWaitFriendlyClosedDelayMS = 1;
            //this.RetryWaitFriendlyClosedDelayMS = 1;
            //this.RetryWaitProcessClosedDelayMS = 1;
            //this.StopOneAppByCloseMainWindowMethodTimeoutMS = 1;
            //this.WaitFirstHeartbeatsTimeout = 1;
            #endregion
        }
        /// <summary>
        /// 启动时读取此参数,确定是否正在调试中.如果是在调试中,只有在调试器注入的时候才会走正常的调试流程.
        /// </summary>
        public bool Debugging { get; set; }
        /// <summary>
        /// 日志记录的模式,是记录到文件还是记录到sql
        /// </summary>
        public LogModeEnum Mode { get; set; }
        /// <summary>
        /// 记录日志的时候,每一种日志要记录的字段都有什么.
        /// </summary>
        public SerializableDictionary<LogLevel, List<LogFieldEnum>> LogEachLevelFields { get; set; }
        /// <summary>
        /// 日志文件的保存路径(服务本身的一些错误信息,启动信息等始终记录到此文件,如果标记为把日常日志也记录到文件)
        /// </summary>
        public string LogFilePath { get; set; }
        /// <summary>
        /// 日志存储到mysql服务器的时候需要配置信息
        /// </summary>
        public class MysqlSettingClass
        {
            public string Ip { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public string Database { get; set; }
            public int Port { get; set; }
            /// <summary>
            /// 日志服务的表名
            /// </summary>
            public string LogTableName { get; set; }
        }
        /// <summary>
        /// 日志存储到mysql服务器的时候,需要配置信息
        /// </summary>
        public MysqlSettingClass MysqlSetting { get; set; }

        /// <summary>
        /// 客户端应用程序的位置(被监测的应用)
        /// </summary>
        public string ClientAppPath { get; set; }

        /// <summary>
        /// 关闭一个进程的时候,使用CloseMainWindow这种友好的方式的最大超时时间,因为程序关闭的信息保存需要时间,如果超过这个时间后,就强制关闭进程
        /// </summary>
        public int StopOneAppByCloseMainWindowMethodTimeoutMS { get; set; }

        /// <summary>
        /// 每一次心跳的有效期是多长时间毫秒.超过这个心跳基数了以后,程序就需要被处理了.
        /// </summary>
        public int PerHeartbeatsValidSpanMS { get; set; }

        /// <summary>
        /// 重新尝试获取或等待新的有效心跳的间隔时间
        /// </summary>
        public int RetryCheckValidHeartbeatsDelayMS { get; set; }

        /// <summary>
        /// 重新尝试等待进程被关闭的间隔时间(不管什么方式关闭)
        /// </summary>
        public int RetryWaitProcessClosedDelayMS { get; set; }

        /// <summary>
        /// 重新尝试等待友好的关闭的结果的间隔时间
        /// </summary>
        public int RetryWaitFriendlyClosedDelayMS { get; set; }

        /// <summary>
        /// 重新尝试等待第一次心跳到来的间隔时间
        /// </summary>
        public int RetryWaitFirstHeartbeatsDelayMS { get; set; }

        /// <summary>
        /// 重新尝试等待客户端进程被启动的间隔时间(启动命令发送后)
        /// </summary>
        public int RetryWaitClientDelayMS { get; set; }

        /// <summary>
        /// 重新信尝试加载设置时间的间隔时间(如果配置不正确,每隔多久检测一次新的配置)
        /// </summary>
        public int RetryLoadSettingDelayMS { get; set; }

        /// <summary>
        /// 检测到进程启动后,等待第一次心跳的超时时间
        /// </summary>
        public int WaitFirstHeartbeatsTimeout { get; set; }

        /// <summary>
        /// 启动程序到获取到程序进程的超时时间是多久
        /// </summary>
        public int WaitClientStartedTimeout { get; set; }
    }
    #region 配置枚举定义
    public enum LogModeEnum
    {
        /// <summary>
        /// 记录日志到文件
        /// </summary>
        Log2File,
        /// <summary>
        /// 记录日志到服务器
        /// </summary>
        Log2Mysql,
    }
    /// <summary>
    /// 日志记录的等级.应当使用一个日志服务器将要记录的日志类型列表来保存.
    /// </summary>
    //public enum LogLevelEnum
    //{
    //    /// <summary>
    //    /// 日常信息
    //    /// </summary>
    //    Info,
    //    /// <summary>
    //    /// 警告
    //    /// </summary>
    //    Warnning,
    //    /// <summary>
    //    /// 错误
    //    /// </summary>
    //    Error,
    //    /// <summary>
    //    /// 程序bug
    //    /// </summary>
    //    Bug,
    //}
    /// <summary>
    /// 记录日志的时候,具体要记录哪些字段.这将能决定日志文件的大小和记录速度.通常记录到文件的时候才需要配置,否则应当为全部记录,应当用一个list保存.通过检索list中有没有相关的字段来确定是否要记录.
    /// </summary>
    public enum LogFieldEnum
    {
        /// <summary>
        /// 记录时间
        /// </summary>
        Time,
        /// <summary>
        /// 记录日志的级别(类型)int形态
        /// </summary>
        Level,
        /// <summary>
        /// 记录日志的级别(类型)string形态
        /// </summary>
        LevelName,
        /// <summary>
        /// 记录标题
        /// </summary>
        Title,
        /// <summary>
        /// 记录消息
        /// </summary>
        Message,
        /// <summary>
        /// 记录传入的字段集合
        /// </summary>
        Params,
    }
    #endregion
    #endregion

    #region 日志数据结构类
    /// <summary>
    /// 一条日志记录
    /// </summary>
    public class LogRecord
    {
        public string Title { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 流水记录
        /// </summary>
        public long Id { get; set; }

        public LogLevel Level { get; set; }
        /// <summary>
        /// 保存到数据库方便看的字段.Level和LevelName设置一个就可以的.
        /// </summary>
        public LogLevel LevelName { get; set; }
        /// <summary>
        /// 日志产生时间,不是记录时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 日志要记录的参数集,可以是保存的任何形式的string,推荐使用json形式
        /// </summary>
        public string Params { get; set; }
    }
    #endregion

    #region 客户端状态枚举
    /// <summary>
    /// 客户端应用程序的状态,根据状态判断该执行的操作.
    /// </summary>
    enum ClientAppStatusEnum
    {
        /// <summary>
        /// 根据配置文件中的客户端程序路径没有找到客户端程序
        /// </summary>
        NotFound,
        /// <summary>
        /// 正在启动客户端应用程序
        /// </summary>
        Startting,
        /// <summary>
        /// 客户端程序已经启动(但是不一定是连接上了心跳,也就是可能客户端正在初始化中)
        /// </summary>
        Started,
        /// <summary>
        /// 已经通过管道连接到了服务
        /// </summary>
        Connected,
        /// <summary>
        /// 客户端已经完成初始化并且正式运行中(接收到了他的心跳)
        /// </summary>
        Running,
        /// <summary>
        /// 断开了管道服务器的连接,那也需要重新启动了.
        /// </summary>
        Disconnected,
        /// <summary>
        /// 本看门狗正在停止客户端应用程序
        /// </summary>
        Stopping,
        /// <summary>
        /// 应用程序已经关闭(被本看门狗关闭)
        /// </summary>
        Stoped,
    }
    #endregion
}
