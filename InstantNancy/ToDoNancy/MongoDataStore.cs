using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoNancy
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using System.Threading.Tasks;

    public class MongoDataStore : IDataStore
    {
        private MongoCollection<BsonDocument> todos;

        public MongoDataStore(string connectionString)
        {
            // 「MongoDatabase.Createは古い形式です」という警告が出るため、修正
            // http://stackoverflow.com/questions/7201847/how-to-get-the-mongo-database-specified-in-connection-string-in-c-sharp

            var server = new MongoClient(connectionString).GetServer();
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            var database = server.GetDatabase(databaseName);
            todos = database.GetCollection("todos");
        }

        public long Count { get { return todos.FindAll().Count(); } }

        public IEnumerable<Todo> GetAll()
        {
            var fields = typeof(Todo).GetProperties().Select(info => info.Name);
            return todos.FindAllAs<Todo>().SetFields(fields.ToArray()).ToArray();
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            var fields = typeof(Todo).GetProperties().Select(info => info.Name);
            var cursor = new Task<IEnumerable<Todo>>(() => todos.FindAllAs<Todo>().SetFields(fields.ToArray()));
            cursor.Start();
            return await cursor;
        }

        public bool TryAdd(Todo todo)
        {
            if (FindById(todo.id) != null)
                return false;

            todos.Insert(todo);
            return true;
        }

        public bool TryRemove(int id, string userName)
        {
            if (FindByIdAndUser(id, userName) == null)
                return false;

            todos.Remove(Query.EQ("_id", id));
            return true;
        }

        private BsonDocument FindById(long id)
        {
            return todos.Find(Query.EQ("_id", id)).FirstOrDefault();
        }

        private BsonDocument FindByIdAndUser(long id, string userName)
        {
            return todos.Find(
                    Query.And(
                        Query.EQ("_id", id),
                        Query.EQ("userName", userName)))
                .FirstOrDefault();
        }

        public bool TryUpdate(Todo todo, string userName)
        {
            if (FindByIdAndUser(todo.id, userName) == null)
                return false;

            todos.Save(todo);
            return true;
        }
    }
}