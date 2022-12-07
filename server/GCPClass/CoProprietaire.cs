using System ;

namespace GCP.GCPClass {
    
    public class CoProprietaire {

        private int _id ; 
        private string _nom ;
        private string _prenom ;

        
        private int _nbTantieme ;

        public int Id {
            get { return _id ;}
        }

        public string Nom
        {
            get { return _nom; }
        }

        public string Prenom
        {
            get { return _prenom; }
        }

        public int NbTantieme
        {
            get { return _nbTantieme; }
        }

        public CoProprietaire(int id, string nom, string prenom, int nbTantieme ) {
            _id = id ; 
            _nom = nom ;
            _prenom = prenom;
            _nbTantieme = nbTantieme; 
        }
    }
}