﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YashLib.VideoServices
{
    public interface IVideo
    {
        Task<byte[]> GetAudioBytesAsync(string url);
    }
}
