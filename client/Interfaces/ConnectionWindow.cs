using Gtk;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cairo ;
using GCP;

public class ConnectionWindow : Window {
 

    private double [,] trs = new double[,] {
        { 0.0, 0.15, 0.30, 0.5, 0.65, 0.80, 0.9, 1.0 },
        { 1.0, 0.0,  0.15, 0.30, 0.5, 0.65, 0.8, 0.9 },
        { 0.9, 1.0,  0.0,  0.15, 0.3, 0.5, 0.65, 0.8 },
        { 0.8, 0.9,  1.0,  0.0,  0.15, 0.3, 0.5, 0.65},
        { 0.65, 0.8, 0.9,  1.0,  0.0,  0.15, 0.3, 0.5 },
        { 0.5, 0.65, 0.8, 0.9, 1.0,  0.0,  0.15, 0.3 },
        { 0.3, 0.5, 0.65, 0.8, 0.9, 1.0,  0.0,  0.15 },
        { 0.15, 0.3, 0.5, 0.65, 0.8, 0.9, 1.0,  0.0, }
    };

   private short count = 0;
   private DrawingArea darea;


    private Label errorLabel ;
    private Button hideButton ;  

    private Thread _connex ;


    private VBox _showError ; 

   private Label charg ; 

    public ConnectionWindow(Thread connex) : base("Connexion en cours")
    {


        SetDefaultSize(250, 150);
        SetPosition(WindowPosition.Center);
        Resizable = false ;


        _connex = connex ;
        _connex.Name ="ConnectionRequest" ; 
        _connex.IsBackground = true ;
        _connex.Start() ;


        // Showing error 
        _showError = new VBox() ;
        hideButton = new Button("Hide") ;
        errorLabel = new Label() ;        
        _showError.PackEnd(hideButton, true, false , 10) ; 
        _showError.PackStart(errorLabel, true, false, 10) ;  
        
        

        
        DeleteEvent += delegate { Application.Quit(); };
      
        GLib.Timeout.Add(100, new GLib.TimeoutHandler(OnTimer));
        darea = new DrawingArea();
       darea.Drawn += OnExpose;

      Add(darea);
        ShowAll();
    }

    public void ShowError(string errorMessage) {

       Remove(darea) ;

        hideButton.Clicked += delegate {
            Gtk.Application.Invoke(delegate {
            Program.mainWindow.EnableOrDisableLoginButtons(true) ;
                Remove(_showError) ;
                Hide() ;
                });
            };
        errorLabel.Text = errorMessage ;
        Add(_showError) ; 

        ShowAll() ; 
    }
    



    private bool OnTimer() 
    {


            count += 1;
        darea.QueueDraw();
        


        return true;
    } 

   
    void OnExpose(object sender, EventArgs args)
    {
        
        try {
        DrawingArea area = (DrawingArea) sender;
        
            Cairo.Context cr =  Gdk.CairoHelper.Create(area.GdkWindow);
             cr.LineWidth = 3;
        cr.LineCap = LineCap.Round;
        int width, height;
        width = Allocation.Width;
        height = Allocation.Height;
        cr.Translate(width/2, height/2);
        for (int i = 0; i < 8; i++) {
            cr.SetSourceRGBA(255, 255, 255, trs[count%8, i]);
            cr.MoveTo(0.0, -10.0);
            cr.LineTo(0.0, -40.0);
            cr.Rotate(Math.PI/4);
            cr.Stroke();
        }
        ((IDisposable) cr.Target).Dispose();                                      
        ((IDisposable) cr).Dispose();
        }
        catch (Exception e) {
            Console.WriteLine(e.Message) ; 
        }
        
        
    
    }
    
}
