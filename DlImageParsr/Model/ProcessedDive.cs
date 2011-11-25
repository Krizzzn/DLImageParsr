using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr.Model
{
    public class ProcessedDive : Dive
    {
        public ProcessedDive(Dive dive)
            : base(dive.DiveLogId, dive.MaxDepthInCentimeters, dive.DurationinSeconds, dive.ImagePath)
        {
            Samples = new List<Sample>();
        }

        public List<Sample> Samples { get; private set; }

        public int SampleRateInSeconds { get {
            if (Samples.Count == 0)
                return 0;
            return DurationinSeconds / Samples.Count;
        } }
    }
}
