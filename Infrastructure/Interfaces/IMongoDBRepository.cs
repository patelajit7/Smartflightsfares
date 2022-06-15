using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.MongoDB;

namespace Infrastructure.Interfaces
{
    public interface IMongoDBRepository
    {
        void Delete<T>(Expression<Func<T, bool>> expression) where T : MongoEntity;
        bool Delete<T>(T item) where T : MongoEntity;
        void DeleteAll<T>() where T : MongoEntity;
        T Single<T>(string id) where T : MongoEntity;
        List<T> Where<T>(Expression<Func<T, bool>> expression) where T : MongoEntity;
        IQueryable<T> All<T>() where T : MongoEntity;
        void Add<T>(T item) where T : MongoEntity;
        void Add<T>(IEnumerable<T> items) where T : MongoEntity;
    }
}
