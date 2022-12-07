using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GCP.GCPClass ;
using System.Data;


using MySql.Data.MySqlClient;

namespace GCP.Databases {
    public class Connection {
        private MySqlConnection _connection ;

        // with custom port
        public Connection(string host, int port, string databaseName , string username, string password ) {

            string connectionString = "SERVER="+host+":"+port+"; DATABASE="+databaseName+"; UID="+username+"; PASSWORD="+password;

            _connection = new MySqlConnection(connectionString);
        }

        // with default port
        public Connection(string host, string username, string databaseName, string password ) {

            string connectionString = "SERVER="+host+"; DATABASE="+databaseName+"; UID="+username+"; PASSWORD="+password;

            _connection = new MySqlConnection(connectionString);

        }

        public string GetUsername(CoProprietaire c) {
            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection ;
            cmd.CommandText = @"
                    SELECT login
                    FROM compte 
                    WHERE id = @id ; 
                    " ;
            cmd.Parameters.AddWithValue("@id", c.Id) ;

            cmd.Prepare() ;

            return cmd.ExecuteScalar().ToString();           
        }


        public bool ConnectionAttempt(LoginInformation li) {

                       if (_connection.State !=  ConnectionState.Open) {
                    _connection.Open() ;
                 }

            if (li.ValidLoginInformation()) {
                try {

                    

                    //Console.WriteLine("le mot de passe : "+li.Password) ;  
                    MySqlCommand cmd = _connection.CreateCommand() ;
                    cmd.Connection = _connection ;
                    cmd.CommandText = @"
                    SELECT COUNT(*) 
                    FROM compte 
                    WHERE login = @log AND password = @pass ; 
                    " ;

                    cmd.Parameters.AddWithValue("@log", li.Login) ;
                    cmd.Parameters.AddWithValue("@pass", li.Password) ;


                    /*
                    for insert 
                    cmd.Parameters.AddWithValue("@Log", li.Login) ;
                    cmd.Parameters.AddWithValue("@Pass", li.Password) ;
*/
                    //Console.WriteLine(cmd.CommandText) ;
                    cmd.Prepare();

                   

                    return (int.Parse(cmd.ExecuteScalar().ToString()) == 1);
            }
            catch(Exception e) {
                    Console.WriteLine("[ERROR] : " + e.Message) ;
                    Console.WriteLine(e.StackTrace) ; 
                }
            }
            return false;
         
        }




        public bool AddVote(int idpro, int idco, bool pourOuContre ) {
             if (_connection.State ==  ConnectionState.Closed) {
                    _connection.Open() ;
                 }

            try {
            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection;

            // a protégé
            cmd.CommandText = "INSERT INTO `vote`(`idPro`, `idCo`, `an`, `pourOuContre`) VALUES('"+idpro+"','"+idco+"', '"+(pourOuContre ? "1" : "0") +"' ) ; " ;
            }
            catch {
                return false ;
            }
            
            return true ;
        }


        public List<Proposition> GetPropositions() {
            List<Proposition> res =  new List<Proposition>() ;
             if (_connection.State ==  ConnectionState.Closed) {
                    _connection.Open() ;
                 }

            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection ;
            cmd.CommandText = "SELECT libelle, idArt FROM proposition;" ;

            using (MySqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.Read()) {
                    while(reader.Read()) {
                        res.Add(new Proposition(reader.GetString(0), reader.GetString(1))) ;
                    }
                }
            }
            
            return res ;      
        }


        public int GetAccountID(LoginInformation li) {
                       if (_connection.State !=  ConnectionState.Open) {
                    _connection.Open() ;
                 }
            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection ;
            cmd.CommandText = "SELECT id FROM compte WHERE login = @log ;" ;

            cmd.Parameters.AddWithValue("@log", li.Login) ;

            cmd.Prepare() ;

            return int.Parse(cmd.ExecuteScalar().ToString());
        }



        public CoProprietaire GetCoProprio(int id ) {
             if (_connection.State !=  ConnectionState.Open) {
                    _connection.Open() ;
                 }

            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection ;
            cmd.CommandText = " SELECT id,nom, prenom, tantieme FROM coproprietaire WHERE id = @id LIMIT 1 ; " ;
            cmd.Parameters.AddWithValue("@id", id) ;

   

            cmd.Prepare() ;

            using MySqlDataReader rdr = cmd.ExecuteReader();

            //_connection.Close() ;
            while (rdr.Read())
            {
                return new CoProprietaire(rdr.GetInt32(0),rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3) );
            }
            return new CoProprietaire(0,"error", "error", 0) ;
        }

/*
        public List<Proposition> GetProposalsNotVoted() {

        }
*/



        public List<CoProprietaire> GetCoProprios() {
            if (_connection.State !=  ConnectionState.Open) {
                    _connection.Open() ;
                 }

            List<CoProprietaire> res = new List<CoProprietaire>() ;  

            MySqlCommand cmd = _connection.CreateCommand() ;
            cmd.Connection = _connection ; 
            cmd.CommandText = "SELECT id,nom, prenom, tantieme FROM coproprietaire; " ;

            using (MySqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.HasRows) {
                      while (reader.Read())
                    {
                    res.Add(new CoProprietaire(reader.GetInt32(0),reader.GetString(1), reader.GetString(2), reader.GetInt32(3)) ) ;
                    }
                }
            }

           
            
            return res ; 
        }
    }
}