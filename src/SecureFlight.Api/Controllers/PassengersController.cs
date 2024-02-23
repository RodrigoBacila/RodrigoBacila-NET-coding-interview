using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecureFlight.Api.Models;
using SecureFlight.Api.Utils;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PassengersController(
    IService<Passenger> passengerService,
    IService<Flight> flightService,
    IMapper mapper)
    : SecureFlightBaseController(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> GetAsync()
    {
        var passengers = await passengerService.GetAllAsync();
        return MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }

    [HttpGet("/get-passengers-by-flight")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> GetPassengersByFlight([FromQuery] long flightId)
    {
        var passengers = await passengerService.FilterAsync(passenger => passenger.Flights.Any(flight => flight.Id == flightId));
        return !passengers.Succeeded ?
            NotFound($"No passengers were found for the flight {flightId}") :
            MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }

    [HttpPut("/add-passenger-to-flight")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> AddPassengerToFlightAsync([FromQuery] long flightId, [FromQuery] string passengerId)
    {
        var passengerResult = await passengerService.FindAsync(passengerId);

        if (!passengerResult.Succeeded || passengerResult.Result == null)
            return NotFound($"No passengers were found with the indicated ID {passengerId}");

        var passenger = passengerResult.Result;

        var flightResult = await flightService.FindAsync(flightId);

        var flight = flightResult.Result;

        if (flight is null)
            return NotFound($"No flights were found with the indicated ID {flightId}");

        if (flight.Passengers.Any(passenger => passenger.Id == passengerId))
            return Ok("The passenger is already in this flight.");

        flight.AddPassengerToFlight(passenger);

        var flightUpdateResult = await flightService.UpdateAsync(flight);

        return flightUpdateResult == null ?
            StatusCode(500, $"There was an error updating the flight {flightId}") :
            MapResultToDataTransferObject<IReadOnlyList<Flight>, IReadOnlyList<FlightDataTransferObject>>(new List<Flight>() { flight });
    }

    [HttpDelete("/remove-passenger-from-flight")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> RemovePassengerFromFlightAsync([FromQuery] long flightId, [FromQuery] string passengerId)
    {
        var passengerResult = await passengerService.FindAsync(passengerId);

        if (!passengerResult.Succeeded || passengerResult.Result == null)
            return NotFound($"No passengers were found with the indicated ID {passengerId}");

        var passenger = passengerResult.Result;

        var flightResult = await flightService.FilterAsync(flight => flight.Passengers.Any(passenger => passenger.Id == passengerId));

        var flightList = flightResult.Result;
        var selectedFlight = flightList.FirstOrDefault(flightItem => flightItem.Id == flightId);

        if (selectedFlight is null)
            return Ok($"The passenger {passengerId} is not present in flight {flightId}");

        selectedFlight.RemovePassengerFromFlight(passenger);

        var flightUpdateResult = await flightService.UpdateAsync(selectedFlight);

        bool passengerRemovedFromSystem = false;

        if (flightList.Count <= 1)
            passengerRemovedFromSystem = await passengerService.DeleteAsync(passenger);

        return flightUpdateResult == null ?
            StatusCode(500, $"There was an error updating the flight {flightId}") :
            Ok($"The passenger {passengerId} was successfully removed from flight {flightId}. {(passengerRemovedFromSystem ? "Since the passenger was not present in any other flights, they have been removed from the system entirely." : "")}");
    }
}