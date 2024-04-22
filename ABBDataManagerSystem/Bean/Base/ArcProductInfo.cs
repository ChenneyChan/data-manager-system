using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricalDataManagerSystem.Database;
using System.Data;
using ElectricalDataManagerSystem.TmctlAdapter;

namespace ElectricalDataManagerSystem.Bean.Base
{
    public class ArcProductInfo
    {
        public static string TABLE_NAME = "ArcProductInfo";

        //DDL
        /* CREATE TABLE `ArcProductInfo` (
        `ID` BIGINT PRIMARY KEY auto_increment,  
        `Sequence` VARCHAR(255) NOT NULL,
        `UserName` VARCHAR(255) NOT NULL,
        `Model` VARCHAR(255) NOT NULL,
        `Code` VARCHAR(255) NOT NULL,
        `RatedCapacity` FLOAT NOT NULL,
        `RatedVoltage` FLOAT NOT NULL,
        `MinCurrent` FLOAT NOT NULL,
        `MaxCurrent` FLOAT NOT NULL,
        `GearCount` INT NOT NULL,
        `CoolingType` VARCHAR(255) NOT NULL,
        `CurrentAdjustType` VARCHAR(255) NOT NULL,
        `TestingDate` VARCHAR(255) NOT NULL,
        `HeatResistanceRating` VARCHAR(255) NOT NULL,
        `IsTemplate` TINYINT(1) NOT NULL
        );
        */

        public static Dictionary<string, string> FieldComments = new Dictionary<string, string>
        {
            {"ID", "ID"},
            {"Sequence", "出厂序号"},
            {"UserName", "用户名称"},
            {"Model", "产品型号"},
            {"Code", "产品代号"},
            {"RatedCapacity", "额定容量 kVA"},
            {"RatedVoltage", "额定电压 kV"},
            {"MinCurrent", "最小电流 A"},
            {"MaxCurrent", "最大电流 A"},
            {"GearCount", "档位个数"},
            {"CoolingType", "冷却方式"},
            {"CurrentAdjustType", "电流调节方式"},
            {"TestingDate", "试验日期"},
            {"HeatResistanceRating", "耐热等级"},
            {"IsTemplate", "模板"},
        };

        public int ID { get; set; } = 0; // ID，唯一自增Key

        public string Sequence { get; set; } = String.Empty; // 出厂序号

        public string UserName { get; set; } = String.Empty; // 用户名称

        public string Model { get; set; } = String.Empty; // 产品型号

        public string Code { get; set; } = String.Empty; // 产品代号

        public float RatedCapacity { get; set; } = 0; // 额定容量 kVA

        public float RatedVoltage { get; set; } = 0; // 额定电压 kV

        public float MinCurrent { get; set; } = 0; // 最小电流 A

        public float MaxCurrent { get; set; } = 0; // 最大电流 A

        public int GearCount { get; set; } = 0;  // 档位个数

        public string CoolingType { get; set; } = String.Empty; // 冷却方式

        public string CurrentAdjustType { get; set; } = String.Empty; // 电流调节方式

        public string TestingDate { get; set; } = String.Empty; // 试验日期

        public string HeatResistanceRating { get; set; } = String.Empty; // 耐热等级

        public int IsTemplate { get; set; } = 0;

        public static List<ArcProductInfo>? GetFromDB(bool filterTemplate = false)
        {
            // 查询数据
            string queryDataSql = $"SELECT * FROM {TABLE_NAME}";
            if (filterTemplate)
            {
                queryDataSql += " WHERE IsTemplate = 1";
            }
            List<ArcProductInfo>? records = DBConnector.QueryFromDB<ArcProductInfo>(queryDataSql, (reader) =>
            {
                if (reader == null)
                {
                    return null;
                }
                return new ArcProductInfo
                {
                    ID = reader.GetInt32(0),
                    Sequence = reader.GetString(1),
                    UserName = reader.GetString(2),
                    Model = reader.GetString(3),
                    Code = reader.GetString(4),
                    RatedCapacity = (float)reader.GetDouble(5),
                    RatedVoltage = (float)reader.GetDouble(6),
                    MinCurrent = (float)reader.GetDouble(7),
                    MaxCurrent = (float)reader.GetDouble(8),
                    GearCount = reader.GetInt32(9),
                    CoolingType = reader.GetString(10),
                    CurrentAdjustType = reader.GetString(11),
                    TestingDate = reader.GetString(12),
                    HeatResistanceRating = reader.GetString(13),
                    IsTemplate = reader.GetInt32(14),
                };
            });
            if (records == null)
            {
                return null;
            }
            Log.Info("ArcProductInfo COUNT = " + records.Count);
            return records;
        }

        public bool WriteToDB()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();

                CreateSqliteTable(connection);

                SQLCommond sqliteCmd;
                sqliteCmd = connection.CreateCommand();
                sqliteCmd.CommandText = $"INSERT INTO {TABLE_NAME} (Sequence, UserName, Model, Code, RatedCapacity, RatedVoltage, MinCurrent, MaxCurrent, GearCount, CoolingType, CurrentAdjustType, TestingDate, HeatResistanceRating, IsTemplate) VALUES " +
                    $"(@Sequence, @UserName, @Model, @Code, @RatedCapacity, @RatedVoltage, @MinCurrent, @MaxCurrent, @GearCount, @CoolingType, @CurrentAdjustType, @TestingDate, @HeatResistanceRating, @IsTemplate)";
                sqliteCmd.Parameters.AddWithValue("@Sequence", Sequence);
                sqliteCmd.Parameters.AddWithValue("@UserName", UserName);
                sqliteCmd.Parameters.AddWithValue("@Model", Model);
                sqliteCmd.Parameters.AddWithValue("@Code", Code);
                sqliteCmd.Parameters.AddWithValue("@RatedCapacity", RatedCapacity);
                sqliteCmd.Parameters.AddWithValue("@RatedVoltage", RatedVoltage);
                sqliteCmd.Parameters.AddWithValue("@MinCurrent", MinCurrent);
                sqliteCmd.Parameters.AddWithValue("@MaxCurrent", MaxCurrent);
                sqliteCmd.Parameters.AddWithValue("@GearCount", GearCount);
                sqliteCmd.Parameters.AddWithValue("@CoolingType", CoolingType);
                sqliteCmd.Parameters.AddWithValue("@CurrentAdjustType", CurrentAdjustType);
                sqliteCmd.Parameters.AddWithValue("@TestingDate", TestingDate);
                sqliteCmd.Parameters.AddWithValue("@HeatResistanceRating", HeatResistanceRating);
                sqliteCmd.Parameters.AddWithValue("@IsTemplate", IsTemplate);
                int count = sqliteCmd.ExecuteNonQuery();
                return count > 0;
            }
        }

        public void UpdateData()
        {
            // 创建 SQLite 连接对象
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                // 打开数据库连接
                connection.Open();
                string sql = $"UPDATE {TABLE_NAME} SET Sequence = @Sequence, UserName = @UserName, Model = @Model, Code = @Code, " +
                    $"RatedCapacity = @RatedCapacity, RatedVoltage = @RatedVoltage, MinCurrent = @MinCurrent, MaxCurrent = @MaxCurrent, " +
                    $"GearCount = @GearCount, CoolingType = @CoolingType, CurrentAdjustType = @CurrentAdjustType, TestingDate = @TestingDate, " +
                    $"HeatResistanceRating = @HeatResistanceRating, IsTemplate = @IsTemplate WHERE ID = @ID";
                SQLCommond command = new SQLCommond(sql, connection);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@Sequence", Sequence);
                command.Parameters.AddWithValue("@UserName", UserName);
                command.Parameters.AddWithValue("@Model", Model);
                command.Parameters.AddWithValue("@Code", Code);
                command.Parameters.AddWithValue("@RatedCapacity", RatedCapacity);
                command.Parameters.AddWithValue("@RatedVoltage", RatedVoltage);
                command.Parameters.AddWithValue("@MinCurrent", MinCurrent);
                command.Parameters.AddWithValue("@MaxCurrent", MaxCurrent);
                command.Parameters.AddWithValue("@GearCount", GearCount);
                command.Parameters.AddWithValue("@CoolingType", CoolingType);
                command.Parameters.AddWithValue("@CurrentAdjustType", CurrentAdjustType);
                command.Parameters.AddWithValue("@TestingDate", TestingDate);
                command.Parameters.AddWithValue("@HeatResistanceRating", HeatResistanceRating);
                command.Parameters.AddWithValue("@IsTemplate", IsTemplate);

                command.ExecuteNonQuery();
            }
        }

        public static void FillDataTable(DataTable dt, int pageSize, int page)
        {
            using (SQLConnection connection = new SQLConnection(DBConnector.GetConnectionString()))
            {
                connection.Open();
                string query = $"SELECT * FROM {TABLE_NAME} ORDER BY ID" +
                    $" LIMIT {pageSize} OFFSET {pageSize * page}";
                using (SQLCommond queryDataCmd = new SQLCommond(query, connection))
                {
                    SQLDataAdapter adapter = new SQLDataAdapter(queryDataCmd);
                    adapter.Fill(dt);
                }
            }
        }

        private void CreateSqliteTable(SQLConnection connection)
        {
            if (!DBConnector.USING_SQLITE)
            {
                return;
            }
            // 创建数据库表
            string createTableSql = $"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (" +
                $"    ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                $"    Sequence TEXT," +
                $"    UserName TEXT," +
                $"    Model TEXT," +
                $"    Code TEXT," +
                $"    RatedCapacity REAL," +
                $"    RatedVoltage REAL," +
                $"    MinCurrent REAL," +
                $"    MaxCurrent REAL," +
                $"    GearCount INTEGER," +
                $"    CoolingType TEXT," +
                $"    CurrentAdjustType TEXT," +
                $"    TestingDate TEXT," +
                $"    HeatResistanceRating TEXT," +
                $"    IsTemplate INTEGER);";
            using (SQLCommond createTableCmd = new SQLCommond(createTableSql, connection))
            {
                createTableCmd.ExecuteNonQuery();
            }
        }

        public override string ToString()
        {
            return $"ID: {ID}, Sequence: {Sequence}, UserName: {UserName}, Model: {Model}, Code: {Code}, " +
                $"RatedCapacity: {RatedCapacity}, RatedVoltage: {RatedVoltage}, MinCurrent: {MinCurrent}, " +
                $"MaxCurrent: {MaxCurrent}, GearCount: {GearCount}, CoolingType: {CoolingType}, " +
                $"CurrentAdjustType: {CurrentAdjustType}, TestingDate: {TestingDate}, " +
                $"HeatResistanceRating: {HeatResistanceRating}, IsTemplate: {IsTemplate}";
        }
    }
}
