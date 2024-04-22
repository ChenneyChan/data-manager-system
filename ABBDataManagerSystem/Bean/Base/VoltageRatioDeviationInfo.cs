using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class VoltageRatioDeviationInfo
    {

        /**
         * CREATE TABLE VoltageRatioDeviationInfo(
            ProductSequence VARCHAR(64),
            SortIndex INT,
            A FLOAT,
            B FLOAT,
            C FLOAT, 
            DateTime DATETIME,
            PRIMARY KEY(ProductSequence, SortIndex)
        );
        **/

        public string ProductSequence = string.Empty;
        public int SortIndex = 0;
        public float A = 0;
        public float B = 0;
        public float C = 0;
        public DateTime? DateTime = null;

        public static string TABLE_NAME = "VoltageRatioDeviationInfo";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ProductSequence", "出厂序号"},
            {"SortIndex", "分接"},
            {"A", "A(%)"},
            {"B", "B(%)"},
            {"C", "C(%)"},
            {"DateTime", "试验时间"},
        };

        public VoltageRatioDeviationInfo()
        {
        }

        public static List<VoltageRatioDeviationInfo>? GetFromDB(string? sequence = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME} ";
            if (sequence != null)
            {
                queryDataSql += $" WHERE ProductSequence = '{sequence}'";
            }
            queryDataSql += $" ORDER BY `SortIndex` ASC";
            Log.Info(queryDataSql);
            List<VoltageRatioDeviationInfo>? records = DBConnector.QueryFromDB<VoltageRatioDeviationInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new VoltageRatioDeviationInfo
                {
                    ProductSequence = reader.GetString("ProductSequence"),
                    SortIndex = reader.GetInt32("SortIndex"),
                    A = !reader.IsDBNull("A") ? reader.GetFloat("A") : 0,
                    B = !reader.IsDBNull("B") ? reader.GetFloat("B") : 0,
                    C = !reader.IsDBNull("C") ? reader.GetFloat("C") : 0,
                    DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("VoltageRatioDeviationInfo COUNT = " + records.Count);
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
                    SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ProductSequence, SortIndex, A, B, C, DateTime) VALUES (@ProductSequence, @SortIndex, @A, @B, @C, @Temperature, @DateTime)", connection);
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@SortIndex", SortIndex);
                    command.Parameters.AddWithValue("@A", A);
                    command.Parameters.AddWithValue("@B", B);
                    command.Parameters.AddWithValue("@C", C);
                    command.Parameters.AddWithValue("@DateTime", DateTime);
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
                SQLCommond command = new SQLCommond($"UPDATE {TABLE_NAME} SET DateTime = @DateTime, A = @A, B = @B, C = @C WHERE ProductSequence = @ProductSequence AND SortIndex = @SortIndex", connection);
                command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                command.Parameters.AddWithValue("@SortIndex", SortIndex);
                command.Parameters.AddWithValue("@A", A);
                command.Parameters.AddWithValue("@B", B);
                command.Parameters.AddWithValue("@C", C);
                command.Parameters.AddWithValue("@DateTime", DateTime);
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
