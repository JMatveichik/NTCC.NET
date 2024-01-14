using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
    public class StageHeating : StageBase
    {
        public StageHeating(string id) : base(id)
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
            return StageResult.Failed;
        }

        
    }
}
