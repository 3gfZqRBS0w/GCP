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
        public string _login ;


        [XmlElement("password")]
        public string _password ;

        public LoginInformation(string login, string password) {
            _login = login ;
            _password = password ; 
        }
        public string Login
        {
            get { return _login; }
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