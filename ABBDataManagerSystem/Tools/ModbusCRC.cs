using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Tools
{
    public class ModbusCRC
    {
        #region 计算CRC校验码
        /// <summary>
        /// 计算CRC校验码，并转换为十六进制字符串
        /// Cyclic Redundancy Check 循环冗余校验码
        /// 是数据通信领域中最常用的一种差错校验码
        /// 特征是信息字段和校验字段的长度可以任意选定
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Calculate_CRC16_C(byte[] data, int offset, int len)
        {
            byte num = 0xff;
            byte num2 = 0xff;

            byte num3 = 1;
            byte num4 = 160;
            byte[] buffer = data;

            for (int i = offset; i < buffer.Length && i < len; i++)
            {
                //位异或运算
                num = (byte)(num ^ buffer[i]);

                for (int j = 0; j <= 7; j++)
                {
                    byte num5 = num2;
                    byte num6 = num;

                    //位右移运算
                    num2 = (byte)(num2 >> 1);
                    num = (byte)(num >> 1);

                    //位与运算
                    if ((num5 & 1) == 1)
                    {
                        //位或运算
                        num = (byte)(num | 0x80);
                    }
                    if ((num6 & 1) == 1)
                    {
                        num2 = (byte)(num2 ^ num4);
                        num = (byte)(num ^ num3);
                    }
                }
            }
            return new byte[] { num, num2 };
        }
        #endregion

    }
}
