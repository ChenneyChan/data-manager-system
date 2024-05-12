using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    public delegate void OnPartialDischargeUpdate(float discharge1, float discharge2, float discharge3, float discharge4);
    public class PartialDischargeCollector
    {
        private SerialPortProcessor SerialPortConnector;
        private OnPartialDischargeUpdate OnPartialDischargeUpdate;

        public PartialDischargeCollector(string port, int boundRate, OnPartialDischargeUpdate OnPartialDischargeUpdate)
        {
            SerialPortConnector = new SerialPortProcessor(port, boundRate);
            this.OnPartialDischargeUpdate = OnPartialDischargeUpdate;
        }

        public bool StartCollect()
        {
            return SerialPortConnector.Start();
        }

        public float[]? SendRequsetAndWaiteData()
        {
            byte[]? data = SerialPortConnector.SendRequestAndReceiveResponseAsync(new byte[] { 0x7e, 0x31, 0x41, 0x0e, 0x0d });
            if (data == null)
            {
                return null;
            }
            return ParseData(data, data.Length);
        }

        public void StopCollect()
        {
            SerialPortConnector.Close();
        }

        private float[]? ParseData(byte[] buffer, int len)
        {
            if (buffer.Length < len)
            {
                return null;
            }
            float[] values = new float[4];
            if (len >= 11)
            {
                values[0] = ParseValue(buffer, 0);
            }
            if (len >= 22)
            {
                values[1] = ParseValue(buffer, 11);
            }
            if (len >= 33)
            {
                values[2] = ParseValue(buffer, 22);
            }
            if (len >= 44)
            {
                values[3] = ParseValue(buffer, 33);
            }
            return values;
        }

        private float ParseValue(byte[] buff, int offset)
        {
            if (buff[offset] != 0x7e || buff[offset + 1] != 0x37 || buff[offset + 10] != 0x0d)
            {
                return 0;
            }
            bool nC = buff[offset + 8] == 0x6e;
            string str = Encoding.ASCII.GetString(buff, 2 + offset, 6);
            float value = Utils.ParseFloat(str);
            if (nC)
            {
                value *= 1000;
            }
            return value;
        }

    }
}
