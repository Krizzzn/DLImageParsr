using System;
using DlImageParsr.Model;

namespace DlImageParsr.Contracts
{
    public interface IImageParser : IDisposable
    {
        System.Collections.Generic.IEnumerable<Pixel> ReadDocument();
    }
}
