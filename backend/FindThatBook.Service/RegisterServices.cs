using FindThatBook.Service;
using FindThatBook.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace FindThatBook.Infrastructure.Service
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISearchService, SearchService>();
            return services;
        }
    }
}
