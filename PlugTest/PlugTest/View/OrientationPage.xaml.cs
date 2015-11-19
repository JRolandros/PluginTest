using DeviceMotion.Plugin;
using PlugTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PlugTest.View
{
	public partial class OrientationPage : ContentPage
	{
        private OrientationViewModel vm;
        private GetDeviceOrientation deviceOrientation;
        public double x_pos;
		public OrientationPage ()
		{
            vm = new OrientationViewModel();
            InitializeComponent ();
            deviceOrientation = new GetDeviceOrientation();
            initViewModel();
            
            CrossDeviceMotion.Current.SensorValueChanged += (s, a) =>
            {
                deviceOrientation.sensorRegistration(s, a);
                initViewModel();
               // x_pos = Double.Parse(this.x.Text);
            };

            this.BindingContext = vm;
        }

        private void initViewModel()
        {  
            vm.TextContent =deviceOrientation.str;
            vm.ValueX = deviceOrientation.degree[0];
            vm.ValueY = deviceOrientation.degree[1];
            vm.ValueZ = deviceOrientation.degree[2];
            vm.Rotation = -deviceOrientation.degree[0];
            vm.RotationX = -deviceOrientation.degree[1];
            vm.RotationY = -deviceOrientation.degree[2];
        }
	}
}
