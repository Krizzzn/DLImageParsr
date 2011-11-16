using System;
using DlImageParsr.Model;

namespace DlImageParsr.Contracts
{
    public interface IImageParser
    {
        System.Collections.Generic.IEnumerable<Pixel> ReadDocument();
    }
}
