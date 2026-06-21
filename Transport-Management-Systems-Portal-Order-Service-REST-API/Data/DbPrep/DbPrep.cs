using Microsoft.EntityFrameworkCore;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<TMSDbContext>()!, isProduction);
            }
        }

        private static void SeedData(TMSDbContext context, bool isProduction)
        {

            if (isProduction)
            {
                Console.WriteLine("Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex) { Console.WriteLine($"Migration failed: {ex.Message}"); }
            }

            Console.WriteLine("Seeding database with initial data...");

            // Order matters due to Foreign Key constraints
            SeedAddresses(context);
            SeedClients(context);
            SeedOrders(context);

            context.SaveChanges();
        }

        private static void SeedAddresses(TMSDbContext context)
        {
            if (context.Addresses.Any()) return;

            var addresses = new List<Address>
            {
                // Required Specific Addresses
                new Address { Line1 = "426 Bucknell Rd", City = "Costa Mesa", State = "CA", PostalCode = "92626", Country = "USA", Latitude = 33.664739, Longitude = -117.917458 },
                new Address { Line1 = "Christ Cathedral", City = "Garden Grove", State = "CA", PostalCode = "92840", Country = "USA", Latitude = 33.787942, Longitude = -117.89886 },
                
                // Random OC/Costa Mesa Vicinity
                new Address { Line1 = "3333 Bristol St (South Coast Plaza)", City = "Costa Mesa", State = "CA", PostalCode = "92626", Country = "USA", Latitude = 33.6913, Longitude = -117.8911 },
                new Address { Line1 = "100 Park Ave", City = "Newport Beach", State = "CA", PostalCode = "92662", Country = "USA", Latitude = 33.6081, Longitude = -117.8825 },
                new Address { Line1 = "1845 Newport Blvd", City = "Costa Mesa", State = "CA", PostalCode = "92627", Country = "USA", Latitude = 33.6421, Longitude = -117.9184 }
            };

            context.Addresses.AddRange(addresses);
            context.SaveChanges();
        }

        private static void SeedClients(TMSDbContext context)
        {
            if (context.Clients.Any()) return;

            context.Clients.AddRange(
                new Client { Name = "Global Logistics Corp", ContactEmail = "logistics@global.com", ContactPhone = "714-555-0101" },
                new Client { Name = "Tech Solutions Inc", ContactEmail = "shipping@techsol.com", ContactPhone = "949-555-0202" }
            );
            context.SaveChanges();
        }

        private static void SeedOrders(TMSDbContext context)
        {
            if (context.Orders.Any()) return;

            var client = context.Clients.First();
            var addresses = context.Addresses.ToList();

            var order = new Order
            {
                OrderNumber = "ORD-2024-001",
                ClientId = client.Id,
                ShipmentAddressId = addresses[0].Id, // Bucknell Rd
                DeliveryAddressId = addresses[1].Id, // Christ Cathedral
                Status = OrderStatus.InTransit,
                Priority = "High",
                CreatedAt = DateTime.UtcNow
            };

            context.Orders.Add(order);
            context.SaveChanges(); // Save to get Order ID for Tracking and Documents

            // Seed associated Tracking Events
            context.TrackingEvents.AddRange(
                new TrackingEvent
                {
                    OrderId = order.Id,
                    EventType = TrackingEventType.Created,
                    Description = "Order received in system",
                    Timestamp = DateTime.UtcNow.AddHours(-5)
                },
                new TrackingEvent
                {
                    OrderId = order.Id,
                    EventType = TrackingEventType.PickedUp,
                    Description = "Package picked up from Bucknell Rd",
                    Latitude = addresses[0].Latitude,
                    Longitude = addresses[0].Longitude,
                    Timestamp = DateTime.UtcNow.AddHours(-2)
                }
            );

            // Seed associated Documents
            // context.Documents.AddRange(
            //     new Document 
            //     { 
            //         OrderId = order.Id, 
            //         Type = DocumentType.Waybill, 
            //         FileName = "waybill_001.pdf", 
            //         StorageUrl = "https://tmsstorage.blob.core.windows.net/docs/waybill_001.pdf" 
            //     },
            //     new Document 
            //     { 
            //         OrderId = order.Id, 
            //         Type = DocumentType.Label, 
            //         FileName = "label_001.pdf", 
            //         StorageUrl = "https://tmsstorage.blob.core.windows.net/docs/label_001.pdf" 
            //     }
            // );
        }
    }
}