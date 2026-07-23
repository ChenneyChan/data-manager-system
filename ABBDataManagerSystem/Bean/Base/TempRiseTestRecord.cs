/*
 * ============================================================================
 * TempRiseTestRecord -- 温升测试数据记录表（替代 commonTempRiseTestRecord）
 *
 * 固定字段：电压/电流/功率/频率 + 水冷设备温度/流量
 * 动态字段：用户自定义的温度通道值，以 JSON 格式存储在 temperatures 列中
 *
 * ----------------------------------------------------------------------------
 * 建表脚本 (MySQL):
 *
 * CREATE TABLE IF NOT EXISTS tempRiseTestRecord (
 *     id                    BIGINT PRIMARY KEY AUTO_INCREMENT,
 *     test_info_id          VARCHAR(36)  NOT NULL COMMENT '关联 commonTempRiseConfigRecord.ID',
 *     workflow_id           VARCHAR(128) NOT NULL COMMENT '产品工作令号',
 *     timestamp             DATETIME     NOT NULL COMMENT '采集时间点',
 *     -- 固定字段: 电压/电流/功率/频率
 *     ua FLOAT DEFAULT NULL COMMENT 'A相电压(V)',
 *     ub FLOAT DEFAULT NULL COMMENT 'B相电压(V)',
 *     uc FLOAT DEFAULT NULL COMMENT 'C相电压(V)',
 *     u3 FLOAT DEFAULT NULL COMMENT '三相平均电压(V)',
 *     ia FLOAT DEFAULT NULL COMMENT 'A相电流(A)',
 *     ib FLOAT DEFAULT NULL COMMENT 'B相电流(A)',
 *     ic FLOAT DEFAULT NULL COMMENT 'C相电流(A)',
 *     i3 FLOAT DEFAULT NULL COMMENT '三相平均电流(A)',
 *     p3 FLOAT DEFAULT NULL COMMENT '三相总有功功率(kW)',
 *     fu FLOAT DEFAULT NULL COMMENT '频率(Hz)',
*     -- 动态字段: 用户自定义温度通道
 *     temperatures          JSON         NOT NULL COMMENT '温度通道值 {"Channel1":23.5,"Channel2":30.0,...}',
*     -- 固定字段: 水冷设备 (仅 AFWF 模式有值)
 *     outlet_water_temp     FLOAT DEFAULT NULL COMMENT '出水口温度(℃)',
 *     inlet_water_temp      FLOAT DEFAULT NULL COMMENT '回水口温度(℃)',
 *     ambient_temp_1        FLOAT DEFAULT NULL COMMENT '环境温度1(℃)',
 *     ambient_temp_2        FLOAT DEFAULT NULL COMMENT '环境温度2(℃)',
 *     outlet_air_temp_1     FLOAT DEFAULT NULL COMMENT '出风口温度1(℃)',
 *     outlet_air_temp_2     FLOAT DEFAULT NULL COMMENT '出风口温度2(℃)',
 *     outlet_air_temp_3     FLOAT DEFAULT NULL COMMENT '出风口温度3(℃)',
 *     outlet_air_temp_4     FLOAT DEFAULT NULL COMMENT '出风口温度4(℃)',
 *     outlet_air_temp_5     FLOAT DEFAULT NULL COMMENT '出风口温度5(℃)',
 *     outlet_air_temp_6     FLOAT DEFAULT NULL COMMENT '出风口温度6(℃)',
 *     outlet_air_temp_7     FLOAT DEFAULT NULL COMMENT '出风口温度7(℃)',
 *     outlet_air_temp_8     FLOAT DEFAULT NULL COMMENT '出风口温度8(℃)',
 *     water_flow_rate       FLOAT DEFAULT NULL COMMENT '水流量(m³/h)',
 *     INDEX idx_workflow_time (workflow_id, timestamp),
 *     INDEX idx_test_info (test_info_id)
 * ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='温升测试数据记录表';
 *
* temperatures JSON 结构示例:
* {
 *   "Channel1": 23.5,
 *   "Channel2": 24.1,
 *   "Channel3": 23.8,
 *   "Channel4": 30.2,
 *   "Channel5": 18.0,
 *   "Channel6": 22.3
* }
 *
 * 迁移说明:
 * 旧表 commonTempRiseTestRecord 保留不动，新数据写入此表。
 * ============================================================================
 */

using ABBDataManagerSystem.Database;
using System.Text.Json;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class TempRiseTestRecord
    {
        public static string TABLE_NAME = "tempRiseTestRecord";
        private const int BATCH_SIZE = 1000;

        public long ID { get; set; }
        public string TestInfoId { get; set; }
        public string WorkflowId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        // 固定: 电压/电流/功率/频率
        public float? Ua { get; set; }
        public float? Ub { get; set; }
        public float? Uc { get; set; }
        public float? U3 { get; set; }
        public float? Ia { get; set; }
        public float? Ib { get; set; }
        public float? Ic { get; set; }
        public float? I3 { get; set; }
        public float? P3 { get; set; }
        public float? FU { get; set; }
        // 动态: 温度 JSON
        public string TemperaturesJson { get; set; } = "{}";
        // 固定: 水冷设备
        public float? OutletWaterTemperature { get; set; }
        public float? InletWaterTemperature { get; set; }
        public float? AmbientTemperature1 { get; set; }
        public float? AmbientTemperature2 { get; set; }
        public float? OutletAirTemperature1 { get; set; }
        public float? OutletAirTemperature2 { get; set; }
        public float? OutletAirTemperature3 { get; set; }
        public float? OutletAirTemperature4 { get; set; }
        public float? OutletAirTemperature5 { get; set; }
        public float? OutletAirTemperature6 { get; set; }
        public float? OutletAirTemperature7 { get; set; }
        public float? OutletAirTemperature8 { get; set; }
        public float? WaterFlowRate { get; set; }
        public bool IsAFWF { get; set; } = false;

        public TempRiseTestRecord() { }

        /// <summary>
        /// 批量插入温升数据记录
        /// </summary>
        public static bool BatchInsertData(List<TempRiseTestRecord> dataRows)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();

                string insertSql = $"INSERT INTO {TABLE_NAME} (test_info_id, workflow_id, timestamp, " +
                    $"ua, ub, uc, u3, ia, ib, ic, i3, p3, fu, " +
                    $"temperatures, " +
                    $"outlet_water_temp, inlet_water_temp, ambient_temp_1, ambient_temp_2, " +
                    $"outlet_air_temp_1, outlet_air_temp_2, outlet_air_temp_3, outlet_air_temp_4, " +
                    $"outlet_air_temp_5, outlet_air_temp_6, outlet_air_temp_7, outlet_air_temp_8, " +
                    $"water_flow_rate) VALUES " +
                    $"(@testInfoId, @workflowId, @timestamp, " +
                    $"@ua, @ub, @uc, @u3, @ia, @ib, @ic, @i3, @p3, @fu, " +
                    $"@temperatures, " +
                    $"@outletWaterTemp, @inletWaterTemp, @ambientTemp1, @ambientTemp2, " +
                    $"@outletAirTemp1, @outletAirTemp2, @outletAirTemp3, @outletAirTemp4, " +
                    $"@outletAirTemp5, @outletAirTemp6, @outletAirTemp7, @outletAirTemp8, " +
                    $"@waterFlowRate)";

                int totalCount = dataRows.Count;
                int processedCount = 0;

                try
                {
                    while (processedCount < totalCount)
                    {
                        int batchEnd = Math.Min(processedCount + BATCH_SIZE, totalCount);
                        using (SQLTransaction transaction = connection.BeginTransaction())
                        {
                            using (SQLCommond command = new SQLCommond())
                            {
                                command.Connection = connection;
                                command.Transaction = transaction;
                                command.CommandText = insertSql;
                                command.CommandTimeout = 300;

                                command.Parameters.AddWithValue("@testInfoId", string.Empty);
                                command.Parameters.AddWithValue("@workflowId", string.Empty);
                                command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                                command.Parameters.AddWithValue("@ua", DBNull.Value);
                                command.Parameters.AddWithValue("@ub", DBNull.Value);
                                command.Parameters.AddWithValue("@uc", DBNull.Value);
                                command.Parameters.AddWithValue("@u3", DBNull.Value);
                                command.Parameters.AddWithValue("@ia", DBNull.Value);
                                command.Parameters.AddWithValue("@ib", DBNull.Value);
                                command.Parameters.AddWithValue("@ic", DBNull.Value);
                                command.Parameters.AddWithValue("@i3", DBNull.Value);
                                command.Parameters.AddWithValue("@p3", DBNull.Value);
                                command.Parameters.AddWithValue("@fu", DBNull.Value);
                                command.Parameters.AddWithValue("@temperatures", "{}");
                                command.Parameters.AddWithValue("@outletWaterTemp", DBNull.Value);
                                command.Parameters.AddWithValue("@inletWaterTemp", DBNull.Value);
                                command.Parameters.AddWithValue("@ambientTemp1", DBNull.Value);
                                command.Parameters.AddWithValue("@ambientTemp2", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp1", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp2", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp3", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp4", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp5", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp6", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp7", DBNull.Value);
                                command.Parameters.AddWithValue("@outletAirTemp8", DBNull.Value);
                                command.Parameters.AddWithValue("@waterFlowRate", DBNull.Value);

                                for (int i = processedCount; i < batchEnd; i++)
                                {
                                    var row = dataRows[i];
                                    command.Parameters["@testInfoId"].Value = row.TestInfoId;
                                    command.Parameters["@workflowId"].Value = row.WorkflowId;
                                    command.Parameters["@timestamp"].Value = row.Timestamp;
                                    command.Parameters["@ua"].Value = (object?)row.Ua ?? DBNull.Value;
                                    command.Parameters["@ub"].Value = (object?)row.Ub ?? DBNull.Value;
                                    command.Parameters["@uc"].Value = (object?)row.Uc ?? DBNull.Value;
                                    command.Parameters["@u3"].Value = (object?)row.U3 ?? DBNull.Value;
                                    command.Parameters["@ia"].Value = (object?)row.Ia ?? DBNull.Value;
                                    command.Parameters["@ib"].Value = (object?)row.Ib ?? DBNull.Value;
                                    command.Parameters["@ic"].Value = (object?)row.Ic ?? DBNull.Value;
                                    command.Parameters["@i3"].Value = (object?)row.I3 ?? DBNull.Value;
                                    command.Parameters["@p3"].Value = (object?)row.P3 ?? DBNull.Value;
                                    command.Parameters["@fu"].Value = (object?)row.FU ?? DBNull.Value;
                                    command.Parameters["@temperatures"].Value = row.TemperaturesJson;
                                    command.Parameters["@outletWaterTemp"].Value = (object?)row.OutletWaterTemperature ?? DBNull.Value;
                                    command.Parameters["@inletWaterTemp"].Value = (object?)row.InletWaterTemperature ?? DBNull.Value;
                                    command.Parameters["@ambientTemp1"].Value = (object?)row.AmbientTemperature1 ?? DBNull.Value;
                                    command.Parameters["@ambientTemp2"].Value = (object?)row.AmbientTemperature2 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp1"].Value = (object?)row.OutletAirTemperature1 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp2"].Value = (object?)row.OutletAirTemperature2 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp3"].Value = (object?)row.OutletAirTemperature3 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp4"].Value = (object?)row.OutletAirTemperature4 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp5"].Value = (object?)row.OutletAirTemperature5 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp6"].Value = (object?)row.OutletAirTemperature6 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp7"].Value = (object?)row.OutletAirTemperature7 ?? DBNull.Value;
                                    command.Parameters["@outletAirTemp8"].Value = (object?)row.OutletAirTemperature8 ?? DBNull.Value;
                                    command.Parameters["@waterFlowRate"].Value = (object?)row.WaterFlowRate ?? DBNull.Value;
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                            }
                        }
                        processedCount = batchEnd;
                        Log.Info($"TempRise batch insert progress: {processedCount}/{totalCount}");
                    }

                    Log.Info("All TempRise records inserted.");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error($"TempRise batch insert error: {ex.Message}, inserted {processedCount}/{totalCount}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除指定 test_info_id 关联的所有数据记录
        /// </summary>
        public static bool DeleteData(string testInfoId)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string sql = $"DELETE FROM {TABLE_NAME} WHERE test_info_id = @testInfoId";
                using (SQLCommond command = new SQLCommond(sql, connection))
                {
                    command.Parameters.AddWithValue("@testInfoId", testInfoId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
