/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;

namespace UpdateIPTParam
{
    [ComVisible(true)]
    public class SampleAutomation
    {
        private readonly InventorServer inventorApplication;

        public SampleAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            try
            {
                //update parameters in the doc
                ChangeParameters(doc);

                //genetate outputs
                var docDir = System.IO.Path.GetDirectoryName(doc.FullFileName);

                //save output file
                var documentType == doc.DocumentType;
                if (documentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    //the name needs to be in sync with OutputIpt localName in Activity
                    var fileName = System.IO.Path.Combine(docDir, "outputFile.ipt");

                    //save file

                    doc.SaveAs(fileName, false);
                }

            }

            catch (Exception e)
            {
                LogTrace("Processing failed: {0}", e.ToString());
            }

        }

        /// <summary>
        /// Change parameters in Inventor document.
        /// </summary>
        /// <param name="doc">The Inventor document.</param>
        /// <param name="json">JSON with changed parameters.</param>

        public void ChangeParameters(Document doc)
        {
            var theparams = GetParameters(doc);

            Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText("params.json"));
            foreach(KeyValuePair<string, string> entry in parameters)
            {
                try
                {
                    Parameter param = theparams[entry.Key.ToLower()];
                    param.Expression = entry.Value;
                }
                catch(Exception e)
                {
                    LogTrace("Cannot update {0}:{1}", entry.Key, e.Message);
                }
            }
            doc.Update();
        }

        /// <summary>
        /// Get parameters for the document.
        /// </summary>
        /// <returns>Parameters. Throws exception if parameters are not found.</returns>
        /// 
        private static Parameters GetParameters(Document doc)
        {
            var docType = doc.DocumentType;
            switch(docType)
            {
                case DocumentTypeEnum.kAssemblyDocumentObject:
                    var asm = doc as AssemblyDocument;
                    return asm.ComponentDefinition.Parameters;
                case DocumentTypeEnum.kPartDocumentObject:
                    var ipt = doc as PartDocument;
                    return ipt.ComponentDefinition.Parameters;
                default:
                    throw new ApplicationException(string.Format("Unexpected document type ({0})", docType));
            }
        }


        /// <summary>
        /// This will appear on the Design Automation output
        /// </summary>
        /// 

        private static void LogTrace(string format, params object[] args) { Trace.TraceInformation(format, args); }



        public void RunWithArguments(Document doc, NameValueMap map)
        {
            LogTrace("Processing " + doc.FullFileName);

            try
            {
                if (doc.DocumentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    using (new HeartBeat())
                    {
                        // TODO: handle the Inventor part here
                    }
                }
                else if (doc.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject) // Assembly.
                {
                    using (new HeartBeat())
                    {
                        // TODO: handle the Inventor assembly here
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }
        }

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}