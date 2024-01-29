﻿using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NTCC.NET.Core.Stages
{
  public class StageMain : StageBase
  {
    public StageMain(string id) : base(id)
    {
      Title = "Технолощгический цикл";
      Description = "Последовательное циклическое выполнение всех стадий";
    }

    public List<StageBase> Stages
    {
      get;
      private set;
    } = new List<StageBase>();

    public int CurrentCycle
    {
      get { return currentCycle; }
      set
      {
        if (value == currentCycle)
          return;

        currentCycle = value;
        OnPropertyChanged();
      }
    }
    private int currentCycle = 0;


    /// <summary>
    /// Действия при подготовке к стадии
    /// </summary>
    /// <returns></returns>
    public override StageResult Prepare()
    {
      OnTick($"Подготовка стадии  : {Title} ...", MessageType.Info);

      var dataPoints = ArtMonbatFacility.DataPoints;

      /*
      YA9 (аварийная продувка  азотом, нормально откры-тый) – закрыть до конца всех стадий
      YA1.1 открыт (пневмоцилиндр скребка вверх)
      датчик верхнего положения скребка CS01(?) - ON
      YA7(подача азота в ресивер) открыт
      Проверяется состояние датчика уровня воды М06.1 в увлажнителе. 
      Если OFF, то открывается клапан подачи воды YA11 до тех пор, пока М06.1 не будет ON.
      Остальные клапаны закрыты
      Контролируется датчик верхнего положения штоков + см. примечания в конце таблицы
      */

      #region Подготовка клапанов
      //Открыть клапан YA4 продувки шкафа электрического
      DataPointHelper.SetDiscreteParameter(this, "YA04.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA5 подачи воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA6 подачи азота в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Открыть клапан YA7 подачи азота в ресивер
      DataPointHelper.SetDiscreteParameter(this, "YA07.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA8 подачи азота в скребок
      DataPointHelper.SetDiscreteParameter(this, "YA08.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA09 аварийной продувки реактора (нормально открытый)
      DataPointHelper.SetDiscreteParameter(this, "YA09.CLS", true, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA10 продувки азотом регулятора расхода газа
      DataPointHelper.SetDiscreteParameter(this, "YA10.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA11 подачи воды в увлажнитель воздуха
      DataPointHelper.SetDiscreteParameter(this, "YA11.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Клапан YA13 подачи пропан-бутана в подогреватель
      DataPointHelper.SetDiscreteParameter(this, "YA13.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA14 подачи воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //Закрыть клапан YA15 подачи азота в тару
      DataPointHelper.SetDiscreteParameter(this, "YA15.OPN", false, (int)OperationDelay.TotalMilliseconds);

      #endregion

      #region Подготовка увлажнителя воздуха

      //Проверяется состояние датчика уровня воды М06.1 в увлажнителе. 
      if (DataPointHelper.CheckDiscreteParameter("M06.1", false))
      {
        //открывается клапан подачи воды YA11 
        DataPointHelper.SetDiscreteParameter(this, "YA11.OPN", true);

        //ожидаем заполнение увлажнителя
        DataPointHelper.WaitDiscreteParameterSet(this, "M06.1", true, TimeSpan.FromSeconds(60));

        //Закрывается клапан подачи воды YA11
        DataPointHelper.SetDiscreteParameter(this, "YA11.OPN", false);
      }

      #endregion

      #region Подготовка системы удаления депозита
      //Управление линейным модулем 1 скребка - открыть
      DataPointHelper.SetDiscreteParameter(this, "YA01.1", true, (int)OperationDelay.TotalMilliseconds);

      //Управление линейным модулем 2 скребка - закрыть
      DataPointHelper.SetDiscreteParameter(this, "YA01.2", false, (int)OperationDelay.TotalMilliseconds);

      //Ожидание верхнего положения скребка
      DataPointHelper.WaitDiscreteParameterSet(this, "CS01", true, TimeSpan.FromSeconds(30));

      //Зажать уплотнения штоков 
      DataPointHelper.SetDiscreteParameter(this, "YA02", true, (int)OperationDelay.TotalMilliseconds);


      //Проверка состояния привода ворошителя M1
      if (DataPointHelper.CheckDiscreteParameter("M01.ON", true))
      {
        //Выключение привода ворошителя M1
        DataPointHelper.SetDiscreteParameter(this, "M01.RUN", false, (int)OperationDelay.TotalMilliseconds);
      }

      //Проверка состояния привода шнека M1
      if (DataPointHelper.CheckDiscreteParameter("M02.ON", true))
      {
        //Выключение привода ворошителя M2
        DataPointHelper.SetDiscreteParameter(this, "M02.RUN", false, (int)OperationDelay.TotalMilliseconds);
      }

      //Проверка состояния привода  M3
      if (DataPointHelper.CheckDiscreteParameter("M03.ON", true))
      {
        //Выключение привода  M3
        DataPointHelper.SetDiscreteParameter(this, "M03.RUN", false, (int)OperationDelay.TotalMilliseconds);
      }

      #endregion

      #region Подготовка нагревателей            

      List<ReactorHeatingZone> reactorHeatingZones = ArtMonbatFacility.ReactorZones.Items.Values.ToList();

      //сбросить все параметры нагрева для всех зон
      foreach (var heater in reactorHeatingZones)
      {
        heater.SetupControl(0.0, 0.0, 0.0, 0.0);
        heater.StartControl();
      }

      //Подать питание на внутренний нагреватель (EK1.1. EK1.2. EK2)
      DataPointHelper.SetDiscreteParameter(this, "HT01.QF.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //Подать питание на наружный нагреватель (EK3. EK7) вверху и снизу
      DataPointHelper.SetDiscreteParameter(this, "HT02.QF.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //Подать питание на наружный нагреватель(EK4.EK5.EK6) по центру
      DataPointHelper.SetDiscreteParameter(this, "HT03.QF.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //Подать питание на нагреватель газа (EK8. EK9)
      DataPointHelper.SetDiscreteParameter(this, "HT04.QF.RUN", true, (int)OperationDelay.TotalMilliseconds);

      #endregion

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      #region Подготовка нагревателей            

      List<ReactorHeatingZone> reactorHeatingZones = ArtMonbatFacility.ReactorZones.Items.Values.ToList();

      //сбросить все параметры нагрева для всех зон
      foreach (var heater in reactorHeatingZones)
      {
        heater.SetupControl(0.0, 0.0, 0.0, 0.0);
        heater.StopControl();
      }

      //Снять питание на внутренний нагреватель (EK1.1. EK1.2. EK2)
      DataPointHelper.SetDiscreteParameter(this, "HT01.QF.RUN", false, (int)OperationDelay.TotalMilliseconds);

      //Снять питание на наружный нагреватель (EK3. EK7) вверху и снизу
      DataPointHelper.SetDiscreteParameter(this, "HT02.QF.RUN", false, (int)OperationDelay.TotalMilliseconds);

      //Снять питание на наружный нагреватель(EK4.EK5.EK6) по центру
      DataPointHelper.SetDiscreteParameter(this, "HT03.QF.RUN", false, (int)OperationDelay.TotalMilliseconds);

      //Снять питание на нагреватель газа (EK8. EK9)
      DataPointHelper.SetDiscreteParameter(this, "HT04.QF.RUN", false, (int)OperationDelay.TotalMilliseconds);

      #endregion
      CurrentStage = null;
      return StageResult.Successful;
    }

    protected override StageResult Main(CancellationToken cancel)
    {
      StageResult result = StageResult.Successful;

      foreach (var stage in Stages)
      {
        result = stage.Do(this);

        if (result != StageResult.Successful)
          break;
      }

      return result;
    }
  }
}