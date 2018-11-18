﻿namespace DevToolsConnector
{
    public interface IDevMessageSerializer
    {
        T DeserializeObject<T>(string pMessage);
        string SerializeObject(object pData);
    }
}
