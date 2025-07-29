using Message.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Message.Infrastructure.Persistence;

public class DbConfig : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("messages").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.SenderId).HasColumnName("sender_id ");
        builder.Property(x => x.ReceiverId).HasColumnName("receiver_id");
        builder.Property(x => x.Type).HasColumnName("message_type");
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.ReadAt).HasColumnName("read_at");
    }
}