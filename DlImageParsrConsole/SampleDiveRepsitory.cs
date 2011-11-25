using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Contracts;
using DlImageParsr.Model;

namespace DlImageParsrConsole
{
    public class SampleDiveRepsitory : IDiveRepository
    {
        public IEnumerable<DlImageParsr.Model.Dive> LoadDives()
        {
            var dives = new Dive[] { new Dive(1, 1000, 500, "testimage1.png"), new Dive(2, 500, 500, "testimage1.png") };
            return dives;
        }

        public void SaveDive(DlImageParsr.Model.ProcessedDive dive)
        {
            Console.WriteLine("Sorry, me can not save!");
        }
    }
}
