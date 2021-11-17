using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace algebra_laba_1
{

    public class BigNum
    {
        //Создадим список, в котором будем хранить отдельные цифры
        private readonly List<byte> digits = new List<byte>();

        
        //Основные конструкторы
        public BigNum(int num)//По int
        {
            if (num < 0) Sign = -1;

            digits.AddRange(GetBytes((uint)Math.Abs(num)));
            RemoveNulls();
        }

        public BigNum(uint num)//По натуральному int
        {
            digits.AddRange(GetBytes(num));
        }

        public BigNum(string stringNum)//По строке
        {
            if (stringNum.StartsWith("-"))
            {
                Sign = -1;
                stringNum = stringNum.Substring(1);
            }

            foreach (var symbol in stringNum.Reverse())
                digits.Add(Convert.ToByte(symbol.ToString()));

            RemoveNulls();
        }

        public BigNum(List<byte> bytes, int sign = 1)//По списку чисел и знаком
        {
            Sign = sign;
            digits = bytes.ToList();
            RemoveNulls();
        }


        //Вспомагательные команды
        private List<byte> GetBytes(uint num)//Возвращает список чисел
        {
            var bytes = new List<byte>();

            while (num >= 0)
            {
                bytes.Add((byte)(num % 10));

                if (num == 0) break;
                else num = num / 10;  
            }
             
            return bytes;
        }

        private void RemoveNulls()//Удаляет 0 в начале обьекта
        {
            for (var i = digits.Count - 1; i > 0; i--)
            {
                if (digits[i] == 0) digits.RemoveAt(i);
                else break;  
            }
        }

        private byte GetByte(int index)//Возвращает число
        {
            if (index < digits.Count) return digits[index];
            else return 0;
        }

        private void SetByte(int index, byte digit)//Устанавливает число по index
        {
            while (digits.Count <= index) digits.Add(0);

            digits[index] = digit;
        }

        public override string ToString()//Возвращает целую строку нашего bigInt
        {
            if (this == Zero) return "0";

            string sign;
            if (Sign == -1) sign = "-";
            else sign = "";

            var str = new StringBuilder(sign);

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                str.Append(digits[i].ToString());
            }

            return str.ToString();
        }

        private static BigNum Exp(byte val, int exp)//Возвращает число, умноженое на кол-во 0 перед ним
        {
            var bigInt = Zero;
            bigInt.SetByte(exp, val);
            bigInt.RemoveNulls();
            return bigInt;
        }


        //Вспомогательные переменные для присвоений и увеличения значений на единицу
        public static BigNum Zero => new BigNum(0);
        public static BigNum One => new BigNum(1);

        //Знак bigInt
        public int Sign { get; set; } = 1;


        //Основные функции 
        private static BigNum Add(BigNum a, BigNum b)//Добавление
        {
            var digits = new List<byte>();
            int compare = CompareOutSign(a, b);
            int sign = 1;

            BigNum max = Zero;
            BigNum min = Zero;
            //a + b = +
            //a - b = if a>b ? + : -
            //-a + b = if a>b ? - : +
            //-a - b = -
            if (compare == 1)
            {
                max = a;
                min = b;

                if (a.Sign != 1) sign = -1;
            }
            else if (compare == -1)
            {
                max = b;
                min = a;

                if (b.Sign != 1) sign = -1;
            }
            else if (compare == 0 && ((a.Sign == -1 && b.Sign == 1) || (a.Sign == 1 && b.Sign == -1)))
                return Zero;

            byte k = 0;
            if ((a.Sign == 1 && b.Sign == 1) || (a.Sign == -1 && b.Sign == -1))
            {
                for (int i = 0; i < Math.Max(a.digits.Count, b.digits.Count); i++)
                {
                    byte sum = (byte)(a.GetByte(i) + b.GetByte(i) + k);
                    if (sum >= 10)
                    {
                        sum -= 10;
                        k = 1;
                    }
                    else k = 0;

                    digits.Add(sum);
                }
                if (k == 1) digits.Add(1);
            }
            else
            {
                for (var i = 0; i < Math.Max(max.digits.Count, min.digits.Count); i++)
                {
                    var sum = max.GetByte(i) - min.GetByte(i) - k;

                    if (sum < 0)
                    {
                        sum += 10;
                        k = 1;
                    }
                    else k = 0;

                    digits.Add((byte)sum);
                }
            }
            

            return new BigNum(digits, sign);
        }

        private static BigNum Sub(BigNum a, BigNum b)//Вычитание
        {
            var digits = new List<byte>();
            var compare = CompareOutSign(a, b);
            int sign = 1;

            BigNum max = Zero;
            BigNum min = Zero;


            if (compare == 1)
            {
                max = a;
                min = b;

                if (a.Sign != 1) sign = -1;
            }
            else if (compare == -1)
            {
                max = b;
                min = a;

                if (b.Sign == 1) sign = -1;
            }
            else return Zero;


            var k = 0;
            if((a.Sign == 1 && b.Sign == 1) || (a.Sign == -1 && b.Sign == -1))
            {
                for (var i = 0; i < Math.Max(max.digits.Count, min.digits.Count); i++)
                {
                    var sum = max.GetByte(i) - min.GetByte(i) - k;

                    if (sum < 0)
                    {
                        sum += 10;
                        k = 1;
                    }
                    else k = 0;

                    digits.Add((byte)sum);
                }
            }
            else
            {
                for (int i = 0; i < Math.Max(a.digits.Count, b.digits.Count); i++)
                {
                    byte sum = (byte)(max.GetByte(i) + min.GetByte(i) + k);
                    if (sum >= 10)
                    {
                        sum -= 10;
                        k = 1;
                    }
                    else k = 0;

                    digits.Add(sum);
                }
                if (k == 1) digits.Add(1);
            }
            

            return new BigNum(digits, sign);
        }

        private static BigNum Mult(BigNum a, BigNum b)//Умножение
        {
            var sum = Zero;

            for (var i = 0; i < a.digits.Count; i++)
            {
                for (int j = 0, carry = 0; (j < b.digits.Count) || (carry > 0); j++)
                {
                    var cur = sum.GetByte(i + j) + a.GetByte(i) * b.GetByte(j) + carry;

                    sum.SetByte(i + j, (byte)(cur % 10));
                    carry = cur / 10;
                }
            }

            if ((a.Sign != 1 && b.Sign == 1) || (a.Sign == 1 && b.Sign != 1))
                sum.Sign = -1;

            return sum;
        }

        private static BigNum Div(BigNum a, BigNum b)//Деление
        {
            var retVal = Zero;
            var curVal = Zero;

            for (var i = a.digits.Count - 1; i >= 0; i--)
            {
                curVal = Add(curVal, Exp(a.GetByte(i), i));

                var x = 0;
                var bot = 0;
                var top = 10;
                while (bot <= top)
                {
                    var m = (top + bot) / 2;
                    var cur = Mult(b, Exp((byte)m, i));
                    if (Compare(cur, curVal) != 1)
                    {
                        x = m;
                        bot = m + 1;
                    }
                    else
                    {
                        top = m - 1;
                    }
                }

                retVal.SetByte(i, (byte)(x % 10));
                var t = Mult(b, Exp((byte)x, i));
                curVal = Sub(curVal, t);
            }

            retVal.RemoveNulls();

            if (a.Sign != b.Sign) retVal.Sign = -1;
            else retVal.Sign = 1;

            return retVal;
        }

        private static BigNum Mod(BigNum a, BigNum b)//Остаток от деления
        {
            var divVal = Div(a, b);
            var multVal = Mult(b, divVal);
            return Sub(a, multVal);
        }

        private static BigNum Pow(BigNum a, BigNum b)//Вознесение в степень
        {
            var curVal = a;
            var amountIter = Sub(b, One);
            while(Compare(amountIter, Zero) != 0)
            {
                amountIter = Sub(amountIter, One);
                curVal = Mult(curVal, a);
            }

            return curVal;
        }

        public static BigNum Sqrt(BigNum a)//Квадратный корень
        {
            var topVal = a;
            var botVal = One;
            var curVal = Zero;
            var recCheckF = Zero;
            var recCheckS = Zero;

            while (true)
            {
                recCheckS = recCheckF;
                recCheckF = curVal;
                curVal = Div(Add(topVal, botVal), Add(One, One));
                

                if (Compare(Pow(curVal, Add(One, One)), a) == 1)
                {
                    
                    topVal = curVal;
                }
                else if (Compare(Pow(curVal, Add(One, One)), a) == -1)
                {
                    if (botVal == curVal) return curVal;
                    botVal = curVal;
                }
                else return curVal;

                if (Compare(recCheckS, curVal) == 0) return curVal; 
            }






        }


        //Основные функции сравнения
        private static int Compare(BigNum a, BigNum b)//Сравнить bigInt
        {
            if (CompareSign(a, b) != 0) return CompareSign(a, b);
            else if (CompareSize(a, b) != 0) return CompareSize(a, b);
            else return CompareDigits(a, b);
        }

        public static int CompareOutSign(BigNum a, BigNum b)//Сравнить без знака
        {
            if (CompareSize(a, b) != 0) return CompareSize(a, b);
            else return CompareDigits(a, b);
        }

        private static int CompareSign(BigNum a, BigNum b)//Сравнить знак
        {
            if (a.Sign > b.Sign) return 1;
            else if (a.Sign < b.Sign) return -1;
            else return 0;
        }

        private static int CompareSize(BigNum a, BigNum b)//Сравнить длинну bigInt
        {
            if (a.digits.Count > b.digits.Count) return 1;
            else if (a.digits.Count < b.digits.Count) return -1;
            else return 0;
        }

        private static int CompareDigits(BigNum a, BigNum b)//Сравнить список чисеел
        {
            int size = Math.Max(a.digits.Count, b.digits.Count);

            for (int i = size - 1; i >= 0; i--)
            {
                if (a.GetByte(i) > b.GetByte(i)) return 1;
                else if (a.GetByte(i) < b.GetByte(i)) return -1;
            }

            return 0;
        }


        //Перегрузка булевых операторов 
        public static bool operator <(BigNum a, BigNum b) => Compare(a, b) < 0;

        public static bool operator >(BigNum a, BigNum b) => Compare(a, b) > 0;

        public static bool operator <=(BigNum a, BigNum b) => Compare(a, b) <= 0;

        public static bool operator >=(BigNum a, BigNum b) => Compare(a, b) >= 0;

        public static bool operator ==(BigNum a, BigNum b) => Compare(a, b) == 0;

        public static bool operator !=(BigNum a, BigNum b) => Compare(a, b) != 0;

        public override bool Equals(object obj) => !(obj is BigNum) ? false : this == (BigNum)obj;


        //Перегрузка основных функциональных операторов
        public static BigNum operator -(BigNum a)
        {
            if (a.Sign == 1) a.Sign = -1;
            else a.Sign = 1;

            return a;
        }

        public static BigNum operator +(BigNum a, BigNum b) => Add(a, b);

        public static BigNum operator -(BigNum a, BigNum b) => Sub(a, b);

        public static BigNum operator *(BigNum a, BigNum b) => Mult(a, b);

        public static BigNum operator /(BigNum a, BigNum b) => Div(a, b);

        public static BigNum operator %(BigNum a, BigNum b) => Mod(a, b);
    }
    
}
