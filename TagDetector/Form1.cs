using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Aruco;
using System.Diagnostics;
using System.Threading;


namespace TagDetector
{
    public partial class Form1 : Form
    {
        VideoCapture capture;

        Bitmap image;
        Thread processThread;
        public Form1()
        {
            InitializeComponent();
            processThread = new Thread(new ThreadStart(processLoop));
            processThread.Priority = ThreadPriority.Highest;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture((int)numericUpDown1.Value);
            capture.Open((int)numericUpDown1.Value, VideoCaptureAPIs.DSHOW);
            capture.Set(VideoCaptureProperties.FrameWidth, 1280);
            capture.Set(VideoCaptureProperties.FrameHeight, 720);
            capture.Set(VideoCaptureProperties.Fps, 60);
            capture.Set(VideoCaptureProperties.FourCC, FourCC.MJPG);
            //dont touch the sort, magic
            processThread.Start();
        }

        void processLoop()
        {
            var lastTime = DateTime.Now;
            Mat frame;
            frame = new Mat();
            bool isDisplay = checkBox1.Checked;
            while (true)
            {
                Trace.WriteLine(capture.Read(frame));

                var detectorParameters = new DetectorParameters();
                detectorParameters.CornerRefinementMethod = CornerRefineMethod.None;

                using var dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.DictAprilTag_16h5);

                CvAruco.DetectMarkers(frame, dictionary, out var corners, out var ids, detectorParameters, out var rejectedPoints);

                if(isDisplay) { 
                    using var detectedMarkers = frame.Clone();
                    CvAruco.DrawDetectedMarkers(detectedMarkers, corners, ids, Scalar.Crimson);
                    Cv2.ImShow("aaa", detectedMarkers);
                    Cv2.WaitKey(1);
                }

                //TODO: pixel pos to camera pos https://docs.opencv.org/4.x/d5/dae/tutorial_aruco_detection.html

                Invoke(new Action(() =>
                {
                    fpslabel.Text = "Calc Time: " + (DateTime.Now - lastTime).TotalSeconds;
                    detlabel.Text = "Detection: " + ids.Length;
                }));
                
                //TODO: send it through websocket, using some sort of line-up mechanism

                lastTime = DateTime.Now;
                
            }
        }
    }
}