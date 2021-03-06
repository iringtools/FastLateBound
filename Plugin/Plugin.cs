﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastLateBoundGenerics;
using System.ComponentModel.Composition;

namespace Plugin
{
    [Export(typeof(IDog))]
    public class Shadow : IDog
    {
    }

    [Export(typeof(IPerson))]
    public class Rob : IPerson
    {
        private IDog owner;

        public void Purchase(IDog p)
        {
            owner = p;
        }
        public IDog Walk(IDog p)
        {
            return p;
        }
    }
}
