using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.StudyPlanner;

public static class StudyPlannerModuleExtensions
{
    public static IServiceCollection AddDayDashStudyPlanner(this IServiceCollection services)
    {
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IStudyPlannerService, StudyPlannerService>();
        return services;
    }
}