using AnkiWP.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AnkiWP
{
    public class col
    {
        [PrimaryKey, AutoIncrement]
        public long id { get; set; }
        public long crt { get; set; }
        public long mod { get; set; }
        public long scm { get; set; }
        public long ver { get; set; }
        public long dty { get; set; }
        public long usn { get; set; }
        public long ls { get; set; }
        public string conf { get; set; }
        public string models { get; set; }
        public string decks { get; set; }
        public string dconf { get; set; }
        public string tags { get; set; }
    }

    public class notes
    {
        [PrimaryKey, AutoIncrement]
        public long id { get; set; }
        public string guid { get; set; }
        public long mid { get; set; }
        public int mon { get; set; }
        public int usn { get; set; }
        public string tags { get; set; }
        public string flds { get; set; }
        public int sfld { get; set; }
        public int csum { get; set; }
        public int flags { get; set; }
        public string data { get; set; }
    }

    public class cards
    {
        [PrimaryKey, AutoIncrement]
        public long id { get; set; }
        public long nid { get; set; }
        public long did { get; set; }
        public int ord { get; set; }
        public int mod { get; set; }
        public int usn { get; set; }
        public int type { get; set; }
        public int queue { get; set; }
        public int due { get; set; }
        public int ivl { get; set; }
        public int factor { get; set; }
        public int reps { get; set; }
        public int lapses { get; set; }
        public int left { get; set; }
        public int odue { get; set; }
        public long odid { get; set; }
        public int flags { get; set; }
        public string data { get; set; }
    }

    public class revlog
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public int cid { get; set; }
        public int usn { get; set; }
        public int ease { get; set; }
        public int ivl { get; set; }
        public int lastIvl { get; set; }
        public int factor { get; set; }
        public int time { get; set; }
        public int type { get; set; }
    }

    public class graves
    {
        [PrimaryKey, AutoIncrement]
        public int usn { get; set; }
        public int oid { get; set; }
        public int type { get; set; }
    }
    
    public class Database
    {
        public static int SCHEMA_VERSION = 11;

        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "collection.anki2"));
        
        private SQLiteAsyncConnection m_database;
        private string m_path;
        private bool m_modified;

        private List<col> m_col;
        private List<notes> m_notes;
        private List<cards> m_cards;
        private List<revlog> m_revlog;
        private List<graves> m_graves;
        private Dictionary<string, Model.Model> m_rootData = new Dictionary<string, Model.Model>();

        public Database(string path, string text = "", int timeout = 0)
        {
            m_path = path;
            m_modified = false;

            m_database = new SQLiteAsyncConnection(m_path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, true);      
        }

        public void Close()
        {
            if (m_database != null)
            {
                var connection = SQLiteConnectionPool.Shared.GetConnection(new SQLiteConnectionString(m_path, true), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                connection.Close();
                m_database = null;
            }
        }

        public async Task Load(Model.Collection collection)
        {            
            {
                var query = m_database.Table<col>();
                m_col = await query.ToListAsync();
            }

            {
                var query = m_database.Table<notes>();
                m_notes = await query.ToListAsync();
            }

            {
                var query = m_database.Table<cards>();
                m_cards = await query.ToListAsync();
            }

            {
                var query = m_database.Table<revlog>();
                m_revlog = await query.ToListAsync();
            }

            {
                var query = m_database.Table<graves>();
                m_graves = await query.ToListAsync();
            }

            if (m_col.Count > 0)
            {
                var col = m_col[0];

                var models = JsonConvert.DeserializeObject<Dictionary<ulong, Model.Model>>(col.models);
                collection.Models = new ObservableCollection<Model.Model>(models.Values);

                var decks = JsonConvert.DeserializeObject<Dictionary<string, Model.Deck>>(col.decks);
                collection.Decks = new ObservableCollection<Model.Deck>(decks.Values);

                collection.Tags = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(col.tags);

                var deckConfigs = JsonConvert.DeserializeObject<Dictionary<string, Model.DeckConfig>>(col.dconf);
                collection.DeckConfigs = new ObservableCollection<Model.DeckConfig>(deckConfigs.Values);

                collection.Config = JsonConvert.DeserializeObject<Model.Config>(col.conf);
            }

            foreach (var card in m_cards)
            {
                collection.Cards.Add(new Model.Card(card));
            }

            foreach (var note in m_notes)
            {
                collection.Notes.Add(new Model.Note(note));
            }
        }

        public async Task Commit()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "collection_test.sqlite"), false);

            await conn.CreateTableAsync<col>();
            await conn.CreateTableAsync<notes>();
            await conn.CreateTableAsync<cards>();
            await conn.CreateTableAsync<revlog>();
            await conn.CreateTableAsync<graves>();

            await conn.ExecuteAsync("create index if not exists ix_notes_usn on notes (usn)");
            await conn.ExecuteAsync("create index if not exists ix_cards_usn on cards (usn)");
            await conn.ExecuteAsync("create index if not exists ix_revlog_usn on revlog (usn)");
            await conn.ExecuteAsync("create index if not exists ix_cards_nid on cards (nid)");
            await conn.ExecuteAsync("create index if not exists ix_cards_sched on cards (did, queue, due)");
            await conn.ExecuteAsync("create index if not exists ix_revlog_cid on revlog (cid)");
            await conn.ExecuteAsync("create index if not exists ix_notes_csum on notes (csum)");

            //await conn.InsertAllAsync(m_col);
            //await conn.InsertAllAsync(m_notes);
            //await conn.InsertAllAsync(m_cards);
            //await conn.InsertAllAsync(m_revlog);
            //await conn.InsertAllAsync(m_graves);

            await conn.InsertAllAsync(m_col);
            await conn.InsertAllAsync(m_notes);
            await conn.InsertAllAsync(m_cards);
            await conn.InsertAllAsync(m_revlog);
            await conn.InsertAllAsync(m_graves);
        }

        public async Task<string> Scalar(string sql, params object[] args)
        {
            return await m_database.ExecuteScalarAsync<string>(sql, args);
        }

        public async Task<int> Execute(string query, params object[] args)
        {
            string sql = query.Trim().ToLower();

            // mark modified?
            string[] temp = { "insert", "update", "delete" };
            foreach (var str in temp)
            {
                if (sql.StartsWith(str))
                {
                    m_modified = true;
                }
            }

            return await m_database.ExecuteAsync(query, args);
        }

        /*
        

        

        

        public void Commit()
        {
            m_db.Commit();
        }

        public void Rollback()
        {
            m_db.Rollback();
        }

        

        public void All()
        {
            throw new NotImplementedException();
        }

        public void First()
        {
            throw new NotImplementedException();
        }

        public void List()
        {
            throw new NotImplementedException();
        }

        

        public void SetProgressHandler()
        {
            throw new NotImplementedException();
        }

        public void Enter()
        {
            m_db.Execute("begin");
        }

        public void Exit()
        {
            Close();
        }

        public void Interrupt()
        {
            throw new NotImplementedException();
        }
         * */


        //public void Create()
        //{
        //    /// Create the database connection.
        //    //m_db = new SQLiteConnection(DB_PATH);

        //    /// Create the table Task, if it doesn't exist.
        //    m_db.CreateTable<Task>();
        //    /// Retrieve the task list from the database.
        //    List<Task> retrievedTasks = m_db.Table<Task>().ToList<Task>();
            
        //}

        //public void Insert()
        //{
        //    // Create a new task.
        //    Task task = new Task()
        //    {
        //        CreationDate = DateTime.Now
        //    };

        //    /// Insert the new task in the Task table.
        //    m_db.Insert(task);
        //}

        public static async Task<int> CreateDB(Database database)
        {
            Debug.Assert(database != null);

            await database.Execute("pragma page_size = 4096");
            await database.Execute("pragma legacy_file_format = 0");
            await database.Execute("vacuum");
            await Database.AddSchema(database);
            await Database.UpdateIndices(database);
            await database.Execute("analyze");
            return SCHEMA_VERSION;
        }

        private static async Task AddSchema(Database database, bool setColConf = true)
        {
            Debug.Assert(database != null);

            await database.Execute(
@"create table if not exists col (
    id              integer primary key,
    crt             integer not null,
    mod             integer not null,
    scm             integer not null,
    ver             integer not null,
    dty             integer not null,
    usn             integer not null,
    ls              integer not null,
    conf            text not null,
    models          text not null,
    decks           text not null,
    dconf           text not null,
    tags            text not null
);");

            await database.Execute(
@"create table if not exists notes (
    id              integer primary key,   /* 0 */
    guid            text not null,         /* 1 */
    mid             integer not null,      /* 2 */
    mod             integer not null,      /* 3 */
    usn             integer not null,      /* 4 */
    tags            text not null,         /* 5 */
    flds            text not null,         /* 6 */
    sfld            integer not null,      /* 7 */
    csum            integer not null,      /* 8 */
    flags           integer not null,      /* 9 */
    data            text not null          /* 10 */
);");

            await database.Execute(
@"create table if not exists cards (
    id              integer primary key,   /* 0 */
    nid             integer not null,      /* 1 */
    did             integer not null,      /* 2 */
    ord             integer not null,      /* 3 */
    mod             integer not null,      /* 4 */
    usn             integer not null,      /* 5 */
    type            integer not null,      /* 6 */
    queue           integer not null,      /* 7 */
    due             integer not null,      /* 8 */
    ivl             integer not null,      /* 9 */
    factor          integer not null,      /* 10 */
    reps            integer not null,      /* 11 */
    lapses          integer not null,      /* 12 */
    left            integer not null,      /* 13 */
    odue            integer not null,      /* 14 */
    odid            integer not null,      /* 15 */
    flags           integer not null,      /* 16 */
    data            text not null          /* 17 */
);");

            await database.Execute(
@"create table if not exists revlog (
    id              integer primary key,
    cid             integer not null,
    usn             integer not null,
    ease            integer not null,
    ivl             integer not null,
    lastIvl         integer not null,
    factor          integer not null,
    time            integer not null,
    type            integer not null
);");

            await database.Execute(
@"create table if not exists graves (
    usn             integer not null,
    oid             integer not null,
    type            integer not null
);");

            await database.Execute(string.Format(
@"insert or ignore into col
values(1,0,0,{0},{1},0,0,0,'','{{}}','','','{{}}');", SCHEMA_VERSION, Time.IntTime(1000)));

            if (setColConf)
            {
                Deck g;
                DeckConfig gc;
                DeckConfig c;
                GetColVars(out g, out gc, out c);
                await AddColVars(database, g, gc, c);
            }
        }

        private static async Task AddColVars(Database database, Deck g, DeckConfig gc, DeckConfig c)
        {
            Debug.Assert(database != null);
            Debug.Assert(g != null);
            Debug.Assert(gc != null);
            Debug.Assert(c != null);
            

            await database.Execute(
                "update col set conf = ?, decks = ?, dconf = ?",
                JsonConvert.SerializeObject(c),
                JsonConvert.SerializeObject(new Dictionary<ulong, Deck> { { 1, g } }),
                JsonConvert.SerializeObject(new Dictionary<ulong, DeckConfig> { { 1, gc } }));
        }

        private static void GetColVars(out Deck g, out DeckConfig gc, out DeckConfig c)
        {
            g = DefaultDeck.Generate();
            g.Id = 1;
            g.Name = "Default";
            g.Conf = 1;
            g.Mod = Time.IntTime();

            gc = DefaultDeckConfig.Generate();
            gc.Id = 1;

            c = DefaultDeckConfig.Generate();
        }

        private static async Task UpdateIndices(Database database)
        {
            Debug.Assert(database != null);

            await database.Execute(@"
-- syncing
create index if not exists ix_notes_usn on notes (usn);
create index if not exists ix_cards_usn on cards (usn);
create index if not exists ix_revlog_usn on revlog (usn);
-- card spacing, etc
create index if not exists ix_cards_nid on cards (nid);
-- scheduling and deck limiting
create index if not exists ix_cards_sched on cards (did, queue, due);
-- revlog by card
create index if not exists ix_revlog_cid on revlog (cid);
-- field uniqueness
create index if not exists ix_notes_csum on notes (csum);
");
        }
    }

    //public class Task
    //{
    //    /// <summary>
    //    /// You can create an integer primary key and let the SQLite control it.
    //    /// </summary>
    //    [PrimaryKey, AutoIncrement]
    //    public int Id { get; set; }

    //    public string Title { get; set; }

    //    public string Text { get; set; }

    //    public DateTime CreationDate { get; set; }

    //    public override string ToString()
    //    {
    //        return Title + ":" + Text + " < " + CreationDate.ToShortDateString() + " " + CreationDate.ToShortTimeString();
    //    }
    //}

    public static class Time
    {
        public static double getTime()
        {
            TimeSpan time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return time.TotalSeconds;
        }

        public static ulong IntTime(ulong scale = 1)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            ulong secondsSinceEpoch = (ulong)t.TotalSeconds;

            return secondsSinceEpoch * scale;
        }
    }
}
