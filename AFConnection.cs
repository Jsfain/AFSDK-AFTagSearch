using OSIsoft.AF;
using System;
using System.Linq;

namespace AFConnections
{
  class AFConnectionMethods
  {
    // -------------------------------------------------------------------------
    // Searches the list of PISystem (AFServer) objects known to the client host
    // for the AF Server speicfied by the optional 'AFServerStr' param.
    // Returns the PISystem object for the specified AFServer. 
    // 
    // If an 'AFServerStr' is not provided then the default PISystem object on
    // the client host is returned.
    // 
    // If no PISytem object matches the 'AFServerStr' then an 
    // InvalidOperationException is thrown.
    // -------------------------------------------------------------------------
    public static PISystem findAFServer( 
       /* ref PISystem AFServer,*/ 
        string AFServerStr = "")
    {
      PISystems AFServers = new PISystems();
      PISystem AFServer = null;

      // try to connect to AF Server. If unable, then the program exits.
      try
      {
        //
        // if an AF Server has been specified try to connect to it. Else try to 
        // connect to the default AF Server.
        //
        if (!string.Equals(AFServerStr, ""))
        {
          AFServer = AFServers.Single(a => a.Name == AFServerStr);
        }
        else
        {
          Console.Write("\r\n\n AF Server not provided.");
          Console.Write("\r\n Attempting to connect to default AF Server.");
          AFServerStr = "Default AF Server";
          AFServer = AFServers.DefaultPISystem;
          Console.Write("\r\n Default server found: \"{0}\"", AFServer.Name);
        }
      }
      catch/*(InvalidOperationException)*/
      {
        throw;
      }

      return AFServer;
    }
  }
}