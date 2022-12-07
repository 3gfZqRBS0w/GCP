using System;
using Cairo;
using Gtk;
using System.Threading;
using System.Threading.Tasks;
using GCP.Networking;
using System.Collections;
using System.Collections.Generic;
using GCP.GCPClass;


namespace GCP.Interfaces
{
    public class MainWindow : Window
    {
        private ToolButton _vote;

        private string _propositionString = "" ; 
        private ToolButton _disconnection;
        private ToolButton _result;
        private ToolButton _profil;
        private ToolButton _history;
        private ConnectionWindow _connectionWindow;
        private VBox _preConnection;
        private VBox _postConnection;

        private Button _connectionButton;
        private Button _quitButton;

        #region TableauProposition
        private ListStore _store;
        private Statusbar _statusbar;

        // without id because is not serverside
        private Connection _Net = new Connection();
        private TreeView treeView;

        enum Column
        {
            libelle,
            articleCode,
        }
        #endregion

        // il faut en faire une fonction

        public void EnableOrDisableLoginButtons(bool isSensitive)
        {
            _connectionButton.Sensitive = isSensitive;
            _quitButton.Sensitive = isSensitive;
        }

        public void EnableOrDisableControlButton(bool isSensitive)
        {

        }





        public ConnectionWindow ConnectionWindow
        {
            get { return _connectionWindow; }
        }

        public void AfterLoginMenu()
        {


            _Net.SendPropositionRequest();

            ConnectionWindow.Hide();


            if (_preConnection.Visible)
            {
                Remove(_preConnection);
            }

            Resize(500, 500);


            Title = "GCP - Tableau de bord";

            Add(_postConnection);
            ShowAll();


        }




        public MainWindow() : base("GCP - Connexion")
        {

            SetDefaultSize(500, 200);
            SetPosition(WindowPosition.Center);
            BorderWidth = 8;
            DeleteEvent += delegate { Application.Quit(); };
            Resizable = false;

            _postConnection = new VBox();
            _preConnection = new VBox();

            ////// POST CONNEXION WINDOW 


            Toolbar toolbar = new Toolbar();
            toolbar.ToolbarStyle = ToolbarStyle.Text;


            _vote = new ToolButton(Stock.About);
            _vote.Label = "Voter";
            _disconnection = new ToolButton(Stock.About);
            _disconnection.Label = "Déconnexion";
            _result = new ToolButton(Stock.About);
            _result.Label = "Résultat";
            _profil = new ToolButton(Stock.About);
            _profil.Label = "Profil";
            _history = new ToolButton(Stock.About);
            _history.Label = "Historique";


            _vote.Clicked += delegate
            {
                Voter a = new Voter();

                if ( _propositionString != "" ) {
                    a.SetProposition(_propositionString ) ;
                }
            };

            _disconnection.Clicked += delegate
            {


                _Net.Disconnect();
                Title = "GCP - Connexion";
                EnableOrDisableLoginButtons(true);
                Resize(500, 200);
                Remove(_postConnection);
                Add(_preConnection);

                ShowAll();
            };



            ScrolledWindow sw = new ScrolledWindow();
            sw.ShadowType = ShadowType.EtchedIn;
            sw.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);



            treeView = new TreeView(_store);
            treeView.RulesHint = true;
            treeView.RowActivated += OnRowActivated;
            //treeView.ActivateOnSingleClick;



            sw.Add(treeView);



            _statusbar = new Statusbar();



            toolbar.Insert(_vote, 0);
            toolbar.Insert(_disconnection, 4);
            toolbar.Insert(_result, 3);
            toolbar.Insert(_profil, 2);
            toolbar.Insert(_history, 1);

            _postConnection.PackStart(sw, true, true, 0);
            _postConnection.PackStart(toolbar, false, false, 0);

            //////////////////////////////////////////////////////



            /// PRE CONNEXION /////////////////////////////////////


            // Label and Entry Connection
            HBox HorizontalBox1 = new HBox();
            HBox HorizontalBox2 = new HBox();

            // Label Connection
            VBox VerticalBox1 = new VBox();
            // Entry Connection
            VBox VerticalBox2 = new VBox();

            // AcceptFocus = true ;


            // Label connectionLabel = new Label();
            //  connectionLabel.Text = "Connexion" ;

            Label usernameLabel = new Label();
            usernameLabel.Text = "Votre nom d'utilisateur : ";

            Label passwordLabel = new Label();
            passwordLabel.Text = "Mot de passe : ";


            Entry usernameEntry = new Entry();
            usernameEntry.PlaceholderText = "Votre nom d'utilisateur";

            Entry passwordEntry = new Entry()
            {
                PlaceholderText = " Votre mot de passe",
                CanFocus = true,
                IsEditable = true,
                Visibility = false,
                InvisibleChar = '●'
            };



            _connectionButton = new Button("Connexion");
            _quitButton = new Button("Quitter");

            _quitButton.Clicked += delegate
            {
                Application.Quit();
            };
            _connectionButton.Clicked += delegate
            {

                EnableOrDisableLoginButtons(false);
                // _connectionWindow = null ;
                _connectionWindow = new ConnectionWindow(new Thread(() => _Net.SendConnectionRequest(new LoginInformation(usernameEntry.Text, passwordEntry.Text))));
            };


            // _preConnection.PackStart(connectionLabel, false, true, 10) ; 

            HorizontalBox1.PackStart(VerticalBox1, true, true, 0);
            HorizontalBox1.PackStart(VerticalBox2, true, true, 0);

            VerticalBox1.PackStart(usernameLabel, false, false, 10);
            VerticalBox1.PackStart(passwordLabel, false, false, 35);

            VerticalBox2.PackStart(usernameEntry, false, true, 10);
            VerticalBox2.PackStart(passwordEntry, false, true, 10);

            HorizontalBox2.PackStart(_connectionButton, true, true, 0);
            HorizontalBox2.PackStart(_quitButton, true, true, 5);

            _preConnection.PackStart(HorizontalBox1, true, true, 10);
            _preConnection.PackStart(HorizontalBox2, true, true, 0);

            Add(_preConnection);

            TreeViewColumn propositionLibelle = new Gtk.TreeViewColumn();
            propositionLibelle.Title = "Proposition";
            TreeViewColumn articleRef = new Gtk.TreeViewColumn();
            articleRef.Title = "Article";

            _store = new ListStore(typeof(string), typeof(string));


            CellRendererText rendererText = new CellRendererText();
            TreeViewColumn column = new TreeViewColumn("Proposition", rendererText, "text", Column.libelle);
            column.SortColumnId = (int)Column.libelle;
            treeView.AppendColumn(column);

            rendererText = new CellRendererText();
            column = new TreeViewColumn("Article", rendererText, "text", Column.articleCode);
            column.SortColumnId = (int)Column.articleCode;
            treeView.AppendColumn(column);



            treeView.Model = _store;


            ShowAll();



        }






        void OnRowActivated(object sender, RowActivatedArgs args)
        {

          //  Console.WriteLine("test");

            TreeIter iter;
            TreeView view = (TreeView)sender;

            if (view.Model.GetIter(out iter, args.Path))
            {
                string row = (string)view.Model.GetValue(iter, (int)Column.libelle);
                row += ", " + (string)view.Model.GetValue(iter, (int)Column.articleCode);

               _propositionString = row ;
               
                _statusbar.Push(0, row);
            }
        }




        public void CreateModel(List<Proposition> propositions)
        {

            Gtk.ListStore blabla = new Gtk.ListStore(typeof(string), typeof(string));


            // treeView.Model = _store ;
            // ShowAll() ;




            foreach (Proposition act in propositions)
            {


                blabla.AppendValues(act.Libelle, act.ArticleNumber);



            }

            treeView.Model = blabla;

            //  ShowAll() ;


        }
    }
}