using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace NTCC.NET.Core.Tools
{
  public class ConfigFileProcessor
  {

    private byte[] key;
    private byte[] iv;

    public ConfigFileProcessor()
    {
      initialize();
    }


    private static string GetEthernetMacAddress()
    {
        try
        {
            var ethernetInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                                    && n.OperationalStatus == OperationalStatus.Up);

            return ethernetInterface?.GetPhysicalAddress().ToString() ?? string.Empty;
        }
        catch 
        { 
          return string.Empty; 
        }
    }
    private void initialize()
    {
      // Получаем имя машины
      string machineName = Environment.MachineName;

      // Получаем MAC-адрес сетевого интерфейса
      string macAddress = GetEthernetMacAddress();

      // Создаем ключ и IV из уникальных данных машины
      using (SHA512 hashAlg = SHA512.Create())
      {
        // Мы будем использовать первые 32 байта хеша в качестве ключа и следующие 16 байт в качестве вектора инициализации.
        var hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(machineName + macAddress));

        byte[] Key = new byte[32];
        byte[] Iv  = new byte[16];

        Buffer.BlockCopy(hash, 0, Key, 0, 32);
        Buffer.BlockCopy(hash, 32, Iv, 0, 16);

        this.key = Key;
        this.iv  = Iv;
      }
    }

    public void EncodeFile(string filePath, string encodedFilePath)
    {
      var bytesToBeEncoded = File.ReadAllBytes(filePath);

      using (Aes aes = Aes.Create())
      {
        aes.Key = this.key;
        aes.IV = this.iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (MemoryStream ms = new MemoryStream())
        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
          cs.Write(bytesToBeEncoded, 0, bytesToBeEncoded.Length);
          cs.FlushFinalBlock();

          File.WriteAllBytes(encodedFilePath, ms.ToArray());
        }
      }
    }

    public void DecodeToFile(string encodedFilePath, string decodedFilePath)
    {
      string xmlString = DecodeToString(encodedFilePath);
      File.WriteAllText(decodedFilePath, xmlString);
    }

    public string DecodeToString(string encodedFilePath)
    {
      string decrypted = "";
      var bytesToBeDecoded = File.ReadAllBytes(encodedFilePath);

      using (Aes aes = Aes.Create())
      {
        aes.Key = this.key;
        aes.IV = this.iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream ms = new MemoryStream(bytesToBeDecoded))
        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        {
          using (StreamReader sr = new StreamReader(cs))
          {
            decrypted = sr.ReadToEnd();
          }
        }
      }

      return decrypted;
    }

    public XDocument GetXDocument(string xmlConfigPath, bool deleteSource = true)
    {
      XDocument xmlDocument = null;
      try
      {
        string configFilePath = Path.ChangeExtension(xmlConfigPath, ".ntcc");

        //если  xml файл конфигурации существует кодируем его и удаляем если запрошено
        if (File.Exists(xmlConfigPath))
        {
          EncodeFile(xmlConfigPath, configFilePath);
          if (deleteSource)
            File.Delete(xmlConfigPath);
        }

        //декодируем файл конфигурации и загружаем его в XDocument
        string xmlString = DecodeToString(configFilePath);
        XmlReader reader = XmlReader.Create(new StringReader(xmlString));

        xmlDocument = XDocument.Load(reader);
      }
      catch (Exception ex)
      {
        throw new ArgumentException($"Ошибка загрузки конфигурационного файла {xmlConfigPath} : {ex.Message}");
      }

      return xmlDocument;
    }
  }
}
