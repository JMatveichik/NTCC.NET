using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
    public enum MessageType
    {
        /// <summary>
        /// Информационные сообщения
        /// </summary>
        Info        = 0,
        /// <summary>
        /// Отладочные  сообщения
        /// </summary>
        Debug       = 1,
        /// <summary>
        /// Предупредительные сообщения
        /// </summary>
        Warning     = 2,
        /// <summary>
        /// Сообщения об ошибках процесса
        /// </summary>
        Error       = 3,
        /// <summary>
        /// Исключительны ситуации
        /// </summary>
        Exception   = 4,
    }
}
