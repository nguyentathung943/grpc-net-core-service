using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcService.Protobuf.Services.CalculatorService
{
    public interface ICalculation
    {
        int Sum(int firstNumber, int secondNumber);
        int DivideInt(int firstNumber, int secondNumber);
        float DivideFloat(int firstNumber, int secondNumber);
        bool IsGreaterThan(int firstNumber, int secondNumber);
    }
}
