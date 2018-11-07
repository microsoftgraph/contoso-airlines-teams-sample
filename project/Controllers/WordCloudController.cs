/* 
 *  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
 *  See LICENSE in the source repository root for complete license information. 
 */

using System.Threading.Tasks;
using System.Web.Mvc;
using Resources;
using System;
using System.Net.Http;

namespace ContosoAirlines.Controllers
{

    public class WordCloudController : Controller
    {
        public ActionResult Index()
        {
            return View("Graph");
        }

        public async Task<ActionResult> CreateWordCloud()
        {
                return View("Graph");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}