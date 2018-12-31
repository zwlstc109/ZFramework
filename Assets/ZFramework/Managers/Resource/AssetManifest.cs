using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

namespace Zframework
{   
    /// <summary>
    /// 打AB包时，生成的资源清单(xml 二进制各一份)
    /// </summary>
    [System.Serializable]
    public class AssetManifest
    {
        [XmlElement]
       public List<AssetElement> AssetLst { get; set; }
    }
    [System.Serializable]
    public class AssetElement
    {
        [XmlAttribute]
        public string Path { get; set; }
        //[XmlAttribute]
        //public uint Crc { get; set; }
        [XmlAttribute]
        public string ABName { get; set; }
        [XmlAttribute]
        public string AssetName { get; set; }
        [XmlElement]
        public List<string> DependAB { get; set; }
    }
}