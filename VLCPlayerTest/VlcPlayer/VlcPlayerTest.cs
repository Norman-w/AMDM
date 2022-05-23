using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using VlcPlayer;
using LibVLCSharp.Shared;
using System.IO;

namespace AMDM.VlcPlayer
{
    public partial class VlcPlayerTest : Form
    {
        //VlcPlayerBase player;
        //LibVLC vlc = new LibVLC(new string[]{"--plugin-path="+@"D:\Visual Studio 2008\Include\VLC\NET40"});
        LibVLC vlc = null;
        LibVLCSharp.Shared.MediaPlayer player = null;
        public VlcPlayerTest()
        {
            InitializeComponent();
            //player = new VlcPlayerBase(Application.StartupPath + "\\plugins");
            ////player = new VlcPlayerBase("D:\\VLC_3.0.15\\VLC媒体播放器 x64\\plugins");
            //player.SetRenderWindow(this.panel1.Handle.ToInt32());
            //vlc = new LibVLC(new string[] { "-I", "dummy", "--ignore-config", "--no-video-title", "--plugin-path=D:\\" });
            vlc = new LibVLC(new string[] { });
            player = new LibVLCSharp.Shared.MediaPlayer(vlc);
        }

        private void VlcPlayerTest_Load(object sender, EventArgs e)
        {
            //player.LoadFile("\\4.mp4");
            //player.Play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            player.Hwnd = this.panel1.Handle;
            FileStream stream = System.IO.File.Open(Application.StartupPath + "\\4.mp4", System.IO.FileMode.Open);
            Media media = new Media(vlc, stream, new string[] { });
            player.Play(media);
        }
    }
}
