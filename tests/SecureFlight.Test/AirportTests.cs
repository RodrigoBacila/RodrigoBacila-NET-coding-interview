using System.Linq;
using System.Threading.Tasks;
using SecureFlight.Infrastructure.Repositories;
using Xunit;

namespace SecureFlight.Test
{
    public class AirportTests
    {
        [Fact]
        public async Task Update_Succeeds()
        {
            //Arrange
            var testingContext = new SecureFlightDatabaseTestContext();
            testingContext.CreateDatabase();
            var repository = new AirportRepository(testingContext);

            //Act
            var currentAirportsData = await repository.GetAllAsync();
            var selectedAirport = currentAirportsData.FirstOrDefault(airportData => airportData.Code == "JFK");

            var newAirportName = "New JFK Airport Name";
            selectedAirport.Name = newAirportName;

            repository.Update(selectedAirport);

            var modifiedAirportDataList = await repository.FilterAsync(airportData => airportData.Code == selectedAirport.Code);
            var modifiedAirportData = modifiedAirportDataList.FirstOrDefault();

            //Assert
            Assert.True(modifiedAirportData.Name.Equals(newAirportName));

            testingContext.DisposeDatabase();
        }
    }
}
