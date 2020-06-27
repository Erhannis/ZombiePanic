using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour {

	public InputField scriptText;
	public InputField scriptName;

	private Dictionary<string, string> scripts;

    public void Start() {
		this.scripts = new Dictionary<string, string>();
		IDbConnection db = getScriptDb();

		// Read and print all values in table
		IDbCommand cmnd_read = db.CreateCommand();
		IDataReader reader;
		string query = "SELECT name, text FROM scripts";
		cmnd_read.CommandText = query;
		reader = cmnd_read.ExecuteReader();

		while (reader.Read()) {
			scripts[reader[0].ToString()] = reader[1].ToString();
		}

		db.Close();
	}

    public void Awake() {
		if (!PlayerPrefs.HasKey("script")) {
			PlayerPrefs.SetString("script",
@"let n = Pos3(0,1,0); // long .x .y .z
let e = Pos3(1,0,0);
let s = Pos3(0,-1,0);
let w = Pos3(-1,0,0);
let u = Pos3(0,0,1);
let d = Pos3(0,0,-1);
while (true) {
  if (!move(n)) {
    get(n);
    if (!put(e) && !put(w) && !put(u) && !put(d)) {
      put(s);
    }
  }
}");
			PlayerPrefs.Save();
		}

		// Create database
		string connection = "URI=file:" + Application.persistentDataPath + "/" + "scripts.db";
		IDbConnection dbcon = new SqliteConnection(connection);
		dbcon.Open();

		// Create table
		{
			IDbCommand dbcmd = dbcon.CreateCommand();
			dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS scripts (name STRING PRIMARY KEY, script STRING)";
			dbcmd.ExecuteReader();
		}

		// Insert values in table
		IDbCommand cmnd = dbcon.CreateCommand();
		cmnd.CommandText = "INSERT INTO tiles (x, y, z, name) VALUES (0,0,0,'origin')";
		cmnd.ExecuteNonQuery();

		// Read and print all values in table
		IDbCommand cmnd_read = dbcon.CreateCommand();
		IDataReader reader;
		string query = "SELECT * FROM tiles";
		cmnd_read.CommandText = query;
		reader = cmnd_read.ExecuteReader();

		while (reader.Read()) {
			Debug.Log("x: " + reader[0].ToString());
			Debug.Log("y: " + reader[1].ToString());
			Debug.Log("z: " + reader[2].ToString());
			Debug.Log("name: " + reader[3].ToString());
		}

		// Close connection
		dbcon.Close();
	}

	public void save() {
		//"INSERT OR REPLACE INTO table(column_list) VALUES(value_list)"
		PlayerPrefs.SetString("script", scriptText.text);
	}

	public void load() {
		scriptText.text = PlayerPrefs.GetString("script");
	}

	public IDbConnection getScriptDb() {
		IDbConnection dbcon = new SqliteConnection("URI=file:" + Application.persistentDataPath + "/" + "scripts.db");
		dbcon.Open();

		// Create table
		{
			IDbCommand dbcmd = dbcon.CreateCommand();
			dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS scripts (name STRING, text STRING)";
			dbcmd.ExecuteReader();
		}

		// Close connection
		//dbcon.Close();

		return dbcon;
	}

	public void dbTest() {
		IDbConnection dbcon = new SqliteConnection("URI=file:" + Application.persistentDataPath + "/" + "settings.db");
		dbcon.Open();

		// Create table
		{
			IDbCommand dbcmd = dbcon.CreateCommand();
			dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS tiles (x INTEGER, y INTEGER, z INTEGER, name STRING, PRIMARY KEY(x, y, z))";
			dbcmd.ExecuteReader();
		}
		{
			IDbCommand dbcmd = dbcon.CreateCommand();
			dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS scripts (name STRING, )";
			dbcmd.ExecuteReader();
		}

		// Insert values in table
		IDbCommand cmnd = dbcon.CreateCommand();
		cmnd.CommandText = "INSERT INTO tiles (x, y, z, name) VALUES (0,0,0,'origin')";
		cmnd.ExecuteNonQuery();

		// Read and print all values in table
		IDbCommand cmnd_read = dbcon.CreateCommand();
		IDataReader reader;
		string query = "SELECT * FROM tiles";
		cmnd_read.CommandText = query;
		reader = cmnd_read.ExecuteReader();

		while (reader.Read()) {
			Debug.Log("x: " + reader[0].ToString());
			Debug.Log("y: " + reader[1].ToString());
			Debug.Log("z: " + reader[2].ToString());
			Debug.Log("name: " + reader[3].ToString());
		}

		// Close connection
		dbcon.Close();
	}
}
