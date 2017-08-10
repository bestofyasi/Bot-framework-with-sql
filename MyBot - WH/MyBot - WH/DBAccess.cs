using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MyBot___WH
{
    [Serializable]
    public class DBAccess
    {
        SqlConnection conn;

        public static string itemId=null;
        public static string itemName=null;
        public static string itemQty = null;

        public DBAccess()
        {
            conn = ConnectionManager.GetConnection();
        }

        public bool addItems(string itemName, int itemQty)
        {
            bool status = false;
            if (conn.State.ToString() == "Closed")
            {
                conn.Open();
            }

            SqlCommand newCmd = conn.CreateCommand();
            newCmd.Connection = conn;
            newCmd.CommandType = CommandType.Text;
            newCmd.CommandText = "insert into dbo.items(name, qty)values ('" + itemName + "', '" + itemQty + "')";
            newCmd.ExecuteNonQuery();

            status = true;

            return status;

        }
        public bool selectAll()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("id", typeof(int));
            
            bool status = false;
            if (conn.State.ToString() == "Closed")
                conn.Open();

            SqlCommand newcmd = conn.CreateCommand();
            newcmd.Connection = conn;
            newcmd.CommandType = CommandType.Text;
            newcmd.CommandText = "select * from items";
            SqlDataReader dr = newcmd.ExecuteReader();
           
            while (dr.Read())
            {
                //dt.Rows.Add(dr["ID"], dr["NAME"],dr["QTY"]);
                //id = dr[0] as string;
                //itemName = dr[1] as string;
                //itemName = ("id"+i);
                //break;

                //for (int i = 0; i < dr.FieldCount; i++)
                //{
                //    itemId[i]= dr[0].ToString() + " " + dr[1].ToString() + " " + dr[2].ToString() + " , " + "\r\n";

                //}
                // itemId += dr[0].ToString() + " "+ dr[1].ToString()+" "+ dr[2].ToString()+" , "+"\r\n";
                //itemName += dr[1].ToString() + " | ";
                //itemQty += dr[2].ToString() + " | ";

                itemId += dr[0].ToString() + ") " + dr[1].ToString() + "- " + dr[2].ToString() + " , " + "\r\n";
            }


            status = true;
            return status;
        }
        public bool deleteItems(int itemID)
        {
            bool status = false;
            if (conn.State.ToString() == "Closed")
            {
                conn.Open();
            }

            SqlCommand newCmd = conn.CreateCommand();
            newCmd.Connection = conn;
            newCmd.CommandType = CommandType.Text;
            newCmd.CommandText = "delete from dbo.items where id= " + itemID;
            newCmd.ExecuteNonQuery();

            status = true;

            return status;

        }
    }
}