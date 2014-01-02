using System.Threading;

namespace DapperTesting.Core.Tests
{
    public abstract class TestBase
    {
        private static readonly object _lock = new object();

        protected void Start()
        {
            Monitor.Enter(_lock);
        }

        protected void End()
        {
            Monitor.Exit(_lock);
        }
    }
}
