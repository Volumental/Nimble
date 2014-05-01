using System;
using Gtk;
using Nimble;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		var openni = new OpenNI();
		openni.Initialize ();
		Console.WriteLine ("Devices");
		foreach (var device in openni.Devices)
		{
			Console.WriteLine (device.Name);
		}
		openni.Shutdown ();

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
