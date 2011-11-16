using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Contracts;
using DlImageParsr.Model;

namespace DlImageParsr.Orchestration
{
    public class Machine
    {
        public Machine(IDiveRepository diveRepository, ISampleFactory sampleFactory)
        {
            if (diveRepository == null)
                throw new ArgumentNullException("diveRepository");
            if (sampleFactory == null)
                throw new ArgumentNullException("sampleFactory");

            SampleFactory = sampleFactory;
            DiveRepository = diveRepository;
        }

        public ISampleFactory SampleFactory { get; private set; }
        public IDiveRepository DiveRepository { get; private set; }

        public Func<Dive, IImageParser> ImageParserFactory { get; set; }

        public void Process()
        {
            DiveRepository.LoadDives();
        }
    }
}
