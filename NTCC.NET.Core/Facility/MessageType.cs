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
        /// Трассировочное сообщение
        /// </summary>
        Trace = 0,
        /// <summary>
        /// Отладочные  сообщения
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Информационные сообщения
        /// </summary>
        Info = 2,
        /// <summary>
        ////Сообщения об удачном дейтсвии
        /// </summary>
        Success     = 3,
        /// <summary>
        /// Предупредительные сообщения
        /// </summary>
        Warning     = 4,
        /// <summary>
        /// Сообщения об ошибках процесса
        /// </summary>
        Error       = 5,
        /// <summary>
        /// Исключительны ситуации
        /// </summary>
        Exception   = 6,
    }
}
