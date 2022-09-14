using OSIsoft.AF;
using AFObjectSearch;
using AFInputParams;
using AFConnections;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AFTagSearch
{
  class Program
  {
    static void Main(string[] args)
    { 
      // parse cmd line args and assign to InputParams instance properties
      List<string> invalidArgsList = new List<string>();
      InputParams ip = new InputParams(in args, ref invalidArgsList);

      // print any invalid cmd line arguments
      foreach (string invalidArg in invalidArgsList)
      {
        Console.Write("\r\n Invalid argument \"{0}\"", invalidArg);
      }
    
      // exit program if input tag list file was not provided
      if (string.Equals(ip.InputFileStr, ""))
      {
        Console.Write("\r\n >> No input tag list file has been specified.");
        Console.Write("  Input tag list file must be provided."); 
        Console.Write("\r\n >> Exiting.\r\n");
        System.Environment.Exit(1);
      }

      // print the cmd line args after assigned to verify they are correct
      printParameters(in ip);

      //
      // read input file for tag names and store in string array. If there is an
      // error reading file contents then program will exit in the function.
      //
      string[] tagNamesList;
      readInTagListFile(ip.InputFileStr, out tagNamesList);

/*
      //
      // load AF Server. An error here will result in the program terminating.
      //
      try
      {
        PISystem myAFServer = AFConnectionMethods.findAFServer(ip.AFServerStr);
      }
      catch(InvalidOperationException e)
      {
        Console.Write("\r\n\n An InvalidOperationException occurred while ");
        Console.Write("attempting to load the PISystem (AFServer) object ");
        Console.Write("with the following message: \"{0}\"", e.Message);

        Console.Write("\r\n\n Verify the name of the AF Server ");
        Console.Write("\"{0}\" is correct ", ip.AFServerStr);
        Console.Write("and available on this machine.");

        Console.Write("\r\n\n ------ TRACE -----");
        Console.Write("\r\n {0}.", e);
        Console.Write("\r\n -----------------");

        Console.Write("\r\n\n Exiting.\r\n");
        System.Environment.Exit(1);
      }
      catch
      {
        Console.Write("\r\n\n Unknown error while trying to load AF Server.");
        Console.Write("\r\n Exiting.");
        System.Environment.Exit(1);
      }
*/
/*
      // search for tags from list file within the specified AF Server / DBs
      AFSearchMethods.searchElementAttributesForTagsInList(
        tagNameList, myAFServer, ip.AFDatabaseStr, ip.OutputFileStr,
        ip.ErrorFileStr, ip.WriteErrorFlag, ip.WriteConsoleFlag);
*/
    }



    // -------------------------------------------------------------------------
    // Prints arguments mapped to the available input parameters for the user 
    // to verify the command line parameters have been assigned as intended. 
    // -------------------------------------------------------------------------
    private static void printParameters(in InputParams ip)
    {
      Console.Write("\r\n\n ------------------------------------------");
      Console.Write("\r\n The following parameters have been set");
      
      // AF Server
      Console.Write("\r\n\n AF SERVER:               ");
      if (string.Equals(ip.AFServerStr, "")) { Console.Write("Default"); }
      else { Console.Write(ip.AFServerStr); }
      
      // AF Database
      Console.Write("\r\n AF DATABASE:             ");
      if (string.Equals(ip.AFDatabaseStr, "")) { Console.Write("ALL AF DBs"); }
      else { Console.Write(ip.AFDatabaseStr); }

      // Rest of input params
      Console.Write("\r\n TAG LIST FILE:           {0}", ip.InputFileStr);
      Console.Write("\r\n OUTPUT FILE:             {0}", ip.OutputFileStr);
      Console.Write("\r\n WRITE OUTPUT TO CONSOLE: {0}", ip.WriteConsoleFlag);
      Console.Write("\r\n ATTRIBUTE ERROR FILE:    {0}", ip.ErrorFileStr);
      Console.Write("\r\n WRITE ERRORS TO FILE:    {0}", ip.WriteErrorFlag);
      Console.Write("\r\n ------------------------------------------");
    }

    // -------------------------------------------------------------------------
    // Reads in all lines of the 'inputFile' into the referenced string array
    // 'tagNamesList'. Program exits if input file cannot be accessed.
    // -------------------------------------------------------------------------
    public static void readInTagListFile(
        string inputFileStr, 
        out string[] tagNamesList)
    {
      tagNamesList = null;             // out param must have value assigned

      // verify input file exists. If YES, read contents into array, else exit.
      if (File.Exists(inputFileStr))
      {
        tagNamesList = File.ReadAllLines(inputFileStr);
      }
      else
      {
        Console.Write("\r\n\n >> Error reading input file.");
        Console.Write("\r\n >> Verify the file \"{0}\" exists", inputFileStr);
        Console.Write("\r\n >> Exiting.\r\n");
        System.Environment.Exit(1);
      }
    }    
  }
}
