using System.Reflection;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Bean
{
    public class BeanUtils
    {
        //public static T? GetValueFromDataTableSelectedItem<T>(DataGrid dataView) where T : class, new()
        //{
        //    if (dataView.SelectedItems.Count == 0)
        //    {
        //        return null;
        //    }
        //    var row = dataView.SelectedItems[0];

        //    T instance = new();

        //    //Log.Info($"===========Begin of record:");
        //    for (int i = 0; i < dataView.Columns.Count; i++)
        //    {
        //        var columnName = dataView.Columns[i].Name;
        //        string? value = row.Cells[columnName].Value.ToString();
        //        //Log.Info($"\t{columnName} = {value}");

        //        try
        //        {
        //            FieldInfo? field = typeof(T).GetField(columnName);
        //            if (field != null)
        //            {
        //                if (field.FieldType == typeof(string))
        //                {
        //                    field.SetValue(instance, value?? "");
        //                }
        //                else if (field.FieldType == typeof(DateTime) || field.FieldType == typeof(DateTime?))
        //                {
        //                    if (value == null)
        //                    {
        //                        field.SetValue(instance, DateTime.Now);
        //                    }
        //                    else
        //                    {
        //                        field.SetValue(instance, DateTime.Parse(value));
        //                    }
        //                }
        //                else if (field.FieldType == typeof(int) || field.FieldType == typeof(int?))
        //                {
        //                    if (value == null || value.Length == 0)
        //                    {
        //                        field.SetValue(instance, null);
        //                    }
        //                    else
        //                    {
        //                        field.SetValue(instance, Utils.ParseInt(value));
        //                    }
        //                }
        //                else if (field.FieldType == typeof(float) || field.FieldType == typeof(float?))
        //                {
        //                    if (value == null || value.Length == 0)
        //                    {
        //                        field.SetValue(instance, null);
        //                    }
        //                    else
        //                    {
        //                        field.SetValue(instance, Utils.ParseFloat(value));
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error("Fail to ProcessField: " + ex.Message);
        //            return null;
        //        }
        //    }
        //    //Log.Info($"===========End of record");
        //    return instance;
        //}
    }
    public delegate bool CheckIsKeyField(string fieldName);
}
