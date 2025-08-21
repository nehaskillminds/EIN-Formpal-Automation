using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EinAutomation.Api.Tests.Services
{
    /// <summary>
    /// Standalone test runner for PDF download methods
    /// Can be executed independently to test various download strategies
    /// </summary>
    public class PdfDownloadTestRunner
    {
        private readonly ILogger<PdfDownloadTestRunner> _logger;
        private readonly string _testUrl = "https://sa.www4.irs.gov/modiein/notices/CP575_1755102640378.pdf";
        private readonly string _outputDirectory = "/tmp/pdf_download_tests";

        public PdfDownloadTestRunner(ILogger<PdfDownloadTestRunner> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Run all PDF download method tests
        /// </summary>
        public async Task RunAllTests()
        {
            _logger.LogInformation("🚀 Starting PDF Download Method Testing...");
            
            // Ensure output directory exists
            Directory.CreateDirectory(_outputDirectory);

            var results = new List<TestResult>();

            // Basic HTTP Methods
            results.Add(await TestBasicHttpClient());
            results.Add(await TestHttpClientWithUserAgent());
            results.Add(await TestHttpClientWithBrowserHeaders());
            results.Add(await TestHttpClientWithIrsHeaders());

            // Advanced HTTP Methods
            results.Add(await TestHttpClientWithRetry());
            results.Add(await TestHttpClientWithTimeout());
            results.Add(await TestHttpClientWithCompression());

            // Authentication Methods
            results.Add(await TestHttpClientWithBasicAuth());
            results.Add(await TestHttpClientWithBearerToken());
            results.Add(await TestHttpClientWithSessionCookies());

            // Content Type Methods
            results.Add(await TestHttpClientWithAcceptHeaders());
            results.Add(await TestHttpClientWithFormData());
            results.Add(await TestHttpClientWithJsonPayload());

            // Connection Methods
            results.Add(await TestHttpClientWithConnectionPooling());
            results.Add(await TestHttpClientWithKeepAlive());
            results.Add(await TestHttpClientWithHttpVersions());

            // Print Results
            PrintResults(results);
        }

        private async Task<TestResult> TestBasicHttpClient()
        {
            try
            {
                using var httpClient = new HttpClient();
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("basic_httpclient.pdf", pdfBytes);
                
                return new TestResult("Basic HttpClient", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("Basic HttpClient", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithUserAgent()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("useragent_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with User-Agent", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with User-Agent", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithBrowserHeaders()
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
                await SaveTestFile("browser_headers_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Browser Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Browser Headers", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithIrsHeaders()
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
                await SaveTestFile("irs_headers_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with IRS Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with IRS Headers", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithRetry()
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
                    await SaveTestFile("retry_httpclient.pdf", pdfBytes);
                    return new TestResult("HttpClient with Retry", true, $"Downloaded {pdfBytes.Length} bytes after retries", pdfBytes.Length);
                }
                
                return new TestResult("HttpClient with Retry", false, "All retries failed", 0);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Retry", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithTimeout()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("timeout_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Timeout", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Timeout", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithCompression()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("compression_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Compression", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Compression", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithBasicAuth()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password")));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("basicauth_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Basic Auth", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Basic Auth", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithBearerToken()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token-123");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("bearer_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Bearer Token", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Bearer Token", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithSessionCookies()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Add("Cookie", "JSESSIONID=test123; IRS_SESSION=test456");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("cookies_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Session Cookies", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Session Cookies", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithAcceptHeaders()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("accept_headers_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Accept Headers", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Accept Headers", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithFormData()
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
                await SaveTestFile("formdata_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Form Data", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Form Data", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithJsonPayload()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var jsonContent = new StringContent("{\"action\":\"download\",\"type\":\"pdf\"}", Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync(_testUrl, jsonContent);
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await SaveTestFile("json_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with JSON Payload", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with JSON Payload", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithConnectionPooling()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("connection_pooling_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Connection Pooling", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Connection Pooling", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithKeepAlive()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.DefaultRequestHeaders.Add("Keep-Alive", "timeout=5, max=1000");
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("keepalive_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with Keep-Alive", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with Keep-Alive", false, ex.Message, 0);
            }
        }

        private async Task<TestResult> TestHttpClientWithHttpVersions()
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                httpClient.DefaultRequestVersion = new Version(2, 0);
                httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                
                var pdfBytes = await httpClient.GetByteArrayAsync(_testUrl);
                await SaveTestFile("http2_httpclient.pdf", pdfBytes);
                
                return new TestResult("HttpClient with HTTP/2", true, $"Downloaded {pdfBytes.Length} bytes", pdfBytes.Length);
            }
            catch (Exception ex)
            {
                return new TestResult("HttpClient with HTTP/2", false, ex.Message, 0);
            }
        }

        private async Task SaveTestFile(string fileName, byte[] content)
        {
            var filePath = Path.Combine(_outputDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, content);
            _logger.LogDebug("Saved test file: {FilePath} ({Size} bytes)", filePath, content.Length);
        }

        private void PrintResults(List<TestResult> results)
        {
            _logger.LogInformation("📊 PDF Download Test Results:");
            _logger.LogInformation("=" * 80);
            
            foreach (var result in results)
            {
                var status = result.Success ? "✅ SUCCESS" : "❌ FAILED";
                _logger.LogInformation("{Status} | {Method} | Size: {FileSize} bytes | {Details}", 
                    status, result.Method, result.FileSize, result.Details);
            }

            var successCount = results.Count(r => r.Success);
            _logger.LogInformation("=" * 80);
            _logger.LogInformation("📈 SUMMARY: {SuccessCount}/{TotalCount} methods succeeded ({SuccessRate:F1}%)", 
                successCount, results.Count, (double)successCount / results.Count * 100);
            
            if (successCount > 0)
            {
                var bestResult = results.Where(r => r.Success).OrderByDescending(r => r.FileSize).First();
                _logger.LogInformation("🏆 BEST RESULT: {Method} - {FileSize} bytes", bestResult.Method, bestResult.FileSize);
            }
        }

        public class TestResult
        {
            public string Method { get; }
            public bool Success { get; }
            public string Details { get; }
            public long FileSize { get; }

            public TestResult(string method, bool success, string details, long fileSize)
            {
                Method = method;
                Success = success;
                Details = details;
                FileSize = fileSize;
            }
        }
    }
}
