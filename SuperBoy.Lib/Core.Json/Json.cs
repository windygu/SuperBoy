
namespace Core.Json
{
    public class Json
    {
        private JsonData _data;
        private int _error = 0;

        public Json(string val, string type)
        {
            this._data = new JsonData(val, type);
        }

        public JsonData Data
        {
            get
            {
                return this._data;
            }
        }

        public int Error
        {
            get
            {
                return this._error;
            }
            set
            {
                this._error = value;
            }
        }
    }
}
