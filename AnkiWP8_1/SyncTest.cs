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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace AnkiWP8_1
{
    public class SyncTest
    {
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

        public static async void ConnectionTest()
        {
            try
            {
                Uri dataUri = new Uri("ms-appx:///Assets/ankiweb.cer");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                Certificate cert = new Certificate(await FileIO.ReadBufferAsync(file));
                CertificateStore trustedStore = CertificateStores.TrustedRootCertificationAuthorities;
                trustedStore.Add(cert);


                //HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                //filter.ClientCertificate = cert;
                //
                //var buffer = await FileIO.ReadBufferAsync(file);
                //string certData = CryptographicBuffer.EncodeToBase64String(buffer);

                

                //CertificateRequestProperties test = new CertificateRequestProperties();
                //test.AttestationCredentialCertificate = cert;
                //await CertificateEnrollmentManager.UserCertificateEnrollmentManager.CreateRequestAsync(test);

                //await CertificateEnrollmentManager.UserCertificateEnrollmentManager.InstallCertificateAsync(certData, InstallOptions.None);


                //IReadOnlyList<Certificate> certs = await CertificateStores.FindAllAsync(new CertificateQuery() { Subje = "Assets/ankiweb.cer" });

                //await CertificateEnrollmentManager.UserCertificateEnrollmentManager.ImportPfxDataAsync(
                //    certData,
                //    "",
                //    ExportOption.NotExportable,
                //    KeyProtectionLevel.NoConsent,
                //    InstallOptions.None,
                //    "MyFriendlyName"); 

                
                
                

                //await CertificateEnrollmentManager.ImportPfxDataAsync(CryptographicBuffer.EncodeToBase64String(await FileIO.ReadBufferAsync(file)), "", ExportOption.NotExportable, KeyProtectionLevel.NoConsent, InstallOptions.None, "name");
                
                //filter.ClientCertificate = certs.FirstOrDefault();

                //var certificate = CryptographicBuffer.EncodeToBase64String(await FileIO.ReadBufferAsync(file));
                //await CertificateEnrollmentManager.InstallCertificateAsync(certificate, InstallOptions.None);

                //HttpClient httpClient = new HttpClient();
            }
            catch (Exception ex)
            {

                throw;
            }
            



            string user = "daeviann@live.com";
            string pass = "TestingTesting";

            Windows.Data.Json.JsonObject jtest = new Windows.Data.Json.JsonObject
                {
                    {user, Windows.Data.Json.JsonValue.CreateStringValue(pass)}
                };

            string reqMethod = "hostKey";
            //string creds = jtest.Stringify();
            string creds = "{\"p\": \"TestingTesting\", \"u\": \"daeviann@live.com\"}";

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    await streamWriter.WriteAsync(creds);
                    await streamWriter.FlushAsync();

                    try
                    {
                        Request(reqMethod, stream);
                        //return await Request("hostKey", stream, 6, false, false);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        throw;
                    }
                }
            }
        }

        public static async void Request(string method, MemoryStream fobj, int comp = 6, bool badAuthRaises = false)
        {
            string BOUNDARY = "Anki-sync-boundary";
            string boundary = "--" + BOUNDARY;
            DataWriter writer = new DataWriter();
            //using (var buffer = new MemoryStream())
            {
                //using (var buf = new StreamWriter(buffer))
                {
                    // compression flag and session key as post vars
                    Dictionary<string, object> vars = new Dictionary<string, object>();
                    vars["c"] = comp > 0 ? 1 : 0;
                    //if (hostKey)
                    //{
                    //    vars["k"] = m_hostKey;
                    //}

                    foreach (var item in vars)
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

                        fobj.Seek(0, SeekOrigin.Begin);

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

                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            Debug.WriteLine(response.ReasonPhrase);
                        }
                    }
                }
            }
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
