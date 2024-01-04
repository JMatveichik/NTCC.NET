using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс выполнения полного цикла
    /// </summary>
    public class StageFullCycle : StageBase
    {

        public StageFullCycle(string id) : base(id)
        {
            
        }

        #region РЕАЛИЗАЦИЯ ФУНКЦИЙ ШАБЛОННОГО МЕТОДА 

        protected override StageResult Prepare()
        {
            throw new NotImplementedException();
            //return StageState.Suсcessful;
        }
        
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
            /*
            while (true)
            {

                //заполнение 
                StageState r = FillUp.Do(this);
                if (r != StageState.Suсcessful)
                    return r;

                //перемешивание
                r = Mixing.Do(this);
                if (r != StageState.Suсcessful)
                    return r;


                //обработка
                r = Treatment.Do(this);
                if (r != StageState.Suсcessful)
                    return r;

                //слив
                r = Drain.Do(this);
                if (r != StageState.Suсcessful)
                    return r;
            }
            */

        }

        protected override StageResult Finalization()
        {
            throw new NotImplementedException();

            //Остановка 
            //StageState r = Stop.Do(this);
            //return r;
        }

        #endregion
    }
}
