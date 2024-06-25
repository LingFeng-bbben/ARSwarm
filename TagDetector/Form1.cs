using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Aruco;
using System.Diagnostics;
using System.Threading;
using WebSocketSharp.Server;

namespace TagDetector
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        Thread processThread;
        public Form1()
        {
            InitializeComponent();
            processThread = new Thread(new ThreadStart(processLoop));
            processThread.Priority = ThreadPriority.Highest;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitCamera();
            processThread.Start();

        }

        private void InitCamera()
        {
            capture = new VideoCapture();
            capture.Open((int)numericUpDown1.Value, VideoCaptureAPIs.DSHOW);
            capture.Set(VideoCaptureProperties.FrameWidth, 1280);
            capture.Set(VideoCaptureProperties.FrameHeight, 720);
            capture.Set(VideoCaptureProperties.Fps, 60);
            capture.Set(VideoCaptureProperties.FourCC, FourCC.MJPG);
            //dont touch the sort, magic
        }

        void processLoop()
        {
            var lastTime = DateTime.Now;
            var nowTime = DateTime.Now;
            Mat frame;
            frame = new Mat();
            bool isDisplay = checkBox1.Checked;
            var markerLength = 0.051f;
            var objpoints = new Mat(4, 3, MatType.CV_32FC1);
            objpoints.At<float>(0, 0) = -markerLength / 2f; objpoints.At<float>(0, 1) = markerLength / 2f;
            objpoints.At<float>(1, 0) = markerLength / 2f; objpoints.At<float>(1, 1) = markerLength / 2f;
            objpoints.At<float>(2, 0) = markerLength / 2f; objpoints.At<float>(2, 1) = -markerLength / 2f;
            objpoints.At<float>(3, 0) = -markerLength / 2f; objpoints.At<float>(3, 1) = -markerLength / 2f;

            float[] cp = { 742.8213031760763f, 0, 644.1494364074133f, 0, 743.32282205624892f, 431.6402401387432f, 0, 0, 1 };
            var camMatrix = new Mat(3,3,MatType.CV_32FC1,cp);

            float[] de = { 0.082341193766002546f, -0.022321391433700741f, 0.00096243883092820637f, -0.0018705574042030097f, -0.243237764739379f };
            var destro = new Mat(5, 0, MatType.CV_32FC1, de);
            while (true)
            {
                Trace.WriteLine(capture.Read(frame));

                var detectorParameters = new DetectorParameters();
                detectorParameters.CornerRefinementMethod = CornerRefineMethod.Subpix;
                detectorParameters.AprilTagDeglitch = 2;
                detectorParameters.AdaptiveThreshWinSizeMin = 10;

                using var dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.DictAprilTag_16h5);

                CvAruco.DetectMarkers(frame, dictionary, out var corners, out var ids, detectorParameters, out var rejectedPoints);


                OutputArray[] rvec = new OutputArray[corners.Length];
                OutputArray[] tvec = new OutputArray[corners.Length];

                for (int i = 0; i < corners.Length; i++)
                {

                    var cornersM = new Mat(corners[i].Length, 1, MatType.CV_32FC2, corners[i]);
                    rvec[i] = new Mat(1, 3, MatType.CV_32FC1);
                    tvec[i] = new Mat(1, 3, MatType.CV_32FC1);
                    Cv2.SolvePnP(objpoints, cornersM, camMatrix, new Mat(), rvec[i], tvec[i]);
                    rvec[i].GetMat().GetArray<float>(out var rarry);
                    tvec[i].GetMat().GetArray<float>(out var tarry);
                    for (int j = 0; j < 3; j++) {
                        Program.Tagpos[ids[i], j] = rarry[j];
                        Program.Tagpos[ids[i], j+3] = tarry[j];
                    }
                }
                

                if (isDisplay)
                {
                    using var detectedMarkers = frame.Clone();
                    CvAruco.DrawDetectedMarkers(detectedMarkers, corners, ids, Scalar.Crimson);
                    for (int i = 0; i < corners.Length; i++) { 
                        Cv2.DrawFrameAxes(detectedMarkers, camMatrix, new Mat(), rvec[i].GetMat(), tvec[i].GetMat(), markerLength * 1.5f, 2);
                    }
                    Cv2.ImShow("aaa", detectedMarkers);
                    Cv2.WaitKey(1);
                }

                nowTime = DateTime.Now;
                //TODO: pixel pos to camera pos https://docs.opencv.org/4.x/d5/dae/tutorial_aruco_detection.html

                Invoke(new Action(() =>
                {
                    fpslabel.Text = "Calc Time: " + (nowTime - lastTime).TotalSeconds;
                    detlabel.Text = "Detection: " + ids.Length;
                }));

                Program.broadcast();
                //TODO: send it through websocket, using some sort of line-up mechanism

                lastTime = DateTime.Now;

            }
        }

    }
}