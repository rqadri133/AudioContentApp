using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AudioContentApp.Models;
using System.Globalization;
using System.IO;
using AudioContentApp.AudioConvertor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace AudioContentApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IAudioConvertor convertor;
        public HomeController(ILogger<HomeController> logger ,   IAudioConvertor _convertor)
        {
            _logger = logger;
            convertor = _convertor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        public IActionResult ConvertAudioToWav(IFormFile file)
        {
            return View();
             

        }


        [HttpPost]
        public async Task<IActionResult> ConvertAudioToWavFile()
        {
            IFormFile file = Request.Form.Files[0];
            FileStream stream = await convertor.ConvertAudioMp3(file);
                       
            return Ok( new { count = stream.Length, Description = "File Uploaded on Azure" });


        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
