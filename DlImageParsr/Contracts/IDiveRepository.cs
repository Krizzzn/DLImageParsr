using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Model;

namespace DlImageParsr.Contracts
{
    public interface IDiveRepository
    {
        IEnumerable<Dive> LoadDives();

        void SaveDive(ProcessedDive dive);
    }
}
