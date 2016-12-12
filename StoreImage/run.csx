/*
    Simple (naive) multiple images upload 
    upload directly through the Function (API) to blob
    optimal way is to upload multiple iamges directly to Blob using Functions just to get SAS token
*/

#r "Microsoft.WindowsAzure.Storage"

using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;

//
//Using CloudBlobContainer to upload multiple images vs. single blob
public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log, Stream outputBlob) 
{ 
    log.Info($"Webhook was triggered!");  

    HttpResponseMessage result = null; 
    
    if (req.Content.IsMimeMultipartContent()) 
    {
            // memory stream of the incomping request 
            var streamProvider = new MultipartMemoryStreamProvider (); 

            log.Info($" ***\t reading input data into stream...");
            req.Content.ReadAsMultipartAsync(streamProvider); 
            log.Info($" ***\t after await on ReadMultpart...");
            
            foreach (HttpContent ctnt in streamProvider.Contents)
            {
               // You would get hold of the inner memory stream here
                Stream stream = ctnt.ReadAsStreamAsync().Result;
                log.Info($"MY UPDATE - Stream length = {stream.Length}"); // just to verify
                if (stream.Length > 1)
                {
                
                    // save the stream to output blob, which will save it to Azure stroage blob
                    stream.CopyTo(outputBlob);
                    result = req.CreateResponse(HttpStatusCode.OK, "great ");
                }    
            }

            result = req.CreateResponse(HttpStatusCode.OK, $"Successfully uploaded images ");             
        }
        else
        {
            log.Info($" ***\t ERROR!!! bad format request ");
            result = req.CreateResponse(HttpStatusCode.NotAcceptable,"This request is not properly formatted");
        }

    return result;
}