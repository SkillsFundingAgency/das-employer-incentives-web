using AutoFixture;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public class TestDataStore
    {
        private readonly Dictionary<string, object> _testdata;
        private readonly Fixture _fixture;
        public TestDataStore()
        {
            _testdata = new Dictionary<string, object>();
            _fixture = new Fixture();
        }

        public T Get<T>(string key = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if(!_testdata.ContainsKey(key.ToLowerInvariant()))
            {
                return default;
            }

            return (T)_testdata[key.ToLowerInvariant()];
        }

        public void Add<T>(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_testdata.ContainsKey(key.ToLowerInvariant()))
            {
                throw new InvalidOperationException("key already exists");
            }

            _testdata.Add(key.ToLowerInvariant(), value);
        }

        public T GetOrCreate<T>(string key = null, Func<T> onCreate = null)
        {
            if (key == null)
            {
                key = typeof(T).FullName;
            }

            if (!_testdata.ContainsKey(key.ToLowerInvariant()))
            {
                if (onCreate == null)
                {
                    _testdata.Add(key.ToLowerInvariant(), _fixture.Create<T>());
                }
                else
                {
                    _testdata.Add(key.ToLowerInvariant(), onCreate.Invoke());
                }
            }

            return (T)_testdata[key.ToLowerInvariant()];
        }
    }
}
