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
    public class ProgramClassForJSON
    {
        public ProgramClass[] programs;
    }

    [Activity(Label = "@string/programs_activity_label", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Programs : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        ProgramClass[] programs;
        int selectedPgm = 0;
        PopupWindow popupWindow;
        int editPosition;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ProgramsLayout);
            ListView lv = FindViewById<ListView>(Resource.Id.stepListViewPgm1);
            Spinner sp = FindViewById<Spinner>(Resource.Id.spinnerPgm1);
            List<string> strings = new List<string>() { "1-es Program", "2-es Program", "3-as Program", "4-es Program", "5-ös Program", "6-os Program", "7-es Program", "8-as Program", "9-es Program", "10-es Program" };
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, strings);
            sp.Adapter = dataAdapter;
            sp.SetSelection(selectedPgm);
            sp.ItemSelected += Sp_ItemSelected;

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.BNWPgm);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.SelectedItemId = Resource.Id.navigation_programs;

            await GetPrograms();
            
            ProgramStepLineAdapter psla = new ProgramStepLineAdapter(this, programs[selectedPgm]);
            lv.Adapter = psla;
            lv.ItemClick += Lv_ItemClick;
           
            Button pbt = FindViewById<Button>(Resource.Id.plusButtonPgm1);
            pbt.Click += Pbt_Click;

            Button mbt = FindViewById<Button>(Resource.Id.minusButtonPgm1);
            mbt.Click += Mbt_Click;
        }

        
        private void Mbt_Click(object sender, EventArgs e)
        {
            if (programs[selectedPgm].numSteps > 0)
            {
                programs[selectedPgm].numSteps--;
                LoadData();
            }
        }

        private void Pbt_Click(object sender, EventArgs e)
        {
            if (programs[selectedPgm].numSteps < 10)
            {
                programs[selectedPgm].numSteps++;
                programs[selectedPgm].steps[programs[selectedPgm].numSteps - 1].station = 1;
                programs[selectedPgm].steps[programs[selectedPgm].numSteps - 1].duration = 10;
                LoadData();
            }
            
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ListView lv = FindViewById<ListView>(Resource.Id.stepListViewPgm1);

            LayoutInflater inflater = (LayoutInflater)(this.GetSystemService(Context.LayoutInflaterService));
            View popupView = inflater.Inflate(Resource.Layout.ProgramPopupLayout, null);
            int width = LinearLayout.LayoutParams.MatchParent;
            int height = LinearLayout.LayoutParams.WrapContent;
            bool focusable = true; // lets taps outside the popup also dismiss it
                        
            Button sb = popupView.FindViewById<Button>(Resource.Id.pgmpuSaveButton);
            sb.Click += Sb_Click;
            Button cb = popupView.FindViewById<Button>(Resource.Id.pgmpuCancelButton);
            cb.Click += Cb_Click;
            EditText et = popupView.FindViewById<EditText>(Resource.Id.pgmpuStation);
            et.Text = programs[selectedPgm].steps[e.Position].station.ToString();
            et = popupView.FindViewById<EditText>(Resource.Id.pgmpuDuration);
            et.Text = programs[selectedPgm].steps[e.Position].duration.ToString();
            editPosition = e.Position;
            popupWindow = new PopupWindow(popupView, width, height, focusable);
            popupWindow.ShowAtLocation(popupView, GravityFlags.Center, 0, 0);

        }

        private void Cb_Click(object sender, EventArgs e)
        {
            if (popupWindow != null) { popupWindow.Dismiss(); }
        }

        private void Sb_Click(object sender, EventArgs e)
        {
            if (popupWindow != null)
            {
                View pv = popupWindow.ContentView;
                EditText et = pv.FindViewById<EditText>(Resource.Id.pgmpuStation);
                programs[selectedPgm].steps[editPosition].station = Convert.ToByte(et.Text);
                et = pv.FindViewById<EditText>(Resource.Id.pgmpuDuration);
                programs[selectedPgm].steps[editPosition].duration = Convert.ToByte(et.Text);
                popupWindow.Dismiss();
                LoadData();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (programs == null || programs.GetUpperBound(0) < 1) { return; }
            _ = SavePrograms();
        }

        private void Sp_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner sp = sender as Spinner;
            if (sp == null) { return; }
            
            selectedPgm = sp.SelectedItemPosition;
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                ListView lv = FindViewById<ListView>(Resource.Id.stepListViewPgm1);
                ProgramStepLineAdapter psla = new ProgramStepLineAdapter(this, programs[selectedPgm]);
                lv.Adapter = psla;
            }
            catch { }
        }

        public async Task GetPrograms()
        {
            try
            {
                HttpClient client = new HttpClient();

                System.Uri uri = new System.Uri("http://" + Common.Instance.LocsiHostname + "/getprograms");
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();


                    ProgramClassForJSON dd = JsonConvert.DeserializeObject<ProgramClassForJSON>(content);
                    programs = dd.programs;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(1000);
                await GetPrograms();
            }
            
        }

        public async Task SavePrograms()
        {
            ProgramClassForJSON dd = new ProgramClassForJSON();
            dd.programs = programs;
            string jsonString = JsonConvert.SerializeObject(dd);
            HttpClient client = new HttpClient();
            System.Uri uri = new Uri("http://" + Common.Instance.LocsiHostname + "/setprograms");
            HttpContent content = new StringContent(jsonString);
            HttpResponseMessage resp = await client.PostAsync(uri, content);
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