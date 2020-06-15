using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioContentApp.AudioConvertor
{
    public interface IAudioConvertor
    {
       public Task<FileStream> ConvertAudioMp3(IFormFile file);

    }
}
