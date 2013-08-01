using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// Load save is used for general serialization serializes xml files
/// remember the file type
/// the only known weakness is that it cannot properly deserialize objects with 
/// generic collections with objects added to the collection in the constructor
/// ex in constructor
/// list = new List<T>();fine
/// list.Add(S);bad dont add objects to generic collections in the constructor if you want them to be serializable
/// </summary>
static public class LoadSave <T>
{

	   static public void SerializeToXML(T mll, string filename)
       {
			
           XmlSerializer serializer = new XmlSerializer(typeof(T));
           TextWriter textWriter = new StreamWriter(filename);
           serializer.Serialize(textWriter, mll);
           textWriter.Close();
		
       }
       static public T DeserializeToSC(String filename)
        {
            T mxb;// needed to store data after we close the filestream
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            TextReader textReader = new StreamReader(filename);
            mxb = (T)deserializer.Deserialize(textReader);
            textReader.Close();
            return mxb;
        }
	static public bool FileExists(String filename)
	{
		return File.Exists(filename);
	}

}
