using System ;
using System.Net ;
using System.Net.Sockets ;
using GCP.Networking ; 
using System.Threading;
using GCP.GCPClass ;
using GCP ;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic ;

using System.Xml;
using System.Xml.Serialization;



namespace GCP.Networking {
    public class Connection {
        private IPAddress _serverIP = IPAddress.Parse("127.0.0.1") ;


        private TcpClient _tcpclnt ;

        private Stream _stm ;
        private int _port = 2401 ;

        // SECRET CODE !!!!
        private string _decryptionCode = "pCw0bX$7OLQEI1!o^y%nc3^#" ;

        private int _timeout = 100 ; 

        private bool _isConnected = false ; 

        public Connection() {}

        public void Disconnect() {
            if ( _tcpclnt != default(TcpClient)) {
                _tcpclnt.Close() ;
            }
        }

        public void SendPropositionRequest() {

            ASCIIEncoding enc = new ASCIIEncoding() ;
            _stm.Write(enc.GetBytes(Encryption.Encrypt("100", _decryptionCode))) ;

            Thread a = new Thread(GetPropositionResponse) ;
            a.Name = "Getting Proposition" ; 
            a.Start() ;

        }


        // THREAD !!!

        private void GetPropositionResponse() {
            byte[] responseRaw = new byte[32768] ;
                string response = "" ;
                Data<Proposition> decryptMess = new Data<Proposition>() ;


                /*
                while ( response.StartsWith("200") || response == ""  ) {

                    Console.WriteLine("On tente de récup") ;

                    Console.WriteLine(response) ;
                    int sizeResponse = _stm.Read(responseRaw,0,100) ;

                    for (int i = 0 ; i < sizeResponse ; i++) {
                        response += Convert.ToChar(responseRaw[i]);
                        }
                 } */
                 
                 while ( response == "") {
                    int sizeResponse = _stm.Read(responseRaw,0,10000) ;
                    for (int i = 0 ; i < sizeResponse ; i++) {
                        response += Convert.ToChar(responseRaw[i]);
                        }

                 }

                // Console.WriteLine(response) ; 


                List<Proposition> data = Data<List<Proposition>>.XmlToData(Encryption.Decrypt(response, _decryptionCode)) ;

              

                  Gtk.Application.Invoke(delegate {
                    Program.mainWindow.CreateModel(data) ;
                   }) ; 
        }


        public void SendConnectionRequest(LoginInformation lg) {

            /*
            byte[] connexRequest = new byte[1024] ;
            try {
                 IPHostEntry host = Dns.GetHostEntry("localhost");
                 IPEndPoint remoteEP = new IPEndPoint(_serverIP, _port);

                 string data = Data<LoginInformation>.DataToXml(lg) ;

                 Console.WriteLine(data) ;
                 Socket sender = new Socket(_serverIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

                try {
                    sender.Connect(remoteEP);

                     Console.WriteLine("Socket connected to {0}",sender.RemoteEndPoint.ToString());
                }
                catch (Exception e) {
                    
                }
            }
            catch( Exception e) {

            }*/

            _tcpclnt = new TcpClient() ;

            

            try {

                ASCIIEncoding enc = new ASCIIEncoding() ;
        
                byte[] data = new byte[1024] ; 
               
                _tcpclnt.Connect(_serverIP, _port) ;
                
                _stm = _tcpclnt.GetStream();

               // Console.Write(authMess.DataToXml(lg)) ;


                byte[] xmlMessage = enc.GetBytes(Encryption.Encrypt(Data<LoginInformation>.DataToXml(lg), _decryptionCode)) ;

                _stm.Write(xmlMessage, 0, xmlMessage.Length);

                // Connection Response

                byte[] responseRaw = new byte[100] ;
                string response = "" ; 

                 int sizeResponse = _stm.Read(responseRaw,0,100) ;

                 for (int i = 0 ; i < sizeResponse ; i++) {
                    response += Convert.ToChar(responseRaw[i]);
                 }

                 
                 /*
                 Close connection 
                 _tcpclnt.Close();
                 */


/* 
 !!!!!! On n'accède pas a un élément graphique depuis un thread directement !!

 Tu dois utiliser la méthode statique :

 Gtk.Application.Invoke(delegate {
    ...
 })

*/

                 if ( response == "000") {
                    Console.WriteLine("Bon mot de passe") ;

                    Gtk.Application.Invoke(delegate {
                        Program.mainWindow.AfterLoginMenu() ;
                    }) ;
                  //  Program.mainWindow.AfterLoginMenu() ;
                 }
                 else if (response == "001" ) {
                    Console.WriteLine("mauvais mot de passe ") ;
                    Gtk.Application.Invoke( delegate {
                        Program.mainWindow.ConnectionWindow.ShowError("L'username ou le mot de passe est incorrect") ;
                    }) ;
                   //Program.mainWindow.ConnectionWindow.ShowError("L'username ou le mot de passe est incorrect") ;
                 }
                 else if (response == "002")  {
                    Console.WriteLine("déjà connecté") ;
                    Gtk.Application.Invoke( delegate {
                        Program.mainWindow.ConnectionWindow.ShowError("You are already connected!") ;
                    }) ;
                 }
                 else {

                    Gtk.Application.Invoke(delegate {
                        Program.mainWindow.ConnectionWindow.ShowError("Le serveur rencontre des problèmes. Veuillez contacter l'administrateur ou l'éditeur du logiciel") ;
                    }) ;
                    //Program.mainWindow.ConnectionWindow.ShowError("Le serveur rencontre des problèmes. Veuillez contacter l'administrateur ou l'éditeur du logiciel") ;
                 }
                

        

/*

to send paquet 
                byte[] bb = new byte[100];
                int k = _stm.Read(bb, 0, 100);

                 for (int i = 0; i < k; i++)
                    Console.Write(Convert.ToChar(bb[i]));
*/

                
            }
            catch (System.Net.Sockets.SocketException e) {
                Gtk.Application.Invoke(delegate {
                Program.mainWindow.ConnectionWindow.ShowError("Le serveur de l'application ne répond pas.") ;
                });
            }
            
            catch (Exception e) {
               // Program.mainWindow.ConnectionWindow.ShowError("L'erreur n'est pas référencé. Veuillez communiquer votre problème au fournisseur du logiciel") ;
               Gtk.Application.Invoke(delegate {
              Program.mainWindow.ConnectionWindow.ShowError(e.Message);
               });
            } 
            finally {
                
            }
            
        }
    }
}