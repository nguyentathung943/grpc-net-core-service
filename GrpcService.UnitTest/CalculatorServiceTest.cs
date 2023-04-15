using Moq;
using GrpcService.Protobuf.Services.CalculatorService;
using Xunit;
using GrpcSumProtobuf;
using GrpcService.UnitTests.Helpers;
using GrpcPrimeFactorsProtobuf;
using GrpcAverageProtobuf;
using GrpcMaxProtobuf;

namespace GrpcService.UnitTests
{
    public class CalculatorServiceTest
    {
        #region Calculation_Do-Sum_Unary
        [Fact]
        public async Task DoSumUnitTest()
        {
            // Arrange
            var mockCalculation = new Mock<ICalculation>();
            mockCalculation
                .Setup(m => m.Sum(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int firstNumber, int secondNumber) => firstNumber + secondNumber);

            var service = new CalculatorService(mockCalculation.Object);
            var firstNumber = 1;
            var secondNumber = 2;

            var request = new SumRequest() { FirstNumber = firstNumber, SecondNumber = secondNumber};

            // Act
            var response = await service.Sum(request, TestServerCallContext.Create());

            // Assert
            mockCalculation.Verify(v => v.Sum(firstNumber, secondNumber));
            Assert.Equal(firstNumber + secondNumber, response.Result);
        }
        #endregion

        #region Calculation_Find-Prime-Factors_Server-Streaming
        [Fact]
        public async Task FindPrimeFactorsUnitTest()
        {
            // Arrange
            var callContext = TestServerCallContext.Create();
            var responseStream = new TestServerStreamWriter<PrimesResponse>(callContext);
            var mockCalculation = new Mock<ICalculation>();

            var service = new CalculatorService(mockCalculation.Object);
            var request = new PrimesRequest() {Number = 6};

            // Act
            var call = service.FindPrimeFactors(request, responseStream, callContext);

            // Assert
            Assert.False(call.IsCompletedSuccessfully, "Not started yet, expec to be false");

            // Server start to process the value
            await call;

            responseStream.Complete();

            var allMessages = new List<PrimesResponse>();

            await foreach (var message in responseStream.ReadAllAsync())
            {
                allMessages.Add(message);
            }

            // 6 = 2 * 3 => 2 and 3 are the primes factors of 6
            Assert.True(allMessages.Count == 2);
            Assert.Equal(2, allMessages[0].Result);
            Assert.Equal(3, allMessages[1].Result);
        }
        #endregion

        #region Calculation_Find-Average_Client-Streaming
        [Fact]
        public async Task FindAverageUnitTest()
        {
            // Arrange
            var mockCalculation = new Mock<ICalculation>();
            mockCalculation
                .Setup(m => m.DivideFloat(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int firstNumber, int secondNumber) => (float)firstNumber / secondNumber);

            var callContext = TestServerCallContext.Create();
            var requestStream = new TestAsyncStreamReader<AverageRequest>(callContext);
            var service = new CalculatorService(mockCalculation.Object);

            // Act
            //var call = service.FindAverage(requestStream, callContext);

            requestStream.AddMessage(new AverageRequest { Value = 1 });
            requestStream.AddMessage(new AverageRequest { Value = 2 });
            requestStream.AddMessage(new AverageRequest { Value = 3 });

            // Complete the client stream
            requestStream.Complete();

            // Calculate the Average
            var response = await service.FindAverage(requestStream, callContext);

            // Assert
            mockCalculation.Verify(v => v.DivideFloat((1+2+3),3));
            Assert.Equal(2, response.Result);
        }
        #endregion

        #region Calculation_Find-CurrentMaxValue_Duplex-Streaming
        [Fact]
        public async Task FindCurrentMaxValueUnitTest()
        {
            // Arrange
            var mockCalculation = new Mock<ICalculation>();
            mockCalculation
                .Setup(m => m.IsGreaterThan(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int firstNumber, int secondNumber) => firstNumber > secondNumber);

            var callContext = TestServerCallContext.Create();
            var requestStream = new TestAsyncStreamReader<MaxRequest>(callContext);
            var responseStream = new TestServerStreamWriter<MaxResponse>(callContext);
            var service = new CalculatorService(mockCalculation.Object);

            // Act
            using var call = service.FindCurrentMax(requestStream, responseStream, callContext);

            MaxResponse? maxResponseReader = null;

            // Assert
            requestStream.AddMessage(new MaxRequest { Value = 1 });
            mockCalculation.Verify(v => v.IsGreaterThan(1, int.MinValue));
            maxResponseReader = await responseStream.ReadNextAsync();
            Assert.Equal(1, maxResponseReader!.CurrentMaxValue);
            Assert.Equal(1, maxResponseReader!.CurrentCompareValue);

            requestStream.AddMessage(new MaxRequest { Value = 2 });
            mockCalculation.Verify(v => v.IsGreaterThan(2, 1));
            maxResponseReader = await responseStream.ReadNextAsync();
            Assert.Equal(2, maxResponseReader!.CurrentMaxValue);
            Assert.Equal(2, maxResponseReader!.CurrentCompareValue);

            requestStream.AddMessage(new MaxRequest { Value = 3 });
            mockCalculation.Verify(v => v.IsGreaterThan(3, 2));
            maxResponseReader = await responseStream.ReadNextAsync();
            Assert.Equal(3, maxResponseReader!.CurrentMaxValue);
            Assert.Equal(3, maxResponseReader!.CurrentCompareValue);

            // Close the Request Stream
            requestStream.Complete();

            // Start to compare sent values
            await call;

            // Close the response stream when completed
            responseStream.Complete();
        }
        #endregion
    }
}
