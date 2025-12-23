using FindThatBook.Infrastructure.Service;
using FindThatBook.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FindThatBook.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services) {
            services.AddScoped<ILLMClient, GeminiClient>();
            services.AddScoped<IOpenLibraryClient, OpenLibraryClient>();
            return services;
        }
    }
}
