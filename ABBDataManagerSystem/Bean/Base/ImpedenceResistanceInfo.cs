using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class ImpedenceResistanceInfo
    {

        /**
         * CREATE TABLE ImpedenceResistanceInfo(
            ProductSequence VARCHAR(255),
            SortIndex INT,
            Resistance FLOAT,
            Impedance FLOAT,
            Temperature FLOAT,
            DateTime DateTime,
            PRIMARY KEY(ProductSequence, SortIndex)
        );**/

        public string ProductSequence = string.Empty;
        public int SortIndex = 0;
        public float Impedance = 0;
        public float Resistance = 0;
        public float Temperature = 0;
        public DateTime? DateTime = null;

        public static string TABLE_NAME = "ImpedenceResistanceInfo";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ProductSequence", "出厂序号"},
            {"SortIndex", "分接"},
            {"Resistance", "直流电阻"},
            {"Impedance", "阻抗"},
            {"Temperature", "温度"},
            {"DateTime", "试验时间"},
        };

        public ImpedenceResistanceInfo()
        {
        }

        public static List<ImpedenceResistanceInfo>? GetFromDB(string? sequence = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME} ";
            if (sequence != null)
            {
                queryDataSql += $" WHERE ProductSequence = '{sequence}'";
            }
            queryDataSql += $" ORDER BY `SortIndex` ASC";
            Log.Info(queryDataSql);
            List<ImpedenceResistanceInfo>? records = DBConnector.QueryFromDB<ImpedenceResistanceInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new ImpedenceResistanceInfo
                {
                    ProductSequence = reader.GetString("ProductSequence"),
                    SortIndex = reader.GetInt32("SortIndex"),
                    Resistance = !reader.IsDBNull("Resistance") ? reader.GetFloat("Resistance") : 0,
                    Impedance = !reader.IsDBNull("Impedance") ? reader.GetFloat("Impedance") : 0,
                    Temperature = !reader.IsDBNull("Temperature") ? reader.GetFloat("Temperature") : 0,
                    DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("ImpedenceResistanceInfo COUNT = " + records.Count);
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
                    SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ProductSequence, SortIndex, Resistance, Impedance, Temperature) VALUES (@ProductSequence, @SortIndex, @Resistance, @Impedance, @Temperature)", connection);
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@SortIndex", SortIndex);
                    command.Parameters.AddWithValue("@Resistance", Resistance);
                    command.Parameters.AddWithValue("@Impedance", Impedance);
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
                SQLCommond command = new SQLCommond($"UPDATE {TABLE_NAME} SET Resistance = @Resistance, Impedance = @Impedance, Temperature = @Temperature WHERE ProductSequence = @ProductSequence AND SortIndex = @SortIndex", connection);
                command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                command.Parameters.AddWithValue("@Resistance", Resistance);
                command.Parameters.AddWithValue("@Impedance", Impedance);
                command.Parameters.AddWithValue("@Temperature", Temperature);
                command.Parameters.AddWithValue("@SortIndex", SortIndex);
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
