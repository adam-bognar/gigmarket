using AutoMapper;
using FluentValidation;
using GigMarket.Application.Common.Behaviors;
using GigMarket.Application.Common.Mappings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Services;

namespace GigMarket.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddScoped<IStripeWebhookService, StripeWebhookService>();
            return services;
        }
    }
}
