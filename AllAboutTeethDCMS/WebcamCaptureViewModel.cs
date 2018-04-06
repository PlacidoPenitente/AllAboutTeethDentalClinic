using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WebCam_Capture;

namespace AllAboutTeethDCMS
{
    public class WebcamCaptureViewModel : PageViewModel
    {
        public WebcamCaptureViewModel()
        {
            StartCameraCommand = new DelegateCommand(new Action(startCamera));
            CaptureCommand = new DelegateCommand(new Action(capture));
            GoBackCommand = new DelegateCommand(new Action(goBack));
        }
        private WebCam webCam;

        private DelegateCommand startCameraCommand;
        private DelegateCommand captureCommand;
        private DelegateCommand goBackCommand;

        public WebCam WebCam { get => webCam; set => webCam = value; }
        public DelegateCommand StartCameraCommand { get => startCameraCommand; set => startCameraCommand = value; }
        public DelegateCommand CaptureCommand { get => captureCommand; set => captureCommand = value; }
        public DelegateCommand GoBackCommand { get => goBackCommand; set => goBackCommand = value; }
        public Image Image { get => image; set => image = value; }
        public string ImageData { get => imageData; set => imageData = value; }

        private bool isWebcamStarted = false;
        private Image image;
        private string imageData;

        public void startCamera()
        {
            if (WebCam == null)
            {
                WebCam = new WebCam();
                WebCam.InitializeWebCam(ref image);
            }
            WebCam.Continue();
            WebCam.Start();
            isWebcamStarted = true;
        }

        public void capture()
        {
            if (isWebcamStarted)
            {
                WebCam.Stop();
                isWebcamStarted = false;
                ImageData = convertToString(Image.Source);
            }
        }

        public void goBack()
        {
            if (isWebcamStarted)
            {
                isWebcamStarted = false;
                WebCam.Stop();
            }
        }
    }
}
