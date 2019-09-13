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
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Locsi
{
    [Activity(Label = "@string/settings_activity_label")]
    public class SettingsAct : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private EditText etip;
        private Button iprb;
        private Spinner sp1;
        private Spinner sp2;
        private EditText utc;
        private CheckBox dls;
        private ConfigMsgClass cfg = new ConfigMsgClass();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.SettingAct);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.SettingsBNV);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_settings;

            etip = FindViewById<EditText>(Resource.Id.IPAddrEditText);
            etip.AfterTextChanged += Etip_AfterTextChanged;
            iprb = FindViewById<Button>(Resource.Id.IPAddrRefrButton);
            iprb.Click += Iprb_Click;

            sp1 = FindViewById<Spinner>(Resource.Id.spinnerSensor1);
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, new List<string>() { "Kikapcsolva", "Záró érintkező", "Nyitó érintkező" });
            sp1.Adapter = dataAdapter;
            sp1.SetSelection(0);
            sp1.ItemSelected += Sp1_ItemSelected;
            sp2 = FindViewById<Spinner>(Resource.Id.spinnerSensor2);
            dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, new List<string>() { "Kikapcsolva", "Záró érintkező", "Nyitó érintkező" });
            sp2.Adapter = dataAdapter;
            sp2.SetSelection(0);
            sp2.ItemSelected += Sp1_ItemSelected;

            utc = FindViewById<EditText>(Resource.Id.utcOffsetText);
            utc.Text = "0";

            dls = FindViewById<CheckBox>(Resource.Id.dlsCheckbox);
            dls.Checked = false;

            _ = ReadData();
        }

        private void Etip_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            iprb.Enabled = true;
        }

        private void Iprb_Click(object sender, EventArgs e)
        {
            iprb.Enabled = false;
            Common.Instance.LocsiHostname = etip.Text;
            _ = ReadData();
        }

        protected override void OnResume()
        {
            base.OnResume();
            //etip = FindViewById<EditText>(Resource.Id.IPAddrEditText);
            etip.Text = Common.Instance.LocsiHostname;
        }

        protected override void OnPause()
        {
            base.OnPause();
            
            Common.Instance.LocsiHostname = etip.Text;
            Common.SaveData();
            WriteData();
        }

        private void Sp1_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner sp = sender as Spinner;
            if (sp == null) { return; }

            
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_settings:
                   
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


        public async Task ReadData()
        {
            try
            {
                HttpClient _client = new HttpClient();
                System.Uri uri = new System.Uri("http://" + Common.Instance.LocsiHostname + "/getconfig");
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    cfg = JsonConvert.DeserializeObject<ConfigMsgClass>(content);
                    sp1.SetSelection(Convert.ToInt32(cfg.sensor1));
                    sp2.SetSelection(Convert.ToInt32(cfg.sensor2));
                    utc.Text = (cfg.utcoffset / 3600F).ToString();
                    dls.Checked = (cfg.dlsoffset != 0);
                    iprb.Enabled = true;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(1000);
                await ReadData();
            }
        }

        public async void WriteData()
        {
            
            cfg.sensor1 = (SensorConfigEnum)(sp1.SelectedItemPosition);
            cfg.sensor2 = (SensorConfigEnum)(sp2.SelectedItemPosition);
            cfg.utcoffset = Convert.ToInt32(Convert.ToSingle(utc.Text) * 3600);
            cfg.dlsoffset = dls.Checked ? 3600 : 0;
            string jsonString = JsonConvert.SerializeObject(cfg);
            HttpClient client = new HttpClient();
            System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/setconfig");
            HttpContent content = new StringContent(jsonString);
            HttpResponseMessage resp = await client.PostAsync(uri, content);
        }

    }
}