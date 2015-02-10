using Newtonsoft.Json;
using SharpCompress.Compressor.BZip2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
//using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace AnkiWP
{
    public class SyncThread
    {
        private string m_path;
        private string m_hostKey;
        private string m_auth;
        private string m_media;
        private string m_mediaUsn;
        private string m_server;
        private Syncer m_client;
        private int m_sentTotal;
        private int m_receivedTotal;
        private double m_byteUpdate;
        private string m_collection;
        private SyncOptions m_fullSyncChoice;

        public SyncThread(string path, string hostKey, string auth, string media)
        {
            m_path = path;
            m_hostKey = hostKey;
            m_auth = auth;
            m_media = media;
        }
        
        public void Run()
        {
            try
            {
                //m_collection = Collection(m_path);
            }
            catch (Exception)
            {
                //Event "corrupt"
                throw;
            }

            //m_server = new RemoteServer(m_hKey);
            m_client = new Syncer(m_collection, m_server);
            m_sentTotal = 0;
            m_receivedTotal = 0;
            m_byteUpdate = DateTime.Now.TimeOfDay.TotalSeconds;

            // throttle updates because UI is slow
            // addHook("sync", syncEvent)
            // addHook("httpSend", sendEvent)
            // addHook("httpRecv", recvEvent)

            // run sync and catch any errors
            try
            {
                //Sync();
            }
            catch (Exception)
            {
                //err = traceback.format_exc()
                //if not isinstance(err, unicode):
                //    err = unicode(err, "utf8", "replace")
                //self.fireEvent("error", err)
                throw;
            }
            finally
            {
                //# don't bump mod time unless we explicitly save
                //self.col.close(save=False)
                //remHook("sync", syncEvent)
                //remHook("httpSend", sendEvent)
                //remHook("httpRecv", recvEvent)
            }
        }

        public void Sync()
        {
            if (!string.IsNullOrWhiteSpace(m_auth))
            {
                //# need to authenticate and obtain host key
                //m_hostKey = m_server.hostKey(auth);
                if (string.IsNullOrWhiteSpace(m_hostKey))
                {
                    //# provided details were invalid
                    //return self.fireEvent("badAuth")
                }
                else
                {
                    //# write new details and tell calling thread to save
                    //self.fireEvent("newKey", self.hkey)
                }
            }

            // run sync and check state
            SyncResult result;
            try
            {
                result = m_client.Sync();
            }
            catch (Exception)
            {
                //log = traceback.format_exc()
                //try:
                //    err = unicode(e[0], "utf8", "ignore")
                //except:
                //    # number, exception with no args, etc
                //    err = ""
                //if "Unable to find the server" in err:
                //    self.fireEvent("offline")
                //else:
                //    if not err:
                //        err = log
                //    if not isinstance(err, unicode):
                //        err = unicode(err, "utf8", "replace")
                //    self.fireEvent("error", err)
                //return
                throw;
            }

            switch (result)
            {
                case SyncResult.BadAuth:
                    //return self.fireEvent("badAuth")
                    break;
                case SyncResult.ClockOff:
                    //return self.fireEvent("clockOff")
                    break;
                case SyncResult.BasicCheckFailed:
                case SyncResult.SanityCheckFailed:
                    //return self.fireEvent("checkFailed")
                    break;
            }

            // note mediaUSN for later
            //m_mediaUsn = m_client.MediaUsn;

            // full sync?
            switch (result)
            {
                case SyncResult.FullSync:
                    // return FullSync();
                    break;
                case SyncResult.NoChanges:
                    //self.fireEvent("noChanges")
                    break;
                case SyncResult.Success:
                    //self.fireEvent("success")
                    break;
            }

            // then move on to media sync
            SyncMedia();
        }

        private enum SyncOptions
        {
            AwaitingChoice,
            Download,
            Cancel,
            Upload,
        };

        private void FullSync()
        {
            SyncOptions fullSyncOption = SyncOptions.Cancel;
            // if the local deck is empty, assume user is trying to download
            //if (m_collection.IsEmpty())
            if (true)
            {
                fullSyncOption = SyncOptions.Download;
            }
            else
            {
                // tell the calling thread we need a decision on sync direction, and
                // wait for a reply

                m_fullSyncChoice = SyncOptions.AwaitingChoice;
                //self.fireEvent("fullSync")

                while (m_fullSyncChoice == SyncOptions.AwaitingChoice)
                {
                    //Thread.Sleep(100);
                }

                fullSyncOption = m_fullSyncChoice;
            }

            if (fullSyncOption == SyncOptions.Cancel)
            {
                return;
            }

            //m_client = FullSyncer(m_collection, m_hostKey, m_server.Con);
            if (fullSyncOption == SyncOptions.Upload)
            {
                //if (!m_client.Upload())
                if (true)
                {
                    //self.fireEvent("upbad")
                }
            }
            else
            {
                //m_client.Download();
            }

            // reopen db and move on to media sync
            //m_collection.Reopen();
            SyncMedia();
        }

        private void SyncMedia()
        {
            if (m_media != null)
            {
                return;
            }

            //m_server = RemoteMediaServer(m_hostKey, m_server.Con);
            //m_client = MediaSyncer(m_collection, m_server);
            SyncResult result = SyncResult.NoChanges; //m_client.Sync(m_mediaUsn);

            switch (result)
            {
                case SyncResult.NoChanges:
                    //self.fireEvent("noMediaChanges")
                    break;
                case SyncResult.SanityCheckFailed:
                    //self.fireEvent("mediaSanity")
                    break;
                default:
                    //self.fireEvent("mediaSuccess")
                    break;
            }
        }

        private void syncEvent()
        {
            // self.fireEvent("sync", type)
        }

        private bool canPost()
        {
            if (DateTime.Now.TimeOfDay.TotalSeconds - m_byteUpdate > 0.1)
            {
                m_byteUpdate = DateTime.Now.TimeOfDay.TotalSeconds;
                return true;
            }

            return false;
        }

        private void sendEvent(int bytes)
        {
            m_sentTotal += bytes;
            if (canPost())
            {
                // self.fireEvent("send", self.sentTotal)
            }
        }

        private void receiveEvent(int bytes)
        {
            m_receivedTotal += bytes;
            if (canPost())
            {
                // lf.fireEvent("recv", self.recvTotal)
            }
        }
    }

    public enum SyncResult
    {
        BadAuth,
        ClockOff,
        BasicCheckFailed,
        SanityCheckFailed,
        FullSync,
        NoChanges,
        Success,
    }

    public class Syncer
    {
        private string m_collection;
        private string m_server;
        private int m_rmod;
        private int m_lmod;
        private int m_maxUsn;
        private int m_minUsn;
        private int m_mediaUsn;
        private int m_rchg;
        private bool m_lNewer = false;

        public Syncer(string collection, string server)
        {
            m_collection = collection;
            m_server = server;
        }

        public SyncResult Sync()
        {
            //# if the deck has any pending changes, flush them first and bump mod time
            //m_collection.Save();

            // step 1: login & metadata
            //runHook("sync", "login")

            int rscm = 0;
            int lscm = 0;

            // timestamps
            double rts = 0.0;
            double lts = 0.0;

            bool result = true; // m_server.Meta(out );
            if (!result)
            {
                return SyncResult.BadAuth;
            }

            //self.rmod, rscm, self.maxUsn, rts, self.mediaUsn = ret
            
            Metadata clientMeta = Meta();
            m_lmod = clientMeta.Mod;
            lscm = clientMeta.Scm;
            m_minUsn = clientMeta.Usn;
            lts = clientMeta.Time;

            if (Math.Abs(rts - lts) > 300)
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

            m_lNewer = m_lmod > m_rmod;
            // step 1.5: check collection is valid
            //if (m_collection.BasicCheck())
            if (false)
            {
                return SyncResult.BasicCheckFailed;
            }

            // step 2: deletions
            //runHook("sync", "meta")
            bool lrem = true;// Removed();
            bool rrem = true; //self.server.start(minUsn=self.minUsn, lnewer=self.lnewer, graves=lrem)
            //Remove(rrem);

            // ...and small objects
            bool lchg = true; //Changes();
            bool rchg = true; //self.server.applyChanges(changes=lchg)
            //MergeChanges(lchg, rchg);

            // step 3: stream large tables from server
            //runHook("sync", "server")
            while (true)
            {
                //runHook("sync", "stream")
                //chunk = self.server.chunk()
                //self.applyChunk(chunk=chunk)
                //    if chunk['done']:
                //        break
            }

            // step 4: stream to server
            //runHook("sync", "client")
            while (true)
            {
                //runHook("sync", "stream")
                //chunk = self.chunk()
                //self.server.applyChunk(chunk=chunk)
                //if chunk['done']:
                //    break
            }

            // step 5: sanity check
            //runHook("sync", "sanity")
            bool check = true; //SanityCheck();
            //ret = self.server.sanityCheck2(client=c)
            //if ret['status'] != "ok":
            //    # roll back and force full sync
            //    self.col.rollback()
            //    self.col.modSchema()
            //    self.col.save()
            //    return "sanityCheckFailed"


            // finalize
            //runHook("sync", "finalize")
            //int mod = m_server.Finish();
            //Finish(mod);
            return SyncResult.Success;
        }

        public Metadata Meta()
        {
            return new Metadata
            {
                //Mod = m_collection.Mod,
                //Scm = m_collection.Scm,
                //Usn = m_collection.Usn,
                Time = IntTime(),
                MediaUsn = 0
            };
        }

        public int Changes()
        {
            // Bundle up small objects.
            //d = dict(models=self.getModels(),
            //         decks=self.getDecks(),
            //         tags=self.getTags())
            //if self.lnewer:
            //    d['conf'] = self.getConf()
            //    d['crt'] = self.col.crt
            //return d
            return 0;
        }

        public int ApplyChanges(int changes)
        {
            m_rchg = changes;
            int lchg = Changes();
            // merge our side before returning
            MergeChanges(lchg, m_rchg);
            return lchg;
        }

        public void MergeChanges(int lchg, int rchg)
        {
            // then the other objects
            //self.mergeModels(rchg['models'])
            //self.mergeDecks(rchg['decks'])
            //self.mergeTags(rchg['tags'])
            //if 'conf' in rchg:
            //    self.mergeConf(rchg['conf'])
            //# this was left out of earlier betas
            //if 'crt' in rchg:
            //    self.col.crt = rchg['crt']
            //self.prepareToChunk()
        }

        public void SanityCheck()
        {
            //if not self.col.basicCheck():
            //    return "failed basic check"
            //for t in "cards", "notes", "revlog", "graves":
            //    if self.col.db.scalar(
            //        "select count() from %s where usn = -1" % t):
            //        return "%s had usn = -1" % t
            //for g in self.col.decks.all():
            //    if g['usn'] == -1:
            //        return "deck had usn = -1"
            //for t, usn in self.col.tags.allItems():
            //    if usn == -1:
            //        return "tag had usn = -1"
            //found = False
            //for m in self.col.models.all():
            //    if self.col.server:
            //        # the web upgrade was mistakenly setting usn
            //        if m['usn'] < 0:
            //            m['usn'] = 0
            //            found = True
            //    else:
            //        if m['usn'] == -1:
            //            return "model had usn = -1"
            //if found:
            //    self.col.models.save()
            //self.col.sched.reset()
            //# check for missing parent decks
            //self.col.sched.deckDueList()
            //# return summary of deck
            //return [
            //    list(self.col.sched.counts()),
            //    self.col.db.scalar("select count() from cards"),
            //    self.col.db.scalar("select count() from notes"),
            //    self.col.db.scalar("select count() from revlog"),
            //    self.col.db.scalar("select count() from graves"),
            //    len(self.col.models.all()),
            //    len(self.col.decks.all()),
            //    len(self.col.decks.allConf()),
            //]
        }

        public void SanityCheck2(int client)
        {
            //server = self.sanityCheck()
            //if client != server:
            //    return dict(status="bad", c=client, s=server)
            //return dict(status="ok")
        }

        public void UsnLim()
        {
            //if self.col.server:
            //    return "usn >= %d" % self.minUsn
            //else:
            //    return "usn = -1"
        }

        public void Finish()
        {
            //if not mod:
            //    # server side; we decide new mod time
            //    mod = intTime(1000)
            //self.col.ls = mod
            //self.col._usn = self.maxUsn + 1
            //# ensure we save the mod time even if no changes made
            //self.col.db.mod = True
            //self.col.save(mod=mod)
            //return mod
        }

        public int IntTime(int scale = 1)
        {
            // The time in integer seconds. Pass scale=1000 to get milliseconds.
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int timestamp  = (int)t.TotalSeconds;
            return timestamp * scale;
        }

        public void PrepareToChuck()
        {
            //self.tablesLeft = ["revlog", "cards", "notes"]
            //self.cursor = None
        }

        public void CursorForTable(int table)
        {
    //        lim = self.usnLim()
    //        x = self.col.db.execute
    //        d = (self.maxUsn, lim)
    //        if table == "revlog":
    //            return x("""
    //select id, cid, %d, ease, ivl, lastIvl, factor, time, type
    //from revlog where %s""" % d)
    //        elif table == "cards":
    //            return x("""
    //select id, nid, did, ord, mod, %d, type, queue, due, ivl, factor, reps,
    //lapses, left, odue, odid, flags, data from cards where %s""" % d)
    //        else:
    //            return x("""
    //select id, guid, mid, mod, %d, tags, flds, '', '', flags, data
    //from notes where %s""" % d)

        }

        public void Chunk()
        {
            //buf = dict(done=False)
            //lim = 2500
            //while self.tablesLeft and lim:
            //    curTable = self.tablesLeft[0]
            //    if not self.cursor:
            //        self.cursor = self.cursorForTable(curTable)
            //    rows = self.cursor.fetchmany(lim)
            //    fetched = len(rows)
            //    if fetched != lim:
            //        # table is empty
            //        self.tablesLeft.pop(0)
            //        self.cursor = None
            //        # if we're the client, mark the objects as having been sent
            //        if not self.col.server:
            //            self.col.db.execute(
            //                "update %s set usn=? where usn=-1"%curTable,
            //                self.maxUsn)
            //    buf[curTable] = rows
            //    lim -= fetched
            //if not self.tablesLeft:
            //    buf['done'] = True
            //return buf
        }

        public void ApplyChunk(int chunk)
        {
            //if "revlog" in chunk:
            //    self.mergeRevlog(chunk['revlog'])
            //if "cards" in chunk:
            //    self.mergeCards(chunk['cards'])
            //if "notes" in chunk:
            //    self.mergeNotes(chunk['notes'])
        }

        public void Removed()
        {
            //cards = []
            //notes = []
            //decks = []
            //if self.col.server:
            //    curs = self.col.db.execute(
            //        "select oid, type from graves where usn >= ?", self.minUsn)
            //else:
            //    curs = self.col.db.execute(
            //        "select oid, type from graves where usn = -1")
            //for oid, type in curs:
            //    if type == REM_CARD:
            //        cards.append(oid)
            //    elif type == REM_NOTE:
            //        notes.append(oid)
            //    else:
            //        decks.append(oid)
            //if not self.col.server:
            //    self.col.db.execute("update graves set usn=? where usn=-1",
            //                         self.maxUsn)
            //return dict(cards=cards, notes=notes, decks=decks)
        }

        public void Start(int minUsn, bool lnewer, int graves)
        {
            //self.maxUsn = self.col._usn
            //self.minUsn = minUsn
            //self.lnewer = not lnewer
            //lgraves = self.removed()
            //self.remove(graves)
            //return lgraves
        }

        public void Remove(int graves)
        {
            //# pretend to be the server so we don't set usn = -1
            //wasServer = self.col.server
            //self.col.server = True
            //# notes first, so we don't end up with duplicate graves
            //self.col._remNotes(graves['notes'])
            //# then cards
            //self.col.remCards(graves['cards'], notes=False)
            //# and decks
            //for oid in graves['decks']:
            //    self.col.decks.rem(oid, childrenToo=False)
            //self.col.server = wasServer

        }

        public void GetModels()
        {
            //if self.col.server:
            //    return [m for m in self.col.models.all() if m['usn'] >= self.minUsn]
            //else:
            //    mods = [m for m in self.col.models.all() if m['usn'] == -1]
            //    for m in mods:
            //        m['usn'] = self.maxUsn
            //    self.col.models.save()
            //    return mods
        }

        public void MergeModels(int rchg)
        {
            //for r in rchg:
            //    l = self.col.models.get(r['id'])
            //    # if missing locally or server is newer, update
            //    if not l or r['mod'] > l['mod']:
            //        self.col.models.update(r)
        }

        public void GetDecks()
        {
            //if self.col.server:
            //    return [
            //        [g for g in self.col.decks.all() if g['usn'] >= self.minUsn],
            //        [g for g in self.col.decks.allConf() if g['usn'] >= self.minUsn]
            //    ]
            //else:
            //    decks = [g for g in self.col.decks.all() if g['usn'] == -1]
            //    for g in decks:
            //        g['usn'] = self.maxUsn
            //    dconf = [g for g in self.col.decks.allConf() if g['usn'] == -1]
            //    for g in dconf:
            //        g['usn'] = self.maxUsn
            //    self.col.decks.save()
            //    return [decks, dconf]
        }

        public void MergeDecks(int rchg)
        {
            //for r in rchg[0]:
            //    l = self.col.decks.get(r['id'], False)
            //    # if missing locally or server is newer, update
            //    if not l or r['mod'] > l['mod']:
            //        self.col.decks.update(r)
            //for r in rchg[1]:
            //    try:
            //        l = self.col.decks.getConf(r['id'])
            //    except KeyError:
            //        l = None
            //    # if missing locally or server is newer, update
            //    if not l or r['mod'] > l['mod']:
            //        self.col.decks.updateConf(r)

        }

        public void GetTags()
        {
            //if self.col.server:
            //    return [t for t, usn in self.col.tags.allItems()
            //            if usn >= self.minUsn]
            //else:
            //    tags = []
            //    for t, usn in self.col.tags.allItems():
            //        if usn == -1:
            //            self.col.tags.tags[t] = self.maxUsn
            //            tags.append(t)
            //    self.col.tags.save()
            //    return tags

        }

        public void MergeTags(int tags)
        {
            //self.col.tags.register(tags, usn=self.maxUsn)

        }

        public void MergeRevlog(int logs)
        {
            //self.col.db.executemany(
            //    "insert or ignore into revlog values (?,?,?,?,?,?,?,?,?)",
            //    logs)

        }

        public void NewerRows(int data, int table, int modIdx)
        {
            //ids = (r[0] for r in data)
            //lmods = {}
            //for id, mod in self.col.db.execute(
            //    "select id, mod from %s where id in %s and %s" % (
            //        table, ids2str(ids), self.usnLim())):
            //    lmods[id] = mod
            //update = []
            //for r in data:
            //    if r[0] not in lmods or lmods[r[0]] < r[modIdx]:
            //        update.append(r)
            //return update

        }

        public void MergeCards(int cards)
        {
            //self.col.db.executemany(
            //    "insert or replace into cards values "
            //    "(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
            //    self.newerRows(cards, "cards", 4))

        }

        public void MergeNotes(int notes)
        {
            //rows = self.newerRows(notes, "notes", 3)
            //self.col.db.executemany(
            //    "insert or replace into notes values (?,?,?,?,?,?,?,?,?,?,?)",
            //    rows)
            //self.col.updateFieldCache([f[0] for f in rows])

        }

        public void GetConf()
        {
            //return self.col.conf

        }

        public void MergeConf(int conf)
        {
            //self.col.conf = conf

        }
    }

    public class Metadata
    {
        public int Mod { get; set; }
        public int Scm { get; set; }
        public int Usn { get; set; }
        public int Time { get; set; }
        public int MediaUsn { get; set; }
    }

    public /*abstract */class HttpSyncer
    {
        private string m_connection;
        private string m_hostKey;


        public HttpSyncer(string connection, string hostKey = null)
        {
            m_hostKey = hostKey;
            m_connection = connection;
        }

        public static async Task<string> CreateHttpConnection()
        {
            string theData = string.Empty;

            try
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/ankiweb.certs"));
                var readStream = await storageFile.OpenStreamForReadAsync();
                using (var reader = new StreamReader(readStream))
                {
                    theData = await reader.ReadToEndAsync();
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("Couldn't find the file 'Data/ankiweb.certs' in app storage. Syncing will not be possible.");
            }
            
            //You must change the path to point to your .cer file location. 
    //X509Certificate Cert = X509Certificate.CreateFromCertFile("C:\\mycert.cer");
    // Handle any certificate errors on the certificate from the server.
    //ServicePointManager.CertificatePolicy = new CertPolicy();
    // You must change the URL to point to your Web server.

            //HttpWebRequest request = WebRequest.CreateHttp(new Uri("https://ankiweb.net/sync/"));
            //request.Method = "POST";
            ////request.Headers =
            //request.
            
             
            
            
            //System.Net.HttpWebRequest
            

            //http.Credentials = new NetworkCredential()
            return theData;
            

            //certs = os.path.join(os.path.dirname(__file__), "ankiweb.certs")
            //if not os.path.exists(certs):
            //    if isWin:
            //        certs = os.path.join(
            //            os.path.dirname(os.path.abspath(sys.argv[0])),
            //            "ankiweb.certs")
            //    elif isMac:
            //        certs = os.path.join(
            //            os.path.dirname(os.path.abspath(sys.argv[0])),
            //            "../Resources/ankiweb.certs")
            //    else:
            //        assert 0, "Your distro has not packaged Anki correctly."
            //return httplib2.Http(
            //    timeout=HTTP_TIMEOUT, ca_certs=certs,
            //    proxy_info=HTTP_PROXY,
            //    disable_ssl_certificate_validation=not not HTTP_PROXY)
            //return certs;
        }

        public void AssertOk(int resp)
        {
            //if resp['status'] != '200':
            //    raise Exception("Unknown response code: %s" % resp['status'])
        }

        public async Task<string> Request(string method, MemoryStream fobj = null, int comp = 6, bool badAuthRaises = true, bool hostKey = true)
        {
            string BOUNDARY = "Anki-sync-boundary";
            string boundary = "--" + BOUNDARY;
            using (var buffer = new MemoryStream())
            {
                using (var buf = new StreamWriter(buffer))
                {
                    //MemoryStream buf = new MemoryStream();
                    //StringBuilder buf = new StringBuilder();

                    // compression flag and session key as post vars
                    Dictionary<string, object> vars = new Dictionary<string, object>();
                    vars["c"] = comp > 0 ? 1 : 0;
                    if (hostKey)
                    {
                        vars["k"] = m_hostKey;
                    }

                    foreach (var item in vars)
                    {
                        await buf.WriteAsync(boundary + "\r\n");
                        await buf.WriteAsync(string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n", item.Key, item.Value));
                    }

                    // payload as raw data or json
                    if (fobj != null)
                    {
                        // header
                        await buf.WriteAsync(boundary + "\r\n");
                        await buf.WriteAsync("Content-Disposition: form-data; name=\"data\"; filename=\"data\"\r\nContent-Type: application/octet-stream\r\n\r\n");

                        await buf.FlushAsync();

                        fobj.Seek(0, SeekOrigin.Begin);

                        var fobjAsArray = fobj.ToArray();

                        if (comp > 0)
                        {
                            using (var zippedStream = new MemoryStream())
                            {
                                using (BZip2Stream bzip = new BZip2Stream(zippedStream, SharpCompress.Compressor.CompressionMode.Compress))
                                {
                                    await bzip.WriteAsync(fobjAsArray, 0, fobjAsArray.Length);
                                    await bzip.FlushAsync();

                                    var temp = zippedStream.ToArray();
                                    await buffer.WriteAsync(temp, 0, temp.Length);
                                }
                            }
                        }
                        else
                        {
                            await buffer.WriteAsync(fobjAsArray, 0, fobjAsArray.Length);
                        }
                        
                        
                        
                        //bzip.WriteAsync()
                        
                        

                        
                        // write file into buffer, optionally compressing
                        
                        //if (comp > 0)
                        //{
                        //    throw new NotImplementedException();
                        //    //tgt = gzip.GzipFile(mode="wb", fileobj=buf, compresslevel=comp)
                        //}
                        //else
                        //{
                        //    target = buf;
                        //}
                        

                        await buf.WriteAsync("\r\n" + boundary + "--\r\n");
                        await buf.FlushAsync();
                    }

                    var size = buffer.Length;

                    //buffer.Position = 0;
                    //using (var reader = new StreamReader(buffer))
                    //{
                    //    string test = reader.ReadToEnd();
                    //}

                    var uri = new Uri("https://ankiweb.net/sync/" + method);
                    HttpWebRequest request = WebRequest.CreateHttp(uri);
                    request.Method = "POST";
                    request.ContentType = string.Format("multipart/form-data; boundary={0}", BOUNDARY);
                    //request.ContentLength = size;
                    //request.Credentials = new NetworkCredential("", "");

                    try
                    {
                        using (var requestStream = (Stream)(await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null)))
                        {                            
                            var bufferAsArray = buffer.ToArray();
                            await requestStream.WriteAsync(bufferAsArray, 0, bufferAsArray.Length);

                            Debug.WriteLine("Test");
                            //requestStream.Close();
                        }
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                    string received = string.Empty;

                    //HttpWebResponse response;
                    //try
                    //{
                    //    using (response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
                    //    {
                    //        var status = response.StatusCode;

                    //        using (var responseStream = response.GetResponseStream())
                    //        {
                    //            using (var streamReader = new StreamReader(responseStream))
                    //            {
                    //                received = await streamReader.ReadToEndAsync();
                    //            }
                    //        }
                    //    }
                    //}
                    //catch (WebException webException)
                    //{
                    //    var httpStatusCode = ((HttpWebResponse)webException.Response).StatusCode;
                    //    if (httpStatusCode == HttpStatusCode.NotFound ||
                    //        httpStatusCode == HttpStatusCode.GatewayTimeout ||
                    //        httpStatusCode == HttpStatusCode.InternalServerError)
                    //    {
                    //        response = (HttpWebResponse)webException.Response;
                            

                    //        if (webException.Status == WebExceptionStatus.UnknownError)
                    //        {
                    //            //configuration.IsResending = true;
                    //            //ResendRequest(configuration, successAction);
                    //            //return;
                    //        }

                    //        //configuration.IsResending = false;
                    //        //throw new ServerTemporaryUnavailabeException();
                    //    }

                    //    Debug.WriteLine(webException.Message);
                    //}

                    //using (var responseStream = response.GetResponseStream())
                    //{
                    //    using (var streamReader = new StreamReader(responseStream))
                    //    {
                    //        received = await streamReader.ReadToEndAsync();
                    //    }
                    //}

                    var response = await HttpExtensions.GetResponseAsync(request);

                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            received = await streamReader.ReadToEndAsync();
                        }
                    }

                    //var res = request.BeginGetResponse(callbackResult =>
                    //{
                    //    try
                    //    {
                    //        var myRequest = (HttpWebRequest)callbackResult.AsyncState;
                    //        var myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                    //        var status = myResponse.StatusCode;

                    //        var stream = myResponse.GetResponseStream();


                    //        //callback(myResponse.GetResponseStream(), null);

                    //        myResponse.Close();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        //callback(Stream.Null, e);
                    //        Debug.WriteLine("Error in RequestHelper.ProcessResponse\nErrorMessage - " + e.Message);
                    //    }
                    //},
                    //request);

                    return received;

                    //        body = buf.getvalue()
                    //        buf.close()
                    //        resp, content = self.con.request(
                    //            SYNC_URL+method, "POST", headers=headers, body=body)
                    //        if not badAuthRaises:
                    //            # return false if bad auth instead of raising
                    //            if resp['status'] == '403':
                    //                return False
                    //        self.assertOk(resp)
                    //        return content
                    }
            }
        }
    }
    
    public static class HttpExtensions
    {
        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var taskComplete = new TaskCompletionSource<HttpWebResponse>();
            request.BeginGetResponse(asyncResponse =>
            {
                try
                {
                    HttpWebRequest responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
                    HttpWebResponse someResponse = (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);
                    taskComplete.TrySetResult(someResponse);
                }
                catch (WebException webExc)
                {
                    HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
                    taskComplete.TrySetResult(failedResponse);
                }
            }, request);

            return taskComplete.Task;
        }
    }

    public class RemoteServer : HttpSyncer
    {
        public RemoteServer(string hostKey)
            : base(hostKey)
        { }

        // Returns hkey or none if user/pw incorrect.
        public async Task<string> HostKey(string user, string password)
        {
            Dictionary<string, string> userPass = new Dictionary<string, string>();
            userPass[user] = password;

            string creds = string.Empty;
            try
            {
                int test = 1;
                //creds = await Newtonsoft.Json.JsonConvert.SerializeObjectAsync(userPass, Formatting.None);
                //creds = Newtonsoft.Json.JsonConvert.SerializeObject(test, Formatting.None);
                Windows.Data.Json.JsonObject jtest = new Windows.Data.Json.JsonObject
                {
                    {user, Windows.Data.Json.JsonValue.CreateStringValue(password)}
                };

                creds = jtest.Stringify();

                Debug.WriteLine(creds);
            }
            catch (Exception ex)
            {
                throw;
            }

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    await streamWriter.WriteAsync(creds);
                    await streamWriter.FlushAsync();

                    try
                    {
                        return await Request("hostKey", stream, 6, false, false);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        throw;
                    }
                }
            }

            return string.Empty;

            //ret = self.req(
            //    "hostKey", StringIO(json.dumps(dict(u=user, p=pw))),
            //    badAuthRaises=False, hkey=False)
            //if not ret:
            //    # invalid auth
            //    return
            //self.hkey = json.loads(ret)['key']
            //return self.hkey

        }

        public void Meta()
        {

            //ret = self.req(
            //    "meta", StringIO(json.dumps(dict(v=SYNC_VER))),
            //    badAuthRaises=False)
            //if not ret:
            //    # invalid auth
            //    return
            //return json.loads(ret)

        }

        public void ApplyChanges(int kw /* **kw */) 
        {

            //return self._run("applyChanges", kw)

        }

        public void Start(int kw /* **kw */)
        {

            //return self._run("start", kw)

        }

        public void Chunk(int kw /* **kw */)
        {

            //return self._run("chunk", kw)

        }

        public void ApplyChunk(int kw /* **kw */)
        {

            //return self._run("applyChunk", kw)

        }

        public void SanityCheck2(int kw /* **kw */)
        {

            //return self._run("sanityCheck2", kw)

        }

        public void Finish(int kw /* **kw */)
        {

            //return self._run("finish", kw)

        }

        public void Run(int cmd, int data)
        {

        //return json.loads(
        //    self.req(cmd, StringIO(json.dumps(data))))

        }
    }

    public class FullSyncer : HttpSyncer
    {
        private int m_collection;

        public FullSyncer(int collection, string hostKey, string con)
            : base(hostKey, con)
        {
            m_collection = collection;
        }

        public void Download()
        {
            //App.Database.Close();


            //runHook("sync", "download")
            //self.col.close()
            //cont = self.req("download")
            //tpath = self.col.path + ".tmp"
            //if cont == "upgradeRequired":
            //    runHook("sync", "upgradeRequired")
            //    return
            //open(tpath, "wb").write(cont)
            //# check the received file is ok
            //d = DB(tpath)
            //assert d.scalar("pragma integrity_check") == "ok"
            //d.close()
            //# overwrite existing collection
            //os.unlink(self.col.path)
            //os.rename(tpath, self.col.path)
            //self.col = None

        }

        public void Upload()
        {

            //"True if upload successful."
            //runHook("sync", "upload")
            //# make sure it's ok before we try to upload
            //if self.col.db.scalar("pragma integrity_check") != "ok":
            //    return False
            //if not self.col.basicCheck():
            //    return False
            //# apply some adjustments, then upload
            //self.col.beforeUpload()
            //if self.req("upload", open(self.col.path, "rb")) != "OK":
            //    return False
            //return True

        }
    }

    public class MediaSyncer
    {
        private int m_collection;
        private string m_server;
        private string m_added;

        public MediaSyncer(int collection, string server = null)
        {
            m_collection = collection;
            m_server = server;
            m_added = null;
        }

        public void Sync(int mediaUsn)
        {

            //# step 1: check if there have been any changes
            //runHook("sync", "findMedia")
            //lusn = self.col.media.usn()
            //# if first sync or resync, clear list of files we think we've sent
            //if not lusn:
            //    self.col.media.forceResync()
            //self.col.media.findChanges()
            //if lusn == mediaUsn and not self.col.media.hasChanged():
            //    return "noChanges"
            //# step 1.5: if resyncing, we need to get the list of files the server
            //# has and remove them from our local list of files to sync
            //if not lusn:
            //    files = self.server.mediaList()
            //    need = self.col.media.removeExisting(files)
            //else:
            //    need = None
            //# step 2: send/recv deletions
            //runHook("sync", "removeMedia")
            //lrem = self.removed()
            //rrem = self.server.remove(fnames=lrem, minUsn=lusn)
            //self.remove(rrem)
            //# step 3: stream files from server
            //runHook("sync", "server")
            //while 1:
            //    runHook("sync", "streamMedia")
            //    usn = self.col.media.usn()
            //    zip = self.server.files(minUsn=usn, need=need)
            //    if self.addFiles(zip=zip):
            //        break
            //# step 4: stream files to the server
            //runHook("sync", "client")
            //while 1:
            //    runHook("sync", "streamMedia")
            //    zip, fnames = self.files()
            //    if not fnames:
            //        # finished
            //        break
            //    usn = self.server.addFiles(zip=zip)
            //    # after server has replied, safe to remove from log
            //    self.col.media.forgetAdded(fnames)
            //    self.col.media.setUsn(usn)
            //# step 5: sanity check during beta testing
            //# NOTE: when removing this, need to move server tidyup
            //# back from sanity check to addFiles
            //s = self.server.mediaSanity()
            //c = self.mediaSanity()
            //if c != s:
            //    # if the sanity check failed, force a resync
            //    self.col.media.forceResync()
            //    return "sanityCheckFailed"

        }

        public void Removed(int fnames, string minUsn = null)
        {

            //return self.col.media.removed()

        }

        public void Remove()
        {

            //self.col.media.syncRemove(fnames)
            //if minUsn is not None:
            //    # we're the server
            //    return self.col.media.removed()

        }

        public void Files()
        {

            //return self.col.media.zipAdded()

        }

        public void AddFiles(bool zip)
        {

            //"True if zip is the last in set. Server returns new usn instead."
            //return self.col.media.syncAdd(zip)

        }

        public void MediaSanity()
        {

        //return self.col.media.sanityCheck()

        }
    }

    public class RemoteMediaServer : HttpSyncer
    {
        public RemoteMediaServer(string hostKey, string con)
            : base(hostKey, con)
        { }

        public void Remove(int kw /* **kw */)
        {

            //return json.loads(
            //    self.req("remove", StringIO(json.dumps(kw))))

        }

        public void Files(int kw /* **kw */)
        {

            //return self.req("files", StringIO(json.dumps(kw)))

        }

        public void AddFiles(bool zip)
        {

            //# no compression, as we compress the zip file instead
            //return json.loads(
            //    self.req("addFiles", StringIO(zip), comp=0))

        }

        public void MediaSanity()
        {

        //return json.loads(
        //    self.req("mediaSanity"))

        }

        public void MediaList()
        {

        //return json.loads(
        //    self.req("mediaList"))

        }
    }
}
