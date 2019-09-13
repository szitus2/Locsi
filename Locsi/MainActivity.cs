using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace Locsi
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        TextView messageSensor;
        TextView programText;
        TextView stepText;
        TextView stationText;
        TextView timeText;
        Button mainButton;

        private CancellationTokenSource cts;
        private bool turnedOn;

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
            mainButton = FindViewById<Button>(Resource.Id.mainButton);
            messageSensor = FindViewById<TextView>(Resource.Id.messageSensor);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_home;


            mainButton.Click += MainButton_Click;
            //StartRefresh();

        }

        private void MainButton_Click(object sender, System.EventArgs e)
        {
            mainButton.SetBackgroundColor(Color.DarkKhaki);
            turnedOn = !turnedOn;

            _ = SetTurnedOn();
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
                        if (pgc.turnedOn)
                        {
                            mainButton.SetBackgroundColor(Color.DarkGreen);
                            mainButton.SetTextColor(Color.White);
                            mainButton.Text = "Bekapcsolva";
                            turnedOn = true;
                        }
                        else
                        {
                            mainButton.SetBackgroundColor(Color.DarkGray);
                            mainButton.SetTextColor(Color.White);
                            mainButton.Text = "Kikapcsolva";
                            turnedOn = false;
                        }

                        if (pgc.sensorLock1 || pgc.sensorLock2)
                        {
                            messageSensor.SetTextColor(Color.Orange);
                            messageSensor.SetTypeface(Typeface.Create("Roboto", TypefaceStyle.Bold), TypefaceStyle.Bold);
                            messageSensor.Text = "" + (pgc.sensorLock1 ? "1-es " : "") + (pgc.sensorLock2 ? "2-es" : "");
                        }
                        else
                        {
                            messageSensor.SetTextColor(Color.White);
                            messageSensor.SetTypeface(Typeface.Create("Roboto", TypefaceStyle.Normal), TypefaceStyle.Normal);
                            messageSensor.Text = "Nincs jelzés";
                        }
                        
                    }
                }
                catch { };
                

                //textMessage.SetText(state, TextView.BufferType.Normal);
                await Task.Delay(1000);
            }
            
        }

        public async Task SetTurnedOn()
        {
            string jsonString = "{\"turnedon\": " + turnedOn.ToString().ToLower() + "}";
            HttpClient client = new HttpClient();
            System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/setturnon");
            HttpContent content = new StringContent(jsonString);
            HttpResponseMessage resp = await client.PostAsync(uri, content);
        }

    }

}

