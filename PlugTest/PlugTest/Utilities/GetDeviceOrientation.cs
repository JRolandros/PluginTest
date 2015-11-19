using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PlugTest.Utilities
{
   public class GetDeviceOrientation
    {
        public  string str;
        //Accelerometer vector
        public float[] gravity = new float[3];
        //Magnetic vector
        public float[] orientation = new float[3];
        public float[] degree = new float[3];
        //permet d'initialiser les timestamp
        private static bool counter=false;
        private long newTimestamp;
        private long lastTimestamp;

        public double dt;
        private IDeviceOrientation orienGetter = DependencyService.Get<IDeviceOrientation>();

        public GetDeviceOrientation()
        {
        }
        public void sensorRegistration( Object s,SensorValueChangedEventArgs a)
        {
            CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Ui);
            CrossDeviceMotion.Current.Start(MotionSensorType.Magnetometer, MotionSensorDelay.Ui);
            CrossDeviceMotion.Current.Start(MotionSensorType.Gyroscope, MotionSensorDelay.Ui);

            //TODO explain each line of code here
            float[] values = new float[3];
             float[] valuesGyro = new float[3];
            switch (a.SensorType)
                {
                    case MotionSensorType.Accelerometer:

                        values[0] = (float)((MotionVector)a.Value).X;
                        values[1] = (float)((MotionVector)a.Value).Y;
                        values[2] = (float)((MotionVector)a.Value).Z;
                        filterLowPass(values, gravity, 0.98f);
                        orienGetter.setAccelerometerParam(gravity);
                        orienGetter.calculateAccMagOrientation();
                        degree = orienGetter.getFusOrientation();
                    dt = orienGetter.getDeltaT();
                    //degree = orienGetter.getOrientation(valuesGyro);
                    break;
                    case MotionSensorType.Magnetometer:     
                        values[0] = (float)((MotionVector)a.Value).X;
                        values[1] = (float)((MotionVector)a.Value).Y;
                        values[2] = (float)((MotionVector)a.Value).Z;
                        orienGetter.setMagnetometerParam(values);
                        
                    //filterLowPass(orienGetter.getOrientation(), orientation, 0.98f);

                    //for (int i = 0; i < 3; i++)
                    //    degree[i] = (float)(orientation[i] * 180 / Math.PI);

                    //give a text title to current direction.
                    //str = directionEstimate(degree[0]);

                    //Console.WriteLine(str);

                    //transfer radian to degree
                    //for (int i = 0; i < 3; i++)
                    //{
                    //    if (degree[i] < 0)
                    //        degree[i] += 360;
                    //}




                    //Debug.WriteLine("Values: {0}, {1}, {2}, {3}", orientation[0], orientation[1], orientation[2], str);

                    break;
                case MotionSensorType.Gyroscope:
                    if(!counter)
                    {
                        counter = true;
                        lastTimestamp = DateTime.Now.ToLocalTime().Ticks;
                        newTimestamp = DateTime.Now.ToLocalTime().Ticks;
                        
                    }
                    dt = (newTimestamp-lastTimestamp)/Math.Pow(10,7);
                    lastTimestamp = newTimestamp;
                    valuesGyro[0] = (float)((MotionVector)a.Value).X;
                    valuesGyro[1] = (float)((MotionVector)a.Value).Y;
                    valuesGyro[2] = (float)((MotionVector)a.Value).Z;
                    orienGetter.gyroFunction(valuesGyro);
                    newTimestamp = DateTime.Now.ToLocalTime().Ticks;
                    degree = orienGetter.getFusOrientation();
                    break;
            };
        }
        public void filterLowPass(float[] arrin, float[] arrout, float alpha)
        {
            int len = arrin.Length;
            //Debug.WriteLine("arrin.length={0}, sizeof float={1}, len={2}", arrin.Length, sizeof(float), len);
            for (int i = 0; i < len; i++)
            {
                arrout[i] = alpha * arrout[i] + (1 - alpha) * arrin[i];
            }
        }
        public string directionEstimate(float d)
        {
            string result = "Result can not be found.";
            if (d >= -30 && d < 30)
                result = "Nord";
            else if (d >= 30 && d < 60)
                result = "Nord-Est";
            else if (d >= 60 && d < 120)
                result = "Est";
            else if (d >= 120 && d < 150)
                result = "Sud-Est";
            else if ((d >= 150 && d <= 180) || (d >= -180 && d < -150))
                result = "Sud";
            else if (d >= -150 && d < -120)
                result = "Sud-West";
            else if (d >= -120 && d < -60)
                result = "West";
            else if (d >= -60 && d < -30)
                result = "Nord-West";

            return result;
        }

        public double distanceMoveX(double angle)
        {
            return 2 * Math.Sin(angle / 2);
        }

        public double distanceMoveY(double angle)
        {
            return 1- Math.Cos(angle / 2);
        }
    }
}
