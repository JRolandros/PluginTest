using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(PlugTest.View.CameraPage), typeof(PlugTest.WinPhone.Pages.CameraPageRenderer))]
namespace PlugTest.WinPhone.Pages
{
    class CameraPageRenderer: VisualElementRenderer<Xamarin.Forms.Page, Microsoft.Phone.Controls.PhoneApplicationPage>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            SetNativeControl(new CameraPage());
        }
    }
}
