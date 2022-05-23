//using System;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;

//using DirectShowLib;

//using Microsoft.Win32;
//using System.Diagnostics;
//using System.IO;
///*
// * 2021-11-21 22-35 该文件依赖directshow 2005那个文件 性能非常高
// * 
// */

//namespace DirectShowLib.Sample
//{
//    public class TestWindowLessCameraCaputer : System.Windows.Forms.Form, ISampleGrabberCB, IDisposable
//    {
//        private System.Windows.Forms.MainMenu mainMenu;
//        private System.Windows.Forms.OpenFileDialog openFileDialog;
//        private System.Windows.Forms.MenuItem menuFile;
//        private System.Windows.Forms.MenuItem menuItem1;
//        private System.Windows.Forms.MenuItem menuFileExit;
//        private IContainer components;

//        // Used to snap picture on Still pin
//        private IAMVideoControl m_VidControl = null;
//        private IPin pPreviewOut = null;
//        private IPin pPreviewOutStill = null;
//        private int m_videoWidth;
//        private int m_videoHeight;
//        private int m_stride;

//#if DEBUG
//        // Allow you to "Connect to remote graph" from GraphEdit
//        DsROTEntry m_rot = null;
//#endif
//        // DirectShow stuff
//        private IFilterGraph2 graphBuilder = null;
//        private IMediaControl mediaControl = null;
//        private IBaseFilter vmr9 = null;
//        private IBaseFilter vmr9_2 = null;
//        private IBaseFilter vmr9_3 = null;
//        private IBaseFilter vmr9_4 = null;

//        private IVMRWindowlessControl9 windowlessCtrl = null;
//        private IVMRWindowlessControl9 windowlessCtrl_2 = null;
//        private IVMRWindowlessControl9 windowlessCtrl_3 = null;
//        private IVMRWindowlessControl9 windowlessCtrl_4 = null;

//        private SaveFileDialog saveFileDialog;
//        private Panel panel1; // Needed to remove delegates
//        private Panel panel2;
//        private Panel panel3;
//        private Panel panel4;

//        private IntPtr m_ipBuffer = IntPtr.Zero;
//        public delegate void Changepicturebox(int num, bool visible, Bitmap bitmap1); //picbox
//        public delegate void Changeimagebox(int num, bool visible);   //imagebox
//        public Changepicturebox changepicbox1;
//        public Changeimagebox changeimgbox1;

//        string file_name_image = null;
//        int count_pic = 0;

//        // Menus stuff
//        private MenuItem menuSnap;

//        public TestWindowLessCameraCaputer()
//        {
//            InitializeComponent();
//            changepicbox1 = FunChangepicture;
//        }

//        /// <summary>
//        /// Nettoyage des ressources utilis閑s.
//        /// </summary>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (components != null)
//                {
//                    components.Dispose();
//                }
//            }
//            // Clean-up DirectShow interfaces
//            CloseInterfaces();
//            base.Dispose(disposing);
//        }

//        #region Code g閚閞?par le Concepteur Windows Form
//        /// <summary>
//        /// M閠hode requise pour la prise en charge du concepteur - ne modifiez pas
//        /// le contenu de cette m閠hode avec l'閐iteur de code.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestWindowLessCameraCaputer));
//            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
//            this.menuFile = new System.Windows.Forms.MenuItem();
//            this.menuSnap = new System.Windows.Forms.MenuItem();
//            this.menuItem1 = new System.Windows.Forms.MenuItem();
//            this.menuFileExit = new System.Windows.Forms.MenuItem();
//            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
//            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
//            this.panel1 = new System.Windows.Forms.Panel();
//            this.panel2 = new System.Windows.Forms.Panel();
//            this.panel3 = new System.Windows.Forms.Panel();
//            this.panel4 = new System.Windows.Forms.Panel();
//            this.SuspendLayout();
//            // 
//            // mainMenu
//            // 
//            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
//            this.menuFile});
//            // 
//            // menuFile
//            // 
//            this.menuFile.Index = 0;
//            this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
//            this.menuSnap,
//            this.menuItem1,
//            this.menuFileExit});
//            this.menuFile.Text = "File";
//            // 
//            // menuSnap
//            // 
//            this.menuSnap.Index = 0;
//            this.menuSnap.Text = "Save image...";
//            this.menuSnap.Click += new System.EventHandler(this.menuSnap_Click);
//            // 
//            // menuItem1
//            // 
//            this.menuItem1.Index = 1;
//            this.menuItem1.Text = "-";
//            // 
//            // menuFileExit
//            // 
//            this.menuFileExit.Index = 2;
//            this.menuFileExit.Text = "Exit";
//            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
//            // 
//            // openFileDialog
//            // 
//            //this.openFileDialog.Filter = resources.GetString("openFileDialog.Filter");
//            // 
//            // panel1
//            // 
//            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
//            this.panel1.Location = new System.Drawing.Point(3, 3);
//            this.panel1.Name = "panel1";
//            this.panel1.Size = new System.Drawing.Size(639, 475);
//            this.panel1.TabIndex = 1;
//            // 
//            // panel2
//            // 
//            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
//            this.panel2.Location = new System.Drawing.Point(654, 3);
//            this.panel2.Name = "panel2";
//            this.panel2.Size = new System.Drawing.Size(639, 475);
//            this.panel2.TabIndex = 1;
//            // 
//            // panel3
//            // 
//            this.panel3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
//            this.panel3.Location = new System.Drawing.Point(3, 493);
//            this.panel3.Name = "panel3";
//            this.panel3.Size = new System.Drawing.Size(639, 475);
//            this.panel3.TabIndex = 2;
//            // 
//            // panel4
//            // 
//            this.panel4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
//            this.panel4.Location = new System.Drawing.Point(654, 493);
//            this.panel4.Name = "panel4";
//            this.panel4.Size = new System.Drawing.Size(639, 475);
//            this.panel4.TabIndex = 3;
//            // 
//            // MainForm
//            // 
//            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
//            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
//            this.ClientSize = new System.Drawing.Size(1305, 980);
//            this.Controls.Add(this.panel2);
//            this.Controls.Add(this.panel1);
//            this.Controls.Add(this.panel4);
//            this.Controls.Add(this.panel3);
//            this.Menu = this.mainMenu;
//            this.Name = "MainForm";
//            this.Text = "VMR Snapper";
//            this.Load += new System.EventHandler(this.MainForm_Load);
//            this.ResumeLayout(false);

//        }
//        #endregion
//        private void CloseInterfaces()
//        {
//            if (mediaControl != null)
//                mediaControl.Stop();

//            if (vmr9 != null)
//            {
//                Marshal.ReleaseComObject(vmr9);
//                vmr9 = null;
//                windowlessCtrl = null;
//            }
//            if (vmr9_2 != null)
//            {
//                Marshal.ReleaseComObject(vmr9_2);
//                vmr9_2 = null;
//                windowlessCtrl_2 = null;
//            }

//            if (graphBuilder != null)
//            {
//                Marshal.ReleaseComObject(graphBuilder);
//                graphBuilder = null;
//                mediaControl = null;
//            }

//        }

//        private void ConfigureVMR9InWindowlessMode()
//        {
//            int hr = 0;

//            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)vmr9;

//            // Not really needed for VMR9 but don't forget calling it with VMR7
//            hr = filterConfig.SetNumberOfStreams(1);
//            DsError.ThrowExceptionForHR(hr);

//            // Change VMR9 mode to Windowless
//            hr = filterConfig.SetRenderingMode(VMR9Mode.Windowless);
//            DsError.ThrowExceptionForHR(hr);

//            windowlessCtrl = (IVMRWindowlessControl9)vmr9;

//            // Set "Parent" window
//            hr = windowlessCtrl.SetVideoClippingWindow(this.panel1.Handle);
//            DsError.ThrowExceptionForHR(hr);

//            // Set Aspect-Ratio
//            hr = windowlessCtrl.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);
//            DsError.ThrowExceptionForHR(hr);

//            if (windowlessCtrl != null)
//            {
//                hr = windowlessCtrl.SetVideoPosition(null, DsRect.FromRectangle(this.panel1.ClientRectangle));
//            }
//        }

//        private void ConfigureVMR9InWindowlessMode_2()
//        {
//            int hr = 0;

//            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)vmr9_2;

//            // Not really needed for VMR9 but don't forget calling it with VMR7
//            hr = filterConfig.SetNumberOfStreams(1);
//            DsError.ThrowExceptionForHR(hr);

//            // Change VMR9 mode to Windowless
//            hr = filterConfig.SetRenderingMode(VMR9Mode.Windowless);
//            DsError.ThrowExceptionForHR(hr);

//            windowlessCtrl_2 = (IVMRWindowlessControl9)vmr9_2;

//            // Set "Parent" window
//            hr = windowlessCtrl_2.SetVideoClippingWindow(this.panel2.Handle);
//            DsError.ThrowExceptionForHR(hr);

//            // Set Aspect-Ratio
//            hr = windowlessCtrl_2.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);
//            DsError.ThrowExceptionForHR(hr);

//            if (windowlessCtrl_2 != null)
//            {
//                hr = windowlessCtrl_2.SetVideoPosition(null, DsRect.FromRectangle(this.panel2.ClientRectangle));
//            }
//        }

//        private void ConfigureVMR9InWindowlessMode_3()
//        {
//            int hr = 0;

//            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)vmr9_3;

//            // Not really needed for VMR9 but don't forget calling it with VMR7
//            hr = filterConfig.SetNumberOfStreams(1);
//            DsError.ThrowExceptionForHR(hr);

//            // Change VMR9 mode to Windowless
//            hr = filterConfig.SetRenderingMode(VMR9Mode.Windowless);
//            DsError.ThrowExceptionForHR(hr);

//            windowlessCtrl_3 = (IVMRWindowlessControl9)vmr9_3;

//            // Set "Parent" window
//            hr = windowlessCtrl_3.SetVideoClippingWindow(this.panel3.Handle);
//            DsError.ThrowExceptionForHR(hr);

//            // Set Aspect-Ratio
//            hr = windowlessCtrl_3.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);
//            DsError.ThrowExceptionForHR(hr);

//            if (windowlessCtrl_3 != null)
//            {
//                hr = windowlessCtrl_3.SetVideoPosition(null, DsRect.FromRectangle(this.panel3.ClientRectangle));
//            }
//        }

//        private void ConfigureVMR9InWindowlessMode_4()
//        {
//            int hr = 0;

//            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)vmr9_4;

//            // Not really needed for VMR9 but don't forget calling it with VMR7
//            hr = filterConfig.SetNumberOfStreams(1);
//            DsError.ThrowExceptionForHR(hr);

//            // Change VMR9 mode to Windowless
//            hr = filterConfig.SetRenderingMode(VMR9Mode.Windowless);
//            DsError.ThrowExceptionForHR(hr);

//            windowlessCtrl_4 = (IVMRWindowlessControl9)vmr9_4;

//            // Set "Parent" window
//            hr = windowlessCtrl_4.SetVideoClippingWindow(this.panel4.Handle);
//            DsError.ThrowExceptionForHR(hr);

//            // Set Aspect-Ratio
//            hr = windowlessCtrl_4.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);
//            DsError.ThrowExceptionForHR(hr);

//            if (windowlessCtrl_4 != null)
//            {
//                hr = windowlessCtrl_4.SetVideoPosition(null, DsRect.FromRectangle(this.panel4.ClientRectangle));
//            }
//        }

//        private void menuFileExit_Click(object sender, System.EventArgs e)
//        {
//            CloseInterfaces();
//            this.Dispose();
//        }

//        //[STAThread]
//        //static void Main()
//        //{
//        //    using (TestWindowLessCameraCaputer form = new TestWindowLessCameraCaputer())
//        //    {
//        //        Application.Run(form);
//        //    }
//        //}

//        private void menuSnap_Click(object sender, EventArgs e)
//        {
//            count_pic++;
//            snapImage();
//            panel3.BackgroundImage = Image.FromFile(file_name_image);
//            panel4.BackgroundImage = Image.FromFile(file_name_image);
//        }

//        // Set the Framerate, and video size
//        private void SetConfigParms(IPin pStill, int iWidth, int iHeight, short iBPP)
//        {
//            int hr;
//            AMMediaType media;
//            VideoInfoHeader v;
//            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;
//            // Get the existing format block
//            hr = videoStreamConfig.GetFormat(out media);
//            DsError.ThrowExceptionForHR(hr);
//            try
//            {
//                // copy out the videoinfoheader
//                v = new VideoInfoHeader();
//                Marshal.PtrToStructure(media.formatPtr, v);
//                // if overriding the width, set the width
//                if (iWidth > 0)
//                {
//                    v.BmiHeader.Width = iWidth;
//                }
//                // if overriding the Height, set the Height
//                if (iHeight > 0)
//                {
//                    v.BmiHeader.Height = iHeight;
//                }
//                // if overriding the bits per pixel
//                if (iBPP > 0)
//                {
//                    v.BmiHeader.BitCount = iBPP;
//                }
//                // Copy the media structure back
//                Marshal.StructureToPtr(v, media.formatPtr, false);
//                // Set the new format
//                hr = videoStreamConfig.SetFormat(media);
//                DsError.ThrowExceptionForHR(hr);
//            }
//            finally
//            {
//                DsUtils.FreeAMMediaType(media);
//                media = null;
//            }
//        }

//        private void SetConfigParmsStill(IPin pStill, int iWidth, int iHeight, short iBPP)
//        {
//            int hr;
//            AMMediaType media;
//            VideoInfoHeader v;
//            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;
//            // Get the existing format block
//            hr = videoStreamConfig.GetFormat(out media);
//            DsError.ThrowExceptionForHR(hr);
//            try
//            {
//                // copy out the videoinfoheader
//                v = new VideoInfoHeader();
//                Marshal.PtrToStructure(media.formatPtr, v);
//                // if overriding the width, set the width
//                if (iWidth > 0)
//                {
//                    v.BmiHeader.Width = iWidth;
//                }
//                // if overriding the Height, set the Height
//                if (iHeight > 0)
//                {
//                    v.BmiHeader.Height = iHeight;
//                }
//                // if overriding the bits per pixel
//                if (iBPP > 0)
//                {
//                    v.BmiHeader.BitCount = iBPP;
//                }
//                // Copy the media structure back
//                Marshal.StructureToPtr(v, media.formatPtr, false);
//                // Set the new format
//                hr = videoStreamConfig.SetFormat(media);
//                DsError.ThrowExceptionForHR(hr);
//            }
//            finally
//            {
//                DsUtils.FreeAMMediaType(media);
//                media = null;
//            }
//        }
//        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
//        {
//            int hr;
//            AMMediaType media = new AMMediaType();

//            // Set the media type to Video/RBG24
//            media.majorType = MediaType.Video;
//            media.subType = MediaSubType.RGB24;
//            media.formatType = FormatType.VideoInfo;
//            hr = sampGrabber.SetMediaType(media);
//            DsError.ThrowExceptionForHR(hr);

//            DsUtils.FreeAMMediaType(media);
//            media = null;

//            // Configure the samplegrabber
//            hr = sampGrabber.SetCallback(this, 1);
//            DsError.ThrowExceptionForHR(hr);
//        }
//        private void ConfigureSampleGrabberStill(ISampleGrabber sampGrabberStill)
//        {
//            int hr;
//            AMMediaType media = new AMMediaType();

//            // Set the media type to Video/RBG24
//            media.majorType = MediaType.Video;
//            media.subType = MediaSubType.RGB24;
//            media.formatType = FormatType.VideoInfo;
//            hr = sampGrabberStill.SetMediaType(media);
//            DsError.ThrowExceptionForHR(hr);

//            DsUtils.FreeAMMediaType(media);
//            media = null;

//            // Configure the samplegrabber
//            hr = sampGrabberStill.SetCallback(this, 1);
//            DsError.ThrowExceptionForHR(hr);
//        }
//        private void SaveSizeInfo(ISampleGrabber sampGrabber)
//        {
//            int hr;
//            // Get the media type from the SampleGrabber
//            AMMediaType media = new AMMediaType();
//            hr = sampGrabber.GetConnectedMediaType(media);
//            DsError.ThrowExceptionForHR(hr);

//            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
//            {
//                throw new NotSupportedException("Unknown Grabber Media Format");
//            }

//            // Grab the size info
//            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
//            m_videoWidth = videoInfoHeader.BmiHeader.Width;
//            m_videoHeight = videoInfoHeader.BmiHeader.Height;
//            m_stride = m_videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

//            DsUtils.FreeAMMediaType(media);
//            media = null;
//        }
//        private void SetupGraph(DsDevice dev, int iWidth, int iHeight, short iBPP)
//        {
//            int hr;
//            ISampleGrabber sampGrabber = null;
//            ISampleGrabber sampGrabberStill = null;    //add by tim

//            IBaseFilter capFilter = null;
//            IPin pCaptureOut = null;
//            IPin pCaptureOutStill = null;              //add by tim
//            IPin pStill = null;                        //add by tim
//            IPin pSampleIn = null;
//            IPin pSampleOut = null;

//            IPin pSampleInStill = null;                //add by tim
//            IPin pSampleOutStill = null;               //add by tim

//            IPin pRenderIn = null;
//            IPin pRenderIn_2 = null;

//            IPin pRenderIn_3 = null;                   //add by tim
//            IPin pRenderIn_4 = null;                   //add by tim

//            // Get the graphbuilder object
//            graphBuilder = new FilterGraph() as IFilterGraph2;
//            try
//            {
//#if DEBUG
//                m_rot = new DsROTEntry(graphBuilder);
//#endif
//                // add the video input device
//                hr = graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
//                DsError.ThrowExceptionForHR(hr);

//                IPin pRaw = null;
//                IPin pSmart = null;

//                IPin pRawStill = null;                  //add by tim         
//                IPin pSmartStill = null;                //add by tim

//                // There is no still pin
//                m_VidControl = null;
//                // Add a splitter
//                IBaseFilter iSmartTee = (IBaseFilter)new SmartTee();
//                IBaseFilter iSmartTeeStill = (IBaseFilter)new SmartTee();  //add by tim
//                try
//                {
//                    hr = graphBuilder.AddFilter(iSmartTee, "SmartTee");
//                    DsError.ThrowExceptionForHR(hr);

//                    hr = graphBuilder.AddFilter(iSmartTeeStill, "SmartTeeStill");  //add by tim
//                    DsError.ThrowExceptionForHR(hr);

//                    // Find the find the capture pin from the video device and the
//                    // input pin for the splitter, and connnect them
//                    //从视频设备中找到捕获管脚和拆分器的输入管脚，然后连接它们
//                    pRaw = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
//                    pSmart = DsFindPin.ByDirection(iSmartTee, PinDirection.Input, 0);

//                    hr = graphBuilder.Connect(pRaw, pSmart);
//                    DsError.ThrowExceptionForHR(hr);

//                    //pStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);             // by tim
//                    //pSmartStill = DsFindPin.ByDirection(iSmartTeeStill, PinDirection.Input, 0); // by tim

//                    //hr = graphBuilder.Connect(pStill, pSmartStill);                             // by tim
//                    //DsError.ThrowExceptionForHR(hr);                                            // by tim

//                    // Now set the capture and still pins (from the splitter)
//                    pPreviewOut = DsFindPin.ByName(iSmartTee, "Preview");
//                    pCaptureOut = DsFindPin.ByName(iSmartTee, "Capture");

//                    pPreviewOutStill = DsFindPin.ByName(iSmartTeeStill, "Preview");
//                    pCaptureOutStill = DsFindPin.ByName(iSmartTeeStill, "Capture");


//                    // If any of the default config items are set, perform the config
//                    // on the actual video device (rather than the splitter)
//                    if (iHeight + iWidth + iBPP > 0)
//                    {
//                        SetConfigParms(pRaw, iWidth, iHeight, iBPP);
//                        //SetConfigParms(pStill, iWidth, iHeight, iBPP);
//                    }
//                }
//                finally
//                {
//                    if (pRaw != null)
//                    {
//                        Marshal.ReleaseComObject(pRaw);
//                    }
//                    if (pRaw != pSmart)
//                    {
//                        Marshal.ReleaseComObject(pSmart);
//                    }
//                    if (pRaw != iSmartTee)
//                    {
//                        Marshal.ReleaseComObject(iSmartTee);
//                    }

//                    if (pStill != null)                             // by tim   
//                    {
//                        Marshal.ReleaseComObject(pStill);
//                    }
//                    if (pStill != pSmartStill)                      // by tim
//                    {
//                        Marshal.ReleaseComObject(pSmartStill);
//                    }
//                    if (pStill != iSmartTeeStill)                   // by tim
//                    {
//                        Marshal.ReleaseComObject(iSmartTeeStill);
//                    }
//                }

//                // Get the SampleGrabber interface
//                sampGrabber = new SampleGrabber() as ISampleGrabber;
//                // Configure the sample grabber
//                IBaseFilter baseGrabFlt = sampGrabber as IBaseFilter;
//                ConfigureSampleGrabber(sampGrabber);
//                pSampleIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
//                pSampleOut = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);

//                // Get the SampleGrabber interface
//                sampGrabberStill = new SampleGrabber() as ISampleGrabber;                              // by tim
//                // Configure the sample grabber Still  
//                IBaseFilter baseGrabFlt_Still = sampGrabberStill as IBaseFilter;                       // by tim
//                ConfigureSampleGrabberStill(sampGrabberStill);
//                pSampleInStill = DsFindPin.ByDirection(baseGrabFlt_Still, PinDirection.Input, 0);      // by tim
//                pSampleOutStill = DsFindPin.ByDirection(baseGrabFlt_Still, PinDirection.Output, 0);    // by tim


//                // Get the default video renderer
//                vmr9 = (IBaseFilter)new VideoMixingRenderer9();
//                ConfigureVMR9InWindowlessMode();
//                hr = graphBuilder.AddFilter(vmr9, "Video Mixing Renderer 9");
//                DsError.ThrowExceptionForHR(hr);
//                pRenderIn = DsFindPin.ByDirection(vmr9, PinDirection.Input, 0);

//                // Get the default video renderer
//                vmr9_2 = (IBaseFilter)new VideoMixingRenderer9();
//                ConfigureVMR9InWindowlessMode_2();
//                hr = graphBuilder.AddFilter(vmr9_2, "Video Mixing Renderer 9_2");
//                DsError.ThrowExceptionForHR(hr);
//                pRenderIn_2 = DsFindPin.ByDirection(vmr9_2, PinDirection.Input, 0);


//                // Get the default video renderer
//                vmr9_3 = (IBaseFilter)new VideoMixingRenderer9();                                      // by tim
//                ConfigureVMR9InWindowlessMode_3();
//                hr = graphBuilder.AddFilter(vmr9_3, "Video Mixing Renderer 9_3");
//                DsError.ThrowExceptionForHR(hr);
//                pRenderIn_3 = DsFindPin.ByDirection(vmr9_3, PinDirection.Input, 0);

//                // Get the default video renderer
//                vmr9_4 = (IBaseFilter)new VideoMixingRenderer9();                                     // by tim
//                ConfigureVMR9InWindowlessMode_4();
//                hr = graphBuilder.AddFilter(vmr9_4, "Video Mixing Renderer 9_4");
//                DsError.ThrowExceptionForHR(hr);
//                pRenderIn_4 = DsFindPin.ByDirection(vmr9_4, PinDirection.Input, 0);


//                hr = graphBuilder.AddFilter(baseGrabFlt, "Ds.NET Grabber");
//                DsError.ThrowExceptionForHR(hr);

//                hr = graphBuilder.AddFilter(baseGrabFlt_Still, "Still");                        //Still
//                DsError.ThrowExceptionForHR(hr);

//                // Connect the Still pin to the sample grabber
//                hr = graphBuilder.Connect(pPreviewOut, pSampleIn);
//                DsError.ThrowExceptionForHR(hr);

//                //// Connect the Still pin to the sample grabber
//                //hr = graphBuilder.Connect(pPreviewOutStill, pSampleInStill);                     // by tim
//                //DsError.ThrowExceptionForHR(hr);

//                // Connect the capture pin to the renderer
//                hr = graphBuilder.Connect(pCaptureOut, pRenderIn_2);
//                DsError.ThrowExceptionForHR(hr);

//                // Connect the capture pin to the renderer
//                hr = graphBuilder.Connect(pSampleOut, pRenderIn);
//                DsError.ThrowExceptionForHR(hr);

//                //// Connect the capture pin to the renderer
//                //hr = graphBuilder.Connect(pCaptureOutStill, pRenderIn_4);                       // by tim
//                //DsError.ThrowExceptionForHR(hr);

//                // Connect the capture pin to the renderer
//                //hr = graphBuilder.Connect(pSampleOutStill, pRenderIn_3);                        // by tim
//                //DsError.ThrowExceptionForHR(hr);

//                SaveSizeInfo(sampGrabber);
//                SaveSizeInfo(sampGrabberStill);                                                 // by tim
//                mediaControl = (IMediaControl)graphBuilder;
//                hr = mediaControl.Run();
//                DsError.ThrowExceptionForHR(hr);
//            }
//            finally
//            {
//                if (sampGrabber != null)
//                {
//                    Marshal.ReleaseComObject(sampGrabber);
//                    sampGrabber = null;
//                }
//                if (pCaptureOut != null)
//                {
//                    Marshal.ReleaseComObject(pCaptureOut);
//                    pCaptureOut = null;
//                }
//                if (pRenderIn != null)
//                {
//                    Marshal.ReleaseComObject(pRenderIn);
//                    pRenderIn = null;
//                }
//                if (pSampleIn != null)
//                {
//                    Marshal.ReleaseComObject(pSampleIn);
//                    pSampleIn = null;
//                }
//                if (sampGrabberStill != null)
//                {
//                    Marshal.ReleaseComObject(sampGrabberStill);
//                    sampGrabberStill = null;
//                }
//                if (pCaptureOutStill != null)
//                {
//                    Marshal.ReleaseComObject(pCaptureOutStill);
//                    pCaptureOutStill = null;
//                }
//                if (pRenderIn_3 != null)
//                {
//                    Marshal.ReleaseComObject(pRenderIn_3);
//                    pRenderIn_3 = null;
//                }
//                if (pRenderIn_4 != null)
//                {
//                    Marshal.ReleaseComObject(pRenderIn_4);
//                    pRenderIn_4 = null;
//                }
//                if (pSampleInStill != null)
//                {
//                    Marshal.ReleaseComObject(pSampleInStill);
//                    pSampleInStill = null;
//                }
//            }
//        }

//        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
//        {
//            Marshal.ReleaseComObject(pSample);
//            return 0;
//        }

//        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
//        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
//        {
//            //按钮捕获的图片及后续处理可以在此执行
//            //{.....}

//            return 0;
//        }


//        public void FunChangepicture(int num, bool visible, Bitmap bitmap1)
//        {
//            if (num == 1)
//            {
//                panel3.BackgroundImage = bitmap1;
//            }
//            else if (num == 2)
//            {
//                panel4.BackgroundImage = bitmap1;
//            }
//        }
//        private void snapImage()
//        {
//            if (windowlessCtrl != null)
//            {
//                IntPtr currentImage = IntPtr.Zero;
//                Bitmap bmp = null;

//                try
//                {
//                    int hr = windowlessCtrl.GetCurrentImage(out currentImage);
//                    DsError.ThrowExceptionForHR(hr);

//                    if (currentImage != IntPtr.Zero)
//                    {
//                        BitmapInfoHeader structure = new BitmapInfoHeader();
//                        Marshal.PtrToStructure(currentImage, structure);

//                        bmp = new Bitmap(structure.Width, structure.Height, (structure.BitCount / 8) * structure.Width, System.Drawing.Imaging.PixelFormat.Format32bppArgb, new IntPtr(currentImage.ToInt64() + 40));
//                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
//                        file_name_image = Application.StartupPath + "\\Image\\pic" + DateTime.Now.ToString("yyyyMMdd") + count_pic.ToString("D4") + ".jpg";
//                        bmp.Save(file_name_image, System.Drawing.Imaging.ImageFormat.Jpeg);
//                    }
//                }
//                catch (Exception anyException)
//                {
//                    MessageBox.Show("Failed getting image: " + anyException.Message);
//                }
//                finally
//                {
//                    if (bmp != null)
//                    {
//                        bmp.Dispose();
//                    }
//                    Marshal.FreeCoTaskMem(currentImage);
//                }
//            }
//        }

//        private void MainForm_Load(object sender, EventArgs e)
//        {
//            Capture(1, 640, 480, 24);
//        }
//        public void Capture(int iDeviceNum, int iWidth, int iHeight, short iBPP)
//        {
//            DsDevice[] capDevices;
//            // Get the collection of video devices
//            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
//            if (iDeviceNum + 1 > capDevices.Length)
//            {
//                throw new Exception("No video capture devices found at that index!");
//            }
//            try
//            {
//                // Set up the capture graph
//                SetupGraph(capDevices[iDeviceNum], iWidth, iHeight, iBPP);
//            }
//            catch
//            {
//                Dispose();
//                throw;
//            }
//        }
//    }
//}