using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExam.Repositories
{
    public interface ICommonRepository
    {
        IQueryable GetLookups(string moduleCode);
    }
}
