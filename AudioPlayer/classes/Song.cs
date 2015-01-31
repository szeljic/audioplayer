using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayer.classes
{
    class Song
    {

        private string name, duration, id;
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = value; }
        }

    }
}
