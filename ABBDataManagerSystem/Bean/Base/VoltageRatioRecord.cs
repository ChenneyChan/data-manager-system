using ABBDataManagerSystem.Database;
using MySql.Data.MySqlClient;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class VoltageRatioInfo
    {
        public static string TABLE_NAME = "voltageRatioInfoRecord";

        public string WorkflowId = String.Empty;
        public int TappingPosition = 0;
        public float? TappingVoltage = null;
        public float? CalRatio = null;
        public float? ErrorAB = null;
        public float? ErrorBC = null;
        public float? ErrorCA = null;
        public string ConnectionGroup1 = String.Empty;
        public string ConnectionGroup2 = String.Empty;

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"WorkflowId", "出厂序号"},
        };

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (workflow_id, TappingPosition, TappingVoltage, CalRatio, ErrorAB, ErrorBC, ErrorCA, ConnectionGroup1, ConnectionGroup2) " +
                    "VALUES(@WorkflowId, @TappingPosition, @TappingVoltage, @CalRatio, @ErrorAB, @ErrorBC, @ErrorCA, @ConnectionGroup1, @ConnectionGroup2)", connection))
                {
                    command.Parameters.AddWithValue("@WorkflowId", WorkflowId);
                    command.Parameters.AddWithValue("@TappingPosition", TappingPosition);
                    command.Parameters.AddWithValue("@TappingVoltage", TappingVoltage);
                    command.Parameters.AddWithValue("@CalRatio", CalRatio);
                    command.Parameters.AddWithValue("@ErrorAB", ErrorAB);
                    command.Parameters.AddWithValue("@ErrorBC", ErrorBC);
                    command.Parameters.AddWithValue("@ErrorCA", ErrorCA);
                    command.Parameters.AddWithValue("@ConnectionGroup1", ConnectionGroup1);
                    command.Parameters.AddWithValue("@ConnectionGroup2", ConnectionGroup2);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static List<VoltageRatioInfo> ReadFromDB(string sequence = "", string tappingPosition = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != "")
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
                if (tappingPosition != "")
                {
                    queryDataSql += $" AND TappingPosition = '{tappingPosition}'";
                }
            }
            List<VoltageRatioInfo>? records = DBConnector.QueryFromDB<VoltageRatioInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new VoltageRatioInfo
                {
                    WorkflowId = reader.GetString("workflow_id"),
                    TappingPosition = reader.GetInt32("TappingPosition"),
                    TappingVoltage = reader.GetFloat("TappingVoltage"),
                    CalRatio = reader.GetFloat("CalRatio"),
                    ErrorAB = reader.GetFloat("ErrorAB"),
                    ErrorBC = reader.GetFloat("ErrorBC"),
                    ErrorCA = reader.GetFloat("ErrorCA"),
                    ConnectionGroup1 = reader.GetString("ConnectionGroup1"),
                    ConnectionGroup2 = reader.GetString("ConnectionGroup2"),
                };
            });
            if (records == null)
            {
                return new List<VoltageRatioInfo>();
            }
            Log.Info("CommonTempRiseTestInfo COUNT = " + records.Count);
            return records;
        }

        public static bool BatchInsertData(List<VoltageRatioInfo> dataRows)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();

                using (SQLTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLCommond command = new SQLCommond())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = $"INSERT INTO {TABLE_NAME} (workflow_id, TappingPosition, TappingVoltage, CalRatio, ErrorAB, ErrorBC, ErrorCA, ConnectionGroup1, ConnectionGroup2) " +
                            "VALUES(@WorkflowId, @TappingPosition, @TappingVoltage, @CalRatio, @ErrorAB, @ErrorBC, @ErrorCA, @ConnectionGroup1, @ConnectionGroup2)";

                        command.Parameters.Add("@WorkflowId", MySqlDbType.String);
                        command.Parameters.Add("@TappingPosition", MySqlDbType.String);
                        command.Parameters.Add("@TappingVoltage", MySqlDbType.Float);
                        command.Parameters.Add("@CalRatio", MySqlDbType.Float);
                        command.Parameters.Add("@ErrorAB", MySqlDbType.Float);
                        command.Parameters.Add("@ErrorBC", MySqlDbType.Float);
                        command.Parameters.Add("@ErrorCA", MySqlDbType.Float);
                        command.Parameters.Add("@ConnectionGroup1", MySqlDbType.String);
                        command.Parameters.Add("@ConnectionGroup2", MySqlDbType.String);

                        //try
                        {
                            foreach (var row in dataRows)
                            {
                                command.Parameters["@WorkflowId"].Value = row.WorkflowId;
                                command.Parameters["@TappingPosition"].Value = row.TappingPosition;
                                command.Parameters["@TappingVoltage"].Value = row.TappingVoltage;
                                command.Parameters["@CalRatio"].Value = row.CalRatio;
                                command.Parameters["@ErrorAB"].Value = row.ErrorAB;
                                command.Parameters["@ErrorBC"].Value = row.ErrorBC;
                                command.Parameters["@ErrorCA"].Value = row.ErrorCA;
                                command.Parameters["@ConnectionGroup1"].Value = row.ConnectionGroup1;
                                command.Parameters["@ConnectionGroup2"].Value = row.ConnectionGroup2;
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            Log.Info("All records have been inserted.");
                            return true;
                        }
                        //catch (Exception ex)
                        //{
                        //    transaction.Rollback();
                        //    Log.Error($"插入失败: {ex.Message}");
                        //    return false;
                        //}
                    }
                }
            }
        }


        public static bool DeleteData(string workflowID)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE workflow_id = @WorkflowId", connection);
                command.Parameters.AddWithValue("@WorkflowId", workflowID);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "WorkflowId");
            };
            return checkIsKeyField;
        }
    }
}
