using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;


using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;
using System.Threading;

namespace PlugTest.WinPhone.Pages
{
    public partial class CameraPage : PhoneApplicationPage
    {
        // Variables 
        PhotoCamera myCamera;
        PageOrientation orientation;

        // Holds the current flash mode. 
        private string currentFlashMode;

        // Holds the current resolution index. 
        int currentResIndex = 0;
        public CameraPage()
        {
            InitializeComponent();

            this.Orientation = Microsoft.Phone.Controls.PageOrientation.Landscape;
            this.OrientationChanged += cam_OrientationChanged;
            ShowPreview();
        }

        void ShowPreview()
        {
            myCamera = new Microsoft.Devices.PhotoCamera(CameraType.Primary);

            myCamera.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(camera_CaptureCompleted);

            // myCamera.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(camera_CaptureImageAvailable);

            viewfinderBrush.SetSource(myCamera);
        }

        void camera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            Thread.Sleep(100);
            Deployment.Current.Dispatcher.BeginInvoke(delegate ()
            {
                MessageBox.Show("Capture Completed");

            });
        }


        private void cam_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
            switch (e.Orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    viewfinderTransform.Rotation = 0;
                    break;
                case PageOrientation.LandscapeRight:
                    viewfinderTransform.Rotation = 180;
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    viewfinderTransform.Rotation = 90;
                    break;
                case PageOrientation.PortraitDown:
                    viewfinderTransform.Rotation = 270;
                    break;
            }
        }
    }
}