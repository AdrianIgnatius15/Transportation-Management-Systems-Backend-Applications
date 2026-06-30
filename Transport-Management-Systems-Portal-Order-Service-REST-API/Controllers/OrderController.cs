using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Order;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Pagination;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Piece;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Service.Interface;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Utilities;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _repo;
        private readonly IDocumentStorageService _documentStorage;

        public OrderController(IOrderRepo repo, IDocumentStorageService documentStorage)
        {
            _repo = repo;
            _documentStorage = documentStorage;
        }

        [HttpGet("emailPagination")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult<PaginatedResult<OrderReadDto>>> GetAllOrdersByClientEmail(
            [FromQuery] PaginationOrderSearchParameters parameters
        )
        {
            if (parameters.Email == "" || parameters.Email == null)
            {
                return BadRequest("Email of the shipper is required!");
            }
            else
            {
                string shipperEmail = parameters.Email;
                if (parameters.PageNumber == 0 && parameters.PageSize == 0)
                {
                    parameters = new PaginationOrderSearchParameters();
                }

                Console.WriteLine("Email", shipperEmail);
                var pagedOrders = await _repo.GetAllOrdersByClientEmailWithPagination(
                    shipperEmail,
                    parameters
                );

                var orderReadDtos = pagedOrders
                    .Items.Select(o =>
                    {
                        var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(o);
                        if (o.Client != null)
                            orderReadDto.Client = MapperUtility.Map<Client, ClientReadDto>(
                                o.Client
                            );
                        if (o.ShipmentAddress != null)
                            orderReadDto.ShipmentAddress = MapperUtility.Map<
                                Address,
                                AddressReadDto
                            >(o.ShipmentAddress);
                        if (o.DeliveryAddress != null)
                            orderReadDto.DeliveryAddress = MapperUtility.Map<
                                Address,
                                AddressReadDto
                            >(o.DeliveryAddress);
                        return orderReadDto;
                    })
                    .ToList();

                var result = new PaginatedResult<OrderReadDto>(
                    orderReadDtos,
                    pagedOrders.TotalCount,
                    pagedOrders.PageNumber,
                    pagedOrders.PageSize
                );

                return Ok(result);
            }
        }

        [HttpGet("all/shipperid")]
        [Authorize(Roles = "shipper,receiver")]
        public async Task<
            ActionResult<PaginatedResult<OrderReadDto>>
        > GetAllOrdersWithShipperIdPaginated([FromQuery] PaginationOrderSearchParameters parameters)
        {
            if (!parameters.Id.HasValue)
            {
                return BadRequest("Provide the shipper's unique ID found in the account details!");
            }
            else
            {
                Guid id = parameters.Id.Value;
                if (parameters.PageNumber == 0 && parameters.PageSize == 0)
                {
                    parameters = new PaginationOrderSearchParameters();
                }

                var ordersPaginated = await _repo.GetAllOrdersByClientIdWithPagination(
                    id,
                    parameters
                );

                var orderReadDtos = ordersPaginated
                    .Items.Select(o =>
                    {
                        var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(o);
                        if (o.Client != null)
                            orderReadDto.Client = MapperUtility.Map<Client, ClientReadDto>(
                                o.Client
                            );
                        if (o.ShipmentAddress != null)
                            orderReadDto.ShipmentAddress = MapperUtility.Map<
                                Address,
                                AddressReadDto
                            >(o.ShipmentAddress);
                        if (o.DeliveryAddress != null)
                            orderReadDto.DeliveryAddress = MapperUtility.Map<
                                Address,
                                AddressReadDto
                            >(o.DeliveryAddress);
                        return orderReadDto;
                    })
                    .ToList();

                var result = new PaginatedResult<OrderReadDto>(
                    orderReadDtos,
                    ordersPaginated.TotalCount,
                    ordersPaginated.PageNumber,
                    ordersPaginated.PageSize
                );

                return Ok(result);
            }
        }

        [HttpPost("all")]
        [Authorize(Roles = "shipper,receiver")]
        public async Task<ActionResult<PaginatedResult<OrderReadDto>>> GetAllOrders(
            [FromBody] PaginationParameters parameters
        )
        {
            if (parameters == null)
            {
                parameters = new PaginationParameters();
            }

            var pagedAllOrders = await _repo.GetAllOrdersWithPagination(parameters);

            var orderReadDtos = pagedAllOrders
                .Items.Select(order =>
                {
                    var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(order);
                    return orderReadDto;
                })
                .ToList();

            return Ok(
                new PaginatedResult<OrderReadDto>(
                    orderReadDtos,
                    pagedAllOrders.TotalCount,
                    pagedAllOrders.PageNumber,
                    pagedAllOrders.PageSize
                )
            );
        }

        [HttpPost("create")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult<string>> CreateOrder(
            [FromBody] OrderCreateDto orderCreateDto
        )
        {
            if (orderCreateDto == null)
            {
                return BadRequest("Order details are empty");
            }

            if (!ModelState.IsValid)
            {
                var validationResults = new List<ValidationResult>();

                return BadRequest(
                    new { errors = validationResults.Select(v => v.MemberNames).ToList() }
                );
            }
            else
            {
                // Map DTOs to entities
                var shipmentAddress = MapperUtility.Map<AddressCreateDto, Address>(
                    orderCreateDto.ShipmentAddress
                );
                var deliveryAddress = MapperUtility.Map<AddressCreateDto, Address>(
                    orderCreateDto.DeliveryAddress
                );

                // Create the order with mapped addresses
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    ClientId = orderCreateDto.ClientId,
                    OrderNumber = Guid.NewGuid().ToString(),
                    Status = orderCreateDto.Status,
                    Priority = orderCreateDto.Priority,
                    ShipmentAddress = shipmentAddress,
                    DeliveryAddress = deliveryAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                // Map and add shipments with their pieces
                if (orderCreateDto.Shipments != null && orderCreateDto.Shipments.Count > 0)
                {
                    foreach (var shipmentDto in orderCreateDto.Shipments)
                    {
                        var shipment = new Shipment
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            Pieces = new List<Piece>(),
                        };

                        // Map and add pieces to the shipment
                        if (shipmentDto.Pieces != null && shipmentDto.Pieces.Count > 0)
                        {
                            foreach (var pieceDto in shipmentDto.Pieces)
                            {
                                var piece = MapperUtility.Map<PieceCreateDto, Piece>(pieceDto);
                                piece.Id = Guid.NewGuid();
                                piece.ShipmentId = shipment.Id;
                                shipment.Pieces.Add(piece);
                            }
                        }

                        order.Shipments.Add(shipment);
                    }
                }

                // EF Core's change tracker will automatically track related entities
                await _repo.CreateOrder(order);
                await _repo.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
        }

        [HttpPatch("{orderId}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult<OrderReadDto>> PatchOrder(
            Guid orderId,
            [FromBody] OrderUpdateDto orderUpdateDto
        )
        {
            if (orderUpdateDto == null)
            {
                return BadRequest("Details of the order to update is not complete!");
            }

            var orderToUpdate = await _repo.GetOrderById(orderId);
            if (orderToUpdate == null)
            {
                return NotFound();
            }

            orderToUpdate.PatchFrom(orderUpdateDto);
            orderToUpdate.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();

            var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(orderToUpdate);
            if (orderToUpdate.Client != null)
                orderReadDto.Client = MapperUtility.Map<Client, ClientReadDto>(
                    orderToUpdate.Client
                );
            if (orderToUpdate.ShipmentAddress != null)
                orderReadDto.ShipmentAddress = MapperUtility.Map<Address, AddressReadDto>(
                    orderToUpdate.ShipmentAddress
                );
            if (orderToUpdate.DeliveryAddress != null)
                orderReadDto.DeliveryAddress = MapperUtility.Map<Address, AddressReadDto>(
                    orderToUpdate.DeliveryAddress
                );

            return Ok(orderReadDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrders()
        {
            var orders = await _repo.GetAllOrders();

            return Ok(orders);
        }

        [HttpGet("{id}", Name = "GetOrderById")]
        // [Authorize(Roles = "shipper")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            if (id.ToString() == "")
            {
                return BadRequest("The order ID is missing! Please try again with an order ID");
            }
            else
            {
                var order = await _repo.GetOrderById(id);

                if (order != null)
                {
                    var orderReadDto = MapperUtility.Map<Order, OrderReadDto>(order);

                    return Ok(orderReadDto);
                }
                else
                {
                    return NotFound("The order ID does not exists!");
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> DeleteOrderById(Guid id)
        {
            if (id.ToString() != "")
            {
                var deletedOrder = await _repo.GetOrderById(id);

                if (deletedOrder != null)
                {
                    _repo.DeleteOrder(deletedOrder);
                    bool deletionStatus = await _repo.SaveChangesAsync();

                    if (deletionStatus)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return Ok(
                            "The order cannot be deleted due to some database transaction issues! Check again in the logs"
                        );
                    }
                }
                else
                {
                    return NotFound("The order cannot be found!");
                }
            }
            else
            {
                return BadRequest(
                    "No order ID passed! Please pass or provide the order ID for deletion process to begin"
                );
            }
        }

        [HttpPost("upload/{orderId}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> UploadDocument(string orderId, IFormFile file)
        {
            if (file.Length != 0)
            {
                await using var stream = file.OpenReadStream();
                var objectKey = await _documentStorage.UploadDocumentAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    orderId
                );

                return Ok(new { objectKey, fileName = file.FileName });
            }
            else
            {
                return BadRequest("There's no file for shipment order attached!");
            }
        }

        [HttpGet("list-documents/{orderId}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> ListUploadedDocuments(string orderId)
        {
            if (orderId.Length == 0 || orderId.Equals(""))
            {
                return BadRequest(
                    "The shipment order ID has not been supplied. Please provide it again"
                );
            }
            else
            {
                return Ok(await _documentStorage.ListOrderDocumentsAsync(orderId));
            }
        }

        [HttpGet("download-documents/{objectKey}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> DownloadDocuments(string objectKey)
        {
            if (objectKey.Length == 0 || objectKey.Equals(""))
            {
                return BadRequest(
                    "The object key of the document intended to be downloaded is required!"
                );
            }
            else
            {
                var stream = await _documentStorage.DownloadDocumentAsync(objectKey);
                return File(stream, "application/octet-stream");
            }
        }

        [HttpGet("generate-presigned-url/{objectKey}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> GeneratePresignedURL(
            string objectKey,
            [FromQuery] int expiryMinutes = 60
        )
        {
            if (objectKey.Length == 0 || objectKey.Equals(""))
            {
                return BadRequest(
                    "The object key of the document intended to be downloaded is required!"
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        url = await _documentStorage.GetPresignedUrlAsync(objectKey, expiryMinutes),
                    }
                );
            }
        }

        [HttpDelete("delete-documents/{objectKey}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> DeleteDocuments(string objectKey)
        {
            if (objectKey.Length == 0 || objectKey.Equals(""))
            {
                return BadRequest(
                    "The object key of the document intended to be downloaded is required!"
                );
            }
            else
            {
                await _documentStorage.DeleteDocumentAsync(objectKey);
                return NoContent();
            }
        }
    }
}
