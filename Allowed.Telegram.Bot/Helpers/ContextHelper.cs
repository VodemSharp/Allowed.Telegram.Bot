using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Allowed.Telegram.Bot.Helpers
{
    public static class ContextHelper
    {
        public static List<T> DataReaderMapToList<T>(this IDataReader dr)
        {
            List<T> list = new List<T>();

            while (dr.Read())
            {
                T obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        public static List<T> FromSqlRaw<T>(this DbContext db, string sql)
        {
            List<T> result = new List<T>();

            try
            {
                using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    db.Database.OpenConnection();

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        result = reader.DataReaderMapToList<T>();
                    }
                }
            }
            finally
            {
                db.Database.CloseConnection();
            }

            return result;
        }

        public static List<object> DataReaderMapToList(this IDataReader dr, Type entityType)
        {
            List<object> list = new List<object>();

            while (dr.Read())
            {
                object obj = Activator.CreateInstance(entityType);
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        public static List<object> FromSqlRaw(this DbContext db, string sql, Type entityType)
        {
            List<object> result = new List<object> { };

            try
            {
                using (DbCommand command = db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    db.Database.OpenConnection();

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        result = reader.DataReaderMapToList(entityType);
                    }
                }
            }
            finally
            {
                db.Database.CloseConnection();
            }

            return result;
        }

        public static DbSet<object> Set(this DbContext db, Type type)
        {
            return (DbSet<object>)db.GetType().GetMethod("Set").MakeGenericMethod(type).Invoke(db, null);
        }
    }
}
