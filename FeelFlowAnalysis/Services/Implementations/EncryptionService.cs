﻿using System.Security.Cryptography;
using FeelFlowAnalysis.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace FeelFlowAnalysis.Services.Implementations;

// Summary:
//     Provides encryption services.
public sealed class EncryptionService : IEncryptionService
{
    private readonly Aes _aes = Aes.Create();

    // Summary:
    //     Initializes a new instance of the Encryption class.
    //
    // Parameters:
    //   settings:
    //     The encryption settings.
    public EncryptionService(IOptions<Settings> settings)
    {
        _aes.Key = Convert.FromBase64String(settings.Value.Encryption.EncryptionKey);
        _aes.IV = Convert.FromBase64String(settings.Value.Encryption.InitializationVector);
    }

    // Summary:
    //     Decrypts an encrypted string.
    //
    // Parameters:
    //   encryptedValue:
    //     The encrypted string to decrypt.
    //
    // Returns:
    //     The decrypted string.
    public string DecryptString(string encryptedValue)
    {
        byte[] cipherText = Convert.FromBase64String(encryptedValue);

        using var decryptor = _aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt, System.Text.Encoding.UTF8);

        return srDecrypt.ReadToEnd();
    }

    // Summary:
    //     Encrypts a string value.
    //
    // Parameters:
    //   value:
    //     The string value to encrypt.
    //
    // Returns:
    //     The encrypted string.
    public string EncryptString(string value)
    {
        using var encryptor = _aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt, System.Text.Encoding.UTF8))
        {
            swEncrypt.Write(value);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    // Summary:
    //     Verifies if entered data matches stored data.
    //
    // Parameters:
    //   storedData:
    //     The data stored in a secure manner.
    //
    //   enteredData:
    //     The data entered by the user.
    //
    // Returns:
    //     true if entered data matches stored data; otherwise, false.
    public bool VerifyString(string storedData, string enteredData)
        => storedData == EncryptString(enteredData);
}