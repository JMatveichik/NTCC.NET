using NTCC.NET.Core.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
    public class ArtMonbatRegisterInfo
    {
        public ArtMonbatRegisterInfo(ushort iAddress, ushort iSize)
        {
            RegisterAddress = iAddress;
            Size = iSize;
        }

        public ushort RegisterAddress
        {
            get; private set;
        }
        public ushort Size { get; private set; }
    }

    public class ArtMonbatChannelsMapper
    {

        private static readonly ArtMonbatChannelsMapper instance = new ArtMonbatChannelsMapper();

        public static ArtMonbatChannelsMapper Instance
        {
            get => instance;
        }

        private void Clear()
        {
            discreteInputsMap.Clear();
            discreteOutputsMap.Clear();
            analogInputsMap.Clear();
            analogOutputsMap.Clear();
        }

        public void Initialize(string xmlConfigPath) 
        {
            Clear();

            ConfigFileProcessor configFileProcessor = new ConfigFileProcessor();
            XDocument xmlDocument = configFileProcessor.GetXDocument(xmlConfigPath);

            foreach (var registersRange in xmlDocument.Descendants("RegistersRange"))
            {
                string type         = registersRange.Attribute("Type")?.Value;
                string startStrAddress = registersRange.Attribute("StartAddress")?.Value;
                string regStrCount     = registersRange.Attribute("Count")?.Value;
                string regStrSize      = registersRange.Attribute("Size")?.Value;

                if (string.IsNullOrEmpty(type))
                    throw new ArgumentException($"Не задан тип регистра.");

                ushort regStartAddress= 0;
                if (!ushort.TryParse(startStrAddress, out regStartAddress))
                    throw new ArgumentException($"Не верное задание адреса регистра для привязки ");

                ushort regCount = 0;
                if (!ushort.TryParse(regStrCount, out regCount))
                    throw new ArgumentException($"Не верное задание количества регистров для привязки");

                ushort regSize= 0;
                if (!ushort.TryParse(regStrSize, out regSize))
                    throw new ArgumentException($"Не верное задание размера регистра");


                switch (type.ToUpper())
                {
                    case "DI":
                        {
                            MapRegisters(ref discreteInputsMap, regStartAddress, regCount, regSize);
                        }
                        break;
                    case "AI":
                        {
                            MapRegisters(ref analogInputsMap, regStartAddress, regCount, regSize);
                        }
                        break;
                    case "DO":
                        {
                            MapRegisters(ref discreteOutputsMap, regStartAddress, regCount, regSize);
                        }
                        break;
                    case "AO":
                        {
                            MapRegisters(ref analogOutputsMap, regStartAddress, regCount, regSize);
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported register type: {type}");
                }
            }
        }

        private ArtMonbatChannelsMapper()
        {
           /*
            //входные дискретные регистры 
            MapRegisters(ref discreteInputsMap, 0, 38, 1);

            //добавляем дискретные регистры регулятора расходов
            discreteInputsMap.Add(new ArtMonbatRegisterInfo(172, 1));
            discreteInputsMap.Add(new ArtMonbatRegisterInfo(173, 1));

            //выходные дискретные регистры 
            MapRegisters(ref discreteOutputsMap, 38, 52, 1);
            //добавляем дискретные регистры регулятора расходов
            discreteOutputsMap.Add(new ArtMonbatRegisterInfo(176, 1));
            discreteOutputsMap.Add(new ArtMonbatRegisterInfo(177, 1));

            //входные аналоговые  регистры  (токовые)
            MapRegisters(ref analogInputsMap, 90, 16, 2);

            //входные аналоговые  регистры  (термопары)
            MapRegisters(ref analogInputsMap, 122, 24, 2);

            analogInputsMap.Add(new ArtMonbatRegisterInfo(170, 2));
            analogInputsMap.Add(new ArtMonbatRegisterInfo(174, 2));
            
            //входные аналоговые  регистры  (частотный преобразователь)
            MapRegisters(ref analogInputsMap, 180, 8, 1);

            analogOutputsMap.Add(new ArtMonbatRegisterInfo(178, 2));
            analogOutputsMap.Add(new ArtMonbatRegisterInfo(188, 1));
           */
        }

        public ushort LastRegisterAddress
        {
            get
            {
                List<ushort> iMaxRegister = new List<ushort>()
                { 
                    discreteInputsMap.Max(reg => reg.RegisterAddress),
                    discreteOutputsMap.Max(reg => reg.RegisterAddress),
                    analogInputsMap.Max(reg => reg.RegisterAddress),
                    analogOutputsMap.Max(reg => reg.RegisterAddress)
                };

                List<List<ArtMonbatRegisterInfo>> regMaps = new List<List<ArtMonbatRegisterInfo>>()
                {
                    discreteInputsMap, 
                    discreteOutputsMap, 
                    analogInputsMap, 
                    analogOutputsMap
                };

                ushort iMax = iMaxRegister.Max();

                foreach(var map in regMaps)
                {
                    var maxReg = map.Select(reg => reg).Where(reg => reg.RegisterAddress == iMax).FirstOrDefault();
                    if (maxReg != null)
                    {
                        iMax = (ushort)(maxReg.RegisterAddress + maxReg.Size);
                        break;
                    }
                }

                return iMax;
            }
        }

        private void MapRegisters(ref List<ArtMonbatRegisterInfo> map, ushort iStartRegister, ushort iRegistersCount, ushort iSize)
        {
            ushort iEndRegister = (ushort)(iStartRegister + iRegistersCount * iSize);
            for (ushort i = iStartRegister; i < iEndRegister; i += iSize)
            {
                map.Add(new ArtMonbatRegisterInfo(i, iSize));
            }
        }

        public List<ArtMonbatRegisterInfo> DiscreteInputsMap { get => discreteInputsMap; }
        private List<ArtMonbatRegisterInfo> discreteInputsMap = new List<ArtMonbatRegisterInfo>();

        public void ExtractDiscreteInputs(ushort[] registers, ref BitArray bits)
        {
            if (discreteInputsMap.Count != bits.Count)
                throw new ArgumentOutOfRangeException("Размер битовой карты не совпадает с количеством входных дискретных регистров");

            for(int i = 0; i < DiscreteInputsMap.Count; i++)
            {
                ArtMonbatRegisterInfo regInfo = DiscreteInputsMap[i];
                bits[i] = registers[regInfo.RegisterAddress] > 0 ? true : false;
            }
        }

        public List<ArtMonbatRegisterInfo> DiscreteOutputsMap { get => discreteOutputsMap;}
        private List<ArtMonbatRegisterInfo> discreteOutputsMap = new List<ArtMonbatRegisterInfo>();

        public void ExtractDiscreteOutputs(ushort[] registers, ref BitArray bits)
        {
            if (DiscreteOutputsMap.Count != bits.Count)
                throw new ArgumentOutOfRangeException("Размер битовой карты не совпадает с количеством выходных дискретных регистров");

            for (int i = 0; i < DiscreteOutputsMap.Count; i++)
            {
                ArtMonbatRegisterInfo regInfo = DiscreteOutputsMap[i];
                bits[i] = registers[regInfo.RegisterAddress] > 0 ? true : false;
            }
        }

        public List<ArtMonbatRegisterInfo> AnalogInputsMap { get => analogInputsMap; }
        private List<ArtMonbatRegisterInfo> analogInputsMap = new List<ArtMonbatRegisterInfo>();

        public void ExtractAnalogInputs(ushort[] registers, ref List<double> values)
        {
            if (AnalogInputsMap.Count != values.Count)
                throw new ArgumentOutOfRangeException("Размер карты не совпадает с количеством выходных аналоговых регистров");
            
            try
            {
                for (int i = 0; i < AnalogInputsMap.Count; i++)
                {
                    ArtMonbatRegisterInfo regInfo = AnalogInputsMap[i];
                    uint combinedValue = 0;

                    if (regInfo.Size == 2)
                    {
                        ushort loByte = registers[regInfo.RegisterAddress];
                        ushort hiByte = registers[regInfo.RegisterAddress + 1];

                        // Объединяем два 16-битных регистра в 32-битное целое число
                        combinedValue = (uint)((loByte << 16) | hiByte);
                        // Преобразуем 32-битное целое число в double
                        values[i] = ConvertTodouble(combinedValue);
                    }
                    else if (regInfo.Size == 1)
                    {
                        values[i] = (double)registers[regInfo.RegisterAddress];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        double ConvertTodouble(uint intValue)
        {
            // Преобразование 32-битного целого числа в double
            return BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0);
        }

        public void ExtractAnalogOutputs(ushort[] registers, ref List<double> values)
        {
            if (AnalogOutputsMap.Count != values.Count)
                throw new ArgumentOutOfRangeException("Размер карты не совпадает с количеством входных аналоговых регистров");

            for (int i = 0; i < AnalogOutputsMap.Count; i++)
            {
                ArtMonbatRegisterInfo regInfo = AnalogOutputsMap[i];
                uint combinedValue = 0;

                if (regInfo.Size == 2)
                {
                    ushort loByte = registers[regInfo.RegisterAddress];
                    ushort hiByte = registers[regInfo.RegisterAddress + 1];

                    // Объединяем два 16-битных регистра в 32-битное целое число
                    combinedValue = (uint)((loByte << 16) | hiByte);
                    values[i] = ConvertTodouble(combinedValue);
                }
                else if (regInfo.Size == 1)
                {
                    values[i] = (double)registers[regInfo.RegisterAddress];
                }

                // Преобразуем 32-битное целое число в double
                
            }
        }

        public List<ArtMonbatRegisterInfo> AnalogOutputsMap { get => analogOutputsMap; }        
        private List<ArtMonbatRegisterInfo> analogOutputsMap = new List<ArtMonbatRegisterInfo>();

        
    }
}
