using ElectricalDataManagerSystem.Database;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class FigureNoInfoFieldDef
    {
        public string FieldName;
        public string Description;
        public string FieldType;
        public string InGSBYQ; // 干式变压器
        public string InGSCBL; // 干式串联
        public string InGSXHXQ; // 干式消弧
        public string InWDY53;  // 无低压-5分-3分
        public string InWDY12;  // 无低压-1分-2分
        public string InYDY;    // 有低压-接地变
        public bool IsNeed;    // 是否必须
    }

    public class FigureNumberInfo
    {

        public static string TABLE_NAME = "FigureNumberInfo";

        public static Dictionary<string, FigureNoInfoFieldDef> Defs = new Dictionary<string, FigureNoInfoFieldDef>
        {
            {"FigureNumber", new FigureNoInfoFieldDef{FieldName = "FigureNumber", Description = "代号尾数", FieldType = "string", InGSBYQ = "AK4", InGSCBL="AM4", InGSXHXQ = "AM4", InWDY53 = "AM4", InWDY12 = "AM4", InYDY = "AK4", IsNeed = false}},
            {"ProductType", new FigureNoInfoFieldDef{FieldName = "ProductType", Description = "产品类型", FieldType = "string", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = false}},
            //{"UserName", new FigureNoInfoFieldDef{FieldName = "UserName", Description = "用户名称", FieldType = "string", InGSBYQ = "L3", InGSCBL="L3", InGSXHXQ = "L3", InWDY53 = "L3", InWDY12 = "L3", InYDY = "L3", IsNeed = false}},
            {"Model", new FigureNoInfoFieldDef{FieldName = "Model", Description = "产品型号", FieldType = "string", InGSBYQ = "L4", InGSCBL="L4", InGSXHXQ = "L4", InWDY53 = "L4", InWDY12 = "L4", InYDY = "L4", IsNeed = true}},
            {"RatedCapacity", new FigureNoInfoFieldDef{FieldName = "RatedCapacity", Description = "额定容量（kVA）", FieldType = "float", InGSBYQ = "L5", InGSCBL="L5", InGSXHXQ = "L5", InWDY53 = "L5", InWDY12 = "L5", InYDY = "L5", IsNeed = true}},
            {"RatedHighVoltage", new FigureNoInfoFieldDef{FieldName = "RatedHighVoltage", Description = "高压额定电压（kV）", FieldType = "float", InGSBYQ = "L6", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "L6", InWDY12 = "L6", InYDY = "L6", IsNeed = true}},
            {"RatedHighCurrent", new FigureNoInfoFieldDef{FieldName = "RatedHighCurrent", Description = "高压额定电流（A）", FieldType = "float", InGSBYQ = "L7", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "L7", InWDY12 = "L7", InYDY = "L7", IsNeed = true}},
            {"RatedLowVoltage", new FigureNoInfoFieldDef{FieldName = "RatedLowVoltage", Description = "低压额定电压（kV）", FieldType = "float", InGSBYQ = "L8", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "L8", IsNeed = true}},
            {"RatedLowCurrent", new FigureNoInfoFieldDef{FieldName = "RatedLowCurrent", Description = "低压额定电流（A）", FieldType = "float", InGSBYQ = "L9", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "L9", IsNeed = true}},
            //{"Sequence", new FigureNoInfoFieldDef{FieldName = "Sequence", Description = "出厂序号", FieldType = "string", InGSBYQ = "AK3", InGSCBL="AM3", InGSXHXQ = "AM3", InWDY53 = "AM3", InWDY12 = "AM3", InYDY = "AK3", IsNeed = false}},
            {"NickName", new FigureNoInfoFieldDef{FieldName = "NickName", Description = "产品代号", FieldType = "string", InGSBYQ = "AK5", InGSCBL="AM5", InGSXHXQ = "AM5", InWDY53 = "AM5", InWDY12 = "AM5", InYDY = "AK5", IsNeed = false}},
            {"ConnectionGroupLabel", new FigureNoInfoFieldDef{FieldName = "ConnectionGroupLabel", Description = "联接组标号", FieldType = "string", InGSBYQ = "AO6", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "AM6", InWDY12 = "AM6", InYDY = "AO6", IsNeed = true}},
            {"HeatResistanceRating", new FigureNoInfoFieldDef{FieldName = "HeatResistanceRating", Description = "耐热等级", FieldType = "string", InGSBYQ = "AJ7", InGSCBL="AM6", InGSXHXQ = "AM6", InWDY53 = "AM7", InWDY12 = "AM7", InYDY = "AJ7", IsNeed = true}},
            {"CoolingType", new FigureNoInfoFieldDef{FieldName = "CoolingType", Description = "冷却方式", FieldType = "string", InGSBYQ = "AJ8", InGSCBL="AM7", InGSXHXQ = "AM7", InWDY53 = "AM8", InWDY12 = "AM8", InYDY = "AJ8", IsNeed = true}},
            {"TestingDate", new FigureNoInfoFieldDef{FieldName = "TestingDate", Description = "试验日期", FieldType = "datetime", InGSBYQ = "AJ9", InGSCBL="AM9", InGSXHXQ = "AM9", InWDY53 = "AM9", InWDY12 = "AM9", InYDY = "AJ11", IsNeed = false}},
            {"SystemVoltage", new FigureNoInfoFieldDef{FieldName = "SystemVoltage", Description = "系统电压（kV）", FieldType = "float", InGSBYQ = "False", InGSCBL="L6", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"EndVoltage", new FigureNoInfoFieldDef{FieldName = "EndVoltage", Description = "端电压（V）", FieldType = "float", InGSBYQ = "False", InGSCBL="L7", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"RatedCurrent", new FigureNoInfoFieldDef{FieldName = "RatedCurrent", Description = "额定电流（A)", FieldType = "float", InGSBYQ = "False", InGSCBL="L8", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ReactanceRate", new FigureNoInfoFieldDef{FieldName = "ReactanceRate", Description = "电抗率（%）", FieldType = "float", InGSBYQ = "False", InGSCBL="L9", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ParallelCapacitorCapacity", new FigureNoInfoFieldDef{FieldName = "ParallelCapacitorCapacity", Description = "并联电容器容量（kvar）", FieldType = "float", InGSBYQ = "False", InGSCBL="AM8", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = false}},
            {"WindingMaterial", new FigureNoInfoFieldDef{FieldName = "WindingMaterial", Description = "绕组材质", FieldType = "string", InGSBYQ = "False", InGSCBL="AM10", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = false}},
            {"RatedVoltage", new FigureNoInfoFieldDef{FieldName = "RatedVoltage", Description = "额定电压（kV）", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "L6", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"MinCurrent", new FigureNoInfoFieldDef{FieldName = "MinCurrent", Description = "最小电流（A）", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "L7", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"MaxCurrent", new FigureNoInfoFieldDef{FieldName = "MaxCurrent", Description = "最大电流（A）", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "L8", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"GearCount", new FigureNoInfoFieldDef{FieldName = "GearCount", Description = "档位数", FieldType = "int", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "L9", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"CurrentRegulationMethod", new FigureNoInfoFieldDef{FieldName = "CurrentRegulationMethod", Description = "电流调节方式", FieldType = "string", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "AM8", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"HighVoltageWindingTapCount", new FigureNoInfoFieldDef{FieldName = "HighVoltageWindingTapCount", Description = "高压绕组分接数", FieldType = "int", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "L8", InWDY12 = "L8", InYDY = "False", IsNeed = true}},
            {"RatedNeutralCurrent", new FigureNoInfoFieldDef{FieldName = "RatedNeutralCurrent", Description = "额定中性点电流（A/h）", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "L9", InWDY12 = "L9", InYDY = "AJ9", IsNeed = true}},
            {"ShortTimeNeutralCurrent", new FigureNoInfoFieldDef{FieldName = "ShortTimeNeutralCurrent", Description = "短时中性点电流（A/s）", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "L10", InWDY12 = "L10", InYDY = "AJ10", IsNeed = true}},
            {"HVCurrentAtRatedCapacityForLV", new FigureNoInfoFieldDef{FieldName = "HVCurrentAtRatedCapacityForLV", Description = "低压为额定容量时的高压电流(A)", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "L10", IsNeed = true}},

            {"HighVoltageResistance", new FigureNoInfoFieldDef{FieldName = "HighVoltageResistance", Description = "高压电阻（Ω）", FieldType = "float", InGSBYQ = "J16", InGSCBL="J17", InGSXHXQ = "J16", InWDY53 = "J17", InWDY12 = "J17", InYDY = "J18", IsNeed = true}},
            {"LowVoltageResistanceMilli", new FigureNoInfoFieldDef{FieldName = "LowVoltageResistanceMilli", Description = "低压电阻（mΩ）", FieldType = "float", InGSBYQ = "J17", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "J19", IsNeed = true}},
            {"NoLoadLossDesignValue", new FigureNoInfoFieldDef{FieldName = "NoLoadLossDesignValue", Description = "空载损耗 (设计)W", FieldType = "float", InGSBYQ = "J19", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "J19", InWDY12 = "J19", InYDY = "J21", IsNeed = true}},
            {"NoLoadLossStandardValue", new FigureNoInfoFieldDef{FieldName = "NoLoadLossStandardValue", Description = "空载损耗 (标准)W", FieldType = "float", InGSBYQ = "R19", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "R19", InWDY12 = "R19", InYDY = "R21", IsNeed = true}},
            {"NoLoadCurrentDesignValue", new FigureNoInfoFieldDef{FieldName = "NoLoadCurrentDesignValue", Description = "空载电流 (设计)%", FieldType = "float", InGSBYQ = "J20", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "J20", InWDY12 = "J20", InYDY = "J22", IsNeed = true}},
            {"NoLoadCurrentStandardValue", new FigureNoInfoFieldDef{FieldName = "NoLoadCurrentStandardValue", Description = "空载电流 (标准)%", FieldType = "float", InGSBYQ = "R20", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "R20", InWDY12 = "R20", InYDY = "R22", IsNeed = true}},
            {"LoadLossDesignValue", new FigureNoInfoFieldDef{FieldName = "LoadLossDesignValue", Description = "负载损耗 (设计)W", FieldType = "float", InGSBYQ = "J21", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "J23", IsNeed = true}},
            {"LoadLossStandardValue", new FigureNoInfoFieldDef{FieldName = "LoadLossStandardValue", Description = "负载损耗 (标准)W", FieldType = "float", InGSBYQ = "R21", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "R23", IsNeed = true}},
            {"ShortCircuitImpedanceDesignValue", new FigureNoInfoFieldDef{FieldName = "ShortCircuitImpedanceDesignValue", Description = "短路阻抗 (设计)%", FieldType = "float", InGSBYQ = "J22", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "J24", IsNeed = true}},
            {"ShortCircuitImpedanceStandardValue", new FigureNoInfoFieldDef{FieldName = "ShortCircuitImpedanceStandardValue", Description = "短路阻抗 (标准)%", FieldType = "float", InGSBYQ = "R22", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "R24", IsNeed = true}},
            {"ElectricReactanceUnbalance10IDesignValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceUnbalance10IDesignValue", Description = "1.0I电抗不平衡 (设计)", FieldType = "float", InGSBYQ = "False", InGSCBL="J20", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ElectricReactanceUnbalance10IStandardValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceUnbalance10IStandardValue", Description = "1.0I电抗不平衡 (标准)", FieldType = "float", InGSBYQ = "False", InGSCBL="R20", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ElectricReactanceDeviation18IDesignValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceDeviation18IDesignValue", Description = "1.8I电抗偏差 (设计)", FieldType = "float", InGSBYQ = "False", InGSCBL="J21", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ElectricReactanceDeviation18IStandardValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceDeviation18IStandardValue", Description = "1.8I电抗偏差 (标准)", FieldType = "float", InGSBYQ = "False", InGSCBL="R21", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ElectricReactanceUnbalance18IDesignValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceUnbalance18IDesignValue", Description = "1.8I电抗不平衡 (设计)", FieldType = "float", InGSBYQ = "False", InGSCBL="J22", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ElectricReactanceUnbalance18IStandardValue", new FigureNoInfoFieldDef{FieldName = "ElectricReactanceUnbalance18IStandardValue", Description = "1.8I电抗不平衡 (标准)", FieldType = "float", InGSBYQ = "False", InGSCBL="R22", InGSXHXQ = "False", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"LossMeasurementDesignValue", new FigureNoInfoFieldDef{FieldName = "LossMeasurementDesignValue", Description = "损耗测量 (设计)W", FieldType = "float", InGSBYQ = "False", InGSCBL="J23", InGSXHXQ = "J19", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"LossMeasurementStandardValue", new FigureNoInfoFieldDef{FieldName = "LossMeasurementStandardValue", Description = "损耗测量 (标准)W", FieldType = "float", InGSBYQ = "False", InGSCBL="R23", InGSXHXQ = "R19", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            //{"RatedCurrent", new FigureNoInfoFieldDef{FieldName = "RatedCurrent", Description = "额定电流A", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "R17", InWDY53 = "False", InWDY12 = "False", InYDY = "False", IsNeed = true}},
            {"ZeroSequenceImpedanceDesignValue", new FigureNoInfoFieldDef{FieldName = "ZeroSequenceImpedanceDesignValue", Description = "零序阻抗 (设计)Ω", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "J21", InWDY12 = "J21", InYDY = "J25", IsNeed = true}},
            {"ZeroSequenceImpedanceStandardValue", new FigureNoInfoFieldDef{FieldName = "ZeroSequenceImpedanceStandardValue", Description = "零序阻抗 (标准)Ω", FieldType = "float", InGSBYQ = "False", InGSCBL="False", InGSXHXQ = "False", InWDY53 = "R21", InWDY12 = "R21", InYDY = "R25", IsNeed = true}},
            {"ReferenceTemperature", new FigureNoInfoFieldDef{FieldName = "ReferenceTemperature", Description = "参考温度（℃）", FieldType = "float", InGSBYQ = "True", InGSCBL="True", InGSXHXQ = "True", InWDY53 = "True", InWDY12 = "True", InYDY = "True", IsNeed = true}},
        };

        public static string[] ProductTypes = new string[]{
            "干式变压器",
            "干式串联电抗器",
            "干式消弧线圈",
            "无低压 高压五分接和三分接兼容母本",
            "无低压 高压一分接和二分接兼容母本",
            "有低压 高压五分接母本"
        };

        public static string? GetMatchFieldByProductType(string? productType)
        {
            if (productType == null) return null;
            switch (productType)
            {
                case "干式变压器":
                    return "InGSBYQ";
                case "干式串联电抗器":
                    return "InGSCBL";
                case "干式消弧线圈":
                    return "InGSXHXQ";
                case "无低压 高压五分接和三分接兼容母本":
                    return "InWDY53";
                case "无低压 高压一分接和二分接兼容母本":
                    return "InWDY12";
                case "有低压 高压五分接母本":
                    return "InYDY";
            }
            return null; ;
        }

        public string FigureNumber = string.Empty;
        public string UserName = string.Empty;
        public string Model = string.Empty;
        public float? RatedCapacity;
        public float? RatedHighVoltage;
        public float? RatedHighCurrent;
        public float? RatedLowVoltage;
        public float? RatedLowCurrent;
        public string Sequence = string.Empty;
        public string NickName = string.Empty;
        public string ConnectionGroupLabel = string.Empty;
        public string HeatResistanceRating = string.Empty;
        public string CoolingType = string.Empty;
        public DateTime? TestingDate;
        public float? SystemVoltage;
        public float? EndVoltage;
        public float? RatedCurrent;
        public float? ReactanceRate;
        public float? ParallelCapacitorCapacity;
        public string WindingMaterial = string.Empty;
        public float? RatedVoltage;
        public float? MinCurrent;
        public float? MaxCurrent;
        public int? GearCount;
        public string CurrentRegulationMethod = string.Empty;
        public int? HighVoltageWindingTapCount;
        public float? RatedNeutralCurrent;
        public float? ShortTimeNeutralCurrent;
        public float? HVCurrentAtRatedCapacityForLV;
        public float? HighVoltageResistance;
        public float? LowVoltageResistanceMilli;
        public float? NoLoadLossDesignValue;
        public float? NoLoadLossStandardValue;
        public float? NoLoadCurrentDesignValue;
        public float? NoLoadCurrentStandardValue;
        public float? LoadLossDesignValue;
        public float? LoadLossStandardValue;
        public float? ShortCircuitImpedanceDesignValue;
        public float? ShortCircuitImpedanceStandardValue;
        public float? ElectricReactanceUnbalance10IDesignValue;
        public float? ElectricReactanceUnbalance10IStandardValue;
        public float? ElectricReactanceDeviation18IDesignValue;
        public float? ElectricReactanceDeviation18IStandardValue;
        public float? ElectricReactanceUnbalance18IDesignValue;
        public float? ElectricReactanceUnbalance18IStandardValue;
        public float? LossMeasurementDesignValue;
        public float? LossMeasurementStandardValue;
        public float? ZeroSequenceImpedanceDesignValue;
        public float? ZeroSequenceImpedanceStandardValue;
        public string ProductType = string.Empty;
        public float? ReferenceTemperature;

        public static List<FigureNumberInfo>? GetFromDB(string? figureNumber = null)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (figureNumber != null)
            {
                queryDataSql += $" WHERE FigureNumber = '{figureNumber}'";
            }
            List<FigureNumberInfo>? records = DBConnector.QueryFromDB<FigureNumberInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new FigureNumberInfo
                {
                    FigureNumber = !reader.IsDBNull("FigureNumber") ? reader.GetString("FigureNumber") : "",
                    UserName = !reader.IsDBNull("UserName") ? reader.GetString("UserName") : "",
                    Model = !reader.IsDBNull("Model") ? reader.GetString("Model") : "",
                    RatedCapacity = !reader.IsDBNull("RatedCapacity") ? reader.GetFloat("RatedCapacity") : null,
                    RatedHighVoltage = !reader.IsDBNull("RatedHighVoltage") ? reader.GetFloat("RatedHighVoltage") : null,
                    RatedHighCurrent = !reader.IsDBNull("RatedHighCurrent") ? reader.GetFloat("RatedHighCurrent") : null,
                    RatedLowVoltage = !reader.IsDBNull("RatedLowVoltage") ? reader.GetFloat("RatedLowVoltage") : null,
                    RatedLowCurrent = !reader.IsDBNull("RatedLowCurrent") ? reader.GetFloat("RatedLowCurrent") : null,
                    Sequence = !reader.IsDBNull("Sequence") ? reader.GetString("Sequence") : "",
                    NickName = !reader.IsDBNull("NickName") ? reader.GetString("NickName") : "",
                    ConnectionGroupLabel = !reader.IsDBNull("ConnectionGroupLabel") ? reader.GetString("ConnectionGroupLabel") : "",
                    HeatResistanceRating = !reader.IsDBNull("HeatResistanceRating") ? reader.GetString("HeatResistanceRating") : "",
                    CoolingType = !reader.IsDBNull("CoolingType") ? reader.GetString("CoolingType") : "",
                    TestingDate = !reader.IsDBNull("TestingDate") ? reader.GetDateTime("TestingDate") : null,
                    SystemVoltage = !reader.IsDBNull("SystemVoltage") ? reader.GetFloat("SystemVoltage") : null,
                    EndVoltage = !reader.IsDBNull("EndVoltage") ? reader.GetFloat("EndVoltage") : null,
                    RatedCurrent = !reader.IsDBNull("RatedCurrent") ? reader.GetFloat("RatedCurrent") : null,
                    ReactanceRate = !reader.IsDBNull("ReactanceRate") ? reader.GetFloat("ReactanceRate") : null,
                    ParallelCapacitorCapacity = !reader.IsDBNull("ParallelCapacitorCapacity") ? reader.GetFloat("ParallelCapacitorCapacity") : null,
                    WindingMaterial = !reader.IsDBNull("WindingMaterial") ? reader.GetString("WindingMaterial") : "",
                    RatedVoltage = !reader.IsDBNull("RatedVoltage") ? reader.GetFloat("RatedVoltage") : null,
                    MinCurrent = !reader.IsDBNull("MinCurrent") ? reader.GetFloat("MinCurrent") : null,
                    MaxCurrent = !reader.IsDBNull("MaxCurrent") ? reader.GetFloat("MaxCurrent") : null,
                    GearCount = !reader.IsDBNull("GearCount") ? reader.GetInt32("GearCount") : null,
                    CurrentRegulationMethod = !reader.IsDBNull("CurrentRegulationMethod") ? reader.GetString("CurrentRegulationMethod") : "",
                    HighVoltageWindingTapCount = !reader.IsDBNull("HighVoltageWindingTapCount") ? reader.GetInt32("HighVoltageWindingTapCount") : null,
                    RatedNeutralCurrent = !reader.IsDBNull("RatedNeutralCurrent") ? reader.GetFloat("RatedNeutralCurrent") : null,
                    ShortTimeNeutralCurrent = !reader.IsDBNull("ShortTimeNeutralCurrent") ? reader.GetFloat("ShortTimeNeutralCurrent") : null,
                    HVCurrentAtRatedCapacityForLV = !reader.IsDBNull("HVCurrentAtRatedCapacityForLV") ? reader.GetFloat("HVCurrentAtRatedCapacityForLV") : null,
                    HighVoltageResistance = !reader.IsDBNull("HighVoltageResistance") ? reader.GetFloat("HighVoltageResistance") : null,
                    LowVoltageResistanceMilli = !reader.IsDBNull("LowVoltageResistanceMilli") ? reader.GetFloat("LowVoltageResistanceMilli") : null,
                    NoLoadLossDesignValue = !reader.IsDBNull("NoLoadLossDesignValue") ? reader.GetFloat("NoLoadLossDesignValue") : null,
                    NoLoadLossStandardValue = !reader.IsDBNull("NoLoadLossStandardValue") ? reader.GetFloat("NoLoadLossStandardValue") : null,
                    NoLoadCurrentDesignValue = !reader.IsDBNull("NoLoadCurrentDesignValue") ? reader.GetFloat("NoLoadCurrentDesignValue") : null,
                    NoLoadCurrentStandardValue = !reader.IsDBNull("NoLoadCurrentStandardValue") ? reader.GetFloat("NoLoadCurrentStandardValue") : null,
                    LoadLossDesignValue = !reader.IsDBNull("LoadLossDesignValue") ? reader.GetFloat("LoadLossDesignValue") : null,
                    LoadLossStandardValue = !reader.IsDBNull("LoadLossStandardValue") ? reader.GetFloat("LoadLossStandardValue") : null,
                    ShortCircuitImpedanceDesignValue = !reader.IsDBNull("ShortCircuitImpedanceDesignValue") ? reader.GetFloat("ShortCircuitImpedanceDesignValue") : null,
                    ShortCircuitImpedanceStandardValue = !reader.IsDBNull("ShortCircuitImpedanceStandardValue") ? reader.GetFloat("ShortCircuitImpedanceStandardValue") : null,
                    ElectricReactanceUnbalance10IDesignValue = !reader.IsDBNull("ElectricReactanceUnbalance10IDesignValue") ? reader.GetFloat("ElectricReactanceUnbalance10IDesignValue") : null,
                    ElectricReactanceUnbalance10IStandardValue = !reader.IsDBNull("ElectricReactanceUnbalance10IStandardValue") ? reader.GetFloat("ElectricReactanceUnbalance10IStandardValue") : null,
                    ElectricReactanceDeviation18IDesignValue = !reader.IsDBNull("ElectricReactanceDeviation18IDesignValue") ? reader.GetFloat("ElectricReactanceDeviation18IDesignValue") : null,
                    ElectricReactanceDeviation18IStandardValue = !reader.IsDBNull("ElectricReactanceDeviation18IStandardValue") ? reader.GetFloat("ElectricReactanceDeviation18IStandardValue") : null,
                    ElectricReactanceUnbalance18IDesignValue = !reader.IsDBNull("ElectricReactanceUnbalance18IDesignValue") ? reader.GetFloat("ElectricReactanceUnbalance18IDesignValue") : null,
                    ElectricReactanceUnbalance18IStandardValue = !reader.IsDBNull("ElectricReactanceUnbalance18IStandardValue") ? reader.GetFloat("ElectricReactanceUnbalance18IStandardValue") : null,
                    LossMeasurementDesignValue = !reader.IsDBNull("LossMeasurementDesignValue") ? reader.GetFloat("LossMeasurementDesignValue") : null,
                    LossMeasurementStandardValue = !reader.IsDBNull("LossMeasurementStandardValue") ? reader.GetFloat("LossMeasurementStandardValue") : null,
                    ZeroSequenceImpedanceDesignValue = !reader.IsDBNull("ZeroSequenceImpedanceDesignValue") ? reader.GetFloat("ZeroSequenceImpedanceDesignValue") : null,
                    ZeroSequenceImpedanceStandardValue = !reader.IsDBNull("ZeroSequenceImpedanceStandardValue") ? reader.GetFloat("ZeroSequenceImpedanceStandardValue") : null,
                    ProductType = !reader.IsDBNull("ProductType") ? reader.GetString("ProductType") : "",
                    ReferenceTemperature = !reader.IsDBNull("ReferenceTemperature") ? reader.GetFloat("ReferenceTemperature") : null,
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("FigureNumberInfo COUNT = " + records.Count);
            return records;
        }

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                //CreateSqliteTable(connection);

                SQLCommond sqlCommond;
                sqlCommond = connection.CreateCommand();
                sqlCommond.CommandText = $"INSERT INTO {TABLE_NAME} (FigureNumber, UserName, Model, RatedCapacity, RatedHighVoltage, RatedHighCurrent, RatedLowVoltage, RatedLowCurrent, Sequence, NickName, " +
                    $"ConnectionGroupLabel, HeatResistanceRating, CoolingType, TestingDate, SystemVoltage, EndVoltage, RatedCurrent, ReactanceRate, ParallelCapacitorCapacity, WindingMaterial, RatedVoltage, " +
                    $"MinCurrent, MaxCurrent, GearCount, CurrentRegulationMethod, HighVoltageWindingTapCount, RatedNeutralCurrent, ShortTimeNeutralCurrent, HVCurrentAtRatedCapacityForLV, HighVoltageResistance, " +
                    $"LowVoltageResistanceMilli, NoLoadLossDesignValue, NoLoadLossStandardValue, NoLoadCurrentDesignValue, NoLoadCurrentStandardValue, LoadLossDesignValue, LoadLossStandardValue, " +
                    $"ShortCircuitImpedanceDesignValue, ShortCircuitImpedanceStandardValue, ElectricReactanceUnbalance10IDesignValue, ElectricReactanceUnbalance10IStandardValue, " +
                    $"ElectricReactanceDeviation18IDesignValue, ElectricReactanceDeviation18IStandardValue, ElectricReactanceUnbalance18IDesignValue, " +
                    $"ElectricReactanceUnbalance18IStandardValue, LossMeasurementDesignValue, LossMeasurementStandardValue, ZeroSequenceImpedanceDesignValue, ZeroSequenceImpedanceStandardValue, ProductType, ReferenceTemperature) " +
                    $"VALUES(@FigureNumber, @UserName, @Model, @RatedCapacity, @RatedHighVoltage, @RatedHighCurrent, @RatedLowVoltage, @RatedLowCurrent, @Sequence, @NickName, @ConnectionGroupLabel, @HeatResistanceRating, " +
                    $"@CoolingType, @TestingDate, @SystemVoltage, @EndVoltage, @RatedCurrent, @ReactanceRate, @ParallelCapacitorCapacity, @WindingMaterial, @RatedVoltage, @MinCurrent, @MaxCurrent, @GearCount, @CurrentRegulationMethod, " +
                    $"@HighVoltageWindingTapCount, @RatedNeutralCurrent, @ShortTimeNeutralCurrent, @HVCurrentAtRatedCapacityForLV, @HighVoltageResistance, @LowVoltageResistanceMilli, @NoLoadLossDesignValue, @NoLoadLossStandardValue, " +
                    $"@NoLoadCurrentDesignValue, @NoLoadCurrentStandardValue, @LoadLossDesignValue, @LoadLossStandardValue, @ShortCircuitImpedanceDesignValue, @ShortCircuitImpedanceStandardValue, @ElectricReactanceUnbalance10IDesignValue, " +
                    $"@ElectricReactanceUnbalance10IStandardValue, @ElectricReactanceDeviation18IDesignValue, @ElectricReactanceDeviation18IStandardValue, @ElectricReactanceUnbalance18IDesignValue, @ElectricReactanceUnbalance18IStandardValue, " +
                    $"@LossMeasurementDesignValue, @LossMeasurementStandardValue, @ZeroSequenceImpedanceDesignValue, @ZeroSequenceImpedanceStandardValue, @ProductType, @ReferenceTemperature)";
                sqlCommond.Parameters.AddWithValue("@FigureNumber", FigureNumber);
                sqlCommond.Parameters.AddWithValue("@UserName", UserName);
                sqlCommond.Parameters.AddWithValue("@Model", Model);
                sqlCommond.Parameters.AddWithValue("@RatedCapacity", RatedCapacity);
                sqlCommond.Parameters.AddWithValue("@RatedHighVoltage", RatedHighVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedHighCurrent", RatedHighCurrent);
                sqlCommond.Parameters.AddWithValue("@RatedLowVoltage", RatedLowVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedLowCurrent", RatedLowCurrent);
                sqlCommond.Parameters.AddWithValue("@Sequence", Sequence);
                sqlCommond.Parameters.AddWithValue("@NickName", NickName);
                sqlCommond.Parameters.AddWithValue("@ConnectionGroupLabel", ConnectionGroupLabel);
                sqlCommond.Parameters.AddWithValue("@HeatResistanceRating", HeatResistanceRating);
                sqlCommond.Parameters.AddWithValue("@CoolingType", CoolingType);
                sqlCommond.Parameters.AddWithValue("@TestingDate", TestingDate);
                sqlCommond.Parameters.AddWithValue("@SystemVoltage", SystemVoltage);
                sqlCommond.Parameters.AddWithValue("@EndVoltage", EndVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedCurrent", RatedCurrent);
                sqlCommond.Parameters.AddWithValue("@ReactanceRate", ReactanceRate);
                sqlCommond.Parameters.AddWithValue("@ParallelCapacitorCapacity", ParallelCapacitorCapacity);
                sqlCommond.Parameters.AddWithValue("@WindingMaterial", WindingMaterial);
                sqlCommond.Parameters.AddWithValue("@RatedVoltage", RatedVoltage);
                sqlCommond.Parameters.AddWithValue("@MinCurrent", MinCurrent);
                sqlCommond.Parameters.AddWithValue("@MaxCurrent", MaxCurrent);
                sqlCommond.Parameters.AddWithValue("@GearCount", GearCount);
                sqlCommond.Parameters.AddWithValue("@CurrentRegulationMethod", CurrentRegulationMethod);
                sqlCommond.Parameters.AddWithValue("@HighVoltageWindingTapCount", HighVoltageWindingTapCount);
                sqlCommond.Parameters.AddWithValue("@RatedNeutralCurrent", RatedNeutralCurrent);
                sqlCommond.Parameters.AddWithValue("@ShortTimeNeutralCurrent", ShortTimeNeutralCurrent);
                sqlCommond.Parameters.AddWithValue("@HVCurrentAtRatedCapacityForLV", HVCurrentAtRatedCapacityForLV);
                sqlCommond.Parameters.AddWithValue("@HighVoltageResistance", HighVoltageResistance);
                sqlCommond.Parameters.AddWithValue("@LowVoltageResistanceMilli", LowVoltageResistanceMilli);
                sqlCommond.Parameters.AddWithValue("@NoLoadLossDesignValue", NoLoadLossDesignValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadLossStandardValue", NoLoadLossStandardValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadCurrentDesignValue", NoLoadCurrentDesignValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadCurrentStandardValue", NoLoadCurrentStandardValue);
                sqlCommond.Parameters.AddWithValue("@LoadLossDesignValue", LoadLossDesignValue);
                sqlCommond.Parameters.AddWithValue("@LoadLossStandardValue", LoadLossStandardValue);
                sqlCommond.Parameters.AddWithValue("@ShortCircuitImpedanceDesignValue", ShortCircuitImpedanceDesignValue);
                sqlCommond.Parameters.AddWithValue("@ShortCircuitImpedanceStandardValue", ShortCircuitImpedanceStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance10IDesignValue", ElectricReactanceUnbalance10IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance10IStandardValue", ElectricReactanceUnbalance10IStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceDeviation18IDesignValue", ElectricReactanceDeviation18IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceDeviation18IStandardValue", ElectricReactanceDeviation18IStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance18IDesignValue", ElectricReactanceUnbalance18IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance18IStandardValue", ElectricReactanceUnbalance18IStandardValue);
                sqlCommond.Parameters.AddWithValue("@LossMeasurementDesignValue", LossMeasurementDesignValue);
                sqlCommond.Parameters.AddWithValue("@LossMeasurementStandardValue", LossMeasurementStandardValue);
                sqlCommond.Parameters.AddWithValue("@ZeroSequenceImpedanceDesignValue", ZeroSequenceImpedanceDesignValue);
                sqlCommond.Parameters.AddWithValue("@ZeroSequenceImpedanceStandardValue", ZeroSequenceImpedanceStandardValue);
                sqlCommond.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCommond.Parameters.AddWithValue("@ReferenceTemperature", ReferenceTemperature);
                int count = sqlCommond.ExecuteNonQuery();
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
                string sql = $"UPDATE {TABLE_NAME} SET UserName = @UserName, Model = @Model, RatedCapacity = @RatedCapacity, RatedHighVoltage = @RatedHighVoltage, RatedHighCurrent = @RatedHighCurrent, RatedLowVoltage = @RatedLowVoltage, " +
                    $"RatedLowCurrent = @RatedLowCurrent, Sequence = @Sequence, NickName = @NickName, ConnectionGroupLabel = @ConnectionGroupLabel, HeatResistanceRating = @HeatResistanceRating, CoolingType = @CoolingType, " +
                    $"TestingDate = @TestingDate, SystemVoltage = @SystemVoltage, EndVoltage = @EndVoltage, RatedCurrent = @RatedCurrent, ReactanceRate = @ReactanceRate, ParallelCapacitorCapacity = @ParallelCapacitorCapacity, " +
                    $"WindingMaterial = @WindingMaterial, RatedVoltage = @RatedVoltage, MinCurrent = @MinCurrent, MaxCurrent = @MaxCurrent, GearCount = @GearCount, CurrentRegulationMethod = @CurrentRegulationMethod, " +
                    $"HighVoltageWindingTapCount = @HighVoltageWindingTapCount, RatedNeutralCurrent = @RatedNeutralCurrent, ShortTimeNeutralCurrent = @ShortTimeNeutralCurrent, HVCurrentAtRatedCapacityForLV = @HVCurrentAtRatedCapacityForLV, " +
                    $"HighVoltageResistance = @HighVoltageResistance, LowVoltageResistanceMilli = @LowVoltageResistanceMilli, NoLoadLossDesignValue = @NoLoadLossDesignValue, NoLoadLossStandardValue = @NoLoadLossStandardValue, " +
                    $"NoLoadCurrentDesignValue = @NoLoadCurrentDesignValue, NoLoadCurrentStandardValue = @NoLoadCurrentStandardValue, LoadLossDesignValue = @LoadLossDesignValue, LoadLossStandardValue = @LoadLossStandardValue, " +
                    $"ShortCircuitImpedanceDesignValue = @ShortCircuitImpedanceDesignValue, ShortCircuitImpedanceStandardValue = @ShortCircuitImpedanceStandardValue, " +
                    $"ElectricReactanceUnbalance10IDesignValue = @ElectricReactanceUnbalance10IDesignValue, ElectricReactanceUnbalance10IStandardValue = @ElectricReactanceUnbalance10IStandardValue, " +
                    $"ElectricReactanceDeviation18IDesignValue = @ElectricReactanceDeviation18IDesignValue, ElectricReactanceDeviation18IStandardValue = @ElectricReactanceDeviation18IStandardValue, " +
                    $"ElectricReactanceUnbalance18IDesignValue = @ElectricReactanceUnbalance18IDesignValue, ElectricReactanceUnbalance18IStandardValue = @ElectricReactanceUnbalance18IStandardValue, " +
                    $"LossMeasurementDesignValue = @LossMeasurementDesignValue, LossMeasurementStandardValue = @LossMeasurementStandardValue, ZeroSequenceImpedanceDesignValue = @ZeroSequenceImpedanceDesignValue, " +
                    $"ZeroSequenceImpedanceStandardValue = @ZeroSequenceImpedanceStandardValue, ProductType = @ProductType, ReferenceTemperature = @ReferenceTemperature WHERE FigureNumber = @FigureNumber";
                SQLCommond sqlCommond = new SQLCommond(sql, connection);
                sqlCommond.Parameters.AddWithValue("@FigureNumber", FigureNumber);
                sqlCommond.Parameters.AddWithValue("@UserName", UserName);
                sqlCommond.Parameters.AddWithValue("@Model", Model);
                sqlCommond.Parameters.AddWithValue("@RatedCapacity", RatedCapacity);
                sqlCommond.Parameters.AddWithValue("@RatedHighVoltage", RatedHighVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedHighCurrent", RatedHighCurrent);
                sqlCommond.Parameters.AddWithValue("@RatedLowVoltage", RatedLowVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedLowCurrent", RatedLowCurrent);
                sqlCommond.Parameters.AddWithValue("@Sequence", Sequence);
                sqlCommond.Parameters.AddWithValue("@NickName", NickName);
                sqlCommond.Parameters.AddWithValue("@ConnectionGroupLabel", ConnectionGroupLabel);
                sqlCommond.Parameters.AddWithValue("@HeatResistanceRating", HeatResistanceRating);
                sqlCommond.Parameters.AddWithValue("@CoolingType", CoolingType);
                sqlCommond.Parameters.AddWithValue("@TestingDate", TestingDate);
                sqlCommond.Parameters.AddWithValue("@SystemVoltage", SystemVoltage);
                sqlCommond.Parameters.AddWithValue("@EndVoltage", EndVoltage);
                sqlCommond.Parameters.AddWithValue("@RatedCurrent", RatedCurrent);
                sqlCommond.Parameters.AddWithValue("@ReactanceRate", ReactanceRate);
                sqlCommond.Parameters.AddWithValue("@ParallelCapacitorCapacity", ParallelCapacitorCapacity);
                sqlCommond.Parameters.AddWithValue("@WindingMaterial", WindingMaterial);
                sqlCommond.Parameters.AddWithValue("@RatedVoltage", RatedVoltage);
                sqlCommond.Parameters.AddWithValue("@MinCurrent", MinCurrent);
                sqlCommond.Parameters.AddWithValue("@MaxCurrent", MaxCurrent);
                sqlCommond.Parameters.AddWithValue("@GearCount", GearCount);
                sqlCommond.Parameters.AddWithValue("@CurrentRegulationMethod", CurrentRegulationMethod);
                sqlCommond.Parameters.AddWithValue("@HighVoltageWindingTapCount", HighVoltageWindingTapCount);
                sqlCommond.Parameters.AddWithValue("@RatedNeutralCurrent", RatedNeutralCurrent);
                sqlCommond.Parameters.AddWithValue("@ShortTimeNeutralCurrent", ShortTimeNeutralCurrent);
                sqlCommond.Parameters.AddWithValue("@HVCurrentAtRatedCapacityForLV", HVCurrentAtRatedCapacityForLV);
                sqlCommond.Parameters.AddWithValue("@HighVoltageResistance", HighVoltageResistance);
                sqlCommond.Parameters.AddWithValue("@LowVoltageResistanceMilli", LowVoltageResistanceMilli);
                sqlCommond.Parameters.AddWithValue("@NoLoadLossDesignValue", NoLoadLossDesignValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadLossStandardValue", NoLoadLossStandardValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadCurrentDesignValue", NoLoadCurrentDesignValue);
                sqlCommond.Parameters.AddWithValue("@NoLoadCurrentStandardValue", NoLoadCurrentStandardValue);
                sqlCommond.Parameters.AddWithValue("@LoadLossDesignValue", LoadLossDesignValue);
                sqlCommond.Parameters.AddWithValue("@LoadLossStandardValue", LoadLossStandardValue);
                sqlCommond.Parameters.AddWithValue("@ShortCircuitImpedanceDesignValue", ShortCircuitImpedanceDesignValue);
                sqlCommond.Parameters.AddWithValue("@ShortCircuitImpedanceStandardValue", ShortCircuitImpedanceStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance10IDesignValue", ElectricReactanceUnbalance10IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance10IStandardValue", ElectricReactanceUnbalance10IStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceDeviation18IDesignValue", ElectricReactanceDeviation18IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceDeviation18IStandardValue", ElectricReactanceDeviation18IStandardValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance18IDesignValue", ElectricReactanceUnbalance18IDesignValue);
                sqlCommond.Parameters.AddWithValue("@ElectricReactanceUnbalance18IStandardValue", ElectricReactanceUnbalance18IStandardValue);
                sqlCommond.Parameters.AddWithValue("@LossMeasurementDesignValue", LossMeasurementDesignValue);
                sqlCommond.Parameters.AddWithValue("@LossMeasurementStandardValue", LossMeasurementStandardValue);
                sqlCommond.Parameters.AddWithValue("@ZeroSequenceImpedanceDesignValue", ZeroSequenceImpedanceDesignValue);
                sqlCommond.Parameters.AddWithValue("@ZeroSequenceImpedanceStandardValue", ZeroSequenceImpedanceStandardValue);
                sqlCommond.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCommond.Parameters.AddWithValue("@ReferenceTemperature", ReferenceTemperature);
                int count = sqlCommond.ExecuteNonQuery();
                return count > 0;
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
                string query = $"SELECT * FROM {TABLE_NAME} ORDER BY FigureNumber" +
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

        public static bool DeleteData(string figureNumber)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE FigureNumber = @FigureNumber", connection);
                command.Parameters.AddWithValue("@FigureNumber", figureNumber);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

    }
}
