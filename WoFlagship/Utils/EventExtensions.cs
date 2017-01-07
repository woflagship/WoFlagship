using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils
{
    public static class EventExtensions
    {
        /// <summary>
        /// 对action中的所有元素都完成调用;
        /// 每一个元素产生的异常将会在所有元素调用完成后一起抛出AggregateException;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="arg"></param>
        public static void InvokeAll<T>(this Action<T> action, T arg)
        {
            if (action == null)
                return;
            List<Exception> exceptions = new List<Exception>();
            foreach (Action<T> singleAction in action.GetInvocationList())
            {
                try { singleAction.Invoke(arg); } catch (Exception ex) { exceptions.Add(ex); }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// 对action中的所有元素都完成调用;
        /// 每一个元素产生的异常将会在所有元素调用完成后一起抛出AggregateException;
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="action"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public static void InvokeAll<T1,T2>(this Action<T1,T2> action, T1 arg1, T2 arg2)
        {
            if (action == null)
                return;
            List<Exception> exceptions = new List<Exception>();
            foreach (Action<T1,T2> singleAction in action.GetInvocationList())
            {
                try { singleAction.Invoke(arg1,arg2); } catch (Exception ex) { exceptions.Add(ex); }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// 对action中的所有元素都完成调用;
        /// 每一个元素产生的异常将会在所有元素调用完成后一起抛出AggregateException;
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="action"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void InvokeAll<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action == null)
                return;
            List<Exception> exceptions = new List<Exception>();
            foreach (Action<T1, T2, T3> singleAction in action.GetInvocationList())
            {
                try { singleAction.Invoke(arg1, arg2, arg3); } catch (Exception ex) { exceptions.Add(ex); }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
