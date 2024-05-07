using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class Scrapper : FacilityElement
  {
    public Scrapper(string id) : base(id)
    {
      Description = "Управление скребком";
      Title = "Система удаления депозита";
    }

    public void SetupControl( string valveMoveDownId, 
                              string valveMoveUpId, 
                              string bottomPositionId, 
                              string topPositionId,
                              string sealsControlId,
                              string nitroValveId)
    {
      ValveMoveDown = ArtMonbatFacility.DataPoints[valveMoveDownId] as DiscreteOutputDataPoint;
      if (ValveMoveDown == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{valveMoveDownId}» для перемещения скребка в нижнее положение.");

      ValveMoveUp = ArtMonbatFacility.DataPoints[valveMoveUpId] as DiscreteOutputDataPoint;
      if (ValveMoveDown == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{valveMoveUpId}» для перемещения скребка в верхнее положение.");

      BottomPositionSensor = ArtMonbatFacility.DataPoints[bottomPositionId] as DiscreteDataPoint;
      if (BottomPositionSensor == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{bottomPositionId}» для определения нижнего положения скребка.");

      TopPositionSensor = ArtMonbatFacility.DataPoints[topPositionId] as DiscreteDataPoint; 
      if (TopPositionSensor == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{topPositionId}» для определения верхнего положения скребка.");

      ScrapperSealsValve = ArtMonbatFacility.DataPoints[sealsControlId] as DiscreteOutputDataPoint;
      if (ScrapperSealsValve == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{sealsControlId}» для управления пневмоцилиндром уплотнения штоков скребка.");

      ScrapperNitroValve = ArtMonbatFacility.DataPoints[nitroValveId] as DiscreteOutputDataPoint;
      if (ScrapperNitroValve == null)
        throw new ArgumentNullException($"Не найден дискретный источник данных «{nitroValveId}» для управления подачей азота на пальцы скребка.");

    }

    /// <summary>
    /// Таймаут перемещения скребка вниз
    /// </summary>
    public TimeSpan MoveDownTimeOut
    {
      get;
      set;
    } = TimeSpan.FromSeconds(5.0);

    /// <summary>
    /// Таймаут перемещения скребка вверх
    /// </summary>
    public TimeSpan MoveUpTimeOut
    {
      get;
      set;
    } = TimeSpan.FromSeconds(5.0);


    #region DataPoints for Scrapper

    /// <summary>
    /// Точка данных для управления пневмоцилиндром линейного модуля скребка для перемещения вниз
    /// </summary>
    public DiscreteOutputDataPoint ValveMoveDown
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для управления пневмоцилиндром линейного модуля скребка для перемещения вверх
    /// </summary>
    public DiscreteOutputDataPoint ValveMoveUp
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для определения нижнего положения скребка
    /// </summary>
    public DiscreteDataPoint BottomPositionSensor
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для определения верхнего положения скребка
    /// </summary>
    public DiscreteDataPoint TopPositionSensor
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для управления подачей азота на пальцы скребка
    /// </summary>
    public DiscreteOutputDataPoint ScrapperNitroValve
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для управления пневмоцилиндром уплотнения штоков скребка
    /// </summary>
    public DiscreteOutputDataPoint ScrapperSealsValve
    {
      get;
      private set;
    } = null;

    #endregion

    #region Scrapper move methods

    /// <summary>
    /// Oжидания прохода скребка в одном напровлении
    /// </summary>
    /// <param name="positionDataPoint">Датчик положения скребка, который будет проверяться для подтверждения завершения перемещения </param>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка к выбранной позиции</param>
    /// <returns>true - если скребок достиг заданной точки , false - если скребок застрял</returns>
    private bool WaitScraperPosition(DiscreteDataPoint positionDataPoint, TimeSpan moveTimeOut)
    {
      DateTime moveStart = DateTime.Now;
      TimeSpan moveTime = TimeSpan.FromMilliseconds(0);

      //ожидаем положения скребка или таймаута
      while (true)
      {
        Thread.Sleep(50);

        //обновляем время движения скребка
        moveTime = DateTime.Now - moveStart;

        //если превышено время ожидания положения скребка
        if (moveTime > moveTimeOut)
          return false;

        if (positionDataPoint.State)
          return true;
      }
    }

    /// <summary>
    /// Перемещение скребка в заданное положение
    /// </summary>
    /// <param name="moveDown"> Направление перемещения (true - вниз, false - вверх)</param>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка к выбранной позиции</param>
    /// <returns>true - если скребок достиг заданной точки , false - если скребок застрял</returns>
    private bool MoveScraperTo(bool moveDown, TimeSpan moveTimeOut)
    {
      //перемещение скребка вниз
      if (moveDown)
      {
        //отключаем линейный модуль скребка #1
        ValveMoveUp.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //включаем линейный модуль скребка #2 - движение вниз
        ValveMoveDown.SetState(true);

        //технологическая задержка
        Thread.Sleep(100);

        //подача азота в сребок если движемся вниз
        ScrapperNitroValve.SetState(true);

        //если не достигли нижнего положения ошибка
        if (!WaitScraperPosition(BottomPositionSensor, moveTimeOut))
          return false;

      }
      else
      {
        //снятие подачи азота в сребок если движемся вверх
        ScrapperNitroValve.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //выключаем линейный модуль скребка #2
        ValveMoveDown.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //включаем линейный модуль скребка #1 - движение вверх
        ValveMoveUp.SetState(true);

        //если не достигли верхнего положения ошибка
        if (!WaitScraperPosition(TopPositionSensor, moveTimeOut))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Перемещение скребка вниз
    /// </summary>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка</param>
    /// <returns>true - если скребок достиг нижнего положения в пределах заданного времени, false - если скребок застрял</returns>
    public bool MoveScraperDown(TimeSpan moveTimeOut)
    {
      return MoveScraperTo(true, moveTimeOut);
    }

    /// <summary>
    /// Перемещение скребка вниз с таймаутом по умолчанию 
    /// </summary>
    /// <returns>true - если скребок достиг нижнего положения в пределах заданного времени, false - если скребок застрял</returns>
    private bool MoveScraperDown()
    {
      //Оповещение о перемещении скребка в нижнее положение
      OnTick($"Попытка перемещения скребка в нижнее положение...", MessageType.Info);
      return MoveScraperTo(true, MoveDownTimeOut);
    }

    /// <summary>
    /// Перемещение скребка вверх
    /// </summary>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка</param>
    /// <returns>True - если скребок достиг верхнего положения в пределах заданного времени, false - если скребок застрял</returns>
    public bool MoveScraperUp(TimeSpan moveTimeOut)
    {
      return MoveScraperTo(false, moveTimeOut);
    }

    /// <summary>
    /// Перемещение скребка вверх с таймаутом по умолчанию 
    /// </summary>
    /// <returns>true - если скребок достиг нижнего положения в пределах заданного времени, false - если скребок застрял</returns>
    public bool MoveScraperUp()
    {
      //оповещение о перемещении скребка в верхнее положение
      OnTick($"Попытка перемещения скребка в верхнее положение...", MessageType.Info);
      return MoveScraperTo(false, MoveDownTimeOut);
    }

    /// <summary>
    /// Процедура прогонки скребка вниз и вверх
    /// </summary>
    /// <returns></returns>
    public bool MakePass()
    {
      try
      {        
        //перемещение скребка в нижнее положение
        if (!MoveScraperDown())
        {
          throw new TimeoutException("Скребок не достиг нижнего положения!");          
        }
        else
        {
          OnTick($"Скребок достиг ниженего положения.", MessageType.Success);
        }

        if (!MoveScraperUp())
        {
          throw new TimeoutException("Скребок не достиг верхнего положения!");
        }
        else 
        {
          OnTick($"Скребок вернулся в верхнее положение.", MessageType.Success);
        }        

        return true; 
      }
      catch (TimeoutException ex)
      {
        OnTick($"Ошибка при перемещении скребка : {ex.Message}. Возвращаем скребок в верхнее положение.", MessageType.Exception);
        
        //при ошибке возвращаем скребок в исходное положение (верхнее положение)
        MoveScraperUp();
        return false;
      }
    }

    /// <summary>
    /// Процедура отжига скребка
    /// </summary>
    /// <returns></returns>
    public bool BurnOut(TimeSpan burnMoveTime)
    {
      OnTick("Запуск отжига скребка", MessageType.Info);

      //отключаем пневмоцилиндр уплотнения штоков скребка
      OnTick("Отключаем пневмоцилиндр уплотнения штоков скребка...", MessageType.Debug);
      ScrapperSealsValve.SetState(false);

      //отжиг без подачи  азота в сребок 
      OnTick("Отключаем пневмоцилиндр подачи азота в скребок...", MessageType.Debug);
      ScrapperNitroValve.SetState(false);

      //отключаем линейный модуль скребка #1
      OnTick("Отключаем цилиндр перемещения скребка вверх", MessageType.Debug);
      ValveMoveUp.SetState(false);

      //технологическая задержка
      Thread.Sleep(50);

      //включаем линейный модуль скребка #2 - движение вниз
      OnTick("Включаем цилиндр перемещения скребка вниз", MessageType.Debug);
      ValveMoveDown.SetState(true);

      //технологическая задержка для движения скребка вниз
      OnTick($"Перемещаем скребок в горячую зону на {burnMoveTime.TotalMilliseconds} ms", MessageType.Debug); ;
      Thread.Sleep(burnMoveTime);

      //отключаем линейный модуль скребка #2
      OnTick("Отключаем цилиндр перемещения скребка вниз", MessageType.Debug);
      ValveMoveDown.SetState(false);

      //технологическая задержка
      Thread.Sleep(50);

      //включаем линейный модуль скребка #1 - движение вверх
      OnTick("Включаем цилиндр перемещения скребка вверх", MessageType.Debug);
      ValveMoveUp.SetState(true);

      //ожидаем положения скребка в верхнем положении
      if (!WaitScraperPosition(TopPositionSensor, MoveUpTimeOut))
      {
        OnTick("Ошибка при перемещении скребка в верхнее положение!", MessageType.Exception);
        return false;
      }

      //отключаем пневмоцилиндр уплотнения штоков скребка
      OnTick("Включаем пневмоцилиндр уплотнения штоков скребка...", MessageType.Debug);
      ScrapperSealsValve.SetState(true);

      //оповещение об окончании отжига скребка
      OnTick("Проход для отжига скребка завершен ...", MessageType.Success);
      return true;
    }

    #endregion
  }
}
