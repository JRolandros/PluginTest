using System;
using System.Collections.Generic;
using System.Text;

namespace PlugTest.Utilities
{
    public interface IDeviceOrientation
    {
        IDeviceOrientation setAccelerometerParam(float x, float y, float z);
        IDeviceOrientation setAccelerometerParam(float[] values);
        IDeviceOrientation setMagnetometerParam(float x, float y, float z);
        IDeviceOrientation setMagnetometerParam(float[] values);
        float[] getRotationMatrix();
        float[] getOrientation();
        float[] getOrientation(float[] rotationMatrix);
        void getRotationMatrix(float[] R, float[] I, float[] elem1, float[] elem2);
        void gyroFunction(float[] values);
        void calculateAccMagOrientation();
        float[] getFusOrientation();
        double getDeltaT();

    }
}
