using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CustomProvider
{
    /// <summary>
    /// Customer class holds the Database information and contains operation to modify the DB.
    /// </summary>
    public class Customer
    {
        #region Constrcutor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ReplicaCon"></param>
        /// <param name="ReplicaName"></param>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="Designation"></param>
        /// <param name="Age"></param>
        /// <param name="DateModified"></param>
        /// <param name="DateCreated"></param>
        public Customer(string ReplicaCon, string ReplicaName, int ID, string Name, string Designation, int Age, DateTime DateModified, DateTime DateCreated)
        {
            m_ReplicaCon = ReplicaCon;
            m_ReplicaName = ReplicaName;
            m_ID = ID;
            m_Name = Name;
            m_Designation = Designation;
            m_Age = Age;
            m_DateModified = DateModified;
            m_DateCreated = DateCreated;
        }

        #endregion

        #region Local Variables
        int m_ID;
        string m_Name;
        string m_Designation;
        int m_Age;
        string m_ReplicaCon;
        string m_ReplicaName;
        DateTime m_DateModified;
        DateTime m_DateCreated;
        #endregion

        #region Public Properties
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get
            {
                return m_ID;
            }
            set
            {

                m_ID = value;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Designation
        /// </summary>
        public string Designation
        {
            get
            {
                return m_Designation;
            }
            set
            {
                m_Designation = value;
            }
        }

        /// <summary>
        /// Age
        /// </summary>
        public int Age
        {
            get
            {
                return m_Age;
            }
            set
            {
                m_Age = value;
            }
        }

        /// <summary>
        /// DateModified
        /// </summary>
        public DateTime DateModified
        {
            get
            {
                return m_DateModified;
            }
            set
            {
                m_DateModified = value;
            }
        }

        /// <summary>
        /// DateCreated
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return m_DateCreated;
            }
            set
            {
                m_DateCreated = value;
            }
        }

        /// <summary>
        /// ReplicaCon
        /// </summary>
        public string ReplicaCon
        {
            get
            {
                return m_ReplicaCon;
            }
            set
            {
                m_ReplicaCon = value;
            }
        }

        /// <summary>
        /// ReplicaName
        /// </summary>
        public string ReplicaName
        {
            get
            {
                return m_ReplicaName;
            }
            set
            {
                m_ReplicaName = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a Customer in Database
        /// </summary>
        public void Create()
        {
            string insertQuerry = "INSERT INTO dbo.Customer VALUES ({0}, '{1}', '{2}',{3},'{4}','{5}')";
            ExecuteSQL(string.Format(insertQuerry, ID, Name.TrimEnd(), Designation.Trim(), Age, DateModified.ToString(),
                DateCreated.ToString()), ReplicaCon);
        }

        /// <summary>
        /// Updates a Customer in Database 
        /// </summary>
        public void Update()
        {
            string updateQuerry = "UPDATE dbo.Customer SET Name = '{0}',Designation ='{1}',Age = {2},DateModified = '{3}' Where ID = {4}";
            ExecuteSQL(string.Format(updateQuerry, Name.TrimEnd(), Designation.Trim(), Age, DateModified.ToString(), ID), ReplicaCon);
        }

        /// <summary>
        ///  Deletes a Customer in Database 
        /// </summary>
        public void Delete()
        {
            string deleteQuerry = "DELETE FROM dbo.Customer Where ID = {0}";
            ExecuteSQL(string.Format(deleteQuerry, ID), ReplicaCon);
        }

        /// <summary>
        /// Returns the customer specified by ID,ReplicCon and Replica name
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ReplicaCon"></param>
        /// <param name="ReplicaName"></param>
        /// <returns></returns>
        public static Customer GetCustomerById(int ID, string ReplicaCon, string ReplicaName)
        {
            Customer customer = null;
            string sql = "SELECT ID,Name,Designation,Age,DateModified,DateCreated FROM Customer Where ID ={0}";
            sql = string.Format(sql, ID);
            using (SqlConnection con = new SqlConnection(ReplicaCon))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        customer = new Customer(ReplicaCon, ReplicaName, ID, dr["Name"].ToString(), dr["Designation"].ToString(), Int32.Parse(dr["Age"].ToString()), (DateTime)dr["DateModified"], (DateTime)dr["DateCreated"]);
                    }
                }
            }
            return customer;
        }

        public static int GetNextId(string ReplicaCon)
        {
            string sql = "SELECT Max(ID) FROM Customer";
            using (SqlConnection con = new SqlConnection(ReplicaCon))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (dr[0].ToString() != string.Empty)
                        {
                            return (int.Parse(dr[0].ToString())) + 1;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                }
            }
            throw new Exception("Error retreiving ID");
        }

        public static Dictionary<string, DateTime> GetUpdatedCustomersId(string ReplicaCon)
        {
            Dictionary<string, DateTime> customersID = new Dictionary<string, DateTime>();
            string sql = "SELECT ID,DateModified FROM Customer";
            using (SqlConnection con = new SqlConnection(ReplicaCon))
            using (SqlDataAdapter adr = new SqlDataAdapter(sql, con))
            {
                DataSet ds = new DataSet();
                adr.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    customersID.Add(dr[0].ToString(), (DateTime)dr[1]);
                }
            }
            return customersID;
        }

        #endregion

        #region Private helper Methods

        private void ExecuteSQL(string Querry, string ConnectionString)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(Querry, con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

    }
}
