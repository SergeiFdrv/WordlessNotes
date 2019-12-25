using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using SQLite;
using Notes.Models;

namespace Notes.Data
{
    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
        }

        public Task<List<Note>> GetNotesAsync() => _database.Table<Note>().ToListAsync();

        public Task<Note> GetNoteAsync(int id) => _database.Table<Note>().Where(i => i.ID == id).FirstOrDefaultAsync();

        public Task<int> SaveNoteAsync(Note note) => (note.ID == 0) ? _database.InsertAsync(note) : _database.UpdateAsync(note);

        public Task<int> DeleteNoteAsync(Note note) => _database.DeleteAsync(note);
    }
}

// SQLite.NET tutorial: https://docs.microsoft.com/ru-ru/xamarin/get-started/quickstarts/database?pivots=windows
// For Android: https://docs.microsoft.com/ru-ru/xamarin/android/data-cloud/data-access/using-sqlite-orm