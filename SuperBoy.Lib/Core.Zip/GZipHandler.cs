using System;
using System.Web;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace Core.Zip
{
    public class GZipHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var Context = HttpContext.Current;
            var request = Context.Request;
            var response = Context.Response;

            var acceptEncoding = request.Headers["Accept-Encoding"];
            // *** Start by checking whether GZip is supported by client

            var useGZip = false;

            if (!string.IsNullOrEmpty(acceptEncoding) &&

                acceptEncoding.ToLower().IndexOf("gzip") > -1)

                useGZip = true;

            // *** Create a cachekey and check whether it exists

            var cacheKey = request.QueryString.ToString() + useGZip.ToString();

            var output = Context.Cache[cacheKey] as byte[];

            if (output != null)
            {
                // *** Yup - read cache and send to client
                SendOutput(output, useGZip);
                return;
            }

            // *** Load the script file 

            var script = "";


            var sr = new StreamReader(context.Server.MapPath(request["src"]));

            script = sr.ReadToEnd();


            // *** Now we're ready to create out output

            // *** Don't GZip unless at least 8k

            if (useGZip && script.Length > 6000)

                output = GZipMemory(script);

            else
            {

                output = Encoding.ASCII.GetBytes(script);

                useGZip = false;

            }
            // *** Add into the cache with one day

            Context.Cache.Add(cacheKey, output, null, DateTime.UtcNow.AddDays(1), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
            // *** Write out to Response object with appropriate Client Cache settings

            this.SendOutput(output, useGZip);
        }
        /// <summary>
        /// Sends the output to the client using appropriate cache settings.
        /// Content should be already encoded and ready to be sent as binary.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="useGZip"></param>
        private void SendOutput(byte[] output, bool useGZip)
        {

            var response = HttpContext.Current.Response;
            response.ContentType = "application/x-javascript";
            if (useGZip)
                response.AppendHeader("Content-Encoding", "gzip");
            //if (!HttpContext.Current.IsDebuggingEnabled)
            // {

            response.ExpiresAbsolute = DateTime.UtcNow.AddYears(1);
            response.Cache.SetLastModified(DateTime.UtcNow);
            response.Cache.SetCacheability(HttpCacheability.Public);
            // }

            response.BinaryWrite(output);
            response.End();

        }


        /// <summary>
        /// Takes a binary input buffer and GZip encodes the input
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>

        public static byte[] GZipMemory(byte[] buffer)
        {

            var ms = new MemoryStream();
            var gZip = new GZipStream(ms, CompressionMode.Compress);
            gZip.Write(buffer, 0, buffer.Length);
            gZip.Close();
            var result = ms.ToArray();
            ms.Close();
            return result;

        }
       
        public static byte[] GZipMemory(string input)
        {
            return GZipMemory(Encoding.ASCII.GetBytes(input));

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}