using System;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PlugTest.Droid.Utilities;
using Xamarin.Forms;
using PlugTest.Utilities;
using Android.Hardware;

//[assembly: Dependency(typeof(PhoneOrientation))]

namespace PlugTest.Droid.Utilities
{
   public class PhoneOrientation : IDeviceOrientation
    {
            private float[] magnetoValues = new float[3];
            private float[] acceleroValues = new float[3];

            public IDeviceOrientation setAccelerometerParam(float x, float y, float z)
            {
                acceleroValues[0] = x;
                acceleroValues[1] = y;
                acceleroValues[2] = z;
                return this;
            }

            public IDeviceOrientation setAccelerometerParam(float[] values)
            {
                //values.CopyTo(acceleroValues, 0);
                acceleroValues = values;
                return this;
            }

            public IDeviceOrientation setMagnetometerParam(float x, float y, float z)
            {
                magnetoValues[0] = x;
                magnetoValues[1] = y;
                magnetoValues[2] = z;
                return this;
            }

            public IDeviceOrientation setMagnetometerParam(float[] values)
            {
                //values.CopyTo(magnetoValues, 0);
                magnetoValues = values;
                return this;
            }

            public float[] getRotationMatrix()
            {
                float[] R = new float[9];
                float[] I = new float[9];

                SensorManager.GetRotationMatrix(R, I, acceleroValues, magnetoValues);
                return R;
            }

            public void getRotationMatrix(float[] R, float[] I, float[] elem1, float[] elem2)
            {
                SensorManager.GetRotationMatrix(R, I, elem1, elem2);
            }

            public float[] getOrientation()
            {
                float[] orientation = new float[3];
                float[] R = new float[9];
                float[] I = new float[9];
                bool success = false;
                if (acceleroValues != null && magnetoValues != null)
                    success = SensorManager.GetRotationMatrix(R, I, acceleroValues, magnetoValues);
                if (success)
                {
                    SensorManager.GetOrientation(R, orientation);
                }
                else
                {
                    Console.WriteLine("GetOrientation failed.");
                }
                return orientation;
            }

            public float[] getOrientation(float[] rotationMatrix)
            {
                throw new NotImplementedException();
            }

        public void gyroFunction(float[] values)
        {
            throw new NotImplementedException();
        }

        public void calculateAccMagOrientation()
        {
            throw new NotImplementedException();
        }

        public float[] getFusOrientation()
        {
            throw new NotImplementedException();
        }

        public double getDeltaT()
        {
            throw new NotImplementedException();
        }
    }
}