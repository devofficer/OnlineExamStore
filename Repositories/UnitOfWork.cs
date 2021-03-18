using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using OnlineExam.Models;


namespace OnlineExam.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();
        private IPositionRepository positionRepository;
        public IPositionRepository PositionRepository
        {
            get
            {
                if (this.positionRepository == null)
                {
                    this.positionRepository = new PositionRepository();
                }
                return positionRepository;
            }
        }
        public T ExecuteQuery<T>(string query, string connectionString = "")
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                context.Database.Connection.Close();
                context.Database.Connection.Dispose();
                context.Database.Connection.ConnectionString = connectionString;
                context.Database.Connection.Open();
            }
            var result = context.Database.SqlQuery<T>(query);
            return result.FirstOrDefault();
        }
        public IEnumerable<T> ExecuteEnumerableQuery<T>(string query, string connectionString = "")
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                context.Database.Connection.Close();
                context.Database.Connection.Dispose();
                context.Database.Connection.ConnectionString = connectionString;
                context.Database.Connection.Open();
            }
            var result = context.Database.SqlQuery<T>(query);
            return result;
        }
        public void Save()
        {
            context.SaveChanges();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
