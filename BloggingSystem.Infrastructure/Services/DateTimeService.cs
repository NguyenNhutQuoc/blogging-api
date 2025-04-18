using System;
using BloggingSystem.Application.Commons.Interfaces;

namespace BloggingSystem.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}