using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Security.Cryptography;

namespace MathProcessor
{
    static class ConfigManager
    {
        static string exePath = Assembly.GetEntryAssembly().Location;
        static string appVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
        static AppSettingsSection appSection = null;
        static Configuration config = null;

        static ConfigManager()
        {
            try
            {
                if (!Directory.Exists(PublicFolderPath))
                {
                    Directory.CreateDirectory(PublicFolderPath);
                }
                bool existed = true;
                if (!File.Exists(PublicConfigFilePath))
                {
                    CopyConfigFile();
                    existed = false;
                }
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = PublicConfigFilePath };
                config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                appSection = config.AppSettings;//(AppSettingsSection)config.GetSection("appSettings");
                if (!existed)
                {
                    SetConfigurationValue("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
                }
            }
            catch { }
        }

        public static bool ShowAd
        {
            get
            {
                return GetConfigurationValue("showGamentryAd") != "0";
            }
            set
            {
                SetConfigurationValue("showGamentryAd", value ? "1" : "0");
            }
        }

        public static string GetConfigurationValue(string key)
        {
            try
            {
                return appSection.Settings[key].Value;
            }
            catch
            {
                return "";
            }
        }

        public static bool SetConfigurationValue(string key, string value)
        {
            try
            {
                if (!File.Exists(PublicConfigFilePath))
                {
                    CopyConfigFile();
                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = PublicConfigFilePath };
                    config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    appSection = config.AppSettings;
                    SetConfigurationValue("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
                }
                if (appSection.Settings.AllKeys.Contains(key))
                {
                    appSection.Settings[key].Value = value;
                }
                else
                {
                    appSection.Settings.Add(new KeyValueConfigurationElement(key, value));
                }
                config.Save();
                return true;
            }
            catch { }
            return false;
        }


        public static string PublicConfigFilePath
        {
            get { return Path.Combine(PublicFolderPath, Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".config"); }
        }

        public static string PublicFolderPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Math_Processor_MV");
            }
        }

        private static void CopyConfigFile()
        {
            try
            {
                CreateDefaultConfigFile(PublicConfigFilePath, "configuration");
            }
            catch { }            
        }

        static void CreateDefaultConfigFile(string path, string root)
        {
            try
            {
                using (Stream file = File.Open(path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        writer.WriteLine("<?xml version=\"1.0\"?>");
                        writer.WriteLine("<" + root + " />");
                    }
                }
            }
            catch { }
        }

        public static bool SetConfigurationValue_AES(string key, string value)
        {
            try
            {
                return SetConfigurationValue(key, EncryptString_Aes(value, AesKeyBytes, AesIVBytes));
            }
            catch { }
            return false;
        }

        public static string GetConfigurationValue_AES(string key)
        {
            try
            {
                string base64String = GetConfigurationValue(key);
                return DecryptString_Aes(base64String, AesKeyBytes, AesIVBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string EncryptString_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString_Aes(string base64Text, byte[] Key, byte[] IV)
        {
            if (base64Text == null || base64Text.Length <= 0)
                throw new ArgumentNullException("base64Text");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            byte[] cipherText = Convert.FromBase64String(base64Text);
            string plaintext = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        //32 bytes
        static byte[] AesKeyBytes
        {
            //get { return Convert.FromBase64String("/lQCPxfDQ4QaEkUsBYkcdkAm/CYeGnwOcoYcZTBAh68="); }
            get { return GetBytes("s01", 32); }
        }

        //16 bytes
        static byte[] AesIVBytes
        {
            //get { return Convert.FromBase64String("ton4ck7hOjyMmuE5QsKXQA=="); }
            get { return GetBytes("s02", 16); }
        }

        static byte[] GetBytes(string key, int size)
        {
            string value = GetConfigurationValue(key);
            byte[] bytes = new byte[size];
            if (string.IsNullOrEmpty(value))
            {
                Random rand = new Random();
                rand.NextBytes(bytes);
                value = Convert.ToBase64String(bytes);
                SetConfigurationValue(key, value);
            }
            bytes = Convert.FromBase64String(value);
            return bytes;
        }
    }
}
