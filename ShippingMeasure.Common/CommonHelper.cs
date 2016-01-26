using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShippingMeasure.Common
{
    public class CommonHelper
    {
        private static byte[] key = new byte[] { 0x18, 0x2e, 0x1f, 0x3c, 0x7f, 0x0a, 0xc4, 0x9a, 0x00, 0xe8, 0xe6, 0xde, 0xa5, 0x50, 0x60, 0xaa, 0x06, 0x4d, 0x39, 0xe3, 0xd9, 0x48, 0x00, 0xb6, 0xf8, 0xfc, 0x00, 0x8a, 0xbd, 0x22, 0xbb, 0xf3 };
        private static byte[] iv = new byte[] { 0xb7, 0xcc, 0xd8, 0xe7, 0x15, 0x57, 0x2e, 0x69, 0xbc, 0x9c, 0x99, 0x1b, 0xe6, 0x58, 0x73, 0xf7 };

        public static void ChangeUICultureTo(string culture, Thread thread = null)
        {
            var selectedThread = (thread != null) ? thread : Thread.CurrentThread;
            Application.CurrentCulture = selectedThread.CurrentCulture = selectedThread.CurrentUICulture = new CultureInfo(culture);
            Config.Language = culture;
        }

        public static void TryLoadUICultureFromConfig()
        {
            string language = Config.Language;

            if (!String.IsNullOrEmpty(language))
            {
                try
                {
                    ChangeUICultureTo(language);
                }
                catch (Exception ex)
                {
                    LogHelper.Write(ex);
                    LogHelper.WriteWarning("Failed to load language configuration, using system default culture information");
                }
            }
        }

        public static String SecureStringToMD5(SecureString input)
        {
            int passwordLength = input.Length;
            char[] passwordChars = new char[passwordLength];

            // Copy the password from SecureString to our char array
            IntPtr passwortPointer = Marshal.SecureStringToBSTR(input);
            Marshal.Copy(passwortPointer, passwordChars, 0, passwordLength);
            Marshal.ZeroFreeBSTR(passwortPointer);

            // Hash the char array
            MD5 md5Hasher = MD5.Create();
            byte[] hashedPasswordBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(passwordChars));

            // Wipe the character array from memory
            for (int i = 0; i < passwordChars.Length; i++)
            {
                passwordChars[i] = '\0';
            }

            // Your implementation of representing the hash in a readable manner
            //String hashString = ConvertToHexString(hashedPasswordBytes);
            var hashString = hashedPasswordBytes.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2"))).ToString();

            // Return the result
            return hashString;
        }

        public static byte[] EncryptStringToBytesWithAes(string plainText)
        {
            return CommonHelper.EncryptStringToBytesWithAes(plainText, CommonHelper.key, CommonHelper.iv);
        }

        public static byte[] EncryptStringToBytesWithAes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        public static string DecryptStringFromBytesWithAes(byte[] cipherText)
        {
            return CommonHelper.DecryptStringFromBytesWithAes(cipherText, CommonHelper.key, CommonHelper.iv);
        }

        public static string DecryptStringFromBytesWithAes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
