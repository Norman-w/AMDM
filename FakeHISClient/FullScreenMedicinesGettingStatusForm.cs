using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using LibVLCSharp.Shared;
using System.IO;
using System.Threading;

namespace FakeHISClient
{
    public partial class FullScreenMedicinesGettingStatusForm : Form
    {
        //LibVLC vlc = new LibVLC(new string[] {});
        //MediaPlayer player;
        public FullScreenMedicinesGettingStatusForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = Screen.PrimaryScreen.Bounds.Size;

            //FileStream fs = new FileStream(Application.StartupPath + "\\1.avi", FileMode.Open);
            //MemoryStream ms = new MemoryStream();
            //fs.CopyTo(ms);
            //fs.Close();
            //Media media = new Media(vlc, ms, new string[] {});
            //media.AddOption(":hw-dec=false");
            //fs.Close();
            //this.player = new MediaPlayer(media);
            //this.player.Playing += player_Playing;
            //this.player.Stopped += player_Stopped;
            //player.Hwnd = this.panel1.Handle;
            //this.panel1.SendToBack();
            //ThreadPool.QueueUserWorkItem((res) =>
            //    {
            //        player.Play();
            //    });
            //player.Play();
        }

        private void FullScreenMedicinesGettingStatusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.player.Dispose();
            //this.vlc.Dispose();
        }
    }
}
