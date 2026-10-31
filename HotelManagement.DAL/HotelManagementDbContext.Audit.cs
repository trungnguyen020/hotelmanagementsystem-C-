using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.DAL
{
    public partial class HotelManagementDbContext
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CapNhatAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            CapNhatAuditTimestamps();
            return base.SaveChanges();
        }

        private void CapNhatAuditTimestamps()
        {
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    var propCreatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                    if (propCreatedAt != null && (propCreatedAt.CurrentValue == null || (DateTime)propCreatedAt.CurrentValue == default))
                    {
                        propCreatedAt.CurrentValue = now;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    var propUpdatedAt = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                    if (propUpdatedAt != null)
                    {
                        propUpdatedAt.CurrentValue = now;
                    }
                }
            }
        }
    }
}

