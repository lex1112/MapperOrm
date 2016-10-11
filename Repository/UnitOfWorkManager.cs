using System;
using System.Threading;

namespace MapperOrm.Repository
{
    public static class UnitOfWorkManager
    {
        private static readonly ThreadLocal<IUnitOfWork> CurrentThreadData = new ThreadLocal<IUnitOfWork>(true);
  
        public static IUnitOfWork Current
        {
            get { return CurrentThreadData.Value; }
            private set { CurrentThreadData.Value = value; }
        }

        public static IUnitOfWork Create(IUnitOfWork uow)
        {
            return Current ?? (Current = uow);
        }

        internal static void OnDisponse(object sender, EventArgs e)
        {
            CurrentThreadData.Values.Remove(CurrentThreadData.Value);
            CurrentThreadData.Value = null;
        }
    }
}
