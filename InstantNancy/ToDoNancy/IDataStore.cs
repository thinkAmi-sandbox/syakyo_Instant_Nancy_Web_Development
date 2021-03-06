using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNancy
{
    public interface IDataStore
    {
        IEnumerable<Todo> GetAll();
        Task<IEnumerable<Todo>> GetAllAsync();
        long Count { get; }
        bool TryAdd(Todo todo);
        bool TryRemove(int id, string userName);
        bool TryUpdate(Todo todo, string userName);
    }
}
