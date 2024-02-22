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
    IService<Passenger> personService,
    IService<Flight> flightService,
    IRepository<Flight> flightRepository,
    IMapper mapper)
    : SecureFlightBaseController(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> Get()
    {
        var passengers = await personService.GetAllAsync();
        return MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }

    [HttpGet("/flights/{flightId:long}/passengers")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> GetPassengersByFlight(long flightId)
    {
        var passengers = await personService.FilterAsync(p => p.Flights.Any(x => x.Id == flightId));
        return !passengers.Succeeded ?
            NotFound($"No passengers were found for the flight {flightId}") :
            MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }

    [HttpPut("/flights/{flightId:long}/passengers/{passengerId}")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> AddPassengerToFlight(long flightId, string passengerId)
    {
        var passengerResult = await personService.FindAsync(passengerId);

        if (!passengerResult.Succeeded || passengerResult.Result == null)
            return NotFound($"No passengers were found with the indicated ID {passengerId}");

        var passenger = passengerResult.Result;

        var flightResult = await flightService.FilterAsync(flight => flight.Id == flightId);

        var flight = flightResult.Result?.FirstOrDefault();

        if (flight == null)
            return NotFound($"No flights were found with the indicated ID {flightId}");

        flight.AddPassengerToFlight(passenger);

        var flightUpdateResult = flightRepository.Update(flight);

        return flightUpdateResult == null ?
            StatusCode(500, $"There was an error updating the flight {flightId}") :
            MapResultToDataTransferObject<IReadOnlyList<Flight>, IReadOnlyList<FlightDataTransferObject>>(flightResult);
    }
}