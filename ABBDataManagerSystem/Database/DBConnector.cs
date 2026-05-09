using MySql.Data.MySqlClient;

namespace ABBDataManagerSystem.Database
{

    public delegate T? ParseDBRow<T>(SQLDataReader reader);

    internal class DBConnector
    {
        static string connectionString = "server=localhost;user=root;database=temp_test;port=3306;password=root";

        public static bool USING_SQLITE = false;


        public static string GetConnectionString()
        {
            if (USING_SQLITE)
            {
                return "Data Source=mydatabase.db;Version=3;";
            }
            return "server=" + Configs.Configs.Host + ";user=" + Configs.Configs.Username
                    + ";database=" + Configs.Configs.DatabaseName + ";port=" + Configs.Configs.Port + ";password=" + Configs.Configs.Password
                    + ";DefaultCommandTimeout=300";
        }

        public static SQLConnection? GetConnection()
        {
            SQLConnection conn = new SQLConnection(GetConnectionString());
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Log.Error("Fail to Open DB Connection " + ex.Message);
            }
            return null;
        }

        public static List<T>? QueryFromDB<T>(string sqlCommand, ParseDBRow<T> parser)
        {
            SQLConnection? connection = GetConnection();
            List<T> list = new();
            if (connection == null)
            {
                return null;
            }
            using (SQLCommond queryDataCmd = new SQLCommond(sqlCommand, connection))
            {
                try
                {
                    using (SQLDataReader reader = queryDataCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T? obj = parser(reader);
                            if (obj != null)
                            {
                                list.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Fail to QueryFromDB: " + ex.Message);
                }
            }
            connection.Close();
            return list;
        }

        public static void TryDbConnect()
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(GetConnectionString());
                {
                    connection.Open();
                    string query = "SELECT * FROM st_user_mng";
                    using MySqlCommand command = new MySqlCommand(query, connection);
                    {
                        using MySqlDataReader reader = command.ExecuteReader();
                        {
                            while (reader.Read())
                            {
                                // 处理结果，例如打印出来  
                                Console.WriteLine(reader[0].ToString()); // 打印第一列的值 
                                Log.Info(reader[0].ToString() + " " + reader[1].ToString() + " " + reader[2].ToString());
                                Log.Info(reader[3].ToString() + " " + reader[4].ToString());
                            }
                        }
                    }

                }

            }
            catch (MySqlException e)
            {
                Log.Error(e.ToString());
            }
            finally
            {

            }

        }
    }
}
