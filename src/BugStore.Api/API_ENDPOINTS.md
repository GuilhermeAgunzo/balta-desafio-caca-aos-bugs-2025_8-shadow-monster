# BugStore API Endpoints

This document lists all the minimal API endpoints created for the BugStore application.

## Base URL
- Development: `https://localhost:{port}`
- Swagger UI: `https://localhost:{port}/swagger`

## Customers Endpoints

### Create Customer
- **POST** `/api/customers`
- **Body:**
```json
{
  "name": "string",
  "email": "string",
  "phone": "string (optional)",
  "birthDate": "2024-01-01T00:00:00Z"
}
```
- **Response:** `201 Created` with customer data or `400 Bad Request`

### Get All Customers (Paginated with Filters)
- **GET** `/api/customers`
- **Query Parameters:**
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `name` (string, optional) - Filter by name (contains)
  - `email` (string, optional) - Filter by email (contains)
  - `phone` (string, optional) - Filter by phone (contains)
- **Response:** `200 OK` with paged customer list or `400 Bad Request`

### Get Customer By ID
- **GET** `/api/customers/{id}`
- **Path Parameters:**
  - `id` (guid) - Customer ID
- **Response:** `200 OK` with customer data or `404 Not Found`

### Get Customer By Email
- **GET** `/api/customers/by-email/{email}`
- **Path Parameters:**
  - `email` (string) - Customer email
- **Response:** `200 OK` with customer data or `404 Not Found`

### Update Customer
- **PUT** `/api/customers/{id}`
- **Path Parameters:**
  - `id` (guid) - Customer ID
- **Body:**
```json
{
  "name": "string",
  "email": "string",
  "phone": "string (optional)",
  "birthDate": "2024-01-01T00:00:00Z"
}
```
- **Response:** `200 OK` with updated customer data or `400 Bad Request`

### Delete Customer
- **DELETE** `/api/customers/{id}`
- **Path Parameters:**
  - `id` (guid) - Customer ID
- **Response:** `204 No Content` or `400 Bad Request`

---

## Orders Endpoints

### Create Order
- **POST** `/api/orders`
- **Body:**
```json
{
  "customerId": "guid",
  "orderLines": [
    {
      "productId": "guid",
      "quantity": 1
    }
  ]
}
```
- **Response:** `201 Created` with order data or `400 Bad Request`

### Get All Orders (Paginated with Filters)
- **GET** `/api/orders`
- **Query Parameters:**
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `customerName` (string, optional) - Filter by customer name (contains)
  - `customerEmail` (string, optional) - Filter by customer email (contains)
  - `customerPhone` (string, optional) - Filter by customer phone (contains)
  - `productTitle` (string, optional) - Filter by product title (contains)
  - `productDescription` (string, optional) - Filter by product description (contains)
  - `productSlug` (string, optional) - Filter by product slug (contains)
  - `productPriceStart` (decimal, optional) - Filter by minimum product price
  - `productPriceEnd` (decimal, optional) - Filter by maximum product price
  - `createdAtStart` (datetime, optional) - Filter by minimum creation date
  - `createdAtEnd` (datetime, optional) - Filter by maximum creation date
  - `updatedAtStart` (datetime, optional) - Filter by minimum update date
- `updatedAtEnd` (datetime, optional) - Filter by maximum update date
- **Response:** `200 OK` with order list or `400 Bad Request`

### Get Order By ID
- **GET** `/api/orders/{id}`
- **Path Parameters:**
  - `id` (guid) - Order ID
- **Response:** `200 OK` with order data or `404 Not Found`

### Get Orders By Customer
- **GET** `/api/orders/by-customer/{customerId}`
- **Path Parameters:**
  - `customerId` (guid) - Customer ID
- **Query Parameters:**
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
- **Response:** `200 OK` with order list or `400 Bad Request`

---

## Products Endpoints

### Create Product
- **POST** `/api/products`
- **Body:**
```json
{
  "title": "string",
  "description": "string",
  "price": 0.00
}
```
- **Response:** `201 Created` with product data or `400 Bad Request`

### Get All Products (Paginated with Filters)
- **GET** `/api/products`
- **Query Parameters:**
  - `pageNumber` (int, default: 1)
  - `pageSize` (int, default: 10)
  - `title` (string, optional) - Filter by title (contains)
  - `description` (string, optional) - Filter by description (contains)
  - `slug` (string, optional) - Filter by slug (contains)
  - `priceStart` (decimal, optional) - Filter by minimum price
  - `priceEnd` (decimal, optional) - Filter by maximum price
- **Response:** `200 OK` with paged product list or `400 Bad Request`

### Get Product By ID
- **GET** `/api/products/{id}`
- **Path Parameters:**
  - `id` (guid) - Product ID
- **Response:** `200 OK` with product data or `404 Not Found`

### Update Product
- **PUT** `/api/products/{id}`
- **Path Parameters:**
  - `id` (guid) - Product ID
- **Body:**
```json
{
  "title": "string",
  "description": "string",
  "price": 0.00
}
```
- **Response:** `200 OK` with updated product data or `400 Bad Request`

### Delete Product
- **DELETE** `/api/products/{id}`
- **Path Parameters:**
  - `id` (guid) - Product ID
- **Response:** `204 No Content` or `400 Bad Request`

---

## Reports Endpoints

### Get Customer Order Summary Report
- **GET** `/api/reports/customer-order-summary`
- **Description:** Returns a summary of orders grouped by customer, including total orders, quantity of products, and total spent.
- **Response:** `200 OK` with report data or `400 Bad Request`
- **Response Format:**
```json
{
  "data": [
    {
      "customerId": "guid",
      "customerName": "string",
      "totalOrders": 0,
      "qtProducts": 0,
      "totalSpent": 0.00
}
  ]
}
```

### Get Revenue By Period Report
- **GET** `/api/reports/revenue-by-period`
- **Query Parameters:**
  - `startDate` (datetime, required) - Start date for the report
  - `endDate` (datetime, required) - End date for the report
- **Description:** Returns revenue grouped by year and month within the specified period.
- **Response:** `200 OK` with report data or `400 Bad Request`
- **Response Format:**
```json
{
  "data": [
    {
      "year": "string",
      "month": "string",
      "totalOrders": 0,
      "totalRevenue": 0.00
    }
  ]
}
```

---

## Response Formats

### Standard Response
```json
{
  "data": {},
  "messages": ["string"],
  "statusCode": 200,
  "isSuccess": true
}
```

### Paged Response
```json
{
  "data": [],
  "messages": ["string"],
  "statusCode": 200,
  "isSuccess": true,
  "totalCount": 0,
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 0
}
```

---

## Handler to Endpoint Mapping

| Handler | HTTP Method | Endpoint | Purpose |
|---------|------------|----------|---------|
| ICreateCustomerHandler | POST | /api/customers | Create new customer |
| IGetCustomersHandler | GET | /api/customers | List customers with pagination and filters |
| IGetCustomerByIdHandler | GET | /api/customers/{id} | Get customer by ID |
| IGetCustomerByEmailHandler | GET | /api/customers/by-email/{email} | Get customer by email |
| IUpdateCustomerHandler | PUT | /api/customers/{id} | Update customer |
| IDeleteCustomerHandler | DELETE | /api/customers/{id} | Delete customer |
| ICreateOrderHandler | POST | /api/orders | Create new order |
| IGetOrdersHandler | GET | /api/orders | List orders with pagination and filters |
| IGetOrderByIdHandler | GET | /api/orders/{id} | Get order by ID |
| IGetOrdersByCustomerHandler | GET | /api/orders/by-customer/{customerId} | List orders by customer |
| ICreateProductHandler | POST | /api/products | Create new product |
| IGetProductsHandler | GET | /api/products | List products with pagination and filters |
| IGetProductByIdHandler | GET | /api/products/{id} | Get product by ID |
| IUpdateProductHandler | PUT | /api/products/{id} | Update product |
| IDeleteProductHandler | DELETE | /api/products/{id} | Delete product |
| ICustomerOrderSummaryReportHandler | GET | /api/reports/customer-order-summary | Generate customer order summary report |
| IRevenueByPeriodReportHandler | GET | /api/reports/revenue-by-period | Generate revenue by period report |

---

## Notes

- All endpoints support JSON content type
- GUIDs should be in standard format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- DateTime fields should be in ISO 8601 format: `2024-01-01T00:00:00Z`
- All endpoints include proper HTTP status codes (200, 201, 204, 400, 404)
- Swagger UI is available in development mode for interactive API testing
- All list endpoints support pagination with `pageNumber` and `pageSize` query parameters
- Filter parameters are optional and use case-insensitive "contains" matching where applicable
