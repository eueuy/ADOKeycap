/**
 * This is a Single Pattern class to Create a DataBase Object via ADOKeycap
 * Useage:
 * Add ADOKeycap 's reference to your project in solution manager.
 * Add this file to your project in solution manager.And do some necessary rework by comment.
 * When you need accessing database , just write like follows:
 * 
 * Database db = DatabaseFactory.GetDatabase();
 * GridView1.DataScource = db.ExecuteDataSet("SELECT * FROM pubs");
 * GridView1.DataBind();
 *  
 * All done!
 * by york zhung
 * york-zh.spaces.live.com
 * **/

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using yueue.ADOKeycap;

    //namespace here if need
    /// <summary>
    /// This is a Single Pattern class to Create a DataBase Object via ADOKeycap
    /// </summary>
    public class DatabaseFactory
    {
        private static Database _DataBase;
        private static Object _ClassLock = typeof(DatabaseFactory);

        private DatabaseFactory()
        {
        }

        /// <summary>
        /// get a "database" object in order to use database
        /// </summary>
        /// <returns>database object link</returns>
        public static Database GetDatabase()
        {
            lock (_ClassLock)  // use "lock" sentence to offer the multi-threading
            {
                if (_DataBase == null)
                {
                    //replace the connection string to yours .In Access database,just replace the "Data\db.mdb" to yours.
                    //Do not change the second parameter "olddb".
                    _DataBase = DatabaseManager.CreateDatabase(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + AppDomain.CurrentDomain.BaseDirectory + @"Data\db.mdb", "oledb");
                }
                return _DataBase;
            }
        }
    }