using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Handlers.Orders;
using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Abstractions.Handlers.Reports;
using BugStore.Application.Handlers.Customers;
using BugStore.Application.Handlers.Orders;
using BugStore.Application.Handlers.Products;
using BugStore.Application.Handlers.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace BugStore.Application;

public static class ApplicationDependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICreateCustomerHandler, CreateCustomerHandler>();
        services.AddScoped<IDeleteCustomerHandler, DeleteCustomerHandler>();
        services.AddScoped<IGetCustomerByEmailHandler, GetCustomerByEmailHandler>();
        services.AddScoped<IGetCustomerByIdHandler, GetCustomerByIdHandler>();
        services.AddScoped<IGetCustomersHandler, GetCustomersHandler>();
        services.AddScoped<IUpdateCustomerHandler, UpdateCustomerHandler>();

        services.AddScoped<ICreateOrderHandler, CreateOrderHandler>();
        services.AddScoped<IGetOrderByIdHandler, GetOrderByIdHandler>();
        services.AddScoped<IGetOrdersByCustomerHandler, GetOrdersByCustomerHandler>();
        services.AddScoped<IGetOrdersHandler, GetOrdersHandler>();

        services.AddScoped<ICreateProductHandler, CreateProductHandler>();
        services.AddScoped<IDeleteProductHandler, DeleteProductHandler>();
        services.AddScoped<IGetProductByIdHandler, GetProductByIdHandler>();
        services.AddScoped<IGetProductsHandler, GetProductsHandler>();
        services.AddScoped<IUpdateProductHandler, UpdateProductHandler>();

        services.AddScoped<ICustomerOrderSummaryReportHandler, CustomerOrderSummaryReportHandler>();
        services.AddScoped<IRevenueByPeriodReportHandler, RevenueByPeriodReportHandler>();
    }
}