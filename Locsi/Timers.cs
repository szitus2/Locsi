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


namespace Locsi
{
    public class ArrayClassForJSON
    {
        public TimerClass[] timers;
    }

    [Activity(Label = "@string/timers_activity_label", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Timers : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public TimerClass[] tims = new TimerClass[10];
        public int selectedTimer = 0;

        private Switch sw;
        private EditText eth;
        private EditText etm;
        private Spinner spp;


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Timers);

            Spinner spin = FindViewById<Spinner>(Resource.Id.spinner1);
            List<string> strings = new List<string>() { "1-es időzítő", "2-es időzítő", "3-as időzítő", "4-es időzítő", "5-ös időzítő", "6-os időzítő", "7-es időzítő", "8-as időzítő", "9-es időzítő", "10-es időzítő" };
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, strings);
            spin.Adapter = dataAdapter;
            spin.ItemSelected += Spin_ItemSelected;

            spp = FindViewById<Spinner>(Resource.Id.spinner2);
            strings = new List<string>() { "1-es Program", "2-es Program", "3-as Program", "4-es Program", "5-ös Program", "6-os Program", "7-es Program", "8-as Program", "9-es Program", "10-es Program" };
            dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, strings);
            spp.Adapter = dataAdapter;
            //spp.ItemSelected += Spin2_ItemSelected;

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.bugyi);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_timers;

            await GetTimers();

            LoadData();

            
        }

        

        protected override void OnPause()
        {
            base.OnPause();
            if (tims[selectedTimer] == null) { return; }
            tims[selectedTimer].enabled = sw.Checked;
            tims[selectedTimer].hour = Convert.ToByte(eth.Text);
            tims[selectedTimer].minute = Convert.ToByte(etm.Text);
            tims[selectedTimer].program = Convert.ToByte(spp.SelectedItemPosition);
            _ = SaveTimers();
        }

        
        private void Spin_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                tims[selectedTimer].enabled = sw.Checked;
                tims[selectedTimer].hour = Convert.ToByte(eth.Text);
                tims[selectedTimer].minute = Convert.ToByte(etm.Text);
                tims[selectedTimer].program = Convert.ToByte(spp.SelectedItemPosition);
                selectedTimer = ((Spinner)sender).SelectedItemPosition;
                sw.Checked = tims[selectedTimer].enabled;
                eth.Text = tims[selectedTimer].hour.ToString();
                etm.Text = tims[selectedTimer].minute.ToString();
                spp.SetSelection(tims[selectedTimer].program);
            }
            catch { }
        }

        /*private async void Spin2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            
        }*/

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
                    intent = new Intent(this, typeof(SettingsAct));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    this.Finish();
                    return true;
                case Resource.Id.navigation_timers:
                    
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

        public async Task GetTimers()
        {
            try
            {
                HttpClient client = new HttpClient();

                System.Uri uri = new System.Uri("http://" + Common.Instance.LocsiHostname + "/gettimers");
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    ArrayClassForJSON dd = JsonConvert.DeserializeObject<ArrayClassForJSON>(content);
                    tims = dd.timers;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(1000);
                await GetTimers();
            }
            
        }

        public async Task SaveTimers()
        {
            ArrayClassForJSON dd = new ArrayClassForJSON();
            dd.timers = tims;
            string jsonString = JsonConvert.SerializeObject(dd);
            HttpClient client = new HttpClient();
            System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/settimers");
            HttpContent content = new StringContent(jsonString);
            HttpResponseMessage resp = await client.PostAsync(uri, content);
        }

        public void LoadData()
        {
            sw = FindViewById<Switch>(Resource.Id.switch1);
            sw.Checked = tims[selectedTimer].enabled;
            eth = FindViewById<EditText>(Resource.Id.editText1);
            eth.Text = tims[selectedTimer].hour.ToString();
            etm = FindViewById<EditText>(Resource.Id.editText2);
            etm.Text = tims[selectedTimer].minute.ToString();
            spp = FindViewById<Spinner>(Resource.Id.spinner2);
            spp.SetSelection(tims[selectedTimer].program);
        }
    }

}
