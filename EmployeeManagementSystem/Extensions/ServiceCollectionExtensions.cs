using EmployeeManagement.Application.Interfaces.Repositories;
using EmployeeManagement.Application.Interfaces.Services;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Email;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Infrastructure.Security;

namespace EmployeeManagementSystem.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailOtpRepository, EmailOtpRepository>();

        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}