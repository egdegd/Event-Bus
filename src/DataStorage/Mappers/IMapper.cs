using System.Data.SqlClient;

namespace DataStorage.Mappers
{
    public interface IMapper<out T>
    {
        T ReadItem(SqlDataReader rd);
    }
}
