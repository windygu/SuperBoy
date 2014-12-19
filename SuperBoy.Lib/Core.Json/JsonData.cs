using System;

namespace Core.Json
{
    public class JsonData
    {
        private string _data;
        private string _type;

        public JsonData(string __data, string __type)
        {
            this._data = __data;
            this._type = __type;
        }

        public string Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }

        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
    }
}
