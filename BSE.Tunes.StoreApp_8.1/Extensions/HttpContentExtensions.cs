using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// Extension methods to allow strongly typed objects to be read from <see cref="HttpContent"/> instances.
    /// </summary>
    public static class HttpContentExtension
    {
        /// <summary>
        /// Returns a <see cref="Task"/> that returns an exception from the <paramref name="content"/> instance.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <returns>A <see cref="Task"/> that returns an exception.</returns>
        public async static Task<Exception> ReadExceptionAsAsync(this HttpContent content)
        {
            Exception exception = null;
            if (content != null)
            {
                var responsestring = await content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responsestring))
                {
                    var exceptionMessages = ParseExceptionMessages(responsestring, null);
                    if (exceptionMessages != null)
                    {
                        exceptionMessages.Reverse();
                        exception = CreateException(exception, exceptionMessages);
                    }
                }
            }
            return exception;
        }

        private static List<object> ParseExceptionMessages(string jsonException, List<object> jsonObjects)
        {
            var json = JObject.Parse(jsonException);
            var messageDefinition = new { Message = "", ExceptionType = "", ExceptionMessage = "", ExceptionSource = "" };
            var exceptionObject = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(jsonException, messageDefinition);
            if (jsonObjects == null)
            {
                jsonObjects = new List<object>();
            }
            jsonObjects.Add(exceptionObject);

            JToken innerExceptionValue;
            if (json.TryGetValue("InnerException", out innerExceptionValue))
            {
                var innerException = innerExceptionValue.ToString();
                return ParseExceptionMessages(innerException, jsonObjects);
            }
            return jsonObjects;
        }

        private static Exception CreateException(Exception exception, List<object> exceptionMessages)
        {
            if (exceptionMessages != null)
            {
                foreach (var message in exceptionMessages)
                {
                    if (message != null)
                    {
                        JObject jobject = JObject.FromObject(message);
                        string exceptionType = null;
                        string exceptionMessage = null;
                        string exceptionSource = null;

                        JToken execptionTypeValue;
                        if (jobject.TryGetValue("ExceptionType", out execptionTypeValue))
                        {
                            exceptionType = execptionTypeValue.ToString();
                        }

                        JToken exceptionMessageValue;
                        if (jobject.TryGetValue("ExceptionMessage", out exceptionMessageValue))
                        {
                            exceptionMessage = exceptionMessageValue.ToString();
                        }

                        JToken exceptionSourceValue;
                        if (jobject.TryGetValue("ExceptionSource", out exceptionSourceValue))
                        {
                            exceptionSource = exceptionSourceValue.ToString();
                        }

                        if (!string.IsNullOrEmpty(exceptionType) && !string.IsNullOrEmpty(exceptionMessage))
                        {
                            Exception tmpException = null;
                            Type type = Type.GetType(exceptionType);
                            if (type != null)
                            {
                                tmpException = (Exception)Activator.CreateInstance(type, new object[] { exceptionMessage, exception });
                            }
                            if (tmpException == null && !string.IsNullOrEmpty(exceptionSource))
                            {
                                System.Reflection.AssemblyName assemblyName = new Reflection.AssemblyName(exceptionSource);
                                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyName);
                                type = assembly.GetType(exceptionType);
                                if (type != null)
                                {
                                    tmpException = (Exception)Activator.CreateInstance(type, new object[] { exceptionMessage, exception });
                                }
                            }
                            if (tmpException != null)
                            {
                                exception = tmpException;
                            }
                        }
                    }
                }
            }
            return exception;
        }
    }
}
