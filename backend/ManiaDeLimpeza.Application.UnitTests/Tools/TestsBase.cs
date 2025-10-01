using ManiaDeLimpeza.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.UnitTests.Tools
{
    public abstract class TestsBase
    {
        protected IServiceProvider ServiceProvider = null!;

        [TestInitialize]
        public void InitializeTestBase()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(DefaultMapperProfile).Assembly);
            ServiceProvider = services.BuildServiceProvider();
        }

        protected T GetRequiredService<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();
    }
}
