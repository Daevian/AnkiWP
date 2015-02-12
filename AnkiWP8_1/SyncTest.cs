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
        public static string SYNC_VER = "8";
        public static string version = "2.0.31";

        public static Col m_col = new Col();
        public static Database m_database;
        public static string m_hKey = string.Empty;
        public static string m_sKey = string.Empty;
        public static ulong m_rmod;     // mod?
        public static ulong m_lmod;     // mod?
        public static ulong m_maxUsn;   // usn?
        public static ulong m_minUsn;   // usn?
        public static ulong m_mediaUsn; // musn?
        public static string m_syncMsg; // msg?
        public static string m_uname;   // uname?
        public static bool m_lnewer;    // newer?

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
            m_col.Path = "collection.anki2";

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
                    var content = await Request("hostKey", writer.DetachBuffer(), postVars);
                    string contentString = await content.ReadAsStringAsync();
                    Windows.Data.Json.JsonObject a;
                    bool result = Windows.Data.Json.JsonObject.TryParse(contentString, out a);
                    m_hKey = a.GetNamedString("key", string.Empty);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }

            SyncResult syncResult = await Sync();
            switch (syncResult)
            {
                case SyncResult.Success:
                    break;
                case SyncResult.NoChanges:
                    break;
                case SyncResult.ServerAbort:
                    Debug.WriteLine("Server Abort: " + m_syncMsg);
                    break;
                case SyncResult.ClockOff:
                    break;
                case SyncResult.FullSync:
                    FullSync();
                    break;
                case SyncResult.BadAuth:
                    break;
                case SyncResult.BasicCheckFailed:
                    break;
                case SyncResult.SanityCheckFailed:
                    break;
                case SyncResult.CheckFailed:
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            // sync media

        }

        public static async void FullSync()
        {
            FullSyncChoice choice = FullSyncChoice.Cancel;
            if (m_col.IsEmpty)
            {
                choice = FullSyncChoice.Download;
            }
            else
            {
                // ask
            }

            switch (choice)
            {
                case FullSyncChoice.Download:
                    Download();
                    break;
                case FullSyncChoice.Upload:
                    bool success = await Upload();
                    if (!success)
                    {
                        // bad upload
                    }
                    break;
                case FullSyncChoice.Cancel:
                    return;
                default:
                    Debug.Assert(false);
                    break;
            }

            m_col.Reopen();
            // self._syncMedia()

        }

        public static async void Download()
        {
            m_col.Close();

            string v = string.Format("ankidesktop,{0},{1}", version, "win:8");

            Dictionary<string, object> postVars = new Dictionary<string, object>();
            postVars.Add("k", m_hKey);
            postVars.Add("v", v);

            var content = await Request("download", postVars: postVars);
            string contentString = await content.ReadAsStringAsync();
            if (contentString.Equals("upgradeRequired"))
            {
                return;
            }

            string tempPath = m_col.Path + ".tmp";

            var localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(tempPath, CreationCollisionOption.ReplaceExisting);
            using (var transaction = await file.OpenTransactedWriteAsync())
            {
                await content.WriteToStreamAsync(transaction.Stream);
                await transaction.CommitAsync();
            }

            var testDB = new Database(tempPath);
            string result = await testDB.Scalar("pragma integrity_check");
            testDB.Close();

            if (!result.Equals("ok"))
            {
                Debug.WriteLine("The database failed the integrity check");
                return;
            }

            await file.RenameAsync(m_col.Path, NameCollisionOption.ReplaceExisting);
        }

        public static async Task<bool> Upload()
        {
            return false;
        }

        public enum FullSyncChoice
        {
            Download,
            Upload,
            Cancel,
        }

        public enum SyncResult
        {
            Success,
            NoChanges,
            ServerAbort,
            ClockOff,
            FullSync,
            BadAuth,
            BasicCheckFailed,
            SanityCheckFailed,
            CheckFailed,
        };

        public static async Task<SyncResult> Sync()
        {
            // step 1: login & metadata
            Dictionary<string, object> postVars = new Dictionary<string, object>();
            postVars.Add("k", m_hKey);
            postVars.Add("s", m_sKey);

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

            var content = await Request("meta", buffer, postVars);
            string contentString = await content.ReadAsStringAsync();

            Windows.Data.Json.JsonObject a;
            bool result = Windows.Data.Json.JsonObject.TryParse(contentString, out a);

            ulong rscm = Convert.ToUInt64(a.GetNamedNumber("scm", 0.0));    // remote schema?
            ulong rts = Convert.ToUInt64(a.GetNamedNumber("ts", 0.0));      // remote timestamp?

            m_rmod = Convert.ToUInt64(a.GetNamedNumber("mod", 0.0));
            m_maxUsn = Convert.ToUInt64(a.GetNamedNumber("usn", 0.0));
            m_mediaUsn = Convert.ToUInt64(a.GetNamedNumber("musn", 0.0));
            m_syncMsg = a.GetNamedString("msg", string.Empty);
            m_uname = a.GetNamedString("uname", string.Empty);

            bool cont = a.GetNamedBoolean("cont", false);
            if (!cont) // continue?
            {
                return SyncResult.ServerAbort;
            }

            Meta meta = GetMeta();
            m_lmod = meta.mod;
            m_minUsn = meta.usn;
            ulong lscm = meta.scm;
            ulong lts = meta.ts;

            if (Math.Abs((long)(rts - lts)) > 300)
            {
                return SyncResult.ClockOff;
            }

            if (m_lmod == m_rmod)
            {
                return SyncResult.NoChanges;
            }
            else if (lscm != rscm)
            {
                return SyncResult.FullSync;
            }

            m_lnewer = m_lmod > m_rmod;

            // step 1.5: check collection is valid

            // step 2: deletions

            // ...and small objects

            // step 3: stream large tables from server

            // step 4: stream to server

            // step 5: sanity check

            // finalize


            return SyncResult.Success;
        }

        public static Meta GetMeta()
        {
            Meta meta;
            meta.mod = m_col.mod;
            meta.scm = m_col.scm;
            meta.usn = m_col.usn;
            meta.ts = IntTime();
            meta.musn = 0;
            meta.msg = string.Empty;
            meta.cont = true;
            return meta;
        }

        public static ulong IntTime(ulong scale = 1)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            ulong secondsSinceEpoch = (ulong)t.TotalSeconds;

            return secondsSinceEpoch * scale;
        }

        public static async Task<IHttpContent> Request(string method, IBuffer fobj = null, Dictionary<string, object> postVars = null, int comp = 6, bool badAuthRaises = false)
        {
            string BOUNDARY = "Anki-sync-boundary";
            string boundary = "--" + BOUNDARY;
            DataWriter writer = new DataWriter();
            
            // compression flag and session key as post vars
            if (postVars == null)
            {
                postVars = new Dictionary<string, object>();
            }
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
                return response.Content;
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

            return null;
        }
    }

    public class Col
    {
        public ulong mod;
        public ulong scm;
        public ulong usn;

        public string Path { get; set; }

        public bool IsEmpty
        {
            get { return true; }
        }

        public void Close()
        {

        }

        public void Reopen()
        {

        }
    }

    public struct Meta
    {
        public ulong mod;
        public ulong scm;
        public ulong usn;
        public ulong ts;
        public ulong musn;
        public string msg;
        public bool cont;
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
