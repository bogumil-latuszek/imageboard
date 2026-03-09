using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

public class FileHashingUtil
{
    public async Task<string> ComputeFileHashAsync(IFormFile file)
    {
        using var sha256 = SHA256.Create();
        await using var stream = file.OpenReadStream();
        byte[] hashBytes = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hashBytes); // e.g., "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855"
    }
}
