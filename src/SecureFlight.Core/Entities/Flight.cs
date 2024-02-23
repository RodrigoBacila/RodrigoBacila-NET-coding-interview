using System;
using System.Collections.Generic;

namespace SecureFlight.Core.Entities;

public class Flight
{
    public long Id { get; set; }

    public string Code { get; set; }

    public string OriginAirport { get; set; }

    public string DestinationAirport { get; set; }

    public Enums.FlightStatus FlightStatusId { get; set; }

    public DateTime DepartureDateTime { get; set; }

    public DateTime ArrivalDateTime { get; set; }

    public FlightStatus Status { get; set; }

    public List<Passenger> Passengers { get; private set; } = new List<Passenger>();

    public List<PassengerFlight> PassengerFlights { get; set; }

    public Airport From { get; set; }

    public Airport To { get; set; }

    public void AddPassengerToFlight(Passenger passengerToAdd)
    {
        Passengers.Add(passengerToAdd);
    }

    public void RemovePassengerFromFlight(Passenger passengerToRemove)
    {
        Passengers.RemoveAll(passenger => passenger.Id == passengerToRemove.Id);
    }
}