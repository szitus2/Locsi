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
using Android.Graphics;

namespace Locsi
{
    class ProgramStepLineAdapter : ArrayAdapter<ProgramStepClass>
    {
        private ProgramClass pgm;
        private Context ctx;
                
        public ProgramStepLineAdapter(Context context, ProgramClass program) : base(context, -1, program.steps)
        {
            this.pgm = program;
            this.ctx = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            LayoutInflater inflater = (LayoutInflater)(ctx.GetSystemService(Context.LayoutInflaterService));
            View rowView = inflater.Inflate(Resource.Layout.ProgramStepLayout, parent, false);
            View emptyView = inflater.Inflate(Resource.Layout.EmptyLayout, parent, false);
            if (pgm.steps[position] == null) { return emptyView; }
            if (pgm.numSteps <= position) { return emptyView; }
            TextView stepText = rowView.FindViewById<TextView>(Resource.Id.pslStepNum);
            TextView stationText = rowView.FindViewById<TextView>(Resource.Id.pslStation);
            TextView durationText = rowView.FindViewById<TextView>(Resource.Id.pslDuration);
            stepText.SetText((position + 1).ToString() +". Fázis", TextView.BufferType.Normal);
            stationText.SetText(pgm.steps[position].station.ToString() + ". Zóna", TextView.BufferType.Normal);
            durationText.SetText(pgm.steps[position].duration.ToString() + " perc", TextView.BufferType.Normal);
            ListView lv = parent as ListView;
            
            return rowView;
        }

         
    }
}