using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Locsi
{
    [Activity(Label = "@string/programs_manual_label", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ManualActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private Spinner sp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ManualLayout);
            sp = FindViewById<Spinner>(Resource.Id.spinnerMan);
            List<string> strings = new List<string>() { "1-es Program", "2-es Program", "3-as Program", "4-es Program", "5-ös Program", "6-os Program", "7-es Program", "8-as Program", "9-es Program", "10-es Program" };
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, strings);
            sp.Adapter = dataAdapter;

            Button bt = FindViewById<Button>(Resource.Id.startButtonMan);
            bt.Click += StartButtonClick;

            bt = FindViewById<Button>(Resource.Id.stopButtonMan);
            bt.Click += StopButtonClick;

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.BNVMan);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_manual;

        }

        private async void StartButtonClick(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/startprogram");
                HttpContent content = new StringContent("{\"program\": " + sp.SelectedItemPosition.ToString());
                HttpResponseMessage resp = await client.PostAsync(uri, content);
            }
            catch { }
            
        }

        private async void StopButtonClick(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/stopprogram");
                //HttpContent content = new StringContent("{\"program\": " + sp.SelectedItemPosition.ToString());
                HttpResponseMessage resp = await client.GetAsync(uri);
            }
            catch { } 
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            Intent intent;
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_settings:
                    intent = new Intent(this, typeof(SettingsAct));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_timers:
                    intent = new Intent(this, typeof(Timers));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_programs:
                    intent = new Intent(this, typeof(Programs));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_manual:
                    
                    return true;
            }
            return false;
        }
    }
}