using System.Data.SqlClient;

namespace OrmExample.Mapping
{
    public interface IMapping
    {
        IEntity Load(int id, SqlDataReader dataReader);
        string[] Columns { get; }
        string TableName { get; }
        SqlParameter[] ModifyParameters(IEntity entity);
    }

}