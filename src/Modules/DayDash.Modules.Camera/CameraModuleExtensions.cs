using Microsoft.Extensions.DependencyInjection;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Camera.Application.Services;

namespace DayDash.Modules.Camera;

public static class CameraModuleExtensions
{
    public static IServiceCollection AddDayDashCamera(this IServiceCollection services)
    {
        services.AddScoped<ILearningGoalParser, LearningGoalParser>();
        return services;
    }
}