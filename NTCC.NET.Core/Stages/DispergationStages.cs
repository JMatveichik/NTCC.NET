using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    public static class DispergationStages
    {      
            
        /// Полный цикл работы
        public static StageBase FullCycle
        {
            get;
            private set;
        } = new StageFullCycle("StageFullCycle");


        /// Стадия подготовки установки
        public static StageBase Initialize
        {
            get;
            private set;
        } = new StageInitialize("StageInitialize");


        /// Стадия слива продукта        
        public static StageBase Drain
        {
            get;
            private set;
        } = new StageDrain("StageDrain");

        /// Стадия слива продукта  в отходы      
        public static StageBase DrainToRecycle
        {
            get;
            private set;
        } = new StageDrainToRecycle("StageDrainToRecycle");

        /// Стадия слива продукта  во внешнюю емкость      
        public static StageBase DrainToExternalTank
        {
            get;
            private set;
        } = new StageDrainToExternalTank("StageDrainToExternalTank");

        /// Стадия заливки рабочей емкости
        public static StageBase FillUp
        {
            get;
            private set;
        } = new StageFillUp("StageFillUp");

        /// Стадия перемешивания пасты
        public static StageBase Mixing
        {
            get;
            private set;
        } = new StageMixing("StageMixing");

        /// Стадия обработки пасты
        public static StageBase Treatment
        {
            get;
            private set;
        } = new StageTreatment("StageTreatment");


        /// Стадия остановки
        public static StageBase Stop
        {
            get;
            private set;
        } = new StageStop("StageStop");


    }
}
