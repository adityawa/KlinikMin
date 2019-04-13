using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klinik.Interfaces
{
    public interface IValidator<T, TEntity> where T : class where TEntity : class
    {
        T Validate(TEntity request);
    }
}
