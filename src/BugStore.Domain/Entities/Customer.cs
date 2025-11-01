using Flunt.Notifications;
using Flunt.Validations;

namespace BugStore.Domain.Entities;

public class Customer : Notifiable<Notification>
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public DateTime BirthDate { get; private set; }

#pragma warning disable CS8618
    public Customer() { } // EF Core constructor
#pragma warning restore CS8618

    private Customer(string name, string email, DateTime birthDate, string? phone = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;

        Validate();
    }

    public static (Customer? customer, IReadOnlyCollection<Notification> errors) Create(string name, string email, DateTime birthDate, string? phone = null)
    {
        var customer = new Customer(name, email, birthDate, phone);
        return (
            customer: customer.IsValid ? customer : null,
            errors: customer.Notifications);
    }

    private void Validate()
    {
        Clear();
        AddNotifications(new Contract<Customer>()
            .Requires()
            .IsNotNullOrEmpty(val: Name, key: "Customer.Name", message: "Name is required")
            .IsNotNullOrEmpty(val: Email, key: "Customer.Email", message: "Email is required")
            .IsGreaterOrEqualsThan(val: BirthDate, comparer: DateTime.MinValue, key: "birthDate", message: "Birth Date is required"));
    }
}