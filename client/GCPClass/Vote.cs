using System ; 

namespace GCP.GCPClass {
    public class Vote {

        public bool secretVote ;

        public Proposition proposition ;

        public Vote(bool secretVote, Proposition proposition) {
            this.secretVote = secretVote ;
            this.proposition = proposition ;
        } 
    }
}