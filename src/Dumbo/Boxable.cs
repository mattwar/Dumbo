using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumbo
{
    /// <summary>
    /// Wouldn't it be nice if the runtime recogizned this interface and treated any
    /// struct implementing it similar to how it special cases Nullable?
    /// </summary>
    public interface IBoxable<TSelf>
    {
        object Box();
        abstract static TSelf Unbox(object boxed);
    }
}
