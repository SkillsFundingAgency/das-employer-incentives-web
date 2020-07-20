using AutoFixture;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests
{
    public class TestData
    {
        private readonly Dictionary<string, object> _testdata;
        private readonly Fixture _fixture;
        public TestData()
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

            if(!_testdata.ContainsKey(key))
            {
                return default;
            }

            return (T)_testdata[key];
        }

        public void Add<T>(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_testdata.ContainsKey(key))
            {
                throw new InvalidOperationException("key alrady exists");
            }

            _testdata.Add(key, value);
        }

        public T GetOrCreate<T>(string key = null, Func<T> onCreate = null)
        {
            if (key == null)
            {
                key = typeof(T).FullName;
            }

            if (!_testdata.ContainsKey(key))
            {
                if (onCreate == null)
                {
                    _testdata.Add(key, _fixture.Create<T>());
                }
                else
                {
                    _testdata.Add(key, onCreate.Invoke());
                }
            }

            return (T)_testdata[key];
        }
    }
}
