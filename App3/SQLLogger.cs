using System;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;
using ExtraUtils;

namespace ColonySym
{
    class SQLLogger
    {
        MySql.Data.MySqlClient.MySqlConnection conn;
        private static ColourWriter printer;
        public bool busy = false;
        public SQLLogger()
        {
            string myConnectionString;
            printer = new ColourWriter();
            printer.USE_COLOURS = true;
            myConnectionString = "server=127.0.0.1;uid=root;" +
                "pwd=Zippy1972;database=ColonySym;";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                busy = true;
            }
        }

        public DataTable RunCode(string statement)
        {
            busy = true;
            MySqlDataReader reader = null;
            DataTable dataTable = new DataTable();
            string selectCmd = statement;
            MySqlCommand command = new MySqlCommand(selectCmd, conn);
            try
            {
                conn.Open();
            }
            catch (Exception er)
            {
                Form1.printer.WriteLine(er.Message, WriterColours.ERROR);
            }
            try
            {
                reader = command.ExecuteReader();
                dataTable.Load(reader);
                conn.Close();
            }
            catch (Exception ex)
            {
                Form1.printer.WriteLine(ex.Message, WriterColours.WARNING);
                conn.Close();
                busy = false;
            }
            busy = false;
            return dataTable;
        }

        public DataTable GetLogs()
        {
            busy = true;
            MySqlDataReader reader = null;
            DataTable dataTable = new DataTable();
            string selectCmd = "select min(date_cre) as start_time, max(date_cre) as end_time, run_hash, count(*) as log_count from colonysym.exe_messages group by run_hash order by start_time asc";
            MySqlCommand command = new MySqlCommand(selectCmd, conn);

            try
            {
                conn.Open();
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
            try
            {
                reader = command.ExecuteReader();
                dataTable.Load(reader);
                conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                conn.Close();
                busy = false;
            }
            busy = false;
            return dataTable;
        }
        public DataTable GetMessages(int MessageType, int hash)
        {
            busy = true;
            //Console.WriteLine("GetMessage "+MessageType+ " " + hash);
            MySqlDataReader reader = null;
            DataTable dataTable = new DataTable();
            int message_type = MessageType;
            int run_hash = hash;
            string selectCmd = "SELECT * FROM  exe_messages where run_hash = @RH";
            MySqlCommand command = new MySqlCommand(selectCmd, conn);
            
            command.Parameters.Add("@RH", MySqlDbType.Int64);
            command.Parameters["@RH"].Value = run_hash;
            // Separate open connection
            try
            {
                conn.Open();
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
                busy = false;
            }
            try
            {
                reader = command.ExecuteReader();
                dataTable.Load(reader);
                conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                conn.Close();
                busy = false;
            }
            busy = false;
            return dataTable;
        }

        public int LogException (Exception e, int hash)
        {
            busy = true;
            int rowsAffected = 0;
            string message_text = e.Message;
            int message_type = 2;
            string source = e.Source;
            string stack_trace = e.StackTrace;
            string target_site = e.TargetSite.Name;
            int run_hash = hash;

            string stm = "INSERT INTO exe_messages (message_text, message_type, source, stack_trace, target_site, run_hash) " +
                           " VALUES (@MSG, @TYP, @SRC, @ST, @TS, @RH)";
            MySqlCommand command = new MySqlCommand(stm, conn);

            command.Parameters.Add("@MSG", MySqlDbType.Text);
            command.Parameters["@MSG"].Value = message_text;

            command.Parameters.Add("@TYP", MySqlDbType.Int16);
            command.Parameters["@TYP"].Value = message_type;

            command.Parameters.Add("@SRC", MySqlDbType.VarChar);
            command.Parameters["@SRC"].Value = source;

            command.Parameters.Add("@ST", MySqlDbType.Text);
            command.Parameters["@ST"].Value = stack_trace;

            command.Parameters.Add("@TS", MySqlDbType.VarChar);
            command.Parameters["@TS"].Value = target_site;

            command.Parameters.Add("@RH", MySqlDbType.Int64);
            command.Parameters["@RH"].Value = run_hash;

            // Separate open connection
            try
            {
                conn.Open();
            } catch (Exception er)
            {
                Console.WriteLine(er.Message);
                conn.Close();
                busy = false;
            }

            try
            {
                rowsAffected = command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine(ex.Message);
                busy = false;
            }
            busy = false;
            return rowsAffected;
        }

        public int LogMessage(string message, string comment, int hash)
        {
            busy = true;
            int rowsAffected = 0;
            string message_text = message;
            int message_type = 0;
            int run_hash = hash;

            string stm = "INSERT INTO exe_messages (message_text, message_type, comment, run_hash) " +
                           " VALUES (@MSG, @TYP, @COM, @RH)";
            MySqlCommand command = new MySqlCommand(stm, conn);

            command.Parameters.Add("@MSG", MySqlDbType.Text);
            command.Parameters["@MSG"].Value = message_text;

            command.Parameters.Add("@TYP", MySqlDbType.Int16);
            command.Parameters["@TYP"].Value = message_type;

            command.Parameters.Add("@COM", MySqlDbType.VarChar);
            command.Parameters["@COM"].Value = comment;

            command.Parameters.Add("@RH", MySqlDbType.Int64);
            command.Parameters["@RH"].Value = run_hash;

            // Separate open connection
            try
            {
                conn.Open();
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                conn.Close();
                busy = false;
            }
            try
            {
                rowsAffected = command.ExecuteNonQuery();
                conn.Close();
                busy = false;
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine(ex.Message);
                busy = false;
            }
            busy = false;
            return rowsAffected;
        }

        public int LogException(Exception e, int hash, string comment)
        {
            busy = true;
            int rowsAffected = 0;
            string message_text = e.Message;
            int message_type = 1;
            string source = e.Source;
            string stack_trace = e.StackTrace;
            string target_site = e.TargetSite.Name;
            int run_hash = hash;

            string stm = "INSERT INTO exe_messages (message_text, message_type, source, stack_trace, target_site, comment, run_hash) " +
                           " VALUES (@MSG, @TYP, @SRC, @ST, @TS, @COM, @RH)";
            MySqlCommand command = new MySqlCommand(stm, conn);

            command.Parameters.Add("@MSG", MySqlDbType.Text);
            command.Parameters["@MSG"].Value = message_text;

            command.Parameters.Add("@TYP", MySqlDbType.Int16);
            command.Parameters["@TYP"].Value = message_type;

            command.Parameters.Add("@SRC", MySqlDbType.VarChar);
            command.Parameters["@SRC"].Value = source;

            command.Parameters.Add("@ST", MySqlDbType.Text);
            command.Parameters["@ST"].Value = stack_trace;

            command.Parameters.Add("@TS", MySqlDbType.VarChar);
            command.Parameters["@TS"].Value = target_site;

            command.Parameters.Add("@COM", MySqlDbType.VarChar);
            command.Parameters["@COM"].Value = comment;

            command.Parameters.Add("@RH", MySqlDbType.Int64);
            command.Parameters["@RH"].Value = run_hash;

            // Separate open connection
            try
            {
                conn.Open();
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                busy = false;
            }

            try
            {
                rowsAffected = command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine(ex.Message);
                busy = false;
            }
            busy = false;
            return rowsAffected;
        }
    }

}
