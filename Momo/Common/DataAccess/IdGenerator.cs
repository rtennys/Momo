using System;
using System.Reflection;

namespace Momo.Common.DataAccess
{
    public static class IdGenerator
    {
        static IdGenerator()
        {
            Initialize(GetNextHiDefaultImpl);
        }

        private static readonly object _lock = new object();
        private static Func<int> _getNextHi;
        private static int _nextHi;
        private static int _hi;
        private static int _lo;

        public static int StartingHi
        {
            get { return 2; } // skip a couple blocks of numbers to leave some reserved IDs
        }
        public static int MaxLo
        {
            get { return 0xf; }
        }
        public static int StartingId
        {
            get { return StartingHi * MaxLo + 1; }
        }

        public static void Initialize(Func<int> getNextHi)
        {
            _lo = MaxLo + 1; // always start out invalid so we grab the next available hi on the first request
            _getNextHi = getNextHi;
        }

        public static int NextId()
        {
            lock (_lock)
            {
                if (_lo > MaxLo)
                {
                    _hi = _getNextHi() * MaxLo;
                    _lo = 1;
                }

                return _hi + _lo++;
            }
        }

        public static void SetId<T>(T entity) where T : IEntity
        {
            PropertyInfo id = null;
            for (var type = typeof(T); type != null && id == null; type = type.BaseType)
                id = type.GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var value = entity is Entity<Guid>
                ? Guid.NewGuid()
                : Convert.ChangeType(NextId(), id.PropertyType);

            id.SetValue(entity, value, null);
        }

        private static int GetNextHiDefaultImpl()
        {
            return _nextHi++;
        }
    }
}