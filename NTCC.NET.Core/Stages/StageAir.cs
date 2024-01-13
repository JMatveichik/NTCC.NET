using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NTCC.NET.Core.Stages
{
    public class StageAir : StageBase
    {
        public StageAir(string id) : base(id)
        {
        }

        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}