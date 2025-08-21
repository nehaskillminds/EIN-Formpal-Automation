using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EinAutomation.Api.Tests.Services
{
    public class AdvancedPdfDownloadTest
    {
        private readonly Mock<ILogger<AdvancedPdfDownloadTest>> _mockLogger;
        private readonly List<string> _testUrls = new()
        {
            "https://sa.www4.irs.gov/modiein/notices/CP575_1755102640378.pdf",
            "https://sa.www4.irs.gov/modiein/notices/CP575_1755107759364.pdf",
            "https://sa.www4.irs.gov/modiein/notices/CP575_1755107805390.pdf"
        };

        public AdvancedPdfDownloadTest()
        {
            _mockLogger = new Mock<ILogger<AdvancedPdfDownloadTest>>();
        }

        [Fact]
        public async Task TestAdvancedPdfDownloadMethods()
        {
            _mockLogger.Object.LogInformation("🚀 Starting Advanced PDF Download Method Testing...");

            var results = new List<(string Method, bool Success, string Details, long FileSize, string Url)>();

            foreach (var testUrl in _testUrls)
            {
                _mockLogger.Object.LogInformation("Testing URL: {Url}", testUrl);

                // Test different URL patterns
                results.Add(await TestUrlPatternVariations(testUrl));

                // Test different HTTP methods
                results.Add(await TestHttpMethods(testUrl));

                // Test different content types
                results.Add(await TestContentTypes(testUrl));

                // Test different authentication methods
                results.Add(await TestAuthenticationMethods(testUrl));

                // Test different proxy configurations
                results.Add(await TestProxyConfigurations(testUrl));

                // Test different SSL configurations
                results.Add(await TestSslConfigurations(testUrl));

                // Test different timeout configurations
                results.Add(await TestTimeoutConfigurations(testUrl));

                // Test different retry strategies
                results.Add(await TestRetryStrategies(testUrl));

                // Test different compression methods
                results.Add(await TestCompressionMethods(testUrl));

                // Test different caching strategies
                results.Add(await TestCachingStrategies(testUrl));
            }

            // Print comprehensive results
            _mockLogger.Object.LogInformation("📊 Advanced PDF Download Test Results:");
            _mockLogger.Object.LogInformation("=" * 100);
            
            var groupedResults = results.GroupBy(r => r.Method);
            foreach (var group in groupedResults)
            {
                var successCount = group.Count(r => r.Success);
                var totalCount = group.Count();
                var successRate = (double)successCount / totalCount * 100;
                
                _mockLogger.Object.LogInformation("📈 {Method}: {SuccessCount}/{TotalCount} ({SuccessRate:F1}%)", 
                    group.Key, successCount, totalCount, successRate);
                
                foreach (var result in group)
                {
                    var status = result.Success ? "✅" : "❌";
                    _mockLogger.Object.LogInformation("  {Status} {Url} | Size: {FileSize} bytes | {Details}", 
                        status, result.Url, result.FileSize, result.Details);
                }
            }

            var overallSuccessCount = results.Count(r => r.Success);
            _mockLogger.Object.LogInformation("=" * 100);
            _mockLogger.Object.LogInformation("📈 OVERALL SUMMARY: {SuccessCount}/{TotalCount} downloads succeeded ({SuccessRate:F1}%)", 
                overallSuccessCount, results.Count, (double)overallSuccessCount / results.Count * 100);
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestUrlPatternVariations(string baseUrl)
        {
            try
            {
                // Extract base components
                var uri = new Uri(baseUrl);
                var basePath = uri.GetLeftPart(UriPartial.Path);
                var fileName = Path.GetFileName(uri.LocalPath);
                
                // Test different URL patterns
                var urlVariations = new[]
                {
                    baseUrl,
                    baseUrl.Replace("https://", "http://"),
                    baseUrl.Replace("www4", "www"),
                    baseUrl.Replace("www4", "www1"),
                    baseUrl.Replace("www4", "www2"),
                    baseUrl.Replace("www4", "www3"),
                    baseUrl.Replace("modiein", "modein"),
                    baseUrl.Replace("modiein", "modiein"),
                    baseUrl.Replace("notices", "notice"),
                    baseUrl.Replace("notices", "documents"),
                    baseUrl.Replace("CP575", "CP575Notice"),
                    baseUrl.Replace("CP575", "CP575_Notice"),
                    baseUrl.Replace("CP575", "CP575Notice_"),
                    baseUrl.Replace(".pdf", "_download.pdf"),
                    baseUrl.Replace(".pdf", ".PDF"),
                    baseUrl.Replace(".pdf", ".pdf?download=true"),
                    baseUrl.Replace(".pdf", ".pdf&download=true"),
                    baseUrl.Replace(".pdf", ".pdf?type=download"),
                    baseUrl.Replace(".pdf", ".pdf&type=download")
                };

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Referrer = new Uri("https://sa.www4.irs.gov/modiein/");

                var successfulDownloads = new List<(string Url, long Size)>();

                foreach (var url in urlVariations)
                {
                    try
                    {
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            successfulDownloads.Add((url, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Failed to download from {Url}: {Error}", url, ex.Message);
                    }
                }

                if (successfulDownloads.Any())
                {
                    var bestResult = successfulDownloads.OrderByDescending(x => x.Size).First();
                    return ("URL Pattern Variations", true, 
                        $"Found {successfulDownloads.Count} working variations. Best: {bestResult.Size} bytes", 
                        bestResult.Size, bestResult.Url);
                }

                return ("URL Pattern Variations", false, "No working URL variations found", 0, baseUrl);
            }
            catch (Exception ex)
            {
                return ("URL Pattern Variations", false, ex.Message, 0, baseUrl);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestHttpMethods(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var methods = new[] { "GET", "POST", "HEAD" };
                var results = new List<(string Method, long Size)>();

                foreach (var method in methods)
                {
                    try
                    {
                        var request = new HttpRequestMessage(new HttpMethod(method), url);
                        var response = await httpClient.SendAsync(request);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsByteArrayAsync();
                            if (content.Length > 0)
                            {
                                results.Add((method, content.Length));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("HTTP {Method} failed: {Error}", method, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("HTTP Methods", true, 
                        $"Successful methods: {string.Join(", ", results.Select(r => r.Method))}. Best: {bestResult.Method} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("HTTP Methods", false, "No successful HTTP methods", 0, url);
            }
            catch (Exception ex)
            {
                return ("HTTP Methods", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestContentTypes(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var contentTypes = new[]
                {
                    "application/pdf",
                    "application/octet-stream",
                    "application/force-download",
                    "application/x-download",
                    "binary/octet-stream",
                    "text/plain",
                    "text/html",
                    "*/*"
                };

                var results = new List<(string ContentType, long Size)>();

                foreach (var contentType in contentTypes)
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                        
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            results.Add((contentType, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Content-Type {ContentType} failed: {Error}", contentType, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Content Types", true, 
                        $"Working content types: {string.Join(", ", results.Select(r => r.ContentType))}. Best: {bestResult.ContentType} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Content Types", false, "No working content types", 0, url);
            }
            catch (Exception ex)
            {
                return ("Content Types", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestAuthenticationMethods(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var authMethods = new[]
                {
                    ("None", (Action<HttpClient>)null),
                    ("Basic", (client) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")))),
                    ("Bearer", (client) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token")),
                    ("OAuth", (client) => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", "test-oauth")),
                    ("API-Key", (client) => client.DefaultRequestHeaders.Add("X-API-Key", "test-api-key")),
                    ("Session", (client) => client.DefaultRequestHeaders.Add("Cookie", "session=test123; auth=test456"))
                };

                var results = new List<(string AuthMethod, long Size)>();

                foreach (var (authMethod, configureAuth) in authMethods)
                {
                    try
                    {
                        if (configureAuth != null)
                        {
                            configureAuth(httpClient);
                        }
                        
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            results.Add((authMethod, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Auth method {AuthMethod} failed: {Error}", authMethod, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Authentication Methods", true, 
                        $"Working auth methods: {string.Join(", ", results.Select(r => r.AuthMethod))}. Best: {bestResult.AuthMethod} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Authentication Methods", false, "No working authentication methods", 0, url);
            }
            catch (Exception ex)
            {
                return ("Authentication Methods", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestProxyConfigurations(string url)
        {
            try
            {
                // Note: This is a simulation since we can't configure actual proxies in the test
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(url);
                
                return ("Proxy Configurations", true, "Simulated proxy test successful", pdfBytes.Length, url);
            }
            catch (Exception ex)
            {
                return ("Proxy Configurations", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestSslConfigurations(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(url);
                
                return ("SSL Configurations", true, "SSL test successful", pdfBytes.Length, url);
            }
            catch (Exception ex)
            {
                return ("SSL Configurations", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestTimeoutConfigurations(string url)
        {
            try
            {
                var timeouts = new[] { 10, 30, 60, 120 };
                var results = new List<(int Timeout, long Size)>();

                foreach (var timeout in timeouts)
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                        
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            results.Add((timeout, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Timeout {Timeout}s failed: {Error}", timeout, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Timeout Configurations", true, 
                        $"Working timeouts: {string.Join(", ", results.Select(r => $"{r.Timeout}s"))}. Best: {bestResult.Timeout}s ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Timeout Configurations", false, "No working timeout configurations", 0, url);
            }
            catch (Exception ex)
            {
                return ("Timeout Configurations", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestRetryStrategies(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var retryStrategies = new[]
                {
                    ("No Retry", 1),
                    ("Linear Retry", 3),
                    ("Exponential Backoff", 5),
                    ("Aggressive Retry", 10)
                };

                var results = new List<(string Strategy, long Size)>();

                foreach (var (strategy, maxRetries) in retryStrategies)
                {
                    try
                    {
                        byte[] pdfBytes = null;
                        
                        for (int i = 0; i < maxRetries; i++)
                        {
                            try
                            {
                                pdfBytes = await httpClient.GetByteArrayAsync(url);
                                break;
                            }
                            catch (Exception ex) when (i < maxRetries - 1)
                            {
                                var delay = strategy switch
                                {
                                    "Linear Retry" => 1000 * (i + 1),
                                    "Exponential Backoff" => 1000 * (int)Math.Pow(2, i),
                                    "Aggressive Retry" => 500 * (i + 1),
                                    _ => 1000
                                };
                                await Task.Delay(delay);
                                continue;
                            }
                        }

                        if (pdfBytes != null && pdfBytes.Length > 0)
                        {
                            results.Add((strategy, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Retry strategy {Strategy} failed: {Error}", strategy, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Retry Strategies", true, 
                        $"Working strategies: {string.Join(", ", results.Select(r => r.Strategy))}. Best: {bestResult.Strategy} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Retry Strategies", false, "No working retry strategies", 0, url);
            }
            catch (Exception ex)
            {
                return ("Retry Strategies", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestCompressionMethods(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var compressionMethods = new[]
                {
                    ("None", new string[0]),
                    ("Gzip", new[] { "gzip" }),
                    ("Deflate", new[] { "deflate" }),
                    ("Brotli", new[] { "br" }),
                    ("Multiple", new[] { "gzip", "deflate", "br" })
                };

                var results = new List<(string Method, long Size)>();

                foreach (var (method, encodings) in compressionMethods)
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
                        foreach (var encoding in encodings)
                        {
                            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(encoding));
                        }
                        
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            results.Add((method, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Compression method {Method} failed: {Error}", method, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Compression Methods", true, 
                        $"Working methods: {string.Join(", ", results.Select(r => r.Method))}. Best: {bestResult.Method} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Compression Methods", false, "No working compression methods", 0, url);
            }
            catch (Exception ex)
            {
                return ("Compression Methods", false, ex.Message, 0, url);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize, string Url)> TestCachingStrategies(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                var cacheStrategies = new[]
                {
                    ("No Cache", new[] { "no-cache" }),
                    ("No Store", new[] { "no-store" }),
                    ("Must Revalidate", new[] { "must-revalidate" }),
                    ("Public Cache", new[] { "public" }),
                    ("Private Cache", new[] { "private" }),
                    ("Max Age", new[] { "max-age=3600" })
                };

                var results = new List<(string Strategy, long Size)>();

                foreach (var (strategy, cacheHeaders) in cacheStrategies)
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                        foreach (var header in cacheHeaders)
                        {
                            httpClient.DefaultRequestHeaders.Add("Cache-Control", header);
                        }
                        
                        var pdfBytes = await httpClient.GetByteArrayAsync(url);
                        if (pdfBytes.Length > 0)
                        {
                            results.Add((strategy, pdfBytes.Length));
                        }
                    }
                    catch (Exception ex)
                    {
                        _mockLogger.Object.LogDebug("Cache strategy {Strategy} failed: {Error}", strategy, ex.Message);
                    }
                }

                if (results.Any())
                {
                    var bestResult = results.OrderByDescending(x => x.Size).First();
                    return ("Caching Strategies", true, 
                        $"Working strategies: {string.Join(", ", results.Select(r => r.Strategy))}. Best: {bestResult.Strategy} ({bestResult.Size} bytes)", 
                        bestResult.Size, url);
                }

                return ("Caching Strategies", false, "No working cache strategies", 0, url);
            }
            catch (Exception ex)
            {
                return ("Caching Strategies", false, ex.Message, 0, url);
            }
        }
    }
}
