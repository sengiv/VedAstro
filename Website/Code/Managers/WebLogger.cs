﻿using System.Xml.Linq;
using Genso.Astrology.Library;
using Microsoft.JSInterop;

namespace Website
{

    /// <summary>
    /// A specialized log manager for website
    /// </summary>
    public static class WebLogger
    {
        private static readonly XElement SourceXml = new XElement("Source", "WebLogger");


        /// <summary>
        /// Tries to ID the user, and sends a log of the visit to API server
        /// Called from MainLayout everytime page is loaded
        /// Note:
        /// - Does not log any url with localhost
        /// - if fail will exit silently
        /// </summary>
        public static async Task Visitor(IJSRuntime jsRuntime)
        {
            try
            {
                //get all visitor data
                var visitorXml = await GetVisitorDataXml();

                //if running code locally, end here
                //since in local errors will show in console
                //and also not to clog server's error log
#if DEBUG
                Console.WriteLine($"BLZ > LogVisitor > DEBUG > skipped logging > \n{visitorXml}");
                return;
#endif

                //send to server for storage
                await SendLogToServer(visitorXml);
            }
            catch (Exception e)
            {
                //if fail exit silently, not priority
                Console.WriteLine($"BLZ > LogVisitor > Failed! \n{e.Message}\n{e.StackTrace}");
            }

        }


        public static async Task Error(XElement errorDataXml)
        {
            await Error(errorDataXml.ToString());
        }

        /// <summary>
        /// Log error when there is no exception data
        /// used when #blazor-error-ui is shown
        /// </summary>
        public static async Task Error(string errorMsg)
        {

            //if running code locally, end here
            //since in local errors will show in console
            //and also not to clog server's error log
#if DEBUG
            Console.WriteLine("BLZ > LogAlert > DEBUG > skipped logging");
            Console.WriteLine($"PAGE NAME:{await AppData.CurrentUrlJS}\nERROR MESSAGE:{errorMsg}");
            return;
#endif

            //place error data into visitor tag
            //this is done because visitor data might hold clues to error
            var visitorXml = new XElement("Visitor");
            var userId = new XElement("UserId", AppData.CurrentUser.Id);
            var visitorId = new XElement("VisitorId", AppData.VisitorId);
            var urlXml = new XElement("Url", await AppData.CurrentUrlJS);
            var errorXml = new XElement("Error", new XElement("Message", errorMsg));
            visitorXml.Add(Tools.BranchXml, SourceXml, userId, visitorId, errorXml, urlXml, Tools.TimeStampSystemXml);

            //send to server for storage
            await SendLogToServer(visitorXml);

            Console.WriteLine("BLZ > LogAlert > An unexpected error occurred and was logged.");

        }

        /// <summary>
        /// Makes a log of the exception in API server
        /// </summary>
        public static async Task Error(Exception exception, string extraInfo = "")
        {

            //if running code locally, end here
            //since in local errors will show in console
            //and also not to clog server's error log
#if DEBUG
            Console.WriteLine("BLZ > LogAlert > DEBUG > skipped logging");
            Console.WriteLine($"{extraInfo}\n{exception.Message}\n{exception.StackTrace}");
            return;
#endif

            //get all visitor data
            //var visitorXml = await GetVisitorDataXml(jsRuntime);

            //convert exception into nice xml
            var errorXml = Tools.ExtractDataFromException(exception);

            //place error data into visitor tag
            //this is done because visitor data might hold clues to error
            var visitorXml = new XElement("Visitor");
            var userId = new XElement("UserId", AppData.CurrentUser?.Id);
            var visitorId = new XElement("VisitorId", AppData.VisitorId);
            var dataXml = new XElement("Data", extraInfo);
            var urlXml = new XElement("Url", await AppData.CurrentUrlJS);

            visitorXml.Add(Tools.BranchXml, SourceXml, userId, visitorId, errorXml, urlXml, dataXml, Tools.TimeStampSystemXml, Tools.TimeStampServerXml);

            //send to server for storage
            await SendLogToServer(visitorXml);

            Console.WriteLine("BLZ > LogAlert > An unexpected error occurred and was logged.");
        }

        /// <summary>
        /// Logs a button click
        /// </summary>
        public static async Task Click(string? buttonText)
        {
            //if running code locally, end here
            //since in local errors will show in console
            //and also not to clog server's error log
#if DEBUG
            Console.WriteLine($"BLZ > LogAlert > DEBUG > skipped logging > {buttonText}");
            return;
#endif

            await Data($"Button Text:{buttonText}");

        }

        /// <summary>
        /// Logs an alert shown to user
        /// </summary>
        public static async Task Alert(dynamic alertData)
        {
            //if running code locally, end here
            //since in local errors will show in console
            //and also not to clog server's error log
#if DEBUG
            Console.WriteLine($"BLZ > LogAlert > DEBUG > skipped logging > {alertData}");
            return;
#endif

            //all alerts except loading box, visitor list popup (use of html instead of title)
            // only visitor list page & loading box uses "html" and not "title",
            // so if can't get it skip it
            var alertMessage = ((dynamic)alertData)?.title ?? "";
            await Data($"Alert Message:{alertMessage}");
        }

        /// <summary>
        /// Simple method to log general data to API
        /// note: will not run debug
        /// </summary>
        public static async Task Data(string data)
        {
            //if running code locally, end here
            //since in local errors will show in console
            //and also not to clog server's error log
            //todo check need
            //#if DEBUG
            //            Console.WriteLine($"BLZ:LogData:DEBUG mode:skipped logging:{data}");
            //            return;
            //#endif

            //get basic visitor data
            var visitorXml = await GetVisitorDataXml();

            //add in button click data
            visitorXml.Add(Tools.BranchXml, SourceXml, new XElement("Data", data), Tools.TimeStampSystemXml, Tools.TimeStampServerXml);

            //send to server for storage
            await SendLogToServer(visitorXml);
        }




        //█▀█ █▀█ █ █░█ ▄▀█ ▀█▀ █▀▀   █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        //█▀▀ █▀▄ █ ▀▄▀ █▀█ ░█░ ██▄   █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█

        /// <summary>
        /// Gets new visitor xml data for logging
        /// </summary>
        private static async Task<XElement> GetVisitorDataXml()
        {
            //get url user is on
            var urlString = await AppData.JsRuntime.GetCurrentUrl();
            //place url in xml
            var urlXml = new XElement("Url", urlString);
            var userIdXml = new XElement("UserId", AppData.CurrentUser?.Id);


            //based on visitor type create the right record data to log
            //this is done to minimize excessive logging
            var visitorXml = AppData.IsNewVisitor
                ? await NewVisitor(userIdXml, urlXml)
                : OldVisitor(userIdXml, urlXml);
            return visitorXml;
        }



        //all possible details are logged
        private static async Task<XElement> NewVisitor(XElement userIdXml, XElement urlXml)
        {
            //get visitor data & format it nicely for storage
            var browserDataXml = await AppData.JsRuntime.InvokeAsyncJson("getVisitorData", "BrowserData");
            var screenDataXml = await AppData.JsRuntime.InvokeAsyncJson("getScreenData", "ScreenData");
            var originUrlXml = new XElement("OriginUrl", await AppData.OriginUrl);
            var visitorIdXml = new XElement("VisitorId", AppData.VisitorId);
            var locationXml = await GetVisitorLocation();
            var visitorElement = new XElement("Visitor");
            visitorElement.Add(Tools.BranchXml, SourceXml, userIdXml, visitorIdXml, urlXml, locationXml, browserDataXml, screenDataXml, originUrlXml, Tools.TimeStampSystemXml, Tools.TimeStampServerXml);

            //mark new visitor as already logged for first time
            AppData.IsNewVisitor = false;

            return visitorElement;
        }

        /// <summary>
        /// Gets current visitor's geo location from 3rd party API (geojs.io)
        /// </summary>
        /// <returns></returns>
        private static async Task<XElement> GetVisitorLocation()
        {
            XElement locationXml;

            //because call to 3rd party API, call not reliable from all locations on earth
            //known to fail from time to time, as such compensate!
            var resultLocation = await ServerManager.ReadFromServerXmlReply(URL.GeoJsApiUrl, AppData.JsRuntime, "Location");
            if (resultLocation.IsPass)
            {
                locationXml = resultLocation.Payload;
            }
            else
            {
                locationXml = new XElement("Location", "API CALL FAIL");
            }

            return locationXml;
        }

        //only needed details are logged
        private static XElement OldVisitor(XElement userIdXml, XElement urlXml)
        {

            //get visitor data & format it nicely for storage
            var visitorElement = new XElement("Visitor");
            var visitorIdXml = new XElement("VisitorId", AppData.VisitorId); //use id generated above
            visitorElement.Add(Tools.BranchXml, SourceXml, userIdXml, visitorIdXml, urlXml, Tools.TimeStampSystemXml, Tools.TimeStampServerXml);

            return visitorElement;
        }

        /// <summary>
        /// Given the Visitor xml element, it will send it to API for safe keeping via WORKER JS!!
        /// </summary>
        private static async Task SendLogToServer(XElement visitorElement)
        {
            try
            {
                //send using worker JS
                await AppData.JsRuntime.InvokeAsync<string>("window.LogThread.postMessage", visitorElement.ToString());

                //send to API for save keeping
                //note:js runtime passed as null, so no internet checking done
                //var result = await ServerManager.WriteToServerXmlReply(ServerManager.AddVisitorApi, visitorElement, null);

                //check result, display error if needed
                //if (!result.IsPass) { Console.WriteLine($"BLZ: ERROR: Add Visitor Api\n{result.Payload.Value}"); }

            }
            catch (Exception e)
            {
                //not important if fail, keep quiet
                Console.WriteLine("BLZ: SendLogToServer Silent Fail");
            }


        }

    }
}
