using System;
using System.Data;

using System.Configuration;
using System.Collections;
using MySql.Data.MySqlClient;

//namespace GeoTrack.Service
//{
/// <summary>
/// 面向数据库的操作。
/// </summary>
public class Permanence
{
    private static string constr = System.Configuration.ConfigurationSettings.AppSettings["trackdb"];
    public static string setconstr(string dbname)
    {
        constr = System.Configuration.ConfigurationSettings.AppSettings[dbname];
        return constr;
    }
    public static int getOutday()
    {
        object o = Permanence.getDBVariant("select day from s_outday limit 0,1");
        if (o != null) return int.Parse(o.ToString());
        return 1;
    }

    /// <summary>
    /// 获得数据库连接对象
    /// </summary>
    /// <returns>MySqlConnection实例</returns>

    public static MySqlConnection getConnection()
    {
        MySqlConnection con = null;
        try
        {
            con = new MySqlConnection(constr);
            return con;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) { con.Close(); con.Dispose(); }
            throw new Exception(err.Message);
        }
    }

    /// <summary>
    /// 增加指定表的一个新记录
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="fields">记录的字段值</param>
    /// <param name="values">记录的值</param>
    /// <returns>bool标识操作是否成功</returns>
    public static bool sqlCreate(string tablename, string fields, string values)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "insert into  " + tablename + " ( " + fields + " ) values ( " + values + " )";
            MySqlCommand com = new MySqlCommand(cmdstr, con);
            //com.Parameters.
            con.Open();
            com.ExecuteNonQuery();
            com.Dispose();
            com = null;
            con.Close();
            con.Dispose();
            con = null;
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) { con.Close(); con.Dispose(); }
            throw new Exception(err.Message);
        }

    }
    /// <summary>
    /// 删除表中符合条件的记录
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="condition">指定的条件</param>
    /// <returns>bool标识操作是否成功</returns>
    public static bool sqlDel(string tablename, string condition)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "delete from  " + tablename + " where " + condition;
            MySqlCommand com = new MySqlCommand(cmdstr, con);
            con.Open();
            com.ExecuteNonQuery();
            com.Dispose();
            com = null;
            con.Close();
            con.Dispose();
            con = null;
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }



    /// <summary>
    /// 编辑表中符合条件的记录
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="values">编辑的值</param>
    /// <param name="condition">指定的条件</param>
    /// <returns>bool标识操作是否成功</returns>
    public static bool sqlMod(string tablename, string values, string condition)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "update  " + tablename + " set " + values + " where " + condition;
            if (condition == "") cmdstr = "update  " + tablename + " set " + values;
            MySqlCommand com = new MySqlCommand(cmdstr, con);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }

    }
    public static DataTable getDataTable(string sqlstr)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();

            MySqlDataAdapter da = new MySqlDataAdapter(sqlstr, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }

    /// <summary>
    /// 数据表查询，返回DataTable
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="fields">指定字段</param>
    /// <param name="condition">指定条件</param>
    /// <returns>DataTable</returns>
    public static DataTable getDataTable(string tablename, string fields, string condition)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "";
            if (condition != "" && condition != null)
                cmdstr = "select " + fields + " from " + tablename + " where " + condition;
            else
                cmdstr = "select " + fields + " from " + tablename;
            MySqlDataAdapter da = new MySqlDataAdapter(cmdstr, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }

    public static DataTable getDataTable_Ex(string sql, string condition)
    {
        MySqlConnection con = null;
        //  double reqps = pagesize;
        string cmd = "";
        string SQL = sql + condition;//+ " limit " + Convert.ToString(currentpage * pagesize) + "," + Convert.ToString(pagesize);

        try
        {
            con = getConnection();
            DataSet ds = new DataSet();
            MySqlDataAdapter sa = new MySqlDataAdapter(SQL, con);
            sa.Fill(ds);
            return ds.Tables[0];

        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }
    }
    /// <summary>
    ///重载数据表查询，返回DataTable
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="fields">指定字段</param>
    /// <param name="condition">指定条件</param>
    /// <param name="sort">排序字段</param>
    /// <returns>DataTable</returns>
    public static DataTable getDataTable(string tablename, string fields, string condition, string sort)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "";
            if (condition != "" && condition != null)
                cmdstr = "select " + fields + " from " + tablename + " where " + condition + " order by " + sort;
            else
                cmdstr = "select " + fields + " from " + tablename + " order by " + sort; ;
            MySqlDataAdapter da = new MySqlDataAdapter(cmdstr, con);

            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }
    public static DataTable getDataTable(string table, string fields, string condition, string limit, string indexstart, string order)
    {
        string rv = "";
        DataTable dt = null; ;
        string sqlstr = " select * from " + table;
        try
        {
            if (fields != null && fields != "")
            {
                sqlstr = " select " + fields + " from " + table;
            }
            if (condition != null && condition != "")
            {
                sqlstr += " where " + condition;

            }
            if (order != null && order != "")
            {
                sqlstr += " order by " + order;
            }
            if (limit != null && limit != "" && indexstart != null && indexstart != "")
            {
                sqlstr += " limit " + indexstart + "," + limit;
            }
            dt = getDataTable(sqlstr);
            System.Diagnostics.Debug.WriteLine("sql:" + sqlstr);

        }
        catch (Exception err)
        { }
        return dt;
    }

    /// <summary>
    /// 数据表查询，获取一个记录行的集合
    /// </summary>
    /// <param name="tablename">表名</param>
    /// <param name="field">字段名</param>
    /// <param name="condition"></param>
    /// <returns>包含一行数据的ArrayList集合</returns>
    public static ArrayList getCVCollect(string tablename, string field, string condition)
    {
        MySqlConnection con = null; ArrayList al = new ArrayList();
        try
        {
            con = getConnection();
            string cmdstr;
            if (condition == "" || condition == null)
                cmdstr = "select " + field + " from " + tablename;
            else
                cmdstr = "select " + field + " from " + tablename + " where " + condition;
            MySqlCommand com = new MySqlCommand(cmdstr, con);
            con.Open();
            MySqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0)) al.Add(reader.GetValue(0));
                //else al.Add(null);
            }
            con.Close();
            return al;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }

    }

    public static bool ExecSQL(string[] al)
    {
        bool exestatus = false;
        MySqlConnection con = getConnection();
        con.Open();
        MySqlTransaction tran = con.BeginTransaction();
        try
        {
            MySqlCommand com = new MySqlCommand();
            com.Connection = con;
            com.Transaction = tran;
            for (int i = 0; i < al.Length; i++)
            {
                com.CommandText = al[i];
                com.ExecuteNonQuery();
            }
            tran.Commit();
            exestatus = true;
        }
        catch
        {
            tran.Rollback();
            exestatus = false;
        }
        finally
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close();
            }
        }

        return exestatus;
    }

    public static bool ExecSQL(string sqlstr)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand(sqlstr, con);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }

        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            //throw new Exception(err.Message);
            return false;
        }

    }

    public static int ExecSQL_ex(string sqlstr)
    {
        MySqlConnection con = null;
        int idx = -1;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand(sqlstr, con);
            con.Open();
            com.ExecuteNonQuery();

            com.CommandText = "SELECT LAST_INSERT_ID()";

            object o = com.ExecuteScalar();

            idx = int.Parse(o.ToString());
            con.Close();
            return idx;
        }

        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            //throw new Exception(err.Message);
            return idx;
        }

    }
    /// <summary>
    /// 获得一个数据库记录值。
    /// </summary>
    /// <param name="sqlstr">要执行的数据语句</param>
    /// <returns>正常返回一个值/返回一个空值System.DBNUll/不存在记录null</returns>
    public static object getDBVariant(string sqlstr)
    {
        MySqlConnection con = null; object rv = null;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand(sqlstr, con);
            con.Open();
            MySqlDataReader reader = com.ExecuteReader();
            bool brv = reader.Read();
            if (brv == false) { con.Close(); return null; }
            if (reader.IsDBNull(0)) { con.Close(); return rv; }
            else rv = reader.GetValue(0);
            con.Close();
            return rv;
        }

        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }

    public static object getDBVariant(string sqlstr, ArrayList al)
    {
        MySqlConnection con = null; object rv = null;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand(sqlstr, con);
            for (int i = 0; i < al.Count; i++)
            {
                com.Parameters.Add(al[i]);
            }
            con.Open();
            MySqlDataReader reader = com.ExecuteReader();
            bool brv = reader.Read();
            if (brv == false) { con.Close(); return null; }
            if (reader.IsDBNull(0)) return rv;
            else rv = reader.GetValue(0);
            con.Close();
            return rv;
        }

        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }
    }

    /// <summary>
    /// 判断有没有一个数据库值是否存在
    /// </summary>
    /// <param name="sqlstr">要执行的数据语句</param>
    /// <returns>标识是否存在</returns>
    public static bool hasDBValue(string sqlstr)
    {
        MySqlConnection con = null; bool rv;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand(sqlstr, con);
            con.Open();
            MySqlDataReader reader = com.ExecuteReader();
            rv = reader.Read();
            if (rv == false) rv = false;
            else if (reader.IsDBNull(0)) rv = false;
            else rv = true;
            com.Dispose();
            con.Close();
            con.Dispose();
            return rv;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
                con.Dispose();
            }
            throw new Exception(err.Message);
        }
    }

    /// <summary>
    ///从数据库获取符合条件的记录的个数。
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="condition">指定的条件</param>
    /// <returns>记录的个数Double</returns>
    public static double GetPages(string tablename, string condition)
    {
        MySqlConnection con = null;
        string cmd = "select count(*) from " + tablename;
        double totalnum = 0;
        try
        {
            if (condition != null && condition.Trim().Length != 0)
            {
                cmd += " where ";
                cmd += condition;
            }
            con = getConnection();
            MySqlCommand com = new MySqlCommand(cmd, con);

            string constr = con.State.ToString();

            con.Open();
            totalnum = double.Parse(com.ExecuteScalar().ToString());
            con.Close();
            return (totalnum);

        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }
    }

    public static double GetPages(string sqlstr)
    {
        MySqlConnection con = null;
        string cmd = sqlstr;
        double totalnum = 0;
        try
        {

            con = getConnection();
            MySqlCommand com = new MySqlCommand(cmd, con);

            string constr = con.State.ToString();

            con.Open();
            totalnum = double.Parse(com.ExecuteScalar().ToString());
            con.Close();
            return (totalnum);

        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }
    }

    /// <summary>
    /// 支持分页形式数据库表查询，不用自增长字段作为分页条件
    /// </summary>
    /// <param name="tablename">数据表名称</param>
    /// <param name="flgfield">表的标示列(id)</param>
    /// <param name="condition">查询的条件</param>
    /// <param name="fields">要返回的字段</param>
    /// <param name="orderstr">排序字段</param>
    /// <param name="currentpage"></param>
    /// <param name="pagesize"></param>
    /// <returns></returns>
    public static System.Data.DataTable GetSplitPageData(string tablename, string flgfield, string condition, string fields, string orderstr, double currentpage, double pagesize)
    {
        MySqlConnection con = null;
        string cmd = "";
        cmd = "SELECT TOP " + pagesize + " " + fields + " FROM " + tablename;
        cmd += " where " + flgfield + " not in (select top " + ((currentpage - 1) * pagesize) + " " + flgfield + " from " + tablename;
        if (condition != "")
            cmd += " where " + condition;
        if (orderstr != "")
            cmd += " order by " + orderstr + " )";
        else
            cmd += " ) ";
        if (condition != "")
            cmd += " and  " + condition;
        if (orderstr != "")
            cmd += " order by " + orderstr;
        try
        {
            con = getConnection();
            DataSet ds = new DataSet();
            MySqlDataAdapter sa = new MySqlDataAdapter(cmd, con);
            sa.Fill(ds);
            return ds.Tables[0];
        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }

    }
    /// <summary>
    /// 获取分页的数据,数据表里必须有排序的id值.请求的当前页从1开始.
    /// </summary>
    /// <param name="tablename"></param>
    /// <param name="fields"></param>
    /// <param name="condition"></param>
    /// <param name="currentpage"></param>
    /// <param name="pagesize"></param>
    /// <returns></returns>
    public static System.Data.DataTable GetSplitPageData(string tablename, string fields, string condition, double currentpage, double pagesize)
    {
        MySqlConnection con = null;
        string cmd = "";
        if (currentpage == 1)
        {
            cmd = "SELECT TOP " + pagesize + " " + fields;
            cmd += " FROM " + tablename;
            if (condition != null && condition.Length != 0)
            {
                cmd += " WHERE  " + condition;
            }

            cmd += " ORDER BY ID";
        }
        else
        {
            cmd = "SELECT TOP " + pagesize + " " + fields + " FROM " + tablename + " where ";
            if (condition != null && condition.Length != 0)
            {
                cmd += condition + " and ";
            }

            cmd += " ID >" +
                " (SELECT MAX(id) FROM (SELECT TOP " + pagesize * (currentpage - 1) + " id FROM " + tablename;
            if (condition != null && condition.Length != 0)
            {
                cmd += " WHERE " + condition;
            }
            cmd += " ORDER BY id) AS T) ORDER BY ID";

        }

        try
        {

            con = getConnection();
            DataSet ds = new DataSet();
            MySqlDataAdapter sa = new MySqlDataAdapter(cmd, con);
            sa.Fill(ds);
            return ds.Tables[0];

        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tablename"></param>
    /// <param name="fields"></param>
    /// <param name="condition"></param>
    /// <param name="currentpage"></param>
    /// <param name="pagesize"></param>
    /// <returns></returns>

    public static System.Data.DataTable GetSplitPageData_Ext(string sql, string condition, double currentpage, double pagesize)
    {
        MySqlConnection con = null;
        double reqps = pagesize;
        string cmd = "";
        string SQL = sql + condition + " limit " + Convert.ToString(currentpage * pagesize) + "," + Convert.ToString(pagesize);

        try
        {
            con = getConnection();
            DataSet ds = new DataSet();
            MySqlDataAdapter sa = new MySqlDataAdapter(SQL, con);
            sa.Fill(ds);
            return ds.Tables[0];

        }
        catch (Exception e)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(e.Message);

        }

    }

    public static string TreatSortStr(string param)
    {
        string rv = "";
        if (param.IndexOf(" desc") != -1)
        {
            rv = param.Replace(" desc", " asc");
        }
        else if (param.IndexOf(" asc") != -1)
        {
            rv = param.Replace(" asc", " desc");
        }
        else
        {
            rv = param + " desc";
        }
        return rv;
    }

    public static DataTable ConvertData(DataTable _dt)
    {
        DataSet ds = _dt.DataSet.Clone();
        ds.Tables.Clear();
        DataTable dt = new DataTable();
        //dt.DataSet = _dt.DataSet.Clone();
        dt = _dt.Clone();
        //dt.TableName = "clonetable";  
        ds.Tables.Add(dt);
        for (int i = _dt.Rows.Count - 1; i >= 0; i--)
        {
            dt.Rows.Add(_dt.Rows[i].ItemArray);
        }
        return dt;
    }



    /// <summary>
    /// 重载增加指定表的一个新记录,传递MySqlParameter
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="parms">MySqlParameter参数的</param>
    /// <returns>bool标识操作是否成功</returns>
    //public static bool sqlCreate(string tablename, ArrayList prams)
    //{
    //    MySqlConnection con=null;
    //    try
    //    {
    //        con=getConnection();
    //        MySqlCommand com=new MySqlCommand();
    //        com.Connection=con;
    //        string values="";
    //        string fields="";
    //        char[] sp=new char[]{'@',','};
    //        foreach (MySqlParameter parameter in prams)
    //        {
    //            values+=parameter.ParameterName+",";
    //            fields+=parameter.ParameterName.TrimStart(sp)+",";
    //            com.Parameters.Add(parameter);
    //        }
    //        com.CommandText="insert into  "+tablename+" ( "+fields.TrimEnd(sp)+" ) values ( "+values.TrimEnd(sp)+" )";
    //        con.Open();
    //        com.ExecuteNonQuery();
    //        con.Close();
    //        con.Dispose();
    //        return true;
    //    }
    //    catch(Exception err)
    //    {
    //        if(con.State==ConnectionState.Open){con.Close();con.Dispose();}
    //        return false;
    //    }

    //}
    /// <summary>
    /// 重载增加指定表的一个新记录,传递MySqlParameter
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="parms">MySqlParameter参数的</param>
    /// <returns>bool标识操作是否成功</returns>
    public static int sqlCreate(string sql, string names, ArrayList prams)
    {
        MySqlConnection con = null;
        int idx = -1;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand();
            com.Connection = con;
            MySqlParameter msp = new MySqlParameter();
            string[] name = names.Split(',');
            for (int i = 0; i < prams.Count; i++)
            {

                com.Parameters.Add(name[i], (object)prams[i]);
            }

            com.CommandText = sql;
            con.Open();
            com.ExecuteNonQuery();

            com.CommandText = "SELECT LAST_INSERT_ID()";

            object o = com.ExecuteScalar();

            idx = int.Parse(o.ToString());

            con.Close();
            con.Dispose();
            return idx;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) { con.Close(); con.Dispose(); }
            return idx;
        }

    }
    public static int sqlCreate(string sql)
    {
        MySqlConnection con = null;
        int idx = -1;
        try
        {
            con = getConnection();
            MySqlCommand com = new MySqlCommand();
            com.Connection = con;

            com.CommandText = sql;
            con.Open();
            com.ExecuteNonQuery();

            com.CommandText = "SELECT LAST_INSERT_ID()";

            object o = com.ExecuteScalar();

            idx = int.Parse(o.ToString());

            con.Close();
            con.Dispose();
            return idx;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) { con.Close(); con.Dispose(); }
            return idx;
        }

    }
    /// <summary>
    /// 重载编辑表中符合条件的记录
    /// </summary>
    /// <param name="tablename">数据库表名</param>
    /// <param name="values">编辑的值</param>
    /// <param name="condition">指定的条件</param>
    /// <param name="prams">MySqlParameter集合</param>
    /// <returns>bool标识操作是否成功</returns>
    public static bool sqlMod(string tablename, string values, string condition, string names, ArrayList prams)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string cmdstr = "update  " + tablename + " set " + values;
            if (condition != "") cmdstr += " where " + condition;
            MySqlCommand com = new MySqlCommand(cmdstr, con);
            string[] name = names.Split(',');
            for (int i = 0; i < prams.Count; i++)
            {
                com.Parameters.Add(new MySqlParameter(name[i], prams[i]));
            }
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }

    }

    public static bool sqlMod(string tablename, string condition, string names, ArrayList prams)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            string values = " ";

            string[] name = names.Split(',');

            for (int j = 0; j < name.Length; j++)
            {
                values += name[j] + "=@" + name[j];
                if (j < name.Length - 1)
                {
                    values += ",";
                }
            }

            string cmdstr = "update  " + tablename + " set " + values;

            if (condition != "") cmdstr += " where " + condition;
            MySqlCommand com = new MySqlCommand(cmdstr, con); ;
            for (int i = 0; i < prams.Count; i++)
            {
                com.Parameters.Add(new MySqlParameter(name[i], prams[i]));
            }
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            throw new Exception(err.Message);
        }

    }

    public static bool sqlMod(string tablename, string condition, ArrayList prams)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            System.Text.StringBuilder values = new System.Text.StringBuilder();
            MySqlCommand com = new MySqlCommand();

            foreach (MySqlParameter parameter in prams)
            {
                values.Append(parameter.ParameterName.TrimStart(new char[] { '@' }) + "=" + parameter.ParameterName + ",");
                com.Parameters.Add(parameter);
            }

            string cmdstr = "update  " + tablename + " set " + values.ToString().TrimEnd(new char[] { ',' });
            if (condition != "") cmdstr += " where " + condition;
            com.CommandText = cmdstr;
            com.Connection = con;
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            return false;
            //throw new Exception(err.Message);
        }

    }
    public static bool sqlMod_Ext(string tablename, string condition, ArrayList prams, Hashtable kv)
    {
        MySqlConnection con = null;
        try
        {
            con = getConnection();
            System.Text.StringBuilder values = new System.Text.StringBuilder();
            MySqlCommand com = new MySqlCommand();

            foreach (MySqlParameter parameter in prams)
            {
                if (kv.ContainsKey(parameter.ParameterName))
                {
                    values.Append(parameter.ParameterName.TrimStart(new char[] { '@' }) + "=" + kv[parameter.ParameterName].ToString() + ",");
                }
                else
                {
                    values.Append(parameter.ParameterName.TrimStart(new char[] { '@' }) + "=" + parameter.ParameterName + ",");
                }
                com.Parameters.Add(parameter);
            }

            string cmdstr = "update  " + tablename + " set " + values.ToString().TrimEnd(new char[] { ',' });
            if (condition != "") cmdstr += " where " + condition;
            com.CommandText = cmdstr;
            com.Connection = con;
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception err)
        {
            if (con.State == ConnectionState.Open) con.Close();
            return false;
            //throw new Exception(err.Message);
        }

    }


    public static string GetXmlStr(DataTable dt, string nodename)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        sb.Append("<" + nodename + "s>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<" + nodename + ">");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.Append("<" + dt.Columns[j].ColumnName + ">");
                if (dt.Rows[i].IsNull(j)) sb.Append("");
                else sb.Append(dt.Rows[i][j].ToString().Replace("&", "&amp; "));
                sb.Append("</" + dt.Columns[j].ColumnName + ">");
            }
            sb.Append("</" + nodename + ">");
        }
        sb.Append("</" + nodename + "s>");
        return sb.ToString();
    }
    public static string GetXmlStrCov(DataTable dt, string nodename)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        sb.Append("<" + nodename + "s>");
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            sb.Append("<" + nodename + ">");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.Append("<" + dt.Columns[j].ColumnName + ">");
                if (dt.Rows[i].IsNull(j)) sb.Append("");
                else sb.Append(dt.Rows[i][j].ToString().Replace("&", "&amp; "));
                sb.Append("</" + dt.Columns[j].ColumnName + ">");
            }
            sb.Append("</" + nodename + ">");
        }
        sb.Append("</" + nodename + "s>");
        return sb.ToString();
    }
    public static string GetXmlStr(DataTable dt, string nodename, string no)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        sb.Append("<" + nodename + "s>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<" + nodename + ">");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.Append("<" + dt.Columns[j].ColumnName + ">");
                if (dt.Rows[i].IsNull(j)) sb.Append("");
                else sb.Append("<![CDATA[" + dt.Rows[i][j].ToString() + "]]>");
                sb.Append("</" + dt.Columns[j].ColumnName + ">");
            }
            sb.Append("</" + nodename + ">");
        }
        sb.Append("</" + nodename + "s>");
        return sb.ToString();
    }

    public static string getProvFromAd(string adcode)
    {
        if (adcode == null || adcode == "") return "";
        if (adcode == "000000") return "国家节点";
        string constr = System.Configuration.ConfigurationSettings.AppSettings["sqlservercon"];
        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(constr);
        object prov = null;
        try
        {
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select name from t_xzqh where adcode='" + adcode + "'", con);

            con.Open();
            prov = cmd.ExecuteScalar();
            con.Close();
        }
        catch
        {
            if (con.State == ConnectionState.Open) con.Close();

        }
        if (prov != null) return prov.ToString();
        else return "";
    }
    public static string getWsFromid(string id)
    {

        try
        {
            object o = Permanence.getDBVariant("select wsdomain from ws_icp where id=" + id);
            return o.ToString();
        }
        catch (Exception err)
        {
            return "";
        }
    }

    public static string getTreatSheetId(string adcode)
    {

        object tid = Permanence.getDBVariant("select tid from t_treat_task where adcode='" + adcode + "' order by rectime desc  limit 0,1");
        string jx = "国";

        if (tid == null)
        {
            if (adcode != "000000")
            {
                jx = Permanence.getDBVariant("select alias from xzqh where left(adcode,2)='" + adcode.Substring(0, 2) + "'").ToString();
                jx = jx.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            return jx + DateTime.Now.ToString("yyyy") + "001";
        }
        else
        {
            if (adcode != "000000")
            {
                jx = Permanence.getDBVariant("select alias from xzqh where left(adcode,2)='" + adcode.Substring(0, 2) + "'").ToString();
                jx = jx.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            string year = tid.ToString().Substring(1, 4);
            string cd = tid.ToString().Substring(5);
            if (year != DateTime.Now.ToString("yyyy"))
            {
                return jx + DateTime.Now.ToString("yyyy") + "001";
            }
            else
            {
                //string cd = tid.ToString();
                //year.PadLeft(4,'0'）;
                int icd = int.Parse(cd) + 1;

                cd = icd.ToString();
                cd = cd.PadLeft(3, '0');
                return jx + DateTime.Now.ToString("yyyy") + cd; ;
            }
        }



    }

    //获取 ws_icp 信息
    public static DataTable getDticp(string domain_id)
    {
        DataTable dt = Permanence.getDataTable("select * from ws_icp where id=" + domain_id);
        return dt;
    }
    //获取domain_id对应的所属地区信息
    public static string getAdFrmWsicp(string id)
    {
        object o = Permanence.getDBVariant("select icp_ad from ws_icp where id= " + id);
        if (o != null) return o.ToString();
        return "";

    }
    //处置 研判后，自动在t_handle_ws表里注册 处置分配信息
    public static void regist_handle_ws(string domain_id)
    {
        string handle_ad = "";
        if (!Permanence.hasDBValue("select id from t_handle_ws where domain_id=" + domain_id))
        {
            handle_ad = getAdFrmWsicp(domain_id);
            Permanence.ExecSQL("insert into t_handle_ws (domain_id,handle_ad,crt_date) values (" + domain_id + ",'" + handle_ad + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
        }
    }

    //待处置表，adcode，domain_id,handle_ad,c_type,relid,

    //检定内容，记录到待处置表。并自动注册
    public static void rec_wait_treat(string id, string c_type, string domain_id, string adcode, string snapid)
    {
        string handle_ad = "";
        string sqlstr = "";
        DataTable dt = Permanence.getDataTable("select id, domain_id,handle_ad from t_handle_ws where domain_id=" + domain_id);
        if (dt.Rows.Count > 0)
        {
            handle_ad = dt.Rows[0]["handle_ad"].ToString();
            if (handle_ad == "")
            {
                handle_ad = getAdFrmWsicp(domain_id);
                if (handle_ad != "")
                {
                    sqlstr = "update t_handle_ws set handle_ad='" + handle_ad + "' where id=" + dt.Rows[0]["id"].ToString();
                    Permanence.ExecSQL(sqlstr);

                    sqlstr = "update t_wait_treat set handle_ad='" + handle_ad + "' where domain_id=" + domain_id;
                    Permanence.ExecSQL(sqlstr);

                }
            }
        }
        else
        {
            handle_ad = getAdFrmWsicp(domain_id);
            sqlstr = "insert into t_handle_ws (domain_id,handle_ad,crt_date) values(" + domain_id + ",'" + handle_ad + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            Permanence.ExecSQL(sqlstr);
        }
        sqlstr = "insert into t_wait_treat (c_type,relid,domain_id,handle_ad,rectime,adcode,snapid) values(" + c_type + "," + id + "," + domain_id + ",'" + handle_ad + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + adcode + "'," + snapid + ")";

        Permanence.ExecSQL(sqlstr);



    }
    //重置，修改后，操作待处置表
    public static void rm_wait_treat(string id, string c_type)
    {
        string sqlstr = "delete from t_wait_treat where relid=" + id + " and c_type=" + c_type;
        Permanence.ExecSQL(sqlstr);

    }
    

    //向user_operate插入操作用户
    public static bool AddUser(int rid, int ctype, int status, string userid, string time, string reason)
    {
        string userinfo = string.Format("insert into user_operate(rid,ctype,status,userid,time,reason) values({0},{1},{2},'{3}','{4}','{5}')", rid,
                ctype, status, userid, time, reason);
        bool flag = Permanence.ExecSQL(userinfo);
        return flag;
    }
    //向user_operate插入操作用户
    public static bool AddUser(int rid, int ctype, int status, string userid, string time, string reason, string adcode)
    {
        string userinfo = string.Format("insert into user_operate(rid,ctype,status,userid,time,reason,adcode) values({0},{1},{2},'{3}','{4}','{5}','{6}')", rid,
                ctype, status, userid, time, reason, adcode);
        bool flag = Permanence.ExecSQL(userinfo);
        return flag;
    }

    public static void delsnapcache(string ctype, string recid)
    {
        string path = "";// AppDomain.CurrentDomain.BaseDirectory + "piccache\\basicimg\\" ;

        string sqlstr = "SELECT distinct cguid FROM trackdb2014.t_snap_basic_img where ctype=" + ctype + " and relid=" + recid;

        DataTable dt = Permanence.getDataTable(sqlstr);
        string fn = "";
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            fn = dt.Rows[i][0].ToString();

            path = AppDomain.CurrentDomain.BaseDirectory + "piccache\\basicimg\\" + fn + ".png";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        sqlstr = "SELECT distinct cguid FROM t_snap_cache where ctype=" + ctype + " and relid=" + recid;

        dt = Permanence.getDataTable(sqlstr);
        fn = "";
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            fn = dt.Rows[i][0].ToString();

            path = AppDomain.CurrentDomain.BaseDirectory + "piccache\\" + fn + ".png";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }

    //重置操作删除相关的取证内容
    public static void delsnap(string ctype, string recid)
    {
        delsnapcache(ctype, recid);
        string sql = "delete from t_snap_markers where ctype=" + ctype + " and relid=" + recid;
        Permanence.ExecSQL(sql);

        sql = "delete from t_snapshot where c_type=" + ctype + " and relid=" + recid;
        Permanence.ExecSQL(sql);

        sql = "delete from t_snap_basic_img where ctype=" + ctype + " and relid=" + recid;
        Permanence.ExecSQL(sql);
        sql = "delete from t_snap_cache where ctype=" + ctype + " and relid=" + recid;
        Permanence.ExecSQL(sql);
    }

}
