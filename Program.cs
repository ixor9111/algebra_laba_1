using System;

namespace algebra_laba_1
{
    class Program
    {
        static void Main(string[] args)
        {
            BigNum num1 = new BigNum("80");
            BigNum num2 = new BigNum("3");
            BigNum num3 = num1 % num2;

            Console.WriteLine(num3.ToString());

        }
    }
}
