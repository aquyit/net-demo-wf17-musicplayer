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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Models
{
    public class Track
    {
        [XmlIgnore]
        public Uri Source { get; set; }

        [XmlAttribute("Source")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string UriString
        {
            get { return Source == null ? null : Source.ToString(); }
            set { Source = value == null ? null : new Uri(value, (value.IndexOf("http") > -1 ? UriKind.Absolute : UriKind.Relative)); }
        }

        [XmlElement]
        public string Artist { get; set; }

        [XmlElement]
        public string Title { get; set; }

        [XmlElement]
        public string Album { get; set; }

        [XmlIgnore]
        public Uri Tile { get; set; }

        [XmlElement("Tile")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string TileString
        {
            get { return Tile == null ? null : Tile.ToString(); }
            set { Tile = value == null ? null : new Uri(value, (value.IndexOf("http") > -1 ? UriKind.Absolute : UriKind.Relative)); }
        }

        public Track()
        {
        }

        public Track(Uri source, string artist, string title, string album) : this (source, artist, title, album, null)
        {           
        }

        public Track(Uri source, string artist, string title, string album, Uri tile)
        {
            Source = source;
            Artist = artist;
            Title = title;
            Album = album;
            Tile = null;
        }
    }
}
