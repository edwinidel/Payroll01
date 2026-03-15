using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2FA.Data;
using _2FA.Data.Entities;
using _2FA.Models;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<NotificationDto>> GetAsync(int employeeId, bool unreadOnly, int page, int pageSize)
        {
            var query = _context.Notifications.AsNoTracking().Where(n => n.EmployeeId == employeeId);
            if (unreadOnly) query = query.Where(n => n.ReadAt == null);

            return await query
                .OrderByDescending(n => n.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto(n.Id, n.Type, n.Title, n.Body, n.Severity, n.Created, n.ReadAt, n.ExpiresAt))
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int employeeId, int notificationId)
        {
            var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.EmployeeId == employeeId);
            if (entity == null) return;
            entity.ReadAt ??= DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task EnqueueAsync(NotificationDto notification)
        {
            var entity = new NotificationEntity
            {
                EmployeeId = 0, // caller must map employeeId explicitly in future overload
                Type = notification.Type,
                Title = notification.Title,
                Body = notification.Body,
                Severity = notification.Severity,
                Created = DateTime.UtcNow,
                ExpiresAt = notification.ExpiresAt,
                Channel = "in-app"
            };
            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
