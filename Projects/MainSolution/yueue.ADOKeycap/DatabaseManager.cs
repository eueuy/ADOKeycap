using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Common;
using System.Collections;
using System.Data.Odbc;
using System.Data.OracleClient;

namespace yueue.ADOKeycap
{
    /// <summary>
    /// 用来创建Database对象的静态类
    /// </summary>
    public static class DatabaseManager
    { 
        /// <summary>
        /// 用来创建Database对象
        /// </summary>
        /// <param name="DatabaseNameInConfigFile">在配置文件中定义的数据库名称name,用来确定对哪个数据库进行操作</param>
        /// <returns>返回对应数据库的Database对象</returns>
        public static Database CreateDatabase(string DatabaseNameInConfigFile)
        {
            string pn = ConfigurationManager.ConnectionStrings[DatabaseNameInConfigFile].ProviderName; //读取配置文件
            string cs = ConfigurationManager.ConnectionStrings[DatabaseNameInConfigFile].ConnectionString;
            return MakeDBObject(cs, pn);
        }
        public static Database CreateDatabase(string ConnectionString, string ProviderName)
        {
            return MakeDBObject(ConnectionString,ProviderName);
        }


        private static Database MakeDBObject(string ConnectionString,string ProviderName)
        {
            //以下判断数据库类型并创建相应的对象
            if (ProviderName.ToUpper().Contains("OLEDB"))
            {
                OleDbDataAdapter oledbda = new OleDbDataAdapter();
                oledbda.SelectCommand = new OleDbCommand();
                oledbda.SelectCommand.Connection = new OleDbConnection(ConnectionString);
                return new Database(oledbda);
            }
            if (ProviderName.ToUpper().Contains("SQL"))
            {
                SqlDataAdapter sqlda = new SqlDataAdapter();
                sqlda.SelectCommand = new SqlCommand();
                sqlda.SelectCommand.Connection = new SqlConnection(ConnectionString);
                return new Database(sqlda);
            }
            if (ProviderName.ToUpper().Contains("ODBC"))
            {
                OdbcDataAdapter odbcda = new OdbcDataAdapter();
                odbcda.SelectCommand = new OdbcCommand();
                odbcda.SelectCommand.Connection = new OdbcConnection(ConnectionString);
                return new Database(odbcda);
            }
            if (ProviderName.ToUpper().Contains("ORACLE"))
            {
                OracleDataAdapter oda = new OracleDataAdapter();
                oda.SelectCommand = new OracleCommand();
                oda.SelectCommand.Connection = new OracleConnection(ConnectionString);
                return new Database(oda);
            }
            return null;
        }

    }
}
