using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Encryption;

namespace aes256pad
{
    public class AES256IO
    {
        private string FilePath { get; set; }
        private string Hash { get; set; }
        private string _password;

        /// <summary>
        /// Read an UTF8 encrypted file (AES-GCM)
        /// </summary>
        /// <param name="filepath">The full path of the file.</param>
        /// <param name="password">The password used to encrypt the file.</param>
        /// <returns>
        /// Decrypted file as a string
        /// </returns>
        public string Read(string filepath, string password)
        {
            string content = InternalRead(filepath, password);
            Hash = GetHashHex(content);
            FilePath = filepath;
            _password = password;
            return content;
        }

        /// <summary>
        /// Encrypt (AES-GCM) and write an UTF8 string to the last read file
        /// </summary>
        /// <param name="content">The string to encrypt.</param>
        /// <param name="forceWrite">Force the write even if content didn't change.</param>
        /// <returns>
        /// true if string was saved to disk (happens only if content is different then last read/write); false otherwise
        /// </returns>
        public bool Write(string content, bool forceWrite = false)
        {
            if (FilePath == null || _password == null)
            {
                throw new InvalidOperationException("Filepath and Encryption password required!");
            }

            var contentHash = GetHashHex(content);
            if (forceWrite || !Hash.Equals(contentHash, StringComparison.InvariantCulture))
            {
                InternalWrite(FilePath, content, _password);
                Hash = contentHash;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Encrypt (AES-GCM) an UTF8 string using specified password and write it to specified file
        /// </summary>
        /// <param name="filepath">The path of the file to write to.</param>
        /// <param name="password">The password to use for the encryption.</param>
        /// <param name="content">The string to encrypt.</param>
        /// <returns>
        /// true if string was saved to disk (happens only if content is different then last read/write); false otherwise
        /// </returns>
        public void Write(string filepath, string content, string password)
        {
            InternalWrite(filepath, content, password);
            FilePath = filepath;
            _password = password;
            Hash = GetHashHex(content);
        }


        private static string InternalRead(string filepath, string password)
        {
            using (var reader = new StreamReader(filepath))
            {
                return AESGCM.SimpleDecryptWithPassword(reader.ReadToEnd(), password);
            }
        }

        private static void InternalWrite(string filepath, string content, string password)
        {
            // Write encrypted content to file
            var encryptedString = AESGCM.SimpleEncryptWithPassword(content, password);
            using (var writer = new StreamWriter(filepath))
            {
                writer.Write(encryptedString);
            }

            // Read it directly after to make sure everything is OK
            using (var reader = new StreamReader(filepath))
            {
                if (!encryptedString.Equals(reader.ReadToEnd()))
                {
                    throw new IOException("File verification failed: unknown file state.");
                }
            }
        }

        private static string GetHashHex(string content)
        {
            var hasher = SHA256.Create();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(content));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}