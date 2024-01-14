using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
    class StagePropane : StageBase
    {
        public StagePropane(string id) : base(id)
        {

        }

        public override StageResult Prepare()
        {
            throw new NotImplementedException();
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
