using H3BpmUpgrade.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H3BpmUpgrade.Business
{
    public static class DataBusiness
    {

        #region 获取数据库表结构

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Version"></param>
        /// <returns></returns>
        public static List<Field> GetTableSchema(string TableName, string Version = "V10")
        {
            var dtColumns = new DataTable();
            List<Field> Fields = new List<Field>();
            var sqlColumns = string.Format(@"SELECT
	*
FROM INFORMATION_SCHEMA.COLUMNS t
WHERE t.TABLE_NAME = '{0}'
ORDER BY t.ORDINAL_POSITION", TableName);
            switch (Version)
            {
                case "V9":
                    dtColumns = H3DBHelper.GetDataTable(sqlColumns);
                    break;
                default:
                    dtColumns = OThinker.H3.Controllers.AppUtility.Engine.Query.QueryTable(sqlColumns);
                    break;
            }
            foreach (DataRow ItemCol in dtColumns.Rows)
            {
                Field field = new Field();
                field.Name = ItemCol["column_name"].ToString();
                field.Type = ItemCol["data_type"].ToString();
                if (ItemCol["DATA_TYPE"].ToString().ToLower() == "char" || ItemCol["DATA_TYPE"].ToString().ToLower() == "nvarchar")
                {
                    field.Length = ItemCol["CHARACTER_MAXIMUM_LENGTH"].ToString();

                }
                Fields.Add(field);

            }

            return Fields;
        }

        /// <summary>
        /// 获取在V9和V10版本中表结构中相同的列
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetTableColumns(string TableName)
        {
            var ProCols = new List<string>();
            var Schema10 = GetTableSchema(TableName);
            var Schema9 = GetTableSchema(TableName, "V9");
            foreach (var filed in Schema10)
            {
                var tt = Schema9.Where(a => a.Name == filed.Name);
                if (Schema9.Contains(filed))
                {
                    ProCols.Add("[" + filed.Name + "]");

                }


            }
            return string.Join(",\n", ProCols.ToArray());
        }

        /// <summary>
        /// 获取V10和V9对应表相同列
        /// </summary>
        /// <param name="TableName">数组：Table10，Table9</param>
        /// <returns></returns>
        public static string GetTableColumns(string[] TableName)
        {
            var ProCols = new List<string>();
            var Schema10 = GetTableSchema(TableName[0]);
            var Schema9 = GetTableSchema(TableName[1], "V9");
            foreach (var filed in Schema10)
            {
                var tt = Schema9.Where(a => a.Name == filed.Name);
                if (Schema9.Contains(filed))
                {
                    ProCols.Add("[" + filed.Name + "]");

                }


            }
            return string.Join(",\n", ProCols.ToArray());
        }

        #endregion

        #region 创建储存过程
        /// <summary>
        /// 创建储存过程
        /// </summary>
        /// <param name="temp"></param>
        public static void CreateProc(Temp temp)
        {
            var DropPeoc = string.Format(@"DROP PROCEDURE {0}", temp.ProcName);
            H3DBHelper.ExecuteNonQuery(DropPeoc);
            if (temp.ProCols.Count > 0)
            {
                //创建存储过程
                var CreateProc = string.Format(@"CREATE PROCEDURE {0}
(
    @TempTable {1} Readonly 
)
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRANSACTION
DELETE FROM {2};
INSERT INTO {2}
           ({3})
        SELECT   
           {3}
              FROM @TempTable
    COMMIT TRANSACTION           
END"
, temp.ProcName
, temp.TypeName
, temp.TableName
, string.Join(",\n", temp.ProCols.ToArray()));
                H3DBHelper.ExecuteNonQuery(CreateProc);

            }

        }

        /// <summary>
        /// 创建数据类型
        /// </summary>
        /// <param name="temp"></param>
        public static void CreateType(Temp temp)
        {

            var DropType = string.Format(@"DROP TYPE {0}", temp.TypeName);

            H3DBHelper.ExecuteNonQuery(DropType);


            var createtype = string.Format(@"CREATE TYPE {0} AS TABLE({1})"
, temp.TypeName
, string.Join(",\n", temp.TypeCols.ToArray()));

            H3DBHelper.ExecuteNonQuery(createtype);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="temp"></param>
        public static void CreateTable(Temp temp)
        {
            //在10版本中创建自定义Table
            var DropTable = string.Format(@"DROP Table {0}", temp.TableName);
            H3DBHelper.ExecuteNonQuery(DropTable);
            var CreateTable = string.Format(@"CREATE TABLE {0}
(
{1}
)"
, temp.TableName
, string.Join(",\n", temp.TypeCols.ToArray()));

            H3DBHelper.ExecuteNonQuery(CreateTable);
        }

        public static Temp GetTableCols(string TableName)
        {
            var Temp = new Temp();
            var TypeCols = new List<string>();
            var ProCols = new List<string>();
            var Schema9 = GetTableSchema(TableName, "V9");
            var Schema10 = GetTableSchema(TableName);
            if (Schema10 == null || Schema10.Count == 0)
            {
                Schema10 = Schema9;
            }
            foreach (var filed in Schema10)
            {
                if (Schema9.Contains(filed))
                {

                    ProCols.Add("[" + filed.Name + "]");
                    if (filed.Length != null)
                    {
                        if (filed.Name.ToLower() == "objectid")
                        {
                            TypeCols.Add(string.Format(@"[{0}] [{1}] ({2}) NOT NULL"
    , filed.Name
    , filed.Type
    , filed.Length));
                        }
                        else
                        {
                            TypeCols.Add(string.Format(@"[{0}] [{1}] ({2}) NULL"
    , filed.Name
    , filed.Type
    , filed.Length));
                        }

                    }
                    else
                    {
                        TypeCols.Add(string.Format(@"[{0}] [{1}]  NULL"
    , filed.Name
    , filed.Type));

                    }
                }


            }
            Temp.TableName = TableName;
            Temp.ProCols = ProCols;
            Temp.TypeCols = TypeCols;
            return Temp;
        }

        public static void SyncTable(string TableName)
        {
            try
            {
                var ProcName = "Proc_" + TableName;
                var SqlTable = GetTableSql(TableName);
                var dt = H3DBHelper.GetDataTable(SqlTable);
                var Parameters = new List<SqlParameter>();
                Parameters.Add(new SqlParameter() { ParameterName = "@TempTable", Value = dt });
                var Result = H3DBHelper.ExecuteProcNonQuery(ProcName, Parameters);
                LogHelper.Info("导入成功:" + TableName);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("导入报错:" + TableName);
                LogHelper.Error("导入报错:" + ex.Message);

            }
        }

        public static void SyncTable(string[] TableName)
        {
            try
            {
                var ProcName = "Proc_" + TableName[0];
                var SqlTable = GetTableSql(TableName);
                var dt = H3DBHelper.GetDataTable(SqlTable);
                var Parameters = new List<SqlParameter>();
                Parameters.Add(new SqlParameter() { ParameterName = "@TempTable", Value = dt });
                var Result = H3DBHelper.ExecuteProcNonQuery(ProcName, Parameters);
                LogHelper.Info("导入成功:" + TableName);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("导入报错:" + TableName);
                LogHelper.Error("导入报错:" + ex.Message);

            }
        }


        private static string GetTableSql(string TableName)
        {
            var stringCol = GetTableColumns(TableName);
            return string.Format(@"SELECT 
{0} 
FROM  
{1}"
, stringCol
, TableName);
        }

        private static string GetTableSql(string[] TableName)
        {
            var stringCol = GetTableColumns(TableName);
            return string.Format(@"SELECT 
{0} 
FROM  
{1}"
, stringCol
, TableName[1]);
        }
    }

    #endregion

}
public struct Field
{
    public string Name;
    public string Type;
    public string Length;
}

public class Temp
{
    public string TableName;
    public string TypeName { get { return "Type_" + TableName; } }
    public string ProcName { get { return "Proc_" + TableName; } }
    public List<string> TypeCols;
    public List<string> ProCols;
}



