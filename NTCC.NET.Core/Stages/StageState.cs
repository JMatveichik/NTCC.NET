namespace NTCC.NET.Core.Stages
{
  public enum StageState
  {
    /// <summary>
    /// Стадия в ожидании начала выполнени
    /// </summary>
    Wait = 0,

    /// <summary>
    /// Начата подготовка стадии к выполнению
    /// </summary>
    Prepearing = 1,

    /// <summary>
    /// Стадия закончила подготовку к выполнению
    /// </summary>
    Prepeared = 2,

    /// <summary>
    /// Ошибка при подготовке стадии к выполнению
    /// </summary>
    PrepeareFailed = 3,

    /// <summary>
    /// Начато выполнение основного алгоритма стадии
    /// </summary>
    Started = 4,

    /// <summary>
    /// Выполнение стадии завершено с ошибкой
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Стадия завершила основной алгоритм работы
    /// </summary>
    Complete = 6,

    /// <summary>
    /// Выполнение стадии первано пользователем
    /// </summary>
    Breaked = 7,

    /// <summary>
    /// Начато завершение стадии
    /// </summary>
    Finalizing = 8,

    /// <summary>
    /// Стадия завершена
    /// </summary>
    Finalized = 9,

    /// <summary>
    /// Стадия пропущена пользователем
    /// </summary>
    Skipped = 10,

    /// <summary>
    /// Возникла программная ошибка при выполнении стадии
    /// </summary>
    Excepted = 11
  }

}
