using ChatApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
