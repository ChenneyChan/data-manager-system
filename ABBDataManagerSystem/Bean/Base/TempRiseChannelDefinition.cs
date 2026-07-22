/*
 * ============================================================================
 * TempRiseChannelDefinition -- 温升通道定义表
 *
 * 每个产品(workflow_id) + 冷却方式(cooling_mode) 可以定义一套独立的温度通道。
 * 通道定义以 JSON 数组形式存储，key 为 (workflow_id, cooling_mode) 唯一。
 *
 * ----------------------------------------------------------------------------
 * 建表脚本 (MySQL):
 *
 * CREATE TABLE IF NOT EXISTS tempRiseChannelDefinition (
 *     id           BIGINT PRIMARY KEY AUTO_INCREMENT,
 *     workflow_id  VARCHAR(128) NOT NULL COMMENT '产品工作令号',
 *     cooling_mode VARCHAR(32)  NOT NULL COMMENT '冷却方式: AF / AF+ / AFWF',
 *     channel_defs JSON         NOT NULL COMMENT '通道定义JSON数组 [{index,roleKey,roleName,title,probe},...]',
 *     updated_at   DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 *     UNIQUE KEY uk_workflow_cooling (workflow_id, cooling_mode)
 * ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='温升测试通道定义表';
 *
 * channel_defs JSON 结构示例:
 * [
 *   {"index":0, "roleKey":"WindingA", "roleName":"绕组A", "title":"高压A相", "probe":"Slot-3"},
 *   {"index":1, "roleKey":"Core",     "roleName":"铁心",   "title":"铁心温度", "probe":"Slot-7"},
 *   ...
 * ]
 * ============================================================================
 */

using ABBDataManagerSystem.Database;
using ABBDataManagerSystem.Tools;
using System.Text.Json;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class TempRiseChannelDefinition
    {
        public static string TABLE_NAME = "tempRiseChannelDefinition";

        public long ID { get; set; }
        public string WorkflowId { get; set; } = string.Empty;
        public string CoolingMode { get; set; } = string.Empty;
        public string ChannelDefsJson { get; set; } = "[]";

        public TempRiseChannelDefinition() { }

        public TempRiseChannelDefinition(string workflowId, string coolingMode, string channelDefsJson)
        {
            WorkflowId = workflowId;
            CoolingMode = coolingMode;
            ChannelDefsJson = channelDefsJson;
        }

        /// <summary>
        /// 插入或更新通道定义（按 workflow_id + cooling_mode 唯一键 upsert）
        /// </summary>
        public bool Save()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string sql = $@"INSERT INTO {TABLE_NAME} (workflow_id, cooling_mode, channel_defs)
                    VALUES (@workflowId, @coolingMode, @channelDefs)
                    ON DUPLICATE KEY UPDATE channel_defs = VALUES(channel_defs)";

                using (SQLCommond command = new SQLCommond(sql, connection))
                {
                    command.Parameters.AddWithValue("@workflowId", WorkflowId);
                    command.Parameters.AddWithValue("@coolingMode", CoolingMode);
                    command.Parameters.AddWithValue("@channelDefs", ChannelDefsJson);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// 按 workflow_id + cooling_mode 读取通道定义。无记录返回 null。
        /// </summary>
        public static TempRiseChannelDefinition? Load(string workflowId, string coolingMode)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string sql = $"SELECT id, workflow_id, cooling_mode, channel_defs FROM {TABLE_NAME} WHERE workflow_id = @workflowId AND cooling_mode = @coolingMode";
                using (SQLCommond command = new SQLCommond(sql, connection))
                {
                    command.Parameters.AddWithValue("@workflowId", workflowId);
                    command.Parameters.AddWithValue("@coolingMode", coolingMode);
                    using (SQLDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TempRiseChannelDefinition
                            {
                                ID = reader.GetInt64("id"),
                                WorkflowId = reader.GetString("workflow_id"),
                                CoolingMode = reader.GetString("cooling_mode"),
                                ChannelDefsJson = reader.GetString("channel_defs"),
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 删除指定 workflow_id + cooling_mode 的通道定义
        /// </summary>
        public static bool Delete(string workflowId, string coolingMode)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string sql = $"DELETE FROM {TABLE_NAME} WHERE workflow_id = @workflowId AND cooling_mode = @coolingMode";
                using (SQLCommond command = new SQLCommond(sql, connection))
                {
                    command.Parameters.AddWithValue("@workflowId", workflowId);
                    command.Parameters.AddWithValue("@coolingMode", coolingMode);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}