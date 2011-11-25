using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlImageParsr.Model;
using DlImageParsr.Contracts;

namespace DlImageParsr.Orchestration
{
    public class SampleFactory : ISampleFactory
    {
        public SampleFactory()
        {
            this._postProcessingFilters = new List<IPostProcessing>();
        }

        public ProcessedDive Create(IEnumerable<Pixel> pixels, Dive dive)
        {
            List<Pixel> listOfPixels = pixels.ToList();
            var pDive = new ProcessedDive(dive);

            var depthRes = GetDepthResolution(listOfPixels, dive);
            var timeRes = GetTimeResolution(listOfPixels, dive);

            var origin = listOfPixels[0];
            listOfPixels.RemoveAll(p => p is SkipPixel);
            listOfPixels.ConvertAll(pixel => new Sample((pixel.Y - origin.Y) * depthRes, (pixel.X - origin.X) * timeRes)).ForEach(pDive.Samples.Add);

            PostProcessingFilters.ToList().ForEach(p => { p.Process(pDive); });

            return pDive;
        }

        public void RemoveLastPlainFromPixelList(List<Pixel> listOfPixels)
        {
            if (listOfPixels == null)
                return;

            int foundindex = 0;
            for (int i = 1; i < listOfPixels.Count; i++) {
                if (listOfPixels[foundindex].Y != listOfPixels[i].Y)
                    foundindex = i;
            }

            while (listOfPixels.Count > foundindex + 1)
                listOfPixels.RemoveAt(listOfPixels.Count - 1);
        }

        public int GetDepthResolution(List<Pixel> listOfPixels, Dive dive)
        {
            if (listOfPixels == null || listOfPixels.Count == 0)
                return -1;

            var origin = listOfPixels[0];
            var maxDepthInPixels = listOfPixels.Max(p => p.Y - origin.Y);

            var inCentimeters = ((float)dive.MaxDepthInCentimeters / (float)maxDepthInPixels);
            return Convert.ToInt32(Math.Round(inCentimeters));
        }

        public int GetTimeResolution(List<Pixel> listOfPixels, Dive dive)
        {
            RemoveLastPlainFromPixelList(listOfPixels);
            return (int)Math.Round((float)dive.DurationinSeconds / ((float)listOfPixels.Count - 1));
        }

        private List<IPostProcessing> _postProcessingFilters;
        public IEnumerable<IPostProcessing> PostProcessingFilters
        {
            get { return _postProcessingFilters; }
        }

        public void AddPostProcessing(IPostProcessing postProcessing)
        {
            this._postProcessingFilters.Add(postProcessing);
        }
    }
}

