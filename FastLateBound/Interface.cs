using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastLateBoundGenerics
{
    public interface IPerson
    {
        void Purchase(IDog p);

        IDog Walk(IDog p);
    }

    public interface IDog
    {
    }
}
