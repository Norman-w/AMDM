using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
/*
 * 2021年11月24日10:41:59  由于speechlib是从.net2.0里面提取出来的,在win7环境使用的时候 如果连续播放20次或者更多的音频,然后再次播放的时候就会抛出
 * msvcrt.dll的0xc0000005的异常,但是在win10中测试了没有问题.
 * 另外如果是使用动态调用组件使用 sapi.spvoice的方式播放也会出现同样的问题.
 * 为了程序的稳定性,改成使用.net3.5以后程序集中包含的system.speech的程序集的方式来使用语音朗读引擎.
 */
namespace AMDM.Manager
{
    /// <summary>
    /// TTS语音朗读引擎播放器
    /// </summary>
    public class TTSSpeaker
    {
        #region 全局变量
        /// <summary>
        /// 播放器,如果是异步播放的时候,每次重新启动播放都需要把这个speaker dispose以后再重新new,然后再播放文本
        /// </summary>
        SpeechSynthesizer speaker = new SpeechSynthesizer();
        #endregion

        #region 原来的方法
        //SpObjectToken 要使用的语言 = null;

        bool noMsbBox = true;
        InstalledVoice spvoice = null;
        bool 已经安装了语音朗读引擎支持库 = true;
        /// <summary>
        /// 设置要使用的语音
        /// </summary>
        /// <param name="voiceName"></param>
        /// <returns></returns>
        public bool SetVoice(string voiceName)
        {
            if (voiceName == null)
            {
                Utils.LogWarnning("给定的语音朗读引擎播放名称为空");
                return false;
            }
            bool ret = false;
            if (this.GetVoices().Contains(voiceName))
            {
                this.speaker.SelectVoice(voiceName);
            }
            return ret;
        }

        /// <summary>
        /// 获取当前已经设置了的语音朗读引擎名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSelectedVoice()
        {
            if (this.spvoice!= null && this.spvoice.VoiceInfo!= null)
            {
                return this.spvoice.VoiceInfo.Name;
            }
            return null;
        }
        /// <summary>
        /// 获取已经安装的所有语音
        /// </summary>
        /// <returns></returns>
        public List<string> GetVoices()
        {
            var voices = speaker.GetInstalledVoices();
            List<string> ret = new List<string>();
            foreach (var token in voices)
            {
                ret.Add(token.VoiceInfo.Name);
            }
            return ret;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="voiceName"></param>
        /// <param name="noMsbBox"></param>
        public TTSSpeaker(string voiceName, bool noMsbBox, bool saveInstalledVoiceInfoFile = false)
        {
            this.noMsbBox = noMsbBox;
            this.speaker = new SpeechSynthesizer();
            var voices = speaker.GetInstalledVoices();
            //MessageBox.Show("2");
            string alltokenstr = "";
            Dictionary<string, InstalledVoice> tokens = new Dictionary<string, InstalledVoice>();
            foreach (var token in voices)
            {
                alltokenstr += token.VoiceInfo.Name + "\r\n";
                tokens.Add(token.VoiceInfo.Name, token);
                if (voiceName != null)
                {
                    if (token.VoiceInfo.Name.Contains(voiceName))
                    {
                        spvoice = token;
                        //要使用的语言 = token;
                    }
                }
            }
            if (saveInstalledVoiceInfoFile)
            {
                System.IO.File.WriteAllText("此计算机已安装的语音库.txt", alltokenstr);
            }
            while (true && noMsbBox == false)
            {
                if (spvoice == null)
                {
                    System.Windows.Forms.MessageBox.Show("未找到语言包" + voiceName + "请重新设置语言或安装此语言包");
                    List<string> names = new List<string>(tokens.Keys);
                    List<string[]> values = new List<string[]>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        values.Add(new string[] { names[i] });
                    }
                    MyCode.Forms.选择 selDlg = new MyCode.Forms.选择("请选择将使用的语音朗读语言", new string[] { "语言名" }, new int[] { 465 }, values);
                    if (System.Windows.Forms.DialogResult.OK == selDlg.ShowDialog())
                    {
                        if (selDlg.SelectedItems.Count == 1)
                        {
                            string selected = selDlg.SelectedItems[0].SubItems[0].Text;
                            if (tokens.ContainsKey(selected))
                            {
                                spvoice = tokens[selected];
                                Utils.LogSuccess("已设置语音朗读引擎使用的音频为:", spvoice.VoiceInfo.Name);
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            this.speaker.SelectVoice(spvoice.VoiceInfo.Name);
        }
        /// <summary>
        /// 异步方式播放文字
        /// </summary>
        /// <param name="text"></param>
        public void Speak(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (spvoice == null) return;
            if (已经安装了语音朗读引擎支持库 == false)
            {
                System.Console.WriteLine(string.Format("没有安装语音朗读引擎支持库,但是你想说:\r\n{0}", text));
                return;
            }
            try
            {
                #region 之前在网上找的是 每次重新播放的时候直接重新初始化一个speaker,
                //这样做的话是速度有点拖延,而且,如果使用的语音包跟之前不一样的话,每次selectvoice的时候还会耽误更多的时间造成页面卡顿.
                //后来看speaker中包含这种cancel的方法.那好了就直接用这种了.经过测试还是没有问题的,反应速度快
                //2021年12月1日09:31:12  然后我又做了一个测试,如果连续播放多次,然后再在别的线程中启动播放,不重新new这个speaker对象的话,会出问题.所以
                //如果speaker是在同一个线程内调用的说话函数,是没有问题的,不然就会出问题.
                if (this.speaker != null)
                {
                    speaker.Dispose();
                }
                speaker = new SpeechSynthesizer();
                if (this.spvoice != null && this.spvoice.VoiceInfo.Id != this.spvoice.VoiceInfo.Id)
                {
                    this.speaker.SelectVoice(spvoice.VoiceInfo.Name);
                }
                #endregion
                #region 这是新方法,每次播放语音之前把之前的关闭掉就可以了.2021年12月1日09:22:26,但是在不同线程中连续调用的时候可能还是会出问题.所以还是启用之前的方法
                //if (this.speaker != null)
                //{
                //    this.speaker.SpeakAsyncCancelAll();
                //}
                #endregion
                this.speaker.SpeakAsync(text);
            }
            catch (Exception e)
            {
                Utils.LogError("使用语音朗读引擎播放文字时发生错误:", e.Message, e.StackTrace);
            }
        }
        /// <summary>
        /// 播放文字,默认使用异步方式
        /// </summary>
        /// <param name="text"></param>
        /// <param name="async"></param>
        public void Speak(string text, bool async = true)
        {
            if (async)
            {
                this.Speak(text);
            }
            else
            {
                try
                {
                    if (this.speaker != null)
                    {
                        this.speaker.SpeakAsyncCancelAll();
                        this.speaker = new SpeechSynthesizer();
                        if (this.spvoice != null && this.spvoice.VoiceInfo.Id != this.spvoice.VoiceInfo.Id)
                        {
                            this.speaker.SelectVoice(spvoice.VoiceInfo.Name);
                        }
                    }
                    this.speaker.Volume = 100;
                    this.speaker.Speak(text);
                }
                catch (Exception e)
                {
                    Utils.LogError("使用同步播放文本转语音时发生错误:", e.Message, e.StackTrace);
                }
            }
        }
        /// <summary>
        /// 销毁对象
        /// </summary>
        public void Dispose()
        {
            if (this.speaker != null)
            {
                this.speaker.Dispose();
            }
            spvoice = null;
        }
        #endregion
    }
}
