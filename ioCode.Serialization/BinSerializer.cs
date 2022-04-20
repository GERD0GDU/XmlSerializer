//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace ioCode.Serialization
{
    public class BinSerializer<T>
    {
        private const string DEF_PASSWORD = "ADBB8B0020074E9C9594A9F5F00920D9";

        [DebuggerBrowsable(DebuggerBrowsableState.Never), Browsable(false)]
        private string _Password = DEF_PASSWORD;
        public string Password
        {
            get { return _Password; }
            set { _Password = string.IsNullOrEmpty(value) ? DEF_PASSWORD : value; }
        }

        public byte[] Serialize(T rootclass)
        {
            string xmlString = XmlSerializer<T>.SerializeObject(rootclass);

            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(xmlString);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(this.Password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Encrypt
            byte[] bytesEncrypted = Cipher.Encrypt(bytesToBeEncrypted, passwordBytes);

            return bytesEncrypted;
        }

        public T Deserialize(byte[] datas)
        {
            // Get the bytes of the string
            byte[] passwordBytes = Encoding.UTF8.GetBytes(this.Password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            // Decrypt
            byte[] bytesDecrypted = Cipher.Decrypt(datas, passwordBytes);

            // convert to xml string
            string xmlString = Encoding.UTF8.GetString(bytesDecrypted);

            return XmlSerializer<T>.DeserializeObject(xmlString);
        }

        public static T DeserializeObject(string value)
        {
            BinSerializer<T> serializer = new BinSerializer<T>();
            try
            {
                return serializer.Deserialize(Convert.FromBase64String(value));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, string password)
        {
            BinSerializer<T> serializer = new BinSerializer<T>() { Password = password };
            try
            {
                byte[] datas = null;
                using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    datas = new byte[reader.Length];
                    reader.Read(datas, 0, datas.Length);
                    reader.Close();
                }
                return serializer.Deserialize(datas);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file)
        {
            return ReadFile(file, null);
        }

        public static string SerializeObject(T config)
        {
            BinSerializer<T> serializer = new BinSerializer<T>();
            try
            {
                return Convert.ToBase64String(serializer.Serialize(config));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return null;
        }

        public static bool WriteFile(string file, string password, T rootclass)
        {
            bool ok = false;
            BinSerializer<T> serializer = new BinSerializer<T>() { Password = password };
            try
            {
                FileMode fileMode = File.Exists(file)
                    ? FileMode.Truncate
                    : FileMode.Create;

                byte[] datas = serializer.Serialize(rootclass);
                using (FileStream writer = new FileStream(file, fileMode, FileAccess.Write, FileShare.Read))
                {
                    writer.Write(datas, 0, datas.Length);
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T rootclass)
        {
            return WriteFile(file, null, rootclass);
        }
    }
}
