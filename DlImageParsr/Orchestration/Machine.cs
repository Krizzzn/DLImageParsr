using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Contracts;

namespace DlImageParsr.Orchestration
{
    public class Machine
    {
        public ISampleFactory SampleFactory { get; private set; }
        public IImageParser ImageParser { get; private set; }
        public IDiveRepository DiveRepository { get; private set; }
    }
}
