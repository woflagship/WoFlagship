using System;

namespace WoFlagship.KancolleAI
{
    class UnExpectedSceneException : Exception
    {
        public UnExpectedSceneException() : base() { }

        public UnExpectedSceneException(string message) : base(message) { }
    }
}
