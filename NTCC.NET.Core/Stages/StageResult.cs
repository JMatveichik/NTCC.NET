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
        /// Стадия прервана из-за возникновения аварийной ситуации
        /// </summary>
        Failed = 1,

        /// <summary>
        /// Стадия прервана пользователем
        /// </summary>
        Breaked = 2,

        /// <summary>
        /// Стадия пропущена пользователем
        /// </summary>
        Skipped = 3,

        /// <summary>
        /// Выполнение стадии завершено по программной ошибке 
        /// </summary>
        Excepted = 4

    }
}
