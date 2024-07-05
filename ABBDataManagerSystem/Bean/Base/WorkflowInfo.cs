using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class WorkflowInfo
    {
        public static string TABLE_NAME = "workflow";

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

        public string No = String.Empty;
        public string CAL = String.Empty;
        public string Type = String.Empty;
        public int Phase;
        public string CONNSymbol = String.Empty;
        public string TappingVoltages = String.Empty;
        public string RatedVoltageInterval = String.Empty;
        public string PartialDischarge = string.Empty;

        public float? Frequency;
        public string CoolingType = string.Empty;
        public string Location = string.Empty;
        public string InsulationLevel = string.Empty;
        public string InsulationClass = string.Empty;
        public float? CoreTempRise;
        public float? HWTempRise;
        public float? LWTempRise;
        public string SoundLevel = string.Empty;
        public string TecnicalStandard = string.Empty;
        public float? ResistanceAO;
        public float? ResistanceBO;
        public float? ResistanceCO;
        public float? ResistanceAO2;
        public float? ResistanceBO2;
        public float? ResistanceCO2;
        public string Operator = string.Empty;
        public string ProtectionDegree = string.Empty;
        public string FirstOrderNo = string.Empty;
        public float? ResistanceHW;
        public float? ResistanceLW;
        public float? ResistanceYW;
        public string LoadLosses75S = string.Empty;
        public string LoadLosses75D = string.Empty;
        public string LoadLosses145S = string.Empty;
        public string LoadLosses145D = string.Empty;
        public string NoLoadLossesS = string.Empty;
        public string NoLoadLossesD = string.Empty;
        public string TotalLosses75S = string.Empty;
        public string TotalLosses75D = string.Empty;
        public string TotalLosses145S = string.Empty;
        public string TotalLosses145D = string.Empty;
        public string ImpedanceS = string.Empty;
        public string ImpedanceD = string.Empty;
        public string ImpedanceYS = string.Empty;
        public string NoLoadCurrentS = string.Empty;
        public string NoLoadCurrentD = string.Empty;
        public string Remark = string.Empty;
        public string RefTemperature = string.Empty;
        public string FatalMark = string.Empty;
        public string HVMaterial = string.Empty;
        public string LVMaterial = string.Empty;
        public bool SoundPowerLevel = false;
        public string Coolant = string.Empty;
        public string FanMode = string.Empty;
        public float? Flow;
        public float? K2;
        public float? BackpackPower;
        public string TempRiseReference = string.Empty;
        public string ExternalCirculation = string.Empty;
        public string FirstTempRise = string.Empty;
        public string BackpackPerformance = string.Empty;

        public float? TempRiseTestingVoltage = null;
        public float? TempRiseTestingCurrent = null;
        public float? TempRiseHVCorrectionFactor = null;
        public float? TempRiseLVCorrectionFactor = null;
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
            {"PartialDischarge", "局放标准"},

            {"Frequency", "频率"},
            {"CoolingType", "冷却方式"},
            {"Location", "使用条件"},
            {"InsulationLevel", "绝缘水平"},
            {"InsulationClass", "绝缘等级"},
            {"CoreTempRise", "铁芯温升(K)"},
            {"HWTempRise", "高压线圈温升(K)"},
            {"LWTempRise", "低压线圈温升(K)"},
            {"SoundLevel", "变压器噪声(dB)"},
            {"TecnicalStandard", "参考标准"},
            {"ResistanceAO", "AO结构电阻"},
            {"ResistanceBO", "BO结构电阻"},
            {"ResistanceCO", "CO结构电阻"},
            {"ResistanceAO2" , "AO结构电阻2"},
            {"ResistanceBO2" , "BO结构电阻2"},
            {"ResistanceCO2" , "CO结构电阻2"},
            {"Operator", "操作员"},
            {"ProtectionDegree", "防护等级"},
            {"FirstOrderNo", "图号的首次工作令"},
            {"ResistanceHW", "75°时高压线圈电阻" },
            {"ResistanceLW", "75°时低压线圈电阻" },
            {"ResistanceYW", "75°时低压线圈电阻" },
            {"LoadLosses75S" , "负载损耗75°标准值" },
            {"LoadLosses75D" , "负载损耗75°设计值" },
            {"LoadLosses145S", "负载损耗145°标准值"},
            {"LoadLosses145D", "负载损耗145°设计值"},
            {"NoLoadLossesS" , "空载损耗标准值"},
            {"NoLoadLossesD" , "空载损耗设计值"},
            {"TotalLosses75S", "总损耗75°标准值"},
            {"TotalLosses75D", "总损耗75°设计值"},
            {"TotalLosses145S", "总损耗145°标准值" },
            {"TotalLosses145D", "总损耗145°设计值" },
            {"ImpedanceS", "阻抗标准值"},
            {"ImpedanceD", "阻抗设计值"},
            {"ImpedanceYS", "阻抗标准值1" },
            {"NoLoadCurrentS", "空载电流标准值(%)" },
            {"NoLoadCurrentD", "空载电流设计值(%)" },
            {"RefTemperature", "参考温度"},
            {"FatalMark" , "错误标记"},
            {"HVMaterial", "高压材质"},
            {"LVMaterial", "低压材质"},
            {"SoundPowerLevel", "是否Power类型的SoundLevel"},
            {"Coolant", "水冷冷却剂"},
            {"FanMode", "水冷风机模式"},
            {"Flow", "水冷流量，m3/h"},
            {"K2", "水冷k2"},
            {"BackpackPower" , "水冷背包功率"},
            {"TempRiseReference" , "水冷温升参考"},
            {"ExternalCirculation", "水冷外循环"},
            {"FirstTempRise" , "水冷首台温升"},
            {"BackpackPerformance", "水冷背包性能"},
            {"Remark", "备注" },
            //{"TempRiseTestingVoltage", "温升试验电压" },
            //{"TempRiseTestingCurrent", "温升试验电流" },
            //{"TempRiseHVCorrectionFactor", "高压温升校正系数" },
            //{"TempRiseLVCorrectionFactor", "低压温升校正系数" },
            //{"TempRiseRelativeTo", "温升相对于" },
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
                    "TappingVoltages, RatedVoltageInterval, TempRiseTestingVoltage, TempRiseTestingCurrent, TempRiseHVCorrectionFactor, TempRiseLVCorrectionFactor, TempRiseRelativeTo, ) " +
                    "VALUES(@ID, @WorkflowType, @RatedPower, @RatedPower1, @RatedPower2, @RatedVoltageHv, @RatedVoltageLv, @RatedVoltageYv, " +
                    "@RatedCurrentHv, @RatedCurrentLv, @RatedCurrentYv, @No, @CAL, @Type, @Phase, @CONNSymbol, @TappingVoltages, @RatedVoltageInterval, " +
                    "@TempRiseTestingVoltage, @TempRiseTestingCurrent, @TempRiseHVCorrectionFactor, @TempRiseLVCorrectionFactor, @TempRiseRelativeTo)", connection))
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
                    command.Parameters.AddWithValue("@TempRiseHVCorrectionFactor", TempRiseHVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseLVCorrectionFactor", TempRiseLVCorrectionFactor);
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
                $"TappingVoltages = @TappingVoltages, RatedVoltageInterval = @RatedVoltageInterval, TempRiseTestingVoltage = @TempRiseTestingVoltage, TempRiseTestingCurrent = @TempRiseTestingCurrent, TempRiseHVCorrectionFactor = @TempRiseHVCorrectionFactor, TempRiseLVCorrectionFactor = @TempRiseLVCorrectionFactor, TempRiseRelativeTo = @TempRiseRelativeTo, " +
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
                    command.Parameters.AddWithValue("@TempRiseHVCorrectionFactor", TempRiseHVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseLVCorrectionFactor", TempRiseLVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseRelativeTo", TempRiseRelativeTo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateTempRiseFields()
        {
            string updateSql = $"UPDATE {TABLE_NAME} SET TempRiseTestingVoltage = @TempRiseTestingVoltage, TempRiseTestingCurrent = @TempRiseTestingCurrent, " +
                "TempRiseHVCorrectionFactor = @TempRiseHVCorrectionFactor, TempRiseLVCorrectionFactor = @TempRiseLVCorrectionFactor, TempRiseRelativeTo = @TempRiseRelativeTo " +
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
                    command.Parameters.AddWithValue("@TempRiseHVCorrectionFactor", TempRiseHVCorrectionFactor);
                    command.Parameters.AddWithValue("@TempRiseLVCorrectionFactor", TempRiseLVCorrectionFactor);
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
                    PartialDischarge = !reader.IsDBNull("PartialDischarge") ? reader.GetString("PartialDischarge") : "",
                    TempRiseTestingVoltage = !reader.IsDBNull("TempRiseTestingVoltage") ? reader.GetFloat("TempRiseTestingVoltage") : null,
                    TempRiseTestingCurrent = !reader.IsDBNull("TempRiseTestingCurrent") ? reader.GetFloat("TempRiseTestingCurrent") : null,
                    TempRiseHVCorrectionFactor = !reader.IsDBNull("TempRiseHVCorrectionFactor") ? reader.GetFloat("TempRiseHVCorrectionFactor") : null,
                    TempRiseLVCorrectionFactor = !reader.IsDBNull("TempRiseLVCorrectionFactor") ? reader.GetFloat("TempRiseLVCorrectionFactor") : null,
                    TempRiseRelativeTo = !reader.IsDBNull("TempRiseRelativeTo") ? reader.GetString("TempRiseRelativeTo") : "",

                    Frequency = !reader.IsDBNull("Frequency") ? reader.GetFloat("Frequency") : null,
                    CoolingType = !reader.IsDBNull("CoolingType") ? reader.GetString("CoolingType") : "",
                    Location = !reader.IsDBNull("Location") ? reader.GetString("Location") : "",
                    InsulationLevel = !reader.IsDBNull("InsulationLevel") ? reader.GetString("InsulationLevel") : "",
                    InsulationClass = !reader.IsDBNull("InsulationClass") ? reader.GetString("InsulationClass") : "",
                    CoreTempRise = !reader.IsDBNull("CoreTempRise") ? reader.GetFloat("CoreTempRise") : null,
                    HWTempRise = !reader.IsDBNull("HWTempRise") ? reader.GetFloat("HWTempRise") : null,
                    LWTempRise = !reader.IsDBNull("LWTempRise") ? reader.GetFloat("LWTempRise") : null,
                    SoundLevel = !reader.IsDBNull("SoundLevel") ? reader.GetString("SoundLevel") : "",
                    TecnicalStandard = !reader.IsDBNull("TecnicalStandard") ? reader.GetString("TecnicalStandard") : "",
                    ResistanceAO = !reader.IsDBNull("ResistanceAO") ? reader.GetFloat("ResistanceAO") : null,
                    ResistanceBO = !reader.IsDBNull("ResistanceBO") ? reader.GetFloat("ResistanceBO") : null,
                    ResistanceCO = !reader.IsDBNull("ResistanceCO") ? reader.GetFloat("ResistanceCO") : null,
                    ResistanceAO2 = !reader.IsDBNull("ResistanceAO2") ? reader.GetFloat("ResistanceAO2") : null,
                    ResistanceBO2 = !reader.IsDBNull("ResistanceBO2") ? reader.GetFloat("ResistanceBO2") : null,
                    ResistanceCO2 = !reader.IsDBNull("ResistanceCO2") ? reader.GetFloat("ResistanceCO2") : null,
                    Operator = !reader.IsDBNull("Operator") ? reader.GetString("Operator") : "",
                    ProtectionDegree = !reader.IsDBNull("ProtectionDegree") ? reader.GetString("ProtectionDegree") : "",
                    FirstOrderNo = !reader.IsDBNull("FirstOrderNo") ? reader.GetString("FirstOrderNo") : "",
                    ResistanceHW = !reader.IsDBNull("ResistanceHW") ? reader.GetFloat("ResistanceHW") : null,
                    ResistanceLW = !reader.IsDBNull("ResistanceLW") ? reader.GetFloat("ResistanceLW") : null,
                    ResistanceYW = !reader.IsDBNull("ResistanceYW") ? reader.GetFloat("ResistanceYW") : null,
                    LoadLosses75S = !reader.IsDBNull("LoadLosses75S") ? reader.GetString("LoadLosses75S") : "",
                    LoadLosses75D = !reader.IsDBNull("LoadLosses75D") ? reader.GetString("LoadLosses75D") : "",
                    LoadLosses145S = !reader.IsDBNull("LoadLosses145S") ? reader.GetString("LoadLosses145S") : "",
                    LoadLosses145D = !reader.IsDBNull("LoadLosses145D") ? reader.GetString("LoadLosses145D") : "",
                    NoLoadLossesS = !reader.IsDBNull("NoLoadLossesS") ? reader.GetString("NoLoadLossesS") : "",
                    NoLoadLossesD = !reader.IsDBNull("NoLoadLossesD") ? reader.GetString("NoLoadLossesD") : "",
                    TotalLosses75S = !reader.IsDBNull("TotalLosses75S") ? reader.GetString("TotalLosses75S") : "",
                    TotalLosses75D = !reader.IsDBNull("TotalLosses75D") ? reader.GetString("TotalLosses75D") : "",
                    TotalLosses145S = !reader.IsDBNull("TotalLosses145S") ? reader.GetString("TotalLosses145S") : "",
                    TotalLosses145D = !reader.IsDBNull("TotalLosses145D") ? reader.GetString("TotalLosses145D") : "",
                    ImpedanceS = !reader.IsDBNull("ImpedanceS") ? reader.GetString("ImpedanceS") : "",
                    ImpedanceD = !reader.IsDBNull("ImpedanceD") ? reader.GetString("ImpedanceD") : "",
                    ImpedanceYS = !reader.IsDBNull("ImpedanceYS") ? reader.GetString("ImpedanceYS") : "",
                    NoLoadCurrentS = !reader.IsDBNull("NoLoadCurrentS") ? reader.GetString("NoLoadCurrentS") : "",
                    NoLoadCurrentD = !reader.IsDBNull("NoLoadCurrentD") ? reader.GetString("NoLoadCurrentD") : "",
                    Remark = !reader.IsDBNull("Remark") ? reader.GetString("Remark") : "",
                    RefTemperature = !reader.IsDBNull("RefTemperature") ? reader.GetString("RefTemperature") : "",
                    FatalMark = !reader.IsDBNull("FatalMark") ? reader.GetString("FatalMark") : "",
                    HVMaterial = !reader.IsDBNull("HVMaterial") ? reader.GetString("HVMaterial") : "",
                    LVMaterial = !reader.IsDBNull("LVMaterial") ? reader.GetString("LVMaterial") : "",
                    SoundPowerLevel = !reader.IsDBNull("SoundPowerLevel") ? reader.GetBoolean("SoundPowerLevel") : false,
                    Coolant = !reader.IsDBNull("Coolant") ? reader.GetString("Coolant") : "",
                    FanMode = !reader.IsDBNull("FanMode") ? reader.GetString("FanMode") : "",
                    Flow = !reader.IsDBNull("Flow") ? reader.GetFloat("Flow") : null,
                    K2 = !reader.IsDBNull("K2") ? reader.GetFloat("K2") : null,
                    BackpackPower = !reader.IsDBNull("BackpackPower") ? reader.GetFloat("BackpackPower") : null,
                    TempRiseReference = !reader.IsDBNull("TempRiseReference") ? reader.GetString("TempRiseReference") : "",
                    ExternalCirculation = !reader.IsDBNull("ExternalCirculation") ? reader.GetString("ExternalCirculation") : "",
                    FirstTempRise = !reader.IsDBNull("FirstTempRise") ? reader.GetString("FirstTempRise") : "",
                    BackpackPerformance = !reader.IsDBNull("BackpackPerformance") ? reader.GetString("BackpackPerformance") : "",

                };
            });
            if (records == null)
            {
                return new List<WorkflowInfo>();
            }
            Log.Info("WorkflowInfo COUNT = " + records.Count);
            return records;
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page, string? filterId = null, bool? blurSearch = false)
        {
            if (page < 0)
            {
                return;
            }
            string where = "";
            if (filterId != null && filterId.Length > 0)
            {
                if (blurSearch == true)
                {
                    where = $" WHERE ID LIKE '%{filterId}%'";
                }
                else
                {
                    where = $" WHERE ID = '{filterId}'";
                }
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

        public static int GetTotalCount(string? filterId = null, bool? blurSearch = false)
        {
            string where = "";
            if (filterId != null && filterId.Length > 0)
            {
                if (blurSearch == true)
                {
                    where = $" WHERE ID LIKE '%{filterId}%'";
                }
                else
                {
                    where = $" WHERE ID = '{filterId}'";
                }
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
            dst.TempRiseHVCorrectionFactor = src.TempRiseHVCorrectionFactor;
            dst.TempRiseLVCorrectionFactor = src.TempRiseLVCorrectionFactor;
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
