using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr.Model
{
    public struct Sample
    {
        public Sample(int depth, int secondsSinceStart) : this ()
        {
            Depth = depth;
            SecondsSinceStart = secondsSinceStart;
        }

        public int Depth { get; private set; }
        public int SecondsSinceStart { get; private set; }
    }
}
