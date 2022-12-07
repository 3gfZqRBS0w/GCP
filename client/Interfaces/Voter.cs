using Gtk;
using Cairo;
using System;
 
namespace GCP.Interfaces {
    class Voter : Window {

        private Label _propositionLabel ;
        private VBox _voteFormBox ;
        private VBox _errorFormBox ; 
        private Button _voteButton, _closeButton ; 

        private Label _errorLabel ;  
        

        private HBox _voteButtonBox ;
        private HBox _completeButtonBox;

        private Button _completeCloseButton ; 
        private RadioButton _pourButton, _contreButton ;



    public void SetProposition(string proposition) {

        Remove(_errorFormBox) ;
        _propositionLabel.Text = proposition ; 
        Add(_voteFormBox) ;

        ShowAll() ;
    }




 
    public Voter() : base("GCP - Vote")
    {
        SetDefaultSize(420, 200);
        SetPosition(WindowPosition.Center);
        DeleteEvent += delegate { Application.Quit(); };
        Resizable = false;

        _pourButton = new RadioButton(null, "POUR") ;
        _contreButton = new RadioButton(_pourButton, "CONTRE") ;


        _voteButton = new Button("Voter") ;



        _closeButton = new Button("Fermer") ; _completeCloseButton  = new Button("Fermer") ;


        _closeButton.Clicked += delegate {
            Hide() ;
        };
        _completeCloseButton.Clicked += delegate {
            Hide();
        }; 

        _voteButton.Clicked += delegate {
            Console.WriteLine("le vote est"+ (_pourButton.Active ? "oui" : "non")) ;
            Hide();
        };


        _propositionLabel = new Label("Nom de la Proposition") ; 
        _voteFormBox = new VBox() ;

        _errorFormBox = new VBox() ;
        
        _voteButtonBox = new HBox() ;
        _completeButtonBox = new HBox() ; 

        _completeButtonBox.PackStart(_voteButton,true, true, 5) ;
        _completeButtonBox.PackStart(_closeButton, true, true, 5) ;




        _voteButtonBox.PackStart(_pourButton, true, false, 0) ;
        _voteButtonBox.PackStart(_contreButton, true, false, 0) ;

        

        _voteFormBox.PackStart(_propositionLabel, true, false, 0) ;
        _voteFormBox.PackEnd(_completeButtonBox, false, false, 5) ; 
        _voteFormBox.PackStart(_voteButtonBox, true, true, 5) ;


        _errorLabel = new Label("Pour voter, il faut double-cliquer sur la proposition ") ;

        _errorFormBox.PackStart(_errorLabel, false, false, 10) ;
        _errorFormBox.PackEnd(_completeCloseButton, false, true, 10) ;


        Add(_errorFormBox) ;


       // Add(_voteFormBox) ;

        ShowAll();
    }
}
}
