using ABBDataManagerSystem.Configs;
using System.IO;
using System.Reflection;

namespace ABBDataManagerSystem
{
    public class Utils
    {
        public static int ParseInt(string? valule, int defaultValue = 0)
        {
            if (valule == null) { return defaultValue; }
            try
            {
                return int.Parse(valule);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? ParseIntNull(string? valule)
        {
            if (valule == null) { return null; }
            try
            {
                return int.Parse(valule);
            }
            catch
            {
                return null;
            }
        }

        public static float ParseFloat(string? valule, float defaultValue = 0)
        {
            if (valule == null) { return defaultValue; }
            try
            {
                return float.Parse(valule);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float? ParseFloatNull(string? valule)
        {
            if (valule == null) { return null; }
            try
            {
                return float.Parse(valule);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ParseDateTimeNull(string? valule)
        {
            if (valule == null) { return null; }
            try
            {
                return DateTime.Parse(valule);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, TestingType> GetTestingTypes()
        {
            var dic = new Dictionary<string, TestingType>
            {
                { "干式消弧线圈试验", TestingType.ArcSuppressionCoilTesting },
                { "干式变压器试验", TestingType.DryTypeTransformerTesting },
                { "干式变压器试验-300", TestingType.DryTypeTransformerTestingWT300 },
                { "干式串联试验", TestingType.DrySeriesConnectionTesting },
                { "高阻抗试验", TestingType.HighImpedanceTesting },
                { "接地变试验", TestingType.GroudingTesting }
            };

            return dic;
        }

        public static string GetTestingNameByType(TestingType testingType)
        {
            var types = GetTestingTypes();
            foreach (var item in types.Keys)
            {
                bool ret = types.TryGetValue(item, out TestingType _testingType);
                if (ret && testingType == _testingType)
                {
                    return item;
                }
            }
            return "";
        }


        public static void ParseCurrentGearValues(in List<float> values, string[] gears)
        {
            string gearStrs = "";
            string gearValues = "";
            values.Clear();
            foreach (string gear in gears)
            {
                var v = gear.Split('A')[0];
                var vv = ParseFloat(v) / 5;
                values.Add(vv);
                gearStrs += gear + " \\ ";
                gearValues += vv.ToString() + " \\ ";
            }
            Log.Info("ParseCurrentGearValues:");
            Log.Info($"GearStrings: {gearStrs}");
            Log.Info($"GearValues: {gearValues}");
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
                var v = number.ToString($"F{x}");
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

        internal static int ParseInt(object selectedItem)
        {
            throw new NotImplementedException();
        }

        public static string DumpBuffer(byte[]buffer, int offset, int len)
        {
            string s = "";
            for (int i = 0; i < len && i<buffer.Length; i++)
            {
                s += buffer[i].ToString("x2") + " ";
            }
            return s;
        }
    }
}
