using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleAI
{
    class UnExpectedSceneException : Exception
    {
        public UnExpectedSceneException() : base() { }

        public UnExpectedSceneException(string message) : base(message) { }
    }
}
