using CloudControl.Web.Tests.Mocks.Overrides;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CloudControl.Web.Tests.Mocks
{
    public class MockedWebApplicationMocks
    {
        private List<Action<IServiceCollection>> CustomRegisters { get; } = new List<Action<IServiceCollection>>();
        private List<(Type, Func<IServiceProvider, object>)> Singletons { get; } = new List<(Type, Func<IServiceProvider, object>)>();
        private List<(Type, Func<IServiceProvider, object>)> Scoped { get; } = new List<(Type, Func<IServiceProvider, object>)>();
        private List<(Type, Func<IServiceProvider, object>)> Transients { get; } = new List<(Type, Func<IServiceProvider, object>)>();

        public void ConfigureAdditionalServices(IServiceCollection services)
        {
            services.AddSingleton<IStartupFilter, StartupFilter>();

            foreach (var register in CustomRegisters)
            {
                register(services);
            }

            foreach (var (type, singleton) in Singletons)
            {
                services.AddSingleton(type, singleton);
            }

            foreach (var (type, service) in Scoped)
            {
                services.AddScoped(type, service);
            }

            foreach (var (type, service) in Transients)
            {
                services.AddTransient(type, service);
            }
        }

        public void AddCustomRegister(Action<IServiceCollection> register)
        {
            CustomRegisters.Add(register);
        }

        public void AddSingleton<T>(Func<IServiceProvider, T> func) where T : class
        {
            Singletons.Add((typeof(T), func));
        }

        public void AddSingleton<T>(T singleton) where T : class
        {
            AddSingleton(sp => singleton);
        }

        public void AddScoped<T>(Func<IServiceProvider, T> serviceFunc) where T : class
        {
            Scoped.Add((typeof(T), serviceFunc));
        }

        public void AddScoped<T>(T scoped) where T : class
        {
            AddScoped(sp => scoped);
        }

        public void AddTransient<T>(Func<IServiceProvider, T> serviceFunc) where T : class
        {
            Transients.Add((typeof(T), serviceFunc));
        }

        public void AddTransient<T>(T service) where T : class
        {
            AddTransient(sp => service);
        }
    }

    public class StartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<MockMiddleware>();
                next(app);
            };
        }
    }
}
