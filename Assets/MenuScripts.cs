using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour {

	public InputField scriptText;

    public void Start() {
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
	}

    public void save() {
		PlayerPrefs.SetString("script", scriptText.text);
	}

	public void load() {
		scriptText.text = PlayerPrefs.GetString("script");
	}

	public void dbTest() {
		// Create database
		string connection = "URI=file:" + Application.persistentDataPath + "/" + "settings.db";

		// Open connection
		IDbConnection dbcon = new SqliteConnection(connection);
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
