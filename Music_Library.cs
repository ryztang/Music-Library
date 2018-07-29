using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;

class MusicLib : Form {
	
    // Constants used when saving to and retrieving from xml file
    private const string XmlNodeTag = "node";
    private const string XmlNodeTextAtt = "text";
    private const string XmlNodeTagAtt = "tag";
    private const string XmlNodeImageIndexAtt = "imageindex";

    // Some elements declared before creating MusicLib() so they can be used in peripheral functions
    StatusBar sb;
    TextBox newnode;
    TreeView library;
    Label newurl;
    TextBox newvideo;
    TreeNode editedNode;
    TextBox searchText;
    TreeNode copiedNode;
    TreeNode cutNode;
    ContextMenu cm;

    // The following contains all the elements in the form
    public MusicLib() {
        Text = "Music Library";
        Size = new Size(500, 500);

        library = new TreeView();

	// Retrieve Library from xml file
        DeserializeTreeView(library, @"c:\Ryan-Projects\Music_Library\Music_Library_Data.xml");

	SuspendLayout();

	sb = new StatusBar();
        sb.Parent = this;

	cm = new ContextMenu();
	cm.MenuItems.Add("Cut", new EventHandler(CutNodeMenu));
	cm.MenuItems.Add("Copy", new EventHandler(CopyNodeMenu));
	cm.MenuItems.Add("Paste", new EventHandler(PasteNodeMenu));
	cm.MenuItems.Add("-");
	cm.MenuItems.Add("Edit", new EventHandler(EditNodeMenu));
	cm.MenuItems.Add("Delete", new EventHandler(DeleteNodeMenu));

	Label newlabel = new Label();
        newlabel.Parent = this;
        newlabel.Text = "New Node:";
	newlabel.Width = 60;
        newlabel.Location = new Point(5, this.Height - 90);
	newlabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

	newnode = new TextBox();
        newnode.Parent = this;
	newnode.Location = new Point(newlabel.Width + 5, this.Height - 90);
        newnode.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
	newnode.Width = this.Width - newlabel.Width - 350;

	newurl = new Label();
        newurl.Parent = this;
        newurl.Text = "YouTube URL:";
	newurl.Width = this.Width - newlabel.Width - newnode.Width - 270;
        newurl.Location = new Point(newlabel.Width + newnode.Width + 10, this.Height - 90);
	newurl.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

	newvideo = new TextBox();
        newvideo.Parent = this;
	newvideo.Location = new Point(newlabel.Width + newnode.Width + newurl.Width + 15, this.Height - 90);
        newvideo.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
	newvideo.Width = this.Width - newlabel.Width - newnode.Width - newurl.Width - 125;

	ToolTip btnTlp = new ToolTip();

	Button add = new Button();
	add.Parent = this;
        add.Location = new Point(this.Width - 100, this.Height - 93);
	add.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        add.Text = "Add Node";
	btnTlp.SetToolTip(add, add.Text);
	add.Click += new EventHandler(AddNode);

	Button delete = new Button();
	delete.Parent = this;
	delete.Text = "Delete Node";
	delete.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
	delete.Location = new Point(this.Width - 100, this.Height - 130);
	btnTlp.SetToolTip(delete, delete.Text);
	delete.Click += new EventHandler(DeleteNode);

	Button edit = new Button();
	edit.Parent = this;
	edit.Text = "Edit Node";
	edit.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
	edit.Location = new Point(5, this.Height - 130);
	btnTlp.SetToolTip(edit, edit.Text);
	edit.Click += new EventHandler(EditNode);

	Button update = new Button();
	update.Parent = this;
	update.Text = "Update Node";
	update.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
	update.Location = new Point(edit.Width + 10, this.Height - 130);
	update.Width = 85;
	btnTlp.SetToolTip(update, update.Text);
	update.Click += new EventHandler(UpdateNode);

	Label searchLabel = new Label();
	searchLabel.Parent = this;
        searchLabel.Text = "Search Music Library:";
	searchLabel.Width = 120;
        searchLabel.Location = new Point(10, 15);
	searchLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top;

	searchText = new TextBox();
        searchText.Parent = this;
	searchText.Location = new Point(searchLabel.Width + 10, 13);
        searchText.Anchor = AnchorStyles.Left | AnchorStyles.Top;
	searchText.Width = 150;
	searchText.KeyDown += new KeyEventHandler(EnterSearch);

	Button searchButton = new Button();
	searchButton.Parent = this;
        searchButton.Location = new Point(searchLabel.Width + searchText.Width + 20, 12);
	searchButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        searchButton.Text = "Search";
	btnTlp.SetToolTip(searchButton, searchButton.Text);
	searchButton.Click += new EventHandler(SearchLibrary);

	library.Parent = this;
	library.Location = new Point(0, 45);
	library.Size = new Size(this.Width - 20, this.Height - 185);
	library.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
	library.Scrollable = true;
	library.Sort();
        library.AfterSelect += new TreeViewEventHandler(SelectNode);
	library.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(DoubleClickFunc);
	library.KeyDown += new KeyEventHandler(DeleteKey);
	library.KeyDown += new KeyEventHandler(CopyNodeKey);
	library.KeyDown += new KeyEventHandler(PasteNodeKey);
	library.KeyDown += new KeyEventHandler(CutNodeKey);
	library.MouseDown += new MouseEventHandler(RightClickSelect);
	library.ContextMenu = cm;

	ResumeLayout();

        CenterToScreen();
    }

    void SelectNode(object sender, TreeViewEventArgs e) {
        sb.Text = e.Node.Text;
    }

    void RightClickSelect(object sender, MouseEventArgs e) {
	if (e.Button == MouseButtons.Right) {
		library.SelectedNode = library.GetNodeAt(e.X, e.Y);
	}
    }

    void AddNode(object sender, EventArgs e) {
	TreeNode parent = library.SelectedNode;
    	if (newnode.Text == "") {
		sb.Text = "Please type in the New Node textbox to add a node";
		return;
	} else if (parent == null) {
		sb.Text = "Please select a node in the library to add to";
		return;
	};
	TreeNode child = new TreeNode();
	child.Text = newnode.Text;
	if (newvideo.Text != "") {
		child.Tag = newvideo.Text;
	};
	parent.Nodes.Add(child);
	library.SelectedNode = null;
	newnode.Text = "";
	newvideo.Text = "";
	sb.Text = "Node successfully added";
	
	// Save Library into xml file
	SerializeTreeView(library, @"c:\Ryan-Projects\Music_Library\Music_Library_Data.xml");
    }

    void DeleteNode(object sender, EventArgs e) {
	Delete();
    }

    void DeleteKey(object sender, KeyEventArgs e) {
	if (e.KeyCode == Keys.Delete) {
		Delete();
	};
    }

    void DeleteNodeMenu(object sender, EventArgs e) {
	Delete();
    }

    void Delete() {
	if (library.SelectedNode == null) {
		sb.Text = "Please select a node in the library to delete";
	} else if (library.SelectedNode.Parent == null) {
		sb.Text = "Cannot delete the root node";
	} else {
		library.SelectedNode.Remove();
		library.SelectedNode = null;
		sb.Text = "Node successfully deleted";
	};
	SerializeTreeView(library, @"c:\Ryan-Projects\Music_Library\Music_Library_Data.xml");
    }

    void CutNodeKey(object sender, KeyEventArgs e) {
	if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control) {
		cutNode = library.SelectedNode;
		BeginCopy();
	}
    }

    void CutNodeMenu(object sender, EventArgs e) {
	cutNode = library.SelectedNode;
	BeginCopy();
    }

    void CopyNodeKey(object sender, KeyEventArgs e) {
	if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) {
		cutNode = null;
		BeginCopy();
	};
    }

    void CopyNodeMenu(object sender, EventArgs e) {
	cutNode = null;
	BeginCopy();
    }

    private void BeginCopy() {
	if (library.SelectedNode == null) {
		sb.Text = "Please select a node to copy/cut";
		return;
	};
	sb.Text = "Node is ready to be copied/moved";
	copiedNode = new TreeNode();
	copiedNode.Text = library.SelectedNode.Text;
	if (library.SelectedNode.Tag != null  && Convert.ToString(library.SelectedNode.Tag) != "") {
		copiedNode.Tag = library.SelectedNode.Tag;
	};
	CopyChildren(library.SelectedNode.Nodes, copiedNode);
    }

    private void CopyChildren(TreeNodeCollection children, TreeNode parentNode) {
	if (children.Count > 0) {
		for (int i = 0; i < children.Count; ++i) {
			TreeNode child = new TreeNode();
			child.Text = children[i].Text;
			if (children[i].Tag != null  && Convert.ToString(children[i].Tag) != "") {
				child.Tag = children[i].Tag;
			};
			parentNode.Nodes.Add(child);
			CopyChildren(children[i].Nodes, child);	// Recurse on the rest of the child nodes
		}
	};
    }

    void PasteNodeKey(object sender, KeyEventArgs e) {
	if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control) {
		CompletePaste();
	}
    }

    void PasteNodeMenu (object sender, EventArgs e) {
	CompletePaste();
    }

    void CompletePaste() {
	if (copiedNode == null) {
		sb.Text = "Please copy/cut a node to paste";
		return;
	} else if (library.SelectedNode == null) {
		sb.Text = "Please select a node to paste to";
		return;
	};
	for (int i = 0; i < library.SelectedNode.Nodes.Count; ++i) {
		if (library.SelectedNode.Nodes[i].Text == copiedNode.Text && library.SelectedNode.Nodes[i].Tag == copiedNode.Tag) {
			sb.Text = "The node to paste already exists under the selected node";
			return;
		};
	}
	if (cutNode != null) {
		cutNode.Remove();
	};
	library.SelectedNode.Nodes.Add(copiedNode);
	sb.Text = "Node successfully copied/moved";
	SerializeTreeView(library, @"c:\Ryan-Projects\Music_Library\Music_Library_Data.xml");
    }

    void EditNode(object sender, EventArgs e) {
	editedNode = library.SelectedNode;
	if (editedNode == null) {
		sb.Text = "Please select a node in the library to edit";
		return;
	} else if (editedNode.Tag != null) {
		newvideo.Text = Convert.ToString(editedNode.Tag);
	};
	newnode.Text = editedNode.Text;
    }

    void EditNodeMenu(object sender, EventArgs e) {
	EditNode(this, e);
    }

    void UpdateNode(object sender, EventArgs e) {
	if (editedNode == null) {
		sb.Text = "Please select a node to edit first";
	} else if (newnode.Text == "") {
		sb.Text = "Please enter valid text to update the node";
	} else {
		editedNode.Text = newnode.Text;
		editedNode.Tag = newvideo.Text;
		newnode.Text = "";
		newvideo.Text = "";
		sb.Text = "Node successfully updated";
	};
	SerializeTreeView(library, @"c:\Ryan-Projects\Music_Library\Music_Library_Data.xml");
    }

    void DoubleClickFunc(object sender, TreeNodeMouseClickEventArgs e) {
	if (e.Node.Tag != null && Convert.ToString(e.Node.Tag) != "") {
		var url = Convert.ToString(e.Node.Tag);
		
		// Open url in Google Chrome, incognito
		using (var process = new System.Diagnostics.Process()) {
			process.StartInfo.FileName = @"chrome.exe";
			process.StartInfo.Arguments = url + " --incognito";
			process.Start();
		}
	} else if (e.Node.IsExpanded) {
		e.Node.Expand();
	} else {
		e.Node.Collapse();
	};
    }

    // Variables used in Search Library functions
    private List<TreeNode> CurrentNodeMatches = new List<TreeNode>();
    private int LastNodeIndex = 0;
    private string LastSearchText;

    // The following search functions are modified versions of those found in stackoverflow.com/questions/11530643/treeview-search
    void SearchLibrary(object sender, EventArgs e) {
        string searchFor = searchText.Text;
        if (String.IsNullOrEmpty(searchFor)) {
		sb.Text = "Please enter text to search in the library";
		LastNodeIndex = 0;
            	return;
        };
        if (LastSearchText != searchFor) {	// Check if it's a new search
            	CurrentNodeMatches.Clear();
            	LastSearchText = searchFor;
            	LastNodeIndex = 0;
            	SearchNodes(searchFor, library.Nodes[0]);	// Find all instances of new search
        };
        if (LastNodeIndex >= 0 && CurrentNodeMatches.Count > 0 && LastNodeIndex < CurrentNodeMatches.Count) {	// Find next instance of previous search
		TreeNode selectedNode = CurrentNodeMatches[LastNodeIndex];
		LastNodeIndex++;
           	library.SelectedNode = selectedNode;
            	library.SelectedNode.Expand();
            	library.Select();
        } else {
		sb.Text = "There are no more matches";
		LastNodeIndex = 0;
	};
    } 

    private void SearchNodes(string SearchText, TreeNode StartNode) {
        while (StartNode != null) {
            	if (StartNode.Text.ToLower().Contains(SearchText.ToLower())) {	// Check if there's a match with the search
               		CurrentNodeMatches.Add(StartNode);
            	};
            	if (StartNode.Nodes.Count != 0) {
               		SearchNodes(SearchText, StartNode.Nodes[0]); //Recursive Search 
            	};
            	StartNode = StartNode.NextNode;
        };
    }

    void EnterSearch(object sender, KeyEventArgs e) {
	if (e.KeyCode == Keys.Enter) {
		SearchLibrary(this, e);
	};
    }

    // The following Serialize and Deserialize functions are modified from those found in www.codeproject.com/Articles/13099/Loading-and-Saving-a-TreeView-control-to-an-XML-fi
    void SerializeTreeView(TreeView treeView, string fileName) {
       	XmlTextWriter textWriter = new XmlTextWriter(fileName, System.Text.Encoding.ASCII);
       	// writing the xml declaration tag
       	textWriter.WriteStartDocument();
       	
		// writing the main tag that encloses all node tags
       	textWriter.WriteStartElement("TreeView");
       
       	// save the nodes, recursive method
       	SaveNodes(treeView.Nodes, textWriter);
       
       	textWriter.WriteEndElement();
         
       	textWriter.Close();
    }

    private void SaveNodes(TreeNodeCollection nodesCollection, XmlTextWriter textWriter) {
       	for (int i = 0; i < nodesCollection.Count; i++) {
            TreeNode node = nodesCollection[i];
            textWriter.WriteStartElement(XmlNodeTag);	// Adds node tag
            textWriter.WriteAttributeString(XmlNodeTextAtt, node.Text);
            textWriter.WriteAttributeString(XmlNodeImageIndexAtt, node.ImageIndex.ToString());
            if(node.Tag != null) textWriter.WriteAttributeString(XmlNodeTagAtt, node.Tag.ToString());
            if (node.Nodes.Count > 0) {
                 SaveNodes(node.Nodes, textWriter);
            }     
            textWriter.WriteEndElement();
       	}
    }

    void DeserializeTreeView(TreeView treeView, string fileName) {
	   	XmlTextReader reader = null;
	   	try {
	        // disabling re-drawing of treeview till all nodes are added
	        treeView.BeginUpdate();    
	        reader = new XmlTextReader(fileName);
	        TreeNode parentNode = null;
	        while (reader.Read()) {
	        	if (reader.NodeType == XmlNodeType.Element) {      
	        		if (reader.Name == XmlNodeTag) {
	          			TreeNode newNode = new TreeNode();
	           			bool isEmptyElement = reader.IsEmptyElement;
	                
	                    // loading node attributes
	                    int attributeCount = reader.AttributeCount;
	                    if (attributeCount > 0) {
	                        for (int i = 0; i < attributeCount; i++) {
	                            reader.MoveToAttribute(i);
	                            SetAttributeValue(newNode, reader.Name, reader.Value);
	                        }        
	                    }
	                    // add new node to Parent Node or TreeView
	                    if(parentNode != null) parentNode.Nodes.Add(newNode);
	                    else treeView.Nodes.Add(newNode);
	                
	                    // making current node 'ParentNode' if its not empty
	                    if (!isEmptyElement) {
	                        parentNode = newNode;
	                    }
	                }                          
	            }
	            // moving up to in TreeView if end tag is encountered
	            else if (reader.NodeType == XmlNodeType.EndElement) {
	                if (reader.Name == XmlNodeTag) {
	                    parentNode = parentNode.Parent;
	        		}
	            } else if (reader.NodeType == XmlNodeType.XmlDeclaration) { 
	                //Ignore Xml Declaration                    
	            } else if (reader.NodeType == XmlNodeType.None) {
	                return;
	            } else if (reader.NodeType == XmlNodeType.Text) {
	                parentNode.Nodes.Add(reader.Value);
	            }
	    
	        }
	   	} finally {
	        // enabling redrawing of treeview after all nodes are added
	        treeView.EndUpdate();      
	        reader.Close(); 
	   	}
    }

    private void SetAttributeValue(TreeNode node, string propertyName, string value) {
     	if (propertyName == XmlNodeTextAtt) {
          	node.Text = value;
     	} else if (propertyName == XmlNodeImageIndexAtt) {
          	node.ImageIndex = int.Parse(value);
     	} else if (propertyName == XmlNodeTagAtt) {
          	node.Tag = value;
     	}
    }

}

// The following class contains the Main function that runs the MusicLib form
class MusicApplication {
    public static void Main() {
        Application.Run(new MusicLib());
    }
}
