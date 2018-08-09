using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Services
{
    public interface IAnalysisRun
    {
        Task RunAnalysis(int id);
    }
}
