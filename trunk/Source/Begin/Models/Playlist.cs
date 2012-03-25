// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    public class Playlist
    {
        [XmlElement]
        public List<Track> Tracks { get; set; }

        [XmlElement]
        public string Name { get; set; }

        public Playlist()
        {
            Tracks = new List<Track>();
            Name = "[UNNAMED]";
        }

        public Playlist(string name)
            : this()
        {
            Name = name;
        }

        public Playlist(List<Track> tracks)
            : this()
        {
            Tracks = tracks;
        }

        public Playlist(string name, List<Track> tracks)
            : this(name)
        {
            Tracks = tracks;
        }

        public Playlist(Track[] tracks)
            : this()
        {
            foreach (var track in tracks)
            {
                Tracks.Add(track);
            }
        }

        public Playlist(string name, Track[] tracks)
            : this(tracks)
        {
            Name = name;
        }
    }
}
