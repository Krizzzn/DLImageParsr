using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr.Model
{
    public class Dive
    {
        public Dive(int id, int maxDepthInCentimeters, int durationInSeconds, string imagePath) { 
            DiveLogId = id;
            DurationinSeconds = durationInSeconds;
            MaxDepthInCentimeters = maxDepthInCentimeters;
            ImagePath = imagePath;
        }

        public int DurationinSeconds { get; private set; }

        public int MaxDepthInCentimeters { get; private set; }

        public List<Sample> Samples { get; private set; }

        public int DiveLogId { get; private set; }

        public string ImagePath { get; private set; }
    }

}
