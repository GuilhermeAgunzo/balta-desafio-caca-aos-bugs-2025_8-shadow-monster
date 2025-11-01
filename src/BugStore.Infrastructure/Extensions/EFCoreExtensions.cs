using BugStore.Application.Requests.Customers;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;

namespace BugStore.Infrastructure.Extensions;

public static class EFCoreExtensions
{
    public static IQueryable<Customer> FilterBy(this IQueryable<Customer> query, GetCustomersRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            query = query.Where(c => c.Name.Contains(req.Name));
        }

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            query = query.Where(c => c.Email.Contains(req.Email));
        }

        if (!string.IsNullOrWhiteSpace(req.Phone))
        {
            query = query.Where(c => c.Phone != null && c.Phone.Contains(req.Phone));
        }

        return query;
    }

    public static IQueryable<Product> FilterBy(this IQueryable<Product> query, GetProductsRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.Title))
        {
            query = query.Where(p => p.Title.Contains(req.Title));
        }

        if (!string.IsNullOrWhiteSpace(req.Description))
        {
            query = query.Where(p => p.Description.Contains(req.Description));
        }

        if (!string.IsNullOrWhiteSpace(req.Slug))
        {
            query = query.Where(p => p.Slug.Contains(req.Slug));
        }

        if (req.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= req.MinPrice.Value);
        }

        if (req.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= req.MaxPrice.Value);
        }

        return query;
    }

    public static IQueryable<Order> FilterBy(this IQueryable<Order> query, GetOrdersRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.CustomerName))
        {
            query = query.Where(o => o.Customer.Name.Contains(req.CustomerName));
        }

        if (!string.IsNullOrWhiteSpace(req.CustomerEmail))
        {
            query = query.Where(o => o.Customer.Email.Contains(req.CustomerEmail));
        }

        if (!string.IsNullOrWhiteSpace(req.CustomerPhone))
        {
            query = query.Where(o => o.Customer.Phone != null && o.Customer.Phone.Contains(req.CustomerPhone));
        }

        if (!string.IsNullOrWhiteSpace(req.ProductTitle))
        {
            query = query.Where(o => o.Lines.Any(ol => ol.Product.Title.Contains(req.ProductTitle)));
        }

        if (!string.IsNullOrWhiteSpace(req.ProductDescription))
        {
            query = query.Where(o => o.Lines.Any(ol => ol.Product.Description.Contains(req.ProductDescription)));
        }

        if (!string.IsNullOrWhiteSpace(req.ProductSlug))
        {
            query = query.Where(o => o.Lines.Any(ol => ol.Product.Slug.Contains(req.ProductSlug)));
        }

        if (req.ProductPriceStart.HasValue)
        {
            query = query.Where(o => o.Lines.Any(ol => ol.Product.Price >= req.ProductPriceStart.Value));
        }

        if (req.ProductPriceEnd.HasValue)
        {
            query = query.Where(o => o.Lines.Any(ol => ol.Product.Price <= req.ProductPriceEnd.Value));
        }

        if (req.CreatedAtStart.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= req.CreatedAtStart.Value);
        }

        if (req.CreatedAtEnd.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= req.CreatedAtEnd.Value);
        }

        if (req.UpdatedAtStart.HasValue)
        {
            query = query.Where(o => o.UpdatedAt >= req.UpdatedAtStart.Value);
        }

        if (req.UpdatedAtEnd.HasValue)
        {
            query = query.Where(o => o.UpdatedAt <= req.UpdatedAtEnd.Value);
        }

        return query;
    }
}
