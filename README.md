# Music-Library
Music Library written in C# (using Winforms) that organizes data in a treeview, saved into an XML file. When opened, it is a form that allows the user to perform all CRUD actions, as well as move, copy/cut and paste, and search. If a new node entered is not a song, leave the YouTube URL blank when adding it. If a new node is a song, enter the song's YouTube URL as well as the song's name when adding it. To listen to a song in the library, double-click the song and it's respective YouTube URL will be opened in Google Chrome.

To run this program, first save both the Music_Library.cs and Music_Library_Data.xml files into the same folder. Then, open the Music_Library.cs file and change all paths used in the Serialize and Deserialize functions to the specific path containing the xml file. Finally, open command prompt, change directory to the directory containing the previous two files, and enter the following: c:\Windows\Microsoft.NET\Framework\v3.5\csc.exe -target:winexe Music_Library.cs

The above is to compile the cs file.
A Music_Library.exe file will then be created in the same directory, which will run the program when opened.
