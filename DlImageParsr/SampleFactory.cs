using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Model;

namespace DlImageParsr
{
    public class SampleFactory : ISampleFactory
    {
        public SampleFactory(Dive dive)
        {
            Dive = dive;
        }

        public ProcessedDive Create(IEnumerable<Pixel> pixels)
        {
            List<Pixel> listOfPixels = pixels.ToList();
            var pDive = new ProcessedDive(Dive);

            var depthRes = GetDepthResolution(listOfPixels);
            var timeRes = GetTimeResolution(listOfPixels);

            listOfPixels.ConvertAll(pixel => new Sample(pixel.Y * depthRes, pixel.X * timeRes)).ForEach(pDive.Samples.Add);
            return pDive;
        }

        public Dive Dive { get; set; }

        public void RemoveLastPlainFromPixelList(List<Pixel> listOfPixels)
        {
            if (listOfPixels == null)
                return;

            int foundindex = 0;
            for (int i = 1; i < listOfPixels.Count; i++) {
                if (listOfPixels[foundindex].Y != listOfPixels[i].Y)
                    foundindex = i;
            }

            while (listOfPixels.Count > foundindex + 1) {
                listOfPixels.RemoveAt(listOfPixels.Count - 1);
            }
        }

        public int GetDepthResolution(List<Pixel> listOfPixels)
        {
            if (listOfPixels == null || listOfPixels.Count == 0)
                return -1;

            var maxDepthInPixels = listOfPixels.Max(p => p.Y);

            var inCentimeters = ((float)Dive.MaxDepthInCentimeters / (float)maxDepthInPixels);
            return Convert.ToInt32(Math.Round(inCentimeters));
        }

        public int GetTimeResolution(List<Pixel> listOfPixels)
        {
            RemoveLastPlainFromPixelList(listOfPixels);
            return (int)Math.Round((float)this.Dive.DurationinSeconds / ((float)listOfPixels.Count - 1));
        }
    }
}

