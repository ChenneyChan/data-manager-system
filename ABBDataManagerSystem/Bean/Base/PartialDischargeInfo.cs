using ElectricalDataManagerSystem.Database;
using System.Data;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class PartialDischargeInfo
    {

        public string WorkflowID = string.Empty;
        public float DischargeA18 = 0;
        public float DischargeB18 = 0;
        public float DischargeC18 = 0;
        public float DischargeA13 = 0;
        public float DischargeB13 = 0;
        public float DischargeC13 = 0;

        public static string TABLE_NAME = "partialDischargeRecord";

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"WorkflowID", "出厂序号"},
            {"DischargeA18", "A相（pC）"},
            {"DischargeB18", "B相（pC）"},
            {"DischargeC18", "C相（pC）"},
            {"DischargeA13", "A相（pC）"},
            {"DischargeB13", "B相（pC）"},
            {"DischargeC13", "C相（pC）"},
        };

        public PartialDischargeInfo()
        {
        }

        public bool InsertData()
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                CreateSqliteTable(connection);
                SQLCommond command = new SQLCommond($"IINSERT INTO {TABLE_NAME} (workflow_id, pca18, pcb18, pcc18, pca13, pcb13, pcc13) VALUES(@WorkflowID, @DischargeA18, @DischargeB18, @DischargeC18, @DischargeA13, @DischargeB13, @DischargeC13)", connection);
                command.Parameters.AddWithValue("@WorkflowID", WorkflowID);
                command.Parameters.AddWithValue("@DischargeA18", DischargeA18);
                command.Parameters.AddWithValue("@DischargeB18", DischargeB18);
                command.Parameters.AddWithValue("@DischargeC18", DischargeC18);
                command.Parameters.AddWithValue("@DischargeA13", DischargeA13);
                command.Parameters.AddWithValue("@DischargeB13", DischargeB13);
                command.Parameters.AddWithValue("@DischargeC13", DischargeC13);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static bool DeleteData(string Sequence)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                SQLCommond command = new SQLCommond($"DELETE FROM {TABLE_NAME} WHERE workflow_id = @Sequence", connection);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                int count = command.ExecuteNonQuery();
                return count > 0;
            }
        }

        public static CheckIsKeyField GetCheckIsKeyFieldDelegate()
        {
            CheckIsKeyField checkIsKeyField = (string filedName) =>
            {
                return (filedName == "WorkflowID");
            };
            return checkIsKeyField;
        }
    }
}
