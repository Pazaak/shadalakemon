using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace shandakemon.core
{
    // An object that controls the crypto-number generator
    public class CRandom
    {
        // Returns a random integer
        public static int RandomInt()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Buffer storage.
                byte[] data = new byte[4];

                // Fill buffer.
                rng.GetBytes(data);

                // Convert to int 32.
                return BitConverter.ToInt32(data, 0);
            }
        }
    }
}
