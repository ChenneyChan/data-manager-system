using ABBDataManagerSystem.Database;
using System.Data;

namespace ABBDataManagerSystem.Bean.Base
{
    public class CommonTempRiseTestRecordInfo
    {
        public static string TABLE_NAME = "commonTempRiseTestRecord";

        public string ID = String.Empty;
        public DateTime Timestamp;
        public float Ua = 0;
        public float Ub = 0;
        public float Uc = 0;
        public float U3 = 0;
        public float Ia = 0;
        public float Ib = 0;
        public float Ic = 0;
        public float I3 = 0;
        public float P3 = 0;
        public float CoreTemp = 0;
        public float WindingTempA = 0;
        public float WindingTempB = 0;
        public float WindingTempC = 0;
        public float EnvTempA = 0;
        public float EnvTempB = 0;
        public float EnvTempC = 0;
        public float EnvTempD = 0;

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "温升试验编号"},
            {"Timestamp", "试验时间"},
            {"CoreTemp", "铁心温度"},
            {"WindingTempA", "绕组A温度"},
            {"WindingTempB", "绕组B温度"},
            {"WindingTempC", "绕组C温度"},
            {"EnvTempA", "环境A温度"},
            {"EnvTempB", "环境B温度"},
            {"EnvTempC", "环境C温度"},
            {"EnvTempD", "环境D温度"},
        };

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                using (SQLCommond command = new SQLCommond($"INSERT INTO {TABLE_NAME} (ID, Timestamp, ua, ub, uc, u3, ia, ib, ic, i3, p3, " +
                    $"CoreTemperature, WindingATemperature, WindingBTemperature, WindingCTemperature, AmbientATemperature, AmbientBTemperature, AmbientCTemperature, AmbientDTemperature) VALUES " +
                    "(@ID, @Timestamp, @ua, @ub, @uc, @u3, @ia, @ib, @ic, @i3, @p3, " +
                    $"@CoreTemp, @WindingTempA, @WindingTempB, @WindingTempC, @EnvTempA, @EnvTempB, @EnvTempC, @EnvTempD)", connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@Timestamp", Timestamp);
                    command.Parameters.AddWithValue("@ua", Ua);
                    command.Parameters.AddWithValue("@ub", Ub);
                    command.Parameters.AddWithValue("@uc", Uc);
                    command.Parameters.AddWithValue("@u3", U3);
                    command.Parameters.AddWithValue("@ia", Ia);
                    command.Parameters.AddWithValue("@ib", Ib);
                    command.Parameters.AddWithValue("@ic", Ic);
                    command.Parameters.AddWithValue("@i3", I3);
                    command.Parameters.AddWithValue("@p3", P3);
                    command.Parameters.AddWithValue("@CoreTemp", CoreTemp);
                    command.Parameters.AddWithValue("@WindingTempA", WindingTempA);
                    command.Parameters.AddWithValue("@WindingTempB", WindingTempB);
                    command.Parameters.AddWithValue("@WindingTempC", WindingTempC);
                    command.Parameters.AddWithValue("@EnvTempA", EnvTempA);
                    command.Parameters.AddWithValue("@EnvTempB", EnvTempB);
                    command.Parameters.AddWithValue("@EnvTempC", EnvTempC);
                    command.Parameters.AddWithValue("@EnvTempD", EnvTempD);
                    int count = command.ExecuteNonQuery();
                    return count > 0;
                }
            }
        }

        public static List<CommonTempRiseTestRecordInfo> ReadFromDB(string sequence = "", string testPhase = "", string testStatus = "")
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (sequence != "")
            {
                queryDataSql += $" WHERE workflow_id = '{sequence}'";
                if (testPhase != "")
                {
                    queryDataSql += $" AND test_phase = '{testPhase}'";
                }
                if (testStatus != "")
                {
                    queryDataSql += $" AND test_status = '{testStatus}'";
                }
            }
            List<CommonTempRiseTestRecordInfo>? records = DBConnector.QueryFromDB<CommonTempRiseTestRecordInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new CommonTempRiseTestRecordInfo
                {
                    ID = reader.GetString("ID"),
                    Timestamp = reader.GetDateTime("Timestamp"),
                };
            });
            if (records == null)
            {
                return new List<CommonTempRiseTestRecordInfo>();
            }
            Log.Info("CommonTempRiseTestRecordInfo COUNT = " + records.Count);
            return records;
        }


        public static bool DeleteData(string ID)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", ID);
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
