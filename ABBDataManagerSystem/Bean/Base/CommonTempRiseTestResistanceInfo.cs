using ABBDataManagerSystem.Database;
using MySql.Data.MySqlClient;

namespace ABBDataManagerSystem.Bean.Base
{
    public class CommonTempRiseTestResistanceInfo
    {
        public static string TABLE_NAME = "commonTempRiseTestResistance";

        public string ID = String.Empty;
        public int VoltageType { get; set; } = 0;
        public int SortIndex { get; set; }
        public string CurrentTime { get; set; } = string.Empty;
        public float CurrentHV { get; set; }
        public float CurrentLV { get; set; }
        public float ResistanceHV { get; set; }
        public float ResistanceLV { get; set; }


        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "温升试验编号"},
            {"VoltageType", "电压类型"},
            {"SortIndex", "索引"},
            {"CurrentTime", "采集时间"},
            {"CurrentHV", "高压电流"},
            {"CurrentLV", "低压电流"},
            {"ResistanceHV", "高压电阻"},
            {"ResistanceLV", "低压电阻"},
        };

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, SortIndex, VoltageType, CurrentTime, CurrentHV, CurrentLV, ResistanceHV, ResistanceLV) " +
                    " VALUES (@ID, @SortIndex, @VoltageType, @CurrentTime, @CurrentHV, @CurrentLV, @ResistanceHV, @ResistanceLV)", connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@SortIndex", SortIndex);
                    command.Parameters.AddWithValue("@VoltageType", VoltageType);
                    command.Parameters.AddWithValue("@CurrentTime", CurrentTime);
                    command.Parameters.AddWithValue("@CurrentHV", CurrentHV);
                    command.Parameters.AddWithValue("@CurrentLV", CurrentLV);
                    command.Parameters.AddWithValue("@ResistanceHV", ResistanceHV);
                    command.Parameters.AddWithValue("@ResistanceLV", ResistanceLV);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static bool DeleteData(string ID, int? VoltageType = null)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string voltageCondition = "";
                if (VoltageType != null)
                {
                    voltageCondition = " AND VoltageType = @VoltageType";
                }
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE ID = @ID {voltageCondition}", connection);
                command.Parameters.AddWithValue("@ID", ID);
                if (VoltageType != null)
                {
                    command.Parameters.AddWithValue("@VoltageType", VoltageType);
                }
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

        public static bool BatchInsertData(List<CommonTempRiseTestResistanceInfo> dataRows)
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
                        command.CommandText = $"INSERT INTO {TABLE_NAME} (ID, SortIndex, VoltageType, CurrentTime, CurrentHV, CurrentLV, ResistanceHV, ResistanceLV) " +
                            " VALUES (@ID, @SortIndex, @VoltageType, @CurrentTime, @CurrentHV, @CurrentLV, @ResistanceHV, @ResistanceLV)";

                        command.Parameters.Add("@ID", MySqlDbType.Int64);
                        command.Parameters.Add("@SortIndex", MySqlDbType.Int32);
                        command.Parameters.Add("@VoltageType", MySqlDbType.Int16);
                        command.Parameters.Add("@CurrentTime", MySqlDbType.String);
                        command.Parameters.Add("@CurrentHV", MySqlDbType.Float);
                        command.Parameters.Add("@CurrentLV", MySqlDbType.Float);
                        command.Parameters.Add("@ResistanceHV", MySqlDbType.Float);
                        command.Parameters.Add("@ResistanceLV", MySqlDbType.Float);

                        //try
                        {
                            foreach (var row in dataRows)
                            {
                                command.Parameters["@ID"].Value = row.ID;
                                command.Parameters["@SortIndex"].Value = row.SortIndex;
                                command.Parameters["@VoltageType"].Value = row.VoltageType;
                                command.Parameters["@CurrentTime"].Value = row.CurrentTime;
                                command.Parameters["@CurrentHV"].Value = row.CurrentHV;
                                command.Parameters["@CurrentLV"].Value = row.CurrentLV;
                                command.Parameters["@ResistanceHV"].Value = row.ResistanceHV;
                                command.Parameters["@ResistanceLV"].Value = row.ResistanceLV;
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
    }
}
