using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DlImageParsr.Model
{
    public interface IDiveRepository
    {
        IEnumerable<Dive> LoadDives();

        void SaveDive(Dive dive);
    }
}
