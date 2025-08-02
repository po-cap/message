using Message.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Message.Infrastructure.Persistence;

public class DbConfig : 
    IEntityTypeConfiguration<Note>, 
    IEntityTypeConfiguration<User>,
    IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("messages").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ItemId).HasColumnName("item_id");
        builder.Property(x => x.SenderId).HasColumnName("sender_id");
        builder.Property(x => x.ReceiverId).HasColumnName("receiver_id");
        builder.Property(x => x.Type).HasColumnName("message_type");
        builder.Property(x => x.Content).HasColumnName("content");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.GetAt).HasColumnName("get_at");
        builder.Property(x => x.ReadAt).HasColumnName("read_at");
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Avatar).HasColumnName("avatar");
        builder.Property(x => x.DisplayName).HasColumnName("display_name");
    }

    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Albums).HasColumnName("albums");
        builder.Property(x => x.Specs).HasColumnName("spec").HasColumnType("jsonb");
        builder.HasOne(x => x.User).WithMany().HasForeignKey("user_id");
    }
}
