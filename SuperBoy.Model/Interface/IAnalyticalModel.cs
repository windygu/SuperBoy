using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperBoy.Model.Interface
{
    public interface IAnalyticalModel
    {
        string SELECT(Public.DatabseSend database);

        string INSTER(Public.DatabseSend database);

        string UPDATE(Public.DatabseSend database);

        string DELTE(Public.DatabseSend database);

        string ONLINE(Public.DatabseSend database);
        string MANAGE(Public.DatabseSend database);
    }
}