using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
    public enum StageResult
    {
        /// <summary>
        /// Стадия завершилась корректно
        /// </summary>
        Successful = 0,

        /// <summary>
        /// Стадия прервана из-за возникновения аварийной ситуации в технологическом процессе
        /// </summary>
        Failed = 1,

        /// <summary>
        /// Стадия остановлена пользователем (останавливается выполнение технологического цикла )
        /// </summary>
        Stopped = 2,

        /// <summary>
        /// Стадия пропущена пользователем (текущая стадия будет остановлена, технологический цикл будет продолжен)
        /// </summary>
        Skipped = 3,

        /// <summary>
        /// Выполнение стадии завершено по программной ошибке 
        /// </summary>
        Excepted = 4
    }
}
