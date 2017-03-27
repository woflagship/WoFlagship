using System;

namespace WoFlagship.KancolleAI
{
    [Serializable]
    class UnExpectedSceneException : Exception
    {
        public UnExpectedSceneException() : base() { }

        public UnExpectedSceneException(string message) : base(message) { }
    }
}
