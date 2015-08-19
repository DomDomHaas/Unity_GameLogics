namespace FullInspector.Internal
{
    using System;
    using System.Collections.Generic;

    internal static class SingletonCache
    {
        private static Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public static T Get<T>()
        {
            object obj2;
            if (!_instances.TryGetValue(typeof(T), out obj2))
            {
                obj2 = Activator.CreateInstance<T>();
                _instances[typeof(T)] = obj2;
            }
            return (T) obj2;
        }
    }
}

