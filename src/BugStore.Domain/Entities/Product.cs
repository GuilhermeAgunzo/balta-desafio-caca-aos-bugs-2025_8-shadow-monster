using Flunt.Notifications;
using Flunt.Validations;

namespace BugStore.Domain.Entities;

public class Product : Notifiable<Notification>
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Slug { get; private set; }
    public decimal Price { get; private set; }

#pragma warning disable CS8618
    public Product() { } // For EF Core
#pragma warning restore CS8618

    private Product(string title, string description, decimal price, string? slug = null)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
        Slug = slug ?? GenerateSlug(title);

        Validate();
    }

    public static (Product? product, IReadOnlyCollection<Notification> errors) Create(string title, string description, decimal price, string? slug = null)
    {
        var product = new Product(title, description, price, slug);
        return (product.IsValid ? product : null, product.Notifications);
    }

    private void Validate()
    {
        Clear();
        AddNotifications(new Contract<Product>()
            .Requires()
            .IsNotNullOrEmpty(Title, "Product.Title", "Title is required")
            .IsNotNullOrEmpty(Description, "Product.Description", "Description is required")
            .IsGreaterThan(Price, 0, "Product.Price", "Price must be greater than zero")
            .IsNotNullOrEmpty(Slug, "Product.Slug", "Slug is required"));
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower().Replace(" ", "-").Replace(".", "").Replace(",", "");
    }
}