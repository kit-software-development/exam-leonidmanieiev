using System;
using System.Linq;

namespace AC.Algos
{
    // 'https://en.wikipedia.org/wiki/RSA_(cryptosystem)'
    public static class RSA
    {
        private static long _totient;

        private static bool IsPrime(long number)
        {
            if ((number & 1) == 0) return (number == 2);

            long limit = (long)Math.Sqrt(number);

            for (long i = 3; i <= limit; i += 2)
                if ((number % i) == 0)
                    return false;

            return true;
        }

        private static long LCM(long n1, long n2)
        {
            long num1, num2;

            if (n1 > n2) { num1 = n1; num2 = n2; }
            else { num1 = n2; num2 = n1; }

            for (long i = 1; i < num2; i++)
                if ((num1 * i) % num2 == 0)
                    return i * num1;

            return num1 * num2;
        }

        private static long ModularMultiplicativeInverse(long e, long totient)
        {
            long t = 0, nt = 1, r = totient, nr = e % totient;

            if (totient < 0) totient = -totient;
            if (e < 0) e = totient - (-e % totient);

            while (nr != 0)
            {
                long quot = (r / nr) | 0;
                long tmp = nt; nt = t - quot * nt; t = tmp;
                tmp = nr; nr = r - quot * nr; r = tmp;
            }

            if (r > 1) return -1;
            if (t < 0) t += totient;

            return t;
        }

        private static long ModularExponentiation(long num, long power, long mod)
        {
            long res;

            for (res = 1; power != 0; power >>= 1)
            {
                if ((power & 1) == 1)
                    res = ((res % mod) * (num % mod)) % mod;
                num = ((num % mod) * (num % mod)) % mod;
            }
            return res;
        }

        public static long GetN()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            long p = (long)rand.Next(1000, 15000) * 2 + 1;
            long q = (long)rand.Next(1000, 15000) * 2 + 1;

            while (!IsPrime(p)) p += 2;
            while (!IsPrime(q) || q == p) q += 2;

            _totient = LCM(p - 1, q - 1);

            return p * q;
        }

        public static long GetE()
        {
            long e;

            do
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                e = (long)rand.Next(1, (int)_totient / 2) * 2 + 1;
                while (!IsPrime(e)) e += 2;
            } while (_totient % e == 0);

            return e;
        }

        public static long GetD(long e)
        {
            return ModularMultiplicativeInverse(e, _totient);
        }

        public static string Encrypt(string message, Tuple<long, long> publicKey)
        {
            long n = publicKey.Item1, e = publicKey.Item2;
            string encryptedMessage = "";

            for (int i = 0; i < message.Length; i++)
            {
                // separate each encrypted char with space for simplicity
                encryptedMessage += 
                    ModularExponentiation(message[i], e, n).ToString() + ' ';
            }

            return encryptedMessage;
        }

        public static string Decrypt(string encryptedMessage, long d, long n)
        {
            string decryptedMessage = "";
            string[] encryptedLetters = encryptedMessage.Split(new char[] { ' ' },
                                        StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < encryptedLetters.Length; i++)
            {
                long longRepr = Convert.ToInt64(encryptedLetters[i]);
                decryptedMessage += (char)ModularExponentiation(longRepr, d, n);
            }

            return decryptedMessage;
        }
    }
}
