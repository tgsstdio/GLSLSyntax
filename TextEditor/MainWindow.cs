using System;
using Gtk;
using Gdk;
using GLSLSyntaxAST;
using Irony.Parsing;

public partial class MainWindow: Gtk.Window
{
	private static Atom mClipboardAtom = Gdk.Atom.Intern("CLIPBOARD", false);
	private Gtk.Clipboard mClipBoard = Gtk.Clipboard.Get (mClipboardAtom);

	private static Atom mPrimary = Gdk.Atom.Intern ("PRIMARY", false);

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		mTextField.CopyClipboard += DoCopy;
		mTextField.PasteClipboard += DoPaste;
		mTextField.CutClipboard += DoCut;

		//GLSLGrammar lang = new GLSLGrammar ();
		//var compiler = new Irony.Parsing.Parser(lang);
		//var tree = compiler.Parse ("void main(void) { float v = vec3(1,1,1); }");
		//CheckNodes (tree.Root, 0);
	}

	public void CheckNodes(ParseTreeNode node, int level)
	{
		for(int i = 0; i < level; i++)
			Console.Write("  ");
		Console.WriteLine(node);

		foreach (ParseTreeNode child in node.ChildNodes)
		{
			CheckNodes (child, level + 1);
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void DoQuit (object sender, EventArgs e)
	{
		Application.Quit ();
	}

	protected void CutText ()
	{
		TextIter start;
		TextIter end;
		if (mTextField.Buffer.GetSelectionBounds (out start, out end))
		{
			string selection = mTextField.Buffer.GetText (start, end, include_hidden_chars: true);
			mTextField.Buffer.Delete (ref start, ref end);
			mClipBoard.Text = selection;
		}
	}

	protected void DoCut (object sender, EventArgs e)
	{
		CutText ();
	}

	protected void CopyText ()
	{
		TextIter start;
		TextIter end;
		if (mTextField.Buffer.GetSelectionBounds (out start, out end))
		{
			string selection = mTextField.Buffer.GetText (start, end, include_hidden_chars: true);
			mClipBoard.Text = selection;
		}
	}

	protected void DoPaste (object sender, EventArgs e)
	{
		string result = mClipBoard.WaitForText ();
		mTextField.Buffer.InsertAtCursor (result);
	}

	protected void DoCopy (object sender, EventArgs e)
	{
		CopyText ();
	}

}