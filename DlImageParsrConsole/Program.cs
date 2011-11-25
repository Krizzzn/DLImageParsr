using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Orchestration;

namespace DlImageParsrConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleDiveRepsitory repo = new SampleDiveRepsitory();

            var machine = new Machine(repo);
            machine.ProcessMessage += Console.WriteLine;
            machine.BeforeSavingDive += new Action<DlImageParsr.Model.ProcessedDive>(ProcessedDiveOutput);

            machine.Process();

            Console.ReadKey();
        }

        static void ProcessedDiveOutput(DlImageParsr.Model.ProcessedDive obj)
        {
            Console.WriteLine("############## DiveId {0}", obj.DiveLogId);
            Console.WriteLine("with {0} samples", obj.Samples.Count);
            Console.WriteLine("with {0} samples", obj.Samples.Count);

            obj.Samples.ForEach(sample => { Console.WriteLine("depth (in cm) after {0} seconds: {1}", sample.SecondsSinceStart, sample.Depth); });
        }
    }
}
