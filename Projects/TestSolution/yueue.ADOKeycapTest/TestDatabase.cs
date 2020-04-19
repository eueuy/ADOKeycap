using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using yueue.ADOKeycap;

namespace yueue.ADOKeycapTest
{
    [TestFixture]
    public class TestDatabase
    {
        private Database db;
        private Database db2;
               
        public TestDatabase()
        {
            db = DatabaseManager.CreateDatabase(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source = F:\yueue.ADOKeycap\Projects\TestSolution\yueue.ADOKeycapTest\bin\Debug\data\testdb1.mdb;","oledb");
            db2 = DatabaseManager.CreateDatabase("testdb2");
        }
        
        [Test]
        public void Test_ExecuteDataSet_Simple()
        {
            DataSet ds = db.ExecuteDataSet("select * from book where ±àºÅ=10",0,0);
            DataSet ds2 = db.ExecuteDataSet("select * from book", 1, 5);
            Assert.AreEqual("²âÊÔÊý¾Ý6", ds.Tables[0].Rows[0][1].ToString());
            Assert.AreEqual("²âÊÔÊý¾Ý2", ds2.Tables[0].Rows[0][1].ToString());
        }

        [Test]
        public void Test_ExecuteDataSet_Param()
        {
            //db.ClearParameters();
            db.AddParameter("@ids",10,DbType.UInt32,50,ParameterDirection.Input);
            db.AddParameter("@f1", "²âÊÔÊý¾Ý6", DbType.String, 50, ParameterDirection.Input);
            DataSet ds = db.ExecuteDataSet("select * from book where ±àºÅ=@ids and ×Ö¶Î1=@f1", "tb1");
            Assert.AreEqual("²âÊÔÊý¾Ý6",ds.Tables["tb1"].Rows[0][1].ToString());
        }

        [Test]
        public void Test_ExecuteDataSet_Procedure()
        {
            db2.AddParameter("@ids",10);
            DataSet ds = db2.ExecuteDataSet("UserView");
            Assert.AreEqual("TESTDATA10", ds.Tables[0].Rows[0][1].ToString().Trim());
        }

        [Test]
        public void Test_ExecuteNonQuery_Simple()
        {
            string testData = DateTime.Now.ToString();
            int i = db.ExecuteNonQuery("Insert Into book (×Ö¶Î1,×Ö¶Î2,×Ö¶Î3)values('"+testData+"','',0)");
            DataSet ds = db.ExecuteDataSet("select * from book where ×Ö¶Î1='" + testData + "'");
            Assert.AreEqual(testData,ds.Tables[0].Rows[0][1].ToString());
        }

        [Test]
        public void Test_ExecuteNonQuery_Param()
        {
            string testData = DateTime.Now.ToString();
            //db.ClearParameters();
            db.AddParameter("@f1", testData);
            int i = db.ExecuteNonQuery("Insert Into book (×Ö¶Î1,×Ö¶Î2,×Ö¶Î3)values(@f1,'kong',0)");
            db.AddParameter("@f1", testData);
            DataSet ds = db.ExecuteDataSet("select * from book where ×Ö¶Î1=@f1");
            Assert.AreEqual(testData, ds.Tables[0].Rows[0][1].ToString());
        }

        [Test]
        public void zTest_ExecuteNonQuery_Procedure_OutputParam()
        {            
            db2.AddParameter("@Ids", 10);
            DbParameter dbp = db2.AddParameter("@Infos", "a", DbType.String, 50,ParameterDirection.Output);
            db2.ExecuteNonQuery("UserInfo");
            Assert.AreEqual("TESTDATA10", dbp.Value.ToString().Trim());
        }

        [Test]
        public void Test_ExecuteScalar_Simple()
        {
            Assert.AreEqual(0, db.ExecuteScalar("SELECT SUM(×Ö¶Î3) FROM book"));
        }

        [Test]
        public void Test_ExecuteReader_Simple()
        {
            DbDataReader odr = db.ExecuteReader("select * from book where ±àºÅ=10");
            odr.Read();
            Assert.AreEqual("²âÊÔÊý¾Ý6", odr[1].ToString());
            if (odr.IsClosed) { odr.Close(); }
        }


        [Test]
        public void Test_ExecuteReader_Param()
        {
            db.AddParameter("@ids", 10);
            DbDataReader odr = db.ExecuteReader("select * from book where ±àºÅ=@ids");
            odr.Read();
            Assert.AreEqual("²âÊÔÊý¾Ý6", odr[1].ToString());
            odr.Close();
        }

        [Test]
        public void Test_ExecuteReader_Procedure()
        {
            db2.AddParameter("@ids", 10);
            DbDataReader odr = db2.ExecuteReader("UserView");
            odr.Read();
            Assert.AreEqual("TESTDATA10", odr[1].ToString().Trim());
            odr.Close();
        }

       
    }
}
