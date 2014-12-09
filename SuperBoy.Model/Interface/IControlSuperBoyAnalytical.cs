using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperBoy.Model.Interface
{
    public interface IControlSuperBoyAnalytical
    {
        object SELECT(Model.Public.DatabseSend database);

        string INSTER(Model.Public.DatabseSend database);

        string UPDATE(Model.Public.DatabseSend database);

        string DELTE(Model.Public.DatabseSend database);

    }
}