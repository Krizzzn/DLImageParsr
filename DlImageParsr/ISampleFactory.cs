using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Model;

namespace DlImageParsr
{
    public interface ISampleFactory
    {
        public void Create(Dive dive, IEnumerable<Pixel> pixels);
    }
}
