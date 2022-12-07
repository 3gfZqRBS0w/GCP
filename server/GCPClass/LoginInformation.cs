using System ;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text ;
using System.Security.Cryptography;

namespace GCP.GCPClass {

    public class LoginInformation {


        
        public LoginInformation() {}

        [XmlElement("login")]
        public string login ;


        [XmlElement("password")]
        public string password ;

        [XmlIgnore]
        private int _limitLengthOfLogin = 32 ;

        [XmlIgnore]
        private int _hashLength = 64 ;  

        public LoginInformation(string login, string password) {
            this.login = login ;

            // hash password after send to server
            //this.password = sha256(password) ;

            this.password = password ; 
        }
        public string Login
        {
            get { return this.login; }
        }

        public string Password {
            get { return this.password; } 
        }

        public bool ValidLoginInformation() {

          //  Console.WriteLine(" ce que je veux :" + password.Length) ;
            if ( login.Length < _limitLengthOfLogin && login.Length > 0 && password.Length == _hashLength ) {
                return true ;
            }
            else {
                
                return false ; 
            }
        }

        public void hashPassword() {
            password = sha256(password) ;
        }

        private string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    } 
}