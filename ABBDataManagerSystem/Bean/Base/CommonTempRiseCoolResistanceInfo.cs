using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class CommonTempRiseCoolResistanceInfo
    {
        public static string TABLE_NAME = "temprisecoolresistance";

        public string WorkflowID = String.Empty;
        public float? HighVoltageResistance1;
        public float? LowVoltageResistance11;
        public float? LowVoltageResistance12;
        public float? HighVoltageResistance2;
        public float? LowVoltageResistance21;
        public float? LowVoltageResistance22;
        public float? HighVoltageCurrent;
        public float? LowVoltageCurrent1;
        public float? LowVoltageCurrent2;
        public float? Temperature;

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"workflow_id", "工作令编号"},
            { "HighVoltageResistance1"   , "第一次高压电阻" },
            { "LowVoltageResistance11"   , "第一次低压电阻1" },
            { "LowVoltageResistance12"   , "第一次低压电阻2" },
            { "HighVoltageResistance2"   , "第二次高压电阻" },
            { "LowVoltageResistance21"   , "第二次低压电阻1" },
            { "LowVoltageResistance22"   , "第二次低压电阻2" },
            { "HighVoltageCurrent"       , "高压电阻电流" },
            { "LowVoltageCurrent1"       , "低压电阻电流1" },
            { "LowVoltageCurrent2"       , "低压电阻电流2" },
            { "Temperature"              , "温度" },
        };

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (workflow_id, highVoltageResistance1, lowVoltageResistance11, lowVoltageResistance12, " +
                    "highVoltageResistance2, lowVoltageResistance21, lowVoltageResistance22, highVoltageCurrent, lowVoltageCurrent1, lowVoltageCurrent2, " + 
                    "temperature) VALUES (@workflow_id, @highVoltageResistance1, @lowVoltageResistance11, @lowVoltageResistance12, @highVoltageResistance2, " + 
                    "@lowVoltageResistance21, @lowVoltageResistance22, @highVoltageCurrent, @lowVoltageCurrent1, @lowVoltageCurrent2, @temperature)", connection))
                {
                    command.Parameters.AddWithValue("@workflow_id", WorkflowID);
                    command.Parameters.AddWithValue("@highVoltageResistance1", HighVoltageResistance1);
                    command.Parameters.AddWithValue("@lowVoltageResistance11", LowVoltageResistance11);
                    command.Parameters.AddWithValue("@lowVoltageResistance12", LowVoltageResistance12);
                    command.Parameters.AddWithValue("@highVoltageResistance2", HighVoltageResistance2);
                    command.Parameters.AddWithValue("@lowVoltageResistance21", LowVoltageResistance21);
                    command.Parameters.AddWithValue("@lowVoltageResistance22", LowVoltageResistance22);
                    command.Parameters.AddWithValue("@highVoltageCurrent", HighVoltageCurrent);
                    command.Parameters.AddWithValue("@lowVoltageCurrent1", LowVoltageCurrent1);
                    command.Parameters.AddWithValue("@lowVoltageCurrent2", LowVoltageCurrent2);
                    command.Parameters.AddWithValue("@temperature", Temperature);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static List<CommonTempRiseCoolResistanceInfo> ReadFromDB(string sequence = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != "")
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
            }
            List<CommonTempRiseCoolResistanceInfo>? records = DBConnector.QueryFromDB<CommonTempRiseCoolResistanceInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new CommonTempRiseCoolResistanceInfo
                {
                    WorkflowID = reader.GetString("workflow_id"),
                    HighVoltageResistance1 = !reader.IsDBNull("highVoltageResistance1") ? reader.GetFloat("highVoltageResistance1") : null,
                    LowVoltageResistance11 = !reader.IsDBNull("lowVoltageResistance11") ? reader.GetFloat("lowVoltageResistance11") : null,
                    LowVoltageResistance12 = !reader.IsDBNull("lowVoltageResistance12") ? reader.GetFloat("lowVoltageResistance12") : null,
                    HighVoltageResistance2 = !reader.IsDBNull("highVoltageResistance2") ? reader.GetFloat("highVoltageResistance2") : null,
                    LowVoltageResistance21 = !reader.IsDBNull("lowVoltageResistance21") ? reader.GetFloat("lowVoltageResistance21") : null,
                    LowVoltageResistance22 = !reader.IsDBNull("lowVoltageResistance22") ? reader.GetFloat("lowVoltageResistance22") : null,
                    HighVoltageCurrent = !reader.IsDBNull("highVoltageCurrent") ? reader.GetFloat("highVoltageCurrent") : null,
                    LowVoltageCurrent1 = !reader.IsDBNull("lowVoltageCurrent1") ? reader.GetFloat("lowVoltageCurrent1") : null,
                    LowVoltageCurrent2 = !reader.IsDBNull("lowVoltageCurrent2") ? reader.GetFloat("lowVoltageCurrent2") : null,
                    Temperature = !reader.IsDBNull("temperature") ? reader.GetFloat("temperature") : null,
                };
            });
            if (records == null)
            {
                return new List<CommonTempRiseCoolResistanceInfo>();
            }
            Log.Info("CommonTempRiseCoolResistanceInfo COUNT = " + records.Count);
            return records;
        }


        public static bool DeleteData(string workflowID)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE workflow_id = @workflow_id", connection);
                command.Parameters.AddWithValue("@workflow_id", workflowID);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "workflow_id");
            };
            return checkIsKeyField;
        }
    }
}
