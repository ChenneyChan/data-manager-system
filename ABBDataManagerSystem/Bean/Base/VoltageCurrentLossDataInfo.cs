using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class VoltageCurrentLossDataInfo
    {
        public static string TABLE_NAME = "VoltageCurrentRecord";
        /**
         * 
         * CREATE TABLE VoltageCurrentLossDataInfo (  
    ProductSequence VARCHAR(255),  
    LoadType VARCHAR(24),  
    ia FLOAT NOT NULL,  
    ib FLOAT NOT NULL,  
    ic FLOAT NOT NULL,  
    i3 FLOAT NOT NULL,  
    ua FLOAT NOT NULL,  
    ub FLOAT NOT NULL,  
    uc FLOAT NOT NULL,  
    u3 FLOAT NOT NULL,  
    pua FLOAT NOT NULL,  
    pub FLOAT NOT NULL,  
    puc FLOAT NOT NULL,  
    pu3 FLOAT NOT NULL, 
    pa FLOAT NOT NULL,  
    pb FLOAT NOT NULL,  
    pc FLOAT NOT NULL,  
    p3 FLOAT NOT NULL,
    PRIMARY KEY(ProductSequence, LoadType)
);
         */
        public static string LoadTypeNoLoad = "空载";
        public static string LoadTypeLoad = "负载";
        public static string LoadType10IN = "1.0IN";
        public static string LoadType18IN = "1.8IN";
        public static string LoadTypeZero = "零序";
        public static string LoadTypeNone = "None";

        public string ProductSequence = String.Empty;
        public string LoadType = LoadTypeNoLoad;
        public string TappingPosition = String.Empty;

        public DateTime DateTime;

        public float ia = 0;
        public float ib = 0;
        public float ic = 0;
        public float i3 = 0;

        public float ua = 0;
        public float ub = 0;
        public float uc = 0;
        public float u3 = 0;

        public float pua = 0;
        public float pub = 0;
        public float puc = 0;
        public float pu3 = 0;

        public float pa = 0;
        public float pb = 0;
        public float pc = 0;
        public float p3 = 0;

        public float fU = 0;
        public float Temperature = 0;


        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ProductSequence", "出厂序号"},
            {"LoadType", "空载负载"},
            {"DateTime", "试验时间"},
            {"ua", "电压有效值(A相)"},
            {"ub", "电压有效值(B相)"},
            {"uc", "电压有效值(C相)"},
            {"u3", "电压有效值(平均)"},
            {"pua", "电压平均值(A相)"},
            {"pub", "电压平均值(B相)"},
            {"puc", "电压平均值(C相)"},
            {"pu3", "电压平均值(平均)"},
            {"ia", "电流(A相)"},
            {"ib", "电流(B相)"},
            {"ic", "电流(C相)"},
            {"i3", "电流(平均)"},
            {"pa", "功率(A相)"},
            {"pb", "功率(B相)"},
            {"pc", "功率(C相)"},
            {"p3", "功率(总和)"},
            {"fU", "频率"},
            {"Temperature", "温度"},
        };

        public static string KeyField = "ProductSequence";

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                CreateSqliteTable(connection);

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (workflow_id, load_type, tapping_position, " +
                    $"uab, ubc, uca, uabc, " +
                    $"ia, ib, ic, iabc, " +
                    $"pa, pb, pc, p, temperature) VALUES (@ProductSequence, @LoadType, @TappingPosition, " +
                    $"@ua, @ub, @uc, @u3, " +
                    $"@ia, @ib, @ic, @i3, " +
                    $"@pa, @pb, @pc, @p3, @Temperature)", connection))
                {
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@LoadType", LoadType);
                    command.Parameters.AddWithValue("@TappingPosition", TappingPosition);
                    command.Parameters.AddWithValue("@ia", ia);
                    command.Parameters.AddWithValue("@ib", ib);
                    command.Parameters.AddWithValue("@ic", ic);
                    command.Parameters.AddWithValue("@i3", i3);
                    command.Parameters.AddWithValue("@ua", ua);
                    command.Parameters.AddWithValue("@ub", ub);
                    command.Parameters.AddWithValue("@uc", uc);
                    command.Parameters.AddWithValue("@u3", u3);
                    //command.Parameters.AddWithValue("@pua", ua);
                    //command.Parameters.AddWithValue("@pub", ub);
                    //command.Parameters.AddWithValue("@puc", uc);
                    //command.Parameters.AddWithValue("@pu3", u3);
                    command.Parameters.AddWithValue("@pa", pa);
                    command.Parameters.AddWithValue("@pb", pb);
                    command.Parameters.AddWithValue("@pc", pc);
                    command.Parameters.AddWithValue("@p3", p3);
                    //command.Parameters.AddWithValue("@fU", fU);
                    command.Parameters.AddWithValue("@Temperature", Temperature);
                    //command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public bool UpdateData()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET ia = @ia, ib = @ib, ic = @ic, iabc = @i3, " +
                $"uab = @ua, ubc = @ub, uca = @uc, uabc = @u3, " +
                $"pa = @pa, pb = @pb, pc = @pc, p = @p3, temperature = @Temperature WHERE workflow_id = @ProductSequence AND load_type = @LoadType " +
                "AND tapping_position = @TappingPosition";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@ProductSequence", ProductSequence);
                    command.Parameters.AddWithValue("@LoadType", LoadType);
                    command.Parameters.AddWithValue("@TappingPosition", TappingPosition);
                    command.Parameters.AddWithValue("@ia", ia);
                    command.Parameters.AddWithValue("@ib", ib);
                    command.Parameters.AddWithValue("@ic", ic);
                    command.Parameters.AddWithValue("@i3", i3);
                    command.Parameters.AddWithValue("@ua", ua);
                    command.Parameters.AddWithValue("@ub", ub);
                    command.Parameters.AddWithValue("@uc", uc);
                    command.Parameters.AddWithValue("@u3", u3);
                    command.Parameters.AddWithValue("@pa", pa);
                    command.Parameters.AddWithValue("@pb", pb);
                    command.Parameters.AddWithValue("@pc", pc);
                    command.Parameters.AddWithValue("@p3", p3);
                    //command.Parameters.AddWithValue("@pua", pua);
                    //command.Parameters.AddWithValue("@pub", pub);
                    //command.Parameters.AddWithValue("@puc", puc);
                    //command.Parameters.AddWithValue("@pu3", pu3);
                    //command.Parameters.AddWithValue("@fU", fU);
                    command.Parameters.AddWithValue("@Temperature", Temperature);
                    //command.Parameters.AddWithValue("@DateTime", DateTime.Now);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<VoltageCurrentLossDataInfo> ReadFromDB(bool withKey = false, string sequence = "", string lossType = "", string tappingPosition = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (withKey && sequence != "")
            {
                queryDataSql += $" WHERE ProductSequence = '{sequence}'";
                if (lossType != "")
                {
                    queryDataSql += $" AND LoadType = '{lossType}'";
                }
                if (tappingPosition != "")
                {
                    queryDataSql += $" AND TappingPosition = '{tappingPosition}'";
                }
            }
            List<VoltageCurrentLossDataInfo>? records = DBConnector.QueryFromDB<VoltageCurrentLossDataInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new VoltageCurrentLossDataInfo
                {
                    ProductSequence = reader.GetString("workflow_id"),
                    LoadType = reader.GetString("load_type"),
                    TappingPosition = reader.GetString("tapping_position"),
                    ia = !reader.IsDBNull("ia") ? (float)reader.GetDouble("ia") : 0,
                    ib = !reader.IsDBNull("ib") ? (float)reader.GetDouble("ib") : 0,
                    ic = !reader.IsDBNull("ic") ? (float)reader.GetDouble("ic") : 0,
                    i3 = !reader.IsDBNull("iabc") ? (float)reader.GetDouble("iabc") : 0,
                    ua = !reader.IsDBNull("uab") ? (float)reader.GetDouble("uab") : 0,
                    ub = !reader.IsDBNull("ubc") ? (float)reader.GetDouble("ubc") : 0,
                    uc = !reader.IsDBNull("uca") ? (float)reader.GetDouble("uca") : 0,
                    u3 = !reader.IsDBNull("uabc") ? (float)reader.GetDouble("uabc") : 0,
                    //pua = !reader.IsDBNull("pua") ? (float)reader.GetDouble("pua") : 0,
                    //pub = !reader.IsDBNull("pub") ? (float)reader.GetDouble("pub") : 0,
                    //puc = !reader.IsDBNull("puc") ? (float)reader.GetDouble("puc") : 0,
                    //pu3 = !reader.IsDBNull("pu3") ? (float)reader.GetDouble("pu3") : 0,
                    pa = !reader.IsDBNull("pa") ? (float)reader.GetDouble("pa") : 0,
                    pb = !reader.IsDBNull("pb") ? (float)reader.GetDouble("pb") : 0,
                    pc = !reader.IsDBNull("pc") ? (float)reader.GetDouble("pc") : 0,
                    p3 = !reader.IsDBNull("p") ? (float)reader.GetDouble("p") : 0,
                    //fU = !reader.IsDBNull("fU") ? (float)reader.GetFloat("fU") : 0,
                    Temperature = !reader.IsDBNull("temperature") ? (float)reader.GetFloat("temperature") : 0,
                    //DateTime = !reader.IsDBNull("DateTime") ? reader.GetDateTime("DateTime") : DateTime.Now,
                };
            });
            if (records == null)
            {
                return new List<VoltageCurrentLossDataInfo>();
            }
            Log.Info("VoltageCurrentLossDataInfo COUNT = " + records.Count);
            return records;
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
                where = $" WHERE workflow_id = '{filterSequence}'";
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT * FROM {TABLE_NAME} {where} ORDER BY workflow_id" +
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
                where = $" WHERE workflow_id = '{filterSequence}'";
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

        public static bool DeleteData(string sequence, string loadType, string tappingPosition = "")
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE workflow_id = @ProductSequence  AND load_type = @LoadType AND tapping_position = @TappingPosition", connection);
                command.Parameters.AddWithValue("@ProductSequence", sequence);
                command.Parameters.AddWithValue("@LoadType", loadType);
                command.Parameters.AddWithValue("@TappingPosition", tappingPosition);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        private void CreateSqliteTable(SQLConnection connection)
        {
            if (!DBConnector.USING_SQLITE)
            {
                return;
            }
            // 创建数据库表
            string createTableSql = $"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (  " +
                $"    ProductSequence TEXT," +
                $"    LoadType TEXT," +
                $"    ia REAL," +
                $"    ib REAL," +
                $"    ic REAL," +
                $"    i3 REAL," +
                $"    ua REAL," +
                $"    ub REAL," +
                $"    uc REAL," +
                $"    u3 REAL," +
                $"    pua REAL," +
                $"    pub REAL," +
                $"    puc REAL," +
                $"    pu3 REAL," +
                $"    pa REAL," +
                $"    pb REAL," +
                $"    pc REAL," +
                $"    p3 REAL, " +
                $"    Temperature REAL, " +
                $"    PRIMARY KEY(ProductSequence, LoadType));";
            using (SQLCommond createTableCmd = new SQLCommond(createTableSql, connection))
            {
                createTableCmd.ExecuteNonQuery();
            }
        }

        public static void Clone(VoltageCurrentLossDataInfo src, VoltageCurrentLossDataInfo dst)
        {
            dst.ProductSequence = src.ProductSequence;
            dst.LoadType = src.LoadType;

            dst.ia = src.ia;
            dst.ib = src.ib;
            dst.ic = src.ic;
            dst.i3 = src.i3;

            dst.ua = src.ua;
            dst.ub = src.ub;
            dst.uc = src.uc;
            dst.u3 = src.u3;

            dst.pua = src.pua;
            dst.pub = src.pub;
            dst.puc = src.puc;
            dst.pu3 = src.pu3;

            dst.pa = src.pa;
            dst.pb = src.pb;
            dst.pc = src.pc;
            dst.p3 = src.p3;
            dst.fU = src.fU;
            dst.Temperature = src.Temperature;
            dst.DateTime = src.DateTime;
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "ProductSequence" || filedName == "LoadType");
            };
            return checkIsKeyField;
        }
    }
}
