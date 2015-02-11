using AnkiWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Certificates;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace AnkiWP8_1
{
    public class SyncTest
    {
        public static string m_hKey = string.Empty;
        public static string m_sKey = string.Empty;

        public static async void ApplicationResuming()
        {
             //var tempDatabaseFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/collection.anki2"));
            //await tempDatabaseFile.CopyAsync(ApplicationData.Current.LocalFolder, "collection.anki2", NameCollisionOption.ReplaceExisting);


            //s_database = new Database(Database.DB_PATH);
            //await s_database.Load(s_collection);
            // await s_database.Commit();

            

            try
            {
                Uri dataUri = new Uri("ms-appx:///Data/ankiweb.cer");

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

                ///*await*/ Launcher.LaunchFileAsync(certificate);
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message.ToString());
            }

            RemoteServer remoteServer = new RemoteServer(string.Empty);
            var hostKey = await remoteServer.HostKey("daeviann@live.com", "TestingTesting");

            HttpSyncer test = new HttpSyncer(await HttpSyncer.CreateHttpConnection());
            await test.Request("download", null, 0);
        }

        public static string Checksum(IBuffer data)
        {
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hash = hashAlgorithm.CreateHash();
            hash.Append(data);

            IBuffer hashBuffer = hash.GetValueAndReset();
            string hashString = CryptographicBuffer.EncodeToHexString(hashBuffer);

            return hashString;
        }

        public static async void ConnectionTest()
        {
            // skey
            {
                var random = new Random();
                var val = random.Next();
                DataWriter writer = new DataWriter();
                writer.WriteString(string.Format("{0}", val));
                m_sKey = Checksum(writer.DetachBuffer()).Substring(0, 8);
            }

            // hkey
            {
                try
                {
                    Uri dataUri = new Uri("ms-appx:///Assets/ankiweb.cer");
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    Certificate cert = new Certificate(await FileIO.ReadBufferAsync(file));
                    CertificateStore trustedStore = CertificateStores.TrustedRootCertificationAuthorities;
                    trustedStore.Add(cert);
                }
                catch (Exception ex)
                {
                    throw;
                }

                string user = "daeviann@live.com";
                string pass = "TestingTesting";

                Windows.Data.Json.JsonObject jCreds = new Windows.Data.Json.JsonObject
                    {
                        {"p", Windows.Data.Json.JsonValue.CreateStringValue(pass)},
                        {"u", Windows.Data.Json.JsonValue.CreateStringValue(user)},
                    };

                string creds = jCreds.Stringify();

                DataWriter writer = new DataWriter();
                writer.WriteString(creds);

                try
                {
                    Dictionary<string, object> postVars = new Dictionary<string, object>();
                    string content = await Request("hostKey", writer.DetachBuffer(), postVars);
                    Windows.Data.Json.JsonObject a;
                    bool result = Windows.Data.Json.JsonObject.TryParse(content, out a);
                    Windows.Data.Json.IJsonValue jValue;
                    result = a.TryGetValue("key", out jValue);
                    m_hKey = jValue.GetString();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }

            TestSync();
        }

        public static async void TestSync()
        {
            Dictionary<string, object> postVars = new Dictionary<string, object>();
            postVars.Add("k", m_hKey);
            postVars.Add("s", m_sKey);

            string SYNC_VER = "8";
            string version = "2.0.31";
            string cv = string.Format("ankidesktop,{0},{1}", version, "win:8");

            Windows.Data.Json.JsonObject jtest = new Windows.Data.Json.JsonObject
                {
                    {"v", Windows.Data.Json.JsonValue.CreateStringValue(SYNC_VER)},
                    {"cv", Windows.Data.Json.JsonValue.CreateStringValue(cv)},
                };

            DataWriter writer = new DataWriter();
            string json = jtest.Stringify();
            writer.WriteString(json);

            IBuffer buffer = writer.DetachBuffer();

            string content = await Request("meta", buffer, postVars);
            Windows.Data.Json.JsonObject a;
            bool result = Windows.Data.Json.JsonObject.TryParse(content, out a);
            Windows.Data.Json.IJsonValue jValue;
            result = a.TryGetValue("key", out jValue);
            m_hKey = jValue.GetString();
        }

        public static async Task<string> Request(string method, IBuffer fobj, Dictionary<string, object> postVars, int comp = 6, bool badAuthRaises = false)
        {
            Debug.Assert(postVars != null);

            string BOUNDARY = "Anki-sync-boundary";
            string boundary = "--" + BOUNDARY;
            DataWriter writer = new DataWriter();
            
            // compression flag and session key as post vars
            postVars["c"] = comp > 0 ? 1 : 0;

            foreach (var item in postVars)
            {
                writer.WriteString(boundary + "\r\n");
                writer.WriteString(string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n", item.Key, item.Value));
            }

            // payload as raw data or json
            if (fobj != null)
            {
                // header
                writer.WriteString(boundary + "\r\n");
                writer.WriteString("Content-Disposition: form-data; name=\"data\"; filename=\"data\"\r\nContent-Type: application/octet-stream\r\n\r\n");

                //fobj.Seek(0, SeekOrigin.Begin);

                var fobjAsArray = fobj.ToArray();

                if (comp > 0)
                {
                    using (var zippedStream = new MemoryStream())
                    {
                        using (GZipStream gzipStream = new GZipStream(zippedStream, CompressionMode.Compress, true))
                        {
                            await gzipStream.WriteAsync(fobjAsArray, 0, fobjAsArray.Length);
                            await gzipStream.FlushAsync();
                        }

                        var temp = zippedStream.ToArray();
                        writer.WriteBytes(temp);
                    }
                }
                else
                {
                    writer.WriteBytes(fobjAsArray);
                }

                writer.WriteString("\r\n" + boundary + "--\r\n");
            }

            //var size = buffer.Length;

            IBuffer contentBuffer = writer.DetachBuffer();
            //string contentString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, contentBuffer);
                    

            var uri = new Uri("https://ankiweb.net/sync/" + method);

            HttpClient httpClient = new HttpClient();
            IHttpContent content = new HttpAnkiContent(contentBuffer, BOUNDARY);
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);//.AsTask(cts.Token);

            if (response.IsSuccessStatusCode)
            {
                string resContent = await response.Content.ReadAsStringAsync();
                return resContent;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Debug.WriteLine(response.ReasonPhrase);
                }

                Debug.Assert(response.IsSuccessStatusCode);

                // - 200: wrong user/pass
                // - 501: client needs upgrade
                // - 502: ankiweb down
                // - 503/504: server too busy
            }

            return string.Empty;
        }
    }

    public class HttpAnkiContent : IHttpContent
    {
        IBuffer m_content;
        HttpContentHeaderCollection m_headers;

        public HttpContentHeaderCollection Headers
        {
            get { return m_headers; }
        }

        public HttpAnkiContent(IBuffer content, string boundary)
        {
            m_content = content;
            m_headers = new HttpContentHeaderCollection();
            m_headers.ContentType = new HttpMediaTypeHeaderValue(string.Format("multipart/form-data; boundary={0}", boundary));
            m_headers.ContentType.CharSet = "UTF-8";
            m_headers.ContentLength = (ulong)content.Length;
        }

        public IAsyncOperationWithProgress<ulong, ulong> BufferAllAsync()
        {
            return AsyncInfo.Run<ulong, ulong>((cancellationToken, progress) =>
            {
                return Task<ulong>.Run(() =>
                {
                    ulong length = m_content.Length;

                    progress.Report(length);
                    return length;
                });
            });
        }

        public IAsyncOperationWithProgress<Windows.Storage.Streams.IBuffer, ulong> ReadAsBufferAsync()
        {
            return AsyncInfo.Run<IBuffer, ulong>((cancellationToken, progress) =>
            {
                return Task<IBuffer>.Run(() =>
                {
                    progress.Report(m_content.Length);
                    return m_content;
                });
            });
        }

        public IAsyncOperationWithProgress<Windows.Storage.Streams.IInputStream, ulong> ReadAsInputStreamAsync()
        {
            return AsyncInfo.Run<IInputStream, ulong>(async (cancellationToken, progress) =>
            {
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                DataWriter writer = new DataWriter(randomAccessStream);
                writer.WriteBuffer(m_content);

                uint bytesStored = await writer.StoreAsync().AsTask(cancellationToken);

                writer.DetachStream();

                progress.Report(randomAccessStream.Size);
                return randomAccessStream.GetInputStreamAt(0);
            });
        }

        public IAsyncOperationWithProgress<string, ulong> ReadAsStringAsync()
        {
            return AsyncInfo.Run<string, ulong>((cancellationToken, progress) =>
            {
                return Task<string>.Run(() =>
                {
                    string contentString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, m_content);

                    progress.Report((ulong)contentString.Length);

                    return contentString;
                });
            });
        }

        public bool TryComputeLength(out ulong length)
        {
            length = m_content.Length;
            return true;
        }

        public IAsyncOperationWithProgress<ulong, ulong> WriteToStreamAsync(Windows.Storage.Streams.IOutputStream outputStream)
        {
            return AsyncInfo.Run<ulong, ulong>(async (cancellationToken, progress) =>
            {
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBuffer(m_content);
                uint bytesWritten = await writer.StoreAsync().AsTask(cancellationToken);

                writer.DetachStream();

                progress.Report(bytesWritten);
                return bytesWritten;
            });
        }

        public void Dispose()
        {
        }
    }
}
