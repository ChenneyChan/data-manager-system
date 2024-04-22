using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
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
        public int SortIndex = 0;
        public string Tapping = string.Empty;
        public float AB = 0;
        public float BC = 0;
        public float CA = 0;
        public float Temperature = 0;
        public DateTime? DateTime = null;

        public static string TABLE_NAME = "DCResistanceInfo";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ProductSequence", "出厂序号"},
            {"SortIndex", "分接"},
            {"Tapping", "一次"},
            {"AB", "AB(Ω)"},
            {"BC", "BC(Ω)"},
            {"CA", "CA(Ω)"},
            {"Temperature", "温度"},
            {"DateTime", "试验时间"},
        };

        public DCResistanceInfo()
        {
        }

        public static List<DCResistanceInfo>? GetFromDB(string? sequence = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME} ";
            if (sequence != null)
            {
                queryDataSql += $" WHERE ProductSequence = '{sequence}'";
            }
            queryDataSql += $" ORDER BY `SortIndex` ASC";
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
                    SortIndex = reader.GetInt32("SortIndex"),
                    AB = !reader.IsDBNull("AB") ? reader.GetFloat("AB") : 0,
                    BC = !reader.IsDBNull("BC") ? reader.GetFloat("BC") : 0,
                    CA = !reader.IsDBNull("CA") ? reader.GetFloat("CA") : 0,
                    Tapping = !reader.IsDBNull("Tapping") ? reader.GetString("Tapping") : "",
                    Temperature = !reader.IsDBNull("Temperature") ? reader.GetFloat("Temperature") : 0,
                    DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : null,
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
                    CreateSqliteTable(connection);
                    SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ProductSequence, SortIndex, Tapping, AB, BC, CA, Temperature, DateTime) VALUES (@ProductSequence, @SortIndex, @Tapping, @AB, @BC, @CA, @Temperature, @DateTime)", connection);
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@SortIndex", SortIndex);
                    command.Parameters.AddWithValue("@Tapping", Tapping);
                    command.Parameters.AddWithValue("@AB", AB);
                    command.Parameters.AddWithValue("@BC", BC);
                    command.Parameters.AddWithValue("@CA", CA);
                    command.Parameters.AddWithValue("@DateTime", DateTime);
                    command.Parameters.AddWithValue("@Temperature", Temperature);
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

        public bool UpdateData()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"UPDATE {TABLE_NAME} SET Tapping = @Tapping, Temperature = @Temperature, DateTime = @DateTime, AB = @AB, BC = @BC, CA = @CA WHERE ProductSequence = @ProductSequence AND SortIndex = @SortIndex", connection);
                command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                command.Parameters.AddWithValue("@SortIndex", SortIndex);
                command.Parameters.AddWithValue("@Tapping", Tapping);
                command.Parameters.AddWithValue("@AB", AB);
                command.Parameters.AddWithValue("@BC", BC);
                command.Parameters.AddWithValue("@CA", CA);
                command.Parameters.AddWithValue("@DateTime", DateTime);
                command.Parameters.AddWithValue("@Temperature", Temperature);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static bool DeleteData(string sequence, int? sortIndex = null)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                var sql = $"DELETE FROM {TABLE_NAME} WHERE ProductSequence = @ProductSequence";
                if (sortIndex != null)
                {
                    sql += $" AND SortIndex = @SortIndex";
                }
                SQLCommond command = new SQLCommond(sql, connection);
                command.Parameters.AddWithValue("@ProductSequence", sequence);
                if (sortIndex != null)
                {
                    command.Parameters.AddWithValue("@SortIndex", sortIndex);
                }
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static int GetTotalCount(string? filterSequence = null)
        {
            string where = "";
            if (filterSequence != null && filterSequence.Length > 0)
            {
                where = $" WHERE ProductSequence = '{filterSequence}'";
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT count(*) FROM {TABLE_NAME} {where}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    using (SQLDataReader reader = queryDataCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return 0;
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page, string? filterSequence = null)
        {
            if (page < 0)
            {
                return;
            }
            string where = "";
            if (filterSequence != null && filterSequence.Length > 0)
            {
                where = $" WHERE ProductSequence = '{filterSequence}'";
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT * FROM {TABLE_NAME} {where} ORDER BY ProductSequence" +
                    $" LIMIT {pageSize} OFFSET {pageSize * page}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    SQLDataAdapter adapter = new SQLDataAdapter(queryDataCmd);
                    adapter.Fill(dt);
                }
            }
        }

        private void CreateSqliteTable(SQLConnection connection)
        {
            if (!DBConnector.USING_SQLITE)
            {
                return;
            }
            // 创建数据库表
            string createTableSql = $"CREATE TABLE {TABLE_NAME} (  \r\n    ProductSequence TEXT PRIMARY KEY,  \r\n    dischargeA REAL,  \r\n    dischargeB REAL,  \r\n    dischargeC REAL  \r\n);";
            using (SQLCommond createTableCmd = new SQLCommond(createTableSql, connection))
            {
                createTableCmd.ExecuteNonQuery();
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "ProductSequence" || filedName == "SortIndex");
            };
            return checkIsKeyField;
        }
    }
}
