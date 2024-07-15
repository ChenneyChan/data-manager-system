using ABBDataManagerSystem.Configs;
using ABBDataManagerSystem.Pages;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace ABBDataManagerSystem
{
    public class Utils
    {
        public static int ParseInt(string? value, int defaultValue = 0)
        {
            if (value == null) { return defaultValue; }
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? ParseIntNull(string? value)
        {
            if (value == null) { return null; }
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static float ParseFloat(string? value, float defaultValue = 0)
        {
            if (value == null) { return defaultValue; }
            try
            {
                return float.Parse(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float? ParseFloatNull(string? value)
        {
            if (value == null || value == "") { return null; }
            try
            {
                return float.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ParseDateTimeNull(string? value)
        {
            if (value == null) { return null; }
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static object? GetFieldValue(object obj, string fieldName)
        {
            // 使用反射获取对象的类型  
            Type type = obj.GetType();
            // 获取指定字段的信息  
            FieldInfo? fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // 如果字段存在，则获取其值并返回  
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }
            else
            {
                // 获取属性信息
                PropertyInfo? property = type.GetProperty(fieldName);
                if (property != null)
                {
                    // 获取属性的值
                    return property.GetValue(obj);
                }
            }
            return null;
        }

        public static Type ToNumberic(String number, out double dValue, out int iValue)
        {
            iValue = 0;
            dValue = 0;
            try
            {
                iValue = Convert.ToInt32(number);
                return typeof(int);
            }
            catch
            {
                try
                {
                    dValue = Convert.ToDouble(number);
                    return typeof(double);
                }
                catch
                {

                }
                return typeof(String);
            }
        }

        public static string GetAppPath()
        {
            return Path.GetDirectoryName(
                  System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Application.StartupPath
        }

        public static string GetUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static float CalculateLineVoltage(float voltageA, float voltageB)
        {
            return (voltageA + voltageB) / 2 * 1.73205f;
        }

        public static string FloatFormat(float number, int x = 2)
        {
            string formattedNumber;
            // 检查数值是否为0，并特殊处理  
            if (number == 0)
            {
                formattedNumber = "0"; // 0的特殊情况处理  
            }
            else
            {
                // 对于非0值，使用 ToString 方法进行格式化，并去除末尾的零和小数点（如果有的话）  
                var v = string.Format("{0:0.00}", number);
                var vv = v.Split('.');
                if (vv.Length == 2)
                {
                    var vv_ = vv[1].TrimEnd('0');
                    if (vv_.Length == 0)
                    {
                        v = vv[0];
                    }
                    else
                    {
                        v = vv[0] + '.' + vv_;
                    }
                }
                else
                {
                    v = vv[0];
                }
                formattedNumber = v;
            }
            return formattedNumber;
        }

        public static string ZeroIsNull(string value)
        {
            if (value == "0")
            {
                return "";
            }
            return value;
        }

        public static string? FloatFormatZeroIsNull(float? value)
        {
            if (value == null)
            {
                return null;
            }
            return ZeroIsNull(FloatFormat((float)value));
        }

        internal static int ParseInt(object selectedItem)
        {
            throw new NotImplementedException();
        }

        public static string DumpBuffer(byte[] buffer, int offset, int len)
        {
            string s = "";
            for (int i = 0; i < len && i < buffer.Length; i++)
            {
                s += buffer[i].ToString("x2") + " ";
            }
            return s;
        }

        public static void ByteCopy(byte[] src, byte[] dst, byte placeholder = 0x20)
        {
            if (src.Length > dst.Length)
            {
                return;
            }
            Array.Copy(src, 0, dst, dst.Length - src.Length, src.Length);
            for (int i = 0; i < dst.Length - src.Length; i++)
            {
                dst[i] = placeholder;
            }
        }

        public static void FloatToBytes(float value, byte[] buf, byte placeholder = 0x20)
        {
            string str = FloatFormat(value);
            var bytes = Encoding.UTF8.GetBytes(str);
            ByteCopy(bytes, buf, placeholder);
        }

        public static void IntToBytes(int value, byte[] buf, byte placeholder = 0x20)
        {
            string str = value.ToString();
            var bytes = Encoding.UTF8.GetBytes(str);
            ByteCopy(bytes, buf, placeholder);
        }

        // 将float转换为大端字节序的byte数组  
        public static byte[] FloatToBigEndianBytes(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            // 如果当前系统是小端字节序，则反转字节顺序  
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        // 将大端字节序的byte数组转换回float
        public static float BigEndianBytesToFloat(byte[] bigEndianBytes)
        {
            // 如果当前系统是小端字节序，则先反转字节顺序  
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bigEndianBytes);
            }

            return BitConverter.ToSingle(bigEndianBytes, 0);
        }

        internal static byte[] FloatToBigEndianBytes(float? v)
        {
            throw new NotImplementedException();
        }

        public static int? GetInt32(string str)
        {
            // 定义匹配数字的正则表达式
            Regex regex = new Regex(@"\d+");

            // 在字符串中查找匹配的数字
            Match match = regex.Match(str);

            // 输出匹配到的数字
            if (match.Success)
            {
                string numberStr = match.Value;
                int number = int.Parse(numberStr); // 将匹配到的数字字符串转换为整数
                return number;
            }
            else
            {
                return null;
            }
        }

        public static float? GetFloat(string str)
        {
            // 定义匹配浮点数值的正则表达式
            string pattern = @"[-+]?\d*\.?\d+";

            // 使用 Regex 类进行匹配
            MatchCollection matches = Regex.Matches(str, pattern);

            // 遍历匹配结果
            foreach (Match match in matches)
            {
                // 将匹配的字符串转换为 float 类型
                if (float.TryParse(match.Value, out float result))
                {
                    return result;
                }
            }
            return null;
        }

        public static float? GetValueWithMill(string? v, bool needMill = true)
        {
            if (v == null)
            {
                return null;
            }
            float? value = GetFloat(v);
            if (value == null)
            {
                return null;
            }
            if (needMill)
            {
                if (v.IndexOf("m") >= 0)
                {
                    return value;
                }
                return value * 1000;
            }
            else
            {
                if (v.IndexOf("m") >= 0)
                {
                    return value / 1000;
                }
                return value;
            }
        }

        public static bool IsLocked(object obj)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(obj, ref lockTaken);
                return !lockTaken;
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(obj);
                }
            }
        }


        public static string FormatFloat(float number, int noneZoreBits = 4)
        {
            // 如果数字为0，直接返回 "0"
            if (number == 0)
            {
                return "0";
            }

            // 将 float 转换为字符串，确保不使用科学计数法
            string numberString = number.ToString("0.#############################", CultureInfo.InvariantCulture);
            if (numberString.IndexOf("E") >= 0)
            {
                return numberString;
            }

            // 找到小数点的位置
            int decimalPointIndex = numberString.IndexOf('.');

            // 如果没有小数点，直接返回原数字
            if (decimalPointIndex == -1)
            {
                return numberString;
            }

            // 获取小数点前后的部分
            string integerPart = numberString.Substring(0, decimalPointIndex);
            string fractionalPart = numberString.Substring(decimalPointIndex + 1);

            // 拼接结果，确保至少4个非零数字
            string result = integerPart + "." + GetSignificantDigits(fractionalPart, noneZoreBits);

            // 去掉多余的0
            return result.TrimEnd('0').TrimEnd('.');
        }

        private static string GetSignificantDigits(string fractionalPart, int significantDigits)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();

            foreach (char c in fractionalPart)
            {
                sb.Append(c);
                if (c != '0') count++;
                if (count >= significantDigits) break;
            }

            return sb.ToString();
        }
    }
}
