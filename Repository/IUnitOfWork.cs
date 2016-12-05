using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapperOrm.Models;

namespace MapperOrm.Repository
{
    internal interface IDetector
    {
        Dictionary<string, IObjectTracker> ObjectDetector { get; }
    }

    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
