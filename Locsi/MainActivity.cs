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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        TextView programText;
        TextView stepText;
        TextView stationText;
        TextView timeText;
        private CancellationTokenSource cts;

        public Common cm = Common.Instance;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            programText = FindViewById<TextView>(Resource.Id.messageProg);
            stationText = FindViewById<TextView>(Resource.Id.messageSta);
            stepText = FindViewById<TextView>(Resource.Id.messageSte);
            timeText = FindViewById<TextView>(Resource.Id.messageTim);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_home;

            //StartRefresh();

        }

        protected override void OnStart()
        {
            
            base.OnStart();
        }

        protected override void OnRestart()
        {
           
            base.OnRestart();
        }

        protected override void OnResume()
        {
            
            base.OnResume();
            StartRefresh();
        }

        protected override void OnStop()
        {
           
            base.OnStop();
        }

        protected override void OnPause()
        {
            
            base.OnPause();
            StopRefresh();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            Intent intent;
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:

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
                    intent = new Intent(this, typeof(ManualActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
            }
            return false;
        }

        public void StartRefresh()
        {
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            var ignore = RefreshDataAsync(cts.Token);
        }

        public void StopRefresh()
        {
            if (cts != null) { cts.Cancel(); }
            cts = null;
        }
       
        public async Task RefreshDataAsync(CancellationToken ct)
        {
            await Task.Delay(1000);
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    HttpClient _client = new HttpClient();
                    System.Uri uri = new System.Uri("http://" + Common.Instance.LocsiHostname + "/gettime");
                    HttpResponseMessage response = await _client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        DTClass dt = JsonConvert.DeserializeObject<DTClass>(content);
                        textMessage.SetText(dt.Date + " " + dt.Time, TextView.BufferType.Normal);
                    }

                    uri = new System.Uri("http://" + Common.Instance.LocsiHostname + "/getcurrentprogram");
                    response = await _client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        ProgClass pgc = JsonConvert.DeserializeObject<ProgClass>(content);
                        if (pgc.currentProgram == 255)
                        {
                            programText.SetText("Nincs futó program", TextView.BufferType.Normal);
                            stationText.SetText("---", TextView.BufferType.Normal);
                            stepText.SetText("--/--", TextView.BufferType.Normal);
                            timeText.SetText("--:--", TextView.BufferType.Normal);
                        }
                        else
                        {
                            programText.SetText((pgc.currentProgram + 1).ToString(), TextView.BufferType.Normal);
                            stationText.SetText(pgc.currentStation.ToString(), TextView.BufferType.Normal);
                            stepText.SetText((pgc.currentStep + 1).ToString() + "/" + pgc.totalStep.ToString(), TextView.BufferType.Normal);
                            timeText.SetText(pgc.remainingMin.ToString() + ":" + pgc.remainingSec.ToString(), TextView.BufferType.Normal);
                        }

                    }
                }
                catch { };
                

                //textMessage.SetText(state, TextView.BufferType.Normal);
                await Task.Delay(1000);
            }
            
        }
    }
}

