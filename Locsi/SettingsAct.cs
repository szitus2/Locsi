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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.SettingAct);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.SettingsBNV);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_settings;

            Spinner sp = FindViewById<Spinner>(Resource.Id.spinnerSensor1);
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, new List<string>() { "Kikapcsolva", "Nyitó érintkező", "Záró érintkező" });
            sp.Adapter = dataAdapter;
            sp.SetSelection(0);
            sp.ItemSelected += Sp1_ItemSelected;
        }

        protected override void OnResume()
        {
            base.OnResume();
            EditText etip = FindViewById<EditText>(Resource.Id.IPAddrEditText);
            etip.Text = Common.Instance.LocsiHostname;
        }

        protected override void OnPause()
        {
            base.OnPause();
            EditText etip = FindViewById<EditText>(Resource.Id.IPAddrEditText);
            Common.Instance.LocsiHostname = etip.Text;
            Common.SaveData();
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




    }
}