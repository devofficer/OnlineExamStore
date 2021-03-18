using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnlineExam.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPositionRepository PositionRepository { get; }
        void Save();
        T ExecuteQuery<T>(string query, string connectionString = "");
        IEnumerable<T> ExecuteEnumerableQuery<T>(string query, string connectionString = "");

    }
}
