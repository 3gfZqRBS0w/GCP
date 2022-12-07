using System ;
using System.Xml;
using System.Xml.Serialization;

namespace GCP.GCPClass {
    public class Proposition {



        public Proposition() {} 

        [XmlElement("libelle")]
        public string _libelle ;

        [XmlElement("articleNumber")]
        public string _articleNumber ;
        
        public Proposition(string libelle, string art) {
            _libelle = libelle ;
            _articleNumber = art ; 
        }


        public string Libelle
        {
            get { return _libelle; }
        }

        public string ArticleNumber
        {
            get { return _articleNumber; }
        }



    }
}