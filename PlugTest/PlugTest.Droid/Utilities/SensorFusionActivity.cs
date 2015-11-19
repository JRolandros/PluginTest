using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using PlugTest.Droid.Utilities;
using Xamarin.Forms;
using PlugTest.Utilities;
using Java.Util;
using DeviceMotion.Plugin.Abstractions;

[assembly: Dependency(typeof(SensorFusionActivity))]
namespace PlugTest.Droid.Utilities
{
    public class SensorFusionActivity : Java.Lang.Object, IDeviceOrientation
    {
        #region properties

        //used in the gyroFunction
        private static float NS2S = 1.0f / 1000000000.0f;
        private float timestamp;
        private bool initState = true;

        public static int TIME_CONSTANT = 30;
        public static float FILTER_COEFFICIENT = 0.98f;
        private Timer fuseTimer = new Timer();

        //
        public static float EPSILON = 0.000000001f;
        // angular speeds from gyro
        private float[] gyro = new float[3];

        // rotation matrix from gyro data
        private static float[] gyroMatrix = new float[9];

        // orientation angles from gyro matrix
        private static float[] gyroOrientation = new float[3];

        // magnetic field vector
        private float[] magnet = new float[3];

        // accelerometer vector
        private float[] accel = new float[3];

        // orientation angles from accel and magnet
        private static float[] accMagOrientation = new float[3];

        // final orientation angles from sensor fusion
        public static float[] fusedOrientation = new float[3];

        // accelerometer and magnetometer based rotation matrix
        private float[] rotationMatrix = new float[9];
        #endregion
        private static long newTimestamp;

        public SensorFusionActivity()
        {
            gyroOrientation[0] = 0.0f;
            gyroOrientation[1] = 0.0f;
            gyroOrientation[2] = 0.0f;

            // initialise gyroMatrix with identity matrix
            gyroMatrix[0] = 1.0f; gyroMatrix[1] = 0.0f; gyroMatrix[2] = 0.0f;
            gyroMatrix[3] = 0.0f; gyroMatrix[4] = 1.0f; gyroMatrix[5] = 0.0f;
            gyroMatrix[6] = 0.0f; gyroMatrix[7] = 0.0f; gyroMatrix[8] = 1.0f;



            // wait for one second until gyroscope and magnetometer/accelerometer
            // data is initialised then scedule the complementary filter task
            fuseTimer.ScheduleAtFixedRate(new calculateFusedOrientationTask(), 1000, TIME_CONSTANT);

        }

        public float[] getFusOrientation()
        {
            return fusedOrientation;
        }
        public void setAccelerometerParam(float[] values)
        {
            accel = values;
        }
        public void setMagnetometerParam(float[] values)
        {
            gyroOrientation= values;
        }

        //you have to set the accelerometer and gyroscope values before calling this.
        public void StartCalculating()
        {
            newTimestamp = new DateTime().Ticks;
            //collect data
            calculateAccMagOrientation();
            gyroFunction(gyroOrientation);

        }


        public double getDeltaT()
        {
            return newTimestamp - timestamp;
        }

        #region implement orientation calculations
        public void calculateAccMagOrientation()
        {
            //SensorManager.get
            if (SensorManager.GetRotationMatrix(rotationMatrix, null, accel, magnet))
            {
                SensorManager.GetOrientation(rotationMatrix, accMagOrientation);
            }
        }




        public void gyroFunction(float[] values)
        {
            // don't start until first accelerometer/magnetometer orientation has been acquired
            if (accMagOrientation == null)
                return;

            // initialisation of the gyroscope based rotation matrix
            if (initState)
            {
                newTimestamp = new DateTime().Ticks;
                float[] initMatrix = new float[9];
                initMatrix = getRotationMatrixFromOrientation(accMagOrientation);
                float[] test = new float[3];
                SensorManager.GetOrientation(initMatrix, test);
                gyroMatrix = matrixMultiplication(gyroMatrix, initMatrix);
                initState = false;
            }

            // copy the new gyro values into the gyro array
            // convert the raw gyro data into a rotation vector
            float[] deltaVector = new float[4];
            if (timestamp != 0)
            {
                float dT = (newTimestamp - timestamp) * NS2S;
                System.Array.Copy(values, 0, gyro, 0, 3);
                //System.Arraycopy(event.values, 0, gyro, 0, 3);
                getRotationVectorFromGyro(gyro, deltaVector, dT / 2.0f);
            }

            // measurement done, save current time for next interval
            timestamp = newTimestamp;

            // convert rotation vector into rotation matrix
            float[] deltaMatrix = new float[9];
            SensorManager.GetRotationMatrixFromVector(deltaMatrix, deltaVector);

            // apply the new rotation interval on the gyroscope based rotation matrix
            gyroMatrix = matrixMultiplication(gyroMatrix, deltaMatrix);

            // get the gyroscope based orientation from the rotation matrix
            SensorManager.GetOrientation(gyroMatrix, gyroOrientation);
        }


        //retrieves gyro rotation values
        private void getRotationVectorFromGyro(float[] gyroValues,
                                       float[] deltaRotationVector,
                                       float timeFactor)
        {
            float[] normValues = new float[3];

            // Calculate the angular speed of the sample
            float omegaMagnitude =
                (float)Math.Sqrt(gyroValues[0] * gyroValues[0] +
                gyroValues[1] * gyroValues[1] +
                gyroValues[2] * gyroValues[2]);

            // Normalize the rotation vector if it's big enough to get the axis
            if (omegaMagnitude > EPSILON)
            {
                normValues[0] = gyroValues[0] / omegaMagnitude;
                normValues[1] = gyroValues[1] / omegaMagnitude;
                normValues[2] = gyroValues[2] / omegaMagnitude;
            }


            // Integrate around this axis with the angular speed by the timestep
            // in order to get a delta rotation from this sample over the timestep
            // We will convert this axis-angle representation of the delta rotation
            // into a quaternion before turning it into the rotation matrix.
            float thetaOverTwo = omegaMagnitude * timeFactor;
            float sinThetaOverTwo = (float)Math.Sin(thetaOverTwo);
            float cosThetaOverTwo = (float)Math.Cos(thetaOverTwo);
            deltaRotationVector[0] = sinThetaOverTwo * normValues[0];
            deltaRotationVector[1] = sinThetaOverTwo * normValues[1];
            deltaRotationVector[2] = sinThetaOverTwo * normValues[2];
            deltaRotationVector[3] = cosThetaOverTwo;
        }
        private static float[] getRotationMatrixFromOrientation(float[] o)
        {
            float[] xM = new float[9];
            float[] yM = new float[9];
            float[] zM = new float[9];

            float sinX = (float)Math.Sin(o[1]);
            float cosX = (float)Math.Cos(o[1]);
            float sinY = (float)Math.Sin(o[2]);
            float cosY = (float)Math.Cos(o[2]);
            float sinZ = (float)Math.Sin(o[0]);
            float cosZ = (float)Math.Cos(o[0]);

            // rotation about x-axis (pitch)
            xM[0] = 1.0f; xM[1] = 0.0f; xM[2] = 0.0f;
            xM[3] = 0.0f; xM[4] = cosX; xM[5] = sinX;
            xM[6] = 0.0f; xM[7] = -sinX; xM[8] = cosX;

            // rotation about y-axis (roll)
            yM[0] = cosY; yM[1] = 0.0f; yM[2] = sinY;
            yM[3] = 0.0f; yM[4] = 1.0f; yM[5] = 0.0f;
            yM[6] = -sinY; yM[7] = 0.0f; yM[8] = cosY;

            // rotation about z-axis (azimuth)
            zM[0] = cosZ; zM[1] = sinZ; zM[2] = 0.0f;
            zM[3] = -sinZ; zM[4] = cosZ; zM[5] = 0.0f;
            zM[6] = 0.0f; zM[7] = 0.0f; zM[8] = 1.0f;

            // rotation order is y, x, z (roll, pitch, azimuth)
            float[] resultMatrix = matrixMultiplication(xM, yM);
            resultMatrix = matrixMultiplication(zM, resultMatrix);
            return resultMatrix;
        }

        private static float[] matrixMultiplication(float[] A, float[] B)
        {
            float[] result = new float[9];

            result[0] = A[0] * B[0] + A[1] * B[3] + A[2] * B[6];
            result[1] = A[0] * B[1] + A[1] * B[4] + A[2] * B[7];
            result[2] = A[0] * B[2] + A[1] * B[5] + A[2] * B[8];

            result[3] = A[3] * B[0] + A[4] * B[3] + A[5] * B[6];
            result[4] = A[3] * B[1] + A[4] * B[4] + A[5] * B[7];
            result[5] = A[3] * B[2] + A[4] * B[5] + A[5] * B[8];

            result[6] = A[6] * B[0] + A[7] * B[3] + A[8] * B[6];
            result[7] = A[6] * B[1] + A[7] * B[4] + A[8] * B[7];
            result[8] = A[6] * B[2] + A[7] * B[5] + A[8] * B[8];

            return result;
        }

        

        class calculateFusedOrientationTask : TimerTask
        {
            public override void Run()
            {
                float oneMinusCoeff = 1.0f - FILTER_COEFFICIENT;
                /*
             * Fix for 179° <--> -179° transition problem:
             * Check whether one of the two orientation angles (gyro or accMag) is negative while the other one is positive.
             * If so, add 360° (2 * math.PI) to the negative value, perform the sensor fusion, and remove the 360° from the result
             * if it is greater than 180°. This stabilizes the output in positive-to-negative-transition cases.
             */

                // azimuth
                if (gyroOrientation[0] < -0.5 * Math.PI && accMagOrientation[0] > 0.0)
                {
                    fusedOrientation[0] = (float)(FILTER_COEFFICIENT * (gyroOrientation[0] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[0]);
                    fusedOrientation[0] -= (fusedOrientation[0] > Convert.ToSingle( Math.PI)) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else if (accMagOrientation[0] < -0.5 * Math.PI && gyroOrientation[0] > 0.0)
                {
                    fusedOrientation[0] = (float)(FILTER_COEFFICIENT * gyroOrientation[0] + oneMinusCoeff * (accMagOrientation[0] + 2.0 * Math.PI));
                    fusedOrientation[0] -= (fusedOrientation[0] > Math.PI) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else
                {
                    fusedOrientation[0] = FILTER_COEFFICIENT * gyroOrientation[0] + oneMinusCoeff * accMagOrientation[0];
                }

                // pitch
                if (gyroOrientation[1] < -0.5 * Math.PI && accMagOrientation[1] > 0.0)
                {
                    fusedOrientation[1] = (float)(FILTER_COEFFICIENT * (gyroOrientation[1] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[1]);
                    fusedOrientation[1] -= (fusedOrientation[1] > Math.PI) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else if (accMagOrientation[1] < -0.5 * Math.PI && gyroOrientation[1] > 0.0)
                {
                    fusedOrientation[1] = (float)(FILTER_COEFFICIENT * gyroOrientation[1] + oneMinusCoeff * (accMagOrientation[1] + 2.0 * Math.PI));
                    fusedOrientation[1] -= (fusedOrientation[1] > Math.PI) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else
                {
                    fusedOrientation[1] = FILTER_COEFFICIENT * gyroOrientation[1] + oneMinusCoeff * accMagOrientation[1];
                }

                // roll
                if (gyroOrientation[2] < -0.5 * Math.PI && accMagOrientation[2] > 0.0)
                {
                    fusedOrientation[2] = (float)(FILTER_COEFFICIENT * (gyroOrientation[2] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[2]);
                    fusedOrientation[2] -= (fusedOrientation[2] > Math.PI) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else if (accMagOrientation[2] < -0.5 * Math.PI && gyroOrientation[2] > 0.0)
                {
                    fusedOrientation[2] = (float)(FILTER_COEFFICIENT * gyroOrientation[2] + oneMinusCoeff * (accMagOrientation[2] + 2.0 * Math.PI));
                    fusedOrientation[2] -= (fusedOrientation[2] > Math.PI) ? Convert.ToSingle(2.0 * Math.PI) : 0;
                }
                else
                {
                    fusedOrientation[2] = FILTER_COEFFICIENT * gyroOrientation[2] + oneMinusCoeff * accMagOrientation[2];
                }

                // overwrite gyro matrix and orientation with fused orientation
                // to comensate gyro drift
                gyroMatrix = getRotationMatrixFromOrientation(fusedOrientation);
                System.Array.Copy(fusedOrientation, 0, gyroOrientation, 0, 3);
            }
        }
        #endregion

        public IDeviceOrientation setAccelerometerParam(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        IDeviceOrientation IDeviceOrientation.setAccelerometerParam(float[] values)
        {
            accel = values;
            return this;
        }

        public IDeviceOrientation setMagnetometerParam(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        IDeviceOrientation IDeviceOrientation.setMagnetometerParam(float[] values)
        {
            magnet = values;
            return this;
        }

        public float[] getRotationMatrix()
        {
            throw new NotImplementedException();
        }

        public float[] getOrientation()
        {
            throw new NotImplementedException();
        }

        public float[] getOrientation(float[] rotationMatrix)
        {
            gyroOrientation = rotationMatrix;

            StartCalculating();
            return fusedOrientation;

        }

        public void getRotationMatrix(float[] R, float[] I, float[] elem1, float[] elem2)
        {
            throw new NotImplementedException();
        }
    }
}