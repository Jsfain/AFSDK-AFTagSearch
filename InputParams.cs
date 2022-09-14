/*******************************************************************************
*                                Class: InputParams (v1)
*
* DESCRIPTION: 
* InputParams class is used for organizing/collecting commonly used params 
* needed for accessing an Asset Framework Server/Database utilizing the AFSDK.
*
* HOW TO USE:
* There are three constructors included. The primary constructor
* is meant to take in arguments passed from the console and assign these to 
* their appropriate class properties. For this constructor, the following 
* arguments are accepted.  
* 
* /AF : (string) specifying an AF Server
* /DB : (string) specifying an AF Database
* /IF : (string) specifying the name of an Input File
* /OF : (string) specifying the name of an Output File
* /EF : (string) specifying the name of an Error File
* /WE : (Bool) Write Error Flag. If present can be used to indicate to a 
*       calling program to write errors to a file and/or the console.
* /WC : (Bool) Write Console Flag. If provided, can be used to tell calling
*       program to write output to the console.
*
* EXAMPLE format from console: 
*     <PROGRAM>.exe /AF "<AFServer>" /DB "<AFDatabase>" /WE /WC
*
*
* Additional constructors include a default constructor and constructor that
* explicitely sets the class properties to the constructor's params passed in.
*******************************************************************************/

using System.Collections.Generic;  // need for List<T>

namespace AFInputParams
{
  public class InputParams
  {
    // 
    // CONSTRUCTORS
    //

    // -------------------------------------------------------------------------
    // Default: Set class properties to default file names and empty strings.
    // -------------------------------------------------------------------------
    public InputParams()
    {
      AFServerStr = string.Empty;
      AFDatabaseStr = string.Empty;
      InputFileStr = string.Empty;
      OutputFileStr = "Output.txt";
      ErrorFileStr = "Errors.txt";
      WriteErrorFlag = false;
      WriteConsoleFlag = false;
    }

    // -------------------------------------------------------------------------
    // Explicitly set class instance properties to param vals passed in.
    // -------------------------------------------------------------------------
    public InputParams(
        string AFServerStr, 
        string AFDatabaseStr, 
        string InputFileStr, 
        string OutputFileStr,
        string ErrorFileStr,
        bool WriteErrorFlag, 
        bool WriteConsoleFlag)
    {
      this.AFServerStr = AFServerStr;
      this.AFDatabaseStr = AFDatabaseStr;
      this.InputFileStr = InputFileStr;
      this.OutputFileStr = OutputFileStr;
      this.ErrorFileStr = ErrorFileStr;
      this.WriteErrorFlag = WriteErrorFlag;
      this.WriteConsoleFlag = WriteConsoleFlag;
    }

    // -------------------------------------------------------------------------
    // This constructor first sets the properties of the class to the default
    // constructor values then updates any properties of class instance passed 
    // in via the string array by parsing arguments in the string array.
    // This is primarily meant for passing in arguments from the console when 
    // executing a using program. This constructor also collects a list of 
    // invalid arguemts that may have been provided, which the calling program 
    // can use, e.g. handle issues with incorrect argumnets or print to 
    // screen/file for informational purposes.
    // -------------------------------------------------------------------------
    public InputParams(
        in string[] args, 
        ref List<string> invalid) : this()
    {     
      // loop to assign additional InputParam property vals
      for (int ndx = 0; ndx < args.Length; ++ndx)
      {
        switch (args[ndx])
        {
          case "/AF":
            this.AFServerStr = args[++ndx];
            break;
          case "/DB":
            this.AFDatabaseStr = args[++ndx];
            break;
          case "/IF":
            this.InputFileStr = args[++ndx];
            break;
          case "/OF":
            this.OutputFileStr = args[++ndx];
            break;
          case "/EF":
            this.ErrorFileStr = args[++ndx];
            break;
          case "/WE":
            this.WriteErrorFlag = true;
            break;
          case "/WC":
            this.WriteConsoleFlag = true;
            break;
          default:
            invalid.Add(args[ndx]);
            break;
        }
      }
    }

    //
    // GETTERS / SETTERS
    //
    
    public string AFServerStr { get; set; }
    public string AFDatabaseStr { get; set; }
    public string InputFileStr { get; set; }
    public string OutputFileStr { get; set; }
    public string ErrorFileStr { get; set; }
    public bool WriteErrorFlag { get; set; }
    public bool WriteConsoleFlag { get; set; }
  }
}