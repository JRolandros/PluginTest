using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PlugTest.Utilities
{
    public class OrientationViewModel: INotifyPropertyChanged
    {
        string textContent;
        double valueX, valueY, valueZ;
        double rotation, rotationX, rotationY;
        public event PropertyChangedEventHandler PropertyChanged;

        public string TextContent
        {
            set
            {
                if (textContent != value)
                {
                    textContent = value;
                    OnPropertyChanged("TextContent");
                }
            }

            get
            {
                return textContent;
            }
        }

        public double ValueX
        {
            set
            {
                if (valueX != value)
                {
                    valueX = value;
                    OnPropertyChanged("ValueX");
                }
            }

            get
            {
                return valueX;
            }
        }

        public double ValueY
        {
            set
            {
                if (valueY != value)
                {
                    valueY = value;
                    OnPropertyChanged("ValueY");
                }
            }

            get
            {
                return valueY;
            }
        }

        public double ValueZ
        {
            set
            {
                if (valueZ != value)
                {
                    valueZ = value;
                    OnPropertyChanged("ValueZ");
                }
            }

            get
            {
                return valueZ;
            }
        }

        public double Rotation
        {
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    OnPropertyChanged("Rotation");
                }
            }
            get
            {
                return rotation;
            }
        }

        public double RotationX
        {
            set
            {
                if (rotationX != value)
                {
                    rotationX = value;
                    OnPropertyChanged("RotationX");
                }
            }
            get
            {
                return rotationX;
            }
        }

        public double RotationY
        {
            set
            {
                if (RotationY != value)
                {
                    rotationY = value;
                    OnPropertyChanged("RotationY");
                }
            }
            get
            {
                return rotationY;
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
