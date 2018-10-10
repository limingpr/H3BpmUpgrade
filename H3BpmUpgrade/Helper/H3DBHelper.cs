using OThinker.Data.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H3BpmUpgrade.Helper
{
    public class H3DBHelper
    {
        public H3DBHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        #region  获取V9系统数据
        public static string ConnectionString = ConfigurationManager.AppSettings["H3Cloud9"].ToString();
        /// <summary>
        /// 获取v9版本数据
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSql)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(strSql);
            cmd.Connection = con;
            con.Open();
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds.Tables[0]; ;
            }
        }


        #endregion

        #region 执行V10SQL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SqlText"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string SqlText)
        {
            try
            {
                CommandFactory factory = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory;
                ICommand command = factory.CreateCommand();
                return command.ExecuteNonQuery(SqlText);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return -1;
            }

        }

        /// <summary>
        ///     执行存储过程，返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static int ExecuteProcNonQuery(string spName, List<SqlParameter> parameterValues)
        {
            try
            {
                CommandFactory factory = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory;
                string connectionString = factory.ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    foreach (SqlParameter p in parameterValues)
                    {
                        if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }

                        cmd.Parameters.Add(p);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);

                throw;
            }
        }
        #endregion
    }

}
