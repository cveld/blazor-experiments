using BlazoredMaterialDesignModal.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazoredMaterialDesignModal
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazoredModal(this IServiceCollection services)
        {
            return services.AddScoped<IModalService, ModalService>();
        }
    }
}
