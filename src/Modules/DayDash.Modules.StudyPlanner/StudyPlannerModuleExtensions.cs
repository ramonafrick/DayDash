using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Infrastructure;
using DayDash.Modules.StudyPlanner.Infrastructure.Persistence;
using DayDash.Modules.StudyPlanner.Infrastructure.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.StudyPlanner;

public static class StudyPlannerModuleExtensions
{
    public static IServiceCollection AddDayDashStudyPlanner(this IServiceCollection services)
    {
        services.AddSingleton<IModelConfiguration, StudyPlannerModelConfiguration>();
        services.AddScoped<IDataSeeder, SubjectConfigSeeder>();

        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<ISubjectConfigRepository, SubjectConfigRepository>();
        services.AddScoped<IStudyPlannerService, StudyPlannerService>();

        services.AddLocalization();
        return services;
    }
}
