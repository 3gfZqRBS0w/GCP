using System;



/*
REMARQUE :

Il faut ajouter les ip aux affichage pour la traçabilité 

*/

/*
SUCCESS AND ERROR CODE

000 : Success auth 
001 : Login or Password incorrect
002 : Already connected

GET CODE

100 : Get propositions


POST CODE

250 : Publish a Vote




*/


// My Class 
using GCP.GCPClass;
using GCP.Databases;
using System.Collections;



using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


using System.Text;



namespace GCP.Networking
{
    public class Connection
    {

        // doit être la même que sur le client
        private int _listenPort = 2401;

        // en miliseconde 
        private int _maxTimeOut = 5000;

        private int _timeBeforeCheckingClientRequest = 1000;

        private string _decryptionCode = "pCw0bX$7OLQEI1!o^y%nc3^#";

        private Databases.Connection _db;

        private Thread _checkingConnectedUser;
        private Thread _checkingUserRequest;

        private ASCIIEncoding _asen = new ASCIIEncoding();

        /* 
        Collection de tout les coproprios connectés  
        
        */
        private Dictionary<CoProprietaire, Socket> _coproConnected = new Dictionary<CoProprietaire, Socket>();

        public Connection(int listenPort, string decryptionCode, Databases.Connection db)
        {
            _listenPort = listenPort;
            _decryptionCode = decryptionCode;
            _db = db;

        }

        public Connection(Databases.Connection db)
        {
            _checkingConnectedUser = new Thread(CheckConnectedClient);
            _checkingUserRequest = new Thread(ClientWantData);

            _checkingConnectedUser.Name = "Checking connected user ";
            _checkingUserRequest.Name = "Checking connexion request ";


            _checkingConnectedUser.Start();
            _checkingUserRequest.Start();

            _db = db;
        }

        private bool CoProprietaireIsConnected(LoginInformation li)
        {
            foreach (CoProprietaire c in _coproConnected.Keys)
            {
                if (_db.GetUsername(_db.GetCoProprio(_db.GetAccountID(li))) == _db.GetUsername(c))
                {
                    return true;
                }
            }
            return false;
        }

        private bool SocketConnected(Socket s)
        {

            /*
            try
            {
                s.Send(_asen.GetBytes("&"));
                
                return true;
            }
            catch (Exception e)
            {

                return false;
                //Console.WriteLine(e.Message) ;
            }
            */

            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);

        }

        public void CheckConnectedClient()
        {

            while (true)
            {
                foreach (KeyValuePair<CoProprietaire, Socket> item in _coproConnected)
                {
                    if (!SocketConnected(item.Value))
                    {
                        Console.WriteLine(item.Key.Nom + " s'est déconnecté.");
                        _coproConnected.Remove(item.Key);
                    }
                }
                Thread.Sleep(_maxTimeOut);
            }
        }



/* Il faut mieux écrire cette méthode

Genre tu peux vérifier si le thread de connexion des utilisateurs est actif par exemple

*/
        public void ClientWantData()
        {
            while (true)
            {

                try {
                    foreach (KeyValuePair<CoProprietaire, Socket> item in _coproConnected)
                {
                    try
                    {
                        byte[] rawMessage = new byte[1024];
                        string message = "";
                        int k = item.Value.Receive(rawMessage);

                        for (int i = 0; i < k; i++)
                            message += Convert.ToChar(rawMessage[i]);

                        message = (Encryption.Decrypt(message, _decryptionCode));

                        // Si le message est vide on casse la boucle
                        if (message == "")
                        {
                            break;
                        }
                        else
                        {
                            switch (message)
                            {
                                case "100":
                                    Console.WriteLine(item.Key.Nom + " a récupérer les proposition");

                                    try {
                                     //   Console.WriteLine("ceci" + Data<List<Proposition>>.DataToXml(_db.GetPropositions())) ;
                                    }
                                    catch(Exception e) {
                                        Console.WriteLine(e.Message) ; 
                                    }


                                   // Console.WriteLine(Encryption.Encrypt(Data<List<Proposition>>.DataToXml(_db.GetPropositions()),_decryptionCode)) ;
                                    item.Value.Send(_asen.GetBytes(Encryption.Encrypt(Data<List<Proposition>>.DataToXml(_db.GetPropositions()),_decryptionCode))) ;
                                    
                                     
                                    break;
                                default:
                                Console.WriteLine(item.Key.Nom + " donne envoie un code inconnue ( Peut-être une mauvaise version ?? " ) ; 
                                break;
                            }
                        }

                    }
                    catch(System.Net.Sockets.SocketException e) {

                    }
                    Thread.Sleep(_timeBeforeCheckingClientRequest);
                    }
                }
                catch(System.InvalidOperationException e) {

                }
            }
        }


        public void ListenConnexionRequest()
        {


            while (true)
            {
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");

                TcpListener myList = new TcpListener(ipAd, _listenPort);

                myList.Start();

                Console.WriteLine("Le serveur à démarrer sur le port " + _listenPort.ToString());
                Console.WriteLine("L'adresse ip du serveur  :" + myList.LocalEndpoint);
                Console.WriteLine("On attend la connexion.....");

                Socket s = myList.AcceptSocket();

                try
                {
                    Console.WriteLine("Connexion ouverte " + s.RemoteEndPoint);

                    byte[] rawMessage = new byte[1024];
                    string message = "";
                    int k = s.Receive(rawMessage);

                    for (int i = 0; i < k; i++)
                        message += Convert.ToChar(rawMessage[i]);

                    Console.WriteLine(Encryption.Decrypt(message, _decryptionCode));


                    //Console.WriteLine(_asen.GetString(message)) ;

                    Console.WriteLine("Reçu..");



                    LoginInformation c = Data<LoginInformation>.XmlToData(Encryption.Decrypt(message, _decryptionCode));

                    c.hashPassword();

                    //string messagex = "<?xml version=\"1.0\" encoding=\"utf-16\"?><LoginInformation xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><login>test</login><password>9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08</password></LoginInformation>" ;

                    //LoginInformation c = Data<LoginInformation>.XmlToData(messagex) ;

                    Console.WriteLine(@"
                New user attempt to connect
                username : {0}
                password : {1}
                ", c.Login, c.Password);



                    if (_db.ConnectionAttempt(c))
                    {

                        Console.WriteLine(" {0} is connected", c.Login);
                        CoProprietaire cp = _db.GetCoProprio(_db.GetAccountID(c));

                        if (CoProprietaireIsConnected(c))
                        {
                            s.Send(_asen.GetBytes("002"));
                            Console.WriteLine("{0} is already connected ! connection refused", c.Login);
                        }
                        else
                        {
                            Console.WriteLine(@"
                    name : {0}
                    prenom : {1}
                    ", cp.Nom, cp.Prenom);

                            s.Send(_asen.GetBytes("000"));
                            _coproConnected.Add(cp, s);
                        }
                    }
                    else
                    {
                        s.Send(_asen.GetBytes("001"));
                    }

                    Console.WriteLine("The number of connected is  {0}", _coproConnected.Count());



                    //_coproConnected.Add()



                    /*ASCIIEncoding _asen = new ASCIIEncoding();
                    s.Send(_asen.GetBytes("La chane recu est la suivante."));
                    Console.WriteLine("\nenvoye réussis");*/
                    /* clean up */


                    //s.Close();
                    myList.Stop();

                    Console.WriteLine("on passe au suivant");






                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur..... " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }


            }


        }
    }
}