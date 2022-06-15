using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Interfaces;
using Infrastructure.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;


namespace Database
{
    public class MongoDBRepository : IMongoDBRepository
    {

        private static MongoDatabase _context;
        public MongoDBRepository()
        {
            _context = MongoDBContext.Instance;
        }
        #region guid
        public MongoCollection<T> GetCollection<T>() where T : MongoEntity
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
            return GetCollection<T>().Remove(Query.EQ("_id", item._id)).DocumentsAffected > 0;
        }
        public void DeleteAll<T>() where T : MongoEntity
        {
            _context.DropCollection(typeof(T).Name);
        }
        public List<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : MongoEntity
        {
            return All<T>().Where<T>(expression).ToList<T>();
        }
        public T Single<T>(string id) where T : MongoEntity
        {
            return GetCollection<T>().FindOne(Query.EQ("_id", ObjectId.Parse(id)));
        }
        public IQueryable<T> All<T>() where T : MongoEntity
        {
            return GetCollection<T>().FindAll().AsQueryable();
        }
        public void Add<T>(T item) where T : MongoEntity
        {
            var collection = GetCollection<T>();

            collection.Save(item);
        }
        public void Add<T>(IEnumerable<T> items) where T : MongoEntity
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }
        #endregion
    }
}

