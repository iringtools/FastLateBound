using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FastLateBoundGenerics
{
    class InterfaceWrapper
    {
        private static KnownT CreateNewObject<T, KnownT>() where T : KnownT, new()
        {
            return new T();
        }

        /// <summary>
        /// Creates static creator delegate for specified type
        /// </summary>
        /// <typeparam name="T2">known parent type of specified type</typeparam>
        /// <param name="T">specified type to create</param>
        /// <returns>delegate</returns>
        public static Func<T2> MakeNewDelegate<T2>(Type T)
        {
            MethodInfo methodInfo = typeof(InterfaceWrapper).GetMethod(
                    "CreateNewObject",
                    BindingFlags.NonPublic | BindingFlags.Static
                );

            MethodInfo genericMethod = methodInfo.MakeGenericMethod(
                    (new Type[] { T, typeof(T2) })
                );

            return Delegate.CreateDelegate(typeof(Func<T2>), genericMethod) as Func<T2>;
        }

        private static Action<knownT, knownT2> Purchase1<T, T2, knownT, knownT2>(MethodInfo method)
            where T : knownT
            where T2 : knownT2
        {
            // create first delegate. It is not fine because its 
            // signature contains unknown types T and T2
            Action<T, T2> d = (Action<T, T2>)Delegate.CreateDelegate(
                    typeof(Action<T, T2>),
                    method
            );

            // create another delegate having necessary signature. 
            // It encapsulates first delegate with a closure
            return delegate (knownT target, knownT2 p) { d((T)target, (T2)p); };
        }

        private static Func<knownT, knownT2, knownR> Walk1<T, T2, knownT, knownT2, knownR>(MethodInfo method)
        where T : knownT
        where T2 : knownT2
        {
            Func<T, T2, knownR> d = (Func<T, T2, knownR>)Delegate.CreateDelegate(typeof(Func<T, T2, knownR>), method);

            return delegate (knownT target, knownT2 p) { return d((T)target, (T2)p); };
        }

        /// <summary>
        /// Creates static caller delegate for specified method
        /// </summary>
        /// <typeparam name="T">signature of the delegate</typeparam>
        /// <param name="method">method to surround</param>
        /// <returns>caller delegate with specified signature</returns>
        public static T MakeCallDelegate<T>(MethodInfo method)
        {
            // we're going to select necessary generic function 
            // and parameterize it with specified signature

            // 1. select function name accordingly to parameters count

            string methodName = method.Name + method.GetParameters().Length.ToString();

            // 2. create parametrization signature
            List<Type> signature = new List<Type>();
            // first type parameter is type of target object
            signature.Add(method.DeclaringType);

            //next parameters are real types of method arguments
            foreach (ParameterInfo pi in method.GetParameters())
            {
                signature.Add(pi.ParameterType);
            }

            // last parameters are known types of method arguments
            signature.AddRange(typeof(T).GetGenericArguments());

            // 3. call generator function with Delegate.Invoke. 
            // We can do it because the generator will be called only once. 
            // Result will be cached somewhere then.
            MethodInfo methodInfo = typeof(InterfaceWrapper).GetMethod(
                    methodName,
                    BindingFlags.NonPublic | BindingFlags.Static
                );

            MethodInfo genericMethod = methodInfo.MakeGenericMethod(signature.ToArray());

            return (T)genericMethod.Invoke(null, new object[] { method });
        }
    }
}
