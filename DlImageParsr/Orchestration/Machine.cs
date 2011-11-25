using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Contracts;
using DlImageParsr.Model;
using DlImageParsr.Filter;

namespace DlImageParsr.Orchestration
{
    public class Machine
    {
        public Machine(IDiveRepository diveRepository) : this(diveRepository, new SampleFactory(), ImageParsing.ImageParser.GetImageParserForDive){
            ((SampleFactory)this.SampleFactory).AddPostProcessing(new ReduceSamples());
        }

        public Machine(IDiveRepository diveRepository, ISampleFactory sampleFactory, Func<Dive, IImageParser> parserFactoryMethod)
        {
            if (diveRepository == null)
                throw new ArgumentNullException("diveRepository");
            if (sampleFactory == null)
                throw new ArgumentNullException("sampleFactory");
            if (parserFactoryMethod == null)
                throw new ArgumentNullException("parserFactoryMethod");

            SampleFactory = sampleFactory;
            DiveRepository = diveRepository;
            ImageParserFactory = parserFactoryMethod;
        }

        public ISampleFactory SampleFactory { get; private set; }
        public IDiveRepository DiveRepository { get; private set; }
        public Func<Dive, IImageParser> ImageParserFactory { get; set; }
        public event Action<string> ProcessMessage;
        public event Action<ProcessedDive> BeforeSavingDive;

        public void Process()
        {
            var theDives = DiveRepository.LoadDives();

            foreach (var dive in theDives.Where(d => d != null)) {
                RaiseMessage(string.Format("Parsing Dive {0}", dive.DiveLogId));

                using (var imgParser = ImageParserFactory(dive))
                {
                    var pixels = imgParser.ReadDocument();

                    if (pixels == null || pixels.Count() == 0)
                        continue;
                    var processesdDive = SampleFactory.Create(pixels, dive);
                    if (processesdDive != null)
                    {
                        if (BeforeSavingDive != null) BeforeSavingDive(processesdDive);
 
                        RaiseMessage(string.Format("Returned Dive {0} with {1} samples", processesdDive.DiveLogId, processesdDive.Samples.Count));
                        DiveRepository.SaveDive(processesdDive);
                    }
                }
            }
        }

        private void RaiseMessage(string message)
        {
            if (ProcessMessage != null)
                ProcessMessage(message);
        }
    }
}
