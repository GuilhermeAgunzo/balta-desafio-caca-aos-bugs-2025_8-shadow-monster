using BugStore.Application.Abstractions.Handlers.Reports;
using BugStore.Application.Requests.Reports;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
   var group = app.MapGroup("/api/reports")
        .WithTags("Reports");

     group.MapGet("customer-order-summary", async (
    [FromServices] ICustomerOrderSummaryReportHandler handler,
            CancellationToken cancellationToken) =>
  {
            var response = await handler.HandleAsync(cancellationToken);
         return response.IsSuccess
     ? Results.Ok(response)
     : Results.BadRequest(response);
        })
        .WithName("GetCustomerOrderSummaryReport");

        group.MapGet("revenue-by-period", async (
         [FromQuery] DateTime startDate,
  [FromQuery] DateTime endDate,
            [FromServices] IRevenueByPeriodReportHandler handler,
          CancellationToken cancellationToken) =>
        {
   var request = new RevenueByPeriodReportRequest 
            { 
     StartDate = startDate,
             EndDate = endDate
 };
          var response = await handler.HandleAsync(request, cancellationToken);
            return response.IsSuccess
        ? Results.Ok(response)
       : Results.BadRequest(response);
        })
        .WithName("GetRevenueByPeriodReport");
    }
}
