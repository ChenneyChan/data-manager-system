using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class PartialDischargeInfo
    {

        /**
         * CREATE TABLE PartialDischarge (  
            Sequence VARCHAR(255),  
            Voltage VARCHAR(255),  
            dischargeA FLOAT,  
            dischargeB FLOAT,  
            dischargeC FLOAT,
            PRIMARY KEY(Sequence, Voltage)
        );**/

        public string Sequence = string.Empty;
        public string Voltage = string.Empty;
        public float DischargeA = 0;
        public float DischargeB = 0;
        public float DischargeC = 0;
        public float DischargeD = 0;
        public DateTime? DateTime = null;

        public static string TABLE_NAME = "PartialDischarge";
        public static string VoltageType1 = "1.3Ur";
        public static string VoltageType2 = "背景噪声";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"Sequence", "出厂序号"},
            {"Voltage", "试验电压"},
            {"DischargeA", "A相（pC）"},
            {"DischargeB", "B相（pC）"},
            {"DischargeC", "C相（pC）"},
            //{"DischargeD", "均值（pC）"},
            {"DateTime", "试验时间" }
        };

        public PartialDischargeInfo()
        {
        }

        public bool InsertData()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                CreateSqliteTable(connection);
                SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (Sequence, Voltage, DischargeA, DischargeB, DischargeC, DateTime) VALUES (@Sequence, @Voltage, @DischargeA, @DischargeB, @DischargeC, @DateTime)", connection);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                command.Parameters.AddWithValue("@Voltage", Voltage);
                command.Parameters.AddWithValue("@DischargeA", DischargeA);
                command.Parameters.AddWithValue("@DischargeB", DischargeB);
                command.Parameters.AddWithValue("@DischargeC", DischargeC);
                command.Parameters.AddWithValue("@DateTime", System.DateTime.Now);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public bool UpdateData()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"UPDATE {TABLE_NAME} SET DischargeA = @DischargeA, DischargeB = @DischargeB, DischargeC = @DischargeC, DateTime = @DateTime WHERE Sequence = @Sequence AND Voltage = @Voltage", connection);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                command.Parameters.AddWithValue("@Voltage", Voltage);
                command.Parameters.AddWithValue("@DischargeA", DischargeA);
                command.Parameters.AddWithValue("@DischargeB", DischargeB);
                command.Parameters.AddWithValue("@DischargeC", DischargeC);
                command.Parameters.AddWithValue("@DateTime", System.DateTime.Now);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static bool DeleteData(string Sequence, string Voltage)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE Sequence = @Sequence AND Voltage = @Voltage", connection);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                command.Parameters.AddWithValue("@Voltage", Voltage);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static List<PartialDischargeInfo>? GetFromDB(string? sequence = null, string? voltageType = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != null)
            {
                queryDataSql += $" WHERE Sequence = '{sequence}'";
                if (voltageType != null)
                {
                    queryDataSql += $" AND Voltage = '{voltageType}'";
                }
            }
            Log.Info(queryDataSql);
            List<PartialDischargeInfo>? records = DBConnector.QueryFromDB<PartialDischargeInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new PartialDischargeInfo
                {
                    Sequence = reader.GetString("Sequence"),
                    Voltage = reader.GetString("Voltage"),
                    DischargeA = !reader.IsDBNull("DischargeA") ? reader.GetFloat("DischargeA") : 0,
                    DischargeB = !reader.IsDBNull("DischargeB") ? reader.GetFloat("DischargeB") : 0,
                    DischargeC = !reader.IsDBNull("DischargeC") ? reader.GetFloat("DischargeC") : 0,
                    DischargeD = !reader.IsDBNull("DischargeD") ? reader.GetFloat("DischargeD") : 0,
                    DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("PartialDischargeInfo COUNT = " + records.Count);
            return records;
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page, string? filterSequence = null)
        {
            if (page < 0)
            {
                return;
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string where = "";
                if (filterSequence != null && filterSequence.Length > 0)
                {
                    where = $" WHERE Sequence = '{filterSequence}'";
                }
                string query = $"SELECT * FROM {TABLE_NAME} {where} ORDER BY Sequence" +
                    $" LIMIT {pageSize} OFFSET {pageSize * page}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    SQLDataAdapter adapter = new SQLDataAdapter(queryDataCmd);
                    adapter.Fill(dt);
                }
            }
        }

        public static int GetTotalCount(string? filterSequence = null)
        {
            string where = "";
            if (filterSequence != null && filterSequence.Length > 0)
            {
                where = $" WHERE Sequence = '{filterSequence}'";
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT count(*) FROM {TABLE_NAME} {where}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    using (SQLDataReader reader = queryDataCmd.ExecuteReader()){
                        while (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                    }
                }
            }
            return 0;
        }

        private void CreateSqliteTable(SQLConnection connection)
        {
            if (!DBConnector.USING_SQLITE)
            {
                return;
            }
            // 创建数据库表
            string createTableSql = $"CREATE TABLE {TABLE_NAME} (  \r\n    Sequence TEXT PRIMARY KEY,  \r\n    dischargeA REAL,  \r\n    dischargeB REAL,  \r\n    dischargeC REAL  \r\n);";
            using (SQLCommond createTableCmd = new SQLCommond(createTableSql, connection))
            {
                createTableCmd.ExecuteNonQuery();
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "Sequence" || filedName == "Voltage");
            };
            return checkIsKeyField;
        }
    }
}
