using ABBDataManagerSystem.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Bean.Base
{
    internal class TempRiseCommonInfo
    {
        public static string TABLE_NAME = "tempRiseCommonInfo";

        public string WorkflowID = string.Empty;
        public int TestIndex = 0;
        public string CoolingMode = string.Empty;
        public string TestingPhase = string.Empty;
        public float? TempRiseTestingVoltage = null;
        public float? TempRiseTestingCurrent = null;
        public float? TempRiseHVCorrectionFactor = null;
        public float? TempRiseLVCorrectionFactor = null;
        public string TempRiseRelativeTo = String.Empty;


        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"WorkflowID", "工作令编号 "},
            {"TempRiseTestingVoltage", "温升试验电压" },
            {"TempRiseTestingCurrent", "温升试验电流" },
            {"TempRiseHVCorrectionFactor", "高压温升校正系数" },
            {"TempRiseLVCorrectionFactor", "低压温升校正系数" },
            {"TempRiseRelativeTo", "温升相对于" },
        };


        public bool UpdateData()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET TempRiseTestingVoltage = @TempRiseTestingVoltage, TempRiseTestingCurrent = @TempRiseTestingCurrent, " +
                "TempRiseHVCorrectionFactor = @TempRiseHVCorrectionFactor, TempRiseLVCorrectionFactor = @TempRiseLVCorrectionFactor, TempRiseRelativeTo = @TempRiseRelativeTo " +
                "WHERE workflow_id = @WorkflowID AND TestIndex = @TestIndex AND CoolingMode = @CoolingMode AND TestingPhase = @TestingPhase";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@WorkflowID", WorkflowID);
                    command.Parameters.AddWithValue("@TestIndex", TestIndex);
                    command.Parameters.AddWithValue("@CoolingMode", CoolingMode);
                    command.Parameters.AddWithValue("@TestingPhase", TestingPhase);
                    command.Parameters.AddWithValue("@TempRiseTestingVoltage", TempRiseTestingVoltage);
                    command.Parameters.AddWithValue("@TempRiseTestingCurrent", TempRiseTestingCurrent);
                    command.Parameters.AddWithValue("@TempRiseHVCorrectionFactor", TempRiseHVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseLVCorrectionFactor", TempRiseLVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool InsertDB()
        {
            string updateSql = $"INSERT INTO {TABLE_NAME} (workflow_id, TestingPhase, TestIndex, CoolingMode, TempRiseTestingVoltage, TempRiseTestingCurrent, TempRiseHVCorrectionFactor, TempRiseLVCorrectionFactor, TempRiseRelativeTo) VALUES " +
                " (@WorkflowID, @TestingPhase, @TestIndex, @CoolingMode, @TempRiseTestingVoltage, @TempRiseTestingCurrent, @TempRiseHVCorrectionFactor, @TempRiseLVCorrectionFactor, @TempRiseRelativeTo)";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@WorkflowID", WorkflowID);
                    command.Parameters.AddWithValue("@TestIndex", TestIndex);
                    command.Parameters.AddWithValue("@TestingPhase", TestingPhase);
                    command.Parameters.AddWithValue("@CoolingMode", CoolingMode);
                    command.Parameters.AddWithValue("@TempRiseTestingVoltage", TempRiseTestingVoltage);
                    command.Parameters.AddWithValue("@TempRiseTestingCurrent", TempRiseTestingCurrent);
                    command.Parameters.AddWithValue("@TempRiseHVCorrectionFactor", TempRiseHVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseLVCorrectionFactor", TempRiseLVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<TempRiseCommonInfo> ReadFromDB(string ID, string testingPhase, int sortIndex, string coolingMode)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            queryDataSql += $" WHERE workflow_id = '{ID}' AND TestingPhase = '{testingPhase}' AND TestIndex = {sortIndex} AND CoolingMode = '{coolingMode}'";
            List<TempRiseCommonInfo>? records = DBConnector.QueryFromDB<TempRiseCommonInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new TempRiseCommonInfo
                {
                    WorkflowID = reader.GetString("workflow_id"),
                    CoolingMode = reader.GetString("CoolingMode"),
                    TestingPhase = reader.GetString("TestingPhase"),
                    TestIndex = reader.GetInt32("TestIndex"),
                    TempRiseRelativeTo = !reader.IsDBNull("TempRiseRelativeTo") ? reader.GetString("TempRiseRelativeTo") : "",
                    TempRiseTestingVoltage = !reader.IsDBNull("TempRiseTestingVoltage") ? reader.GetFloat("TempRiseTestingVoltage") : null,
                    TempRiseTestingCurrent = !reader.IsDBNull("TempRiseTestingCurrent") ? reader.GetFloat("TempRiseTestingCurrent") : null,
                    TempRiseHVCorrectionFactor = !reader.IsDBNull("TempRiseHVCorrectionFactor") ? reader.GetFloat("TempRiseHVCorrectionFactor") : null,
                    TempRiseLVCorrectionFactor = !reader.IsDBNull("TempRiseLVCorrectionFactor") ? reader.GetFloat("TempRiseLVCorrectionFactor") : null,
                };
            });
            if (records == null)
            {
                return new List<TempRiseCommonInfo>();
            }
            Log.Info("TempRiseCommonInfo COUNT = " + records.Count);
            return records;
        }

    }
}
