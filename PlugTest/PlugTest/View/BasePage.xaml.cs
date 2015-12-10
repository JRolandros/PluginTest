using DeviceMotion.Plugin;
using PlugTest.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PlugTest.View
{
	public partial class BasePage : ContentPage
	{
        
        double x_position = 0.35;
        double y_position = 0.5;
        private OrientationViewModel vm;
        private GetDeviceOrientation deviceOrientation;
        private Rectangle rect;
        public ObservableCollection<ToolbarItem> LeftToolbarItems { get; set; }
        public BasePage ()
		{
            vm = new OrientationViewModel();
            InitializeComponent ();

            deviceOrientation = new GetDeviceOrientation();
            initViewModel();

            LeftToolbarItems = new ObservableCollection<ToolbarItem>();

            BoxView bv1 = new BoxView();
            bv1.Color = Color.Green;
            rect = new Rectangle(.35, .5, 50, 50);
            AbsoluteLayout.SetLayoutBounds(bv1, rect);
            AbsoluteLayout.SetLayoutFlags(bv1, AbsoluteLayoutFlags.PositionProportional);
            
            al.Children.Add(bv1);


            CrossDeviceMotion.Current.SensorValueChanged += (s, a) =>
            {
                deviceOrientation.sensorRegistration(s, a);
                initViewModel();

                x_position = deviceOrientation.distanceMoveX( vm.RotationX);
                //y_position= deviceOrientation.distanceMoveX(vm.RotationY);
                //y_position -= 0.5 * vm.ValueY * Math.Pow(deviceOrientation.dt, 2);

                // x_pos.Text = "X: " + x_position + " Y: " + vm.ValueX + " Z: "+vm.ValueZ;
                //bv1.Rotation = vm.Rotation;
                //bv1.RotationX = vm.RotationX;
                //bv1.RotationY = vm.RotationY;
                AbsoluteLayout.SetLayoutBounds(bv1, new Rectangle(x_position, y_position, 50, 50));
            };

            this.BindingContext = vm;
        }

        private void initViewModel()
        {
            //vm.TextContent = deviceOrientation.str;
            vm.ValueX = deviceOrientation.gravity[0];
            vm.ValueY = deviceOrientation.gravity[1];
            vm.ValueZ = deviceOrientation.gravity[2];
            vm.Rotation = -deviceOrientation.degree[0];
            vm.RotationX = -deviceOrientation.degree[1];
            vm.RotationY = -deviceOrientation.degree[2];
        }

        private double getModuleAccel()
        {
            return Math.Sqrt(Math.Pow(vm.ValueX, 2) + Math.Pow(vm.ValueY, 2) + Math.Pow(vm.ValueZ, 2));
        }
    }
}
