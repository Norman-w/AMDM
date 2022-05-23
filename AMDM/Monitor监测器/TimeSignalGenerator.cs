using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



/*
 * 时间信号产生器应该只产生和管理时间信号.产生信号以后调用相关的处理器进行处理即可.
 * 如果外部想要查看时间信号产生器产生的信号的记录等情况的时候.可以访问他的公共变量.
 */
namespace AMDM
{
    internal class TimeSignalGenerator
    {
        public class TimerInfo
        {
            public Action CallbackAction { get; set; }
            public Timer Timer { get; set; }
            public long DelayTimeMs { get; set; }
            public Type OwnerType { get; set; }
            public bool OnOff { get; set; }
        }
        //Action应该包含当前timer执行的了第几次,(可能不行 因为数字可能太大了) 上次执行时间 下次执行时间等参数 具体的想一下
        Dictionary<Type, TimerInfo> timersDic = new Dictionary<Type, TimerInfo>();
        public bool RegisterIntervalAction(Type ownerType, long delayTimeMS, Action action)
        {
            if (ownerType == null || this.timersDic.ContainsKey(ownerType) || delayTimeMS<10 || delayTimeMS> 1000*3600*24 || action == null)
            {
                return false;
            }
            else 
            {
                TimerCallback cb = new TimerCallback(this.onTimer);
                Timer newTimer = new Timer(cb, ownerType, 0, delayTimeMS);
                var info = new TimerInfo()
                {
                    Timer = newTimer,
                    CallbackAction = action,
                    DelayTimeMs = delayTimeMS,
                    OwnerType = ownerType,
                    OnOff = true
                };
                timersDic.Add(ownerType, info);
                return true;
            }
        }
        public bool UnRegisterIntervalAction(Type ownerType)
        {
            if (ownerType == null || this.timersDic.ContainsKey(ownerType) == false)
            {
                return false;
            }
            var info = this.timersDic[ownerType];
            info.OnOff = false;
            info.Timer.Dispose();
            info.Timer = null;
            this.timersDic.Remove(ownerType);
            return true;
        }

        void onTimer(object ownerTypeObj)
        {
            Type ownerType = ownerTypeObj as Type;
            if (this.timersDic.ContainsKey(ownerType))
            {
                var info = this.timersDic[ownerType];
                if (info.OnOff == false)
                {
                    return;
                }
                info.CallbackAction();
            }
        }
    }
}
