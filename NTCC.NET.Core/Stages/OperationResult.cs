namespace Dispergator.Common.Stages
{
    public enum OperationResult
    {
        /// <summary>
        /// Удачное завершение операции
        /// </summary>
        Suсcessful = 0,

        /// <summary>
        /// Завершение операции по ошибке
        /// </summary>
        Failed = 1,

        /// <summary>
        /// Операция завершена по таймауту
        /// </summary>
        Timeout = 2,

        /// <summary>
        /// Операция первана пользователем
        /// </summary>
        Breaked = 3,

        /// <summary>
        /// Операция пропущена пользователем
        /// </summary>
        Skipped = 4,

        /// <summary>
        /// Завершение с программной ошибкой
        /// </summary>
        Excepted = 5

    }
}
