using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.PgSql
{
    public class HashSql
    {
        public string sql;
        public NpgsqlParameter[] para;
    }
    public class PgSqlHelper
    {

        #region 执行查询语句，返回DataSet
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                DataSet ds = new DataSet();
                try
                {

                    connection.Open();
                    NpgsqlDataAdapter command = new NpgsqlDataAdapter(SQLString, connection);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }
        /// <summary>
        /// 重载 执行查询语句，返回DataSet，用于参数化查询
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, params NpgsqlParameter[] values)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(SQLString, connection);
                    cmd.Parameters.AddRange(values);
                    NpgsqlDataAdapter command = new NpgsqlDataAdapter(cmd);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataTable Query_GetDataTable(string SQLString)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    NpgsqlDataAdapter command = new NpgsqlDataAdapter(SQLString, connection);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                if (ds != null && ds.Tables != null && ds.Tables.Count != 0)
                    return ds.Tables[0];
                else
                    return null;
            }
        }
        /// <summary>
        /// 重载 执行查询语句，返回DataSet，用于参数化查询
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataTable Query_GetDataTable(string SQLString, params NpgsqlParameter[] values)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(SQLString, connection);
                    cmd.Parameters.AddRange(values);
                    NpgsqlDataAdapter command = new NpgsqlDataAdapter(cmd);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                if (ds != null && ds.Tables != null && ds.Tables.Count != 0)
                    return ds.Tables[0];
                else
                    return null;
            }
        }
        #endregion
        #region 执行简单的增删查该
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 重载用于带参数查询 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params NpgsqlParameter[] values)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(SQLString, connection))
                {
                    try
                    {
                        cmd.Parameters.AddRange(values);
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion
        #region 执行Command.ExecuteNonQuery(),返回受影响的行数
        /// <summary>
        /// 执行Command.ExecuteNonQuery(),返回受影响的行数
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数</param>
        /// <returns></returns>         
        public static int ExecuteNonQuery(string cmdText, params NpgsqlParameter[] parameters)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                int result = -1;
                conn.Open();
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    if (parameters == null)
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.Parameters.AddRange(parameters);
                        result = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    return result;
                }
            }
        }
        #endregion
        #region 执行Command.ExecuteScalar(),返回首行首列
        /// <summary>
        /// 执行Command.ExecuteScalar(),返回首行首列
        /// </summary>
        /// <param name="cmdText">执行的语句</param>
        /// <param name="parameters">params传入的参数</param>
        /// <returns></returns>    
        public static object ExecuteScalar(string cmdText, params NpgsqlParameter[] parameters)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                object obj = null;
                conn.Open();
                using (NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    if (parameters == null)
                    {
                        obj = cmd.ExecuteScalar();
                    }
                    else
                    {
                        cmd.Parameters.AddRange(parameters);
                        obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                    return obj;
                }
            }
        }
        #endregion
        #region 事务

        public int SqlList(List<HashSql> SQLStringList)
        {
            int result = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(PgConfig.ConnectionStr))
            {
                conn.Open();
                using (NpgsqlTransaction trans = conn.BeginTransaction())
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    try
                    {
                        //循环
                        foreach (HashSql myDE in SQLStringList)
                        {
                            string cmdText = myDE.sql;
                            NpgsqlParameter[] cmdParms = myDE.para;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        result = 1;
                    }
                    catch
                    {
                        trans.Rollback();
                        result = 0;
                    }
                }
            }
            return result;
        }
        #endregion
        private static void PrepareCommand(NpgsqlCommand cmd, NpgsqlConnection conn, NpgsqlTransaction trans, string cmdText, NpgsqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (NpgsqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }





}
