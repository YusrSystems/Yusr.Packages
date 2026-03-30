namespace Yusr.Reporting.Abstractions
{
    public static class UrlHelper
    {
        public static async Task<byte[]?> FetchImageBytesAsync(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
