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
    WorkflowId VARCHAR(255),  
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
    PRIMARY KEY(WorkflowId, LoadType)
);
         */
        public string ID = String.Empty;
        public string WorkflowType = String.Empty;

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
        public int Phase;
        public string CONNSymbol = String.Empty;
        public string TappingVoltages = String.Empty;
        public string RatedVoltageInterval = String.Empty;
        public float? TempRiseTestingVoltage = null;
        public float? TempRiseTestingCurrent = null;
        public string TempRiseCorrectionFactor = String.Empty;
        public string TempRiseRelativeTo = String.Empty;

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
            {"TempRiseTestingVoltage", "温升试验电压" },
            {"TempRiseTestingCurrent", "温升试验电流" },
            {"TempRiseCorrectionFactor", "温升校正系数" },
            {"TempRiseRelativeTo", "温升相对于" },
        };

        public static string KeyField = "WorkflowId";

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, WorkflowType, RatedPower, RatedPower1, RatedPower2, " +
                    "RatedVoltageHv, RatedVoltageLv, RatedVoltageYv, RatedCurrentHv, RatedCurrentLv, RatedCurrentYv, No, CAL, Type, Phase, CONNSymbol, " +
                    "TappingVoltages, RatedVoltageInterval, TempRiseTestingVoltage, TempRiseTestingCurrent, TempRiseCorrectionFactor, TempRiseRelativeTo, ) " + 
                    "VALUES(@ID, @WorkflowType, @RatedPower, @RatedPower1, @RatedPower2, @RatedVoltageHv, @RatedVoltageLv, @RatedVoltageYv, " + 
                    "@RatedCurrentHv, @RatedCurrentLv, @RatedCurrentYv, @No, @CAL, @Type, @Phase, @CONNSymbol, @TappingVoltages, @RatedVoltageInterval, " +
                    "@TempRiseTestingVoltage, @TempRiseTestingCurrent, @TempRiseCorrectionFactor, @TempRiseRelativeTo)", connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@WorkflowType", WorkflowType);
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
                    command.Parameters.AddWithValue("@TempRiseTestingVoltage", TempRiseTestingVoltage);
                    command.Parameters.AddWithValue("@TempRiseTestingCurrent", TempRiseTestingCurrent);
                    command.Parameters.AddWithValue("@TempRiseCorrectionFactor", TempRiseCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public bool UpdateData()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET WorkflowType = @WorkflowType, RatedPower = @RatedPower, RatedPower1 = @RatedPower1, " +
                $"RatedPower2 = @RatedPower2, RatedVoltageHv = @RatedVoltageHv, RatedVoltageLv = @RatedVoltageLv, RatedVoltageYv = @RatedVoltageYv, RatedCurrentHv = @RatedCurrentHv, " +
                $"RatedCurrentLv = @RatedCurrentLv, RatedCurrentYv = @RatedCurrentYv, No = @No, CAL = @CAL, Type = @Type, Phase = @Phase, CONNSymbol = @CONNSymbol, " +
                $"TappingVoltages = @TappingVoltages, RatedVoltageInterval = @RatedVoltageInterval, TempRiseTestingVoltage = @TempRiseTestingVoltage, TempRiseTestingCurrent = @TempRiseTestingCurrent, TempRiseCorrectionFactor = @TempRiseCorrectionFactor, TempRiseRelativeTo = @TempRiseRelativeTo, " +
                "WHERE ID = @ID";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@WorkflowType", WorkflowType);
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
                    command.Parameters.AddWithValue("@TempRiseTestingVoltage", TempRiseTestingVoltage);
                    command.Parameters.AddWithValue("@TempRiseTestingCurrent", TempRiseTestingCurrent);
                    command.Parameters.AddWithValue("@TempRiseCorrectionFactor", TempRiseCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateTempRiseFields()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET TempRiseTestingVoltage = @TempRiseTestingVoltage, TempRiseTestingCurrent = @TempRiseTestingCurrent, " + 
                "TempRiseCorrectionFactor = @TempRiseCorrectionFactor, TempRiseRelativeTo = @TempRiseRelativeTo " +
                "WHERE ID = @ID";

            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open(); using (SQLCommond command = new SQLCommond(updateSql, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@TempRiseTestingVoltage", TempRiseTestingVoltage);
                    command.Parameters.AddWithValue("@TempRiseTestingCurrent", TempRiseTestingCurrent);
                    command.Parameters.AddWithValue("@TempRiseCorrectionFactor", TempRiseCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<WorkflowInfo> ReadFromDB(string ID = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (ID != "")
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
                    RatedPower = !reader.IsDBNull("RatedPower") ? reader.GetInt32("RatedPower") : 0,
                    RatedPower1 = !reader.IsDBNull("RatedPower1") ? reader.GetInt32("RatedPower1") : 0,
                    RatedPower2 = !reader.IsDBNull("RatedPower2") ? reader.GetInt32("RatedPower2") : 0,
                    RatedVoltageHv = !reader.IsDBNull("RatedVoltageHv") ? reader.GetFloat("RatedVoltageHv") : 0,
                    RatedVoltageLv = !reader.IsDBNull("RatedVoltageLv") ? reader.GetFloat("RatedVoltageLv") : 0,
                    RatedVoltageYv = !reader.IsDBNull("RatedVoltageYv") ? reader.GetFloat("RatedVoltageYv") : 0,
                    RatedCurrentHv = !reader.IsDBNull("RatedCurrentHv") ? reader.GetFloat("RatedCurrentHv") : 0,
                    RatedCurrentLv = !reader.IsDBNull("RatedCurrentLv") ? reader.GetFloat("RatedCurrentLv") : 0,
                    RatedCurrentYv = !reader.IsDBNull("RatedCurrentYv") ? reader.GetFloat("RatedCurrentYv") : 0,
                    No = !reader.IsDBNull("No") ? reader.GetString("No") : "",
                    CAL = !reader.IsDBNull("CAL") ? reader.GetString("CAL") : "",
                    Type = !reader.IsDBNull("Type") ? reader.GetString("Type") : "",
                    Phase = !reader.IsDBNull("Phase") ? reader.GetInt32("Phase") : 0,
                    CONNSymbol = !reader.IsDBNull("CONNSymbol") ? reader.GetString("CONNSymbol") : "",
                    TappingVoltages = !reader.IsDBNull("TappingVoltages") ? reader.GetString("TappingVoltages") : "",
                    RatedVoltageInterval = !reader.IsDBNull("RatedVoltageInterval") ? reader.GetString("RatedVoltageInterval") : "",
                    TempRiseTestingVoltage = !reader.IsDBNull("TempRiseTestingVoltage") ? reader.GetFloat("TempRiseTestingVoltage") : null,
                    TempRiseTestingCurrent = !reader.IsDBNull("TempRiseTestingCurrent") ? reader.GetFloat("TempRiseTestingCurrent") : null,
                    TempRiseCorrectionFactor = !reader.IsDBNull("TempRiseCorrectionFactor") ? reader.GetString("TempRiseCorrectionFactor") : "",
                    TempRiseRelativeTo = !reader.IsDBNull("TempRiseRelativeTo") ? reader.GetString("TempRiseRelativeTo") : "",
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
            dst.TempRiseTestingVoltage = src.TempRiseTestingVoltage;
            dst.TempRiseTestingCurrent = src.TempRiseTestingCurrent;
            dst.TempRiseCorrectionFactor = src.TempRiseCorrectionFactor;
            dst.TempRiseRelativeTo = src.TempRiseRelativeTo;
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
