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
    /// ��������Database����ľ�̬��
    /// </summary>
    public static class DatabaseManager
    { 
        /// <summary>
        /// ��������Database����
        /// </summary>
        /// <param name="DatabaseNameInConfigFile">�������ļ��ж�������ݿ�����name,����ȷ�����ĸ����ݿ���в���</param>
        /// <returns>���ض�Ӧ���ݿ��Database����</returns>
        public static Database CreateDatabase(string DatabaseNameInConfigFile)
        {
            string pn = ConfigurationManager.ConnectionStrings[DatabaseNameInConfigFile].ProviderName; //��ȡ�����ļ�
            string cs = ConfigurationManager.ConnectionStrings[DatabaseNameInConfigFile].ConnectionString;
            return MakeDBObject(cs, pn);
        }
        public static Database CreateDatabase(string ConnectionString, string ProviderName)
        {
            return MakeDBObject(ConnectionString,ProviderName);
        }


        private static Database MakeDBObject(string ConnectionString,string ProviderName)
        {
            //�����ж����ݿ����Ͳ�������Ӧ�Ķ���
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
