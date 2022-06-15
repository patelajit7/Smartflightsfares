using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.HelpingModel.BookingEntities;
using Infrastructure.HelpingModel.Operations;

namespace Database
{
   public class OptProcedure
    {
        public static DocuSignsVM GetDocuSign(int _id, int _filterType,int _docuSignVM, string _connectionString)
        {
            DocuSignsVM docuSigns = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlDataAdapter sqlDataAdapter;
                    DataSet ds = new DataSet();
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "uspGetDocuSigns";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Id", _id);
                        command.Parameters.AddWithValue("@filterType", _filterType);
                        command.Parameters.AddWithValue("@docuSignVM", _docuSignVM);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        docuSigns = new DocuSignsVM();
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            DocuSigns response = null;
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                            response = new DocuSigns();
                            response.Id = Convert.ToInt32(row["Id"]);
                            response.BookingId = Convert.ToInt32(row["BookingId"]);
                            response.CardId = Convert.ToInt32(row["CardId"]);
                            response.AgentId = Convert.ToInt32(row["AgentId"]);
                            response.PortalId = Convert.ToInt32(row["PortalId"]);
                            response.AcceptedAmount = Convert.ToDouble(row["AcceptedAmount"]);
                            response.BookingAmount = Convert.ToDouble(row["BookingAmount"]);
                            response.CardNumber = row["CardNumber"].ToString();
                                response.EmailId = row["EmailId"].ToString();
                                response.CCHolderName = row["CCHolderName"].ToString();
                            response.IP = row["IP"].ToString();
                            if (row["IsAccepted"] != DBNull.Value)
                            {
                                response.IsAccepted = Convert.ToBoolean(row["IsAccepted"]);
                            }
                            if (row["AcceptedDate"] != DBNull.Value)
                            {
                                response.AcceptedDate = Convert.ToDateTime(row["AcceptedDate"]);
                            }
                            response.Created =  Convert.ToDateTime(row["Created"]);
                            response.IsActive = Convert.ToBoolean(row["IsActive"]);
                                response.Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : 0;
                                response.EnvelopeId = row["EnvelopeId"] != DBNull.Value ? Convert.ToString(row["EnvelopeId"]) : "-";
                                response.SendIP = row["SendIP"] != DBNull.Value ? Convert.ToString(row["SendIP"]) : "-";
                                docuSigns.DocuSigns = response;
                            }
                        }
                        if (ds != null && ds.Tables.Count > 1)
                        {
                            DocuSignsDetails response = null;
                            docuSigns.DocuSignsDetails = new List<DocuSignsDetails>();
                            foreach (DataRow row in ds.Tables[1].Rows)
                            {
                                response = new DocuSignsDetails();
                                response.Id = Convert.ToInt32(row["Id"]);
                                response.DocuSingsId = Convert.ToInt32(row["DocuSingsId"]);
                                response.Status = Convert.ToInt32(row["Status"]);
                                response.IP = row["IP"].ToString();
                                response.Created = Convert.ToDateTime(row["Created"]);
                                docuSigns.DocuSignsDetails.Add(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return docuSigns;
        }

        public static bool DocuSignsAccepted(int _id, int _status, string _ip, string _connectionString)
        {
            bool isAuth = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    DataSet ds = new DataSet();
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "DocuSignsAccepted";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Id", _id);
                        command.Parameters.AddWithValue("@Status", _status);
                        command.Parameters.AddWithValue("@IP", _ip);
                        command.Parameters.Add("@U_ID", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                         command.ExecuteNonQuery();
                        int tid = (int)command.Parameters["@U_ID"].Value;
                        if (tid > 0)
                        {
                            isAuth = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isAuth;
        }

        public static void SaveContactSegmentDatails(Dictionary<string, DataTable> contactSegmentTbl, string _connectionString)
        {            
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    DataSet ds = new DataSet();
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "uspFlightContactDataInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@tblContacts", contactSegmentTbl["tblContacts"]);
                        command.Parameters.AddWithValue("@tblSegments", contactSegmentTbl["tblSegments"]);
                        command.ExecuteNonQuery();  
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveOfferContactSegmentDatails(Dictionary<string, DataTable> contactSegmentTbl, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    DataSet ds = new DataSet();
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "uspFlightOfferContactDataInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@tblContacts", contactSegmentTbl["tblContacts"]);
                        command.Parameters.AddWithValue("@tblSegments", contactSegmentTbl["tblSegments"]);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveBagInsurance(float _totalPrice, string _serviceNumber, bool _status, string _statusCode, string _errors, int _bookingId, int _productId,string ProductName, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "BaggageInsurancesInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@BookingId", _bookingId);
                        command.Parameters.AddWithValue("@TotalPrice", _totalPrice);
                        command.Parameters.AddWithValue("@ServiceNumber", _serviceNumber);
                        command.Parameters.AddWithValue("@ProductId", _productId);
                        command.Parameters.AddWithValue("@ProductName", ProductName);
                        command.Parameters.AddWithValue("@Status", _status);
                        command.Parameters.AddWithValue("@StatusCode", _statusCode);
                        command.Parameters.AddWithValue("@ErrorMessage", _errors);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveTravelInsurance(TravelInsurance _travelInsurance, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "TravelInsurancesInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@BookingId", _travelInsurance.BookingId);
                        command.Parameters.AddWithValue("@PolicyNumber", _travelInsurance.PolicyNumber);
                        command.Parameters.AddWithValue("@RefNumber", _travelInsurance.RefNumber);
                        command.Parameters.AddWithValue("@GroupNumber", _travelInsurance.GroupNumber);
                        command.Parameters.AddWithValue("@TotalPrice", _travelInsurance.TotalPrice);
                        command.Parameters.AddWithValue("@Warnings", !string.IsNullOrEmpty(_travelInsurance.Warnings)? _travelInsurance.Warnings: "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
