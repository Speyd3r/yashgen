﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    /// <summary>
    /// youtube-dl handler
    /// </summary>
    class YoutubeDl
    {
        readonly static string OUTPUT_FOLDER = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "yashgen");

        static string errors = "";

        /// <summary>
        /// Downloads an audio track from a YouTube video as m4a.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static string CallYoutubeDl(string videoId)
        {
            errors = "";
            string tempFile = Path.Combine(OUTPUT_FOLDER, videoId + ".m4a");
            ProcessStartInfo info = new ProcessStartInfo(
                 "youtube-dl.exe", "--ignore-config -f bestaudio -x --audio-format m4a --add-metadata -o " +
                 OUTPUT_FOLDER + "/%(id)s.%(ext)s " + videoId
                );
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = info;
            //process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Dispose();
            if(errors != "")
            {
                throw new YoutubeDlException(errors);
            }
            return tempFile;
        }

        /*private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {

        }*/

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Trim() != "")
                {
                    errors += e.Data;
                }
            }
        }

    }


}
