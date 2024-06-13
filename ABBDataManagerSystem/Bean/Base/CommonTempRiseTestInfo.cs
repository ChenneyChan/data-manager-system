using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class CommonTempRiseTestInfo
    {
        public static string TABLE_NAME = "commonTempRiseConfigRecord";

        public string ID = String.Empty;
        public string WorkflowId = String.Empty;
        public string TestingPhase = String.Empty;
        public string TestingStatus = String.Empty;
        public string CoolingMode = String.Empty;
        public int TestingIndex = 1;
        public DateTime? DateTime = null;

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "温升试验编号"},
            {"WorkflowId", "出厂序号"},
            {"TestingPhase", "试验阶段"},
            {"TestingStatus", "试验状态"},
            {"TestingIndex", "试验次数"},
            {"CoolingMode", "冷却方式"},
            {"DateTime", "试验时间"},
        };

        public CommonTempRiseTestInfo()
        {
            ID = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, workflow_id, testing_phase, testing_status, testing_index, cooling_mode, " +
                    $"datetime) VALUES (@ID, @WorkflowId, @TestingPhase, @TestingStatus, @TestingIndex, @CoolingMode, " +
                    $"@DateTime)", connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@WorkflowId", WorkflowId);
                    command.Parameters.AddWithValue("@TestingPhase", TestingPhase);
                    command.Parameters.AddWithValue("@TestingStatus", TestingStatus);
                    command.Parameters.AddWithValue("@TestingIndex", TestingIndex);
                    command.Parameters.AddWithValue("@CoolingMode", CoolingMode);
                    command.Parameters.AddWithValue("@DateTime", DateTime??System.DateTime.Now);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static List<CommonTempRiseTestInfo> ReadFromDB(string sequence = "", string testPhase = "", string testStatus = "", string coolingMode = "", int testIndex = 1)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != "")
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
                if (testPhase != "")
                {
                    queryDataSql += $" AND testing_phase = '{testPhase}'";
                }
                if (testStatus != "")
                {
                    queryDataSql += $" AND testing_status = '{testStatus}'";
                }
                if (coolingMode != "")
                {
                    queryDataSql += $" AND cooling_mode = '{coolingMode}'";
                }
                if (testIndex != 0)
                {
                    queryDataSql += $" AND testing_index = '{testIndex}'";
                }
            }
            List<CommonTempRiseTestInfo>? records = DBConnector.QueryFromDB<CommonTempRiseTestInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new CommonTempRiseTestInfo
                {
                    ID = reader.GetString("ID"),
                    WorkflowId = reader.GetString("workflow_id"),
                    TestingPhase = reader.GetString("testing_phase"),
                    TestingStatus = reader.GetString("testing_status"),
                    TestingIndex = reader.GetInt32("testing_index"),
                    CoolingMode = reader.GetString("cooling_mode"),
                    DateTime = !reader.IsDBNull("datetime") ? reader.GetDateTime("datetime") : System.DateTime.Now,
                };
            });
            if (records == null)
            {
                return new List<CommonTempRiseTestInfo>();
            }
            Log.Info("CommonTempRiseTestInfo COUNT = " + records.Count);
            return records;
        }


        public static bool DeleteData(string workflowID, string testPhase, string testStatus, int testIndex)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE workflow_id = @WorkflowId  AND testing_phase = @TestPhase AND testing_status = @TestStatus AND testing_index = @TestIndex", connection);
                command.Parameters.AddWithValue("@WorkflowId", workflowID);
                command.Parameters.AddWithValue("@TestPhase", testPhase);
                command.Parameters.AddWithValue("@TestStatus", testStatus);
                command.Parameters.AddWithValue("@TestIndex", testIndex);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
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
