using System ;

namespace GCP.GCPClass {
    
    public class CoProprietaire {

        private string _nom ;
        private string _prenom ;
        private int _nbTantieme ;

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

        public CoProprietaire(string nom, string prenom, int nbTantieme ) {
            _nom = nom ;
            _prenom = prenom;
            _nbTantieme = nbTantieme; 
        }




    }
}