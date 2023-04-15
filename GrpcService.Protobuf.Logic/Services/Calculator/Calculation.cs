using GrpcGreeterProtobuf;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace GrpcService.Protobuf.Services.CalculatorService
{
    public class Calculation: ICalculation
    {
        public Calculation()
        {}

        public float DivideFloat(int firstNumber, int secondNumber)
        {
            return (float)(firstNumber) / secondNumber;
        }

        public int DivideInt(int firstNumber, int secondNumber)
        {
            return firstNumber / secondNumber;
        }

        public bool IsGreaterThan(int firstNumber, int secondNumber)
        {
            return firstNumber > secondNumber;
        }

        public int Sum(int firstNumber, int secondNumber)
        {
            return firstNumber + secondNumber;
        }
    }
}
