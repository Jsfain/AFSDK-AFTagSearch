using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using OSIsoft.AF.Analysis;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace AFObjectSearch
{
  class AFSearchMethods
  {
    public static void 
    searchElementAttributesForTagsInList(
        string[] tagList,
        PISystem afServer = null,
        string afDatabase = "",
        string oFileName = "Output.txt",
        string attrErrorFileName = "Errors.txt", 
        bool writeErrorsFlag = false, 
        bool writeConsoleFlag = false)
    {

      // write csv column headings to the output file.
      try
      {
        StreamWriter oFile = new StreamWriter(oFileName, true);
        oFile.Write("PI Point, Attribute, Attribute Path, Analysis,");
        oFile.Write("AF Server, AF Database, PI Server");
        oFile.Close();
      }
      catch(DirectoryNotFoundException)
      {
        Console.Write("\r\n Output file path does not exist.");
        Console.Write("\r\n Exiting\n");
        System.Environment.Exit(1);
      }

      //
      // write csv column headings to attribute errors file only if the 
      // writeErrorsFlag is set.
      //
      if (writeErrorsFlag)
      {
        try
        {
          StreamWriter attrErrorFile = new StreamWriter(attrErrorFileName, true);
          attrErrorFile.Write("Exception Type, Attribute,");
          attrErrorFile.Write("Attribute Path, AF Database\n");
          attrErrorFile.Close();
        }
        catch(DirectoryNotFoundException)
        {
          Console.Write("\r\n Output file path does not exist.");
          Console.Write("\r\n Exiting\n");
          System.Environment.Exit(1);
        }
      }    

      //
      // performe tag search in the specified AF Database. If no db has been
      // specified then search for tags in all AF Databases on the specified
      // AF Server. 
      //
      try
      {
        if (!string.Equals(afDatabase, ""))
        {
          // Search for specified AF Database on the AF Server
          AFDatabase db = afServer.Databases.Single(
                                            a => a.Name == afDatabase);
          searchDatabase(db, tagList, oFileName, attrErrorFileName, 
                         writeErrorsFlag, writeConsoleFlag);
        }
        else
        {
          Console.Write("\r\n No AF Database has been specified.");
          Console.Write(" Searching all AF Databases\n");
          foreach (AFDatabase db in afServer.Databases)
          {
            searchDatabase(db, tagList, oFileName, attrErrorFileName,
                           writeErrorsFlag, writeConsoleFlag);
          }
        }
      }
      catch (InvalidOperationException err)
      {
        Console.Write(err);
        Console.Write("\r\n\n >>>> Verify the AF Database");
        Console.Write("\"{0}\" is correct.", afDatabase);
        Console.Write("\r\n >>>> Exiting Program\n");
        System.Environment.Exit(1);
      }
      catch (CommunicationException err)
      {
        Console.Write(err);
        Console.Write("\r\n\n >>>> Could not connect to AF Server {0}.\n",
                     afServer.Name);
        Console.Write("\r\n >>>> Exiting Program\n");
        System.Environment.Exit(1);
      }
    }
  
    //
    // iterates through each of the top-level elements in the database and
    // and passes each to 'searchAllElements' method which searches the entire
    // element hierarchy under the element passed to it.
    //
    private static void searchDatabase(
          AFDatabase db, 
          string[] tagList, 
          string oFile, 
          string errFile, 
          bool writeErr, 
          bool writeCons)
    {
      Console.Write("\r\n\n >>> Searching AF Database: {0}", db);
      
      // iterate through each of the top-level elements in the database
      foreach (AFElement elem in db.Elements)
      {
        searchAllElements(elem, tagList, oFile, errFile, writeErr, writeCons);
      }
    }
    
    // recursively search each element of the database
    private static void searchAllElements(
          AFElement elem, 
          string[] tagList, 
          string outputFile, 
          string attrErrorFile, 
          bool writeErrs, 
          bool writeCons)
    {
      //
      // if the element passed in has a child element then recursively call
      // this function with each of the child elements.
      //
      if (elem.HasChildren)
      {
        foreach (AFElement childElem in elem.Elements)
        {
          searchAllElements(childElem, tagList, outputFile, attrErrorFile, 
                            writeErrs, writeCons);
        }
      }
      
      // keyed list of AF Analysis Output Attributes to their Analysis. 
      AFKeyedResults<AFAttribute, AFAnalysis> analysisOutputAttributeList = 
                           AFAnalysisRule.GetOutputsForAnalyses(elem.Analyses);
      
      if (writeCons)
      {
        Console.Write("\r\n\tElement \"{0}\" at path \"{1}\"",
                          elem.Name, elem.GetPath());
      }

      //
      // Search each attribute under the element. If the attribute is an 
      // Analysis Output attribute, then the name of the analysis is also
      // passed to the attribute search method, otherwise "NULL" is passed.
      //
      foreach (AFAttribute attr in elem.Attributes)
      {
        string analysisSource = "NULL";
        if (analysisOutputAttributeList[attr] != null)
        {
          analysisSource = analysisOutputAttributeList[attr].Name;
        }

        searchAllAttributes(elem, attr, tagList, outputFile, attrErrorFile,
                             writeErrs, writeCons, analysisSource);
      }
    }

    //
    // check each attribute under the element passed in. If an attribute 
    // has children, then this function is called recursively with the child
    // attribute.
    //
    private static void searchAllAttributes(
          AFElement elem, 
          AFAttribute attr, 
          string[] tagList, 
          string outputFile, 
          string attrErrorFile, 
          bool writeErrs, 
          bool writeCons, 
          string analysisSource)
    {

      //
      // if attribute passed in has children then recursively call method with
      // with each of the child attributes.
      //
      if (attr.HasChildren)
      {
        foreach (AFAttribute childAttr in attr.Attributes)
        {
          searchAllAttributes(elem, childAttr, tagList, 
                              outputFile, attrErrorFile, writeErrs, 
                              writeCons, analysisSource);
        }
      }

      //
      // Write to output file if PPDR attribute references a tag specified in 
      // the tag list. If an error is encountered with a PPDR attribute, then
      // the error will be written to the error file if writeErrs flas is set.
      //
      try
      {
        if (attr.PIPoint != null)
        {
          if (writeCons)
          {
            Console.Write("\r\n\t\t PPDR attribute \"{0}\"", attr.Name);
          }
          foreach (string tag in tagList)
          {
            if (string.Equals(tag, attr.PIPoint.Name, 
                              StringComparison.OrdinalIgnoreCase))
            {
              StreamWriter of = new StreamWriter(outputFile, true);
              of.Write("\r\n{0}, {1}, {2}, {3}, {4}, {5}, {6}", 
                           attr.PIPoint.Name, 
                           attr.Name, 
                           attr.GetPath(), 
                           analysisSource, 
                           attr.PISystem, 
                           attr.Database, 
                           attr.PIPoint.Server);
              of.Close();
            }
          }
        }
      }

      // write errors to attribute error file if writeErrs flas is set.
      catch (PIPointInvalidException)
      {
        if (writeErrs)
        {
          StreamWriter attrErrorOF = new StreamWriter(attrErrorFile, true);
          attrErrorOF.Write("\r\nPIPointInvalidException, {0}, {1}, {2}", 
                                attr.Name, attr.GetPath(), 
                                attr.Element.Database.Name);
          attrErrorOF.Close();
        }
      }
      catch
      {
        if (writeErrs)
        {
          StreamWriter attrErrorOF = new StreamWriter(attrErrorFile, true);
          attrErrorOF.Write("\r\nUnhandled, {0}, {1}, {2}", 
                                  attr.Name, attr.GetPath(),
                                  attr.Element.Database.Name);
          attrErrorOF.Close();
        }
      }
    }
  } 
  // END AFSearchMethods class
} 
// END AFObjectSearch namespace

