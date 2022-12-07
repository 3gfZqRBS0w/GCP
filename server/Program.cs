using System;
using GCP.GCPClass ;
using GCP.Databases ;
using GCP.Networking ;
using System.Threading;

namespace GCP
{
    internal class Program
    {

        private static Databases.Connection _Db = new Databases.Connection("127.0.0.1", "admin","immobilier","password") ;
        private static Networking.Connection _Net = new Networking.Connection(_Db) ;

        static void Main(string[] args)
        {

            // for checking connexion request 

            
            Thread listenConnection = new Thread(_Net.ListenConnexionRequest);
            listenConnection.Start() ;
            


             




            /*
            Databases.Connection connec = new Databases.Connection("127.0.0.1", "admin","immobilier","password") ;


            List<CoProprietaire> a = connec.GetCoProprio() ;

            foreach(CoProprietaire cp in a ) {
                Console.WriteLine(@"
                nom : {0}
                prenom : {1}
                Tantieme : {2}
                ",cp.Nom, cp.Prenom, cp.NbTantieme.ToString() ) ;       
            }
            */

        }
    }
}