using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Keycloak;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Utilities;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepo _repo;

        public ClientController(IClientRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("user-registration-sync")]
        public async Task<IActionResult> UserRegistrationSync([FromBody] KeycloakEvent @event)
        {
            Console.WriteLine("Registration user sync invoked");
            //1. Security Check (Match the secret from Docker)
            var secret = Request.Headers["X-Event-Secret"];
            if (secret != "tms-event-user-2478") return Unauthorized("Not allowed to synchronize user data");

            //2. Only process registration events, not others
            if (@event.Type != "REGISTER") return Ok("Not needed to synchronize");

            //3. Extract the information from the body and add the data.
            Console.WriteLine(JsonSerializer.Serialize(@event));
            var clientData = new Client
            {
              Id = new Guid(@event.UserId),
              ContactEmail = @event.Details.GetValueOrDefault("email") ?? "",
              ContactPhone = @event.Details.GetValueOrDefault("phoneNumber") ?? "",
              Name = @event.Details.GetValueOrDefault("first_name") + " " + @event.Details.GetValueOrDefault("last_name")
            };

            await _repo.CreateClient(clientData);
            
            if(await _repo.SaveChangesAsync())
            {
                return Accepted();
            }

            return BadRequest("Error encountered when synchronising user registered data");
        }

        [HttpGet("shipper/accountdetails/{uid}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult<ClientReadDto>> GetShipperAccountDetails(Guid uid)
        {
            if (uid.ToString() == null || uid.ToString() == "")
            {
                return BadRequest("There is no UID of the shipper account supplied. Please try again!");
            } else
            {
                var shipper = await _repo.GetClientById(uid);

                if (shipper != null)
                {
                    var shipperReadDTO = MapperUtility.Map<Client, ClientReadDto>(shipper);
                    return Ok(shipperReadDTO);
                } else
                {
                    return NotFound($"The shipper account with the UID {uid} cannot be found");
                }
            }
        }

        [HttpPut("shipper/updateaccount/{uid}")]
        [Authorize(Roles = "shipper")]
        public async Task<ActionResult> UpdateShipperAccount(Guid uid, [FromBody] ClientUpdateDto clientUpdateDto)
        {
            if (uid.ToString() == null || uid.ToString() == "")
            {
                return BadRequest("There is no UID of the shipper account supplied. Please try again!");
            } else
            {
                if (clientUpdateDto == null || 
                    string.IsNullOrWhiteSpace(clientUpdateDto.Name) ||
                    string.IsNullOrWhiteSpace(clientUpdateDto.ContactEmail) ||
                    string.IsNullOrWhiteSpace(clientUpdateDto.ContactPhone))
                {
                    return BadRequest("Required fields are missing. Please provide Name, ContactEmail, and ContactPhone.");
                } else
                {

                    if (await _repo.ShipperAccountExists(uid))
                    {
                        var shipperAccountToUpdate = MapperUtility.Map<ClientUpdateDto, Client>(clientUpdateDto);
                        // IMPORTANT: Use the uid from the URL parameter, not the one from the request body
                        shipperAccountToUpdate.Id = uid;
                        await _repo.UpdateClient(shipperAccountToUpdate);
                        bool isUpdatedFlag = await _repo.SaveChangesAsync();

                        if (isUpdatedFlag)
                        {
                            Console.WriteLine("Shipper account has been updated!");
                            return NoContent();
                        } else
                        {
                            Console.WriteLine("Shipper account was not updated!");
                            return BadRequest("Failed to update shipper account");
                        }
                    } else
                    {
                        return NotFound($"The shipper account with the UID {uid} does not exists");
                    }
                }
            }
        }
    }
}