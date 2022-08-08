using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Interfaces;
using Infrastructure.MongoDB;
using MongoDB.Driver;


namespace Database
{
    public class MongoDBRepository : IMongoDBRepository
    {

        private static IMongoDatabase _context;
        public MongoDBRepository()
        {
            _context = MongoDBContext.Instance;
        }
        #region guid
        public IMongoCollection<T> GetCollection<T>() where T : MongoEntity
        {
            return _context.GetCollection<T>(typeof(T).Name);
        }
        public void Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : MongoEntity
        {
            var items = All<T>().Where(expression);
            foreach (T item in items)
            {
                Delete(item);
            }
        }
        public bool Delete<T>(T item) where T : MongoEntity
        {
            return GetCollection<T>().DeleteManyAsync(o => o._id.Equals(item._id)).Result.DeletedCount > 0;
        }
        public void DeleteAll<T>() where T : MongoEntity
        {
            _context.DropCollection(typeof(T).Name);
        }
        public List<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : MongoEntity
        {
            return All<T>().Where<T>(expression).ToList<T>();
        }
        public T Single<T>(Expression<Func<T, bool>> expression) where T : MongoEntity
        {
            return All<T>().Where(expression).SingleOrDefault();
        }
        public IQueryable<T> All<T>() where T : MongoEntity
        {
            return GetCollection<T>().AsQueryable();
        }
        public void Add<T>(T item) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.InsertOneAsync(item);
        }
        public void Add<T>(IEnumerable<T> items) where T : MongoEntity
        {
            var collection = GetCollection<T>();
            collection.InsertManyAsync(items);
        }
        #endregion
    }
}

