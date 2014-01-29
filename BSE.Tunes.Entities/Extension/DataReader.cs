using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    public static class DataReader
    {
        public static byte[] GetBytes(this IDataRecord dataReader, string strFieldName, bool nullAllowed, byte[] arbyteDefault)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader",
                    string.Format(CultureInfo.InvariantCulture, "IDataReader.GetBytes Field '{0}'", strFieldName));
            }
            object obj = dataReader[strFieldName];
            if (obj is System.DBNull)
            {
                if (nullAllowed == true)
                {
                    return arbyteDefault;
                }
                else
                {
                    throw new ArgumentException("nullAllowed",
                        string.Format(CultureInfo.InvariantCulture, "IDataReader.GetBytes Field '{0}' is null.", strFieldName));
                }
            }
            else
            {
                byte[] arb = obj as byte[];
                if (arb != null)
                {
                    return arb;
                }
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "IDataReader.GetBytes Field '{0}'", strFieldName));
        }

        public static Guid GetGuid(this IDataRecord dataReader, string strFieldName, bool nullAllowed, Guid defaultGuid)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader",
                    string.Format(CultureInfo.InvariantCulture, "IDataReader.GetGuid Field '{0}'", strFieldName));
            }
            object obj = dataReader[strFieldName];
            if (obj is System.DBNull)
            {
                if (nullAllowed == true)
                {
                    return defaultGuid;
                }
                else
                {
                    throw new ArgumentException("nullAllowed",
                        string.Format(CultureInfo.InvariantCulture, "IDataReader.GetGuid Field '{0}' is null.", strFieldName));
                }
            }
            else if (obj is Guid)
            {
                return (Guid)obj;
            }
            else if (obj is string)
            {
                try
                {
                    return new Guid((string)obj);
                }
                catch (FormatException)
                {
                    throw new FormatException(string.Format(CultureInfo.InvariantCulture, "IDataReader.GetGuid Field '{0}'", strFieldName));
                }
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "IDataReader.GetGuid Field '{0}'", strFieldName));
        }

        public static int GetInt32(this IDataReader dataReader, string strFieldName, bool nullAllowed, int iDefault)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader",
                    string.Format(CultureInfo.InvariantCulture, "IDataReader.GetInt32 Field '{0}'", strFieldName));
            }
            object obj = dataReader[strFieldName];
            if (obj is System.DBNull)
            {
                if (nullAllowed == true)
                {
                    return iDefault;
                }
                else
                {
                    throw new ArgumentException("nullAllowed",
                        string.Format(CultureInfo.InvariantCulture, "IDataReader.GetInt32 Field '{0}' is null.", strFieldName));
                }
            }
            else if (obj is int)
            {
                return (int)obj;
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "IDataReader.GetInt32 Field '{0}'", strFieldName));
        }
        public static string GetString(this IDataRecord dataReader, string strFieldName, bool nullAllowed, string strDefault)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader",
                    string.Format(CultureInfo.InvariantCulture, "IDataReader.GetString Field '{0}'", strFieldName));
            }
            object obj = dataReader[strFieldName];
            if (obj is System.DBNull)
            {
                if (nullAllowed == true)
                {
                    return strDefault;
                }
                else
                {
                    throw new ArgumentException("nullAllowed",
                        string.Format(CultureInfo.InvariantCulture, "IDataReader.GetString Field '{0}' is null.", strFieldName));
                }
            }
            else
            {
                string item = obj as string;
                if (item != null)
                {
                    return item;
                }
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "IDataReader.GetString Field '{0}'", strFieldName));
        }
        public static DateTime GetDateTime(this IDataRecord dataReader, string strFieldName, bool nullAllowed, DateTime dtDefault)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader",
                    string.Format(CultureInfo.InvariantCulture, "IDataReader.GetDateTime Field '{0}'", strFieldName));
            }
            object obj = dataReader[strFieldName];
            if (obj is System.DBNull)
            {
                if (nullAllowed == true)
                {
                    return dtDefault;
                }
                else
                {
                    throw new ArgumentException("nullAllowed",
                        string.Format(CultureInfo.InvariantCulture, "IDataReader.GetDateTime Field '{0}' is null.", strFieldName));
                }
            }
            else if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "ModelSql.GetDateTime Field '{0}'", strFieldName));
        }
        public static TimeSpan GetTimeSpan(this IDataRecord dataReader, string strFieldName, bool nullAllowed, TimeSpan tsDefault)
        {
            DateTime dateTime = GetDateTime(dataReader, strFieldName, nullAllowed, new DateTime());
            if (dateTime != null)
            {
                return new TimeSpan(
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second);
            }
            return new TimeSpan(0, 0, 0);
        }
    }
}
