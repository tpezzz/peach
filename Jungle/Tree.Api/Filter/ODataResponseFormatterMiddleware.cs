using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Tree.Api.Filter {
    public class ODataResponseFormatterMiddleware {
        private readonly Func<IDictionary<string, object>, Task> next;

        private static readonly string oDataErrorMessageFieldName = "\"message\":";

        public ODataResponseFormatterMiddleware(Func<IDictionary<string, object>, Task> next) {
            this.next = next;
        }

        private async Task InvokeForOData(IDictionary<string, object> env) {
            // do not use async with streams
            var context = new OwinContext(env);

            using (var readContentStream = new MemoryStream()) {
                var outputStream = context.Response.Body;
                context.Response.Body = readContentStream;

                await next(env);

                readContentStream.Seek(0, SeekOrigin.Begin);

                if (context.Response.StatusCode == 200) {
                    readContentStream.CopyTo(outputStream);
                }
                else {
                    var errorMessage = new StreamReader(readContentStream).ReadToEnd();

                    // add 1 for quotation mark: \"
                    var messageStart = errorMessage.IndexOf(oDataErrorMessageFieldName) + oDataErrorMessageFieldName.Length + 1;
                    var messageEnd = errorMessage.LastIndexOf("\"");
                    var outputMessage = errorMessage.Substring(messageStart, messageEnd - messageStart).Trim();

                    var outputJson = new JavaScriptSerializer().Serialize(new { Message = outputMessage });
                    var outputJsonBytes = Encoding.ASCII.GetBytes(outputJson);

                    context.Response.ContentLength = outputJsonBytes.Length;
                    outputStream.Write(outputJsonBytes, 0, outputJsonBytes.Length);
                }
            }
        }

        public async Task Invoke(IDictionary<string, object> env) {
            var context = new OwinContext(env);

            // filter only odata controllers
            if (!context.Request.Path.Value.StartsWith("/odata")) {
                await this.next(env);
            }
            else {
                await InvokeForOData(env);
            }
        }
    }
}