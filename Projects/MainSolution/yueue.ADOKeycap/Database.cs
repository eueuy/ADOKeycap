using System;
using System.Collections.Generic;
using System.Text;
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
    /// ִ����Ҫ��������
    /// </summary>
    public class Database
    {
        private DbDataAdapter mDataAdapter; //ָ�����DbDataAdapter������
        private DbCommand mCommand;  //ָ�����DbDataAdapter.SelectCommand������

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="DDA">���һ��ʵ�����˵�DbDataAdapter��������</param>
        public Database(DbDataAdapter DDA)
        {
            mDataAdapter = DDA;
            mCommand = DDA.SelectCommand;
        }

        /// <summary>
        /// �ж�һ��stirng�Ƿ�Ϊ�������
        /// </summary>
        /// <param name="SQLText">Ŀ��string</param>
        /// <returns>�����Ƿ�Ϊ������̵ĵ���</returns>
        private bool IsProcedure(string SQLText)
        {
            if (SQLText.Contains(" "))
            {
                string[] tmp;
                tmp = SQLText.Split(' ');
                if (tmp[0].ToUpper() == "EXECUTE" || tmp[0].ToUpper() == "EXEC")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// ִ�����ݿ��������Ӱ�������
        /// </summary>
        /// <param name="SQLText">SQL���</param>
        /// <returns>��Ӱ�������</returns>
        public int ExecuteNonQuery(string SQLText)
        {
            if (IsProcedure(SQLText)) { mCommand.CommandType = CommandType.StoredProcedure; } else { mCommand.CommandType = CommandType.Text; }
            mCommand.CommandText = SQLText;
            try
            {
                mCommand.Connection.Open();
                return mCommand.ExecuteNonQuery();
            }
            finally
            {
                mCommand.Connection.Close();
                ClearParameters();
            }
        }

        /// <summary>
        /// ִ�����ݿ������DataReader
        /// </summary>
        /// <param name="SQLText">SQL����</param>
        /// <returns>����DataReader</returns>
        public DbDataReader ExecuteReader(string SQLText)
        {
            if (IsProcedure(SQLText)) { mCommand.CommandType = CommandType.StoredProcedure; } else { mCommand.CommandType = CommandType.Text; }
            mCommand.CommandText = SQLText;
            mCommand.Connection.Open();
            try
            {
                return mCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            finally
            { ClearParameters(); }
        }

        /// <summary>
        /// ִ��ͳ�Ƶķ���
        /// </summary>
        /// <param name="SQLText">SQL����</param>
        /// <returns>����object�������ͳ�����ݻ��߽���ĵ�һ�е�һ��</returns>
        public object ExecuteScalar(string SQLText)
        {
            if (IsProcedure(SQLText)) { mCommand.CommandType = CommandType.StoredProcedure; } else { mCommand.CommandType = CommandType.Text; }
            mCommand.CommandText = SQLText;
            try
            {
                mCommand.Connection.Open();
                return mCommand.ExecuteScalar();
            }
            finally
            {
                mCommand.Connection.Close();
                ClearParameters();
            }   
        }

        /// <summary>
        /// ִ�в�ѯ��������DataSet����
        /// </summary>
        /// <param name="SQLText">SQl����</param>
        /// <param name="VisualTableName">�������</param>
        /// <param name="StartIndex">�ƶ����ض������Ժ������</param>
        /// <param name="Count">�ƶ��ܹ����ض�����</param>
        /// <returns>���ذ�Ҫ������˵�DataSet</returns>
        public DataSet ExecuteDataSet(string SQLText,string VisualTableName, int StartIndex, int Count)
        {
            DataSet ds = new DataSet();
            if (IsProcedure(SQLText)) { mCommand.CommandType = CommandType.StoredProcedure; } else { mCommand.CommandType = CommandType.Text; }
            mCommand.CommandText = SQLText;
            try
            {
                mCommand.Connection.Open();
                mDataAdapter.Fill(ds, StartIndex, Count, VisualTableName);
                return ds;
            }
            finally
            {
                mCommand.Connection.Close();
                ClearParameters();
            }      
        }
        //����Ϊ���ص���,������ʵ�ʴ���
        public DataSet ExecuteDataSet(string SQLText, int StartIndex, int Count){return ExecuteDataSet(SQLText, "Table1", StartIndex, Count);}
        public DataSet ExecuteDataSet(string SQLText, string VisualTableName){return ExecuteDataSet(SQLText, VisualTableName, 0, 0);}
        public DataSet ExecuteDataSet(string SQLText){return ExecuteDataSet(SQLText,"Table1", 0, 0);}

        /// <summary>
        /// ���һ������
        /// </summary>
        /// <param name="ParameterName">����������</param>
        /// <param name="Value">������ֵ</param>
        /// <param name="Type">����ֵ������</param>
        /// <param name="Size">����ֵ�Ĵ�С</param>
        /// <param name="Direction">�����ķ�������</param>
        /// <returns>������Ӻ�Ĳ�������DbParameter</returns>
        public DbParameter AddParameter(string ParameterName, object Value, DbType Type, int Size, ParameterDirection Direction)
        {
            DbParameter dbp = mCommand.CreateParameter();
            dbp.ParameterName = ParameterName;
            dbp.Value = Value;
            dbp.DbType = Type;
            if (Size != 0) { dbp.Size = Size; }
            dbp.Direction = Direction;
            mCommand.Parameters.Add(dbp);
            return dbp;
        }
        //����Ϊ���ص���,������ʵ�ʴ���        
        public DbParameter AddParameter(string ParameterName, object Value, DbType Type, int Size) { return AddParameter(ParameterName, Value, Type, Size,ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, object Value , ParameterDirection Direction) { return AddParameter(ParameterName, Value, DbType.Object, 0, Direction); }
        public DbParameter AddParameter(string ParameterName, object Value) { return AddParameter(ParameterName, Value,DbType.Object,0,ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, string Value) { return AddParameter(ParameterName, Value, DbType.String, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Int32 Value) { return AddParameter(ParameterName, Value, DbType.Int32, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Int16 Value) { return AddParameter(ParameterName, Value, DbType.Int16, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Boolean Value) { return AddParameter(ParameterName, Value, DbType.Boolean, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, UInt32 Value) { return AddParameter(ParameterName, Value, DbType.UInt32, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, UInt16 Value) { return AddParameter(ParameterName, Value, DbType.UInt16, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Byte Value) { return AddParameter(ParameterName, Value, DbType.Byte, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Decimal Value) { return AddParameter(ParameterName, Value, DbType.Decimal, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Double Value) { return AddParameter(ParameterName, Value, DbType.Double, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, DateTime Value) { return AddParameter(ParameterName, Value, DbType.DateTime, 0, ParameterDirection.Input); }
        public DbParameter AddParameter(string ParameterName, Single Value) { return AddParameter(ParameterName, Value, DbType.Single, 0, ParameterDirection.Input); }

        public DbParameter AddOutParameter(string ParameterName, object Value, DbType Type, int Size) { return AddParameter(ParameterName, Value, Type, Size, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, object Value) { return AddParameter(ParameterName, Value, DbType.Object, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, string Value) { return AddParameter(ParameterName, Value, DbType.String, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Int32 Value) { return AddParameter(ParameterName, Value, DbType.Int32, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Int16 Value) { return AddParameter(ParameterName, Value, DbType.Int16, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Boolean Value) { return AddParameter(ParameterName, Value, DbType.Boolean, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, UInt32 Value) { return AddParameter(ParameterName, Value, DbType.UInt32, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, UInt16 Value) { return AddParameter(ParameterName, Value, DbType.UInt16, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Byte Value) { return AddParameter(ParameterName, Value, DbType.Byte, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Decimal Value) { return AddParameter(ParameterName, Value, DbType.Decimal, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Double Value) { return AddParameter(ParameterName, Value, DbType.Double, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, DateTime Value) { return AddParameter(ParameterName, Value, DbType.DateTime, 0, ParameterDirection.Output); }
        public DbParameter AddOutParameter(string ParameterName, Single Value) { return AddParameter(ParameterName, Value, DbType.Single, 0, ParameterDirection.Output); }


        /// <summary>
        /// ���DbParameterCollection������DbParameter������
        /// </summary>
        public void ClearParameters()
        {
            mCommand.Parameters.Clear();
        }

        public DbParameterCollection Parameters
        {
            get { return mCommand.Parameters; }
        }

        public DbCommand Command
        {
            get { return mCommand; }
        }

        public DbDataAdapter DataAdapter
        {
            get { return mDataAdapter; }
        }

        public DbTransaction Transaction
        {
            get { return mCommand.Transaction; }
        }
   
    }
}
