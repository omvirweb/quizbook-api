using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public class sqlhelperAlt
{
    public SqlConnection Constr { get; set; }
    public sqlhelperAlt()
    {
        Constr = new SqlConnection(ConfigurationManager.AppSettings["cnStrAnkAlt"].ToString());
    }

    public int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] Parameters)
    {
        int res = 0;
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.Connection = Constr;
            foreach (SqlParameter p in Parameters)
            {
                cmd.Parameters.Add(p);
            }
            Constr.Open();
            res = cmd.ExecuteNonQuery();
            // Call the overload that takes a connection in place of the connection string
            //res = ExecuteNonQuery(con, commandType, commandText, commandParameters);
            return res;
        }

        catch
        {
            return res;
        }
        finally
        {
            Constr.Close();
        }
    }
    public int ExecuteNonQuery(CommandType commandType, string commandText)
    {
        int res = 0;
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.Connection = Constr;
            Constr.Open();
            res = cmd.ExecuteNonQuery();
            return res;
        }
        catch
        {
            return res;
        }
        finally
        {
            Constr.Close();

        }
    }

    public DataSet GetDataTable(CommandType type, string commamdText)
    {
        DataSet dt = new DataSet();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Constr;
            cmd.CommandType = type;
            cmd.CommandText = commamdText;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
        catch
        {
            return dt;
        }
    }
    public DataSet GetDataTable(CommandType type, string commamdText, params SqlParameter[] commandParameters)
    {
        DataSet dt = new DataSet();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = Constr;
            cmd.CommandType = type;
            cmd.CommandText = commamdText;
            foreach (SqlParameter p in commandParameters)
            {
                cmd.Parameters.Add(p);
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }
        catch
        {
            return dt;
        }
    }

    public DataTable GetDataTable(string commamdText)
    {
        DataTable dt = new DataTable();
        try
        {

            SqlDataAdapter da = new SqlDataAdapter(commamdText, Constr);
            da.Fill(dt);
            return dt;
        }
        catch
        {
            return dt;
        }
    }
}
