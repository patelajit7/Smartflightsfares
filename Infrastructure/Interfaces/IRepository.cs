using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRepository<T>
    {
        int Insert(T Value);
        int Update(T Value);
        int Delete(T Value);
        T GetEntity(T Value);
        List<T> GetEntityDetails(int ID);
        List<T> GetAllDetails();
    }
    public abstract class Repository<T>
    {
        public abstract List<T> GetUserDetails(int UserID);
        public abstract List<T> GetUserDetails(string SearchUsers);

    }
}
