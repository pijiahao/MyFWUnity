using MyFWUnity.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Services
{
    public interface ICommonService<TSource, TEntity> : IBaseService where TEntity : class where TSource : BaseDataModel<TSource, TEntity>
    {
        void Add(TSource source);
        void Add(List<TSource> sources);
        void Update(TSource source);
        void Update(List<TSource> sources);
        void Delete(long id);
        void Delete(long[] ids);
    }
}
