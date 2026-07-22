using ABBDataManagerSystem.Database;
using MySql.Data.MySqlClient;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class CommonTempRiseTestRecordInfo
    {
        public static string TABLE_NAME = "commonTempRiseTestRecord";

        public string ID = String.Empty;
        public DateTime Timestamp;
        public float? Ua = null;
        public float? Ub = null;
        public float? Uc = null;
        public float? U3 = null;
        public float? Ia = null;
        public float? Ib = null;
        public float? Ic = null;
        public float? I3 = null;
       public float? P3 = null;
        public float? FU = null;
       public float? CoreTemp = null;
        public float? WindingTempA = null;
        public float? WindingTempB = null;
        public float? WindingTempC = null;
        public float? EnvTempA = null;
        public float? EnvTempB = null;
        public float? EnvTempC = null;
        public float? EnvTempD = null;
        public float? Outlet1 = null;
        public float? Outlet2 = null;
        public float? Outlet3 = null;
        public float? Outlet4 = null;
        public float? Outlet5 = null;
        public float? Outlet6 = null;
        public float? Inlet1 = null;
        public float? Inlet2 = null;
        public float? Inlet3 = null;
        public float? TopTemp = null;
        public float? OutletWaterTemperature = null;
        public float? InletWaterTemperature = null;
        public float? AmbientTemperature1 = null;
        public float? AmbientTemperature2 = null;
        public float? OutletAirTemperature1 = null;
        public float? OutletAirTemperature2 = null;
        public float? OutletAirTemperature3 = null;
        public float? OutletAirTemperature4 = null;
        public float? OutletAirTemperature5 = null;
        public float? OutletAirTemperature6 = null;
        public float? OutletAirTemperature7 = null;
        public float? OutletAirTemperature8 = null;
        public float? WaterFlowRate = null;
        public bool IsAFWF = false;
        public string WorkflowID = string.Empty;


        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "温升试验编号"},
            {"Timestamp", "试验时间"},
            {"CoreTemp", "铁心温度"},
            {"WindingTempA", "绕组A温度"},
            {"WindingTempB", "绕组B温度"},
            {"WindingTempC", "绕组C温度"},
            {"EnvTempA", "环境A温度"},
            {"EnvTempB", "环境B温度"},
            {"EnvTempC", "环境C温度"},
            {"EnvTempD", "环境D温度"},
            {"OutletWaterTemperature", "出水口温度"},
            {"InletWaterTemperature", "进水口温度"},
            {"AmbientTemperature1", "外循环环境温度1"},
            {"AmbientTemperature2", "外循环环境温度2"},
            {"OutletAirTemperature1", "外循环出风口温度1"},
            {"OutletAirTemperature2", "外循环出风口温度2"},
            {"OutletAirTemperature3", "外循环出风口温度3"},
            {"OutletAirTemperature4", "外循环出风口温度4"},
            {"OutletAirTemperature5", "外循环出风口温度5"},
            {"OutletAirTemperature6", "外循环出风口温度6"},
            {"OutletAirTemperature7", "外循环出风口温度7"},
            {"OutletAirTemperature8", "外循环出风口温度8"},
           {"WaterFlowRate", "冷却液流量"},
            {"FU", "频率"},
       };

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

               using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, Timestamp, ua, ub, uc, u3, ia, ib, ic, i3, p3, " +
                   $"CoreTemperature, WindingATemperature, WindingBTemperature, WindingCTemperature, AmbientATemperature, AmbientBTemperature, AmbientCTemperature, AmbientDTemperature, " +
                   "Outlet1, Outlet2, Outlet3, Outlet4, Outlet5, Outlet6, Inlet1, Inlet2, Inlet3, TopTemperature, " +
                    "OutletWaterTemperature, InletWaterTemperature, AmbientTemperature1, AmbientTemperature2, OutletAirTemperature1, OutletAirTemperature2, OutletAirTemperature3, OutletAirTemperature4, OutletAirTemperature5, OutletAirTemperature6, OutletAirTemperature7, OutletAirTemperature8, WaterFlowRate, fu" +
                   ") VALUES " +
                   "(@ID, @Timestamp, @ua, @ub, @uc, @u3, @ia, @ib, @ic, @i3, @p3, " +
                   $"@CoreTemp, @WindingTempA, @WindingTempB, @WindingTempC, @EnvTempA, @EnvTempB, @EnvTempC, @EnvTempD, " +
                    "@Outlet1, @Outlet2, @Outlet3, @Outlet4, @Outlet5, @Outlet6, @Inlet1, @Inlet2, @Inlet3, @TopTemperature, " +
                    "@OutletWaterTemperature, @InletWaterTemperature, @AmbientTemperature1, @AmbientTemperature2, @OutletAirTemperature1, @OutletAirTemperature2, @OutletAirTemperature3, @OutletAirTemperature4, @OutletAirTemperature5, @OutletAirTemperature6, @OutletAirTemperature7, @OutletAirTemperature8, @WaterFlowRate, @fu" +
                   ")", connection))
               {
                   command.Parameters.AddWithValue("@ID", ID);
                   command.Parameters.AddWithValue("@Timestamp", Timestamp);
                   command.Parameters.AddWithValue("@ua", Ua);
                   command.Parameters.AddWithValue("@ub", Ub);
                   command.Parameters.AddWithValue("@uc", Uc);
                   command.Parameters.AddWithValue("@u3", U3);
                   command.Parameters.AddWithValue("@ia", Ia);
                   command.Parameters.AddWithValue("@ib", Ib);
                   command.Parameters.AddWithValue("@ic", Ic);
                   command.Parameters.AddWithValue("@i3", I3);
                   command.Parameters.AddWithValue("@p3", P3);
                    command.Parameters.AddWithValue("@fu", FU);
                   command.Parameters.AddWithValue("@CoreTemp", CoreTemp);
                    command.Parameters.AddWithValue("@WindingTempA", WindingTempA);
                    command.Parameters.AddWithValue("@WindingTempB", WindingTempB);
                    command.Parameters.AddWithValue("@WindingTempC", WindingTempC);
                    command.Parameters.AddWithValue("@EnvTempA", EnvTempA);
                    command.Parameters.AddWithValue("@EnvTempB", EnvTempB);
                    command.Parameters.AddWithValue("@EnvTempC", EnvTempC);
                    command.Parameters.AddWithValue("@EnvTempD", EnvTempD);
                    command.Parameters.AddWithValue("@Outlet1", Outlet1);
                    command.Parameters.AddWithValue("@Outlet2", Outlet2);
                    command.Parameters.AddWithValue("@Outlet3", Outlet3);
                    command.Parameters.AddWithValue("@Outlet4", Outlet4);
                    command.Parameters.AddWithValue("@Outlet5", Outlet5);
                    command.Parameters.AddWithValue("@Outlet6", Outlet6);
                    command.Parameters.AddWithValue("@Inlet1", Inlet1);
                    command.Parameters.AddWithValue("@Inlet2", Inlet2);
                    command.Parameters.AddWithValue("@Inlet3", Inlet3);
                    command.Parameters.AddWithValue("@TopTemperature", TopTemp);
                    command.Parameters.AddWithValue("@OutletWaterTemperature", OutletWaterTemperature);
                    command.Parameters.AddWithValue("@InletWaterTemperature", InletWaterTemperature);
                    command.Parameters.AddWithValue("@AmbientTemperature1", AmbientTemperature1);
                    command.Parameters.AddWithValue("@AmbientTemperature2", AmbientTemperature2);
                    command.Parameters.AddWithValue("@OutletAirTemperature1", OutletAirTemperature1);
                    command.Parameters.AddWithValue("@OutletAirTemperature2", OutletAirTemperature2);
                    command.Parameters.AddWithValue("@OutletAirTemperature3", OutletAirTemperature3);
                    command.Parameters.AddWithValue("@OutletAirTemperature4", OutletAirTemperature4);
                    command.Parameters.AddWithValue("@OutletAirTemperature5", OutletAirTemperature5);
                    command.Parameters.AddWithValue("@OutletAirTemperature6", OutletAirTemperature6);
                    command.Parameters.AddWithValue("@OutletAirTemperature7", OutletAirTemperature7);
                    command.Parameters.AddWithValue("@OutletAirTemperature8", OutletAirTemperature8);
                    command.Parameters.AddWithValue("@WaterFlowRate", WaterFlowRate);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static List<CommonTempRiseTestRecordInfo> ReadFromDB(string sequence = "", string testPhase = "", string testStatus = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != "")
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
                if (testPhase != "")
                {
                    queryDataSql += $" AND test_phase = '{testPhase}'";
                }
                if (testStatus != "")
                {
                    queryDataSql += $" AND test_status = '{testStatus}'";
                }
            }
            List<CommonTempRiseTestRecordInfo>? records = DBConnector.QueryFromDB<CommonTempRiseTestRecordInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new CommonTempRiseTestRecordInfo
                {
                    ID = reader.GetString("ID"),
                    Timestamp = reader.GetDateTime("Timestamp"),
                };
            });
            if (records == null)
            {
                return new List<CommonTempRiseTestRecordInfo>();
            }
            Log.Info("CommonTempRiseTestRecordInfo COUNT = " + records.Count);
            return records;
        }


        public static bool DeleteData(string ID)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", ID);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "ID");
            };
            return checkIsKeyField;
        }

        private static readonly int BATCH_SIZE = 500;

        public static bool BatchInsertData(List<CommonTempRiseTestRecordInfo> dataRows)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();

               string insertSql = $"INSERT INTO {TABLE_NAME} (ID, Timestamp, ua, ub, uc, u3, ia, ib, ic, i3, p3, " +
                   $"CoreTemperature, WindingATemperature, WindingBTemperature, WindingCTemperature, AmbientATemperature, AmbientBTemperature, AmbientCTemperature, AmbientDTemperature, " +
                   $"Outlet1, Outlet2, Outlet3, Outlet4, Outlet5, Outlet6, Inlet1, Inlet2, Inlet3, TopTemperature, " +
                    $"OutletWaterTemperature, InletWaterTemperature, AmbientTemperature1, AmbientTemperature2, OutletAirTemperature1, OutletAirTemperature2, OutletAirTemperature3, OutletAirTemperature4, OutletAirTemperature5, OutletAirTemperature6, OutletAirTemperature7, OutletAirTemperature8, WaterFlowRate, fu, " +
                   $"workflow_id) VALUES " +
                   $"(@ID, @Timestamp, @ua, @ub, @uc, @u3, @ia, @ib, @ic, @i3, @p3, " +
                   $"@CoreTemp, @WindingTempA, @WindingTempB, @WindingTempC, @EnvTempA, @EnvTempB, @EnvTempC, @EnvTempD, " +
                    $"@Outlet1, @Outlet2, @Outlet3, @Outlet4, @Outlet5, @Outlet6, @Inlet1, @Inlet2, @Inlet3, @TopTemperature, " +
                    $"@OutletWaterTemperature, @InletWaterTemperature, @AmbientTemperature1, @AmbientTemperature2, @OutletAirTemperature1, @OutletAirTemperature2, @OutletAirTemperature3, @OutletAirTemperature4, @OutletAirTemperature5, @OutletAirTemperature6, @OutletAirTemperature7, @OutletAirTemperature8, @WaterFlowRate, @fu, " +
                   $"@WorkflowID)";

                int totalCount = dataRows.Count;
                int processedCount = 0;

                try
                {
                    while (processedCount < totalCount)
                    {
                        int batchEnd = Math.Min(processedCount + BATCH_SIZE, totalCount);
                        using (SQLTransaction transaction = connection.BeginTransaction())
                        {
                            using (SQLCommond command = new SQLCommond())
                            {
                                command.Connection = connection;
                                command.Transaction = transaction;
                                command.CommandText = insertSql;
                                command.CommandTimeout = 300;

                                command.Parameters.Add("@ID", MySqlDbType.Int64);
                                command.Parameters.Add("@Timestamp", MySqlDbType.DateTime);
                                command.Parameters.Add("@ua", MySqlDbType.Float);
                                command.Parameters.Add("@ub", MySqlDbType.Float);
                                command.Parameters.Add("@uc", MySqlDbType.Float);
                                command.Parameters.Add("@u3", MySqlDbType.Float);
                                command.Parameters.Add("@ia", MySqlDbType.Float);
                                command.Parameters.Add("@ib", MySqlDbType.Float);
                                command.Parameters.Add("@ic", MySqlDbType.Float);
                                command.Parameters.Add("@i3", MySqlDbType.Float);
                               command.Parameters.Add("@p3", MySqlDbType.Float);
                                command.Parameters.Add("@fu", MySqlDbType.Float);
                               command.Parameters.Add("@CoreTemp", MySqlDbType.Float);
                                command.Parameters.Add("@WindingTempA", MySqlDbType.Float);
                                command.Parameters.Add("@WindingTempB", MySqlDbType.Float);
                                command.Parameters.Add("@WindingTempC", MySqlDbType.Float);
                                command.Parameters.Add("@EnvTempA", MySqlDbType.Float);
                                command.Parameters.Add("@EnvTempB", MySqlDbType.Float);
                                command.Parameters.Add("@EnvTempC", MySqlDbType.Float);
                                command.Parameters.Add("@EnvTempD", MySqlDbType.Float);
                                command.Parameters.Add("@WorkflowID", MySqlDbType.String);
                                command.Parameters.Add("@Outlet1", MySqlDbType.Float);
                                command.Parameters.Add("@Outlet2", MySqlDbType.Float);
                                command.Parameters.Add("@Outlet3", MySqlDbType.Float);
                                command.Parameters.Add("@Outlet4", MySqlDbType.Float);
                                command.Parameters.Add("@Outlet5", MySqlDbType.Float);
                                command.Parameters.Add("@Outlet6", MySqlDbType.Float);
                                command.Parameters.Add("@Inlet1", MySqlDbType.Float);
                                command.Parameters.Add("@Inlet2", MySqlDbType.Float);
                                command.Parameters.Add("@Inlet3", MySqlDbType.Float);
                                command.Parameters.Add("@TopTemperature", MySqlDbType.Float);
                                command.Parameters.Add("@OutletWaterTemperature", MySqlDbType.Float);
                                command.Parameters.Add("@InletWaterTemperature", MySqlDbType.Float);
                                command.Parameters.Add("@AmbientTemperature1", MySqlDbType.Float);
                                command.Parameters.Add("@AmbientTemperature2", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature1", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature2", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature3", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature4", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature5", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature6", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature7", MySqlDbType.Float);
                                command.Parameters.Add("@OutletAirTemperature8", MySqlDbType.Float);
                                command.Parameters.Add("@WaterFlowRate", MySqlDbType.Float);

                                for (int i = processedCount; i < batchEnd; i++)
                                {
                                    var row = dataRows[i];
                                    command.Parameters["@ID"].Value = row.ID;
                                    command.Parameters["@Timestamp"].Value = row.Timestamp;
                                    command.Parameters["@ua"].Value = row.Ua;
                                    command.Parameters["@ub"].Value = row.Ub;
                                    command.Parameters["@uc"].Value = row.Uc;
                                    command.Parameters["@u3"].Value = row.U3;
                                    command.Parameters["@ia"].Value = row.Ia;
                                    command.Parameters["@ib"].Value = row.Ib;
                                    command.Parameters["@ic"].Value = row.Ic;
                                    command.Parameters["@i3"].Value = row.I3;
                                   command.Parameters["@p3"].Value = row.P3;
                                    command.Parameters["@fu"].Value = row.FU;
                                   command.Parameters["@CoreTemp"].Value = row.CoreTemp;
                                    command.Parameters["@WindingTempA"].Value = row.WindingTempA;
                                    command.Parameters["@WindingTempB"].Value = row.WindingTempB;
                                    command.Parameters["@WindingTempC"].Value = row.WindingTempC;
                                    command.Parameters["@EnvTempA"].Value = row.EnvTempA;
                                    command.Parameters["@EnvTempB"].Value = row.EnvTempB;
                                    command.Parameters["@EnvTempC"].Value = row.EnvTempC;
                                    command.Parameters["@EnvTempD"].Value = row.EnvTempD;
                                    command.Parameters["@WorkflowID"].Value = row.WorkflowID;
                                    command.Parameters["@Outlet1"].Value = row.Outlet1;
                                    command.Parameters["@Outlet2"].Value = row.Outlet2;
                                    command.Parameters["@Outlet3"].Value = row.Outlet3;
                                    command.Parameters["@Outlet4"].Value = row.Outlet4;
                                    command.Parameters["@Outlet5"].Value = row.Outlet5;
                                    command.Parameters["@Outlet6"].Value = row.Outlet6;
                                    command.Parameters["@Inlet1"].Value = row.Inlet1;
                                    command.Parameters["@Inlet2"].Value = row.Inlet2;
                                    command.Parameters["@Inlet3"].Value = row.Inlet3;
                                    command.Parameters["@TopTemperature"].Value = row.TopTemp;
                                    command.Parameters["@OutletWaterTemperature"].Value = row.OutletWaterTemperature;
                                    command.Parameters["@InletWaterTemperature"].Value = row.InletWaterTemperature;
                                    command.Parameters["@AmbientTemperature1"].Value = row.AmbientTemperature1;
                                    command.Parameters["@AmbientTemperature2"].Value = row.AmbientTemperature2;
                                    command.Parameters["@OutletAirTemperature1"].Value = row.OutletAirTemperature1;
                                    command.Parameters["@OutletAirTemperature2"].Value = row.OutletAirTemperature2;
                                    command.Parameters["@OutletAirTemperature3"].Value = row.OutletAirTemperature3;
                                    command.Parameters["@OutletAirTemperature4"].Value = row.OutletAirTemperature4;
                                    command.Parameters["@OutletAirTemperature5"].Value = row.OutletAirTemperature5;
                                    command.Parameters["@OutletAirTemperature6"].Value = row.OutletAirTemperature6;
                                    command.Parameters["@OutletAirTemperature7"].Value = row.OutletAirTemperature7;
                                    command.Parameters["@OutletAirTemperature8"].Value = row.OutletAirTemperature8;
                                    command.Parameters["@WaterFlowRate"].Value = row.WaterFlowRate;
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                        }
                        processedCount = batchEnd;
                        Log.Info($"Batch insert progress: {processedCount}/{totalCount}");
                    }

                    Log.Info("All records have been inserted.");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred: {ex.Message}, inserted {processedCount}/{totalCount}");
                    return false;
                }
            }
        }

        public static string MigrateDBColumns()
        {
            var newColumns = new[] { "fu", "OutletAirTemperature5", "OutletAirTemperature6", "OutletAirTemperature7", "OutletAirTemperature8" };
            var results = new System.Collections.Generic.List<string>();
            try
            {
                using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
                {
                    connection.Open();
                    foreach (var col in newColumns)
                    {
                        try
                        {
                            string sql = $"ALTER TABLE {TABLE_NAME} ADD COLUMN {col} FLOAT DEFAULT NULL";
                            using (SQLCommond command = new SQLCommond(sql, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                            results.Add($"{col}: 添加成功");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Duplicate column"))
                                results.Add($"{col}: 已存在，跳过");
                            else
                                results.Add($"{col}: 失败 - {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"数据库连接失败: {ex.Message}";
            }
            return string.Join("\n", results);
        }
    }
}
