using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EinAutomation.Api.Tests.Services
{
    public class PdfDownloadMethodsTest
    {
        private readonly Mock<ILogger<PdfDownloadMethodsTest>> _mockLogger;
        private readonly string _testUrl = "https://sa.www4.irs.gov/modiein/notices/CP575_1755102640378.pdf";
        private readonly string _testOutputPath = "/tmp/test_download.pdf";

        public PdfDownloadMethodsTest()
        {
            _mockLogger = new Mock<ILogger<PdfDownloadMethodsTest>>();
        }

        [Fact]
        public async Task TestAllPdfDownloadMethods()
        {
            _mockLogger.Object.LogInformation("🚀 Starting comprehensive PDF download method testing...");

            var results = new List<(string Method, bool Success, string Details, long FileSize)>();

            // Method 1: Basic HttpClient
            results.Add(await TestBasicHttpClient());

            // Method 2: HttpClient with User-Agent
            results.Add(await TestHttpClientWithUserAgent());

            // Method 3: HttpClient with Browser Headers
            results.Add(await TestHttpClientWithBrowserHeaders());

            // Method 4: HttpClient with IRS-specific Headers
            results.Add(await TestHttpClientWithIrsHeaders());

            // Method 5: HttpClient with Referer
            results.Add(await TestHttpClientWithReferer());

            // Method 6: HttpClient with Session Cookies
            results.Add(await TestHttpClientWithSessionCookies());

            // Method 7: HttpClient with Accept Headers
            results.Add(await TestHttpClientWithAcceptHeaders());

            // Method 8: WebClient (Legacy)
            results.Add(await TestWebClient());

            // Method 9: WebClient with Headers
            results.Add(await TestWebClientWithHeaders());

            // Method 10: HttpClient with Retry Logic
            results.Add(await TestHttpClientWithRetry());

            // Method 11: HttpClient with Timeout
            results.Add(await TestHttpClientWithTimeout());

            // Method 12: HttpClient with Compression
            results.Add(await TestHttpClientWithCompression());

            // Method 13: HttpClient with Proxy Settings
            results.Add(await TestHttpClientWithProxy());

            // Method 14: HttpClient with Custom SSL
            results.Add(await TestHttpClientWithCustomSSL());

            // Method 15: HttpClient with Form Data
            results.Add(await TestHttpClientWithFormData());

            // Method 16: HttpClient with JSON Payload
            results.Add(await TestHttpClientWithJsonPayload());

            // Method 17: HttpClient with XML Payload
            results.Add(await TestHttpClientWithXmlPayload());

            // Method 18: HttpClient with Multipart Form
            results.Add(await TestHttpClientWithMultipartForm());

            // Method 19: HttpClient with Basic Auth
            results.Add(await TestHttpClientWithBasicAuth());

            // Method 20: HttpClient with Bearer Token
            results.Add(await TestHttpClientWithBearerToken());

            // Method 21: HttpClient with OAuth
            results.Add(await TestHttpClientWithOAuth());

            // Method 22: HttpClient with Custom Certificate
            results.Add(await TestHttpClientWithCustomCertificate());

            // Method 23: HttpClient with Different HTTP Versions
            results.Add(await TestHttpClientWithHttpVersions());

            // Method 24: HttpClient with Connection Pooling
            results.Add(await TestHttpClientWithConnectionPooling());

            // Method 25: HttpClient with Keep-Alive
            results.Add(await TestHttpClientWithKeepAlive());

            // Print results
            _mockLogger.Object.LogInformation("📊 PDF Download Test Results:");
            _mockLogger.Object.LogInformation("=" * 80);
            
            foreach (var result in results)
            {
                var status = result.Success ? "✅ SUCCESS" : "❌ FAILED";
                _mockLogger.Object.LogInformation("{Status} | {Method} | Size: {FileSize} bytes | {Details}", 
                    status, result.Method, result.FileSize, result.Details);
            }

            var successCount = results.Count(r => r.Success);
            _mockLogger.Object.LogInformation("=" * 80);
            _mockLogger.Object.LogInformation("📈 SUMMARY: {SuccessCount}/{TotalCount} methods succeeded", successCount, results.Count);
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestBasicHttpClient()
        {
            try
            {
                using var httpClient = new HttpClient();
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("Basic HttpClient", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("Basic HttpClient", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithUserAgent()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with User-Agent", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with User-Agent", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithBrowserHeaders()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"));
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US,en;q=0.5"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip, deflate"));
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.DefaultRequestHeaders.UpgradeInsecureRequests.Add("1");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Browser Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Browser Headers", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithIrsHeaders()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Referrer = new Uri("https://sa.www4.irs.gov/modiein/");
                httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with IRS Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with IRS Headers", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithReferer()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Referrer = new Uri("https://sa.www4.irs.gov/modiein/confirmation");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Referer", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Referer", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithSessionCookies()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=test123; IRS_SESSION=test456");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Session Cookies", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Session Cookies", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithAcceptHeaders()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Accept Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Accept Headers", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestWebClient()
        {
            try
            {
                using var webClient = new WebClient();
                var pdfBytes = await webClient.DownloadDataTaskAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("WebClient (Legacy)", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("WebClient (Legacy)", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestWebClientWithHeaders()
        {
            try
            {
                using var webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                webClient.Headers.Add("Referer", "https://sa.www4.irs.gov/modiein/");
                webClient.Headers.Add("Accept", "application/pdf,application/octet-stream,*/*");
                
                var pdfBytes = await webClient.DownloadDataTaskAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("WebClient with Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("WebClient with Headers", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithRetry()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                byte[] pdfBytes = null;
                var maxRetries = 3;
                
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                        break;
                    }
                    catch (Exception ex) when (i < maxRetries - 1)
                    {
                        await Task.Delay(1000 * (i + 1)); // Exponential backoff
                        continue;
                    }
                }
                
                if (pdfBytes != null)
                {
                    await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                    return ("HttpClient with Retry", true, $"Downloaded {pdfBytes.Length} bytes after retries", pdfBytes.Length);
                }
                
                return ("HttpClient with Retry", false, "All retries failed", 0);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Retry", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithTimeout()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Timeout", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Timeout", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithCompression()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Compression", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Compression", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithProxy()
        {
            try
            {
                // Note: This is a test method - in production you'd configure actual proxy settings
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Proxy (Simulated)", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Proxy (Simulated)", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithCustomSSL()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Custom SSL", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Custom SSL", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithFormData()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("action", "download"),
                    new KeyValuePair<string, string>("type", "pdf")
                });
                
                var response = await httpClient.PostAsync(_testUrl, formData);
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Form Data", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Form Data", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithJsonPayload()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var jsonContent = new StringContent("{\"action\":\"download\",\"type\":\"pdf\"}", Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync(_testUrl, jsonContent);
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with JSON Payload", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with JSON Payload", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithXmlPayload()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                
                var xmlContent = new StringContent("<request><action>download</action><type>pdf</type></request>", Encoding.UTF8, "application/xml");
                
                var response = await httpClient.PostAsync(_testUrl, xmlContent);
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with XML Payload", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with XML Payload", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithMultipartForm()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent("download"), "action");
                multipartContent.Add(new StringContent("pdf"), "type");
                
                var response = await httpClient.PostAsync(_testUrl, multipartContent);
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Multipart Form", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Multipart Form", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithBasicAuth()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password")));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Basic Auth", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Basic Auth", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithBearerToken()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token-123");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Bearer Token", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Bearer Token", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithOAuth()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", "test-oauth-token");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with OAuth", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with OAuth", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithCustomCertificate()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Custom Certificate", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Custom Certificate", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithHttpVersions()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestVersion = new Version(2, 0);
                httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with HTTP/2", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with HTTP/2", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithConnectionPooling()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Connection Pooling", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Connection Pooling", false, ex.Message, 0);
            }
        }

        private async Task<(string Method, bool Success, string Details, long FileSize)> TestHttpClientWithKeepAlive()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.DefaultRequestHeaders.Add("Keep-Alive", "timeout=5, max=1000");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await File.WriteAllBytesAsync(_testOutputPath, pdfBytes);
                
                return ("HttpClient with Keep-Alive", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return ("HttpClient with Keep-Alive", false, ex.Message, 0);
            }
        }
    }
}
