
using BloggingSystem.Application.Commons.Interfaces;
using BloggingSystem.Infrastructure.Data;
using BloggingSystem.Infrastructure.Services;
using BloggingSystem.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloggingSystem.Infrastructure
{
    /// <summary>
    /// Extensions for service collection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add infrastructure services
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<BloggingDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")),
                    b => b.MigrationsAssembly(typeof(BloggingDbContext).Assembly.FullName)));
            
            services.Configure<CloudinarySettings>( configuration.GetSection("CloudinarySettings"));
                    
            // Add repositories
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            
            // Add domain event service
            services.AddScoped<IDomainEventService, DomainEventService>();
            
            // Add notification services
            services.AddScoped<INotificationService, NotificationService>();
            
            // Register notification channels
            services.AddTransient<IStartupTask, NotificationChannelRegistrationTask>();
            
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<ISlugService, SlugService>();
            
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDateTime, DateTimeService>();
            
            services.Configure<S3Settings>(configuration.GetSection("S3"));
            services.AddScoped<IFileStorageService, S3StorageService>();

            // Đăng ký Adapter để chuyển từ ICloudinaryService sang IFileStorageService
            services.AddScoped<ICloudinaryService, CloudinaryToS3Adapter>();
            
            return services;
        }
        
    }
    
    /// <summary>
    /// Interface for startup tasks
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Execute startup task
        /// </summary>
        void Execute();
    }
    
    /// <summary>
    /// Startup task for registering notification channels
    /// </summary>
    public class NotificationChannelRegistrationTask : IStartupTask
    {
        private readonly INotificationService _notificationService;
        private readonly IEnumerable<INotificationChannel> _channels;
        
        public NotificationChannelRegistrationTask(
            INotificationService notificationService,
            IEnumerable<INotificationChannel> channels)
        {
            _notificationService = notificationService;
            _channels = channels;
        }
        
        /// <summary>
        /// Register all notification channels
        /// </summary>
        public void Execute()
        {
            foreach (var channel in _channels)
            {
                _notificationService.RegisterChannel(channel);
            }
        }
    }
}