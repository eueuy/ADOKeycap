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
    /// 执行主要操作的类
    /// </summary>
    public class Database
    {
        private DbDataAdapter mDataAdapter; //指向传入的DbDataAdapter的引用
        private DbCommand mCommand;  //指向传入的DbDataAdapter.SelectCommand的引用

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DDA">获得一个实例化了的DbDataAdapter的派生类</param>
        public Database(DbDataAdapter DDA)
        {
            mDataAdapter = DDA;
            mCommand = DDA.SelectCommand;
        }

        /// <summary>
        /// 判断一个stirng是否为储存过程
        /// </summary>
        /// <param name="SQLText">目标string</param>
        /// <returns>返回是否为储存过程的调用</returns>
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
        /// 执行数据库命令返回受影响的行数
        /// </summary>
        /// <param name="SQLText">SQL语句</param>
        /// <returns>受影响的行数</returns>
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
        /// 执行数据库命令返回DataReader
        /// </summary>
        /// <param name="SQLText">SQL命令</param>
        /// <returns>返回DataReader</returns>
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
        /// 执行统计的方法
        /// </summary>
        /// <param name="SQLText">SQL命令</param>
        /// <returns>返回object对象代表统计数据或者结果的第一列第一行</returns>
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
        /// 执行查询返回填充的DataSet对象
        /// </summary>
        /// <param name="SQLText">SQl命令</param>
        /// <param name="VisualTableName">虚拟表名</param>
        /// <param name="StartIndex">制定返回多少行以后的数据</param>
        /// <param name="Count">制定总共返回多少行</param>
        /// <returns>返回按要求填充了的DataSet</returns>
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
        //下面为重载调用,不包含实际代码
        public DataSet ExecuteDataSet(string SQLText, int StartIndex, int Count){return ExecuteDataSet(SQLText, "Table1", StartIndex, Count);}
        public DataSet ExecuteDataSet(string SQLText, string VisualTableName){return ExecuteDataSet(SQLText, VisualTableName, 0, 0);}
        public DataSet ExecuteDataSet(string SQLText){return ExecuteDataSet(SQLText,"Table1", 0, 0);}

        /// <summary>
        /// 添加一个参数
        /// </summary>
        /// <param name="ParameterName">参数的名称</param>
        /// <param name="Value">参数的值</param>
        /// <param name="Type">参数值的类型</param>
        /// <param name="Size">参数值的大小</param>
        /// <param name="Direction">参数的返回类型</param>
        /// <returns>返回添加后的参数对象DbParameter</returns>
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
        //下面为重载调用,不包含实际代码        
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
        /// 清除DbParameterCollection中所有DbParameter的引用
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
