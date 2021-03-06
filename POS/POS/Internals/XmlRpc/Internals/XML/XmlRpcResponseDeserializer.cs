namespace Rpc.Internals
{
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>Class to deserialize XML data representing a response.</summary>
    internal class XmlRpcResponseDeserializer : XmlRpcDeserializer
    {
        static private XmlRpcResponseDeserializer _singleton;

        /// <summary>A static singleton instance of this deserializer.</summary>
        [Obsolete("This object is now thread safe, just use an instance.", false)]
        static public XmlRpcResponseDeserializer Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new XmlRpcResponseDeserializer();
                }

                return _singleton;
            }
        }

        /// <summary>Static method that parses XML data into a response using the Singleton.</summary>
        /// <param name="xmlData"><c>StreamReader</c> containing an XML-RPC response.</param>
        /// <returns><c>XmlRpcResponse</c> object resulting from the parse.</returns>
        override public Object Deserialize(TextReader xmlData)
        {
            XmlTextReader reader = new XmlTextReader(xmlData);
            XmlRpcResponse response = new XmlRpcResponse();
            bool done = false;
	
            lock (this)
            {
                this.Reset();
	    
                while (!done && reader.Read())
                {
                    this.DeserializeNode(reader); // Parent parse...
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case FAULT:
                                    response.Value = this._value;
                                    response.IsFault = true;
                                    break;
                                case PARAM:
                                    response.Value = this._value;
                                    this._value = null;
                                    this._text = null;
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return response;
        }
    }
}