﻿using System;
using System.Data.SQLite;
using System.Diagnostics;
using Mono.Data.Sqlite;

namespace TechnicSolderHelper.SQL
{
    public class FtbPermissionsSqlHelper : SqlHelper
    {
        private readonly String _createTableString;

        public FtbPermissionsSqlHelper()
            : base("ftbperms")
        {
            _createTableString = String.Format("CREATE TABLE IF NOT EXISTS `{0}` ( `ID` INTEGER NOT NULL, `ModName` TEXT, `ModAuthor` TEXT, `ShortName` TEXT, `ModID` TEXT UNIQUE, `PublicPerm` TEXT , `PrivatePerm` TEXT, `ModLink` TEXT, `PermLink` TEXT, `CustPrivate` TEXT, `CustFTB` TEXT, PRIMARY KEY(ID));", TableName);
            ExecuteDatabaseQuery(_createTableString);
        }

        /// <summary>
        /// Checks if the ftb database contains any permissions for the mod
        /// </summary>
        /// <param name="toCheck">
        /// The ID of the mod to check for</param>
        /// <param name="isPublic">
        /// If false, Checks for private distribution permissions. 
        /// If true, Checks for public distribution permissions.</param>
        /// <returns>Return the level of distribution.
        /// If no level found, return PermissionLevel.Unknown</returns>
        public PermissionLevel DoFtbHavePermission(String toCheck, Boolean isPublic)
        {
            toCheck = toCheck.Replace("'", "`");

            var sql = String.Format("SELECT PublicPerm, PrivatePerm FROM {0} WHERE ShortName LIKE '{1}' OR ModID LIKE '{1}';", TableName, toCheck.ToLower());
            if (IsUnix())
            {
                using (SqliteConnection db = new SqliteConnection(ConnectionString))
                {
                    db.Open();
                    using (SqliteCommand cmd = new SqliteCommand(sql, db))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            String level = "";
                            while (reader.Read())
                            {
                                level = isPublic ? reader["PublicPerm"].ToString() : reader["PrivatePerm"].ToString();
                            }

                            switch (level.ToLower())
                            {
                                case "open":
                                    return PermissionLevel.Open;
                                case "closed":
                                    return PermissionLevel.Closed;
                                case "ftb":
                                    return PermissionLevel.Ftb;
                                case "notify":
                                    return PermissionLevel.Notify;
                                case "request":
                                    return PermissionLevel.Request;
                                default:
                                    return PermissionLevel.Unknown;
                            }
                        }
                    }
                }
            }
            using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
            {
                db.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        String level = "";
                        while (reader.Read())
                        {
                            level = isPublic ? reader["PublicPerm"].ToString() : reader["PrivatePerm"].ToString();
                        }

                        switch (level.ToLower())
                        {
                            case "open":
                                return PermissionLevel.Open;
                            case "closed":
                                return PermissionLevel.Closed;
                            case "ftb":
                                return PermissionLevel.Ftb;
                            case "notify":
                                return PermissionLevel.Notify;
                            case "request":
                                return PermissionLevel.Request;
                            default:
                                return PermissionLevel.Unknown;
                        }
                    }
                }
            }
        }

        public enum InfoType
        {
            //PermLink,
            //CustPrivate,
            //CustFtb,
            ModLink,
            ModAuthor,
            //ShortName,
            //ModId,
            ModName
        }

        public String GetInfoFromModId(String modId, InfoType infoType)
        {
            modId = modId.Replace("'", "`");

            String sql = String.Format("SELECT {2} FROM {0} WHERE ModID LIKE '{1}' OR ShortName LIKE '{1}';", TableName, modId.ToLower(), infoType);

            if (IsUnix())
            {
                using (SqliteConnection db = new SqliteConnection(ConnectionString))
                {
                    db.Open();
                    using (SqliteCommand cmd = new SqliteCommand(sql, db))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader[infoType.ToString()].ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
                {
                    db.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader[infoType.ToString()].ToString();
                            }
                        }
                    }
                }
            }
            return "";
        }

        public String GetInfoFromShortName(String shortName, InfoType infoType)
        {
            shortName = shortName.Replace("'", "`");

            String sql = String.Format("SELECT {2} FROM {0} WHERE ShortName LIKE '{1}';", TableName, shortName.ToLower(), infoType);

            if (IsUnix())
            {
                using (SqliteConnection db = new SqliteConnection(ConnectionString))
                {
                    db.Open();
                    using (SqliteCommand cmd = new SqliteCommand(sql, db))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader[infoType.ToString()].ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
                {
                    db.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader[infoType.ToString()].ToString();
                            }
                        }
                    }
                }
            }
            return "";
        }

        public String GetShortName(String modId)
        {
            modId = modId.Replace("'", "`");

            String sql = String.Format("SELECT ShortName FROM {0} WHERE ModID LIKE '{1}';", TableName, modId);

            if (IsUnix())
            {
                using (SqliteConnection db = new SqliteConnection(ConnectionString))
                {
                    db.Open();
                    using (SqliteCommand cmd = new SqliteCommand(sql, db))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader["ShortName"].ToString();
                            }
                        }
                    }
                }
            }
            else
            {
                using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
                {
                    db.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader["ShortName"].ToString();
                            }
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Adds a mod to the FTB permission database
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="modAuthor"></param>
        /// <param name="modId"></param>
        /// <param name="publicPermissions"></param>
        /// <param name="privatePermissions"></param>
        /// <param name="modLink"></param>
        /// <param name="permLink"></param>
        /// <param name="custPrivate"></param>
        /// <param name="custFtb"></param>
        /// <param name="shortname"></param>
        public void AddFtbModPerm(String modName, String modAuthor, String modId, String publicPermissions, String privatePermissions, String modLink, String permLink, String custPrivate, String custFtb, String shortname)
        {
            modName = modName.Replace("'", "`");
            modAuthor = modAuthor.Replace("'", "`");
            modId = modId.Replace("'", "`");

            String sql = String.Format("INSERT OR REPLACE INTO {0}(ModName, ModAuthor, ModID, PublicPerm, PrivatePerm, ModLink, PermLink, CustPrivate, CustFTB, ShortName) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '{10}');", TableName, modName, modAuthor, modId, publicPermissions, privatePermissions, modLink, permLink, custPrivate, custFtb, shortname);

            ExecuteDatabaseQuery(sql);
        }

        public void AddFtbModPerm(String modId, String shortName)
        {
            modId = modId.Replace("'", "`");

            String sql = String.Format("INSERT OR IGNORE INTO {0}(ShortName, ModID) VALUES ('{1}', '{2}');", TableName, shortName, modId);
            Debug.WriteLine(sql);
            ExecuteDatabaseQuery(sql);
        }

        public override void ResetTable()
        {
            base.ResetTable();
            ExecuteDatabaseQuery(_createTableString);
        }
    }
}
