using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Contracts;
using DlImageParsr.Model;

namespace DlImageParsr.Filter
{
    public class ReduceSamples : IPostProcessing
    {
        public ReduceSamples() { }

        public void Process(ProcessedDive dive)
        {
            if (dive == null)
                throw new ArgumentNullException();

            dive.Samples.Where((sample, index) => index % 2 == 1).ToList().ForEach(sample => { dive.Samples.Remove(sample); });
        }
    }
}
