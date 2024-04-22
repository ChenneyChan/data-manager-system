using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
{


    public class ProductInfo
    {
        public static string TABLE_NAME = "ProductInfo";

        //DDL
        /* CREATE TABLE `ProductInfo` (
        `ID` BIGINT PRIMARY KEY auto_increment,  
        `Sequence` VARCHAR(64) NOT NULL,
        `UserName` VARCHAR(64),
        `FigureNumber` VARCHAR(64),
        `TestingDate` DATETIME,
        );
        */

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "ID"},
            {"Sequence", "出厂序号"},
            {"UserName", "用户名称"},
            {"FigureNumber", "代号尾数（图号）"},
            {"TestingDate", "试验日期"},
        };

        public int ID  = 0; // ID，唯一自增Key

        public string Sequence = String.Empty; // 出厂序号

        public string UserName = String.Empty; // 用户名称

        public string FigureNumber = String.Empty; // 代号尾数（图号）

        public DateTime? TestingDate = null; // 试验日期


        public static List<ProductInfo>? GetFromDB(string? sequence = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != null)
            {
                queryDataSql += $" WHERE Sequence = '{sequence}'";
            }
            List<ProductInfo>? records = DBConnector.QueryFromDB<ProductInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new ProductInfo
                {
                    ID = reader.GetInt32(0),
                    Sequence = reader.GetString(1),
                    UserName = !reader.IsDBNull("UserName") ? reader.GetString("UserName") : "",
                    FigureNumber = !reader.IsDBNull("FigureNumber") ? reader.GetString("FigureNumber") : "",
                    TestingDate = !reader.IsDBNull("TestingDate") ? reader.GetDateTime("TestingDate") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("ProductInfo COUNT = " + records.Count);
            return records;
        }

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                CreateSqliteTable(connection);

                SQLCommond sqliteCmd;
                sqliteCmd = connection.CreateCommand();
                sqliteCmd.CommandText = $"INSERT INTO {TABLE_NAME} (Sequence, UserName, FigureNumber, TestingDate) VALUES " +
                    $"(@Sequence, @UserName, @FigureNumber, @TestingDate)";
                sqliteCmd.Parameters.AddWithValue("@Sequence", Sequence);
                sqliteCmd.Parameters.AddWithValue("@UserName", UserName);
                sqliteCmd.Parameters.AddWithValue("@FigureNumber", FigureNumber);
                sqliteCmd.Parameters.AddWithValue("@TestingDate", TestingDate);
                int count = sqliteCmd.ExecuteNonQuery();
                return count > 0;
            }
        }

        public bool UpdateData()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();
                string sql = $"UPDATE {TABLE_NAME} SET Sequence = @Sequence, UserName = @UserName, FigureNumber = @FigureNumber, " +
                    $"TestingDate = @TestingDate  WHERE ID = @ID";
                SQLCommond command = new SQLCommond(sql, connection);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.AddWithValue("@FigureNumber", FigureNumber);
                command.Parameters.AddWithValue("@TestingDate", TestingDate);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page)
        {
            if (page < 0)
            {
                return;
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT * FROM {TABLE_NAME} ORDER BY ID" +
                    $" LIMIT {pageSize} OFFSET {pageSize * page}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    SQLDataAdapter adapter = new SQLDataAdapter(queryDataCmd);
                    adapter.Fill(dt);
                }
            }
        }

        public static int GetTotalCount()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT count(*) FROM {TABLE_NAME}";
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

        private void CreateSqliteTable(SQLConnection connection)
        {
            if (!DBConnector.USING_SQLITE)
            {
                return;
            }
            // 创建数据库表
            string createTableSql = $"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (" +
                $"    ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                $"    Sequence TEXT," +
                $"    UserName TEXT," +
                $"    FigureNumber TEXT," +
                $"    TestingDate DATETIME);";
            using (SQLCommond createTableCmd = new SQLCommond(createTableSql, connection))
            {
                createTableCmd.ExecuteNonQuery();
            }
        }

        public static bool DeleteData(string Sequence)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE Sequence = @Sequence", connection);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }


        public override string ToString()
        {
            return $"ID: {ID}, Sequence: {Sequence}, UserName: {UserName}, FigureNumber: {FigureNumber}, TestingDate: {TestingDate}";
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "Sequence" || filedName == "ID");
            };
            return checkIsKeyField;
        }
    }
}
