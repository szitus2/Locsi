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

namespace Locsi
{
    public class ConfigMsgClass
    {
        public bool turnedon;
        public SensorConfigEnum sensor1;
        public SensorConfigEnum sensor2;
        public int utcoffset;
        public int dlsoffset;
    }
}