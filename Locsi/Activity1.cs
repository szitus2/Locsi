using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Locsi
{
    [Activity(Label = "@string/programs_activity_label", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]

    public class Activity1 : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ProgramsLayout);
            ListView lv = FindViewById<ListView>(Resource.Id.stepListViewPgm1);
            ProgramClass pgm = new ProgramClass();
            pgm.numSteps = 4;
            pgm.steps[0] = new ProgramStepClass() { station = 1, duration = 10 };
            pgm.steps[1] = new ProgramStepClass() { station = 2, duration = 11 };
            pgm.steps[2] = new ProgramStepClass() { station = 3, duration = 12 };
            pgm.steps[3] = new ProgramStepClass() { station = 4, duration = 13 };
            pgm.steps[4] = new ProgramStepClass() { station = 2, duration = 14 };
            pgm.steps[5] = new ProgramStepClass() { station = 1, duration = 15 };
            pgm.steps[6] = new ProgramStepClass() { station = 4, duration = 16 };
            pgm.steps[7] = new ProgramStepClass() { station = 3, duration = 17 };
            pgm.steps[8] = new ProgramStepClass() { station = 2, duration = 18 };
            pgm.steps[9] = new ProgramStepClass() { station = 4, duration = 19 };

            ProgramStepLineAdapter psla = new ProgramStepLineAdapter(this, pgm);
            lv.Adapter = psla;
        }
    }
}