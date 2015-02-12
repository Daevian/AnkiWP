using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

               /* var models = await JsonConvert.DeserializeObjectAsync<Dictionary<string, Model.Model>>(col.models);
                collection.Models = new ObservableCollection<Model.Model>(models.Values);

                var decks = await JsonConvert.DeserializeObjectAsync<Dictionary<string, Model.Deck>>(col.decks);
                collection.Decks = new ObservableCollection<Model.Deck>(decks.Values);

                collection.Tags = await JsonConvert.DeserializeObjectAsync<Dictionary<string, dynamic>>(col.tags);

                var deckConfigs = await JsonConvert.DeserializeObjectAsync<Dictionary<string, Model.DeckConfig>>(col.dconf);
                collection.DeckConfigs = new ObservableCollection<Model.DeckConfig>(deckConfigs.Values);

                collection.Config = await JsonConvert.DeserializeObjectAsync<Model.Config>(col.conf);*/
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

        /*
        public int Execute(string sqlText, params object[] args)
        {
            string sql = sqlText.Trim().ToLower();

            // mark modified?
            string[] temp = { "insert", "update", "delete" };
            foreach (var str in temp)
            {
                if (sql.StartsWith(str))
                {
                    m_modified = true;
                }
            }

            return m_db.Execute(sql, args);
        }

        public void ExecuteMany(string sqlText)
        {
            throw new NotImplementedException();
        }

        public void ExecuteScript()
        {
            throw new NotImplementedException();
        }

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
    }
}
