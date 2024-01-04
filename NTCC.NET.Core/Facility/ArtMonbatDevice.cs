using Modbus.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
    /// <summary>
    /// Класс для работы шкафом управлиения
    /// </summary>
    public class ArtMonbatDevice : AcquisitionDeviceBase
    {

        public ArtMonbatDevice(string id, string registerMappingFilePath) : base(id)
        {           
            ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
            mapper.Initialize(registerMappingFilePath);
            TotalRegisters = mapper.LastRegisterAddress;

            DiscreteInputs = new BitArray(mapper.DiscreteInputsMap.Count);
            DiscreteOutputs = new BitArray(mapper.DiscreteOutputsMap.Count);

            AnalogInputs = new List<double>(mapper.AnalogInputsMap.Count);
            for (int i = 0; i < mapper.AnalogInputsMap.Count; i++)
                AnalogInputs.Add(0.0f);

            AnalogOutputs = new List<double>(mapper.AnalogOutputsMap.Count);
            for (int i = 0; i < mapper.AnalogOutputsMap.Count; i++)
                AnalogOutputs.Add(0.0f);

            registers = new ushort[TotalRegisters];
        }

        /// <summary>
        /// IP Adress of inverter device
        /// </summary>
        public string IP
        {
            get;
            private set;
        }


        /// <summary>
        /// TCP/IP Port
        /// </summary>
        public ushort Port
        {
            get;
            private set;
        } = 512;

        /// <summary>
        ////Идентификатор модбас устройства
        /// </summary>
        public byte UnitID
        {
            get;
            private set;
        } = 1;

        //общее число считываемых регистров
        private ushort TotalRegisters = 0;

        private TcpClient client = null;
        private ModbusIpMaster master = null;

        public override bool Connect(string connection, int timeout)
        {
            string[] addr = connection.Split(':');

            string ip = "192.168.100.54";
            ushort port = 502;
            byte unitID = 1;

            if (addr.GetLength(0) == 3)
            {
                ip = addr[0];
                ushort.TryParse(addr[1], out port);
                byte.TryParse(addr[2], out unitID);
            }
            else if (addr.GetLength(0) == 2)
            {
                ip = addr[0];
                ushort.TryParse(addr[1], out port);
            }
            else if (addr.GetLength(0) == 1)
            {
                ip = addr[0];
                ushort.TryParse(addr[1], out port);
            }
            else
                throw new ArgumentException("Invalid connection string");

            IP = ip;
            Port = port;
            UnitID = unitID;

            client = new TcpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP), port);

            try
            {
                client.Connect(ep);
                master = ModbusIpMaster.CreateIp(client);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        //private List<ushort> registers = new List<ushort>();

        ushort[] registers = null;

        //lock object
        object lockModbus = new object();

        protected override void UpdateData() 
        {
            lock (lockModbus)
            {
                ushort iMaxRegisterPerRequest = 125;
                ushort iStartRegister  = 0;
                ushort iRegisterToRead = iMaxRegisterPerRequest;
                ushort iRegistersToReadLeft = TotalRegisters;
                try
                {
                    while (iRegistersToReadLeft != 0)
                    {
                        ushort[] response = master.ReadHoldingRegisters(UnitID, iStartRegister, iRegisterToRead);

                        Array.Copy(response, 0, registers, iStartRegister, response.Length);

                        iStartRegister += iRegisterToRead;
                        iRegistersToReadLeft -= iRegisterToRead;
                        iRegisterToRead = (iRegistersToReadLeft > iMaxRegisterPerRequest) ? iMaxRegisterPerRequest : iRegistersToReadLeft;

                    }
                }
                catch(Modbus.SlaveException mbex)
                {
                    Debug.WriteLine(mbex.Message);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }               

                base.UpdateData();
            }
        }

        public override void SetDiscreteOutput(int ch, bool state)
        {
            lock (lockModbus)
            {
                ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
                ArtMonbatRegisterInfo regInfo = mapper.DiscreteOutputsMap[ch];

                ushort stateValue = (ushort)(state ? 1 : 0);
                master.WriteSingleRegister((ushort)regInfo.RegisterAddress, stateValue);
            }
        }

        public override void SetAnalogOutput(int ch, double value)
        {
            lock (lockModbus)
            {
                ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
                ArtMonbatRegisterInfo regInfo = mapper.AnalogOutputsMap[ch];

                float newVal = (float)value;

                if (regInfo.Size == 2)
                {
                    ushort[] resultRegisters = FloatToModbusRegisters((float)value);
                    master.WriteMultipleRegisters((ushort)regInfo.RegisterAddress, resultRegisters);
                }
                else if (regInfo.Size == 1)
                {
                    ushort resultRegisters = (ushort)(value);
                    master.WriteSingleRegister((ushort)regInfo.RegisterAddress, resultRegisters);
                }
            }
        }

        public static ushort[] FloatToModbusRegisters(float floatValue)
        {
            byte[] floatBytes = BitConverter.GetBytes(floatValue);
            List<ushort> ra = new List<ushort>();
            
            for (int i = 0; i < floatBytes.Length; i += 2)
            {
                ra.Insert(0,  BitConverter.ToUInt16( new byte[] { floatBytes[i], floatBytes[i + 1] }, 0) );
            }

            return ra.ToArray();
        }

        protected override void UpdateDiscreteInputs()
        {
            ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
            mapper.ExtractDiscreteInputs(registers, ref discreteInputs);
        }

        protected override void UpdateDiscreteOutputs()
        {
            ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
            mapper.ExtractDiscreteOutputs(registers, ref discreteOutputs);
        }

        protected override void UpdateAnalogInputs()
        {
            ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
            mapper.ExtractAnalogInputs(registers, ref analogInputs);
        }

        protected override void UpdateAnalogOutputs()
        {
            ArtMonbatChannelsMapper mapper = ArtMonbatChannelsMapper.Instance;
            mapper.ExtractAnalogOutputs(registers, ref analogOutputs);
        }
    }
}
