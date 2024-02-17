namespace NTCC.NET.Core.Stages
{
  public enum StageState
  {
    /// <summary>
    /// Стадия в ожидании начала выполнени
    /// </summary>
    Wait = 0,

    /// <summary>
    /// Стадия закончила подготовку к выполнению
    /// </summary>
    Prepeared = 1,

    /// <summary>
    /// Начато выполнение основного алгоритма стадии
    /// </summary>
    Started = 2,

    /// <summary>
    /// Выполнение стадии завершено с ошибкой
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Стадия завершила основной алгоритм работы
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Выполнение стадии первано пользователем
    /// </summary>
    Stopped = 5,

    /// <summary>
    /// Стадия завершена
    /// </summary>
    Finalized = 6,

    /// <summary>
    /// Стадия пропущена пользователем
    /// </summary>
    Skipped = 7,

    /// <summary>
    /// Возникла программная ошибка при выполнении стадии
    /// </summary>
    Excepted = 8
  }

}
