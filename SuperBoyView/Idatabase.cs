using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SuperBoyView
{
    interface Idatabase
    {
        /// <summary>
        /// this method is used to query all the data in the database
        /// </summary>
        /// <param name="count"></param>
        DataSet SelectALL(int count);
        /// <summary>
        /// this method is used to query all data with conditions
        /// </summary>
        /// <param name="Where"></param>
        /// <param name="count"></param>
        DataSet SelectALL(string Where, int count);
        /// <summary>
        /// this method is used to modify a data
        /// </summary>
        /// <param name="Where"></param>
        int Update(string Where);
    }
}
