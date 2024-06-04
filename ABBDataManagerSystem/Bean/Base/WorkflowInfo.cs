using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class WorkflowInfo
    {
        public static string TABLE_NAME = "workflow";
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
        public string ID = String.Empty;
        public string WorkflowType = String.Empty;
        public string TappingPosition = String.Empty;

        public int RatedPower;
        public int RatedPower1;
        public int RatedPower2;

        public float RatedVoltageHv;
        public float RatedVoltageLv;
        public float RatedVoltageYv;
        public float RatedCurrentHv;
        public float RatedCurrentLv;
        public float RatedCurrentYv;

        public string No   = String.Empty;
        public string CAL  = String.Empty;
        public string Type = String.Empty;
        public string Phase   = String.Empty;
        public string CONNSymbol = String.Empty;
        public string TappingVoltages = String.Empty;
        public string RatedVoltageInterval = String.Empty;

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "工作令编号 "},
            {"WorkflowType", "工作令类型"},
            {"RatedPower", "额定容量"},
            {"RatedPower1", "额定容量1"},
            {"RatedPower2", "额定容量2"},
            {"RatedVoltageHv", "高压侧额定电压"},
            {"RatedVoltageLv", "低压侧额定电压"},
            {"RatedVoltageYv", "低压侧额定电压2"},
            {"RatedCurrentHv", "高压侧额定电流"},
            {"RatedCurrentLv", "低压侧额定电流"},
            {"RatedCurrentYv", "低压侧额定电流2"},
            {"No", "图号" },
            {"CAL", "计算单号" },
            {"Type", "型号" },
            {"Phase", "相数" },
            {"CONNSymbol", "联结组别" },
            {"TappingVoltages", "分接电压"},
            {"RatedVoltageInterval", "分接"},
        };

        public static string KeyField = "ProductSequence";

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, WorkflowType, TappingPosition, RatedPower, RatedPower1, RatedPower2, " +
                    $"RatedVoltageHv, RatedVoltageLv, RatedVoltageYv, RatedCurrentHv, RatedCurrentLv, RatedCurrentYv, " +
                    $"No, CAL, Type, Phase, CONNSymbol, TappingVoltages, RatedVoltageInterval) VALUES (@ID, @WorkflowType, @TappingPosition, @RatedPower, @RatedPower1, @RatedPower2, " +
                    $"@RatedVoltageHv, @RatedVoltageLv, @RatedVoltageYv, @RatedCurrentHv, @RatedCurrentLv, @RatedCurrentYv, @No, @CAL, @Type, @Phase, " +
                    $"@CONNSymbol, @TappingVoltages, @RatedVoltageInterval)", connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@WorkflowType", WorkflowType);
                    command.Parameters.AddWithValue("@TappingPosition", TappingPosition);
                    command.Parameters.AddWithValue("@RatedPower", RatedPower);
                    command.Parameters.AddWithValue("@RatedPower1", RatedPower1);
                    command.Parameters.AddWithValue("@RatedPower2", RatedPower2);
                    command.Parameters.AddWithValue("@RatedVoltageHv", RatedVoltageHv);
                    command.Parameters.AddWithValue("@RatedVoltageLv", RatedVoltageLv);
                    command.Parameters.AddWithValue("@RatedVoltageYv", RatedVoltageYv);
                    command.Parameters.AddWithValue("@RatedCurrentHv", RatedCurrentHv);
                    command.Parameters.AddWithValue("@RatedCurrentLv", RatedCurrentLv);
                    command.Parameters.AddWithValue("@RatedCurrentYv", RatedCurrentYv);
                    command.Parameters.AddWithValue("@No", No);
                    command.Parameters.AddWithValue("@CAL", CAL);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@Phase", Phase);
                    command.Parameters.AddWithValue("@CONNSymbol", CONNSymbol);
                    command.Parameters.AddWithValue("@TappingVoltages", TappingVoltages);
                    command.Parameters.AddWithValue("@RatedVoltageInterval", RatedVoltageInterval);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public bool UpdateData()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET RatedPower = @RatedPower, RatedPower1 = @RatedPower1, " +
                $"RatedPower2 = @RatedPower2, RatedVoltageHv = @RatedVoltageHv, RatedVoltageLv = @RatedVoltageLv, RatedVoltageYv = @RatedVoltageYv, RatedCurrentHv = @RatedCurrentHv, " +
                $"RatedCurrentLv = @RatedCurrentLv, RatedCurrentYv = @RatedCurrentYv, No = @No, CAL = @CAL, Type = @Type, Phase = @Phase, CONNSymbol = @CONNSymbol, " +
                $"TappingVoltages = @TappingVoltages, RatedVoltageInterval = @RatedVoltageInterval WHERE ID = @ID, WorkflowType = @WorkflowType, TappingPosition = @TappingPosition";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@WorkflowType", WorkflowType);
                    command.Parameters.AddWithValue("@TappingPosition", TappingPosition);
                    command.Parameters.AddWithValue("@RatedPower", RatedPower);
                    command.Parameters.AddWithValue("@RatedPower1", RatedPower1);
                    command.Parameters.AddWithValue("@RatedPower2", RatedPower2);
                    command.Parameters.AddWithValue("@RatedVoltageHv", RatedVoltageHv);
                    command.Parameters.AddWithValue("@RatedVoltageLv", RatedVoltageLv);
                    command.Parameters.AddWithValue("@RatedVoltageYv", RatedVoltageYv);
                    command.Parameters.AddWithValue("@RatedCurrentHv", RatedCurrentHv);
                    command.Parameters.AddWithValue("@RatedCurrentLv", RatedCurrentLv);
                    command.Parameters.AddWithValue("@RatedCurrentYv", RatedCurrentYv);
                    command.Parameters.AddWithValue("@No", No);
                    command.Parameters.AddWithValue("@CAL", CAL);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@Phase", Phase);
                    command.Parameters.AddWithValue("@CONNSymbol", CONNSymbol);
                    command.Parameters.AddWithValue("@TappingVoltages", TappingVoltages);
                    command.Parameters.AddWithValue("@RatedVoltageInterval", RatedVoltageInterval);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<WorkflowInfo> ReadFromDB(bool withKey = false, string ID = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (withKey && ID != "")
            {
                queryDataSql += $" WHERE ID = '{ID}'";
            }
            List<WorkflowInfo>? records = DBConnector.QueryFromDB<WorkflowInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new WorkflowInfo
                {
                    ID = reader.GetString("ID"),
                    WorkflowType = reader.GetString("WorkflowType"),
                    TappingPosition = reader.GetString("TappingPosition"),
                    RatedPower = reader.GetInt32("RatedPower"),
                    RatedPower1 = reader.GetInt32("RatedPower1"),
                    RatedPower2 = reader.GetInt32("RatedPower2"),
                    RatedVoltageHv = reader.GetFloat("RatedVoltageHv"),
                    RatedVoltageLv = reader.GetFloat("RatedVoltageLv"),
                    RatedVoltageYv = reader.GetFloat("RatedVoltageYv"),
                    RatedCurrentHv = reader.GetFloat("RatedCurrentHv"),
                    RatedCurrentLv = reader.GetFloat("RatedCurrentLv"),
                    RatedCurrentYv = reader.GetFloat("RatedCurrentYv"),
                    No = reader.GetString("No"),
                    CAL = reader.GetString("CAL"),
                    Type = reader.GetString("Type"),
                    Phase = reader.GetString("Phase"),
                    CONNSymbol = reader.GetString("CONNSymbol"),
                    TappingVoltages = reader.GetString("TappingVoltages"),
                    RatedVoltageInterval = reader.GetString("RatedVoltageInterval"),
                };
            });
            if (records == null)
            {
                return new List<WorkflowInfo>();
            }
            Log.Info("WorkflowInfo COUNT = " + records.Count);
            return records;
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page, string? filterId= null)
        {
            if (page < 0)
            {
                return;
            }
            string where = "";
            if (filterId != null && filterId.Length > 0)
            {
                where = $" WHERE ID = '{filterId}'";
            }
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT * FROM {TABLE_NAME} {where} ORDER BY ID" +
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
                where = $" WHERE ID = '{filterSequence}'";
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

        public static bool DeleteData(string id)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", id);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static void Clone(WorkflowInfo src, WorkflowInfo dst)
        {
            dst.ID = src.ID;
            dst.WorkflowType = src.WorkflowType;
            dst.TappingPosition = src.TappingPosition;
            dst.RatedPower = src.RatedPower;
            dst.RatedPower1 = src.RatedPower1;
            dst.RatedPower2 = src.RatedPower2;
            dst.RatedVoltageHv = src.RatedVoltageHv;
            dst.RatedVoltageLv = src.RatedVoltageLv;
            dst.RatedVoltageYv = src.RatedVoltageYv;
            dst.RatedCurrentHv = src.RatedCurrentHv;
            dst.RatedCurrentLv = src.RatedCurrentLv;
            dst.RatedCurrentYv = src.RatedCurrentYv;
            dst.No = src.No;
            dst.CAL = src.CAL;
            dst.Type = src.Type;
            dst.Phase = src.Phase;
            dst.CONNSymbol = src.CONNSymbol;
            dst.TappingVoltages = src.TappingVoltages;
            dst.RatedVoltageInterval = src.RatedVoltageInterval;
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "ID");
            };
            return checkIsKeyField;
        }
    }
}
