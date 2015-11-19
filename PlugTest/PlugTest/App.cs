using PlugTest.View;
using Geolocator.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;

namespace PlugTest
{
	public class App : Application
	{
        OrientationPage myPage;
		public App ()
		{
            MainPage = new CameraPage();
            
        }

		protected override void OnStart ()
		{
            CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Game);
            CrossDeviceMotion.Current.Start(MotionSensorType.Magnetometer, MotionSensorDelay.Game);
            //CrossDeviceMotion.Current.Start(MotionSensorType.Compass);

            // Handle when your app starts
        }

        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
