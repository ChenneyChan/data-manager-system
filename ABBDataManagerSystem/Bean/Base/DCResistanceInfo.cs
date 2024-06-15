using ABBDataManagerSystem.Database;
using MySql.Data.MySqlClient;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class DCResistanceInfo
    {

        /**
         * CREATE TABLE DCResistanceInfo(
            ProductSequence VARCHAR(64),
            SortIndex INT,
            Tapping VARCHAR(12) NOT NULL DEFAULT "",
            AB FLOAT,
            BC FLOAT,
            CA FLOAT,   
            Temperature FLOAT,
            DateTime DATETIME,
            PRIMARY KEY(ProductSequence, SortIndex)
        );**/

        public string ProductSequence = string.Empty;
        public string Winding = string.Empty;
        public string Tapping = string.Empty;
        public float? AB = 0;
        public float? BC = 0;
        public float? CA = 0;
        public float? Temperature = 0;
        public float? MaxError = 0; // 最大不平衡差
        public DateTime? DateTime = null;

        public static string TABLE_NAME = "DCResistanceInfo";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ProductSequence", "出厂序号"},
            {"Winding", "绕组"},
            {"Tapping", "分接"},
            {"AB", "AB(Ω)"},
            {"BC", "BC(Ω)"},
            {"CA", "CA(Ω)"},
            {"Temperature", "温度"},
            {"MaxError", "最大不平衡差"},
            {"DateTime", "试验时间"},
        };


        public static List<DCResistanceInfo>? GetFromDB(string? sequence = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME} ";
            if (sequence != null)
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
            }
            queryDataSql += $" ORDER BY `Winding` ASC";
            Log.Info(queryDataSql);
            List<DCResistanceInfo>? records = DBConnector.QueryFromDB<DCResistanceInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new DCResistanceInfo
                {
                    ProductSequence = reader.GetString("ProductSequence"),
                    Winding = reader.GetString("Winding"),
                    Tapping = reader.GetString("Tapping"),
                    AB = !reader.IsDBNull("AB") ? reader.GetFloat("AB") : 0,
                    BC = !reader.IsDBNull("BC") ? reader.GetFloat("BC") : 0,
                    CA = !reader.IsDBNull("CA") ? reader.GetFloat("CA") : 0,
                    Temperature = !reader.IsDBNull("Temperature") ? reader.GetFloat("Temperature") : 0,
                    DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : null,
                    MaxError = !reader.IsDBNull("MaxError") ? reader.GetFloat("MaxError") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("DCResistanceInfo COUNT = " + records.Count);
            return records;
        }

        public bool InsertData()
        {
            try
            {
                using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
                {
                    connection.Open();
                    SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (workflow_id, Winding, Tapping, AB, BC, CA, Temperature, DateTime, MaxError) VALUES (@ProductSequence, @Winding, @Tapping, @AB, @BC, @CA, @Temperature, @DateTime, @MaxError)", connection);
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@Winding", Winding);
                    command.Parameters.AddWithValue("@Tapping", Tapping);
                    command.Parameters.AddWithValue("@AB", AB);
                    command.Parameters.AddWithValue("@BC", BC);
                    command.Parameters.AddWithValue("@CA", CA);
                    command.Parameters.AddWithValue("@DateTime", DateTime);
                    command.Parameters.AddWithValue("@Temperature", Temperature);
                    command.Parameters.AddWithValue("@MaxError", MaxError);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
            catch (Exception e)
            {
                Log.Error("fail to insert " + e.Message);
                return false;
            }
        }

        public static bool BatchInsertData(List<DCResistanceInfo> dataRows)
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
                        command.CommandText = $"INSERT INTO {TABLE_NAME} (workflow_id, Winding, Tapping, AB, BC, CA, Temperature, DateTime, MaxError) " + 
                            "VALUES (@ProductSequence, @Winding, @Tapping, @AB, @BC, @CA, @Temperature, @DateTime, @MaxError)";

                        command.Parameters.Add("@ProductSequence", MySqlDbType.String);
                        command.Parameters.Add("@Winding", MySqlDbType.String);
                        command.Parameters.Add("@Tapping", MySqlDbType.String);
                        command.Parameters.Add("@AB", MySqlDbType.Float);
                        command.Parameters.Add("@BC", MySqlDbType.Float);
                        command.Parameters.Add("@CA", MySqlDbType.Float);
                        command.Parameters.Add("@Temperature", MySqlDbType.Float);
                        command.Parameters.Add("@MaxError", MySqlDbType.Float);
                        command.Parameters.Add("@DateTime", MySqlDbType.DateTime);

                        //try
                        {
                            foreach (var row in dataRows)
                            {
                                command.Parameters["@ProductSequence"].Value = row.@ProductSequence;
                                command.Parameters["@Winding"].Value = row.@Winding;
                                command.Parameters["@Tapping"].Value = row.@Tapping;
                                command.Parameters["@AB"].Value = row.@AB;
                                command.Parameters["@BC"].Value = row.@BC;
                                command.Parameters["@CA"].Value = row.@CA;
                                command.Parameters["@Temperature"].Value = row.@Temperature;
                                command.Parameters["@MaxError"].Value = row.@MaxError;
                                command.Parameters["@DateTime"].Value = row.@DateTime;
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

        public static bool DeleteData(string sequence, string? winding = null)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                var sql = $"DELETE FROM {TABLE_NAME} WHERE workflow_id = @ProductSequence";
                if (winding != null)
                {
                    sql += $" AND Winding = @Winding";
                }
                SQLCommond command = new SQLCommond(sql, connection);
                command.Parameters.AddWithValue("@ProductSequence", sequence);
                if (winding != null)
                {
                    command.Parameters.AddWithValue("@Winding", winding);
                }
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "ProductSequence" || filedName == "Winding" || filedName == "Tapping");
            };
            return checkIsKeyField;
        }
    }
}
